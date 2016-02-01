using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PL.BookKeeping.Entities.Misc
{
	public class AmountRule
	{
		#region Fields

		private enum Operator
		{
			Unspecified,
			EqualTo,
			SmallerThan,
			EqualToOrSmallerThan,
			GreaterThan,
			EqualToOrGreaterThan
		}

		private Operator mMinOperator;
		private decimal? mMinValue;
		private Operator mMaxOperator;
		private decimal? mMaxValue;

		#endregion Fields

		#region From and to string

		public override string ToString()
		{
			if (isValid())
			{
				if (mMinOperator == Operator.Unspecified)
				{
					return string.Format("x {0} {1}", operatorToString(mMaxOperator), mMaxValue.Value.ToString("0.00", CultureInfo.InvariantCulture));
				}
				else
				{
					return String.Format("{0} {1} x {2} {3}",
						mMinValue.Value.ToString("0.00", CultureInfo.InvariantCulture),
						operatorToString(mMinOperator),
						operatorToString(mMaxOperator),
						mMaxValue.Value.ToString("0.00", CultureInfo.InvariantCulture));
				}
			}

			return null;
		}

		public bool FromString(string expression)
		{
			bool retValue = false;

			if (expression != null)
			{
				// Remove all spaces from the expression to simplify the regular expressions.
				expression = removeSpaces(expression).ToLower();

				if (isSingleExpression(expression))
				{
					mMinOperator = Operator.Unspecified;
					mMinValue = null;
					mMaxOperator = operatorFromString(Regex.Match(expression, @"(<=|>=|<|>|=)").Value);
					mMaxValue = Convert.ToDecimal(Regex.Match(expression, @"(?!=)\d+\.?\d*").Value, CultureInfo.InvariantCulture);
					retValue = true;
				}
				else if (isDoubleExpression(expression))
				{
					mMinValue = Convert.ToDecimal(Regex.Match(expression, @"\d+(\.|,)?\d*(?=(<=|>=|<|>)x)").Value, CultureInfo.InvariantCulture);
					mMinOperator = operatorFromString(Regex.Match(expression, @"(<=|>=|<|>)(?=x)").Value);
					mMaxOperator = operatorFromString(Regex.Match(expression, @"(?<=x)(<=|>=|<|>)").Value);
					mMaxValue = Convert.ToDecimal(Regex.Match(expression, @"(\d+(\.|,)?\d*)$").Value, CultureInfo.InvariantCulture);
					retValue = true;
				}
			}

			return retValue;
		}

		#endregion From and to string

		#region IsValid check

		public bool IsValid(string expression)
		{
			return isSingleExpression(expression) ||
				   isDoubleExpression(expression);
		}

		#endregion IsValid check

		#region IsMatch check

		public bool IsMatch(decimal value)
		{
			bool retValue = false;

			if (isValid())
			{
				if (value < 0)
				{
					value *= -1;
				}

				retValue = isMatch(mMaxOperator, mMaxValue.Value, value);

				if (retValue && mMinOperator != Operator.Unspecified)
				{
					retValue &= isMatch(invert(mMinOperator), mMinValue.Value, value);
				}
			}

			return retValue;
		}

		#endregion IsMatch check

		#region Helper methods

		private string removeSpaces(string expression)
		{
			return expression.Trim().Replace(" ", string.Empty);
		}

		private bool isSingleExpression(string expression)
		{
			//  ^x(<=|>=|<|>|=)\d+(\.|,)?\d*
			//     ^ assert position at start of the string
			//     x matches the character x literally(case sensitive)
			//  1st Capturing group(<=|>=|<|>|=)
			//      1st Alternative: <=
			//          <= matches the characters <= literally
			//      2nd Alternative: >=
			//          >= matches the characters >= literally
			//      3rd Alternative: <
			//          < matches the characters < literally
			//      4th Alternative: >
			//          > matches the characters > literally
			//      5th Alternative: =
			//          = matches the character = literally
			//  \d + match a digit[0 - 9]
			//      Quantifier: +Between one and unlimited times, as many times as possible, giving back as needed[greedy]
			//  2nd Capturing group(\.|,) ?
			//      Quantifier : ? Between zero and one time, as many times as possible, giving back as needed[greedy]
			//      Note: A repeated capturing group will only capture the last iteration. Put a capturing group around the repeated group to capture all iterations or use a non-capturing group instead if you're not interested in the data
			//      1st Alternative: \.
			//          \. matches the character.literally
			//      2nd Alternative: ,
			//          , matches the character , literally
			//  \d* match a digit[0 - 9]
			//      Quantifier: *Between zero and unlimited times, as many times as possible, giving back as needed[greedy]
			return Regex.IsMatch(removeSpaces(expression), @"^x(<=|>=|<|>|=)\d+(\.|,)?\d*");
		}

		private bool isDoubleExpression(string expression)
		{
			// See isSingleExpression for a description of the regular expression.
			return Regex.IsMatch(removeSpaces(expression), @"^\d+(\.|,)?\d*(<=|>=|<|>)x(<=|>=|<|>)\d+(\.|,)?\d*");
		}

		private bool isMatch(Operator theOperator, decimal limit, decimal value)
		{
			var minComparisson = value.CompareTo(limit);

			switch (theOperator)
			{
				case Operator.EqualTo:
					return minComparisson == 0;

				case Operator.EqualToOrGreaterThan:
					return minComparisson >= 0;

				case Operator.GreaterThan:
					return minComparisson > 0;

				case Operator.SmallerThan:
					return minComparisson < 0;

				case Operator.EqualToOrSmallerThan:
					return minComparisson <= 0;

				case Operator.Unspecified:
				default:
					return false;
			}
		}

		private bool isValid()
		{
			if ((mMaxOperator == Operator.Unspecified || mMaxValue == null)
				|| (mMinOperator != Operator.Unspecified && mMinValue == null))
			{
				return false;
			}

			return true;
		}

		private Operator operatorFromString(string operatorAsString)
		{
			if (operatorAsString == "=")
			{
				return Operator.EqualTo;
			}
			else if (operatorAsString == "<")
			{
				return Operator.SmallerThan;
			}
			else if (operatorAsString == "<=")
			{
				return Operator.EqualToOrSmallerThan;
			}
			else if (operatorAsString == ">")
			{
				return Operator.GreaterThan;
			}
			else if (operatorAsString == ">=")
			{
				return Operator.EqualToOrGreaterThan;
			}
			else
			{
				return Operator.Unspecified;
			}
		}

		private string operatorToString(Operator anOperator)
		{
			switch (anOperator)
			{
				case Operator.EqualTo: return "=";
				case Operator.SmallerThan: return "<";
				case Operator.EqualToOrSmallerThan: return "<=";
				case Operator.GreaterThan: return ">";
				case Operator.EqualToOrGreaterThan: return ">=";
				case Operator.Unspecified:
				default:
					return "?";
			}
		}

		private Operator invert(Operator anOperator)
		{
			switch (anOperator)
			{
				case Operator.EqualToOrGreaterThan:
					return Operator.EqualToOrSmallerThan;

				case Operator.GreaterThan:
					return Operator.SmallerThan;

				case Operator.SmallerThan:
					return Operator.GreaterThan;

				case Operator.EqualToOrSmallerThan:
					return Operator.EqualToOrGreaterThan;

				case Operator.EqualTo:
				case Operator.Unspecified:
				default:
					return anOperator;
			}
		}

		#endregion Helper methods
	}
}