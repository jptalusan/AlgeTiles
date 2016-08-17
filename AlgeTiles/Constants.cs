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

namespace AlgeTiles
{
	class Constants
	{
		public static string FACTOR = "Factor";
		public static string MULTIPLY = "Multiply";
		public static int ONE_VAR = 1;
		public static int TWO_VAR = 2;
		public static string NEW_Q = "new Q";
		public static string CHK = "chk";
		public static string REFR = "refr";

		public static string DRAG = "drag";
		public static string REMOVE = "remove";
		public static string ROTATE = "rotate";

		public static string VARIABLE_COUNT = "variable_count";

		public static string ONE_TILE = "tile_1";
		public static string X_TILE = "x_tile";
		public static string X2_TILE = "x2_tile";

		public static string ONE_TILE_ROT = "tile_1_rot";
		public static string X_TILE_ROT = "x_tile_rot";
		public static string X2_TILE_ROT = "x2_tile_rot";
		//TODO: Add for y, xy, x2 and rotated versions (probably just for y and xy)

		public static int SUBTRACT = 0;
		public static int ADD = 1;
		public static int DELAY = 1500;
	}
}