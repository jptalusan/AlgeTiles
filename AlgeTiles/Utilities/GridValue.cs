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
	public class GridValue
	{
		//Add type (x, x2, 1)
		//Add group?
		int x2TileCount = 0;
		int y2TileCount = 0;
		int xyTileCount = 0;
		int xTileCount = 0;
		int yTileCount = 0;
		int oneTileCount = 0;

		public int x2Val
		{
			get
			{
				return x2TileCount;
			}
			set
			{
				x2TileCount = value;
			}
		}

		public int y2Val
		{
			get
			{
				return y2TileCount;
			}
			set
			{
				y2TileCount = value;
			}
		}

		public int xyVal
		{
			get
			{
				return xyTileCount;
			}
			set
			{
				xyTileCount = value;
			}
		}

		public int xVal
		{
			get
			{
				return xTileCount;
			}
			set
			{
				xTileCount = value;
			}
		}

		public int yVal
		{
			get
			{
				return yTileCount;
			}
			set
			{
				yTileCount = value;
			}
		}

		public int oneVal
		{
			get
			{
				return oneTileCount;
			}
			set
			{
				oneTileCount = value;
			}
		}

		public GridValue(int x2Count = 0, int y2Count = 0, int xyCount = 0,
							int xCount = 0, int yCount = 0, int oneCount = 0)
		{
			x2Val = x2Count;
			y2Val = y2Count;
			xyVal = xyCount;
			xVal = xCount;
			yVal = yCount;
			oneVal = oneCount;
		}

		public void init()
		{
			x2Val = 0;
			y2Val = 0;
			xyVal = 0;
			xVal = 0;
			yVal = 0;
			oneVal = 0;
		}

		public static GridValue operator -(GridValue g1, GridValue g2)
		{
			return new GridValue(g1.x2Val - g2.x2Val, g1.y2Val - g2.y2Val, g1.xyVal - g2.xyVal,
								g1.xVal - g2.xVal, g1.yVal - g2.yVal, g1.oneVal - g2.oneVal);
		}

		public static GridValue operator +(GridValue g1, GridValue g2)
		{
			return new GridValue(g1.x2Val + g2.x2Val, g1.y2Val + g2.y2Val, g1.xyVal + g2.xyVal,
								g1.xVal + g2.xVal, g1.yVal + g2.yVal, g1.oneVal + g2.oneVal);
		}

		public override string ToString()
		{
			return (String.Format("x2:{0}, y2:{1}, xy:{2}, x:{3}, y:{4}, one:{5}", x2Val, y2Val, xyVal, xVal, yVal, oneVal));
		}

		public int getCount()
		{
			return x2Val + y2Val + xyVal + xVal + yVal + oneVal;
		}
	}
}