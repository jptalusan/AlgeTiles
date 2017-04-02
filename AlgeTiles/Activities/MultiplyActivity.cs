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
using Android.Text;
using Android.Text.Style;

namespace AlgeTiles
{
	[Activity(Label = "MultiplyActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class MultiplyActivity : AlgeTilesActivity
	{
		private static string TAG = "AlgeTiles:Multiply";
		public Button customQuestion;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			ActionBar.Hide();

			numberOfVariables = Intent.GetIntExtra(Constants.VARIABLE_COUNT, 0);

			SetContentView(Resource.Layout.Multiply);
			activityType = Constants.MULTIPLY;
			listeners = new Listeners(this);
			// Create your application here
			result = (TextView)FindViewById(Resource.Id.result);

			tile_1 = (AlgeTilesTextView)FindViewById(Resource.Id.tile_1);
			x_tile = (AlgeTilesTextView)FindViewById(Resource.Id.x_tile);
			y_tile = (AlgeTilesTextView)FindViewById(Resource.Id.y_tile);
			xy_tile = (AlgeTilesTextView)FindViewById(Resource.Id.xy_tile);
			x2_tile = (AlgeTilesTextView)FindViewById(Resource.Id.x2_tile);
			y2_tile = (AlgeTilesTextView)FindViewById(Resource.Id.y2_tile);

			tile_1.Click += listeners.tile_Click;
			x_tile.Click += listeners.tile_Click;
			y_tile.Click += listeners.tile_Click;
			x2_tile.Click += listeners.tile_Click;
			y2_tile.Click += listeners.tile_Click;
			xy_tile.Click += listeners.tile_Click;

			upperLeftGrid = FindViewById<AlgeTilesRelativeLayout>(Resource.Id.upperLeft);
			upperRightGrid = FindViewById<AlgeTilesRelativeLayout>(Resource.Id.upperRight);
			lowerLeftGrid = FindViewById<AlgeTilesRelativeLayout>(Resource.Id.lowerLeft);
			lowerRightGrid = FindViewById<AlgeTilesRelativeLayout>(Resource.Id.lowerRight);

			upperLeftGrid.backGroundResource = Resource.Drawable.upperLeftGridshape;
			upperRightGrid.backGroundResource = Resource.Drawable.upperRightGridshape;
			lowerLeftGrid.backGroundResource = Resource.Drawable.lowerLeftGridshape;
			lowerRightGrid.backGroundResource = Resource.Drawable.lowerRightGridshape;

			upperMiddleGrid = FindViewById<GridLayout>(Resource.Id.upperMiddle);
			middleLeftGrid = FindViewById<GridLayout>(Resource.Id.middleLeft);
			middleRightGrid = FindViewById<GridLayout>(Resource.Id.middleRight);
			lowerMiddleGrid = FindViewById<GridLayout>(Resource.Id.lowerMiddle);

			space1 = FindViewById<Space>(Resource.Id.spaceAfterEquation);
			space2 = FindViewById<Space>(Resource.Id.spaceBeforeTutorial);
			space3 = FindViewById<Space>(Resource.Id.space3);
			space4 = FindViewById<Space>(Resource.Id.space4);
			space5 = FindViewById<Space>(Resource.Id.space5);

			x2TV = FindViewById<TextView>(Resource.Id.x2TV);
			x2TV.Text = "x\xB2 + ";

			customQuestion = FindViewById<Button>(Resource.Id.custom);
			customQuestion.Click += CustomQuestion_Click;

			ViewTreeObserver vto = upperLeftGrid.ViewTreeObserver;
			vto.GlobalLayout += (sender, e) =>
			{
				if (!isFirstTime)
				{
					heightInPx = upperLeftGrid.Height;
					widthInPx = upperLeftGrid.Width;
					upperLeftGrid.SetMinimumHeight(0);
					upperLeftGrid.SetMinimumWidth(0);
					isFirstTime = true;

					LinearLayout.LayoutParams par_1 = (LinearLayout.LayoutParams)tile_1.LayoutParameters;
					TileUtilities.TileFactor tF = TileUtilities.getTileFactors(tile_1.getTileType());
					par_1.Height = heightInPx / 7;
					par_1.Width = heightInPx / 7;
					//tile_1.setDimensions(par_1.Height, par_1.Width);
					tile_1.SetBackgroundResource(tF.id);
					tile_1.Text = tF.text;
					tile_1.LayoutParameters = par_1;
					Log.Debug(TAG, tile_1.getTileType());
					
					LinearLayout.LayoutParams par_x = (LinearLayout.LayoutParams)x_tile.LayoutParameters;
					tF = TileUtilities.getTileFactors(x_tile.getTileType());
					par_x.Height = (int)(heightInPx / tF.heightFactor);
					par_x.Width = heightInPx / 7;
					//x_tile.setDimensions(par_x.Height, par_x.Width);
					x_tile.SetBackgroundResource(tF.id);
					x_tile.Text = tF.text;
					x_tile.LayoutParameters = par_x;
					Log.Debug(TAG, x_tile.getTileType());

					LinearLayout.LayoutParams par_x2 = (LinearLayout.LayoutParams)x2_tile.LayoutParameters;
					tF = TileUtilities.getTileFactors(x2_tile.getTileType());
					par_x2.Height = (int)(heightInPx / tF.heightFactor);
					par_x2.Width = (int)(heightInPx / tF.widthFactor);
					//x2_tile.setDimensions(par_x2.Height, par_x2.Width);
					x2_tile.SetBackgroundResource(tF.id);
					if (tF.text.Length > 1 && !tF.text.Equals("xy"))
					{
						var cs = new SpannableStringBuilder(tF.text);
						cs.SetSpan(new SuperscriptSpan(), 1, 2, SpanTypes.ExclusiveExclusive);
						cs.SetSpan(new RelativeSizeSpan(0.75f), 1, 2, SpanTypes.ExclusiveExclusive);
						x2_tile.TextFormatted = cs;
					}
					else
					{
						x2_tile.Text = tF.text;
					}
					x2_tile.LayoutParameters = par_x2;
					Log.Debug(TAG, x2_tile.getTileType());

					x2ET.SetHeight(par_x2.Height);
					result.SetHeight(par_x2.Height);
					space1.LayoutParameters = par_1;
					space2.LayoutParameters = par_1;
					space3.LayoutParameters = par_1;
					space4.LayoutParameters = par_1;
					space5.LayoutParameters = par_1;
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
			upperMiddleGrid.Touch += listeners.Layout_Touch;
			lowerMiddleGrid.Touch += listeners.Layout_Touch;

			//Together form one Part of the formula
			middleLeftGrid.Touch += listeners.Layout_Touch;
			middleRightGrid.Touch += listeners.Layout_Touch;

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

			tutorialButton = FindViewById<Button>(Resource.Id.tutorial);
			tutorialButton.Click += listeners.toggle_click;

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

			setupNewQuestion(numberOfVariables);

			correct = MediaPlayer.Create(this, Resource.Raw.correct);
			incorrect = MediaPlayer.Create(this, Resource.Raw.wrong);

			x2ET = FindViewById<EditText>(Resource.Id.x2_value);
			xET = FindViewById<EditText>(Resource.Id.x_value);
			oneET = FindViewById<EditText>(Resource.Id.one_value);

			editTextList.Add(x2ET);
			editTextList.Add(xET);
			editTextList.Add(oneET);

			refreshScreen(Constants.MULTIPLY, gridValueList, innerGridLayoutList, outerGridLayoutList);

			rectTileListList.Add(upperRightRectTileList);
			rectTileListList.Add(upperLeftRectTileList);
			rectTileListList.Add(lowerLeftRectTileList);
			rectTileListList.Add(lowerRightRectTileList);

			settingsDialog = new Dialog(this);
			settingsDialog.Window.RequestFeature(WindowFeatures.NoTitle);
		}

		private void CustomQuestion_Click(object sender, EventArgs e)
		{
			var dialog = CustomEquationDialog.NewInstance();
			dialog.Dismissed += (s, events) =>
			{
				if (events.vars[0].HasValue)
					Console.WriteLine("Done with dialog: " + events.vars[0] + "," + events.vars[1] + "," + events.vars[2] + "," + events.vars[3]);

				//Replace vars with event.vars and rerun the setupQuestionString(vars) after checking if this is valid, if not, show toast
				if (AlgorithmUtilities.isSuppliedMultiplyEquationValid(events.vars, Constants.ONE_VAR))
				{
					vars.Clear();
					for(int i = 0; i < events.vars.Length; ++i)
					{
						vars.Add(events.vars[i].GetValueOrDefault());
					}
					setupQuestionString(vars);
					refreshScreen(Constants.MULTIPLY, gridValueList, innerGridLayoutList, outerGridLayoutList);
				}
				else
				{
					Toast.MakeText(this, "Invalid, please enter values again.", ToastLength.Short).Show();
				}
			};
			dialog.Show(FragmentManager, "dialog");
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
					setupNewQuestion(numberOfVariables);
					refreshScreen(Constants.MULTIPLY, gridValueList, innerGridLayoutList, outerGridLayoutList);
					break;
				case Constants.REFR:
					refreshScreen(Constants.MULTIPLY, gridValueList, innerGridLayoutList, outerGridLayoutList);
					break;
				case Constants.CHK:
					checkAnswers(this);
					break;
				}
			});

			alertDialog.SetButton2("No", (s, ev) =>
			{

			});

			alertDialog.Show();
		}
		//HERER

		protected override void setupQuestionString(List<int> vars)
		{
			string output = "(";
			//vars = (ax + b)(cx + d)
			int ax = vars[0];
			int b = vars[1];
			int cx = vars[2];
			int d = vars[3];

			if (ax != 0)
				output += ax + "x+";
			if (b != 0)
				output += b;
			else
				output = output.Remove(output.Length - 1);

			output += ")(";

			if (cx != 0)
				output += cx + "x+";
			if (d != 0)
				output += d;
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
			return Resource.Layout.Multiply;
		}
	}
}