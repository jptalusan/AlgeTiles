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

		//This hack will suck, but im tired, better to create a new class with + and - objects though
		private Number xVar;
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

			xVar = new Number(0, 0);
		}

		private void button_click(object sender, EventArgs e)
		{
			var button = sender as Button;
			Log.Debug(TAG, button.Text);
			if (Constants.NEW_Q == button.Text)
			{
				isFirstAnswerCorrect = false;
				int[] vars = AlgorithmUtilities.RNG(Constants.FACTOR, numberOfVariables);
				//(ax + b)(cx + d)
				if (Constants.ONE_VAR == numberOfVariables)
				{
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

				//TODO: Remove all views in grids?
			} else if (Constants.CHK == button.Text)
			{
				if (!isFirstAnswerCorrect)
				{
					//TODO take into account the negative values (other side of the gridlayouts)
					if (((xVarCountFirstGroup == varsA &&
							oneVarCountFirstGroup == varsB) &&
							(xVarCountSecondGroup == varsC &&
							oneVarCountSecondGroup == varsD)) ||
							((xVarCountSecondGroup == varsA &&
							oneVarCountSecondGroup == varsB) &&
							(xVarCountFirstGroup == varsC &&
							oneVarCountFirstGroup == varsD)))
					{
						isFirstAnswerCorrect = true;

						upperLeftGrid.Drag += GridLayout_Drag;
						upperRightGrid.Drag += GridLayout_Drag;

						lowerLeftGrid.Drag += GridLayout_Drag;
						lowerRightGrid.Drag += GridLayout_Drag;

						upperMiddleGrid.Drag -= GridLayout_Drag;
						middleLeftGrid.Drag -= GridLayout_Drag;
						middleRightGrid.Drag -= GridLayout_Drag;
						lowerMiddleGrid.Drag -= GridLayout_Drag;

						//TODO: light up and play sound
						Toast.MakeText(Application.Context, "correct", ToastLength.Short).Show();
					} else
					{
						upperLeftGrid.Drag -= GridLayout_Drag;
						upperRightGrid.Drag -= GridLayout_Drag;

						lowerLeftGrid.Drag -= GridLayout_Drag;
						lowerRightGrid.Drag -= GridLayout_Drag;

						Toast.MakeText(Application.Context, "incorrect", ToastLength.Short).Show();
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

		//TODO: when original tile is rotated, cloned image is not rotated
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
						//result.Text = currentButtonType;
					}
					Log.Debug(TAG, "DragAction.Started");
					break;
				case DragAction.Entered:
					Log.Debug(TAG, "DragAction.Entered");
					v.SetBackgroundResource(Resource.Drawable.shape_droptarget);
					break;
				case DragAction.Exited:
					Log.Debug(TAG, "DragAction.Exited");
					currentOwner = (ViewGroup)view.Parent;
					hasButtonBeenDroppedInCorrectzone = false;
					v.SetBackgroundResource(Resource.Drawable.shape);
					break;
				case DragAction.Drop:
					Log.Debug(TAG, "DragAction.Drop");
					if (null != drag_data)
					{
						currentButtonType = drag_data.GetItemAt(0).Text;
						//result.Text = currentButtonType + ": " + numberOfCloneButtons;
					}

					ImageView imageView = new ImageView(this);
					Boolean wasImageDropped = false;

					//Check if x_tile is rotated before fitting or rotate before dropping automatically
					if ((v.Id == Resource.Id.upperMiddle ||
						 v.Id == Resource.Id.middleLeft ||
						 v.Id == Resource.Id.middleRight ||
						 v.Id == Resource.Id.lowerMiddle) &&
						currentButtonType.Equals("x2_tile"))
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
						if ((currentButtonType.Equals("x_tile") && !rotateToggle.Checked) ||
								(currentButtonType.Equals("x_tile_rot") && rotateToggle.Checked))
						{
							imageView.SetBackgroundResource(Resource.Drawable.x_tile_rot);
							if (v.Id == Resource.Id.middleLeft)
								++xNegVarCountFirstGroup;
							if (v.Id == Resource.Id.middleRight)
								++xVarCountFirstGroup;
						}
						else if (currentButtonType.Equals(Constants.ONE_TILE))
						{
							int resID = Resources.GetIdentifier(currentButtonType, "drawable", PackageName);
							imageView.SetBackgroundResource(resID);
							if (v.Id == Resource.Id.middleLeft)
								++oneNegVarCountFirstGroup;
							if (v.Id == Resource.Id.middleRight)
								++oneVarCountFirstGroup;

						}
						wasImageDropped = true;
					}
					//Second group x
					else if (!isFirstAnswerCorrect &&
							(v.Id == Resource.Id.upperMiddle ||
							v.Id == Resource.Id.lowerMiddle))
					{
						int cnt = 0;
						for (int i = 0; i < v.ChildCount; ++i)
						{
							++cnt;
						}
						Log.Debug(TAG, cnt + "");

						if ((currentButtonType.Equals("x_tile_rot") && rotateToggle.Checked) ||
								(currentButtonType.Equals("x_tile") && !rotateToggle.Checked))
						{
							imageView.SetBackgroundResource(Resource.Drawable.x_tile);
							if (v.Id == Resource.Id.lowerMiddle)
								++xNegVarCountSecondGroup;
							if (v.Id == Resource.Id.upperMiddle)
								++xVarCountSecondGroup;
						} else if (currentButtonType.Equals(Constants.ONE_TILE))
						{
							int resID = Resources.GetIdentifier(currentButtonType, "drawable", PackageName);
							imageView.SetBackgroundResource(resID);
							if (v.Id == Resource.Id.lowerMiddle)
								++oneNegVarCountSecondGroup;
							if (v.Id == Resource.Id.upperMiddle)
								++oneVarCountSecondGroup;
						}
						wasImageDropped = true;
					}

					if (wasImageDropped)
					{
						imageView.Tag = currentButtonType;

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
					Log.Debug(TAG, "x group 1: " + xVarCountFirstGroup);
					Log.Debug(TAG, "1 group 1: " + oneVarCountFirstGroup);

					Log.Debug(TAG, "x group 2: " + xVarCountSecondGroup);
					Log.Debug(TAG, "1 group 2: " + oneVarCountSecondGroup);

					break;
				case DragAction.Ended:
					Log.Debug(TAG, "DragAction.Ended");
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
					data = ClipData.NewPlainText(BUTTON_TYPE, "tile_1");
					break;
				case Resource.Id.x_tile:
					data = ClipData.NewPlainText(BUTTON_TYPE, "x_tile");
					break;
				case Resource.Id.x_tile_rot:
					data = ClipData.NewPlainText(BUTTON_TYPE, "x_tile_rot");
					break;
				case Resource.Id.x2_tile:
					data = ClipData.NewPlainText(BUTTON_TYPE, "x2_tile");
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
				Log.Debug(TAG, "Remove");
				Log.Debug(TAG, id + "");
				Log.Debug(TAG, Resource.Id.tile_1 + "");
				Log.Debug(TAG, Resource.Id.x_tile + "");
				Log.Debug(TAG, Resource.Id.x2_tile + "");

				//if (touchedImageView.Tag.ToString() == "tile_1")
				//	--numberOfTile_1s;
				//if (touchedImageView.Tag.ToString() == "x_tile")
				//	--numberOfX_tiles;
				//if (touchedImageView.Tag.ToString() == "x2_tile")
				//	--numberOfX2_tiles;
				//result.Text = "tile_1: " + numberOfTile_1s + ", x_tile: " + numberOfX_tiles + ", x2_tile: " + numberOfX2_tiles;
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
	}
}