using DatasetNormalizer.Csv;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatasetNormalizer.Rules
{
    internal record FloorDoubleRule : NormalizerRule
    {
        public FloorDoubleRule(char delimiter, char quotes, bool ignoreLastEmptyColumn = false)
        {
            Delimiter = delimiter;
            Quotes = quotes;
            IgnoreLastEmptyColumn = ignoreLastEmptyColumn;
        }

        public char Delimiter { get; }
        public char Quotes { get; }
        public bool IgnoreLastEmptyColumn { get; }

        public override string Handle(string line, int lineIndex)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            var csvInfo = new CsvInfo(Delimiter, Quotes, IgnoreLastEmptyColumn);
            string[] columns = csvInfo.SplitLine(line);

            var newColumns = columns.Select(FloorIfDouble);
            return string.Join(Delimiter, newColumns);
        }

        private static string FloorIfDouble(string obj)
        {           
            return double.TryParse(obj, out double doubleValue)
                ? Math.Floor(doubleValue).ToString()
                : obj;
        }
    }
}
