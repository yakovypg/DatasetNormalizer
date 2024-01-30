using System;
using System.Collections.Generic;
using DatasetNormalizer.Services;

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

            int xPriority = RulePriorityService.GetRulePriority(x);
            int yPriority = RulePriorityService.GetRulePriority(y);
            int priorityCompare = xPriority.CompareTo(yPriority);

            return priorityCompare != 0
                ? priorityCompare
                : -1;
        }
    }
}
