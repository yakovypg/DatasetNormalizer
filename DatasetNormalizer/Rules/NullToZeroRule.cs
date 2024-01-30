namespace DatasetNormalizer.Rules
{
    internal record NullToZeroRule : NormalizerRule
    {
        public NullToZeroRule(char delimiter)
        {
            Delimiter = delimiter;
        }

        public char Delimiter { get; }

        public override string Handle(string line, int lineIndex)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            string nullItem = $"{Delimiter}{Delimiter}";
            string zeroItem = $"{Delimiter}0{Delimiter}";

            while (line.Contains(nullItem))
            {
                line = line.Replace(nullItem, zeroItem);
            }

            return line;
        }
    }
}