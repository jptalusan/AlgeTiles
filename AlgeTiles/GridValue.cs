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
	class GridValue
	{
		//Add type (x, x2, 1)
		//Add group?
		int x2TileCount = 0;
		int xTileCount = 0;
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

		public GridValue(int x2Count = 0, int xCount = 0, int oneCount = 0)
		{
			x2Val = x2Count;
			xVal = xCount;
			oneVal = oneCount;
		}

		public void init()
		{
			x2Val = 0;
			xVal = 0;
			oneVal = 0;
		}

		public static GridValue operator -(GridValue g1, GridValue g2)
		{
			return new GridValue(g1.x2Val - g2.x2Val, g1.xVal - g2.xVal, g1.oneVal - g2.oneVal);
		}

		public static GridValue operator +(GridValue g1, GridValue g2)
		{
			return new GridValue(g1.x2Val + g2.x2Val, g1.xVal + g2.xVal, g1.oneVal + g2.oneVal);
		}

		public override string ToString()
		{
			return (String.Format("{0}:x2, {1}:x, {2}:1", x2Val, xVal, oneVal));
		}
	}
}