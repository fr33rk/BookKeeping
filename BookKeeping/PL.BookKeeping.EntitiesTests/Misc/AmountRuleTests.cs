using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace PL.BookKeeping.Entities.Misc.Tests
{
    [TestClass()]
    public class AmountRuleTests
    {
        private class IsMatchTestSet
        {
            public IsMatchTestSet(string name, string rule, decimal value, bool expectedResult)
            {
                Name = name;
                Rule = rule;
                Value = value;
                ExpectedResult = expectedResult;
            }

            public string Rule;
            public decimal Value;
            public bool ExpectedResult;
            public string Name;
        }

        private void RunIsMatchTest(IsMatchTestSet testSet)
        {
            AmountRule rule = new AmountRule();
            rule.FromString(testSet.Rule);

            Assert.AreEqual(testSet.ExpectedResult, rule.IsMatch(testSet.Value), string.Format("Test {0}({1}) failed. {2} expected",
                testSet.Rule, testSet.Name, testSet.Value));
        }

        [TestMethod()]
        public void IsMatchTest()
        {
            // A) Test the signle equation
            var tests = new List<IsMatchTestSet>();
            tests.Add(new IsMatchTestSet("A.1", "x = 1.05", 1.05m, true));
            tests.Add(new IsMatchTestSet("A.2", "x = 1.05", 1.04m, false));
            tests.Add(new IsMatchTestSet("A.3", "x < 1.05", 1.04m, true));
            tests.Add(new IsMatchTestSet("A.4", "x < 1.05", 1.05m, false));
            tests.Add(new IsMatchTestSet("A.5", "x < 1.05", 1.06m, false));
            tests.Add(new IsMatchTestSet("A.6", "x <= 1.05", 1.04m, true));
            tests.Add(new IsMatchTestSet("A.7", "x <= 1.05", 1.05m, true));
            tests.Add(new IsMatchTestSet("A.8", "x <= 1.05", 1.06m, false));
            tests.Add(new IsMatchTestSet("A.9", "x >= 1.05", 1.04m, false));
            tests.Add(new IsMatchTestSet("A.10", "x >= 1.05", 1.05m, true));
            tests.Add(new IsMatchTestSet("A.11", "x >= 1.05", 1.06m, true));
            tests.Add(new IsMatchTestSet("A.12", "x > 1.05", 1.04m, false));
            tests.Add(new IsMatchTestSet("A.13", "x > 1.05", 1.05m, false));
            tests.Add(new IsMatchTestSet("A.14", "x > 1.05", 1.06m, true));

            // B) Test the double equation.
            tests.Add(new IsMatchTestSet("B.1", "0 < x < 1.05", 1.04m, true));
            tests.Add(new IsMatchTestSet("B.2", "0 < x < 1.05", 1.05m, false));
            tests.Add(new IsMatchTestSet("B.3", "0 < x < 1.05", 1.06m, false));
            tests.Add(new IsMatchTestSet("B.4", "0 < x <= 1.05", 1.04m, true));
            tests.Add(new IsMatchTestSet("B.5", "0 < x <= 1.05", 1.05m, true));
            tests.Add(new IsMatchTestSet("B.6", "0 < x <= 1.05", 1.06m, false));
            tests.Add(new IsMatchTestSet("B.7", "0 < x >= 1.05", 1.04m, false));
            tests.Add(new IsMatchTestSet("B.8", "0 < x >= 1.05", 1.05m, true));
            tests.Add(new IsMatchTestSet("B.9", "0 < x >= 1.05", 1.06m, true));
            tests.Add(new IsMatchTestSet("B.10", "0 < x > 1.05", 1.04m, false));
            tests.Add(new IsMatchTestSet("B.11", "0 < x > 1.05", 1.05m, false));
            tests.Add(new IsMatchTestSet("B.12", "0 < x > 1.05", 1.06m, true));

            tests.Add(new IsMatchTestSet("C.1", "1.02 < x < 1.05", 1.03m, true));

            foreach (var test in tests)
            {
                RunIsMatchTest(test);
            }
        }
    }
}