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
	public class AlgeTilesImageView : ImageView
	{
		private string tileType;
		private int resID;
		
		public AlgeTilesImageView(Context context) :
            base(context)
        {

		}
		public AlgeTilesImageView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {

		}

		public AlgeTilesImageView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {

		}

		public void setImageResource(int resId)
		{
			this.resID = resId;
			base.SetImageResource(resId);
		}

		public int getResourceId()
		{
			return resID;
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