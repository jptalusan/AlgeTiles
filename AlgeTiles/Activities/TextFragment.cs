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
	public class TextFragment : Android.Support.V4.App.Fragment
	{
		private string input = "";
		private TextView tv;
		private Button button;

		public TextFragment(string input)
		{
			this.input = input;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.text_fragment, container, false);
			tv = view.FindViewById<TextView>(Resource.Id.text);
			button = view.FindViewById<Button>(Resource.Id.button);

			tv.Text = input;

			if (input.Contains("END"))
			{
				button.Visibility = ViewStates.Visible;
				button.Text = "Return to Main Menu";
				button.Click += buttonClick;
			}
			return view;
		}

		private void buttonClick(object sender, EventArgs e)
		{
			Activity.Finish();
		}
	}
}