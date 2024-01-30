using DatasetNormalizer.Rules;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using DatasetNormalizer.Services;

namespace DatasetNormalizer
{
    internal class Normalizer
    {
        private SortedSet<NormalizerRule> _rules;

        public Normalizer() : this(new List<NormalizerRule>())
        {
        }

        public Normalizer(IEnumerable<NormalizerRule> rules)
        {
            if (rules is null)
                throw new ArgumentNullException(nameof(rules));

            var comparer = new NormalizerRuleComparer();
            _rules = new SortedSet<NormalizerRule>(rules, comparer);
        }

        public void AddRule(NormalizerRule rule)
        {
            _rules.Add(rule);
        }

        public void Normalize(string inputFilePath, string outputFilePath)
        {
            SimplifyRules();

            using var reader = new StreamReader(inputFilePath);
            using var writer = new StreamWriter(outputFilePath);

            var addHeaderRule = _rules.FirstOrDefault(t => t is AddHeaderRule);

            if (addHeaderRule is not null)
            {
                string line = addHeaderRule.Handle(string.Empty, 0);
                writer.WriteLine(line);

                _rules.Remove(addHeaderRule);
            }

            int lineIndex = 0;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine() ?? string.Empty;
                line = HandleLine(line, lineIndex);

                if (!string.IsNullOrEmpty(line))
                    writer.WriteLine(line);

                lineIndex++;
            }
        }

        private void SimplifyRules()
        {
            RemoveColumnRule[] removeColumnRules = _rules
                .Where(t => t is RemoveColumnRule)
                .Cast<RemoveColumnRule>()
                .ToArray();

            if (removeColumnRules.Length == 0)
                return;

            RemoveColumnRule combinedRule = RemoveColumnRule.Concat(removeColumnRules);

            foreach (var rule in removeColumnRules)
            {
                _rules.Remove(rule);
            }

            _rules.Add(combinedRule);
        }

        private string HandleLine(string line, int lineIndex)
        {
            if (lineIndex == 0 && _rules.Any(t => t is IgnoreFirstRowRule))
                return IgnoreFirstLine(line);

            foreach (NormalizerRule rule in _rules)
            {
                line = rule.Handle(line, lineIndex);
            }

            return line;
        }

        private string IgnoreFirstLine(string line)
        {
            var ignoreFirstRowRule = _rules.FirstOrDefault(t => t is IgnoreFirstRowRule);

            if (ignoreFirstRowRule is null)
                return line;
            
            int ignoreFirstRowRulePriority = RulePriorityService.GetRulePriority(ignoreFirstRowRule);
            var rules = _rules.Where(t => RulePriorityService.GetRulePriority(t) < ignoreFirstRowRulePriority);

            foreach (NormalizerRule rule in rules)
            {
                line = rule.Handle(line, 0);
            }

            return line;
        }
    }
}
