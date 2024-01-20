namespace DatasetNormalizer.Rules
{
    internal record RemoveHeaderRule : NormalizerRule
    {
        public override string Handle(string line, int lineIndex)
        {
            return lineIndex == 0
                ? string.Empty
                : line;
        }
    }
}
