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
	public class AlgeTilesTextView : TextView
	{
		private string tileType;
		
		public AlgeTilesTextView(Context context) :
            base(context)
        {
			this.TextAlignment = TextAlignment.Center;
			this.Gravity = GravityFlags.Center;
			this.SetTextColor(Color.Black);
		}
		public AlgeTilesTextView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {

		}

		public AlgeTilesTextView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {

		}

		public void setTileType(string tileType)
		{
			this.tileType = tileType;
		}

		public string getTileType()
		{
			return tileType;
		}
	}
}