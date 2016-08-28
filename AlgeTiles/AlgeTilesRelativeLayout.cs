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

//http://stackoverflow.com/questions/22779422/custom-view-extending-relative-layout
//http://stackoverflow.com/questions/5292470/need-help-with-drawing-on-a-custom-made-view
//http://syedwasihaider.github.io/blog/2015/07/12/XamarinViews/
namespace AlgeTiles
{
	public class AlgeTilesRelativeLayout : RelativeLayout
	{
		private Paint paint;
		private bool bDrawRects = false;
		private bool bClearRects = false;
		private int height = 0;
		private int width = 0;
		private List<Rect> rList = new List<Rect>();

		public AlgeTilesRelativeLayout(Context context) :
			base(context)
		{
			paint = new Paint();
		}

		public AlgeTilesRelativeLayout(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
			paint = new Paint();
		}

		public AlgeTilesRelativeLayout(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
			paint = new Paint();
		}

		public void drawRects(List<RectTile> rectTiles)
		{
			foreach (RectTile rectTile in rectTiles)
				rList.Add(rectTile.getRect());

			bDrawRects = true;
		}

		public void clearRects(int height, int width)
		{
			bDrawRects = false;
			bClearRects = true;
			this.height = height;
			this.width = width;
			rList.Clear();
		}

		protected override void OnDraw(Canvas canvas)
		{
			if (bDrawRects)
			{
				paint.Color = Color.Black;
				paint.SetStyle(Paint.Style.Stroke);
				paint.SetPathEffect(new DashPathEffect(new float[] { 10, 20 }, 0));
				foreach(Rect r in rList)
					canvas.DrawRect(r, paint);
			} 

			if (bClearRects)
			{
				paint.Color = Color.White;
				paint.SetStyle(Paint.Style.Stroke);
				canvas.DrawRect(new Rect(0, 0, width, height), paint);
			}
		}
	}
}