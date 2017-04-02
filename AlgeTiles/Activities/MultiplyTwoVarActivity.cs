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
	[Activity(Label = "MultiplyTwoVarActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class MultiplyTwoVarActivity : AlgeTilesActivity
	{
		private static string TAG = "AlgeTiles:MultiplyTwoVarActivity";
		public Button customQuestion;

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
			y2TV = FindViewById<TextView>(Resource.Id.y2TV);
			x2TV.Text = "x\xB2 + ";
			y2TV.Text = "y\xB2 + ";

			customQuestion = FindViewById<Button>(Resource.Id.custom);
			customQuestion.Click += CustomQuestion_Click;

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

					LinearLayout.LayoutParams par_1 = (LinearLayout.LayoutParams)tile_1.LayoutParameters;
					TileUtilities.TileFactor tF = TileUtilities.getTileFactors(tile_1.getTileType());
					par_1.Height = (int)(heightInPx / 7);
					par_1.Width = (int)(heightInPx / 7);
					tile_1.SetBackgroundResource(tF.id);
					tile_1.Text = tF.text;
					tile_1.LayoutParameters = par_1;

					LinearLayout.LayoutParams par_x = (LinearLayout.LayoutParams)x_tile.LayoutParameters;
					tF = TileUtilities.getTileFactors(x_tile.getTileType());
					par_x.Height = (int)(heightInPx / tF.heightFactor);
					par_x.Width = (int)(heightInPx / 7);
					x_tile.SetBackgroundResource(tF.id);
					x_tile.Text = tF.text;
					x_tile.LayoutParameters = par_x;

					LinearLayout.LayoutParams par_y = (LinearLayout.LayoutParams)y_tile.LayoutParameters;
					tF = TileUtilities.getTileFactors(y_tile.getTileType());
					par_y.Height = (int)(heightInPx / tF.heightFactor);
					par_y.Width = (int)(heightInPx / 7);
					y_tile.SetBackgroundResource(tF.id);
					y_tile.Text = tF.text;
					y_tile.LayoutParameters = par_y;

					LinearLayout.LayoutParams par_x2 = (LinearLayout.LayoutParams)x2_tile.LayoutParameters;
					tF = TileUtilities.getTileFactors(x2_tile.getTileType());
					par_x2.Height = (int)(heightInPx / tF.heightFactor);
					par_x2.Width = (int)(heightInPx / tF.widthFactor);
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

					LinearLayout.LayoutParams par_y2 = (LinearLayout.LayoutParams)y2_tile.LayoutParameters;
					tF = TileUtilities.getTileFactors(y2_tile.getTileType());
					par_y2.Height = (int)(heightInPx / tF.heightFactor);
					par_y2.Width = (int)(heightInPx / tF.widthFactor);
					y2_tile.SetBackgroundResource(tF.id);
					if (tF.text.Length > 1 && !tF.text.Equals("xy"))
					{
						var cs = new SpannableStringBuilder(tF.text);
						cs.SetSpan(new SuperscriptSpan(), 1, 2, SpanTypes.ExclusiveExclusive);
						cs.SetSpan(new RelativeSizeSpan(0.75f), 1, 2, SpanTypes.ExclusiveExclusive);
						y2_tile.TextFormatted = cs;
					}
					else
					{
						y2_tile.Text = tF.text;
					}
					y2_tile.LayoutParameters = par_y2;

					LinearLayout.LayoutParams par_xy = (LinearLayout.LayoutParams)xy_tile.LayoutParameters;
					tF = TileUtilities.getTileFactors(xy_tile.getTileType());
					par_xy.Height = (int)(heightInPx / tF.heightFactor);
					par_xy.Width = (int)(heightInPx / tF.widthFactor);
					xy_tile.SetBackgroundResource(tF.id);
					if (tF.text.Length > 1 && !tF.text.Equals("xy"))
					{
						var cs = new SpannableStringBuilder(tF.text);
						cs.SetSpan(new SuperscriptSpan(), 1, 2, SpanTypes.ExclusiveExclusive);
						cs.SetSpan(new RelativeSizeSpan(0.75f), 1, 2, SpanTypes.ExclusiveExclusive);
						xy_tile.TextFormatted = cs;
					}
					else
					{
						xy_tile.Text = tF.text;
					}
					xy_tile.LayoutParameters = par_xy;

					x2ET.SetHeight(par_x2.Height);
					xET.SetHeight(par_x2.Height);
					result.SetHeight(par_x2.Height / 2);
					float resultTextSize = result.TextSize;
					result.SetTextSize(ComplexUnitType.Sp, resultTextSize / Resources.DisplayMetrics.Density);
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

			settingsDialog = new Dialog(this);
			settingsDialog.Window.RequestFeature(WindowFeatures.NoTitle);
		}

 		private void CustomQuestion_Click(object sender, EventArgs e)
 		{
 			var dialog = CustomEquationDialogTwoVar.NewInstance();
 			dialog.Dismissed += (s, events) =>
 			{
 				if (events.vars[0].HasValue)
 					Console.WriteLine("Done with dialog: " + events.vars[0] + "," + events.vars[1] + "," + events.vars[2] + "," + events.vars[3] + "," + events.vars[4] + "," + events.vars[5]);
 
 				//Replace vars with event.vars and rerun the setupQuestionString(vars) after checking if this is valid, if not, show toast
 				if (AlgorithmUtilities.isSuppliedMultiplyEquationValid(events.vars, Constants.TWO_VAR))
 				{
 					vars.Clear();
 					for(int i = 0; i<events.vars.Length; ++i)
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
					sv.ScrollTo(0, 0);
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

		//private void setupNewQuestion()
		//{
		//	vars = AlgorithmUtilities.RNG(Constants.MULTIPLY, numberOfVariables);
		//	//Debug
		//	AlgorithmUtilities.expandingVars(vars);

		//	for (int i = 0; i < gridValueList.Count; ++i)
		//	{
		//		gridValueList[i].init();
		//	}

		//	setupQuestionString(vars);

		//	foreach (int i in vars)
		//	{
		//		Log.Debug(TAG, i + "");
		//	}
		//}

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