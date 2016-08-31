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
using Android.Media;
using System.Threading.Tasks;
using Android.Preferences;

namespace AlgeTiles
{
	[Activity(Label = "MultiplyTwoVarActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class MultiplyTwoVarActivity : AlgeTilesActivity
	{
		private static string TAG = "AlgeTiles:MultiplyTwoVarActivity";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			ActionBar.Hide();

			numberOfVariables = Intent.GetIntExtra(Constants.VARIABLE_COUNT, 0);

			SetContentView(Resource.Layout.MultiplyTwoVar);

			// Create your application here
			activityType = Constants.MULTIPLY;
			listeners = new Listeners(this);

			result = (TextView)FindViewById(Resource.Id.result);

			tile_1 = (ImageButton)FindViewById(Resource.Id.tile_1);
			x_tile = (ImageButton)FindViewById(Resource.Id.x_tile);
			y_tile = (ImageButton)FindViewById(Resource.Id.y_tile);
			xy_tile = (ImageButton)FindViewById(Resource.Id.xy_tile);
			x2_tile = (ImageButton)FindViewById(Resource.Id.x2_tile);
			y2_tile = (ImageButton)FindViewById(Resource.Id.y2_tile);

			tile_1_rot = (ImageButton)FindViewById(Resource.Id.tile_1_rot);
			x_tile_rot = (ImageButton)FindViewById(Resource.Id.x_tile_rot);
			y_tile_rot = (ImageButton)FindViewById(Resource.Id.y_tile_rot);
			xy_tile_rot = (ImageButton)FindViewById(Resource.Id.xy_tile_rot);

			tile_1.LongClick += listeners.tile_LongClick;
			x_tile.LongClick += listeners.tile_LongClick;
			y_tile.LongClick += listeners.tile_LongClick;
			x2_tile.LongClick += listeners.tile_LongClick;
			y2_tile.LongClick += listeners.tile_LongClick;
			xy_tile.LongClick += listeners.tile_LongClick;

			tile_1_rot.LongClick += listeners.tile_LongClick;
			x_tile_rot.LongClick += listeners.tile_LongClick;
			y_tile_rot.LongClick += listeners.tile_LongClick;
			xy_tile_rot.LongClick += listeners.tile_LongClick;

			upperLeftGrid = FindViewById<AlgeTilesRelativeLayout>(Resource.Id.upperLeft);
			upperRightGrid = FindViewById<AlgeTilesRelativeLayout>(Resource.Id.upperRight);
			lowerLeftGrid = FindViewById<AlgeTilesRelativeLayout>(Resource.Id.lowerLeft);
			lowerRightGrid = FindViewById<AlgeTilesRelativeLayout>(Resource.Id.lowerRight);

			upperMiddleGrid = FindViewById<GridLayout>(Resource.Id.upperMiddle);
			middleLeftGrid = FindViewById<GridLayout>(Resource.Id.middleLeft);
			middleRightGrid = FindViewById<GridLayout>(Resource.Id.middleRight);
			lowerMiddleGrid = FindViewById<GridLayout>(Resource.Id.lowerMiddle);

			ViewTreeObserver vto2 = upperLeftGrid.ViewTreeObserver;
			vto2.GlobalLayout += (sender, e) =>
			{
				if (!isFirstTime)
				{
					heightInPx = upperLeftGrid.Height;
					widthInPx = upperLeftGrid.Width;
					upperLeftGrid.SetMinimumHeight(0);
					upperLeftGrid.SetMinimumWidth(0);
					isFirstTime = true;
				}
			};

			outerGridLayoutList.Add(upperLeftGrid);
			outerGridLayoutList.Add(upperRightGrid);
			outerGridLayoutList.Add(lowerLeftGrid);
			outerGridLayoutList.Add(lowerRightGrid);

			innerGridLayoutList.Add(upperMiddleGrid);
			innerGridLayoutList.Add(middleLeftGrid);
			innerGridLayoutList.Add(middleRightGrid);
			innerGridLayoutList.Add(lowerMiddleGrid);

			//For multiply this is the initial grid available
			//Together form one Part of the formula
			upperMiddleGrid.Drag += listeners.GridLayout_Drag;
			lowerMiddleGrid.Drag += listeners.GridLayout_Drag;

			//Together form one Part of the formula
			middleLeftGrid.Drag += listeners.GridLayout_Drag;
			middleRightGrid.Drag += listeners.GridLayout_Drag;

			//Shade red the other grids
			for (int i = 0; i < outerGridLayoutList.Count; ++i)
				outerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.unavailable);

			removeToggle = (ToggleButton)FindViewById(Resource.Id.remove);
			dragToggle = (ToggleButton)FindViewById(Resource.Id.drag);
			rotateToggle = (ToggleButton)FindViewById(Resource.Id.rotate);
			muteToggle = (ToggleButton)FindViewById(Resource.Id.mute);

			removeToggle.Click += listeners.toggle_click;
			dragToggle.Click += listeners.toggle_click;
			rotateToggle.Click += listeners.toggle_click;
			muteToggle.Click += listeners.toggle_click;

			prefs = PreferenceManager.GetDefaultSharedPreferences(this);
			muteToggle.Checked = prefs.GetBoolean(Constants.MUTE, false);

			newQuestionButton = (Button)FindViewById<Button>(Resource.Id.new_question_button);
			refreshButton = (Button)FindViewById<Button>(Resource.Id.refresh_button);
			checkButton = (Button)FindViewById<Button>(Resource.Id.check_button);

			newQuestionButton.Click += button_click;
			refreshButton.Click += button_click;
			checkButton.Click += button_click;

			upperLeftGV = new GridValue();
			upperRightGV = new GridValue();
			lowerLeftGV = new GridValue();
			lowerRightGV = new GridValue();

			midUpGV = new GridValue();
			midLowGV = new GridValue();
			midLeftGV = new GridValue();
			midRightGV = new GridValue();

			gridValueList.Add(upperLeftGV);
			gridValueList.Add(upperRightGV);
			gridValueList.Add(lowerLeftGV);
			gridValueList.Add(lowerRightGV);

			gridValueList.Add(midUpGV);
			gridValueList.Add(midLowGV);
			gridValueList.Add(midLeftGV);
			gridValueList.Add(midRightGV);

			setupNewQuestion();

			correct = MediaPlayer.Create(this, Resource.Raw.correct);
			incorrect = MediaPlayer.Create(this, Resource.Raw.wrong);

			x2ET = FindViewById<EditText>(Resource.Id.x2_value);
			y2ET = FindViewById<EditText>(Resource.Id.y2_value);
			xyET = FindViewById<EditText>(Resource.Id.xy_value);
			xET = FindViewById<EditText>(Resource.Id.x_value);
			yET = FindViewById<EditText>(Resource.Id.y_value);
			oneET = FindViewById<EditText>(Resource.Id.one_value);

			editTextList.Add(x2ET);
			editTextList.Add(y2ET);
			editTextList.Add(xyET);
			editTextList.Add(xET);
			editTextList.Add(yET);
			editTextList.Add(oneET);

			sv = FindViewById<ScrollView>(Resource.Id.sv);

			refreshScreen(Constants.MULTIPLY, gridValueList, innerGridLayoutList, outerGridLayoutList);

			rectTileListList.Add(upperRightRectTileList);
			rectTileListList.Add(upperLeftRectTileList);
			rectTileListList.Add(lowerLeftRectTileList);
			rectTileListList.Add(lowerRightRectTileList);
		}

		private void button_click(object sender, EventArgs e)
		{
			var button = sender as Button;
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			AlertDialog alertDialog = builder.Create();
			alertDialog.SetMessage("Are you sure?");

			alertDialog.SetButton("Yes", (s, ev) =>
			{
				switch (button.Text)
			{
				case Constants.NEW_Q:
					setupNewQuestion();
					refreshScreen(Constants.MULTIPLY, gridValueList, innerGridLayoutList, outerGridLayoutList);
					sv.ScrollTo(0, 0);
					break;
				case Constants.REFR:
					refreshScreen(Constants.MULTIPLY, gridValueList, innerGridLayoutList, outerGridLayoutList);
					break;
				case Constants.CHK:
					checkAnswers();
					break;
				}
			});

			alertDialog.SetButton2("No", (s, ev) =>
			{

			});

			alertDialog.Show();
		}

		private void setupNewQuestion()
		{
			vars = AlgorithmUtilities.RNG(Constants.MULTIPLY, numberOfVariables);
			//Debug
			AlgorithmUtilities.expandingVars(vars);

			for (int i = 0; i < gridValueList.Count; ++i)
			{
				gridValueList[i].init();
			}

			setupQuestionString(vars);

			foreach (int i in vars)
			{
				Log.Debug(TAG, i + "");
			}
		}

		protected override void setupQuestionString(List<int> vars)
		{
			string output = "";
			output += "(";
			//vars = (ax + by + c)(dx + ey + f)
			int ax = vars[0];
			int by = vars[1];
			int c = vars[2];

			int dx = vars[3];
			int ey = vars[4];
			int f = vars[5];

			if (ax != 0)
				output += ax + "x+";
			if (by != 0)
				output += by + "y+";
			if (c != 0)
				output += c;
			else
				output = output.Remove(output.Length - 1);

			output += ")(";

			if (dx != 0)
				output += dx + "x+";
			if (ey != 0)
				output += ey + "y+";
			if (f != 0)
				output += f;
			else
				output = output.Remove(output.Length - 1);

			output += ")";
			output = output.Replace(" ", "");
			output = output.Replace("+-", "-");
			output = output.Replace("+", " + ");
			output = output.Replace("-", " - ");
			result.Text = output;
		}

		protected override int getLayoutResourceId()
		{
			return Resource.Layout.MultiplyTwoVar;
		}
	}
}