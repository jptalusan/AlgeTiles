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

namespace AlgeTiles
{
	[Activity(Label = "MultiplyActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class MultiplyActivity : Activity
	{
		private static string TAG = "AlgeTiles:Multiply";
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

		private EditText x2ET;
		private EditText xET;
		private EditText oneET;

		private List<EditText> editTextList = new List<EditText>();

		private bool isFirstTime = false;

		private int heightInPx = 0;
		private int widthInPx = 0;

		private List<RectTile> upperRightRectTileList = new List<RectTile>();
		private List<RectTile> upperLeftRectTileList = new List<RectTile>();
		private List<RectTile> lowerRightRectTileList = new List<RectTile>();
		private List<RectTile> lowerLeftRectTileList = new List<RectTile>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			ActionBar.Hide();

			numberOfVariables = Intent.GetIntExtra(Constants.VARIABLE_COUNT, 0);

			SetContentView(Resource.Layout.Multiply);

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
			upperMiddleGrid.Drag += GridLayout_Drag;
			lowerMiddleGrid.Drag += GridLayout_Drag;

			//Together form one Part of the formula
			middleLeftGrid.Drag += GridLayout_Drag;
			middleRightGrid.Drag += GridLayout_Drag;

			//Shade red the other grids
			for (int i = 0; i < outerGridLayoutList.Count; ++i)
				outerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.unavailable);

			removeToggle = (ToggleButton)FindViewById(Resource.Id.remove);
			dragToggle = (ToggleButton)FindViewById(Resource.Id.drag);
			rotateToggle = (ToggleButton)FindViewById(Resource.Id.rotate);
			muteToggle = (ToggleButton)FindViewById(Resource.Id.mute);

			removeToggle.Click += toggle_click;
			dragToggle.Click += toggle_click;
			rotateToggle.Click += toggle_click;
			muteToggle.Click += toggle_click;

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
			xET = FindViewById<EditText>(Resource.Id.x_value);
			oneET = FindViewById<EditText>(Resource.Id.one_value);

			editTextList.Add(x2ET);
			editTextList.Add(xET);
			editTextList.Add(oneET);

			refreshScreen(Constants.MULTIPLY, gridValueList, innerGridLayoutList, outerGridLayoutList);
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
					refreshScreen(Constants.MULTIPLY, gridValueList, innerGridLayoutList, outerGridLayoutList);
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

			x2ET.Enabled = false;
			xET.Enabled = false;
			oneET.Enabled = false;

			x2ET.Text = "";
			xET.Text = "";
			oneET.Text = "";

			isFirstAnswerCorrect = false;
			isSecondAnswerCorrect = false;
			isThirdAnswerCorrect = false;

			for (int i = 0; i < inGLList.Count; ++i)
			{
				inGLList[i].SetBackgroundResource(Resource.Drawable.shape);
				inGLList[i].Drag -= GridLayout_Drag;
			}

			for (int i = 0; i < outGLList.Count; ++i)
			{
				outGLList[i].SetBackgroundResource(Resource.Drawable.shape);
				outGLList[i].Drag -= GridLayout_Drag;
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
					inGLList[i].Drag -= GridLayout_Drag;
				}

				for (int i = 0; i < outGLList.Count; ++i)
				{
					outGLList[i].SetBackgroundResource(Resource.Drawable.shape);
					outGLList[i].Drag += GridLayout_Drag;
				}
			}
			else
			{
				for (int i = 0; i < inGLList.Count; ++i)
				{
					inGLList[i].SetBackgroundResource(Resource.Drawable.shape);
					inGLList[i].Drag += GridLayout_Drag;
				}

				for (int i = 0; i < outGLList.Count; ++i)
				{
					outGLList[i].SetBackgroundResource(Resource.Drawable.unavailable);
					outGLList[i].Drag -= GridLayout_Drag;
				}
			}
		}

		private void checkAnswers()
		{
			if (!isFirstAnswerCorrect)
			{
				GridValue[] gvArr = { midUpGV, midLowGV, midLeftGV, midRightGV };
				if (AlgorithmUtilities.isFirstAnswerCorrect(vars, gvArr, numberOfVariables))
				{
					isFirstAnswerCorrect = true;

					//Change color of draggable areas to signify "Done/Correct"
					upperLeftGrid.Drag += GridLayout_Drag;
					upperRightGrid.Drag += GridLayout_Drag;

					lowerLeftGrid.Drag += GridLayout_Drag;
					lowerRightGrid.Drag += GridLayout_Drag;

					//Shade red the other grids
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
						outerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.shape);

					//Loop through inner and prevent deletions by removing: clonedImageView_Touch
					for (int i = 0; i < innerGridLayoutList.Count; ++i)
					{
						innerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.ok);
						innerGridLayoutList[i].Drag -= GridLayout_Drag;
						for (int j = 0; j < innerGridLayoutList[i].ChildCount; ++j)
						{
							var iv = innerGridLayoutList[i].GetChildAt(j) as AlgeTilesTextView;
							iv.LongClick -= clonedImageView_Touch;
						}
					}

					expandedVars = AlgorithmUtilities.expandingVars(vars);
					foreach (var i in expandedVars)
						Log.Debug(TAG, i + "");
					Toast.MakeText(Application.Context, "1:correct", ToastLength.Short).Show();

					x2ET.Enabled = true;
					xET.Enabled = true;
					oneET.Enabled = true;

					if (!muteToggle.Checked)
						correct.Start();

					generateInnerLayoutTileArrays();
					upperRightGrid.drawRects(upperRightRectTileList);
					upperLeftGrid.drawRects(upperLeftRectTileList);
					lowerRightGrid.drawRects(lowerRightRectTileList);
					lowerLeftGrid.drawRects(lowerLeftRectTileList);
				}
				else
				{
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
						outerGridLayoutList[i].Drag -= GridLayout_Drag;

					incorrectPrompt(innerGridLayoutList);

					Toast.MakeText(Application.Context, "1:incorrect", ToastLength.Short).Show();
				}
			}
			else if (!isSecondAnswerCorrect)
			{
				Log.Debug(TAG, "isSecondAnswerCorrect branch");
				GridValue[] gvArr = { upperLeftGV, upperRightGV, lowerLeftGV, lowerRightGV };

				for (int i = 0; i < gvArr.Length; ++i)
					Log.Debug(TAG, gvArr[i].ToString());
				if (AlgorithmUtilities.isSecondAnswerCorrect(expandedVars, gvArr, numberOfVariables))
				{
					//Cancelling out
					int posX = 0;
					ViewGroup posXVG = null;
					if (upperRightGV.xVal != 0)
					{
						posX = upperRightGV.xVal;
						posXVG = upperRightGrid;
					} else
					{
						if (lowerLeftGV.xVal != 0)
						{
							posX = lowerLeftGV.xVal;
							posXVG = lowerLeftGrid;
						}
					}

					int negX = 0;
					ViewGroup negXVG = null;
					if (upperLeftGV.xVal != 0)
					{
						negX = upperLeftGV.xVal;
						negXVG = upperLeftGrid;
					}
					else
					{
						if (lowerRightGV.xVal != 0)
						{
							negX = lowerRightGV.xVal;
							negXVG = lowerRightGrid;
						}
					}

					if (posX != 0 && negX != 0)
					{
						Log.Debug(TAG, "Cancelling out: " + posX + ", " + negX);
						int xToRemove = posX > negX ? negX : posX;
						Log.Debug(TAG, "To remove: " + xToRemove);
						List<AlgeTilesTextView> tobeRemoved = new List<AlgeTilesTextView>();
						for (int j = 0; j < posXVG.ChildCount; ++j)
						{
							AlgeTilesTextView alIV = posXVG.GetChildAt(j) as AlgeTilesTextView;
							if (alIV.getTileType().Equals(Constants.X_TILE) ||
								alIV.getTileType().Equals(Constants.X_TILE_ROT))
							{
								tobeRemoved.Add(alIV);
							}
						}

						List<AlgeTilesTextView> negTobeRemoved = new List<AlgeTilesTextView>();
						for (int j = 0; j < negXVG.ChildCount; ++j)
						{
							AlgeTilesTextView negalIV = negXVG.GetChildAt(j) as AlgeTilesTextView;
							if (negalIV.getTileType().Equals(Constants.X_TILE) ||
								negalIV.getTileType().Equals(Constants.X_TILE_ROT))
							{
								negTobeRemoved.Add(negalIV);
							}
						}

						for (int j = 0; j < xToRemove; ++j)
						{
							posXVG.RemoveView(tobeRemoved[j]);
							negXVG.RemoveView(negTobeRemoved[j]);
						}
					}
					//End Cancelling out
					Toast.MakeText(Application.Context, "2:correct", ToastLength.Short).Show();
					if (!muteToggle.Checked)
						correct.Start();

					//Loop through inner and prevent deletions by removing: clonedImageView_Touch
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
					{
						outerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.ok);
						outerGridLayoutList[i].Drag -= GridLayout_Drag;
						for (int j = 0; j < outerGridLayoutList[i].ChildCount; ++j)
						{
							var iv = outerGridLayoutList[i].GetChildAt(j) as AlgeTilesTextView;
							iv.LongClick -= clonedImageView_Touch;
						}
					}
					isSecondAnswerCorrect = true;
				}
				else
				{

					Toast.MakeText(Application.Context, "2:incorrect", ToastLength.Short).Show();
					incorrectPrompt(outerGridLayoutList);
				}
			}
			else if (!isThirdAnswerCorrect)
			{
				//TODO: Accomodate for two variables
				int[] answer = new int[3];
				int temp = 0;
				answer[0] = int.TryParse(x2ET.Text, out temp) ? temp : 0;
				answer[1] = int.TryParse(xET.Text, out temp) ? temp : 0;
				answer[2] = int.TryParse(oneET.Text, out temp) ? temp : 0;
				if (Math.Abs(answer[0] + answer[1] + answer[2]) == 0)
				{
					isThirdAnswerCorrect = false;
				}
				else
				{
					for (int i = 0; i < expandedVars.Count; ++i)
					{
						if (expandedVars[i] != answer[i])
							isThirdAnswerCorrect = false;
					}
					isThirdAnswerCorrect = true;
				}

				if (isThirdAnswerCorrect)
				{
					Toast.MakeText(Application.Context, "3:correct", ToastLength.Short).Show();
					for (int i = 0; i < editTextList.Count; ++i)
					{
						editTextList[i].SetBackgroundResource(Resource.Drawable.ok);
						editTextList[i].Enabled = false;
					}
					if (!muteToggle.Checked)
						correct.Start();
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
			isFirstAnswerCorrect = false;
			vars = AlgorithmUtilities.RNG(Constants.MULTIPLY, numberOfVariables);

			//(ax + b)(cx + d)
			if (Constants.ONE_VAR == numberOfVariables)
			{
				for (int i = 0; i < gridValueList.Count; ++i)
				{
					gridValueList[i].init();
				}

				setupQuestionString(vars);
			}

			foreach (int i in vars)
			{
				Log.Debug(TAG, i + "");
			}
		}

		private void setupQuestionString(List<int> vars)
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

		//Add case where the image did not exit
		//Probably check if the parent GridLayout - sender, is equal to the receiver (at ondrop) if not then do nothing.
		private void GridLayout_Drag(object sender, Android.Views.View.DragEventArgs e)
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
					//TODO: how to know which grid is which?
					//Log.Debug(TAG, "DragAction.Started");
					break;
				case DragAction.Entered:
					//Log.Debug(TAG, "DragAction.Entered");
					v.SetBackgroundResource(Resource.Drawable.shape_droptarget);
					break;
				case DragAction.Exited:
					//Log.Debug(TAG, "DragAction.Exited");
					//upperRightGrid.clearRects(heightInPx, widthInPx);
					currentOwner = (ViewGroup)view.Parent;
					hasButtonBeenDroppedInCorrectzone = false;
					v.SetBackgroundResource(Resource.Drawable.shape);
					break;
				case DragAction.Location:
					x = e.Event.GetX(); //width
					y = e.Event.GetY(); //height
					break;
				case DragAction.Drop:
					//Log.Debug(TAG, "DragAction.Drop");
					if (null != drag_data)
					{
						currentButtonType = drag_data.GetItemAt(0).Text;
					}

					AlgeTilesTextView algeTilesIV = new AlgeTilesTextView(this);
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
					else if (!isFirstAnswerCorrect &&
							(v.Id == Resource.Id.upperLeft ||
							 v.Id == Resource.Id.upperRight ||
							 v.Id == Resource.Id.lowerLeft ||
							 v.Id == Resource.Id.lowerRight))
					{
						//Do nothing
					}
					//Handle drops for second part of problem (expanded form)
					else if (isFirstAnswerCorrect &&
							(v.Id == Resource.Id.upperLeft ||
							 v.Id == Resource.Id.upperRight ||
							 v.Id == Resource.Id.lowerLeft ||
							 v.Id == Resource.Id.lowerRight))
					{
						wasImageDropped = true;
					}
					//Handle auto rotate for x_tile (middle)
					//First group x
					else if (!isFirstAnswerCorrect &&
							(v.Id == Resource.Id.middleLeft ||
							 v.Id == Resource.Id.middleRight))
					{
						if (v.Id == Resource.Id.middleLeft)
							algeTilesIV.RotationY = 180;
						wasImageDropped = true;
						isDroppedAtCenter = true;
					}
					//Second group x
					else if (!isFirstAnswerCorrect &&
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
						Log.Debug(TAG, "Dropped: " + currentButtonType);
						double heightFactor = 0;
						double widthFactor = 0;
						switch (currentButtonType)
						{
							case Constants.X2_TILE:
							case Constants.X2_TILE_ROT:
								algeTilesIV.SetBackgroundResource(Resource.Drawable.x);
								algeTilesIV.Text = "x2";
								heightFactor = Constants.X_LONG_SIDE;
								widthFactor = Constants.X_LONG_SIDE;
								break;
							case Constants.X_TILE:
								algeTilesIV.SetBackgroundResource(Resource.Drawable.x);
								algeTilesIV.Text = "x";
								heightFactor = Constants.X_LONG_SIDE;
								widthFactor = Constants.ONE_SIDE;
								break;
							case Constants.X_TILE_ROT:
								algeTilesIV.SetBackgroundResource(Resource.Drawable.x);
								algeTilesIV.Text = "x";
								heightFactor = Constants.ONE_SIDE;
								widthFactor = Constants.X_LONG_SIDE;
								break;
							case Constants.ONE_TILE:
							case Constants.ONE_TILE_ROT:
								algeTilesIV.SetBackgroundResource(Resource.Drawable.one);
								algeTilesIV.Text = "1";
								heightFactor = Constants.ONE_SIDE;
								widthFactor = Constants.ONE_SIDE;
								break;
						}

						x = e.Event.GetX(); //width
						y = e.Event.GetY(); //height

						if (!isDroppedAtCenter)
						{
							Rect r = checkIfUserDropsOnRect(v.Id, currentButtonType, x, y);
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
									gParms.Height = (int) (heightInPx / heightFactor);
									gParms.Width = (int) (heightInPx / widthFactor);
								}
								else
								{
									gParms.Width = (int) (heightInPx / heightFactor);
									gParms.Height = (int)(heightInPx / widthFactor);
								}
							}
							else
							{
								gParms.SetGravity(GravityFlags.FillHorizontal);
								if (rotateToggle.Checked)
								{
									gParms.Width = (int)(heightInPx / heightFactor);
									gParms.Height = (int)(heightInPx / widthFactor);
								}
								else
								{
									gParms.Height = (int)(heightInPx / heightFactor);
									gParms.Width = (int)(heightInPx / widthFactor);
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
				default:
					break;
			}
		}

		private Rect checkIfUserDropsOnRect(int vId, string tileType, float x, float y)
		{
			if (Resource.Id.upperLeft == vId)
			{
				foreach(RectTile r in upperLeftRectTileList)
				{
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						r.setTilePresence(true);
						return r.getRect();
					}
				}
			}

			if (Resource.Id.upperRight == vId)
			{
				foreach (RectTile r in upperRightRectTileList)
				{
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						r.setTilePresence(true);
						return r.getRect();
					}
				}
			}

			if (Resource.Id.lowerLeft == vId)
			{
				foreach (RectTile r in lowerLeftRectTileList)
				{
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						r.setTilePresence(true);
						return r.getRect();
					}
				}
			}

			if (Resource.Id.lowerRight == vId)
			{
				foreach (RectTile r in lowerRightRectTileList)
				{
					if (r.isPointInsideRect(x, y) && r.isTileTypeSame(tileType) && !r.getTilePresence())
					{
						r.setTilePresence(true);
						return r.getRect();
					}
				}
			}

			return null;
		}

		private void generateInnerLayoutTileArrays()
		{
			List<string> midUp = new List<string>();
			List<string> midLeft = new List<string>();
			List<string> midRight = new List<string>();
			List<string> midDown = new List<string>();

			List<List<string>> output = new List<List<string>>();
			output.Add(midUp);
			output.Add(midLeft);
			output.Add(midRight);
			output.Add(midDown);

			//midup, midleft, midright, middown
			for (int i = 0; i < innerGridLayoutList.Count; ++i)
			{
				GridLayout gl = (GridLayout)innerGridLayoutList[i];
				for (int j = 0; j < gl.ChildCount; ++j)
				{
					AlgeTilesTextView al = gl.GetChildAt(j) as AlgeTilesTextView;
					switch (al.getTileType())
					{
						case Constants.X_TILE:
						case Constants.X_TILE_ROT:
							output[i].Add(al.getTileType());
							break;
						case Constants.Y_TILE:
						case Constants.Y_TILE_ROT:
							output[i].Add(al.getTileType());
							break;
						case Constants.ONE_TILE:
						case Constants.ONE_TILE_ROT:
							output[i].Add(al.getTileType());
							break;
					}
				}
			}

			int height = heightInPx;
			int width = widthInPx;

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

		private void clonedImageView_Touch(object sender, View.LongClickEventArgs e)
		{
			var touchedImageView = (sender) as AlgeTilesTextView;
			ViewGroup vg = (ViewGroup)touchedImageView.Parent;
			if (removeToggle.Checked)
			{
				Log.Debug(TAG, "Switch: Remove");
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