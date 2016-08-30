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
using Android.Graphics;

namespace AlgeTiles
{
	public class TileUtilities
	{
		private static string TAG = "AlgeTiles:TileUtilities";

		public static string getTileTypeOfProduct(string tile1, string tile2)
		{
			string output = "";
			if (tile1.Contains(Constants.X_TILE) &&
				tile2.Contains(Constants.X_TILE))
				output = Constants.X2_TILE;

			if (tile1.Contains(Constants.Y_TILE) &&
				tile2.Contains(Constants.Y_TILE))
				output = Constants.Y2_TILE;

			if ((tile1.Contains(Constants.X_TILE) &&
				tile2.Contains(Constants.Y_TILE)) ||
				(tile1.Contains(Constants.Y_TILE) &&
				tile2.Contains(Constants.X_TILE)))
				output = Constants.XY_TILE;

			if ((tile1.Contains(Constants.X_TILE) &&
				tile2.Contains(Constants.ONE_TILE)) ||
				(tile1.Contains(Constants.ONE_TILE) &&
				tile2.Contains(Constants.X_TILE)))
				output = Constants.X_TILE;

			if ((tile1.Contains(Constants.Y_TILE) &&
				tile2.Contains(Constants.ONE_TILE)) ||
				(tile1.Contains(Constants.ONE_TILE) &&
				tile2.Contains(Constants.Y_TILE)))
				output = Constants.Y_TILE;

			if (tile1.Contains(Constants.ONE_TILE) &&
				tile2.Contains(Constants.ONE_TILE))
				output = Constants.ONE_TILE;

			return output;
		}

		public class TileFactor
		{
			public int id = 0;
			public string text = "";
			public double heightFactor = 0;
			public double widthFactor = 0;

			public TileFactor()
			{

			}

			public TileFactor(int id, string text, double heightFactor, double widthFactor)
			{
				this.id = id;
				this.text = text;
				this.heightFactor = heightFactor;
				this.widthFactor = widthFactor;
			}
		}

		public static TileFactor getTileFactors(string tileType)
		{
			TileFactor tF = new TileFactor();

			switch (tileType)
			{
				case Constants.X2_TILE:
				case Constants.X2_TILE_ROT:
					tF.id = Resource.Drawable.x;
					tF.text = "x2";
					tF.heightFactor = Constants.X_LONG_SIDE;
					tF.widthFactor = Constants.X_LONG_SIDE;
					break;
				case Constants.Y2_TILE:
				case Constants.Y2_TILE_ROT:
					tF.id = Resource.Drawable.y;
					tF.text = "y2";
					tF.heightFactor = Constants.Y_LONG_SIDE;
					tF.widthFactor = Constants.Y_LONG_SIDE;
					break;
				case Constants.X_TILE:
					tF.id = Resource.Drawable.x;
					tF.text = "x";
					tF.heightFactor = Constants.X_LONG_SIDE;
					tF.widthFactor = Constants.ONE_SIDE;
					break;
				case Constants.Y_TILE:
					tF.id = Resource.Drawable.y;
					tF.text = "y";
					tF.heightFactor = Constants.Y_LONG_SIDE;
					tF.widthFactor = Constants.ONE_SIDE;
					break;
				case Constants.X_TILE_ROT:
					tF.id = Resource.Drawable.x;
					tF.text = "x";
					tF.heightFactor = Constants.ONE_SIDE;
					tF.widthFactor = Constants.X_LONG_SIDE;
					break;
				case Constants.Y_TILE_ROT:
					tF.id = Resource.Drawable.y;
					tF.text = "y";
					tF.heightFactor = Constants.ONE_SIDE;
					tF.widthFactor = Constants.Y_LONG_SIDE;
					break;
				case Constants.ONE_TILE:
				case Constants.ONE_TILE_ROT:
					tF.id = Resource.Drawable.one;
					tF.text = "1";
					tF.heightFactor = Constants.ONE_SIDE;
					tF.widthFactor = Constants.ONE_SIDE;
					break;
				case Constants.XY_TILE:
					tF.id = Resource.Drawable.xy;
					tF.text = "xy";
					tF.heightFactor = Constants.Y_LONG_SIDE;
					tF.widthFactor = Constants.X_LONG_SIDE;
					break;
				case Constants.XY_TILE_ROT:
					tF.id = Resource.Drawable.xy;
					tF.text = "xy";
					tF.heightFactor = Constants.X_LONG_SIDE;
					tF.widthFactor = Constants.Y_LONG_SIDE;
					break;
			}
			return tF;
		}

		public static int[] getDimensionsOfProduct(int height, string verticalTile, string horizontalTile)
		{
			int[] output = { 0, 0 };

			output[0] = (int) (height / getTileFactors(verticalTile).heightFactor);
			output[1] = (int) (height / getTileFactors(horizontalTile).heightFactor);

			return output;
		}

		public static void checkWhichParentAndUpdate(int id, string tile, int process, List<GridValue> gridValueList)
		{
			if (Constants.ADD == process)
			{
				//OUTER
				if (Resource.Id.upperLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++gridValueList[0].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++gridValueList[0].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++gridValueList[0].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++gridValueList[0].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++gridValueList[0].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++gridValueList[0].oneVal;
				}
				if (Resource.Id.upperRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++gridValueList[1].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++gridValueList[1].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++gridValueList[1].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++gridValueList[1].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++gridValueList[1].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++gridValueList[1].oneVal;
				}
				if (Resource.Id.lowerLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++gridValueList[2].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++gridValueList[2].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++gridValueList[2].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++gridValueList[2].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++gridValueList[2].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++gridValueList[2].oneVal;
				}
				if (Resource.Id.lowerRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++gridValueList[3].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++gridValueList[3].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++gridValueList[3].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++gridValueList[3].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++gridValueList[3].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++gridValueList[3].oneVal;
				}

				//CENTER
				if (Resource.Id.upperMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++gridValueList[4].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++gridValueList[4].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++gridValueList[4].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++gridValueList[4].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++gridValueList[4].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++gridValueList[4].oneVal;
				}
				if (Resource.Id.lowerMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++gridValueList[5].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++gridValueList[5].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++gridValueList[5].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++gridValueList[5].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++gridValueList[5].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++gridValueList[5].oneVal;
				}
				if (Resource.Id.middleLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++gridValueList[6].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++gridValueList[6].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++gridValueList[6].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++gridValueList[6].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++gridValueList[6].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++gridValueList[6].oneVal;
				}
				if (Resource.Id.middleRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++gridValueList[7].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++gridValueList[7].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++gridValueList[7].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++gridValueList[7].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++gridValueList[7].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++gridValueList[7].oneVal;
				}
			}
			//REMOVE
			else if (Constants.SUBTRACT == process)
			{
				//OUTER
				if (Resource.Id.upperLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--gridValueList[0].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--gridValueList[0].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--gridValueList[0].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--gridValueList[0].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--gridValueList[0].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--gridValueList[0].oneVal;
				}
				if (Resource.Id.upperRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--gridValueList[1].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--gridValueList[1].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--gridValueList[1].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--gridValueList[1].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--gridValueList[1].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--gridValueList[1].oneVal;
				}
				if (Resource.Id.lowerLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--gridValueList[2].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--gridValueList[2].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--gridValueList[2].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--gridValueList[2].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--gridValueList[2].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--gridValueList[2].oneVal;
				}
				if (Resource.Id.lowerRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--gridValueList[3].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--gridValueList[3].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--gridValueList[3].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--gridValueList[3].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--gridValueList[3].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--gridValueList[3].oneVal;
				}

				//CENTER
				if (Resource.Id.upperMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--gridValueList[4].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--gridValueList[4].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--gridValueList[4].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--gridValueList[4].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--gridValueList[4].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--gridValueList[4].oneVal;
				}
				if (Resource.Id.lowerMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--gridValueList[5].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--gridValueList[5].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--gridValueList[5].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--gridValueList[5].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--gridValueList[5].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--gridValueList[5].oneVal;
				}
				if (Resource.Id.middleLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--gridValueList[6].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--gridValueList[6].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--gridValueList[6].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--gridValueList[6].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--gridValueList[6].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--gridValueList[6].oneVal;
				}
				if (Resource.Id.middleRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--gridValueList[7].x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--gridValueList[7].y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--gridValueList[7].xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--gridValueList[7].xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--gridValueList[7].yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--gridValueList[7].oneVal;
				}
			}
		}

		public static Rect checkIfUserDropsOnRect(int vId, string tileType, 
			float x, float y, int command, 
			List<List<RectTile>> rectTileListList)
		{
			if (Resource.Id.upperRight == vId)
			{
				foreach (RectTile r in rectTileListList[0])
				{
					Log.Debug(TAG, "tiletype input: " + tileType + ", rect: " + r.getTileType());
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						if (Constants.ADD == command)
							r.setTilePresence(true);
						return r.getRect();
					}
					else if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && r.getTilePresence())
					{
						if (Constants.SUBTRACT == command)
						{
							r.setTilePresence(false);
							return null;
						}
					}
				}
			}

			if (Resource.Id.upperLeft == vId)
			{
				foreach (RectTile r in rectTileListList[1])
				{
					Log.Debug(TAG, "tiletype input: " + tileType + ", rect: " + r.getTileType());
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						if (Constants.ADD == command)
							r.setTilePresence(true);
						return r.getRect();
					}
					else if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && r.getTilePresence())
					{
						if (Constants.SUBTRACT == command)
						{
							r.setTilePresence(false);
							return null;
						}
					}
				}
			}

			if (Resource.Id.lowerLeft == vId)
			{
				foreach (RectTile r in rectTileListList[2])
				{
					Log.Debug(TAG, "tiletype input: " + tileType + ", rect: " + r.getTileType());
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						if (Constants.ADD == command)
							r.setTilePresence(true);
						return r.getRect();
					}
					else if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && r.getTilePresence())
					{
						if (Constants.SUBTRACT == command)
						{
							r.setTilePresence(false);
							return null;
						}
					}
				}
			}

			if (Resource.Id.lowerRight == vId)
			{
				foreach (RectTile r in rectTileListList[3])
				{
					Log.Debug(TAG, "tiletype input: " + tileType + ", rect: " + r.getTileType());
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						if (Constants.ADD == command)
							r.setTilePresence(true);
						return r.getRect();
					}
					else if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && r.getTilePresence())
					{
						if (Constants.SUBTRACT == command)
						{
							r.setTilePresence(false);
							return null;
						}
					}
				}
			}

			return null;
		}

		public static void generateInnerLayoutTileArrays(int heightInPx, int widthInPx, 
			List<ViewGroup> innerGridLayoutList, 
			List<List<RectTile>> rectTileListList)
		{
			Log.Debug(TAG, "generateInnerLayoutTileArrays");
			List<string> midUp = new List<string>();
			List<string> midLeft = new List<string>();
			List<string> midRight = new List<string>();
			List<string> midDown = new List<string>();

			List<List<string>> output = new List<List<string>>();
			output.Add(midUp);
			output.Add(midLeft);
			output.Add(midRight);
			output.Add(midDown);

			//midup, midleft, midright, middown
			for (int i = 0; i < innerGridLayoutList.Count; ++i)
			{
				Log.Debug(TAG, i + "");
				GridLayout gl = (GridLayout)innerGridLayoutList[i];
				for (int j = 0; j < gl.ChildCount; ++j)
				{
					AlgeTilesTextView al = gl.GetChildAt(j) as AlgeTilesTextView;
					Log.Debug(TAG, al.getTileType());

					switch (al.getTileType())
					{
						case Constants.X_TILE:
						case Constants.X_TILE_ROT:
							output[i].Add(al.getTileType());
							break;
						case Constants.Y_TILE:
						case Constants.Y_TILE_ROT:
							output[i].Add(al.getTileType());
							break;
						case Constants.ONE_TILE:
						case Constants.ONE_TILE_ROT:
							output[i].Add(al.getTileType());
							break;
					}
				}
			}

			int height = heightInPx;
			int width = widthInPx;

			//upmid x midRight = quadrant1
			if (midUp.Count != 0 || midRight.Count != 0)
			{
				Log.Debug(TAG, "in upmid x midright");
				int top = height; //height of relative layout
				int bottom = height; //height of relative layout
				for (int i = 0; i < midUp.Count; ++i)
				{
					int left = 0;
					int right = 0;
					bool firstPass = true;
					for (int j = 0; j < midRight.Count; ++j)
					{
						int[] productDimensions = getDimensionsOfProduct(height, midUp[i], midRight[j]);
						if (firstPass)
						{
							//top = subtract height of first tile in midup ( then subtract next tile  ) etc...
							top -= productDimensions[0];
							//bottom = height at i = 0, else bottom = previous top
							bottom = top + productDimensions[0]; //don't add to stack since only getting the latest top/height
							firstPass = false;
						}
						//right = width of midleft (then add next tile) etc...
						right += productDimensions[1]; //width adds up
													   //left = 0 at start, else width of midleft
						left = right - productDimensions[1];

						Rect r = new Rect(left, top, right, bottom);
						rectTileListList[0].Add(new RectTile(r, getTileTypeOfProduct(midUp[i], midRight[j])));
					}
				}
			}

			//upmid x midLeft = quadrant2
			if (midUp.Count != 0 || midLeft.Count != 0)
			{
				Log.Debug(TAG, "in upmid x midLeft");
				int top = height;
				int bottom = height;
				for (int i = 0; i < midUp.Count; ++i)
				{
					int left = width;
					int right = width;
					bool firstPass = true;
					for (int j = 0; j < midLeft.Count; ++j)
					{
						int[] productDimensions = getDimensionsOfProduct(height, midUp[i], midLeft[j]);
						if (firstPass)
						{
							top -= productDimensions[0];
							bottom = top + productDimensions[0];
							firstPass = false;
						}
						left -= productDimensions[1];
						right = left + productDimensions[1];

						Rect r = new Rect(left, top, right, bottom);
						rectTileListList[1].Add(new RectTile(r, getTileTypeOfProduct(midUp[i], midLeft[j])));
					}
				}
			}

			//loMid x midLeft = quadrant3
			if (midDown.Count != 0 || midLeft.Count != 0)
			{
				Log.Debug(TAG, "in loMid x midLeft");
				int top = 0;
				int bottom = 0;
				for (int i = 0; i < midDown.Count; ++i)
				{
					int left = width;
					int right = width;
					bool firstPass = true;
					for (int j = 0; j < midLeft.Count; ++j)
					{
						int[] productDimensions = getDimensionsOfProduct(height, midDown[i], midLeft[j]);
						if (firstPass)
						{
							bottom += productDimensions[0];
							top = bottom - productDimensions[0];
							firstPass = false;
						}
						left -= productDimensions[1];
						right = left + productDimensions[1];

						Rect r = new Rect(left, top, right, bottom);
						rectTileListList[2].Add(new RectTile(r, getTileTypeOfProduct(midDown[i], midLeft[j])));
					}
				}
			}

			//loMid x midRight = quadrant4
			if (midDown.Count != 0 || midRight.Count != 0)
			{
				Log.Debug(TAG, "in loMid x midRight");
				int top = 0;
				int bottom = 0;
				for (int i = 0; i < midDown.Count; ++i)
				{
					int left = 0;
					int right = 0;
					bool firstPass = true;
					for (int j = 0; j < midRight.Count; ++j)
					{
						int[] productDimensions = getDimensionsOfProduct(height, midDown[i], midRight[j]);
						if (firstPass)
						{
							bottom += productDimensions[0];
							top = bottom - productDimensions[0];
							firstPass = false;
						}
						right += productDimensions[1];
						left = right - productDimensions[1];
						Rect r = new Rect(left, top, right, bottom);
						rectTileListList[3].Add(new RectTile(r, getTileTypeOfProduct(midDown[i], midRight[j])));
					}
				}
			}
		}
	}
}