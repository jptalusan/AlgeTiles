using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using System.Text.RegularExpressions;

namespace AlgeTiles
{
	static class AlgorithmUtilities
	{
		private static string TAG = "AlgorithmUtilities";
		public static List<int> RNG(string activityType, int numberOfVariables)
		{
			int[] multipyOneVarChoices = { -3, -1, 0, 1, 3 };
			int[] multipyTwoVarChoices = { -2, -1, 0, 1, 2 };
			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;
			int e = 0;
			int f = 0;
			var rnd = new Random();
			List<int> vars = new List<int>();

			Log.Debug(TAG, activityType + "," + numberOfVariables);
			if (Constants.MULTIPLY == activityType)
			{
				//(ax + b)(cx + d)
				if (Constants.ONE_VAR == numberOfVariables)
				{
					while (a == 0 && b == 0)
					{
						a = PickRandom(multipyOneVarChoices);
						b = a >= 0 ? PickRandom(-9, 9 - (3 * a)) : PickRandom(-9 - (3 * a), 9);
					}
					while (c == 0 && d == 0)
					{
						c = PickRandom(multipyOneVarChoices);
						d = c >= 0 ? PickRandom(-9, 9 - (3 * c)) : PickRandom(-9 - (3 * c), 9);
					}

					vars.Add(a);
					vars.Add(b);
					vars.Add(c);
					vars.Add(d);
				}
				//(ax + by + c)(dx + ey + f)
				else if (Constants.TWO_VAR == numberOfVariables)
				{

					a = PickRandom(multipyTwoVarChoices);
					b = a > 0 ? PickRandom(-2, 2 - a) : PickRandom(-2 - (2 * a), 2);
					//e
					if (a >= 0 && b >= 0)
						c = PickRandom(-8, 8 - (3 * a) - (3 * b));
					if (a >= 0 && b < 0)
						c = PickRandom(-8 - (3 * b), 8 - (3 * a));
					if (a < 0 && b >= 0)
						c = PickRandom(-8 - (3 * a), 8 - (3 * b));
					if (a < 0 && b < 0)
						c = PickRandom(-8 - (3 * a) - (3 * b), 8);

					d = PickRandom(multipyTwoVarChoices);
					e = d > 0 ? PickRandom(-2, 2 - d) : PickRandom(-2 - (2 * d), 2);
					//f
					if (d >= 0 && e >= 0)
						f = PickRandom(-8, 8 - (3 * d) - (3 * e));
					if (d >= 0 && e < 0)
						f = PickRandom(-8 - (3 * e), 8 - (3 * d));
					if (d < 0 && e >= 0)
						f = PickRandom(-8 - (3 * d), 8 - (3 * e));
					if (d < 0 && e < 0)
						f = PickRandom(-8 - (3 * d) - (3 * e), 8);

					//TODO: Add more values for e and f generation
					vars.Add(a); //ax
					vars.Add(b); //by
					vars.Add(c); //c

					vars.Add(d); //ax
					vars.Add(e); //by
					vars.Add(f); //c
				}
			} else if (Constants.FACTOR == activityType)
			{
				//(ax + b)(cx + d)
				if (Constants.ONE_VAR == numberOfVariables)
				{
					while (a == 0 && b == 0)
					{
						a = PickRandom(-3, 3);
						b = a >= 0 ? PickRandom(-9, 9 - (3 * a)) : PickRandom(-9 - (3 * a), 9);
					}

					while (c == 0 && d == 0)
					{
						c = PickRandom(-3, 3);
						d = c >= 0 ? PickRandom(-9, 9 - (3 * c)) : PickRandom(-9 - (3 * c), 9);
					}

					vars.Add(a);
					vars.Add(b);
					vars.Add(c);
					vars.Add(d);
				}
			}

			foreach (int i in vars)
				Log.Debug(TAG, "Generated: " + i);
			//TODO: Fix for 2 variables
			if (areConstantsOnlyOneWwithValues(vars, numberOfVariables))
				return RNG(activityType, numberOfVariables);
			
			return vars;
		}

		private static bool areConstantsOnlyOneWwithValues(List<int> vars, int numberOfVariables)
		{
			if (numberOfVariables == 2)
			{
				int total = 0;
				for (int i = 0; i <= 5; ++i)
				{
					total += vars[i];
				}
				if (total == 0)
					return true;
				return false;
			} else
			{
				if (vars[0] == 0 && vars[2] == 0)
					return true;
			}
			return false;
		}

		public static int PickRandom(params int[] Selection)
		{
			var rnd = new Random();
			return Selection[rnd.Next(Selection.Length)];
		}

		public static int PickRandom(int a, int b)
		{
			var rnd = new Random();
			return rnd.Next(a, b);
		}

		public static bool isFirstAnswerCorrect(List<int> vars, GridValue[] gvArr, int numberOfVariables)
		{
			Log.Debug(TAG, "isFirstAnswerCorrect");
			foreach (int i in vars)
			{
				Log.Debug(TAG, i + "");
			}

			Log.Debug(TAG, "Mult: GvArr: midUpGV, midLowGV, midLeftGV, midRightGV");
			for (int i = 0; i < gvArr.Length; ++i)
				Log.Debug(TAG, "GvArr:" + gvArr[i].ToString());
			//For 1 variable
			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;

			//Should expand/change when checking for outer grids
			GridValue midUp = gvArr[0];
			GridValue midLo = gvArr[1];
			GridValue midLeft = gvArr[2];
			GridValue midRight = gvArr[3];

			if (Constants.ONE_VAR == numberOfVariables)
			{
				a = vars[0];
				b = vars[1];
				c = vars[2];
				d = vars[3];
			}

			if ((a == (midUp - midLo).xVal && b == (midUp - midLo).oneVal) &&
				(c == (midRight - midLeft).xVal && d == (midRight - midLeft).oneVal))
			{
				return true;
			}
			else if ((a == (midRight - midLeft).xVal && b == (midRight - midLeft).oneVal) &&
					 (c == (midUp - midLo).xVal && d == (midUp - midLo).oneVal))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		//For 2 var
		public static bool isFirstAnswerCorrect(List<int> vars, GridValue2Var[] gvArr)
		{
			Log.Debug(TAG, "isFirstAnswerCorrect");
			foreach (int i in vars)
			{
				Log.Debug(TAG, i + "");
			}

			Log.Debug(TAG, "Mult: GvArr: midUpGV, midLowGV, midLeftGV, midRightGV");
			for (int i = 0; i < gvArr.Length; ++i)
				Log.Debug(TAG, "GvArr:" + gvArr[i].ToString());
			//For 1 variable
			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;
			int e = 0;
			int f = 0;

			//Should expand/change when checking for outer grids
			GridValue2Var midUp = gvArr[0];
			GridValue2Var midLo = gvArr[1];
			GridValue2Var midLeft = gvArr[2];
			GridValue2Var midRight = gvArr[3];

			a = vars[0]; //x
			b = vars[1]; //y
			c = vars[2]; //one

			d = vars[3]; //x
			e = vars[4]; //y
			f = vars[5]; //one

			if ((a == (midUp - midLo).xVal && b == (midUp - midLo).yVal && c == (midUp - midLo).oneVal) &&
				(d == (midRight - midLeft).xVal && e == (midRight - midLeft).yVal && f == (midRight - midLeft).oneVal))
			{
				return true;
			}
			else if ((a == (midRight - midLeft).xVal && b == (midRight - midLeft).yVal && c == (midRight - midLeft).oneVal) &&
					 (d == (midUp - midLo).xVal && e == (midUp - midLo).yVal && f == (midUp - midLo).oneVal))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool isSecondAnswerCorrect(List<int> vars, GridValue[] gvArr, int numberOfVariables)
		{
			Log.Debug(TAG, "isSecondAnswerCorrect");
			foreach (int i in vars)
			{
				Log.Debug(TAG, i + "");
			}
			Log.Debug(TAG, "Mult: GvArr:  upperLeftGV, upperRightGV, lowerLeftGV, lowerRightGV ");
			for (int i = 0; i < gvArr.Length; ++i)
				Log.Debug(TAG, "GvArr:" + gvArr[i].ToString());
			//For 1 variable
			int a = 0;
			int b = 0;
			int c = 0;

			//Should expand/change when checking for outer grids
			GridValue upLeft = gvArr[0];
			GridValue upRight = gvArr[1];
			GridValue downLeft = gvArr[2];
			GridValue downRight = gvArr[3];

			a = vars[0];
			b = vars[1];
			c = vars[2];

			if (a == (upRight + downLeft - (upLeft + downRight)).x2Val &&
				b == (upRight + downLeft - (upLeft + downRight)).xVal &&
				c == (upRight + downLeft - (upLeft + downRight)).oneVal)
				return true;
			else
			{
				return false;
			}
		}

		//For 2 var
		public static bool isSecondAnswerCorrect(List<int> vars, GridValue2Var[] gvArr, int numberOfVariables)
		{
			Log.Debug(TAG, "isSecondAnswerCorrect");
			foreach (int i in vars)
			{
				Log.Debug(TAG, i + "");
			}
			Log.Debug(TAG, "Mult: GvArr:  upperLeftGV, upperRightGV, lowerLeftGV, lowerRightGV ");
			for (int i = 0; i < gvArr.Length; ++i)
				Log.Debug(TAG, "GvArr:" + gvArr[i].ToString());

			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;
			int e = 0;
			int f = 0;

			//Should expand/change when checking for outer grids
			GridValue2Var upLeft = gvArr[0];
			GridValue2Var upRight = gvArr[1];
			GridValue2Var downLeft = gvArr[2];
			GridValue2Var downRight = gvArr[3];

			a = vars[0]; //x2
			b = vars[1]; //y2
			c = vars[2]; //xy
			d = vars[3]; //x
			e = vars[4]; //y
			f = vars[5]; //one

			if (a == (upRight + downLeft - (upLeft + downRight)).x2Val &&
				b == (upRight + downLeft - (upLeft + downRight)).y2Val &&
				c == (upRight + downLeft - (upLeft + downRight)).xyVal &&
				d == (upRight + downLeft - (upLeft + downRight)).xVal &&
				e == (upRight + downLeft - (upLeft + downRight)).yVal &&
				f == (upRight + downLeft - (upLeft + downRight)).oneVal)
				return true;
			else
			{
				return false;
			}
		}

		public static List<int> expandingVars(List<int> vars)
		{
			foreach (int i in vars)
				Log.Debug(TAG, "Expanding: " + i);
			List<int> output = new List<int>();
			//For 1 variable
			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;
			//For 2 variables
			int e = 0;
			int f = 0;
			if (vars.Count <= 4)
			{
				a = vars[0];
				b = vars[1];
				c = vars[2];
				d = vars[3];

				output.Add(a * c);
				output.Add((a * d) + (b * c));
				output.Add(b * d);
			} else //two variables
			{
				a = vars[0]; //ax
				b = vars[1]; //by
				c = vars[2]; //c

				d = vars[3]; //dx
				e = vars[4]; //ey
				f = vars[5]; //f

				output.Add(a * d); //x2
				output.Add(b * e); //y2
				output.Add((a * e) + (b * d)); //xy
				output.Add((a * f) + (c * d)); //x
				output.Add((b * f) + (c * e)); //y
				output.Add(c * f); //one
			}
			if (vars.Count <= 4)
				Log.Debug(TAG, "x2, x, 1");
			else
				Log.Debug(TAG, "x2, y2, xy, x, y, 1");
			foreach (int i in output)
				Log.Debug(TAG, "Expanded: " + i);
			return output;
		}

		private static string removeSpacesFromString(string input)
		{
			return Regex.Replace(input, @"\s+", "");
		}

		public static bool isThirdAnswerCorrect(List<int> vars, string answer)
		{
			answer = removeSpacesFromString(answer);

			return false;
		}
	}
}