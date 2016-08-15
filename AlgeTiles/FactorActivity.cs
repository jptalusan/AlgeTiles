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
	[Activity(Label = "FactorActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class FactorActivity : Activity//, View.IOnTouchListener, View.IOnDragListener
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
		private int numberOfVariables = 0;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
			ActionBar.Hide();
			SetContentView(Resource.Layout.Factory);
			// Create your application here
			result = (TextView) FindViewById(Resource.Id.result);

			tile_1 = (ImageButton)FindViewById(Resource.Id.tile_1);
			x_tile = (ImageButton)FindViewById(Resource.Id.x_tile);
			x_tile_rot = (ImageButton)FindViewById(Resource.Id.x_tile_rot);
			x2_tile = (ImageButton)FindViewById(Resource.Id.x2_tile);

			tile_1.LongClick += tile_LongClick;
			x_tile.LongClick += tile_LongClick;
			x_tile_rot.LongClick += tile_LongClick;
			x2_tile.LongClick += tile_LongClick;

			FindViewById(Resource.Id.upperLeft).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.upperMiddle).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.upperRight).Drag += GridLayout_Drag;

			//Restrict x^2 from being dragged here.
			FindViewById(Resource.Id.middleLeft).Drag += GridLayout_Drag;
			//FindViewById(Resource.Id.middleMiddle).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.middleRight).Drag += GridLayout_Drag;

			FindViewById(Resource.Id.lowerLeft).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.lowerMiddle).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.lowerRight).Drag += GridLayout_Drag;


			removeToggle = (ToggleButton)FindViewById(Resource.Id.remove);
			dragToggle = (ToggleButton)FindViewById(Resource.Id.drag);
			rotateToggle = (ToggleButton)FindViewById(Resource.Id.rotate);

			removeToggle.CheckedChange += toggle_click;
			dragToggle.CheckedChange += toggle_click;
			rotateToggle.CheckedChange += toggle_click;

			result.Text = "tile_1: " + numberOfTile_1s + ", x_tile: " + numberOfX_tiles + ", x2_tile: " + numberOfX2_tiles;

			numberOfVariables = Intent.GetIntExtra(Constants.VARIABLE_COUNT, 0);
			newQuestionButton = (Button)FindViewById<Button>(Resource.Id.new_question_button);
			newQuestionButton.Click += button_click;
		}

		private void button_click(object sender, EventArgs e)
		{
			var button = sender as Button;
			Log.Debug(TAG, button.Text);
			if (Constants.NEW_Q == button.Text)
			{
				int[] questionArray = AlgorithmUtilities.RNG(Constants.FACTOR, numberOfVariables);
				foreach (int i in questionArray)
				{
					Log.Debug(TAG, i + "");
				}
			}
		}

		private void toggle_click(object sender, EventArgs e)
		{
			ToggleButton clicked_toggle = (sender) as ToggleButton;
			int buttonText = clicked_toggle.Id;
			switch(buttonText)
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
					} else
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
						result.Text = currentButtonType;
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
						result.Text = currentButtonType + ": " + numberOfCloneButtons;
					}

					//Check if x_tile is rotated before fitting or rotate before dropping automatically
					if ((v.Id == Resource.Id.upperMiddle ||
						v.Id == Resource.Id.middleLeft ||
						v.Id == Resource.Id.middleRight ||
						v.Id == Resource.Id.lowerMiddle) &&
						currentButtonType.Equals("x2_tile"))
					{
						//Do nothing
					} else
					{
						// Gets the item containing the dragged data
						ImageView imageView = new ImageView(this);
						if (currentButtonType.Equals("x_tile") && 
							!rotateToggle.Checked && 
							(v.Id == Resource.Id.middleLeft ||
							v.Id == Resource.Id.middleRight))
						{
							imageView.SetBackgroundResource(Resource.Drawable.x_tile_rot);
						}
						else if (currentButtonType.Equals("x_tile_rot") &&
							rotateToggle.Checked &&
							(v.Id == Resource.Id.upperMiddle ||
							v.Id == Resource.Id.lowerMiddle))
						{
							imageView.SetBackgroundResource(Resource.Drawable.x_tile);
						}
						else
						{
							int resID = Resources.GetIdentifier(currentButtonType, "drawable", PackageName);
							imageView.SetBackgroundResource(resID);
						}

						imageView.Tag = currentButtonType;

						//Probably should put weight or alignment here
						LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(
							ViewGroup.LayoutParams.WrapContent,
							ViewGroup.LayoutParams.WrapContent);
						imageView.LayoutParameters = linearLayoutParams;
						imageView.LongClick += clonedImageView_Touch;

						if (currentButtonType.Equals("tile_1"))
							++numberOfTile_1s;
						if (currentButtonType.Equals("x_tile") || currentButtonType.Equals("x_tile_rot"))
							++numberOfX_tiles;
						if (currentButtonType.Equals("x2_tile"))
							++numberOfX2_tiles;


						GridLayout container = (GridLayout)v;
						container.AddView(imageView);
						view.Visibility = ViewStates.Visible;
						v.SetBackgroundResource(Resource.Drawable.shape);
						hasButtonBeenDroppedInCorrectzone = true;
					}
					result.Text = "tile_1: " + numberOfTile_1s + ", x_tile: " + numberOfX_tiles + ", x2_tile: " + numberOfX2_tiles;

					break;
				case DragAction.Ended:
					Log.Debug(TAG, "DragAction.Ended");
					v.SetBackgroundResource(Resource.Drawable.shape);
					if (!hasButtonBeenDroppedInCorrectzone && 
						currentButtonType.Equals(CLONED_BUTTON))
					{
						currentOwner.RemoveView(view);
					} else
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
			if(removeToggle.Checked)
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

				if (touchedImageView.Tag.ToString() == "tile_1")
					--numberOfTile_1s;
				if (touchedImageView.Tag.ToString() == "x_tile")
					--numberOfX_tiles;
				if (touchedImageView.Tag.ToString() == "x2_tile")
					--numberOfX2_tiles;
				result.Text = "tile_1: " + numberOfTile_1s + ", x_tile: " + numberOfX_tiles + ", x2_tile: " + numberOfX2_tiles;
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