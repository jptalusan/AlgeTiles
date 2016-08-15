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
				} else if (Constants.TWO_VAR == numberOfVariables)
				{
					int[] vars = new int[6];
					a = PickRandom(multipyTwoVarChoices);
					c = PickRandom(multipyTwoVarChoices);
					b = a < 0 ? PickRandom(-2 - (2 * a), 2) : PickRandom(-2, 2 - (2 * a));
					d = c < 0 ? PickRandom(-2 - (2 * c), 2) : PickRandom(-2, 2 - (2 * c));
					//TODO: Add more values for e and f generation
					vars[0] = a;
					vars[1] = b;
					vars[2] = c;
					vars[3] = d;
					return vars;
				}
			} else if (Constants.FACTOR == activityType)
			{
				//TODO: Not working just copied from multiply. multiply is working
				if (Constants.ONE_VAR == numberOfVariables)
				{
					int[] vars = new int[4];
					a = PickRandom(multipyOneVarChoices);
					c = PickRandom(multipyOneVarChoices);
					b = a >= 0 ? PickRandom(-9, 9 - (3 * a)) : PickRandom(-9 - (3 * a), 9);
					d = c >= 0 ? PickRandom(-9, 9 - (3 * c)) : PickRandom(-9 - (3 * c), 9);
					vars[0] = a;
					vars[1] = b;
					vars[2] = c;
					vars[3] = d;
					return vars;
				}
				else if (Constants.TWO_VAR == numberOfVariables)
				{
					int[] vars = new int[6];
					a = PickRandom(multipyTwoVarChoices);
					c = PickRandom(multipyTwoVarChoices);
					b = a >= 0 ? PickRandom(-2, 2 - (2 * a)) : PickRandom(-2 - (2 * a), 2);
					d = c >= 0 ? PickRandom(-2, 2 - (2 * c)) : PickRandom(-2 - (2 * c), 2);
					//TODO: Add more values for e and f generation
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