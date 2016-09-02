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

namespace AlgeTiles.Activities
{
	public class TutorialFragment : Android.Support.V4.App.Fragment
	{
		int IDIMG { get; set; }
		public TutorialFragment(int id)
		{
			IDIMG = id;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.tutorial_fragment, container, false);
			((ImageView)view.FindViewById(Resource.Id.imageview_card)).SetImageResource(IDIMG);
			return view;
		}
	}
}