using System;

namespace DatasetNormalizer.Rules
{
    internal record ReplaceRule : NormalizerRule
    {
        public ReplaceRule(string oldSubstring, string newSubstring)
        {
            if (string.IsNullOrEmpty(oldSubstring))
                throw new ArgumentException($"Argument '{nameof(oldSubstring)}' is null or empty.", nameof(oldSubstring));

            OldSubstring = oldSubstring;
            NewSubstring = newSubstring ?? string.Empty;
        }

        public string OldSubstring { get; }
        public string NewSubstring { get; }

        public override string Handle(string line, int lineIndex)
        {
            return !string.IsNullOrEmpty(line)
                ? line.Replace(OldSubstring, NewSubstring)
                : line;
        }
    }
}
