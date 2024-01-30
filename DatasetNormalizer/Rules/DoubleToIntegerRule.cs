using DatasetNormalizer.Csv;
using System.Linq;

namespace DatasetNormalizer.Rules
{
    internal record DoubleToIntegerRule : NormalizerRule
    {
        public DoubleToIntegerRule(char delimiter, char quotes, bool ignoreLastEmptyColumn = false)
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

            var newColumns = columns.Select(ConvertToIntegerIfDouble);
            return string.Join(Delimiter, newColumns);
        }

        private static string ConvertToIntegerIfDouble(string obj)
        {
            if (!double.TryParse(obj, out _))
                return obj;
            
            obj = obj.Replace(",", string.Empty).Replace(".", string.Empty);

            while (obj.Length > 1 && !int.TryParse(obj, out _))
            {
                obj = obj.Remove(obj.Length - 1);
            }

            return obj;
        }
    }
}