using DatasetNormalizer.Csv;
using System.Linq;

namespace DatasetNormalizer.Rules
{
    internal record LimitLengthRule : NormalizerRule
    {
        public LimitLengthRule(int maxLength, char delimiter, char quotes, bool ignoreLastEmptyColumn = false)
        {
            MaxLength = maxLength;
            Delimiter = delimiter;
            Quotes = quotes;
            IgnoreLastEmptyColumn = ignoreLastEmptyColumn;
        }

        public int MaxLength { get; set; }
        public char Delimiter { get; }
        public char Quotes { get; }
        public bool IgnoreLastEmptyColumn { get; }

        public override string Handle(string line, int lineIndex)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            var csvInfo = new CsvInfo(Delimiter, Quotes, IgnoreLastEmptyColumn);
            string[] columns = csvInfo.SplitLine(line);

            var newColumns = columns.Select(t => LimitLength(t, MaxLength));
            return string.Join(Delimiter, newColumns);
        }

        private static string LimitLength(string text, int maxLength)
        {           
            return text.Length <= maxLength
                ? text
                : text[0..maxLength];
        }
    }
}
