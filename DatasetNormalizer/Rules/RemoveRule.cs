using System;

namespace DatasetNormalizer.Rules
{
    internal record RemoveRule : NormalizerRule
    {
        public RemoveRule(string substring)
        {
            if (string.IsNullOrEmpty(substring))
                throw new ArgumentException($"Argument '{nameof(substring)}' is null or empty.", nameof(substring));

            Substring = substring;
        }

        public string Substring { get; }

        public override string Handle(string line, int lineIndex)
        {
            return !string.IsNullOrEmpty(line)
                ? line.Replace(Substring, "")
                : line;
        }
    }
}
