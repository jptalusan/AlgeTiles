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

namespace AlgeTiles
{
	[Activity(Label = "AlgeTiles", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class HomeScreenActivity : Activity
	{
		private Button multiplyActivityButton;
		private Button factorActivityButton;
		private String activityType = "";
		private int variableCount= -1;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			ActionBar.Hide();
			SetContentView(Resource.Layout.HomeScreen);
			// Create your application here
			multiplyActivityButton = FindViewById<Button>(Resource.Id.multiply_button);
			multiplyActivityButton.Click += button_click;

			factorActivityButton = FindViewById<Button>(Resource.Id.factor_button);
			factorActivityButton.Click += button_click;
		}

		private void button_click(object sender, EventArgs e)
		{
			var button = (sender) as Button;
			activityType = button.Text;

			Button oneVarBtn = FindViewById<Button>(Resource.Id.one_variable_button);
			Button twoVarBtn = FindViewById<Button>(Resource.Id.two_variable_button);

			multiplyActivityButton.Visibility = ViewStates.Gone;
			factorActivityButton.Visibility = ViewStates.Gone;

			oneVarBtn.Visibility = ViewStates.Visible;
			twoVarBtn.Visibility = ViewStates.Visible;

			oneVarBtn.Click += var_click;
			twoVarBtn.Click += var_click;
		}

		private void var_click(object sender, EventArgs e)
		{
			var button = (sender) as Button;

			if ("Factor" == activityType)
			{
				var intent = new Intent(this, typeof(FactorActivity));
				intent.PutExtra(Constants.VARIABLE_COUNT, (int)Char.GetNumericValue(button.Text[0]));
				StartActivity(intent);
			}
			else if ("Multiply" == activityType)
			{
				var intent = new Intent(this, typeof(MultiplyActivity));
				intent.PutExtra(Constants.VARIABLE_COUNT, (int)Char.GetNumericValue(button.Text[0]));
				StartActivity(intent);
			}
		}
	}
}