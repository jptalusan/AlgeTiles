//using System;
//using Android.App;
//using Android.Content;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Android.OS;

////https://developer.xamarin.com/recipes/android/other_ux/gestures/detect_a_touch/
////http://pumpingco.de/adding-drag-and-drop-to-your-android-application-with-xamarin/

//namespace AlgeTiles
//{
//    [Activity(Label = "AlgeTiles", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
//	public class MainActivity : Activity
//    {
//        private TextView result;
//		private FrameLayout upperLeft;
//		private FrameLayout upperRight;
//		private FrameLayout lowerLeft;
//		private FrameLayout lowerRight;

//		protected override void OnCreate(Bundle bundle)
//        {
//			//Remove title bar
//			RequestWindowFeature(WindowFeatures.NoTitle);
//			//Remove notification bar
//			Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

//			// Set our view from the "main" layout resource
//			SetContentView(Resource.Layout.Factory);

//			// Get UI elements out of the layout
//			result = FindViewById<TextView>(Resource.Id.result);
//			var button1 = FindViewById<Button>(Resource.Id.button1);
//			button1.LongClick += Button1_LongClick;
//			//var button2 = FindViewById<Button>(Resource.Id.button2);
//			//button2.Click += Button2_Click;
//			//var dropZone = FindViewById<FrameLayout>(Resource.Id.dropzone);
//			//dropZone.Drag += HandleDrag;

//			upperLeft = FindViewById<FrameLayout>(Resource.Id.upperLeft);
//			upperRight = FindViewById<FrameLayout>(Resource.Id.upperRight);
//			lowerLeft = FindViewById<FrameLayout>(Resource.Id.lowerLeft);
//			lowerRight = FindViewById<FrameLayout>(Resource.Id.lowerRight);
//			upperLeft.Drag += HandleDragUpperLeft;
//			upperRight.Drag += HandleDragUpperRight;
//			lowerLeft.Drag += HandleDragLowerLeft;
//			lowerRight.Drag += HandleDragLowerRight;

//			base.OnCreate(bundle);
//        }

//        void HandleDragUpperLeft (object sender, Android.Views.View.DragEventArgs e)
//        {
//            var evt = e.Event;
//            switch (evt.Action)
//            {
//                case DragAction.Started:
//                case DragAction.Ended:
//                    e.Handled = true;
//                    break;
//                case DragAction.Entered:
//                    result.Text = "Drop it like it's hot!";
//                    break;
//                case DragAction.Exited:
//                    result.Text = "Drop something here!";
//                    break;
//                case DragAction.Drop:
//                    e.Handled = true;
//					View view = (View)evt.LocalState;
//					//ViewGroup owner = (ViewGroup)view.Parent;
//					//owner.RemoveView(view);
//					if (null != view)
//						result.Text = "View is not null";
//					else
//						result.Text = "View is null";

//					//var data = e.Event.ClipData.GetItemAt(0).Text;
//     //               if (null != data)
//     //                   result.Text = data + " at Upper Left.";
//                    break;
//            }
//        }

//		void HandleDragUpperRight (object sender, Android.Views.View.DragEventArgs e)
//		{
//			var evt = e.Event;
//			switch (evt.Action)
//			{
//				case DragAction.Started:
//				case DragAction.Ended:
//					e.Handled = true;
//					break;
//				case DragAction.Entered:
//					result.Text = "Drop it like it's hot!";
//					break;
//				case DragAction.Exited:
//					result.Text = "Drop something here!";
//					break;
//				case DragAction.Drop:
//					e.Handled = true;
//					View view = (View)evt.LocalState;
//					//ViewGroup owner = (ViewGroup)view.Parent;
//					//owner.RemoveView(view);
//					upperRight.AddView(view);
//					var data = e.Event.ClipData.GetItemAt(0).Text;
//					if (null != data)
//						result.Text = data + " at Upper Right.";
//					break;
//			}
//		}

//		void HandleDragLowerLeft (object sender, Android.Views.View.DragEventArgs e)
//		{
//			var evt = e.Event;
//			switch (evt.Action)
//			{
//				case DragAction.Started:
//				case DragAction.Ended:
//					e.Handled = true;
//					break;
//				case DragAction.Entered:
//					result.Text = "Drop it like it's hot!";
//					break;
//				case DragAction.Exited:
//					result.Text = "Drop something here!";
//					break;
//				case DragAction.Drop:
//					e.Handled = true;
//					View view = (View)evt.LocalState;
//					//ViewGroup owner = (ViewGroup)view.Parent;
//					//owner.RemoveView(view);
//					lowerLeft.AddView(view);
//					var data = e.Event.ClipData.GetItemAt(0).Text;
//					if (null != data)
//						result.Text = data + " at Lower Left.";
//					break;
//			}
//		}

//		void HandleDragLowerRight (object sender, Android.Views.View.DragEventArgs e)
//		{
//			var evt = e.Event;
//			switch (evt.Action)
//			{
//				case DragAction.Started:
//				case DragAction.Ended:
//					e.Handled = true;
//					break;
//				case DragAction.Entered:
//					result.Text = "Drop it like it's hot!";
//					break;
//				case DragAction.Exited:
//					result.Text = "Drop something here!";
//					break;
//				case DragAction.Drop:
//					e.Handled = true;
//					View view = (View)evt.LocalState;
//					//ViewGroup owner = (ViewGroup)view.Parent;
//					//owner.RemoveView(view);
//					lowerRight.AddView(view);
//					var data = e.Event.ClipData.GetItemAt(0).Text;
//					if (null != data)
//						result.Text = data + " at Lower Right";
//					break;
//			}
//		}
//		void Button1_LongClick(object sender, EventArgs e)
//        {
//            var data = ClipData.NewPlainText("category", "Button 1");
//            ((sender) as Button).StartDrag(data, new View.DragShadowBuilder(((sender) as Button)), null, 0);
//        }

//        //void Button2_Click(object sender, EventArgs e)
//        //{
//        //    var data = ClipData.NewPlainText("category", "Element 2");
//        //    ((sender) as Button).StartDrag(data, new View.DragShadowBuilder(((sender) as Button)), null, 0);
//        //}

//    }
//}

