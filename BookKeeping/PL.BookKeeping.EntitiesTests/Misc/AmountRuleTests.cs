using NUnit.Framework;

namespace PL.BookKeeping.Entities.Misc.Tests
{
	[TestFixture()]
	public class AmountRuleTests
	{
		#region IsMatchTest

		[Test, TestCaseSource("IsMatchTestCases")]
		public void IsMatchTest(string ruleDef, decimal value, bool expectedResult)
		{
			AmountRule rule = new AmountRule();
			rule.FromString(ruleDef);

			Assert.AreEqual(expectedResult, rule.IsMatch(value));
		}

		private static object[] IsMatchTestCases =
		{
			// A) Test the single equation
			new object[] { "x = 1.05"  , 1.05m, true  },
			new object[] { "x = 1.05"  , 1.04m, false },
			new object[] { "x < 1.05"  , 1.04m, true  },
			new object[] { "x < 1.05"  , 1.05m, false },
			new object[] { "x < 1.05"  , 1.06m, false },
			new object[] { "x <= 1.05" , 1.04m, true  },
			new object[] { "x <= 1.05" , 1.05m, true  },
			new object[] { "x <= 1.05" , 1.06m, false },
			new object[] { "x >= 1.05" , 1.04m, false },
			new object[] { "x >= 1.05" , 1.05m, true  },
			new object[] { "x >= 1.05" , 1.06m, true  },
			new object[] { "x > 1.05"  , 1.04m, false },
			new object[] { "x > 1.05"  , 1.05m, false },
			new object[] { "x > 1.05"  , 1.06m, true  },

			// B) Test the double equation.
			new object[] { "0 < x < 1.05" , 1.04m, true  },
			new object[] { "0 < x < 1.05" , 1.05m, false },
			new object[] { "0 < x < 1.05" , 1.06m, false },
			new object[] { "0 < x <= 1.05", 1.04m, true  },
			new object[] { "0 < x <= 1.05", 1.05m, true  },
			new object[] { "0 < x <= 1.05", 1.06m, false },
			new object[] { "0 < x >= 1.05", 1.04m, false },
			new object[] { "0 < x >= 1.05", 1.05m, true  },
			new object[] { "0 < x >= 1.05", 1.06m, true  },
			new object[] { "0 < x > 1.05" , 1.04m, false },
			new object[] { "0 < x > 1.05" , 1.05m, false },
			new object[] { "0 < x > 1.05" , 1.06m, true  },

			// C) Test the double equation. With not 0
			new object[] { "1.02 < x < 1.05", 1.03m, true },
		};

		#endregion IsMatchTest

		#region IsValidTest

		[Test, TestCaseSource("IsValidTestSet")]
		public void IsValidTest(string input, bool expectedResult)
		{
			AmountRule rule = new AmountRule();

			Assert.AreEqual(expectedResult, rule.FromString(input));
		}

		private static object[] IsValidTestSet =
		{
			// A) Single expressions
			// A.1) Operators
			new object[] { "x<1" , true },
			new object[] { "x<=1", true },
			new object[] { "x>1" , true },
			new object[] { "x>=1", true },
			new object[] { "x=1" , true },
			new object[] { "x%1" , false},
			// A.2) Spaces
			new object[] { "x =1" , true },
			new object[] { "x = 1" , true },
			new object[] { "x  = 1" , true },
			new object[] { "x  =  1" , true },
			// A.3) Values
			new object[] { "x = 1" , true },
			new object[] { "x = 1." , true },
			new object[] { "x = 1.0" , true },
			new object[] { "x = 1.01" , true },
			new object[] { "x = 1,01" , true },
			new object[] { "x = -1,01" , false },
			new object[] { "x = -1,O1" , false },
			// A.4) x
			new object[] { "X = 1" , true },
			new object[] { "Y = 1" , false },
			new object[] { " = 1" , false },

			// B) Double expressions
			// B.1) Operators
			new object[] { "0<x<1" , true },
			new object[] { "0<=x<=1", true },
			new object[] { "0>X>1" , true },
			new object[] { "0>=x>=1", true },
			new object[] { "0=x=1" , false },
			new object[] { "0<x%1" , false},
			// B.2) Spaces
			new object[] { "0 < x< 1" , true },
			new object[] { "0 <  x <  1" , true },
			new object[] { "0<  x  <1" , true },
			new object[] { "  0<x<1   " , true },
			// B.3) Values
			new object[] { "1 < x < 2" , true },
			new object[] { "1. < x < 2" , true },
			new object[] { "1.0 < x < 2" , true },
			new object[] { "1.1 < x < 2" , true },
			new object[] { "1,0 < x < 2" , true },
			new object[] { "-1< x < 1" , false },
			new object[] { "1,O < x < 2" , false },

			// B.4) x
			new object[] { "1 < X < 2" , true },
			new object[] { "1 < y < 2" , false },
			new object[] { "1 < < 2" , false },
		};

		#endregion IsValidTest

		#region FromStringTest

		[Test, TestCaseSource("IsValidTestSet")]
		public void FromStringTest(string input, bool expectedResult)
		{
			AmountRule rule = new AmountRule();

			Assert.AreEqual(expectedResult, rule.FromString(input));
		}

		#endregion FromStringTest

		#region ToStringTest

		[Test, TestCaseSource("ToStringTestSet")]
		public void ToStringTest(string input, string output)
		{
			AmountRule rule = new AmountRule();
			rule.FromString(input);

			Assert.AreEqual(output, rule.ToString());
		}

		private static object[] ToStringTestSet =
		{
			// A) Spaces
			new object[] {"x<1", "x < 1.00"},
			new object[] {"x =1", "x = 1.00"},
			new object[] {"x = 1", "x = 1.00"},
			new object[] {"x  =  1", "x = 1.00"},
			new object[] {"X = 1", "x = 1.00"},
			new object[] {"1<x<2", "1.00 < x < 2.00"},
			new object[] {"  1  <   x<2", "1.00 < x < 2.00"},

			// B) x
			new object[] {"X < 1", "x < 1.00"},
			new object[] {"1 < X < 2", "1.00 < x < 2.00"},

			// C) invalid
			new object[] {"y < 1", null},

			// D) Value
			new object[] {"x = 1.", "x = 1.00"},
			new object[] {"x = 1,", "x = 1.00"},
			new object[] {"x = 1.0", "x = 1.00"},
			new object[] {"x = 1,00", "x = 1.00"},
			new object[] {"x = 1.01", "x = 1.01"},
			new object[] {"x = 1.001", "x = 1.00"},
			new object[] {"x = 1.005", "x = 1.01"},
			new object[] {"1. < x < 2.005", "1.00 < x < 2.01"},
			new object[] {"1, < x < 2,005", "1.00 < x < 2.01"},
		};

		#endregion ToStringTest
	}
}