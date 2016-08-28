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
using Android.Graphics;

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

			long seed = Java.Lang.JavaSystem.CurrentTimeMillis();
			var rnd = new Random((int)seed);
			
			List<int> vars = new List<int>();

			Log.Debug(TAG, activityType + "," + numberOfVariables);

			if (Constants.MULTIPLY == activityType)
			{
				//(ax + b)(cx + d)
				if (Constants.ONE_VAR == numberOfVariables)
				{
					while (a == 0 && b == 0)
					{
						//a = multipyOneVarChoices[rnd.Next(multipyOneVarChoices.Length)];
						a = PickRandom(rnd, multipyOneVarChoices);
						b = a >= 0 ? PickRandom(rnd, -9, 9 - (3 * a)) : PickRandom(rnd, -9 - (3 * a), 9);
					}
					while (c == 0 && d == 0)
					{
						c = PickRandom(rnd, multipyOneVarChoices);
						d = c >= 0 ? PickRandom(rnd, -9, 9 - (3 * c)) : PickRandom(rnd, -9 - (3 * c), 9);
					}

					vars.Add(a);
					vars.Add(b);
					vars.Add(c);
					vars.Add(d);
				}
				//(ax + by + c)(dx + ey + f)
				else if (Constants.TWO_VAR == numberOfVariables)
				{

					a = PickRandom(rnd, multipyTwoVarChoices);
					b = a > 0 ? PickRandom(rnd, -2, 2 - a) : PickRandom(rnd, -2 - (2 * a), 2);
					//e
					if (a >= 0 && b >= 0)
						c = PickRandom(rnd, -8, 8 - (3 * a) - (3 * b));
					if (a >= 0 && b < 0)
						c = PickRandom(rnd, -8 - (3 * b), 8 - (3 * a));
					if (a < 0 && b >= 0)
						c = PickRandom(rnd, -8 - (3 * a), 8 - (3 * b));
					if (a < 0 && b < 0)
						c = PickRandom(rnd, -8 - (3 * a) - (3 * b), 8);

					d = PickRandom(rnd, multipyTwoVarChoices);
					e = d > 0 ? PickRandom(rnd, -2, 2 - d) : PickRandom(rnd, -2 - (2 * d), 2);
					//f
					if (d >= 0 && e >= 0)
						f = PickRandom(rnd, -8, 8 - (3 * d) - (3 * e));
					if (d >= 0 && e < 0)
						f = PickRandom(rnd, -8 - (3 * e), 8 - (3 * d));
					if (d < 0 && e >= 0)
						f = PickRandom(rnd, -8 - (3 * d), 8 - (3 * e));
					if (d < 0 && e < 0)
						f = PickRandom(rnd, -8 - (3 * d) - (3 * e), 8);

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
						a = PickRandom(rnd, -3, 3);
						b = a >= 0 ? PickRandom(rnd, -9, 9 - (3 * a)) : PickRandom(rnd, -9 - (3 * a), 9);
					}

					while (c == 0 && d == 0)
					{
						c = PickRandom(rnd, -3, 3);
						d = c >= 0 ? PickRandom(rnd, -9, 9 - (3 * c)) : PickRandom(rnd, -9 - (3 * c), 9);
					}

					int gcdOfFirst = GCD(a, b);
					int gcdOfSecond = GCD(c, d);

					a /= gcdOfFirst;
					b /= gcdOfFirst;
					c /= gcdOfSecond;
					d /= gcdOfSecond;

					vars.Add(a);
					vars.Add(b);
					vars.Add(c);
					vars.Add(d);
				}
			}

			//TODO: Fix for 2 variables
			if (areConstantsOnlyOneWwithValues(activityType, vars, numberOfVariables))
				return RNG(activityType, numberOfVariables);

			return vars;
		}

		private static bool areConstantsOnlyOneWwithValues(String activityType, List<int> vars, int numberOfVariables)
		{
			if (numberOfVariables == 2)
			{
				int ax = vars[0];
				int by = vars[1];
				int dx = vars[3];
				int ey = vars[4];

				int total1 = ax + by;
				int total2 = dx + ey;
				if (total1 == 0 || total2 == 0)
					return true;
				return false;
			}
			else if (Constants.MULTIPLY == activityType)
			{
				int ax = vars[0];
				int b = vars[1];
				int cx = vars[2];
				int d = vars[3];
				if (ax == 0 || cx == 0)
					return true;
			}
			else if (Constants.FACTOR == activityType)
			{
				int ax = vars[0];
				int b = vars[1];
				int cx = vars[2];
				int d = vars[3];

				if (ax == 0 || b == 0 || cx == 0 || d == 0)
					return true;
			}
			return false;
		}

		public static int PickRandom(Random rnd, params int[] Selection)
		{
			return Selection[rnd.Next(Selection.Length)];
		}

		public static int PickRandom(Random rnd, int a, int b)
		{
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

		public static Rect getRectOfView(AlgeTilesImageView alIV)
		{
			RelativeLayout.LayoutParams rPrms = (RelativeLayout.LayoutParams)alIV.LayoutParameters;
			return new Rect(rPrms.LeftMargin, rPrms.TopMargin, rPrms.LeftMargin + rPrms.Width, rPrms.TopMargin + rPrms.Height);
		}

		public static int GCD(int p, int q)
		{
			if (q == 0)
			{
				return p;
			}

			int r = p % q;

			return GCD(q, r);
		}
	}
}