using System.Linq;

namespace DatasetNormalizer.Rules
{
    internal record AddHeaderRule : NormalizerRule
    {
        public AddHeaderRule(int columnCount, char delimiter, bool isHeaderLong = false)
        {
            ColumnCount = columnCount;
            Delimiter = delimiter;
            IsHeaderLong = isHeaderLong;
        }

        public int ColumnCount { get; }
        public char Delimiter { get; }
        public bool IsHeaderLong { get; }

        public override string Handle(string line, int lineIndex)
        {
            if (lineIndex > 0)
                return line;

            string prefix = IsHeaderLong ? "column" : "c";
            var columns = Enumerable.Range(1, ColumnCount).Select(t => $"{prefix}{t}");

            return string.Join(Delimiter, columns);
        }
    }
}
