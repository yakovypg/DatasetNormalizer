using DatasetNormalizer.Csv;
using System.Collections.Generic;
using System.Linq;

namespace DatasetNormalizer.Rules
{
    internal record RemoveColumnRule : NormalizerRule
    {
        public RemoveColumnRule(int columnIndex, char delimiter, char quotes, bool ignoreLastEmptyColumn = false)
            : this(new int[] { columnIndex }, delimiter, quotes, ignoreLastEmptyColumn)
        {
        }

        public RemoveColumnRule(int[] columnIndexes, char delimiter, char quotes, bool ignoreLastEmptyColumn = false)
        {
            ColumnIndexes = columnIndexes;
            Delimiter = delimiter;
            Quotes = quotes;
            IgnoreLastEmptyColumn = ignoreLastEmptyColumn;
        }

        public int[] ColumnIndexes { get; }
        public char Delimiter { get; }
        public char Quotes { get; }
        public bool IgnoreLastEmptyColumn { get; }

        public override string Handle(string line, int lineIndex)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            var csvInfo = new CsvInfo(Delimiter, Quotes, IgnoreLastEmptyColumn);
            string[] columns = csvInfo.SplitLine(line);

            var newColumns = columns.Where((t, i) => !ColumnIndexes.Contains(i));
            return string.Join(Delimiter, newColumns);
        }

        public static RemoveColumnRule Concat(params RemoveColumnRule[] rules)
        {
            if (rules is null)
                throw new ArgumentNullException(nameof(rules));

            if (rules.Length == 0)
                throw new ArgumentException("It is not possible to concat 0 rules.", nameof(rules));

            RemoveColumnRule firstRule = rules[0];

            char delimiter = firstRule.Delimiter;
            char quotes = firstRule.Quotes;
            bool ignoreLastEmptyColumn = firstRule.IgnoreLastEmptyColumn;

            if (rules.Any(t => t.Delimiter != delimiter || t.Quotes != quotes || t.IgnoreLastEmptyColumn != ignoreLastEmptyColumn))
                throw new ArgumentException("It is not possible to concat rules with different configuration.", nameof(rules));

            List<int> indexes = new();

            foreach (RemoveColumnRule rule in rules)
            {
                indexes.AddRange(rule.ColumnIndexes);
            }

            return new RemoveColumnRule(indexes.ToArray(), delimiter, quotes, ignoreLastEmptyColumn);
        }
    }
}
