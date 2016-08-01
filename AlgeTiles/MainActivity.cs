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
namespace AlgeTiles
{
	[Activity(Label = "AlgeTiles", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity//, View.IOnTouchListener, View.IOnDragListener
	{
		public static Context context { get; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);
			// Create your application here
			FindViewById(Resource.Id.myimage1).Touch += ImageView_Touch;
			FindViewById(Resource.Id.myimage2).Touch += ImageView_Touch;
			FindViewById(Resource.Id.myimage3).Touch += ImageView_Touch;
			FindViewById(Resource.Id.myimage4).Touch += ImageView_Touch;

			//FindViewById(Resource.Id.myimage1).SetOnTouchListener(this);
			//FindViewById(Resource.Id.myimage2).SetOnTouchListener(this);
			//FindViewById(Resource.Id.myimage3).SetOnTouchListener(this);
			//FindViewById(Resource.Id.myimage4).SetOnTouchListener(this);

			FindViewById(Resource.Id.topleft).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.topright).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.bottomleft).Drag += GridLayout_Drag;
			FindViewById(Resource.Id.bottomright).Drag += GridLayout_Drag;

			//FindViewById(Resource.Id.topleft).SetOnDragListener(this);
			//FindViewById(Resource.Id.topright).SetOnDragListener(this);
			//FindViewById(Resource.Id.bottomleft).SetOnDragListener(this);
			//FindViewById(Resource.Id.bottomright).SetOnDragListener(this);

			//shape = GetDrawable(Resource.Drawable.shape);
			//normalShape = GetDrawable(Resource.Drawable.shape_droptarget);
		}

		//public bool OnTouch(View view, MotionEvent e)
		//{
		//	if (e.Action == MotionEventActions.Down)
		//	{
		//		ClipData data = ClipData.NewPlainText("", "");
		//		View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(view);
		//		view.StartDrag(data, shadowBuilder, view, 0);
		//		view.Visibility = ViewStates.Invisible;
		//		return true;
		//	}
		//	else
		//	{
		//		return false;
		//	}
		//}

		//public bool OnDrag(View v, DragEvent e)
		//{
		//	//int shape = (int)typeof(Resource.Drawable).GetField("shape").GetValue(null);
		//	//int normalShape = (int)typeof(Resource.Drawable).GetField("normalShape").GetValue(null);
		//	var action = e.Action;
		//	switch (action)
		//	{
		//		case DragAction.Started:
		//			break;
		//		case DragAction.Entered:
		//			v.SetBackgroundDrawable(shape);
		//			//v.SetBackgroundResource(shape);
		//			break;
		//		case DragAction.Exited:
		//			v.SetBackgroundDrawable(normalShape);
		//			//v.SetBackgroundResource(normalShape);
		//			break;
		//		case DragAction.Drop:
		//			//Current view (i.e. item dropped into this)
		//			View view = (View)e.LocalState;
		//			////Get layout owning the view
		//			//ViewGroup owner = (ViewGroup)view.Parent;
		//			////Remove the view from the owner
		//			//owner.RemoveView(view);

		//			//Cloning imageView
		//			//ImageView Setup

		//			ImageView imageView = new ImageView(this);
		//			imageView.SetBackgroundDrawable(GetDrawable(Resource.Drawable.Icon));
		//			//setting image position
		//			LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
		//					ViewGroup.LayoutParams.WrapContent);
		//			imageView.LayoutParameters = linearLayoutParams;
		//			imageView.Touch += ImageView_Touch;
		//			//imageView.SetOnTouchListener(this);
		//			//Pick neww view (currently on drag)
		//			GridLayout container = (GridLayout)v;
		//			//Add new view
		//			//container.AddView(view);
		//			container.AddView(imageView);
		//			view.Visibility = ViewStates.Visible;
		//			break;
		//		case DragAction.Ended:
		//			v.SetBackgroundDrawable(normalShape);
		//			//v.SetBackgroundResource(normalShape);
		//			break;
		//		default:
		//			break;
		//	}
		//	return true;
		//}
		
		//Add case where the image did not exit
		private void GridLayout_Drag(object sender, Android.Views.View.DragEventArgs e)
		{
			//var shape = context.Resources.GetDrawable(Resource.Drawable.shape, null);
			//var normalShape = context.Resources.GetDrawable(Resource.Drawable.shape_droptarget, null);
			var v = (GridLayout)sender;
			switch (e.Event.Action)
			{
				case DragAction.Started:
					break;
				case DragAction.Entered:
					v.SetBackgroundResource(Resource.Drawable.shape);
					break;
				case DragAction.Exited:
					v.SetBackgroundResource(Resource.Drawable.shape_droptarget);
					break;
				case DragAction.Drop:
					View view = (View)e.Event.LocalState;

					ImageView imageView = new ImageView(this);
					imageView.SetBackgroundResource(Resource.Drawable.Icon);

					LinearLayout.LayoutParams linearLayoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
							ViewGroup.LayoutParams.WrapContent);
					imageView.LayoutParameters = linearLayoutParams;

					imageView.Touch += ImageView_Touch;

					GridLayout container = (GridLayout)v;
					container.AddView(imageView);
					view.Visibility = ViewStates.Visible;
					break;
				case DragAction.Ended:
					v.SetBackgroundResource(Resource.Drawable.shape_droptarget);
					break;
				default:
					break;
			}
		}

		//http://stackoverflow.com/questions/18836432/how-to-find-the-view-of-a-button-in-its-click-eventhandler
		private void ImageView_Touch(object sender, Android.Views.View.TouchEventArgs e)
		{
			var imageViewTouch = (ImageView)sender;
			ClipData data = ClipData.NewPlainText("", "");
			View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(imageViewTouch);
			imageViewTouch.StartDrag(data, shadowBuilder, imageViewTouch, 0);
			imageViewTouch.Visibility = ViewStates.Invisible;
		}
	}
}