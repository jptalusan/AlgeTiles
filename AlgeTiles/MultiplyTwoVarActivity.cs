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
	[Activity(Label = "MultiplyTwoVarActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class MultiplyTwoVarActivity : Activity
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

		private ImageButton tile_1;
		private ImageButton x_tile;
		private ImageButton y_tile;
		private ImageButton x2_tile;
		private ImageButton y2_tile;
		private ImageButton xy_tile;

		private ImageButton tile_1_rot;
		private ImageButton x_tile_rot;
		private ImageButton y_tile_rot;
		private ImageButton xy_tile_rot;

		private Button newQuestionButton;
		private Button refreshButton;
		private Button checkButton;

		private int numberOfVariables = 0;

		private Boolean isFirstAnswerCorrect = false;
		private Boolean isSecondAnswerCorrect = false;
		private Boolean isThirdAnswerCorrect = true;

		private GridLayout upperLeftGrid;
		private GridLayout upperMiddleGrid;
		private GridLayout upperRightGrid;
		private GridLayout middleLeftGrid;
		private GridLayout middleRightGrid;
		private GridLayout lowerLeftGrid;
		private GridLayout lowerMiddleGrid;
		private GridLayout lowerRightGrid;

		//Four outer grids
		private GridValue2Var upperLeftGV;
		private GridValue2Var upperRightGV;
		private GridValue2Var lowerLeftGV;
		private GridValue2Var lowerRightGV;

		//Four center grids
		private GridValue2Var midUpGV;
		private GridValue2Var midLowGV;
		private GridValue2Var midLeftGV;
		private GridValue2Var midRightGV;

		private List<int> vars = new List<int>();
		private List<int> expandedVars = new List<int>();
		private List<GridLayout> innerGridLayoutList = new List<GridLayout>();
		private List<GridLayout> outerGridLayoutList = new List<GridLayout>();
		private List<GridValue2Var> gridValue2VarList = new List<GridValue2Var>();

		private MediaPlayer correct;
		private MediaPlayer incorrect;

		private EditText x2ET;
		private EditText y2ET;
		private EditText xyET;
		private EditText xET;
		private EditText yET;
		private EditText oneET;

		private ScrollView sv;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			ActionBar.Hide();

			numberOfVariables = Intent.GetIntExtra(Constants.VARIABLE_COUNT, 0);

			SetContentView(Resource.Layout.MultiplyTwoVar);

			// Create your application here
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

			tile_1.LongClick += tile_LongClick;
			x_tile.LongClick += tile_LongClick;
			y_tile.LongClick += tile_LongClick;
			x2_tile.LongClick += tile_LongClick;
			y2_tile.LongClick += tile_LongClick;
			xy_tile.LongClick += tile_LongClick;

			tile_1_rot.LongClick += tile_LongClick;
			x_tile_rot.LongClick += tile_LongClick;
			y_tile_rot.LongClick += tile_LongClick;
			xy_tile_rot.LongClick += tile_LongClick;

			upperLeftGrid = FindViewById<GridLayout>(Resource.Id.upperLeft);
			upperMiddleGrid = FindViewById<GridLayout>(Resource.Id.upperMiddle);
			upperRightGrid = FindViewById<GridLayout>(Resource.Id.upperRight);

			//Restrict x^2 from being dragged here.
			middleLeftGrid = FindViewById<GridLayout>(Resource.Id.middleLeft);
			//FindViewById(Resource.Id.middleMiddle).Drag += GridLayout_Drag;
			middleRightGrid = FindViewById<GridLayout>(Resource.Id.middleRight);

			lowerLeftGrid = FindViewById<GridLayout>(Resource.Id.lowerLeft);
			lowerMiddleGrid = FindViewById<GridLayout>(Resource.Id.lowerMiddle);
			lowerRightGrid = FindViewById<GridLayout>(Resource.Id.lowerRight);

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

			removeToggle.Click += toggle_click;
			dragToggle.Click += toggle_click;
			rotateToggle.Click += toggle_click;
			
			newQuestionButton = (Button)FindViewById<Button>(Resource.Id.new_question_button);
			refreshButton = (Button)FindViewById<Button>(Resource.Id.refresh_button);
			checkButton = (Button)FindViewById<Button>(Resource.Id.check_button);

			newQuestionButton.Click += button_click;
			refreshButton.Click += button_click;
			checkButton.Click += button_click;

			upperLeftGV = new GridValue2Var();
			upperRightGV = new GridValue2Var();
			lowerLeftGV = new GridValue2Var();
			lowerRightGV = new GridValue2Var();

			midUpGV = new GridValue2Var();
			midLowGV = new GridValue2Var();
			midLeftGV = new GridValue2Var();
			midRightGV = new GridValue2Var();

			gridValue2VarList.Add(upperLeftGV);
			gridValue2VarList.Add(upperRightGV);
			gridValue2VarList.Add(lowerLeftGV);
			gridValue2VarList.Add(lowerRightGV);

			gridValue2VarList.Add(midUpGV);
			gridValue2VarList.Add(midLowGV);
			gridValue2VarList.Add(midLeftGV);
			gridValue2VarList.Add(midRightGV);

			vars = AlgorithmUtilities.RNG(Constants.MULTIPLY, numberOfVariables);
			setupQuestionString(vars);

			correct = MediaPlayer.Create(this, Resource.Raw.correct);
			incorrect = MediaPlayer.Create(this, Resource.Raw.wrong);

			x2ET = FindViewById<EditText>(Resource.Id.x2_value);
			y2ET = FindViewById<EditText>(Resource.Id.y2_value);
			xyET = FindViewById<EditText>(Resource.Id.xy_value);
			xET = FindViewById<EditText>(Resource.Id.x_value);
			yET = FindViewById<EditText>(Resource.Id.y_value);
			oneET = FindViewById<EditText>(Resource.Id.one_value);

			sv = FindViewById<ScrollView>(Resource.Id.sv);

			refreshScreen(Constants.MULTIPLY, gridValue2VarList, innerGridLayoutList, outerGridLayoutList);
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
			switch(button.Text)
			{
				case Constants.NEW_Q:
					setupNewQuestion();
					refreshScreen(Constants.MULTIPLY, gridValue2VarList, innerGridLayoutList, outerGridLayoutList);
					sv.ScrollTo(0, 0);
					break;
				case Constants.REFR:
					refreshScreen(Constants.MULTIPLY, gridValue2VarList, innerGridLayoutList, outerGridLayoutList);
					break;
				case Constants.CHK:
					checkAnswers();
					break;
			}
		}

		private void refreshScreen(string ActivityType, List<GridValue2Var> gvList, List<GridLayout> inGLList, List<GridLayout> outGLList)
		{
			x2ET.Enabled = false;
			y2ET.Enabled = false;
			xyET.Enabled = false;
			xET.Enabled = false;
			yET.Enabled = false;
			oneET.Enabled = false;

			x2ET.Text = "";
			y2ET.Text = "";
			xyET.Text = "";
			xET.Text = "";
			yET.Text = "";
			oneET.Text = "";

			isFirstAnswerCorrect = false;
			isSecondAnswerCorrect = false;
			isThirdAnswerCorrect = true;

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
				GridValue2Var[] gvArr = { midUpGV, midLowGV, midLeftGV, midRightGV };
				if (AlgorithmUtilities.isFirstAnswerCorrect(vars, gvArr))
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
							var iv = innerGridLayoutList[i].GetChildAt(j) as ImageView;
							iv.LongClick -= clonedImageView_Touch;
						}
					}

					expandedVars = AlgorithmUtilities.expandingVars(vars);
					foreach (var i in expandedVars)
						Log.Debug(TAG, "Expanded in activity: " + i);
					Toast.MakeText(Application.Context, "1:correct", ToastLength.Short).Show();

					//TODO:accomodate for 2 variables (right now just for one)
					x2ET.Enabled = true;
					y2ET.Enabled = true;
					xyET.Enabled = true;
					xET.Enabled = true;
					yET.Enabled = true;
					oneET.Enabled = true;

					correct.Start();
				}
				else
				{
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
						outerGridLayoutList[i].Drag -= GridLayout_Drag;

					incorrectPrompt(innerGridLayoutList);

					Toast.MakeText(Application.Context, "1:incorrect", ToastLength.Short).Show();
				}
			}
			else if (isFirstAnswerCorrect)
			{
				Log.Debug(TAG, "isSecondAnswerCorrect branch");
				GridValue2Var[] gvArr = { upperLeftGV, upperRightGV, lowerLeftGV, lowerRightGV };


				for (int i = 0; i < gvArr.Length; ++i)
					Log.Debug(TAG, gvArr[i].ToString());
				if (AlgorithmUtilities.isSecondAnswerCorrect(expandedVars, gvArr, numberOfVariables))
				{
					Toast.MakeText(Application.Context, "2:correct", ToastLength.Short).Show();
					correct.Start();

					//Loop through inner and prevent deletions by removing: clonedImageView_Touch
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
					{
						outerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.ok);
						outerGridLayoutList[i].Drag -= GridLayout_Drag;
						for (int j = 0; j < outerGridLayoutList[i].ChildCount; ++j)
						{
							var iv = outerGridLayoutList[i].GetChildAt(j) as ImageView;
							iv.LongClick -= clonedImageView_Touch;
						}
					}
				}
				else
				{

					Toast.MakeText(Application.Context, "2:incorrect", ToastLength.Short).Show();
					incorrectPrompt(outerGridLayoutList);
				}
			}
			else
			{
				//TODO: Accomodate for two variables
				int[] answer = new int[3];
				int temp = 0;
				answer[0] = int.TryParse(x2ET.Text, out temp) ? temp : 0;
				answer[1] = int.TryParse(xET.Text, out temp) ? temp : 0;
				answer[2] = int.TryParse(oneET.Text, out temp) ? temp : 0; ;
				for (int i = 0; i < expandedVars.Count; ++i)
				{
					if (expandedVars[i] != answer[i])
						isThirdAnswerCorrect = false;
				}
				if (isThirdAnswerCorrect)
				{
					Toast.MakeText(Application.Context, "3:correct", ToastLength.Short).Show();
					correct.Start();
					//TODO: Refresh then new question?
				}
				else
				{
					Toast.MakeText(Application.Context, "3:incorrect", ToastLength.Short).Show();
					incorrect.Start();
				}
			}
		}

		public async void incorrectPrompt(List<GridLayout> gvList)
		{
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

			for (int i = 0; i < gridValue2VarList.Count; ++i)
			{
				gridValue2VarList[i].init();
			}

			setupQuestionString(vars);

			foreach (int i in vars)
			{
				Log.Debug(TAG, i + "");
			}
		}

		private void setupQuestionString(List<int> vars)
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

			output += ")(";

			if (dx != 0)
				output += dx + "x+";
			if (ey != 0)
				output += ey + "y+";
			if (f != 0)
				output += f;

			output += ")";
			output = output.Replace(" ", "");
			output = output.Replace("+-", "-");
			output = output.Replace("+", " + ");
			output = output.Replace("-", " - ");
			result.Text = output;
		}

		private void GridLayout_Drag(object sender, Android.Views.View.DragEventArgs e)
		{
			var v = (GridLayout)sender;
			View view = (View)e.Event.LocalState;
			var button_type = result.Text;
			var drag_data = e.Event.ClipData;

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
				case DragAction.Drop:
					if (null != drag_data)
					{
						currentButtonType = drag_data.GetItemAt(0).Text;
					}

					ImageView imageView = new ImageView(this);
					Boolean wasImageDropped = false;

					//Check if x_tile is rotated before fitting or rotate before dropping automatically
					if ((v.Id == Resource.Id.upperMiddle ||
						 v.Id == Resource.Id.middleLeft ||
						 v.Id == Resource.Id.middleRight ||
						 v.Id == Resource.Id.lowerMiddle) &&
						currentButtonType.Equals(Constants.X2_TILE) &&
						currentButtonType.Equals(Constants.Y2_TILE) &&
						currentButtonType.Equals(Constants.XY_TILE))
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
						int resID = Resources.GetIdentifier(currentButtonType, "drawable", PackageName);
						imageView.SetBackgroundResource(resID);
						wasImageDropped = true;
					}
					//Handle auto rotate for x_tile (middle)
					//First group x
					else if (!isFirstAnswerCorrect &&
							(v.Id == Resource.Id.middleLeft ||
							 v.Id == Resource.Id.middleRight))
					{
						if (v.Id == Resource.Id.middleLeft)
							imageView.RotationY = 180;
						if ((currentButtonType.Equals(Constants.X_TILE) && !rotateToggle.Checked) ||
							(currentButtonType.Equals(Constants.X_TILE_ROT) && rotateToggle.Checked))
							
						{
							imageView.SetBackgroundResource(Resource.Drawable.x_tile_rot);
						}
						else if ((currentButtonType.Equals(Constants.Y_TILE) && !rotateToggle.Checked) ||
							(currentButtonType.Equals(Constants.Y_TILE_ROT) && rotateToggle.Checked))
						{
							imageView.SetBackgroundResource(Resource.Drawable.y_tile_rot);
						}
						else if (currentButtonType.Equals(Constants.ONE_TILE))
						{
							int resID = Resources.GetIdentifier(currentButtonType, "drawable", PackageName);
							imageView.SetBackgroundResource(resID);
						}
						wasImageDropped = true;
					}
					//Second group x
					else if (!isFirstAnswerCorrect &&
							(v.Id == Resource.Id.upperMiddle ||
							v.Id == Resource.Id.lowerMiddle))
					{
						if (v.Id == Resource.Id.upperMiddle)
							imageView.RotationX = 180;
						if ((currentButtonType.Equals(Constants.X_TILE_ROT) && rotateToggle.Checked) ||
								(currentButtonType.Equals(Constants.X_TILE) && !rotateToggle.Checked))
						{
							imageView.SetBackgroundResource(Resource.Drawable.x_tile);
						}
						else if ((currentButtonType.Equals(Constants.Y_TILE_ROT) && rotateToggle.Checked) ||
								 (currentButtonType.Equals(Constants.Y_TILE) && !rotateToggle.Checked))
						{
							imageView.SetBackgroundResource(Resource.Drawable.y_tile);
						}
						else if (currentButtonType.Equals(Constants.ONE_TILE))
						{
							int resID = Resources.GetIdentifier(currentButtonType, "drawable", PackageName);
							imageView.SetBackgroundResource(resID);
						}
						wasImageDropped = true;
					}

					if (wasImageDropped)
					{
						imageView.Tag = currentButtonType;
						checkWhichParentAndUpdate(v.Id, currentButtonType, Constants.ADD);
						//Probably should put weight or alignment here
						LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(
							ViewGroup.LayoutParams.WrapContent,
							ViewGroup.LayoutParams.WrapContent);
						imageView.LayoutParameters = linearLayoutParams;
						imageView.LongClick += clonedImageView_Touch;

						GridLayout container = (GridLayout)v;
						container.AddView(imageView);
						view.Visibility = ViewStates.Visible;
						v.SetBackgroundResource(Resource.Drawable.shape);
						hasButtonBeenDroppedInCorrectzone = true;
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
				case Resource.Id.y_tile:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.Y_TILE);
					break;
				case Resource.Id.y_tile_rot:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.Y_TILE_ROT);
					break;
				case Resource.Id.xy_tile_rot:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.XY_TILE_ROT);
					break;
				case Resource.Id.xy_tile:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.XY_TILE);
					break;
				case Resource.Id.y2_tile:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.Y2_TILE);
					break;				
				case Resource.Id.x2_tile:
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
			var touchedImageView = (sender) as ImageView;
			ViewGroup vg = (ViewGroup)touchedImageView.Parent;
			if (removeToggle.Checked)
			{
				Log.Debug(TAG, "Switch: Remove");
				vg.RemoveView(touchedImageView);
				touchedImageView.Visibility = ViewStates.Gone;
				Vibrator vibrator = (Vibrator)GetSystemService(Context.VibratorService);
				vibrator.Vibrate(30);

				int id = touchedImageView.Id;

				checkWhichParentAndUpdate(vg.Id, touchedImageView.Tag.ToString(), Constants.SUBTRACT);
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
				//OUTER
				if (Resource.Id.upperLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++upperLeftGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++upperLeftGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++upperLeftGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++upperLeftGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++upperLeftGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++upperLeftGV.oneVal;
				}
				if (Resource.Id.upperRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++upperRightGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++upperRightGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++upperRightGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++upperRightGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++upperRightGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++upperRightGV.oneVal;
				}
				if (Resource.Id.lowerLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++lowerLeftGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++lowerLeftGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++lowerLeftGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++lowerLeftGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++lowerLeftGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++lowerLeftGV.oneVal;
				}
				if (Resource.Id.lowerRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++lowerRightGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++lowerRightGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++lowerRightGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++lowerRightGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++lowerRightGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++lowerRightGV.oneVal;
				}

				//CENTER
				if (Resource.Id.upperMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++midUpGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++midUpGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++midUpGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midUpGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++midUpGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++midUpGV.oneVal;
				}
				if (Resource.Id.lowerMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++midLowGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++midLowGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++midLowGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midLowGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++midLowGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++midLowGV.oneVal;
				}
				if (Resource.Id.middleLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++midLeftGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++midLeftGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++midLeftGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midLeftGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++midLeftGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++midLeftGV.oneVal;
				}
				if (Resource.Id.middleRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						++midRightGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						++midRightGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						++midRightGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midRightGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						++midRightGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						++midRightGV.oneVal;
				}
			}
			//REMOVE
			else if (Constants.SUBTRACT == process)
			{
				//OUTER
				if (Resource.Id.upperLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--upperLeftGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--upperLeftGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--upperLeftGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--upperLeftGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--upperLeftGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--upperLeftGV.oneVal;
				}
				if (Resource.Id.upperRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--upperRightGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--upperRightGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--upperRightGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--upperRightGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--upperRightGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--upperRightGV.oneVal;
				}
				if (Resource.Id.lowerLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--lowerLeftGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--lowerLeftGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--lowerLeftGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--lowerLeftGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--lowerLeftGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--lowerLeftGV.oneVal;
				}
				if (Resource.Id.lowerRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--lowerRightGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--lowerRightGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--lowerRightGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--lowerRightGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--lowerRightGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--lowerRightGV.oneVal;
				}

				//CENTER
				if (Resource.Id.upperMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--midUpGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--midUpGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--midUpGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midUpGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--midUpGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--midUpGV.oneVal;
				}
				if (Resource.Id.lowerMiddle == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--midLowGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--midLowGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--midLowGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midLowGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--midLowGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--midLowGV.oneVal;
				}
				if (Resource.Id.middleLeft == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--midLeftGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--midLeftGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--midLeftGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midLeftGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--midLeftGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--midLeftGV.oneVal;
				}
				if (Resource.Id.middleRight == id)
				{
					if (Constants.X2_TILE == tile || Constants.X2_TILE_ROT == tile)
						--midRightGV.x2Val;
					if (Constants.Y2_TILE == tile || Constants.Y2_TILE_ROT == tile)
						--midRightGV.y2Val;
					if (Constants.XY_TILE == tile || Constants.XY_TILE_ROT == tile)
						--midRightGV.xyVal;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midRightGV.xVal;
					if (Constants.Y_TILE == tile || Constants.Y_TILE_ROT == tile)
						--midRightGV.yVal;
					if (Constants.ONE_TILE == tile || Constants.ONE_TILE_ROT == tile)
						--midRightGV.oneVal;
				}
			}
		}
	}
}