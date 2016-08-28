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
	public class TileUtilities
	{
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

		public static double[] getTileFactors(string tileType)
		{
			double[] output = { 0, 0 }; //height x width
			double heightFactor = 0;
			double widthFactor = 0;
			switch (tileType)
			{
				case Constants.X2_TILE:
				case Constants.X2_TILE_ROT:
					heightFactor = Constants.X_LONG_SIDE;
					widthFactor = Constants.X_LONG_SIDE;
					break;
				case Constants.Y2_TILE:
				case Constants.Y2_TILE_ROT:
					heightFactor = Constants.Y_LONG_SIDE;
					widthFactor = Constants.Y_LONG_SIDE;
					break;
				case Constants.X_TILE:
					heightFactor = Constants.X_LONG_SIDE;
					widthFactor = Constants.ONE_SIDE;
					break;
				case Constants.Y_TILE:
					heightFactor = Constants.Y_LONG_SIDE;
					widthFactor = Constants.ONE_SIDE;
					break;
				case Constants.X_TILE_ROT:
					heightFactor = Constants.X_LONG_SIDE;
					widthFactor = Constants.ONE_SIDE;
					break;
				case Constants.Y_TILE_ROT:
					heightFactor = Constants.ONE_SIDE;
					widthFactor = Constants.Y_LONG_SIDE;
					break;
				case Constants.ONE_TILE:
				case Constants.ONE_TILE_ROT:
					heightFactor = Constants.ONE_SIDE;
					widthFactor = Constants.ONE_SIDE;
					break;
				case Constants.XY_TILE:
					heightFactor = Constants.Y_LONG_SIDE;
					widthFactor = Constants.X_LONG_SIDE;
					break;
				case Constants.XY_TILE_ROT:
					heightFactor = Constants.X_LONG_SIDE;
					widthFactor = Constants.Y_LONG_SIDE;
					break;
			}

			output[0] = heightFactor;
			output[1] = widthFactor;
			return output;
		}

		public static int[] getDimensionsOfProduct(int height, string verticalTile, string horizontalTile)
		{
			int[] output = { 0, 0 };
			double[] verticalTileFactors = getTileFactors(verticalTile);
			double[] horizontalTileFactors = getTileFactors(horizontalTile);
			output[0] = (int) (height / verticalTileFactors[0]);
			output[1] = (int) (height / horizontalTileFactors[0]);

			return output;
		}
	}
}