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
using Android.Content.Res;

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
			TypedArray a = context.Theme.ObtainStyledAttributes(
				attrs,
				Resource.Styleable.AlgeTilesTextView, 0, 0);

			this.tileType = a.GetString(Resource.Styleable.AlgeTilesTextView_tileType);
			this.TextAlignment = TextAlignment.Center;
			this.Gravity = GravityFlags.Center;
			this.SetTextColor(Color.Black);
			a.Recycle();
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