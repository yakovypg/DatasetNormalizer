using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DatasetNormalizer.Csv
{
    internal class CsvInfo
    {
        public CsvInfo(char delimiter = ',', char quotes = '"', bool ignoreLastEmptyColumn = false)
        {
            Delimiter = delimiter;
            Quotes = quotes;
            IgnoreLastEmptyColumn = ignoreLastEmptyColumn;
        }

        public char Delimiter { get; }
        public char Quotes { get; }
        public bool IgnoreLastEmptyColumn { get; }

        public string[] GetHeader(string filePath)
        {
            string? headerLine = null;

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream || string.IsNullOrEmpty(headerLine))
                {
                    headerLine = reader.ReadLine();
                }
            }

            if (string.IsNullOrEmpty(headerLine))
                throw new ApplicationException("Dataset is empty.");

            return SplitLine(headerLine);
        }

        public int GetColumnCount(string filePath)
        {
            string[] columns = GetHeader(filePath);
            return columns.Length;
        }

        public string[] SplitLine(string line)
        {
            List<string> parts = new();

            int partStart = 0;
            bool isInQuotes = false;

            for (int i = 0; i < line.Length; ++i)
            {
                char currSymbol = line[i];

                if (currSymbol == Quotes)
                {
                    isInQuotes = !isInQuotes;
                }
                else if (currSymbol == Delimiter && !isInQuotes)
                {
                    string part = line[partStart..i];
                    parts.Add(part);
                    partStart = i + 1;
                }
            }

            string lastPart = line[partStart..line.Length];
            parts.Add(lastPart);

            if (parts.Count > 0 && string.IsNullOrEmpty(parts[^1]) && IgnoreLastEmptyColumn)
                parts.RemoveAt(parts.Count - 1);

            return parts.ToArray();
        }
    }
}
