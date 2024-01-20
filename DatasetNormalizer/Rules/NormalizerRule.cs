namespace DatasetNormalizer.Rules
{
    internal abstract record NormalizerRule
    {
        public abstract string Handle(string line, int lineIndex);
    }
}
