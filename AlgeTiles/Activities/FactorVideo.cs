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

namespace AlgeTiles.Activities
{
	[Activity(Label = "FactorVideo", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class FactorVideo : Activity
	{
		private VideoView vv;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			ActionBar.Hide();
			SetContentView(Resource.Layout.video_fragment);
			int id = Intent.GetIntExtra(Constants.VIDEO_ID, 0);
			vv = (VideoView)FindViewById(Resource.Id.video);
			MediaController mediaController = new MediaController(this, true);
			vv.SetMediaController(mediaController);
			String uriPath = "android.resource://AlgeTiles.AlgeTiles/" + id;
			var uri = Android.Net.Uri.Parse(uriPath);
			mediaController.SetAnchorView(vv);
			//mediaController.Show(2000);
			vv.SetVideoURI(uri);
			vv.RequestFocus();
			vv.Start();
			// Create your application here
		}
	}
}