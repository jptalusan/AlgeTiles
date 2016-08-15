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
	}
}