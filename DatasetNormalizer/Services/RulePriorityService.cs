using System;
using System.Collections.Generic;
using DatasetNormalizer.Rules;

namespace DatasetNormalizer.Services
{
    internal static class RulePriorityService
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

        public static int GetRulePriority(NormalizerRule rule)
        {
            int priority = RuleTypePriority.IndexOf(rule.GetType());
            
            return priority >= 0
                ? priority
                : throw new NotSupportedException("Rule type not supported.");
        }
    }
}