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
using Java.Util.Concurrent.Atomic;

namespace AlgeTiles
{
	[Activity(Label = "FactorActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class FactorActivity : Activity
	{
		private static string TAG = "AlgeTiles:Factor";
		public static Context context { get; }
		private static string BUTTON_TYPE = "BUTTON_TYPE";
		private static string CLONED_BUTTON = "CLONE_BUTTON";
		private static string ORIGINAL_BUTTON = "ORIGINAL_BUTTON";
		private TextView result;
		private Boolean hasButtonBeenDroppedInCorrectzone = false;
		private string currentButtonType = "";
		private ViewGroup currentOwner;

		private ToggleButton removeToggle;
		private ToggleButton dragToggle;
		private ToggleButton rotateToggle;
		private ToggleButton muteToggle;

		private ImageButton tile_1;
		private ImageButton x_tile;
		private ImageButton x2_tile;

		private ImageButton tile_1_rot;
		private ImageButton x_tile_rot;
		private ImageButton x2_tile_rot;

		private Button newQuestionButton;
		private Button refreshButton;
		private Button checkButton;

		private int numberOfVariables = 0;

		private Boolean isFirstAnswerCorrect = false;
		private Boolean isSecondAnswerCorrect = false;
		private Boolean isThirdAnswerCorrect = false;

		private AlgeTilesRelativeLayout upperLeftGrid;
		private AlgeTilesRelativeLayout upperRightGrid;
		private AlgeTilesRelativeLayout lowerLeftGrid;
		private AlgeTilesRelativeLayout lowerRightGrid;

		private GridLayout upperMiddleGrid;
		private GridLayout middleLeftGrid;
		private GridLayout middleRightGrid;
		private GridLayout lowerMiddleGrid;

		//Four outer grids
		private GridValue upperLeftGV;
		private GridValue upperRightGV;
		private GridValue lowerLeftGV;
		private GridValue lowerRightGV;

		//Four center grids
		private GridValue midUpGV;
		private GridValue midLowGV;
		private GridValue midLeftGV;
		private GridValue midRightGV;

		private List<int> vars = new List<int>();
		private List<int> expandedVars = new List<int>();
		private List<ViewGroup> innerGridLayoutList = new List<ViewGroup>();
		private List<ViewGroup> outerGridLayoutList = new List<ViewGroup>();
		private List<GridValue> gridValueList = new List<GridValue>();

		private MediaPlayer correct;
		private MediaPlayer incorrect;

		private EditText x_value_1;
		private EditText one_value_1;
		private EditText x_value_2;
		private EditText one_value_2;

		private List<EditText> editTextList = new List<EditText>();

		private bool isFirstTime = false;

		private int heightInPx = 0;
		private int widthInPx = 0;

		public List<RectTile> upperRightRectTileList = new List<RectTile>();
		public List<RectTile> upperLeftRectTileList = new List<RectTile>();
		public List<RectTile> lowerRightRectTileList = new List<RectTile>();
		public List<RectTile> lowerLeftRectTileList = new List<RectTile>();

		List<string> midUp = new List<string>();
		List<string> midLeft = new List<string>();
		List<string> midRight = new List<string>();
		List<string> midDown = new List<string>();

		private List<List<RectTile>> rectTileListList = new List<List<RectTile>>();
	
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			ActionBar.Hide();
			SetContentView(Resource.Layout.Factor);
			Log.Debug(TAG, "Enter FACTOR");
			// Create your application here
			result = (TextView)FindViewById(Resource.Id.result);

			tile_1 = (ImageButton)FindViewById(Resource.Id.tile_1);
			x_tile = (ImageButton)FindViewById(Resource.Id.x_tile);
			x2_tile = (ImageButton)FindViewById(Resource.Id.x2_tile);

			tile_1_rot = (ImageButton)FindViewById(Resource.Id.tile_1_rot);
			x_tile_rot = (ImageButton)FindViewById(Resource.Id.x_tile_rot);
			x2_tile_rot = (ImageButton)FindViewById(Resource.Id.x2_tile_rot);

			tile_1.LongClick += tile_LongClick;
			x_tile.LongClick += tile_LongClick;
			x2_tile.LongClick += tile_LongClick;

			tile_1_rot.LongClick += tile_LongClick;
			x_tile_rot.LongClick += tile_LongClick;
			x2_tile_rot.LongClick += tile_LongClick;

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
					setupNewQuestion();
					refreshScreen(Constants.FACTOR, gridValueList, innerGridLayoutList, outerGridLayoutList);
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
			upperLeftGrid.Drag += Layout_Drag;
			upperRightGrid.Drag += Layout_Drag;
			lowerLeftGrid.Drag += Layout_Drag;
			lowerRightGrid.Drag += Layout_Drag;

			//Shade red the other grids
			for (int i = 0; i < innerGridLayoutList.Count; ++i)
				innerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.unavailable);

			removeToggle = (ToggleButton)FindViewById(Resource.Id.remove);
			dragToggle = (ToggleButton)FindViewById(Resource.Id.drag);
			rotateToggle = (ToggleButton)FindViewById(Resource.Id.rotate);
			muteToggle = (ToggleButton)FindViewById(Resource.Id.mute);

			removeToggle.Click += toggle_click;
			dragToggle.Click += toggle_click;
			rotateToggle.Click += toggle_click;
			muteToggle.Click += toggle_click;

			numberOfVariables = Intent.GetIntExtra(Constants.VARIABLE_COUNT, 0);

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

			x_value_1 = FindViewById<EditText>(Resource.Id.x_value_1);
			one_value_1 = FindViewById<EditText>(Resource.Id.one_value_1);

			x_value_2 = FindViewById<EditText>(Resource.Id.x_value_2);
			one_value_2 = FindViewById<EditText>(Resource.Id.one_value_2);

			editTextList.Add(x_value_1);
			editTextList.Add(one_value_1);

			editTextList.Add(x_value_2);
			editTextList.Add(one_value_2);

			refreshScreen(Constants.FACTOR, gridValueList, innerGridLayoutList, outerGridLayoutList);

			rectTileListList.Add(upperRightRectTileList);
			rectTileListList.Add(upperLeftRectTileList);
			rectTileListList.Add(lowerLeftRectTileList);
			rectTileListList.Add(lowerRightRectTileList);
		}

		private void toggle_click(object sender, EventArgs e)
		{
			ToggleButton clicked_toggle = (sender) as ToggleButton;
			int buttonText = clicked_toggle.Id;
			switch (buttonText)
			{
				case Resource.Id.remove:
					dragToggle.Checked = dragToggle.Checked ? false : false;
					if (rotateToggle.Checked)
					{
						FindViewById<LinearLayout>(Resource.Id.notRotatedButtonLayout).Visibility = ViewStates.Visible;
						FindViewById<LinearLayout>(Resource.Id.rotatedButtonLayout).Visibility = ViewStates.Gone;
					}
					rotateToggle.Checked = rotateToggle.Checked ? false : false;
					break;
				case Resource.Id.drag:
					removeToggle.Checked = removeToggle.Checked ? false : false;
					if (rotateToggle.Checked)
					{
						FindViewById<LinearLayout>(Resource.Id.notRotatedButtonLayout).Visibility = ViewStates.Visible;
						FindViewById<LinearLayout>(Resource.Id.rotatedButtonLayout).Visibility = ViewStates.Gone;
					}
					rotateToggle.Checked = rotateToggle.Checked ? false : false;
					break;
				case Resource.Id.rotate:
					//Also rotate original tiles
					if (rotateToggle.Checked)
					{
						FindViewById<LinearLayout>(Resource.Id.notRotatedButtonLayout).Visibility = ViewStates.Gone;
						FindViewById<LinearLayout>(Resource.Id.rotatedButtonLayout).Visibility = ViewStates.Visible;
					}
					else
					{
						FindViewById<LinearLayout>(Resource.Id.notRotatedButtonLayout).Visibility = ViewStates.Visible;
						FindViewById<LinearLayout>(Resource.Id.rotatedButtonLayout).Visibility = ViewStates.Gone;
					}
					removeToggle.Checked = removeToggle.Checked ? false : false;
					dragToggle.Checked = dragToggle.Checked ? false : false;
					break;
			}
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
						refreshScreen(Constants.FACTOR, gridValueList, innerGridLayoutList, outerGridLayoutList);
						break;
					case Constants.REFR:
						refreshScreen(Constants.FACTOR, gridValueList, innerGridLayoutList, outerGridLayoutList);
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

		private void refreshScreen(string ActivityType, List<GridValue> gvList, List<ViewGroup> inGLList, List<ViewGroup> outGLList)
		{
			upperRightGrid.clearRects(heightInPx, widthInPx);
			upperLeftGrid.clearRects(heightInPx, widthInPx);
			lowerRightGrid.clearRects(heightInPx, widthInPx);
			lowerLeftGrid.clearRects(heightInPx, widthInPx);

			upperRightRectTileList.Clear();
			upperLeftRectTileList.Clear();
			lowerRightRectTileList.Clear();
			lowerLeftRectTileList.Clear();

			generateInnerLayoutTileArrays();

			upperRightGrid.drawRects(upperRightRectTileList);
			upperLeftGrid.drawRects(upperLeftRectTileList);
			lowerRightGrid.drawRects(lowerRightRectTileList);
			lowerLeftGrid.drawRects(lowerLeftRectTileList);

			upperRightGrid.updatesomething(true);
			upperLeftGrid.updatesomething(true);
			lowerRightGrid.updatesomething(true);
			lowerLeftGrid.updatesomething(true);

			upperRightGrid.SetBackgroundResource(Resource.Drawable.shape_droptarget);
			upperRightGrid.SetBackgroundResource(Resource.Drawable.shape);

			upperLeftGrid.SetBackgroundResource(Resource.Drawable.shape_droptarget);
			upperLeftGrid.SetBackgroundResource(Resource.Drawable.shape);

			lowerRightGrid.SetBackgroundResource(Resource.Drawable.shape_droptarget);
			lowerRightGrid.SetBackgroundResource(Resource.Drawable.shape);

			lowerLeftGrid.SetBackgroundResource(Resource.Drawable.shape_droptarget);
			lowerLeftGrid.SetBackgroundResource(Resource.Drawable.shape);


			for (int i = 0; i < editTextList.Count; ++i)
			{
				editTextList[i].SetBackgroundResource(Resource.Drawable.shape);
				editTextList[i].Enabled = false;
				editTextList[i].Text = "";
			}

			isFirstAnswerCorrect = false;
			isSecondAnswerCorrect = false;
			isThirdAnswerCorrect = false;

			for (int i = 0; i < inGLList.Count; ++i)
			{
				inGLList[i].SetBackgroundResource(Resource.Drawable.shape);
				inGLList[i].Drag -= Layout_Drag;
			}

			for (int i = 0; i < outGLList.Count; ++i)
			{
				outGLList[i].SetBackgroundResource(Resource.Drawable.shape);
				outGLList[i].Drag -= Layout_Drag;
			}

			for (int i = 0; i < gvList.Count; ++i)
			{
				gvList[i].init();
			}

			for (int i = 0; i < inGLList.Count; ++i)
			{
				for (int j = 0; j < inGLList[i].ChildCount; ++j)
				{
					View v = inGLList[i].GetChildAt(j);
					inGLList[i].RemoveAllViews();
				}
			}

			for (int i = 0; i < outGLList.Count; ++i)
			{
				for (int j = 0; j < outGLList[i].ChildCount; ++j)
				{
					View v = outGLList[i].GetChildAt(j);
					outGLList[i].RemoveAllViews();
				}
			}

			if (Constants.FACTOR == ActivityType)
			{
				for (int i = 0; i < inGLList.Count; ++i)
				{
					inGLList[i].SetBackgroundResource(Resource.Drawable.unavailable);
					inGLList[i].Drag -= Layout_Drag;
				}

				for (int i = 0; i < outGLList.Count; ++i)
				{
					outGLList[i].SetBackgroundResource(Resource.Drawable.shape);
					outGLList[i].Drag += Layout_Drag;
				}
			}
			else
			{
				for (int i = 0; i < inGLList.Count; ++i)
				{
					inGLList[i].SetBackgroundResource(Resource.Drawable.shape);
					inGLList[i].Drag += Layout_Drag;
				}

				for (int i = 0; i < outGLList.Count; ++i)
				{
					outGLList[i].SetBackgroundResource(Resource.Drawable.unavailable);
					outGLList[i].Drag -= Layout_Drag;
				}
			}
		}

		//TODO: URGENT: must compute the right tiles in the correct quadrant in the form of (ax + b) (cx + d)
		private void checkAnswers()
		{
			//TODO: rename, for factor activity, second answer is first answer
			if (!isFirstAnswerCorrect)
			{
				GridValue[] gvArr = { upperLeftGV, upperRightGV, lowerLeftGV, lowerRightGV };

				if (AlgorithmUtilities.isSecondAnswerCorrect(expandedVars, gvArr, numberOfVariables))
				{
					Toast.MakeText(Application.Context, "2:correct", ToastLength.Short).Show();
					if (!muteToggle.Checked)
						correct.Start();
					isFirstAnswerCorrect = true;

					//Loop through inner and prevent deletions by removing: clonedImageView_Touch
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
					{
						outerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.ok);
						outerGridLayoutList[i].Drag -= Layout_Drag;
						for (int j = 0; j < outerGridLayoutList[i].ChildCount; ++j)
						{
							var iv = outerGridLayoutList[i].GetChildAt(j) as View;
							iv.LongClick -= clonedImageView_Touch;
						}
					}

					//Shade red the other grids
					for (int i = 0; i < innerGridLayoutList.Count; ++i)
					{
						innerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.shape);
						innerGridLayoutList[i].Drag += Layout_Drag;
					}

					//TODO:accomodate for 2 variables (right now just for one)
					x_value_1.Enabled = true;
					one_value_1.Enabled = true;
					x_value_2.Enabled = true;
					one_value_2.Enabled = true;
				}
				else
				{
					Toast.MakeText(Application.Context, "2:incorrect", ToastLength.Short).Show();
					incorrectPrompt(outerGridLayoutList);
				}
			}
			else if (!isSecondAnswerCorrect)
			{
				GridValue[] gvArr = { midUpGV, midLowGV, midLeftGV, midRightGV };
				if (AlgorithmUtilities.isFirstAnswerCorrect(vars, gvArr, numberOfVariables))
				{
					//TODO: First answer is second in factor
					isSecondAnswerCorrect = true;

					for (int i = 0; i < outerGridLayoutList.Count; ++i)
						outerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.ok);

					//Loop through inner and prevent deletions by removing: clonedImageView_Touch
					for (int i = 0; i < innerGridLayoutList.Count; ++i)
					{
						innerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.ok);
						innerGridLayoutList[i].Drag -= Layout_Drag;
						for (int j = 0; j < innerGridLayoutList[i].ChildCount; ++j)
						{
							var iv = innerGridLayoutList[i].GetChildAt(j) as View;
							iv.LongClick -= clonedImageView_Touch;
						}
					}
					expandedVars = AlgorithmUtilities.expandingVars(vars);
					Toast.MakeText(Application.Context, "1:correct", ToastLength.Short).Show();
					if (!muteToggle.Checked)
						correct.Start();
				}
				else
				{
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
						outerGridLayoutList[i].Drag -= Layout_Drag;

					incorrectPrompt(innerGridLayoutList);
					Toast.MakeText(Application.Context, "1:incorrect", ToastLength.Short).Show();
				}
			}
			else if (!isThirdAnswerCorrect)
			{
				int[] answer = new int[4];
				int temp = 0;
				answer[0] = int.TryParse(x_value_1.Text, out temp) ? temp : 0;
				answer[1] = int.TryParse(one_value_1.Text, out temp) ? temp : 0;
				answer[2] = int.TryParse(x_value_2.Text, out temp) ? temp : 0; ;
				answer[3] = int.TryParse(one_value_2.Text, out temp) ? temp : 0;

				if (Math.Abs(answer[0] + answer[1] + answer[2] + answer[3]) == 0)
				{
					isThirdAnswerCorrect = false;
				}
				else
				{
					if ((vars[0] == answer[0] && vars[1] == answer[1]) &&
						(vars[2] == answer[2] && vars[3] == answer[3]) ||
						(vars[0] == answer[2] && vars[1] == answer[3]) &&
						(vars[2] == answer[0] && vars[3] == answer[1]))
						isThirdAnswerCorrect = true;
				}
				if (isThirdAnswerCorrect)
				{
					Toast.MakeText(Application.Context, "3:correct/end", ToastLength.Short).Show();
					if (!muteToggle.Checked)
						correct.Start();
					for (int i = 0; i < editTextList.Count; ++i)
					{
						editTextList[i].SetBackgroundResource(Resource.Drawable.ok);
						editTextList[i].Enabled = false;
					}
				}
				else
				{
					Toast.MakeText(Application.Context, "3:incorrect", ToastLength.Short).Show();
					incorrectPrompt(editTextList);
				}
			}
		}

		public async void incorrectPrompt(List<EditText> gvList)
		{
			if (!muteToggle.Checked)
				incorrect.Start();
			for (int i = 0; i < gvList.Count; ++i)
				gvList[i].SetBackgroundResource(Resource.Drawable.notok);
			await Task.Delay(Constants.DELAY);
			for (int i = 0; i < gvList.Count; ++i)
				gvList[i].SetBackgroundResource(Resource.Drawable.shape);
		}

		public async void incorrectPrompt(List<ViewGroup> gvList)
		{
			if (!muteToggle.Checked)
				incorrect.Start();
			for (int i = 0; i < gvList.Count; ++i)
				gvList[i].SetBackgroundResource(Resource.Drawable.notok);
			await Task.Delay(Constants.DELAY);
			for (int i = 0; i < gvList.Count; ++i)
				gvList[i].SetBackgroundResource(Resource.Drawable.shape);
		}

		private void setupNewQuestion()
		{
			vars = AlgorithmUtilities.RNG(Constants.FACTOR, numberOfVariables);

			string temp = "";
			foreach (int i in vars)
				temp += i + ",";
			Log.Debug(TAG, "factors (ax + b)(cx + d):" + temp);

			expandedVars = AlgorithmUtilities.expandingVars(vars);
			string temp2 = "";
			foreach (int i in expandedVars)
				temp2 += i + ",";
			Log.Debug(TAG, "expanded (ax^2 + bx + c):" + temp2);


			//(ax + b)(cx + d)
			if (Constants.ONE_VAR == numberOfVariables)
			{
				for (int i = 0; i < gridValueList.Count; ++i)
				{
					gridValueList[i].init();
				}
				setupQuestionString(expandedVars);
			}
		}

		private void setupQuestionString(List<int> vars)
		{
			string output = "";
			//vars = (ax^2 + bx + c)
			int ax2 = vars[0];
			int bx = vars[1];
			int c = vars[2];

			if (ax2 != 0)
				output += ax2 + "x^2+";
			if (bx != 0)
				output += bx + "x+";
			if (c != 0)
				output += c;

			output = output.Replace(" ", "");
			output = output.Replace("+-", "-");
			output = output.Replace("+", " + ");
			output = output.Replace("-", " - ");
			result.Text = output;
		}

		private void Layout_Drag(object sender, View.DragEventArgs e)
		{
			var v = (ViewGroup)sender;
			View view = (View)e.Event.LocalState;
			var button_type = result.Text;
			var drag_data = e.Event.ClipData;
			bool isDroppedAtCenter = false;
			float x = 0.0f;
			float y = 0.0f;

			switch (e.Event.Action)
			{
				case DragAction.Started:
					hasButtonBeenDroppedInCorrectzone = false;
					if (null != drag_data)
					{
						currentButtonType = drag_data.GetItemAt(0).Text;
					}
					break;
				case DragAction.Entered:
					v.SetBackgroundResource(Resource.Drawable.shape_droptarget);
					break;
				case DragAction.Exited:
					currentOwner = (ViewGroup)view.Parent;
					hasButtonBeenDroppedInCorrectzone = false;
					v.SetBackgroundResource(Resource.Drawable.shape);
					break;
				case DragAction.Location:
					x = e.Event.GetX(); //width
					y = e.Event.GetY(); //height
					break;
				case DragAction.Drop:
					Log.Debug(TAG, "Drop");
					if (null != drag_data)
					{
						currentButtonType = drag_data.GetItemAt(0).Text;
					}

					AlgeTilesTextView algeTilesIV = new AlgeTilesTextView(this);
					algeTilesIV.setTileType(currentButtonType);
					Boolean wasImageDropped = false;

					//Check if x_tile is rotated before fitting or rotate before dropping automatically
					if ((v.Id == Resource.Id.upperMiddle ||
						 v.Id == Resource.Id.middleLeft ||
						 v.Id == Resource.Id.middleRight ||
						 v.Id == Resource.Id.lowerMiddle) &&
						currentButtonType.Equals(Constants.X2_TILE))
					{
						//Do nothing
					}
					else if (isFirstAnswerCorrect &&
							(v.Id == Resource.Id.upperLeft ||
							 v.Id == Resource.Id.upperRight ||
							 v.Id == Resource.Id.lowerLeft ||
							 v.Id == Resource.Id.lowerRight))
					{
						//Do nothing
					}
					//Handle drops for second part of problem (expanded form)
					else if (!isFirstAnswerCorrect &&
							(v.Id == Resource.Id.upperLeft ||
							 v.Id == Resource.Id.upperRight ||
							 v.Id == Resource.Id.lowerLeft ||
							 v.Id == Resource.Id.lowerRight))
					{
						wasImageDropped = true;
					}
					//Handle auto rotate for x_tile (middle)
					//First group x
					else if (isFirstAnswerCorrect &&
							(v.Id == Resource.Id.middleLeft ||
							 v.Id == Resource.Id.middleRight))
					{
						if (v.Id == Resource.Id.middleLeft)
						{
							algeTilesIV.RotationY = 180;
						}

						wasImageDropped = true;
						isDroppedAtCenter = true;
					}
					//Second group x
					else if (isFirstAnswerCorrect &&
							(v.Id == Resource.Id.upperMiddle ||
							v.Id == Resource.Id.lowerMiddle))
					{
						if (v.Id == Resource.Id.upperMiddle)
							algeTilesIV.RotationX = 180;
						wasImageDropped = true;
						isDroppedAtCenter = true;
					}

					algeTilesIV.setTileType(currentButtonType);

					if (wasImageDropped)
					{
						ViewGroup container = (ViewGroup)v;
						Log.Debug(TAG, currentButtonType);
						int heightFactor = 0;
						int widthFactor = 0;
						switch (currentButtonType)
						{
							case Constants.X2_TILE:
							case Constants.X2_TILE_ROT:
								algeTilesIV.SetBackgroundResource(Resource.Drawable.x);
								algeTilesIV.Text = "x2";
								heightFactor = 3;
								widthFactor = 3;
								break;
							case Constants.X_TILE:
								algeTilesIV.SetBackgroundResource(Resource.Drawable.x);
								algeTilesIV.Text = "x";
								heightFactor = 3;
								widthFactor = 9;
								break;
							case Constants.X_TILE_ROT:
								algeTilesIV.SetBackgroundResource(Resource.Drawable.x);
								algeTilesIV.Text = "x";
								heightFactor = 9;
								widthFactor = 3;
								break;
							case Constants.ONE_TILE:
							case Constants.ONE_TILE_ROT:
								algeTilesIV.SetBackgroundResource(Resource.Drawable.one);
								algeTilesIV.Text = "1";
								heightFactor = 9;
								widthFactor = 9;
								break;
						}

						x = e.Event.GetX(); //width
						y = e.Event.GetY(); //height

						if (!isDroppedAtCenter)
						{
							Rect r = checkIfUserDropsOnRect(v.Id, currentButtonType, x, y, Constants.ADD);
							if (null != r)
							{
								RelativeLayout.LayoutParams par = new RelativeLayout.LayoutParams(
									ViewGroup.LayoutParams.WrapContent,
									ViewGroup.LayoutParams.WrapContent);
								par.Height = r.Height();
								par.Width = r.Width();
								par.TopMargin = r.Top;
								par.LeftMargin = r.Left;
								algeTilesIV.LayoutParameters = par;
								algeTilesIV.LongClick += clonedImageView_Touch;
								container.AddView(algeTilesIV);
								checkWhichParentAndUpdate(v.Id, currentButtonType, Constants.ADD);
								hasButtonBeenDroppedInCorrectzone = true;
							}
						}
						else
						{
							GridLayout.LayoutParams gParms = new GridLayout.LayoutParams();
							if (v.Id == Resource.Id.middleLeft ||
							 v.Id == Resource.Id.middleRight)
							{
								gParms.SetGravity(GravityFlags.FillVertical);
								if (rotateToggle.Checked)
								{
									gParms.Height = heightInPx / heightFactor;
									gParms.Width = heightInPx / widthFactor;
								}
								else
								{
									gParms.Width = heightInPx / heightFactor;
									gParms.Height = heightInPx / widthFactor;
								}
							}
							else
							{
								gParms.SetGravity(GravityFlags.FillHorizontal);
								if (rotateToggle.Checked)
								{
									gParms.Width = heightInPx / heightFactor;
									gParms.Height = heightInPx / widthFactor;
								}
								else
								{
									gParms.Height = heightInPx / heightFactor;
									gParms.Width = heightInPx / widthFactor;
								}
							}
							algeTilesIV.LayoutParameters = gParms;
							algeTilesIV.LongClick += clonedImageView_Touch;
							container.AddView(algeTilesIV);
							checkWhichParentAndUpdate(v.Id, currentButtonType, Constants.ADD);

							//Auto re-arrange of center tiles
							List<AlgeTilesTextView> centerTileList = new List<AlgeTilesTextView>();
							Log.Debug(TAG, "Container count: " + container.ChildCount);
							for (int i = 0; i < container.ChildCount; ++i)
							{
								AlgeTilesTextView a = (AlgeTilesTextView)container.GetChildAt(i);
								centerTileList.Add(a);
								Log.Debug(TAG, "Center count: " + i + ", " + a.getTileType());
							}
							container.RemoveAllViews();

							List<AlgeTilesTextView> sortedList = centerTileList.OrderByDescending(o => o.getTileType()).ToList();
							for (int i = 0; i < sortedList.Count; ++i)
							{
								Log.Debug(TAG, "Tile order:" + sortedList[i].getTileType());
								container.AddView(sortedList[i]);
							}
							//End of auto re-arrange
						}
						view.Visibility = ViewStates.Visible;
						v.SetBackgroundResource(Resource.Drawable.shape);
					}
					break;
				case DragAction.Ended:
					v.SetBackgroundResource(Resource.Drawable.shape);
					if (!hasButtonBeenDroppedInCorrectzone &&
						currentButtonType.Equals(CLONED_BUTTON))
					{
						currentOwner.RemoveView(view);
					}
					else
					{
						view.Visibility = ViewStates.Visible;
					}
					break;
				default:
					break;
			}
		}

		private Rect checkIfUserDropsOnRect(int vId, string tileType, float x, float y, int command)
		{
			if (Resource.Id.upperLeft == vId)
			{
				foreach (RectTile r in upperLeftRectTileList)
				{
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						if (Constants.ADD == command)
							r.setTilePresence(true);
						return r.getRect();
					}
					else if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && r.getTilePresence())
					{
						if (Constants.SUBTRACT == command)
						{
							r.setTilePresence(false);
							return null;
						}
					}
				}
			}

			if (Resource.Id.upperRight == vId)
			{
				foreach (RectTile r in upperRightRectTileList)
				{
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						if (Constants.ADD == command)
							r.setTilePresence(true);
						return r.getRect();
					}
					else if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && r.getTilePresence())
					{
						if (Constants.SUBTRACT == command)
						{
							r.setTilePresence(false);
							return null;
						}
					}
				}
			}

			if (Resource.Id.lowerLeft == vId)
			{
				foreach (RectTile r in lowerLeftRectTileList)
				{
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						if (Constants.ADD == command)
							r.setTilePresence(true);
						return r.getRect();
					}
					else if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && r.getTilePresence())
					{
						if (Constants.SUBTRACT == command)
						{
							r.setTilePresence(false);
							return null;
						}
					}
				}
			}

			if (Resource.Id.lowerRight == vId)
			{
				foreach (RectTile r in lowerRightRectTileList)
				{
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						if (Constants.ADD == command)
							r.setTilePresence(true);
						return r.getRect();
					}
					else if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && r.getTilePresence())
					{
						if (Constants.SUBTRACT == command)
						{
							r.setTilePresence(false);
							return null;
						}
					}
				}
			}

			return null;
		}


		private void generateInnerLayoutTileArrays()
		{
			Log.Debug(TAG, "generateInnerLayoutTileArrays()");

			midUp.Clear();
			midRight.Clear();
			midDown.Clear();
			midLeft.Clear();

			upperRightRectTileList.Clear();
			upperLeftRectTileList.Clear();
			lowerRightRectTileList.Clear();
			lowerLeftRectTileList.Clear();

			List<int> input = vars;

			int ax = input[0]; //ax
			int b = input[1]; //b

			int cx = input[2]; //cx
			int d = input[3]; //d

			Log.Debug(TAG, ax + "," + b + "," + cx + "," + d);

			if (ax > 0)
			{
				for (int i = 0; i < ax; ++i)
					midRight.Add(Constants.X_TILE);
			}
			else if (ax < 0)
			{
				for (int i = 0; i > ax; --i)
					midLeft.Add(Constants.X_TILE);
			}

			if (b > 0)
			{
				for (int i = 0; i < b; ++i)
					midRight.Add(Constants.ONE_TILE);
			}
			else if (b < 0)
			{
				for (int i = 0; i > b; --i)
					midLeft.Add(Constants.ONE_TILE);
			}
			
			if (cx > 0)
			{
				for (int i = 0; i < cx; ++i)
					midUp.Add(Constants.X_TILE);
			}
			else if (cx < 0)
			{
				for (int i = 0; i > cx; --i)
					midDown.Add(Constants.X_TILE);
			}

			if (d > 0)
			{
				for (int i = 0; i < d; ++i)
					midUp.Add(Constants.ONE_TILE);
			}
			else if (d < 0)
			{
				for (int i = 0; i > d; --i)
					midDown.Add(Constants.ONE_TILE);
			}

			int height = heightInPx;
			int width = widthInPx;
			//int height = 500;
			//int width = 698;

			//upmid x midRight = quadrant1
			if (midUp.Count != 0 || midRight.Count != 0)
			{
				int top = height; //height of relative layout
				int bottom = height; //height of relative layout
				for (int i = 0; i < midUp.Count; ++i)
				{
					int left = 0;
					int right = 0;
					bool firstPass = true;
					for (int j = 0; j < midRight.Count; ++j)
					{
						int[] productDimensions = TileUtilities.getDimensionsOfProduct(height, midUp[i], midRight[j]);
						//Log.Debug(TAG, height + ":" + midUp[i] + "," + midRight[j]);
						//Log.Debug(TAG, productDimensions[0] + "," + productDimensions[1]);
						if (firstPass)
						{
							//top = subtract height of first tile in midup ( then subtract next tile  ) etc...
							top -= productDimensions[0];
							//bottom = height at i = 0, else bottom = previous top
							bottom = top + productDimensions[0]; //don't add to stack since only getting the latest top/height
							firstPass = false;
						}
						//right = width of midleft (then add next tile) etc...
						right += productDimensions[1]; //width adds up
													   //left = 0 at start, else width of midleft
						left = right - productDimensions[1];

						Rect r = new Rect(left, top, right, bottom);
						upperRightRectTileList.Add(new RectTile(r, TileUtilities.getTileTypeOfProduct(midUp[i], midRight[j])));
						Log.Debug(TAG, "Q1: " + TileUtilities.getTileTypeOfProduct(midUp[i], midRight[j]) + ":" + r.Left + "," + r.Top + ":" + r.Right + "," + r.Bottom);
					}
				}
			}

			//upmid x midLeft = quadrant2
			if (midUp.Count != 0 || midLeft.Count != 0)
			{
				int top = height;
				int bottom = height;
				for (int i = 0; i < midUp.Count; ++i)
				{
					int left = width;
					int right = width;
					bool firstPass = true;
					for (int j = 0; j < midLeft.Count; ++j)
					{
						int[] productDimensions = TileUtilities.getDimensionsOfProduct(height, midUp[i], midLeft[j]);
						//Log.Debug(TAG, height + ":" + midUp[i] + "," + midRight[j]);
						//Log.Debug(TAG, productDimensions[0] + "," + productDimensions[1]);
						if (firstPass)
						{
							top -= productDimensions[0];
							bottom = top + productDimensions[0];
							firstPass = false;
						}
						left -= productDimensions[1];
						right = left + productDimensions[1];

						Rect r = new Rect(left, top, right, bottom);
						upperLeftRectTileList.Add(new RectTile(r, TileUtilities.getTileTypeOfProduct(midUp[i], midLeft[j])));
						Log.Debug(TAG, "Q2: " + TileUtilities.getTileTypeOfProduct(midUp[i], midLeft[j]) + ":" + r.Left + "," + r.Top + ":" + r.Right + "," + r.Bottom);
					}
				}
			}

			//loMid x midLeft = quadrant3
			if (midDown.Count != 0 || midLeft.Count != 0)
			{
				int top = 0;
				int bottom = 0;
				for (int i = 0; i < midDown.Count; ++i)
				{
					int left = width;
					int right = width;
					bool firstPass = true;
					for (int j = 0; j < midLeft.Count; ++j)
					{
						int[] productDimensions = TileUtilities.getDimensionsOfProduct(height, midDown[i], midLeft[j]);
						//Log.Debug(TAG, height + ":" + midUp[i] + "," + midRight[j]);
						//Log.Debug(TAG, productDimensions[0] + "," + productDimensions[1]);
						if (firstPass)
						{
							bottom += productDimensions[0];
							top = bottom - productDimensions[0];
							firstPass = false;
						}
						left -= productDimensions[1];
						right = left + productDimensions[1];

						Rect r = new Rect(left, top, right, bottom);
						lowerLeftRectTileList.Add(new RectTile(r, TileUtilities.getTileTypeOfProduct(midDown[i], midLeft[j])));
						Log.Debug(TAG, "Q3: " + TileUtilities.getTileTypeOfProduct(midDown[i], midLeft[j]) + ":" + r.Left + "," + r.Top + ":" + r.Right + "," + r.Bottom);
					}
				}
			}

			//loMid x midRight = quadrant4
			if (midDown.Count != 0 || midRight.Count != 0)
			{
				int top = 0;
				int bottom = 0;
				for (int i = 0; i < midDown.Count; ++i)
				{
					int left = 0;
					int right = 0;
					bool firstPass = true;
					for (int j = 0; j < midRight.Count; ++j)
					{
						int[] productDimensions = TileUtilities.getDimensionsOfProduct(height, midDown[i], midRight[j]);
						//Log.Debug(TAG, height + ":" + midUp[i] + "," + midRight[j]);
						//Log.Debug(TAG, productDimensions[0] + "," + productDimensions[1]);
						if (firstPass)
						{
							bottom += productDimensions[0];
							top = bottom - productDimensions[0];
							firstPass = false;
						}
						right += productDimensions[1];
						left = right - productDimensions[1];
						Rect r = new Rect(left, top, right, bottom);
						lowerRightRectTileList.Add(new RectTile(r, TileUtilities.getTileTypeOfProduct(midDown[i], midRight[j])));
						Log.Debug(TAG, "Q4: " + TileUtilities.getTileTypeOfProduct(midDown[i], midRight[j]) + ":" + r.Left + "," + r.Top + ":" + r.Right + "," + r.Bottom);
					}
				}
			}
		}

		private void tile_LongClick(object sender, View.LongClickEventArgs e)
		{
			var imageViewTouch = (sender) as ImageButton;
			ClipData data = ClipData.NewPlainText(BUTTON_TYPE, ORIGINAL_BUTTON);
			switch (imageViewTouch.Id)
			{
				case Resource.Id.tile_1:
				case Resource.Id.tile_1_rot:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.ONE_TILE);
					break;
				case Resource.Id.x_tile:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.X_TILE);
					break;
				case Resource.Id.x_tile_rot:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.X_TILE_ROT);
					break;
				case Resource.Id.x2_tile:
				case Resource.Id.x2_tile_rot:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.X2_TILE);
					break;
			}

			dragToggle.Checked = false;
			removeToggle.Checked = false;

			View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(imageViewTouch);
			imageViewTouch.StartDrag(data, shadowBuilder, imageViewTouch, 0);
		}

		//http://stackoverflow.com/questions/18836432/how-to-find-the-view-of-a-button-in-its-click-eventhandler
		//TODO: When top most layer textview increases in length, the edit text gets pushed
		private void clonedImageView_Touch(object sender, View.LongClickEventArgs e)
		{
			var touchedImageView = (sender) as AlgeTilesTextView;
			ViewGroup vg = (ViewGroup)touchedImageView.Parent;
			if (removeToggle.Checked)
			{
				Log.Debug(TAG, "Switch: Remove");
				checkIfUserDropsOnRect(vg.Id,
					touchedImageView.getTileType(),
					touchedImageView.Left + 10,
					touchedImageView.Top + 10,
					Constants.SUBTRACT);
				vg.RemoveView(touchedImageView);
				touchedImageView.Visibility = ViewStates.Gone;
				Vibrator vibrator = (Vibrator)GetSystemService(Context.VibratorService);
				vibrator.Vibrate(30);

				int id = touchedImageView.Id;

				checkWhichParentAndUpdate(vg.Id, touchedImageView.getTileType(), Constants.SUBTRACT);
			}

			if (dragToggle.Checked)
			{
				Log.Debug(TAG, "Switch: Drag");
			}

			//TODO: Not working
			if (rotateToggle.Checked)
			{
				Log.Debug(TAG, "Switch: Rotate");
				touchedImageView.Rotation = touchedImageView.Rotation - 90;
			}
		}

		private void updateResults(string text)
		{
			result.Text = text;
		}

		private void checkWhichParentAndUpdate(int id, string tile, int process)
		{
			if (Constants.ADD == process)
			{
				if (Resource.Id.upperLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++upperLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++upperLeftGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++upperLeftGV.oneVal;
				}
				if (Resource.Id.upperRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++upperRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++upperRightGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++upperRightGV.oneVal;
				}
				if (Resource.Id.lowerLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++lowerLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++lowerLeftGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++lowerLeftGV.oneVal;
				}
				if (Resource.Id.lowerRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++lowerRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++lowerRightGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++lowerRightGV.oneVal;
				}

				//CENTER
				if (Resource.Id.upperMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++midUpGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midUpGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++midUpGV.oneVal;
				}
				if (Resource.Id.lowerMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++midLowGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midLowGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++midLowGV.oneVal;
				}
				if (Resource.Id.middleLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++midLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midLeftGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++midLeftGV.oneVal;
				}
				if (Resource.Id.middleRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++midRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midRightGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++midRightGV.oneVal;
				}

			}
			//REMOVE
			else if (Constants.SUBTRACT == process)
			{
				if (Resource.Id.upperLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--upperLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--upperLeftGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--upperLeftGV.oneVal;
				}
				if (Resource.Id.upperRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--upperRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--upperRightGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--upperRightGV.oneVal;
				}
				if (Resource.Id.lowerLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--lowerLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--lowerLeftGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--lowerLeftGV.oneVal;
				}
				if (Resource.Id.lowerRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--lowerRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--lowerRightGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--lowerRightGV.oneVal;
				}

				//CENTER
				if (Resource.Id.upperMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--midUpGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midUpGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--midUpGV.oneVal;
				}
				if (Resource.Id.lowerMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--midLowGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midLowGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--midLowGV.oneVal;
				}
				if (Resource.Id.middleLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--midLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midLeftGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--midLeftGV.oneVal;
				}
				if (Resource.Id.middleRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--midRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midRightGV.xVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--midRightGV.oneVal;
				}
			}
		}
	}
}
