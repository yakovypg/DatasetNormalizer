using System;
using System.Collections.Generic;

namespace DatasetNormalizer.Rules
{
    internal class NormalizerRuleComparer : IComparer<NormalizerRule>
    {
        private static readonly List<Type> RuleTypePriority = new()
        {
            typeof(RemoveRowRule),
            typeof(RemoveColumnRule),
            typeof(RemoveHeaderRule),
            typeof(AddHeaderRule),
            typeof(IgnoreFirstRowRule),
            typeof(RemoveLastCharRule),
            typeof(RemoveRule),
            typeof(ReplaceRule),
            typeof(FloorDoubleRule),
            typeof(DoubleToIntegerRule),
            typeof(StringToNumberRule),
            typeof(LimitLengthRule),
            typeof(NullToZeroRule),
        };

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
            int priority = RuleTypePriority.IndexOf(rule.GetType());
            
            return priority >= 0
                ? priority
                : throw new NotSupportedException("Rule type not supported.");
        }
    }
}
