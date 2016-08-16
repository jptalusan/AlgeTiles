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

namespace AlgeTiles
{
	static class AlgorithmUtilities
	{
		private static string TAG = "AlgorithmUtilities";
		public static int[] RNG(string activityType, int numberOfVariables)
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

			Log.Debug(TAG, activityType + "," + numberOfVariables);
			if (Constants.MULTIPLY == activityType)
			{
				//(ax + b)(cx + d)
				if (Constants.ONE_VAR == numberOfVariables)
				{
					int[] vars = new int[4];
					a = PickRandom(multipyOneVarChoices);
					c = PickRandom(multipyOneVarChoices);
					b = a < 0 ? PickRandom(-9 - (3 * a), 9) : PickRandom(-9, 9 - (3 * a));
					d = c < 0 ? PickRandom(-9 - (3 * c), 9) : PickRandom(-9, 9 - (3 * c));

					vars[0] = a;
					vars[1] = b;
					vars[2] = c;
					vars[3] = d;
					return vars;
				}
				//(ax + by + e)(cx + dy + f)
				else if (Constants.TWO_VAR == numberOfVariables)
				{
					int[] vars = new int[6];
					a = PickRandom(multipyTwoVarChoices);
					c = PickRandom(multipyTwoVarChoices);
					b = a < 0 ? PickRandom(-2 - (2 * a), 2) : PickRandom(-2, 2 - (2 * a));
					d = c < 0 ? PickRandom(-2 - (2 * c), 2) : PickRandom(-2, 2 - (2 * c));
					
					//e
					if (a >= 0 && b >= 0)
						e = PickRandom(-8, 8 - (3 * a) - (3 * b));
					if (a >= 0 && b < 0)
						e = PickRandom(-8-(3 * b), 8 - (3 * a));
					if (a < 0 && b >= 0)
						e = PickRandom(-8 - (3 * a), 8 - (3 * b));
					if (a < 0 && b < 0)
						e = PickRandom(-8 - (3 * a) - (3 * b), 8);

					//f
					if (c >= 0 && d >= 0)
						f = PickRandom(-8, 8 - (3 * c) - (3 * d));
					if (c >= 0 && d < 0)
						f = PickRandom(-8 - (3 * d), 8 - (3 * c));
					if (c < 0 && d >= 0)
						f = PickRandom(-8 - (3 * c), 8 - (3 * d));
					if (c < 0 && d < 0)
						f = PickRandom(-8 - (3 * c) - (3 * d), 8);

					//TODO: Add more values for e and f generation
					vars[0] = a;
					vars[1] = b;
					vars[2] = c;
					vars[3] = d;
					vars[4] = e;
					vars[5] = f;
					return vars;
				}
			} else if (Constants.FACTOR == activityType)
			{
				//(ax + b)(cx + d)
				if (Constants.ONE_VAR == numberOfVariables)
				{
					int[] vars = new int[4];
					a = PickRandom(-3, 3);
					c = PickRandom(-3, 3);
					b = a >= 0 ? PickRandom(-9, 9 - (3 * a)) : PickRandom(-9 - (3 * a), 9);
					d = c >= 0 ? PickRandom(-9, 9 - (3 * c)) : PickRandom(-9 - (3 * c), 9);
					vars[0] = a;
					vars[1] = b;
					vars[2] = c;
					vars[3] = d;
					return vars;
				}
			}
			return null;
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

		public static bool isFirstAnswerCorrect(int[] vars, GridValue[] gvArr, int numberOfVariables)
		{
			//For 1 variable
			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;
			//For 2 variables
			int e = 0;
			int f = 0;

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

			if (Constants.TWO_VAR == numberOfVariables)
			{
				e = vars[4];
				f = vars[5];
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

		public static bool isSecondAnswerCorrect(List<int> vars, GridValue[] gvArr, int numberOfVariables)
		{
			foreach (int i in vars)
			{
				Log.Debug(TAG, i + "");
			}
			for (int i = 0; i < gvArr.Length; ++i)
				Log.Debug(TAG, "GvArr:" + gvArr[i].ToString());
			//For 1 variable
			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;
			//For 2 variables
			int e = 0;
			int f = 0;

			//Should expand/change when checking for outer grids
			GridValue upLeft = gvArr[0];
			GridValue upRight = gvArr[1];
			GridValue downLeft = gvArr[2];
			GridValue downRight = gvArr[3];

			if (Constants.ONE_VAR == numberOfVariables)
			{
				a = vars[0];
				b = vars[1];
				c = vars[2];
				//d = vars[3];
			}

			if (Constants.TWO_VAR == numberOfVariables)
			{
				e = vars[4];
				f = vars[5];
			}

			if (a == (upRight + downLeft - (upLeft + downRight)).x2Val &&
				b == (upRight + downLeft - (upLeft + downRight)).xVal &&
				c == (upRight + downLeft - (upLeft + downRight)).oneVal)
				return true;
			else
			{
				return false;
			}
		}

		public static List<int> expandingVars(int[] vars)
		{
			List<int> output = new List<int>();
			//For 1 variable
			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;
			//For 2 variables
			int e = 0;
			int f = 0;
			if (vars.Length <= 4)
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

			}
			return output;
		}
	}
}