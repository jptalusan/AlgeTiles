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

//https://developer.xamarin.com/recipes/android/other_ux/gestures/detect_a_touch/
//Set tag
//http://stackoverflow.com/questions/5291726/what-is-the-main-purpose-of-settag-gettag-methods-of-view
//Multiple buttons 1 handler
//http://stackoverflow.com/questions/3814234/how-can-i-subscribe-multiple-buttons-to-the-same-event-handler-and-act-according
//use switch case
//Context current activity
//http://stackoverflow.com/questions/25613225/get-current-activity-from-application-context-monoandroid
//Deprecated
//http://stackoverflow.com/questions/29041027/android-getresources-getdrawable-deprecated-api-22
//Sample drag and drop
//http://pumpingco.de/adding-drag-and-drop-to-your-android-application-with-xamarin/
//Problem, restricting in own layout parent only
//http://stackoverflow.com/questions/17111135/android-constrain-drag-and-drop-to-a-bounding-box
//http://stackoverflow.com/questions/20491071/how-can-i-restrict-the-dragzone-for-a-view-in-android
//BUGs
//On dragging original button, it is still labeled as such (onDrop)

//TODO: Gridlayout stretches depending on imageview inside but still retains the same row/col count
//http://stackoverflow.com/questions/21950937/how-to-prevent-cells-in-gridlayout-from-stretching
//TODO: Working, multiply not yet
namespace AlgeTiles
{
	[Activity(Label = "MultiplyActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class MultiplyActivity : Activity//, View.IOnTouchListener, View.IOnDragListener
	{
		private static string TAG = "AlgeTiles";
		public static Context context { get; }
		private static string BUTTON_TYPE = "BUTTON_TYPE";
		private static string CLONED_BUTTON = "CLONE_BUTTON";
		private static string ORIGINAL_BUTTON = "ORIGINAL_BUTTON";
		private TextView result;
		private Boolean hasButtonBeenDroppedInCorrectzone = false;
		private string currentButtonType = "";
		private ViewGroup currentOwner;
		private int numberOfCloneButtons = 1;

		private ToggleButton removeToggle;
		private ToggleButton dragToggle;
		private ToggleButton rotateToggle;

		private ImageButton tile_1;
		private ImageButton x_tile;
		private ImageButton x_tile_rot;
		private ImageButton x2_tile;

		private int numberOfTile_1s = 0;
		private int numberOfX_tiles = 0;
		private int numberOfX2_tiles = 0;

		private Button newQuestionButton;
		private Button refreshButton;
		private Button checkButton;

		private int numberOfVariables = 0;

		private Boolean isFirstAnswerCorrect = false;

		private GridLayout upperLeftGrid;
		private GridLayout upperMiddleGrid;
		private GridLayout upperRightGrid;
		private GridLayout middleLeftGrid;
		private GridLayout middleRightGrid;
		private GridLayout lowerLeftGrid;
		private GridLayout lowerMiddleGrid;
		private GridLayout lowerRightGrid;

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

		private int xVarCountFirstGroup = 0;
		private int oneVarCountFirstGroup = 0;
		private int xNegVarCountFirstGroup = 0;
		private int oneNegVarCountFirstGroup = 0;

		private int xVarCountSecondGroup = 0;
		private int oneVarCountSecondGroup = 0;
		private int xNegVarCountSecondGroup = 0;
		private int oneNegVarCountSecondGroup = 0;

		private int varsA = 0;
		private int varsB = 0;
		private int varsC = 0;
		private int varsD = 0;
		private int varsE = 0;
		private int varsF = 0;

		private int[] vars = { };
		List<int> expandedVars = new List<int>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			ActionBar.Hide();
			SetContentView(Resource.Layout.Factory);
			// Create your application here
			result = (TextView)FindViewById(Resource.Id.result);

			tile_1 = (ImageButton)FindViewById(Resource.Id.tile_1);
			x_tile = (ImageButton)FindViewById(Resource.Id.x_tile);
			x_tile_rot = (ImageButton)FindViewById(Resource.Id.x_tile_rot);
			x2_tile = (ImageButton)FindViewById(Resource.Id.x2_tile);

			tile_1.LongClick += tile_LongClick;
			x_tile.LongClick += tile_LongClick;
			x_tile_rot.LongClick += tile_LongClick;
			x2_tile.LongClick += tile_LongClick;

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

			//Together form one Part of the formula
			upperMiddleGrid.Drag += GridLayout_Drag;
			lowerMiddleGrid.Drag += GridLayout_Drag;

			//Together form one Part of the formula
			middleLeftGrid.Drag += GridLayout_Drag;
			middleRightGrid.Drag += GridLayout_Drag;
			
			removeToggle = (ToggleButton)FindViewById(Resource.Id.remove);
			dragToggle = (ToggleButton)FindViewById(Resource.Id.drag);
			rotateToggle = (ToggleButton)FindViewById(Resource.Id.rotate);

			removeToggle.CheckedChange += toggle_click;
			dragToggle.CheckedChange += toggle_click;
			rotateToggle.CheckedChange += toggle_click;

			//result.Text = "tile_1: " + numberOfTile_1s + ", x_tile: " + numberOfX_tiles + ", x2_tile: " + numberOfX2_tiles;

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
		}

		private void button_click(object sender, EventArgs e)
		{
			var button = sender as Button;
			Log.Debug(TAG, button.Text);
			if (Constants.NEW_Q == button.Text)
			{
				isFirstAnswerCorrect = false;
				bool isVarsValid = false;
				while (!isVarsValid)
				{
					vars = AlgorithmUtilities.RNG(Constants.FACTOR, numberOfVariables);
					if (vars[0] != 0 &&
						vars[1] != 0 &&
						vars[2] != 0 &&
						vars[3] != 0)
						isVarsValid = true;
				}
				
				//(ax + b)(cx + d)
				if (Constants.ONE_VAR == numberOfVariables)
				{
					midUpGV.init();
					midLowGV.init();
					midLeftGV.init();
					midRightGV.init();

					upperLeftGV.init();
					upperRightGV.init();
					lowerLeftGV.init();
					lowerRightGV.init();

					xVarCountFirstGroup = 0;
					oneVarCountFirstGroup = 0;

					xVarCountSecondGroup = 0;
					oneVarCountSecondGroup = 0;

					varsA = vars[0];
					varsB = vars[1];
					varsC = vars[2];
					varsD = vars[3];

					//Don't display number if 0
					string ax = vars[0] != 0 ? vars[0] + "x" : "";
					string b = vars[1] != 0 ? " + " + vars[1] + "" : "";
					string cx = vars[2] != 0 ? vars[2] + "x" : "";
					string d = vars[3] != 0 ? " + " + vars[3] + "" : "";

					result.Text = "(" + ax + b + ")(" + cx + d + ")";
				}

				foreach (int i in vars)
				{
					Log.Debug(TAG, i + "");
				}
			} else if (Constants.REFR == button.Text)
			{
				xVarCountFirstGroup = 0;
				oneVarCountFirstGroup = 0;

				xVarCountSecondGroup = 0;
				oneVarCountSecondGroup = 0;

				//Just better to loop through all views and remove views in a loop
				for (int i = 0; i < upperLeftGrid.ChildCount; ++i)
				{
					View v = upperLeftGrid.GetChildAt(i);
					upperLeftGrid.RemoveView(v);
				}

			} else if (Constants.CHK == button.Text)
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

						upperMiddleGrid.Drag -= GridLayout_Drag;
						middleLeftGrid.Drag -= GridLayout_Drag;
						middleRightGrid.Drag -= GridLayout_Drag;
						lowerMiddleGrid.Drag -= GridLayout_Drag;

						expandedVars = AlgorithmUtilities.expandingVars(vars);
						foreach (var i in expandedVars)
							Log.Debug(TAG, i + "");
						Toast.MakeText(Application.Context, "1:correct", ToastLength.Short).Show();
					}
					else
					{
						upperLeftGrid.Drag -= GridLayout_Drag;
						upperRightGrid.Drag -= GridLayout_Drag;

						lowerLeftGrid.Drag -= GridLayout_Drag;
						lowerRightGrid.Drag -= GridLayout_Drag;

						Toast.MakeText(Application.Context, "1:incorrect", ToastLength.Short).Show();
					}
				} else
				{					
					GridValue[] gvArr = { upperLeftGV, upperRightGV, lowerLeftGV, lowerRightGV };


					for (int i = 0; i < gvArr.Length; ++i)
						Log.Debug(TAG, gvArr[i].ToString());
					if (AlgorithmUtilities.isSecondAnswerCorrect(expandedVars, gvArr, numberOfVariables))
					{
						Toast.MakeText(Application.Context, "2:correct", ToastLength.Short).Show();
						//TODO:Ask user to then fill up the edit text boxes with the expanded equation
					} else
					{
						Toast.MakeText(Application.Context, "2:incorrect", ToastLength.Short).Show();
					}

				}

			}
		}

		private void toggle_click(object sender, EventArgs e)
		{
			ToggleButton clicked_toggle = (sender) as ToggleButton;
			int buttonText = clicked_toggle.Id;
			switch (buttonText)
			{
				case Resource.Id.remove:
					if (dragToggle.Checked)
						dragToggle.Checked = false;
					if (rotateToggle.Checked)
						rotateToggle.Checked = false;
					break;
				case Resource.Id.drag:
					if (removeToggle.Checked)
						removeToggle.Checked = false;
					if (rotateToggle.Checked)
						rotateToggle.Checked = false;
					break;
				case Resource.Id.rotate:
					//Also rotate original tiles
					if (rotateToggle.Checked)
					{
						x_tile.Visibility = ViewStates.Gone;
						x_tile_rot.Visibility = ViewStates.Visible;
					}
					else
					{
						x_tile.Visibility = ViewStates.Visible;
						x_tile_rot.Visibility = ViewStates.Gone;
					}

					if (removeToggle.Checked)
						removeToggle.Checked = false;
					if (dragToggle.Checked)
						dragToggle.Checked = false;
					break;
			}
		}

		//Add case where the image did not exit
		//Probably check if the parent GridLayout - sender, is equal to the receiver (at ondrop) if not then do nothing.
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
					//Log.Debug(TAG, "DragAction.Started");
					break;
				case DragAction.Entered:
					//Log.Debug(TAG, "DragAction.Entered");
					v.SetBackgroundResource(Resource.Drawable.shape_droptarget);
					break;
				case DragAction.Exited:
					//Log.Debug(TAG, "DragAction.Exited");
					currentOwner = (ViewGroup)view.Parent;
					hasButtonBeenDroppedInCorrectzone = false;
					v.SetBackgroundResource(Resource.Drawable.shape);
					break;
				case DragAction.Drop:
					//Log.Debug(TAG, "DragAction.Drop");
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
						if ((currentButtonType.Equals(Constants.X_TILE) && !rotateToggle.Checked) ||
								(currentButtonType.Equals(Constants.X_TILE_ROT) && rotateToggle.Checked))
						{
							imageView.SetBackgroundResource(Resource.Drawable.x_tile_rot);
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
						if ((currentButtonType.Equals(Constants.X_TILE_ROT) && rotateToggle.Checked) ||
								(currentButtonType.Equals(Constants.X_TILE) && !rotateToggle.Checked))
						{
							imageView.SetBackgroundResource(Resource.Drawable.x_tile);
						} else if (currentButtonType.Equals(Constants.ONE_TILE))
						{
							int resID = Resources.GetIdentifier(currentButtonType, "drawable", PackageName);
							imageView.SetBackgroundResource(resID);
						}
						wasImageDropped = true;
					}

					if (wasImageDropped)
					{
						imageView.Tag = currentButtonType;
						Log.Debug(TAG, "Check: " + currentButtonType);
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
					//Log.Debug(TAG, "x group 1: " + xVarCountFirstGroup);
					//Log.Debug(TAG, "1 group 1: " + oneVarCountFirstGroup);

					//Log.Debug(TAG, "x group 2: " + xVarCountSecondGroup);
					//Log.Debug(TAG, "1 group 2: " + oneVarCountSecondGroup);

					break;
				case DragAction.Ended:
					//Log.Debug(TAG, "DragAction.Ended");
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
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.ONE_TILE);
					break;
				case Resource.Id.x_tile:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.X_TILE);
					break;
				case Resource.Id.x_tile_rot:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.X_TILE_ROT);
					break;
				case Resource.Id.x2_tile:
					data = ClipData.NewPlainText(BUTTON_TYPE, Constants.X2_TILE);
					break;
			}
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

				//var touchedImageViewTag = touchedImageView.GetTag(0).ToString();
				int id = touchedImageView.Id;
				//Log.Debug(TAG, "Remove");
				//Log.Debug(TAG, id + "");
				//Log.Debug(TAG, Resource.Id.tile_1 + "");
				//Log.Debug(TAG, Resource.Id.x_tile + "");
				//Log.Debug(TAG, Resource.Id.x2_tile + "");

				checkWhichParentAndUpdate(vg.Id, touchedImageView.Tag.ToString(), Constants.REMOVE);
			}

			if (dragToggle.Checked)
			{
				Log.Debug(TAG, "Switch: Drag");
			}

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
				if(Resource.Id.upperLeft == id)
				{
					if (Constants.X2_TILE == tile)
						++upperLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++upperLeftGV.xVal;
					if (Constants.ONE_TILE == tile)
						++upperLeftGV.oneVal;
				}
				if (Resource.Id.upperRight == id)
				{
					if (Constants.X2_TILE == tile)
						++upperRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++upperRightGV.xVal;
					if (Constants.ONE_TILE == tile)
						++upperRightGV.oneVal;
				}
				if (Resource.Id.lowerLeft == id)
				{
					if (Constants.X2_TILE == tile)
						++lowerLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++lowerLeftGV.xVal;
					if (Constants.ONE_TILE == tile)
						++lowerLeftGV.oneVal;
				}
				if (Resource.Id.lowerRight == id)
				{
					if (Constants.X2_TILE == tile)
						++lowerRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++lowerRightGV.xVal;
					if (Constants.ONE_TILE == tile)
						++lowerRightGV.oneVal;
				}

				//CENTER
				if (Resource.Id.upperMiddle == id)
				{
					if (Constants.X2_TILE == tile)
						++midUpGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midUpGV.xVal;
					if (Constants.ONE_TILE == tile)
						++midUpGV.oneVal;
				}
				if (Resource.Id.lowerMiddle == id)
				{
					if (Constants.X2_TILE == tile)
						++midLowGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midLowGV.xVal;
					if (Constants.ONE_TILE == tile)
						++midLowGV.oneVal;
				}
				if (Resource.Id.middleLeft == id)
				{
					if (Constants.X2_TILE == tile)
						++midLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midLeftGV.xVal;
					if (Constants.ONE_TILE == tile)
						++midLeftGV.oneVal;
				}
				if (Resource.Id.middleRight == id)
				{
					if (Constants.X2_TILE == tile)
						++midRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						++midRightGV.xVal;
					if (Constants.ONE_TILE == tile)
						++midRightGV.oneVal;
				}

			}
			//REMOVE
			else if (Constants.REMOVE == process)
			{
				if (Resource.Id.upperLeft == id)
				{
					if (Constants.X2_TILE == tile)
						--upperLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--upperLeftGV.xVal;
					if (Constants.ONE_TILE == tile)
						--upperLeftGV.oneVal;
				}
				if (Resource.Id.upperRight == id)
				{
					if (Constants.X2_TILE == tile)
						--upperRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--upperRightGV.xVal;
					if (Constants.ONE_TILE == tile)
						--upperRightGV.oneVal;
				}
				if (Resource.Id.lowerLeft == id)
				{
					if (Constants.X2_TILE == tile)
						--lowerLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--lowerLeftGV.xVal;
					if (Constants.ONE_TILE == tile)
						--lowerLeftGV.oneVal;
				}
				if (Resource.Id.lowerRight == id)
				{
					if (Constants.X2_TILE == tile)
						--lowerRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--lowerRightGV.xVal;
					if (Constants.ONE_TILE == tile)
						--lowerRightGV.oneVal;
				}

				//CENTER
				if (Resource.Id.upperMiddle == id)
				{
					if (Constants.X2_TILE == tile)
						--midUpGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midUpGV.xVal;
					if (Constants.ONE_TILE == tile)
						--midUpGV.oneVal;
				}
				if (Resource.Id.lowerMiddle == id)
				{
					if (Constants.X2_TILE == tile)
						--midLowGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midLowGV.xVal;
					if (Constants.ONE_TILE == tile)
						--midLowGV.oneVal;
				}
				if (Resource.Id.middleLeft == id)
				{
					if (Constants.X2_TILE == tile)
						--midLeftGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midLeftGV.xVal;
					if (Constants.ONE_TILE == tile)
						--midLeftGV.oneVal;
				}
				if (Resource.Id.middleRight == id)
				{
					if (Constants.X2_TILE == tile)
						--midRightGV.x2Val;
					if (Constants.X_TILE == tile || Constants.X_TILE_ROT == tile)
						--midRightGV.xVal;
					if (Constants.ONE_TILE == tile)
						--midRightGV.oneVal;
				}

			}
		}
	}
}