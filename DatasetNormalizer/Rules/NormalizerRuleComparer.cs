using System.Collections.Generic;

namespace DatasetNormalizer.Rules
{
    internal class NormalizerRuleComparer : IComparer<NormalizerRule>
    {
        public int Compare(NormalizerRule? x, NormalizerRule? y)
        {
            if(x is null)
                throw new ArgumentNullException(nameof(x));

            if(y is null)
                throw new ArgumentNullException(nameof(y));

            if (x.Equals(y))
                return 0;

            int xPriority = GetRulePriority(x);
            int yPriority = GetRulePriority(y);
            int priorityCompare = xPriority.CompareTo(yPriority);

            return priorityCompare != 0
                ? priorityCompare
                : -1;
        }

        private static int GetRulePriority(NormalizerRule rule)
        {
            if (rule is RemoveRowRule)
                return 0;

            if (rule is RemoveColumnRule)
                return 1;

            if (rule is RemoveHeaderRule)
                return 2;

            if (rule is AddHeaderRule)
                return 3;

            if (rule is RemoveLastCharRule)
                return 4;

            if (rule is RemoveRule)
                return 5;

            if (rule is ReplaceRule)
                return 6;

            if (rule is StringToNumberRule)
                return 7;
            
            if (rule is LimitLengthRule)
                return 8;

            throw new NotSupportedException("Rule type not supported.");
        }
    }
}
