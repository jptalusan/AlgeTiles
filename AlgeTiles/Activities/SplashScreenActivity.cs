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
using static Android.Views.Animations.Animation;
using Android.Views.Animations;
using static Android.Widget.ImageView;

namespace AlgeTiles.Activities
{
	[Activity(Label = "SplashScreenActivity", MainLauncher = false, Icon = "@drawable/ic_launcher", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Theme = "@style/MyCustomTheme")]
	public class SplashScreenActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			SetContentView(Resource.Layout.SplashScreen);

            //showImages();

            var intent = new Intent(this, typeof(HomeScreenActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
            // Create your application here
        }

		private void showImages()
		{
			LinearLayout layoutBase = FindViewById<LinearLayout>(Resource.Id.llayout);
			ImageView img = new ImageView(this);
			img.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			img.Visibility = ViewStates.Visible;
			img.SetBackgroundResource(Resource.Drawable.ateneo);
			img.SetScaleType(ScaleType.FitXy);
			layoutBase.AddView(img);
		}
	}
}