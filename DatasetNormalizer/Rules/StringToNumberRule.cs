using DatasetNormalizer.Csv;
using System.Collections.Generic;
using System.Linq;

namespace DatasetNormalizer.Rules
{
    internal record StringToNumberRule : NormalizerRule
    {
        public StringToNumberRule(char delimiter, char quotes, bool ignoreLastEmptyColumn = false)
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

            var newColumns = columns.Select(t => ConvertToNumber(t));
            return string.Join(Delimiter, newColumns);
        }

        private static string ConvertToNumber(string obj)
        {
            if (int.TryParse(obj, out int intValue))
                return intValue.ToString();
            
            if (double.TryParse(obj, out double doubleValue))
                return doubleValue.ToString();

            int hash = obj.GetHashCode();
            return hash.ToString();
        }
    }
}
