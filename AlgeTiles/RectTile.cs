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
using Android.Graphics;

namespace AlgeTiles
{
	public class RectTile
	{
		private Rect r;
		private string tileType;
		private int height;
		private int width;
		private bool alreadyHasTile = false;

		public RectTile(Rect r, string tileType)
		{
			this.r = r;
			height = r.Top - r.Bottom;
			width = r.Right - r.Left;
			this.tileType = tileType;
		}

		public string getTileType()
		{
			return tileType;
		}

		public int getHeight()
		{
			return height;
		}

		public int getWidth()
		{
			return width;
		}

		public Rect getRect()
		{
			return r;
		}

		public void setTilePresence(bool b)
		{
			alreadyHasTile = b;
		}

		public bool getTilePresence()
		{
			return alreadyHasTile;
		}

		public bool isPointInsideRect(float x, float y)
		{
			if (y > r.Top && y < r.Bottom && x > r.Left && x < r.Right)
				return true;
			return false;
		}

		public bool isTileTypeSame(string tileType)
		{
			if (this.tileType.Contains(tileType) || tileType.Contains(this.tileType))
				return true;
			return false;
		}
	}
}