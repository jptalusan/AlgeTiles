using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using System.Threading.Tasks;

//http://stackoverflow.com/questions/9838301/how-can-i-make-my-button-to-do-something-in-fragments-viewpager
namespace AlgeTiles.Activities
{
	public class TutorialFragment : Android.Support.V4.App.Fragment
	{
		int IDIMG { get; set; }
		ImageView iv;

		public TutorialFragment(int id)
		{
			IDIMG = id;
		}

		public void setIDIMG(int id)
		{
			IDIMG = id;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.tutorial_fragment, container, false);
			return view;
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			var metrics = Resources.DisplayMetrics;
			var widthInDp = ConvertPixelsToDp(metrics.WidthPixels/2);
			var heightInDp = ConvertPixelsToDp(metrics.HeightPixels/2);
			iv = (ImageView)view.FindViewById(Resource.Id.imageview_card);
			var t = Task.Run(async () =>
			{
				iv.SetImageBitmap(await LocalImageService.LoadDrawableAsync(Resources, IDIMG, widthInDp, heightInDp));
			});
			t.Wait();
			//Button clicks?
		}

		public override void OnResume()
		{
			base.OnResume();
		}

		private int ConvertPixelsToDp(float pixelValue)
		{
			var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
			return dp;
		}
	}
}