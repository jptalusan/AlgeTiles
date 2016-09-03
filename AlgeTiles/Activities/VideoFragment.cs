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
		private VideoView vv;
		private MediaController mediaController;
		private int id;
		public VideoFragment(int resource)
		{
			this.id = resource;
		}
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.video_fragment, container, false);
			return view;
		}

		public override void OnViewCreated(View view, Bundle savedInstanceState)
		{
			base.OnViewCreated(view, savedInstanceState);
			var metrics = Resources.DisplayMetrics;
			vv = (VideoView)view.FindViewById(Resource.Id.video);
			mediaController = new MediaController(Activity, true);
			vv.SetMediaController(mediaController);
			SetMenuVisibility(true);
		}

		public override void OnResume()
		{
			base.OnResume();
			vv.Prepared += OnVideoPlayerPrepared;
			vv.Start();
		}

		public override void OnPause()
		{
			base.OnPause();
			vv.Prepared -= OnVideoPlayerPrepared;
			vv.StopPlayback();
			mediaController.Hide();
		}

		public override void OnStart()
		{
			base.OnStart();
			vv.Prepared += OnVideoPlayerPrepared;
			LaunchVideo();
		}

		public override void OnStop()
		{
			base.OnStop();
			vv.Prepared -= OnVideoPlayerPrepared;
			vv.StopPlayback();
			mediaController.Hide();
		}

		public override void OnHiddenChanged(bool hidden)
		{
			base.OnHiddenChanged(hidden);
			if (hidden)
			{
				UserVisibleHint = false;
				vv.Pause();
			}
			else
			{
				UserVisibleHint = true;
				LaunchVideo();
			}
		}

		public override bool UserVisibleHint
		{
			get
			{
				return base.UserVisibleHint;
			}

			set
			{
				base.UserVisibleHint = value;
			}
		}

		public override void SetMenuVisibility(bool menuVisible)
		{
			base.SetMenuVisibility(menuVisible);
		}

		private void OnVideoPlayerPrepared(object sender, EventArgs e)
		{
			mediaController.SetAnchorView(vv);

			//show media controls for 3 seconds when video starts to play
			mediaController.Show(3000);
		}

		private void LaunchVideo()
		{
			String uriPath = "android.resource://AlgeTiles.AlgeTiles/" + id;
			var uri = Android.Net.Uri.Parse(uriPath);
			vv.SetVideoURI(uri);
			vv.RequestFocus();
			vv.Start();
		}
	}
}