namespace DatasetNormalizer.Rules
{
    internal record RemoveRowRule : NormalizerRule
    {
        public RemoveRowRule(int rowIndex)
        {
            RowIndex = rowIndex;
        }

        public int RowIndex { get; }

        public override string Handle(string line, int lineIndex)
        {
            return RowIndex == lineIndex
                ? string.Empty
                : line;
        }
    }
}
