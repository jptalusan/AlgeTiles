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
		public const string FACTOR = "Factor";
		public const string MULTIPLY = "Multiply";
		public const int ONE_VAR = 1;
		public const int TWO_VAR = 2;
		public const string NEW_Q = "new Q";
		public const string CHK = "chk";
		public const string REFR = "refr";
		public const string MUTE = "mute";
		public const string FIRST_TIME = "firsttime";

		public const string DRAG = "drag";
		public const string REMOVE = "remove";
		public const string ROTATE = "rotate";

		public const string VARIABLE_COUNT = "variable_count";
		public const string VIDEO_ID = "video_id";

		public const string ONE_TILE = "tile_1";
		public const string X_TILE = "x_tile";
		public const string Y_TILE = "y_tile";
		public const string X2_TILE = "x2_tile";
		public const string Y2_TILE = "y2_tile";
		public const string XY_TILE = "xy_tile";

		public const string ONE_TILE_ROT = "tile_1_rot";
		public const string X_TILE_ROT = "x_tile_rot";
		public const string Y_TILE_ROT = "y_tile_rot";
		public const string X2_TILE_ROT = "x2_tile_rot";
		public const string Y2_TILE_ROT = "y2_tile_rot";
		public const string XY_TILE_ROT = "xy_tile_rot";

		public const string BUTTON_TYPE = "BUTTON_TYPE";
		public const string CLONED_BUTTON = "CLONE_BUTTON";
		public const string ORIGINAL_BUTTON = "ORIGINAL_BUTTON";
		//TODO:const  Add for y, xy, x2 and rotated versions (probably just for y and xy)

		public const int SUBTRACT = 0;
		public const int ADD = 1;
		public const int DELAY = 1500;
		public const int CANCELOUT_DELAY = 1500;

		public const double SNAP_GRID_INTERVAL = 9.0;

		public const double X_LONG_SIDE = 3.0;
		public const double ONE_SIDE = 9.0;
		public const double Y_LONG_SIDE = 2.6;

		public const string PROCEED = "Proceed to";
		public const string CORRECT = "Correct";
		public const string WRONG = "Wrong";
		public const string MULTIPLICATION= " multiplication";
		public const string COEFFICIENTS = " coefficients";

		public const string CORRECT_PLACEMENT = "Correct Placement of Tiles";
		public const string PROCEED_TO_FACTOR = "Proceed to Factoring";
		public const string PROCEED_TO_MULTIP = "Proceed to Multiplication";
		public const string CORRECT_FACTORS = "Correct Factors";
		public const string CORRECT_MULTIP = "Correct Multiplication";
		public const string PROCEED_TO_COEFF = "Proceed to coefficients";


		//Equations
		public static List<string> EQUATIONS = new List<string>
		{
			EQUATION_001,
			EQUATION_002,
			EQUATION_003,
			EQUATION_004,
			EQUATION_005
		};

		public const string EQUATION_001 = "1,-2,1,3";
		public const string EQUATION_002 = "-2,5,-3,7";
		public const string EQUATION_003 = "-2,5,-1,1";
		public const string EQUATION_004 = "1,4,3,-2";
		public const string EQUATION_005 = "2,3,1,-4";
	}
}