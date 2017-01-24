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
using Android.Preferences;
using Android.Text;
using Android.Text.Style;
using AlgeTiles.Activities;

namespace AlgeTiles
{
	public class Listeners
	{
		private static string TAG = "AlgeTiles:Listeners";
		AlgeTilesActivity a;
		public Listeners(AlgeTilesActivity a)
		{
			this.a = a;
		}
		//TODO: When top most layer textview increases in length, the edit text gets pushed
		public void clonedImageView_Touch(object sender, View.LongClickEventArgs e)
		{
			var touchedImageView = (sender) as AlgeTilesTextView;
			ViewGroup vg = (ViewGroup)touchedImageView.Parent;
			if (a.removeToggle.Checked)
			{
				Log.Debug(TAG, "Switch: Remove");
				TileUtilities.checkIfUserDropsOnRect(vg.Id,
					touchedImageView.getTileType(),
					touchedImageView.Left + 10,
					touchedImageView.Top + 10,
					Constants.SUBTRACT,
					a.rectTileListList);
				vg.RemoveView(touchedImageView);
				touchedImageView.Visibility = ViewStates.Gone;
				Vibrator vibrator = (Vibrator)a.GetSystemService(Context.VibratorService);
				vibrator.Vibrate(30);

				int id = touchedImageView.Id;

				TileUtilities.checkWhichParentAndUpdate(vg.Id, touchedImageView.getTileType(), Constants.SUBTRACT, a.gridValueList);
			}

			if (a.dragToggle.Checked)
			{
				Log.Debug(TAG, "Switch: Drag");
			}

			//TODO: Not working
			if (a.rotateToggle.Checked)
			{
				Log.Debug(TAG, "Switch: Rotate");
				touchedImageView.Rotation = touchedImageView.Rotation - 90;
			}
		}

		public void GridLayout_Drag(object sender, Android.Views.View.DragEventArgs e)
		{
			var v = (ViewGroup)sender;
			View view = (View)e.Event.LocalState;
			//can just get currentButtonType from view.getTileType()
			var drag_data = e.Event.ClipData;
			bool isDroppedAtCenter = false;
			float x = 0.0f;
			float y = 0.0f;

			switch (e.Event.Action)
			{
				case DragAction.Started:
					a.hasButtonBeenDroppedInCorrectzone = false;
					AlgeTilesTextView aTv = (AlgeTilesTextView)view;
					a.currentButtonType = aTv.getTileType();
					//if (null != drag_data)
					//{
					//	a.currentButtonType = drag_data.GetItemAt(0).Text;
					//}
					break;
				case DragAction.Entered:
					v.SetBackgroundResource(Resource.Drawable.shape_droptarget);
					break;
				case DragAction.Exited:
					a.currentOwner = (ViewGroup)view.Parent;
					a.hasButtonBeenDroppedInCorrectzone = false;
					v.SetBackgroundResource(Resource.Drawable.shape);
					break;
				case DragAction.Location:
					x = e.Event.GetX(); //width
					y = e.Event.GetY(); //height
					break;
				case DragAction.Drop:
					if (null != drag_data)
					{
						a.currentButtonType = drag_data.GetItemAt(0).Text;
					}
					Log.Debug(TAG, "Dropped: " + a.currentButtonType);

					AlgeTilesTextView algeTilesIV = new AlgeTilesTextView(a);
					Boolean wasImageDropped = false;

					if (a.activityType.Equals(Constants.MULTIPLY))
					{
						if (!a.isFirstAnswerCorrect &&
							(a.currentButtonType.Equals(Constants.X_TILE) ||
							a.currentButtonType.Equals(Constants.Y_TILE) ||
							a.currentButtonType.Equals(Constants.ONE_TILE)))
						{
							if (v.Id == Resource.Id.middleLeft)
								algeTilesIV.RotationY = 180;
							if (v.Id == Resource.Id.upperMiddle)
								algeTilesIV.RotationX = 180;

							wasImageDropped = true;
							isDroppedAtCenter = true;
						}
						else if (a.isFirstAnswerCorrect)
						{
							wasImageDropped = true;
						}
					} else
					{
						if (a.isFirstAnswerCorrect &&
							(a.currentButtonType.Equals(Constants.X_TILE) ||
							a.currentButtonType.Equals(Constants.Y_TILE) ||
							a.currentButtonType.Equals(Constants.ONE_TILE)))
						{
							if (v.Id == Resource.Id.middleLeft)
								algeTilesIV.RotationY = 180;
							if (v.Id == Resource.Id.upperMiddle)
								algeTilesIV.RotationX = 180;
							wasImageDropped = true;
							isDroppedAtCenter = true;
						}
						else if (!a.isFirstAnswerCorrect)
						{
							wasImageDropped = true;
						}
					}

					algeTilesIV.setTileType(a.currentButtonType);

					if (wasImageDropped)
					{
						ViewGroup container = (ViewGroup)v;
						Log.Debug(TAG, a.currentButtonType);
						double heightFactor = 0;
						double widthFactor = 0;
						TileUtilities.TileFactor tF = TileUtilities.getTileFactors(a.currentButtonType);
						algeTilesIV.SetBackgroundResource(tF.id);

						if (tF.text.Length > 1 && !tF.text.Equals("xy"))
						{
							var cs = new SpannableStringBuilder(tF.text);
							cs.SetSpan(new SuperscriptSpan(), 1, 2, SpanTypes.ExclusiveExclusive);
							cs.SetSpan(new RelativeSizeSpan(0.75f), 1, 2, SpanTypes.ExclusiveExclusive);
							algeTilesIV.TextFormatted = cs;
						} else
						{
							algeTilesIV.Text = tF.text;
						}

						heightFactor = tF.heightFactor;
						widthFactor = tF.widthFactor;
						x = e.Event.GetX();
						y = e.Event.GetY();

						if (!isDroppedAtCenter)
						{
							Rect r = TileUtilities.checkIfUserDropsOnRect(v.Id, a.currentButtonType, x, y, Constants.ADD, a.rectTileListList);
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
								algeTilesIV.LongClick += a.listeners.clonedImageView_Touch;
								container.AddView(algeTilesIV);
								TileUtilities.checkWhichParentAndUpdate(v.Id, a.currentButtonType, Constants.ADD, a.gridValueList);
								a.hasButtonBeenDroppedInCorrectzone = true;
								//algeTilesIV.setDimensions(par.Height, par.Width);

							}
						}
						else
						{
							GridLayout.LayoutParams gParms = new GridLayout.LayoutParams();
							if (v.Id == Resource.Id.middleLeft || v.Id == Resource.Id.middleRight)
							{
								gParms.SetGravity(GravityFlags.Center);
								gParms.Height = (int)(a.heightInPx / widthFactor);
								gParms.Width = (int)(a.heightInPx / heightFactor);
							}
							else
							{
								gParms.SetGravity(GravityFlags.Center);
								gParms.Height = (int)(a.heightInPx / heightFactor);
								gParms.Width = (int)(a.heightInPx / widthFactor);
							}
							//algeTilesIV.setDimensions(gParms.Height, gParms.Width);

							algeTilesIV.LayoutParameters = gParms;
							algeTilesIV.LongClick += a.listeners.clonedImageView_Touch;
							container.AddView(algeTilesIV);
							TileUtilities.checkWhichParentAndUpdate(v.Id, a.currentButtonType, Constants.ADD, a.gridValueList);

							//Auto re-arrange of center tiles
							List<AlgeTilesTextView> centerTileList = new List<AlgeTilesTextView>();
							for (int i = 0; i < container.ChildCount; ++i)
							{
								AlgeTilesTextView a = (AlgeTilesTextView)container.GetChildAt(i);
								centerTileList.Add(a);
							}
							container.RemoveAllViews();

							List<AlgeTilesTextView> sortedList = centerTileList.OrderByDescending(o => o.getTileType()).ToList();
							for (int i = 0; i < sortedList.Count; ++i)
							{
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
					if (!a.hasButtonBeenDroppedInCorrectzone &&
						a.currentButtonType.Equals(Constants.CLONED_BUTTON))
					{
						a.currentOwner.RemoveView(view);
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

		public void tile_LongClick(object sender, View.LongClickEventArgs e)
		{
			var imageViewTouch = (sender) as AlgeTilesTextView;
			ClipData data = ClipData.NewPlainText(Constants.BUTTON_TYPE, Constants.ORIGINAL_BUTTON);
			switch (imageViewTouch.Id)
			{
				case Resource.Id.tile_1:
					data = ClipData.NewPlainText(Constants.BUTTON_TYPE, Constants.ONE_TILE);
					break;
				case Resource.Id.x_tile:
					data = ClipData.NewPlainText(Constants.BUTTON_TYPE, Constants.X_TILE);
					break;
				case Resource.Id.y_tile:
					data = ClipData.NewPlainText(Constants.BUTTON_TYPE, Constants.Y_TILE);
					break;
				case Resource.Id.xy_tile:
					data = ClipData.NewPlainText(Constants.BUTTON_TYPE, Constants.XY_TILE);
					break;
				case Resource.Id.y2_tile:
					data = ClipData.NewPlainText(Constants.BUTTON_TYPE, Constants.Y2_TILE);
					break;
				case Resource.Id.x2_tile:
					data = ClipData.NewPlainText(Constants.BUTTON_TYPE, Constants.X2_TILE);
					break;
			}
			a.dragToggle.Checked = false;
			a.removeToggle.Checked = false;

			View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(imageViewTouch);
			imageViewTouch.StartDrag(data, shadowBuilder, imageViewTouch, 0);
		}

		public void toggle_click(object sender, EventArgs e)
		{
			View clicked_toggle = (sender) as View;
			int buttonText = clicked_toggle.Id;
			switch (buttonText)
			{
				case Resource.Id.remove:
					a.dragToggle.Checked = a.dragToggle.Checked ? false : false;
					if (a.rotateToggle.Checked)
					{
						a.FindViewById<LinearLayout>(Resource.Id.notRotatedButtonLayout).Visibility = ViewStates.Visible;
					}
					a.rotateToggle.Checked = a.rotateToggle.Checked ? false : false;
					break;
				case Resource.Id.drag:
					a.removeToggle.Checked = a.removeToggle.Checked ? false : false;
					if (a.rotateToggle.Checked)
					{
						a.FindViewById<LinearLayout>(Resource.Id.notRotatedButtonLayout).Visibility = ViewStates.Visible;
					}
					a.rotateToggle.Checked = a.rotateToggle.Checked ? false : false;
					break;
				case Resource.Id.rotate:
					//Also rotate original tiles
					if (a.rotateToggle.Checked)
					{
						a.FindViewById<LinearLayout>(Resource.Id.notRotatedButtonLayout).Visibility = ViewStates.Gone;
					}
					else
					{
						a.FindViewById<LinearLayout>(Resource.Id.notRotatedButtonLayout).Visibility = ViewStates.Visible;
					}
					a.removeToggle.Checked = a.removeToggle.Checked ? false : false;
					a.dragToggle.Checked = a.dragToggle.Checked ? false : false;
					break;
				case Resource.Id.mute:
					ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(a);
					ISharedPreferencesEditor editor = prefs.Edit();
					editor.PutBoolean(Constants.MUTE, a.muteToggle.Checked);
					// editor.Commit();    // applies changes synchronously on older APIs
					editor.Apply();        // applies changes asynchronously on newer APIs
					break;
				case Resource.Id.tutorial:
					//https://developer.android.com/training/animation/screen-slide.html
					//https://www.bignerdranch.com/blog/viewpager-without-fragments/
					//https://developer.xamarin.com/samples/monodroid/ActionBarViewPager/
					//https://components.xamarin.com/gettingstarted/xamandroidsupportdesign
					//http://stackoverflow.com/questions/7693633/android-image-dialog-popup
					var intent = new Intent(a, typeof(TutorialActivity));
					a.StartActivity(intent);
					break;
			}
		}
	}
}