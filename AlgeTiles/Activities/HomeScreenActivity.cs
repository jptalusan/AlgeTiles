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
using Android.Preferences;
using AlgeTiles.Activities;

namespace AlgeTiles
{
	[Activity(Label = "AlgeTiles", MainLauncher = true, Icon = "@drawable/ic_launcher", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class HomeScreenActivity : Activity
	{
		private Button multiplyActivityButton;
		private Button factorActivityButton;
		private Button factorTutorialButton;
		private Button multiplyTutorialButton;
		private Button generalTutorialButton;
		private String activityType = "";
		private Button oneVarBtn;
		private Button twoVarBtn;
		private bool cameFromTutorial = false;
		public ISharedPreferences prefs;
		public string UserName { get; set; }
		internal Context PackageContext { get; set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			ActionBar.Hide();
			SetContentView(Resource.Layout.HomeScreen);

			prefs = PreferenceManager.GetDefaultSharedPreferences(this);

			multiplyActivityButton = FindViewById<Button>(Resource.Id.multiply_button);
			multiplyActivityButton.Click += button_click;

			factorActivityButton = FindViewById<Button>(Resource.Id.factor_button);
			factorActivityButton.Click += button_click;

			factorTutorialButton = FindViewById<Button>(Resource.Id.factorTutorialButton);
			multiplyTutorialButton = FindViewById<Button>(Resource.Id.multiplyTutorialButton);
			generalTutorialButton = FindViewById<Button>(Resource.Id.generalTutorialButton);
			factorTutorialButton.Click += tutorial_button_click;
			multiplyTutorialButton.Click += tutorial_button_click;
			generalTutorialButton.Click += tutorial_button_click;

			oneVarBtn = FindViewById<Button>(Resource.Id.one_variable_button);
			twoVarBtn = FindViewById<Button>(Resource.Id.two_variable_button);

			if (prefs.GetBoolean(Constants.FIRST_TIME, true))
			{
				prefs.Edit().PutBoolean(Constants.FIRST_TIME, false).Apply();

				var intent = new Intent(this, typeof(TutorialActivity));
				intent.AddFlags(ActivityFlags.ClearTop);
				StartActivity(intent);
				cameFromTutorial = true;
			}
		}

		private void tutorial_button_click(object sender, EventArgs e)
		{
			View clicked_toggle = (sender) as View;
			int buttonText = clicked_toggle.Id;
			switch (buttonText)
			{
				case Resource.Id.factorTutorialButton:
					var factorIntent = new Intent(this, typeof(TutorialActivity));
					factorIntent.PutExtra("TutoriialType", Constants.FACTOR);
					factorIntent.AddFlags(ActivityFlags.ClearTop);
					StartActivity(factorIntent);
					break;
				case Resource.Id.multiplyTutorialButton:
					var multiplyIntent = new Intent(this, typeof(TutorialActivity));
					multiplyIntent.PutExtra("TutoriialType", Constants.MULTIPLY);
					multiplyIntent.AddFlags(ActivityFlags.ClearTop);
					StartActivity(multiplyIntent);
					break;
				case Resource.Id.generalTutorialButton:
					var intent = new Intent(this, typeof(TutorialActivity));
					intent.PutExtra("TutoriialType", "General");
					intent.AddFlags(ActivityFlags.ClearTop);
					StartActivity(intent);
					break;
				default:
					break;
			}
		}

		private void button_click(object sender, EventArgs e)
		{
			var button = (sender) as Button;
			activityType = button.Text;

			multiplyActivityButton.Visibility = ViewStates.Gone;
			factorActivityButton.Visibility = ViewStates.Gone;
			factorTutorialButton.Visibility = ViewStates.Invisible;
			multiplyTutorialButton.Visibility = ViewStates.Invisible;
			generalTutorialButton.Visibility = ViewStates.Invisible;

			oneVarBtn.Visibility = ViewStates.Visible;
			if (Constants.FACTOR == button.Text) //Since Factor only uses 1 variable
				twoVarBtn.Visibility = ViewStates.Invisible;
			else
				twoVarBtn.Visibility = ViewStates.Visible;

			oneVarBtn.Click += var_click;
			twoVarBtn.Click += var_click;
		}

		private void var_click(object sender, EventArgs e)
		{
			var button = (sender) as Button;

			if (Constants.FACTOR == activityType)
			{
				var intent = new Intent(this, typeof(FactorActivity));
				if ((int)Char.GetNumericValue(button.Text[0]) == 1)
				{
					intent.PutExtra(Constants.VARIABLE_COUNT, (int)Char.GetNumericValue(button.Text[0]));
					intent.AddFlags(ActivityFlags.ClearTop);
				}
				else
					Toast.MakeText(Application.Context, "Not implemented.", ToastLength.Short).Show();
				StartActivity(intent);
			}
			else if (Constants.MULTIPLY == activityType && Constants.ONE_VAR == (int)Char.GetNumericValue(button.Text[0]))
			{
				var intent = new Intent(this, typeof(MultiplyActivity));
				intent.PutExtra(Constants.VARIABLE_COUNT, (int)Char.GetNumericValue(button.Text[0]));
				intent.AddFlags(ActivityFlags.ClearTop);
				StartActivity(intent);
			}
			else if (Constants.MULTIPLY == activityType && Constants.TWO_VAR == (int)Char.GetNumericValue(button.Text[0]))
			{
				var intent = new Intent(this, typeof(MultiplyTwoVarActivity));
				intent.PutExtra(Constants.VARIABLE_COUNT, (int)Char.GetNumericValue(button.Text[0]));
				intent.AddFlags(ActivityFlags.ClearTop);
				StartActivity(intent);
			}
		}

		public override void OnBackPressed()
		{
			if (!cameFromTutorial)
			{
				if (multiplyActivityButton.Visibility == ViewStates.Gone)
				{
					multiplyActivityButton.Visibility = ViewStates.Visible;
					factorActivityButton.Visibility = ViewStates.Visible;
					factorTutorialButton.Visibility = ViewStates.Visible;
					multiplyTutorialButton.Visibility = ViewStates.Visible;
					generalTutorialButton.Visibility = ViewStates.Visible;

					oneVarBtn.Visibility = ViewStates.Gone;
					twoVarBtn.Visibility = ViewStates.Gone;
				}
				else
				{
					Finish();
				}
			} else
			{
				Finish();
			}
		}
	}
}