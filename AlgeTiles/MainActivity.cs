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
namespace AlgeTiles
{
	[Activity(Label = "AlgeTiles", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
	public class MainActivity : Activity//, View.IOnTouchListener, View.IOnDragListener
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
		
		private Switch deleteSwitch;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Factory);
			// Create your application here
			result = (TextView) FindViewById(Resource.Id.result);

			FindViewById(Resource.Id.button1).LongClick += ImageView_Touch;

			FindViewById(Resource.Id.upperLeft).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.upperRight).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.lowerLeft).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.lowerRight).Drag += GridLayout_Drag;

			deleteSwitch = (Switch)FindViewById(Resource.Id.deleteSwitch);
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
						result.Text = currentButtonType;
					}
					Log.Debug(TAG, "DragAction.Started");
					break;
				case DragAction.Entered:
					Log.Debug(TAG, "DragAction.Entered");
					v.SetBackgroundResource(Resource.Drawable.shape);
					break;
				case DragAction.Exited:
					Log.Debug(TAG, "DragAction.Exited");
					currentOwner = (ViewGroup)view.Parent;
					hasButtonBeenDroppedInCorrectzone = false;
					v.SetBackgroundResource(Resource.Drawable.shape_droptarget);
					break;
				case DragAction.Drop:
					Log.Debug(TAG, "DragAction.Drop");
					// Gets the item containing the dragged data
					if (null != drag_data)
					{
						currentButtonType = drag_data.GetItemAt(0).Text;
						result.Text = currentButtonType + ": " + numberOfCloneButtons;
					}

					ImageView imageView = new ImageView(this);
					imageView.SetBackgroundResource(Resource.Drawable.Icon);

					LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(
						ViewGroup.LayoutParams.WrapContent,
						ViewGroup.LayoutParams.WrapContent);
					imageView.LayoutParameters = linearLayoutParams;

					imageView.LongClick += clonedImageView_Touch;
					++numberOfCloneButtons;

					GridLayout container = (GridLayout)v;
					container.AddView(imageView);
					view.Visibility = ViewStates.Visible;

					hasButtonBeenDroppedInCorrectzone = true;

					//TODO: Fix the logic between this and the one in onEnded
					//check if the current Owner and new owner are the same as well as check the button type
					//if (currentOwner == v &&
					//	currentButtonType.Equals(CLONED_BUTTON))
					//{
					//	ImageView imageView = new ImageView(this);
					//	imageView.SetBackgroundResource(Resource.Drawable.Icon);

					//	LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(
					//		ViewGroup.LayoutParams.WrapContent,
					//		ViewGroup.LayoutParams.WrapContent);
					//	imageView.LayoutParameters = linearLayoutParams;

					//	imageView.LongClick += clonedImageView_Touch;

					//	GridLayout container = (GridLayout)v;
					//	container.AddView(imageView);
					//	view.Visibility = ViewStates.Visible;					
					//	hasButtonBeenDroppedInCorrectzone = false;
					//} else if (currentOwner != v && 
					//	currentButtonType.Equals(ORIGINAL_BUTTON))
					//{
					//	ImageView imageView = new ImageView(this);
					//	imageView.SetBackgroundResource(Resource.Drawable.Icon);

					//	LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(
					//		ViewGroup.LayoutParams.WrapContent,
					//		ViewGroup.LayoutParams.WrapContent);
					//	imageView.LayoutParameters = linearLayoutParams;

					//	imageView.LongClick += clonedImageView_Touch;

					//	GridLayout container = (GridLayout)v;
					//	container.AddView(imageView);
					//	view.Visibility = ViewStates.Visible;

					//	hasButtonBeenDroppedInCorrectzone = true;
					//} else
					//{
					//	view.Visibility = ViewStates.Visible;
					//	hasButtonBeenDroppedInCorrectzone = false;
					//}			
					break;
				case DragAction.Ended:
					Log.Debug(TAG, "DragAction.Ended");
					v.SetBackgroundResource(Resource.Drawable.shape_droptarget);
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

		//http://stackoverflow.com/questions/18836432/how-to-find-the-view-of-a-button-in-its-click-eventhandler
		private void ImageView_Touch(object sender, View.LongClickEventArgs e)
		{
			Log.Debug(TAG, "ImageView_Touch");
			var imageViewTouch = (sender) as ImageView;
			ClipData data = ClipData.NewPlainText(BUTTON_TYPE, ORIGINAL_BUTTON);
			View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(imageViewTouch);
			imageViewTouch.StartDrag(data, shadowBuilder, imageViewTouch, 0);
			//imageViewTouch.Visibility = ViewStates.Invisible;
		}

		private void clonedImageView_Touch(object sender, View.LongClickEventArgs e)
		{
			Log.Debug(TAG, "Switch: " + deleteSwitch.Checked);
			if(deleteSwitch.Checked)
			{
				var imageViewTouch = (sender) as ImageView;
				ViewGroup vg = (ViewGroup)imageViewTouch.Parent;
				vg.RemoveView(imageViewTouch);
				imageViewTouch.Visibility = ViewStates.Gone;
			} else
			{
				Log.Debug(TAG, "clonedImageView_Touch");
				var imageViewTouch = (sender) as ImageView;
				ClipData data = ClipData.NewPlainText(BUTTON_TYPE, CLONED_BUTTON);
				View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(imageViewTouch);
				imageViewTouch.StartDrag(data, shadowBuilder, imageViewTouch, 0);
				imageViewTouch.Visibility = ViewStates.Invisible;
			}
		}
	}
}