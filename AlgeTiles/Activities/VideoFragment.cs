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
using System.Threading.Tasks;
using Android.Content.Res;
using Android.Media;
using Android.Graphics;

//https://forums.xamarin.com/discussion/6375/raw-resource-file-access
//http://geekswithblogs.net/lorilalonde/archive/2015/07/22/video-playback-in-your-xamarin.android-apps---part-2-adding.aspx
namespace AlgeTiles.Activities
{
	public class VideoFragment : Android.Support.V4.App.Fragment
	{
		private static string TAG = "VideoFragment";
		private int id;

		public VideoFragment(int resource)
		{
			this.id = resource;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Log.Debug(TAG, "OnCreate");
			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.button_fragment, container, false);
			Button button = view.FindViewById<Button>(Resource.Id.button);

			if (id == Resource.Raw.var1)
			{
				button.Text = "Factor Tutorial Video";
			} else
			{
				button.Text = "Multiply Tutorial Video";
			}

			button.Click += buttonClick;
			return view;
		}

		private void buttonClick(object sender, EventArgs e)
		{
			var intent = new Intent(Activity, typeof(FactorVideo));
			intent.PutExtra(Constants.VIDEO_ID, id);
			StartActivity(intent);
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
		}
	}
}