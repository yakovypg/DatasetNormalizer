namespace DatasetNormalizer.Rules
{
    internal record RemoveLastCharRule : NormalizerRule
    {
        public override string Handle(string line, int lineIndex)
        {
            return line?.Length > 0
                ? line[..^1]
                : line ?? string.Empty;
        }
    }
}
