using DatasetNormalizer.Csv;
using DatasetNormalizer.Rules;
using Mono.Options;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DatasetNormalizer
{
    internal class Program
    {
        #region Config

        private const char DEFAULT_QUOTES = '"';
        private const char DEFAULT_DELIMITER = ',';

        private static string? inputFilePath;
        private static string? outputFilePath;

        private static char quotes = DEFAULT_QUOTES;
        private static char delimiter = DEFAULT_DELIMITER;
        private static char headerDelimiter = DEFAULT_DELIMITER;

        private static int columnCount;
        private static int? limitLength;

        private static bool showHelp;
        private static bool removeHeader;
        private static bool addHeader;
        private static bool longHeader;
        private static bool floorDoubles;
        private static bool convertDoublesToIntegers;
        private static bool convertNullsToZeros;
        private static bool convertStringsToNumbers;
        private static bool removeLastChar;
        private static bool ignoreFirstRow;
        private static bool ignoreLastEmptyColumn;

        private static readonly List<int> removeRowList = new();
        private static readonly List<int> removeColumnList = new();
        private static readonly List<string> removeList = new();
        private static readonly List<(string, string)> replaceList = new();

        private static readonly OptionSet options = new()
        {
            { "h|help", "show help", t => showHelp = t is not null },
            { "i|input=", "input file path", t => inputFilePath = t },
            { "o|output=", "output file path", t => outputFilePath = t },
            { "q|quotes=", "quotes symbol", (char t) => quotes = t },
            { "d|delimiter=", "delimiter symbol", (char t) => delimiter = t },
            { "header-delimiter=", "header delimiter symbol", (char t) => headerDelimiter = t },
            { "limit-length=", "limit item length", (int t) => limitLength = t },
            { "remove-row=", "remove row", t => Array.ForEach(ParseRange(t), removeRowList.Add) },
            { "remove-column=", "remove column", t => Array.ForEach(ParseRange(t), removeColumnList.Add) },
            { "add-header", "add header", t => addHeader = t is not null },
            { "remove-header", "remove header", t => removeHeader = t is not null },
            { "long-header", "indicates whether the header should be long", t => longHeader = t is not null },
            { "floor-double", "floor doubles", t => floorDoubles = t is not null },
            { "double-to-integer", "convert doubles to integers", t => convertDoublesToIntegers = t is not null },
            { "null-to-zero", "convert nulls to zeros", t => convertNullsToZeros = t is not null },
            { "string-to-number", "convert strings to numbers", t => convertStringsToNumbers = t is not null },
            { "remove-last", "remove last char", t => removeLastChar = t is not null },
            { "remove=", "remove substring", t => removeList.Add(t) },
            { "replace=", "replace substring (format: OLD/NEW)", t => replaceList.Add(ParseReplaceConfig(t)) },
            { "ignore-first-row", "indicates whether the first row should be ignored", t => ignoreFirstRow = t is not null },
            { "ignore-last-empty-column", "indicates whether the last column should be ignored if it is empty", t => ignoreLastEmptyColumn = t is not null },
        };

        #endregion

        private static void Main(string[] args)
        {
            if (!TryParseArgs(args))
                return;

            if (showHelp)
            {
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            CorrectInput(removeList);
            CorrectInput(replaceList);

            TryNormalize();
        }

        private static bool TryNormalize()
        {
            var normalizer = new Normalizer();

            if (removeHeader)
                normalizer.AddRule(new RemoveHeaderRule());

            if (addHeader)
                normalizer.AddRule(new AddHeaderRule(columnCount, headerDelimiter, longHeader));

            if (removeLastChar)
                normalizer.AddRule(new RemoveLastCharRule());

            if (floorDoubles)
                normalizer.AddRule(new FloorDoubleRule(headerDelimiter, quotes, ignoreLastEmptyColumn));
            
            if (convertDoublesToIntegers)
                normalizer.AddRule(new DoubleToIntegerRule(headerDelimiter, quotes, ignoreLastEmptyColumn));
            
            if (convertNullsToZeros)
                normalizer.AddRule(new NullToZeroRule(headerDelimiter));
            
            if (convertStringsToNumbers)
                normalizer.AddRule(new StringToNumberRule(headerDelimiter, quotes, ignoreLastEmptyColumn));
            
            if (ignoreFirstRow)
                normalizer.AddRule(new IgnoreFirstRowRule());

            if (limitLength.HasValue)
                normalizer.AddRule(new LimitLengthRule(limitLength.Value, headerDelimiter, quotes, ignoreLastEmptyColumn));

            foreach (string s in removeList)
                normalizer.AddRule(new RemoveRule(s));

            foreach ((string oldStr, string newStr) in replaceList)
                normalizer.AddRule(new ReplaceRule(oldStr, newStr));

            foreach (int index in removeRowList)
                normalizer.AddRule(new RemoveRowRule(index));

            foreach (int index in removeColumnList)
                normalizer.AddRule(new RemoveColumnRule(index, delimiter, quotes, ignoreLastEmptyColumn));

            try
            {
                normalizer.Normalize(inputFilePath ?? string.Empty, outputFilePath ?? string.Empty);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        #region Parsers

        private static bool TryParseArgs(string[] args)
        {
            try
            {
                ParseArgs(args);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private static void ParseArgs(string[] args)
        {
            List<string> extraArgs = options.Parse(args);

            if (extraArgs.Count > 0)
            {
                string extraArgsStr = string.Join(", ", extraArgs);
                throw new ArgumentException($"Arguments '{extraArgsStr}' are unknown.", nameof(args));
            }

            if (showHelp)
                return;

            if (string.IsNullOrEmpty(inputFilePath))
                throw new ApplicationException("Input file path not specified.");

            if (string.IsNullOrEmpty(outputFilePath))
                throw new ApplicationException("Output file path not specified.");

            if (inputFilePath == outputFilePath)
                throw new ApplicationException("Input file path is same as output file path.");

            var csvInfo = new CsvInfo(delimiter, quotes, ignoreLastEmptyColumn);
            columnCount = csvInfo.GetColumnCount(inputFilePath) - removeColumnList.Distinct().Count();

            if (columnCount <= 0)
                throw new ApplicationException("It is not possible to remove all columns.");
        }

        private static (string, string) ParseReplaceConfig(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("Invalid data.", nameof(data));

            string[] parts = data.Split('/');

            if (parts.Length != 2)
                throw new ArgumentException("Invalid data.", nameof(data));

            return (parts[0], parts[1]);
        }

        private static int[] ParseRange(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("Invalid data.", nameof(data));

            string[] parts = data.Split('-');

            if (parts.Length == 1)
            {
                if (!int.TryParse(parts[0], out int value))
                    throw new ArgumentException("Invalid data.", nameof(data));
                
                return new int[] { value };
            }

            if (parts.Length != 2)
                throw new ArgumentException("Invalid data.", nameof(data));
            
            if (!int.TryParse(parts[0], out int start) || !int.TryParse(parts[1], out int end))
                throw new ArgumentException("Invalid data.", nameof(data));
            
            if (start > end)
                (start, end) = (end, start);
            
            return Enumerable.Range(start, end - start + 1).ToArray();
        }

        #endregion

        #region Correctors

        private static void CorrectInput(IList<string> input)
        {
            for (int i = 0; i < input.Count; ++i)
            {
                input[i] = Correct(input[i]);
            }
        }

        private static void CorrectInput(IList<(string, string)> input)
        {
            for (int i = 0; i < input.Count; ++i)
            {
                (string oldStr, string newStr) curr = input[i];

                string correctedOldStr = Correct(curr.oldStr);
                string correctedNewStr = Correct(curr.newStr);

                input[i] = (correctedOldStr, correctedNewStr);
            }
        }

        private static string Correct(string data)
        {
            if (data.Contains(@"\n"))
                data = data.Replace(@"\n", "\n");

            if (data.Contains(@"\t"))
                data = data.Replace(@"\t", "\t");

            if (data.Contains(@"\v"))
                data = data.Replace(@"\v", "\v");

            if (data.Contains(@"\r"))
                data = data.Replace(@"\r", "\r");

            return data;
        }

        #endregion
    }
}

