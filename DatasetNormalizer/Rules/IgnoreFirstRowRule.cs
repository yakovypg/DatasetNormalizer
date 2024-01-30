namespace DatasetNormalizer.Rules
{
    internal record IgnoreFirstRowRule : NormalizerRule
    {
        public override string Handle(string line, int lineIndex)
        {
            return line ?? string.Empty;
        }
    }
}