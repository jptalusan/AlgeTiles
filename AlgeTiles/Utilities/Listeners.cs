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

		public void Layout_Touch(object sender, View.TouchEventArgs e)
		{
			var v = (ViewGroup)sender;
			float x = e.Event.GetX(0);
			float y = e.Event.GetY(0);
			bool isDroppedAtCenter = false;

			if (a.oneTile_Clicked)
			{
				a.currentButtonType = Constants.ONE_TILE;
			}
			else if (a.xTile_Clicked)
			{
				a.currentButtonType = Constants.X_TILE;
			}
			else if (a.x2Tile_Clicked)
			{
				a.currentButtonType = Constants.X2_TILE;
			}
			else if (a.xyTile_Clicked)
			{
				a.currentButtonType = Constants.XY_TILE;
			}
			else if (a.yTile_Clicked)
			{
				a.currentButtonType = Constants.Y_TILE;
			}
			else if (a.y2Tile_Clicked)
			{
				a.currentButtonType = Constants.Y2_TILE;
			}
			else
			{
				//Do nothing
				a.currentButtonType = "";
			}

			switch (e.Event.Action)
			{
				case MotionEventActions.Down:
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
					}
					else
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
						}
						else
						{
							algeTilesIV.Text = tF.text;
						}

						heightFactor = tF.heightFactor;
						widthFactor = tF.widthFactor;

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
						a.resetBGColors(v);
					}
					break;
				case MotionEventActions.Up:
					a.resetBGColors(v);
					break;
				default:
					break;
			}
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

					a.tile_1.Selected = false;
					a.oneTile_Clicked = false;

					a.x_tile.Selected = false;
					a.xTile_Clicked = false;

					a.x2_tile.Selected = false;
					a.x2Tile_Clicked = false;

					a.xy_tile.Selected = false;
					a.xyTile_Clicked = false;

					a.y_tile.Selected = false;
					a.yTile_Clicked = false;

					a.y2_tile.Selected = false;
					a.y2Tile_Clicked = false;

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

		//http://stackoverflow.com/questions/4747311/how-can-i-keep-one-button-as-pressed-after-click-on-it
		public void tile_Click(object sender, EventArgs e)
		{
			var imageViewTouch = (sender) as AlgeTilesTextView;
			if (imageViewTouch.Selected)
			{
				imageViewTouch.Selected = false;
				a.oneTile_Clicked = false;
				a.xTile_Clicked = false;
				a.x2Tile_Clicked = false;
				a.xyTile_Clicked = false;
				a.yTile_Clicked = false;
				a.y2Tile_Clicked = false;
			}
			else
			{
				imageViewTouch.Selected = true;
			}

			switch (imageViewTouch.Id)
			{
				case Resource.Id.tile_1:
					a.oneTile_Clicked = imageViewTouch.Selected;

					a.x_tile.Selected = false;
					a.xTile_Clicked = false;

					a.x2_tile.Selected = false;
					a.x2Tile_Clicked = false;

					a.xy_tile.Selected = false;
					a.xyTile_Clicked = false;

					a.y_tile.Selected = false;
					a.yTile_Clicked = false;

					a.y2_tile.Selected = false;
					a.y2Tile_Clicked = false;
					break;
				case Resource.Id.x_tile:
					a.xTile_Clicked = imageViewTouch.Selected;

					a.tile_1.Selected = false;
					a.oneTile_Clicked = false;
					
					a.x2_tile.Selected = false;
					a.x2Tile_Clicked = false;

					a.xy_tile.Selected = false;
					a.xyTile_Clicked = false;

					a.y_tile.Selected = false;
					a.yTile_Clicked = false;

					a.y2_tile.Selected = false;
					a.y2Tile_Clicked = false;
					break;
				case Resource.Id.x2_tile:
					a.x2Tile_Clicked = imageViewTouch.Selected;

					a.tile_1.Selected = false;
					a.oneTile_Clicked = false;
					
					a.x_tile.Selected = false;
					a.xTile_Clicked = false;

					a.xy_tile.Selected = false;
					a.xyTile_Clicked = false;

					a.y_tile.Selected = false;
					a.yTile_Clicked = false;

					a.y2_tile.Selected = false;
					a.y2Tile_Clicked = false;
					break;
				case Resource.Id.xy_tile:
					a.xyTile_Clicked = imageViewTouch.Selected;

					a.x_tile.Selected = false;
					a.xTile_Clicked = false;

					a.x2_tile.Selected = false;
					a.x2Tile_Clicked = false;

					a.tile_1.Selected = false;
					a.oneTile_Clicked = false;

					a.y_tile.Selected = false;
					a.yTile_Clicked = false;

					a.y2_tile.Selected = false;
					a.y2Tile_Clicked = false;
					break;
				case Resource.Id.y_tile:
					a.yTile_Clicked = imageViewTouch.Selected;

					a.tile_1.Selected = false;
					a.oneTile_Clicked = false;

					a.x2_tile.Selected = false;
					a.x2Tile_Clicked = false;

					a.xy_tile.Selected = false;
					a.xyTile_Clicked = false;

					a.x_tile.Selected = false;
					a.xTile_Clicked = false;

					a.y2_tile.Selected = false;
					a.y2Tile_Clicked = false;
					break;
				case Resource.Id.y2_tile:
					a.y2Tile_Clicked = imageViewTouch.Selected;

					a.tile_1.Selected = false;
					a.oneTile_Clicked = false;

					a.x_tile.Selected = false;
					a.xTile_Clicked = false;

					a.xy_tile.Selected = false;
					a.xyTile_Clicked = false;

					a.y_tile.Selected = false;
					a.yTile_Clicked = false;

					a.x2_tile.Selected = false;
					a.x2Tile_Clicked = false;
					break;
			}
			a.dragToggle.Checked = false;
			a.removeToggle.Checked = false;
		}
	}
}