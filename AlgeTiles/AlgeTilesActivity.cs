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
using Android.Media;
using System.Threading.Tasks;
using Android.Util;

namespace AlgeTiles
{
	public abstract class AlgeTilesActivity : Activity
	{
		private static string TAG = "AlgeTiles:AlgeTilesActivity";
		public string activityType = "";
		public Context context { get; }
		public TextView result;
		public Boolean hasButtonBeenDroppedInCorrectzone = false;
		public string currentButtonType = "";
		public ViewGroup currentOwner;

		public ToggleButton removeToggle;
		public ToggleButton dragToggle;
		public ToggleButton rotateToggle;
		public ToggleButton muteToggle;

		public ImageButton tile_1;
		public ImageButton x_tile;
		public ImageButton y_tile;
		public ImageButton x2_tile;
		public ImageButton y2_tile;
		public ImageButton xy_tile;

		public ImageButton tile_1_rot;
		public ImageButton x_tile_rot;
		public ImageButton x2_tile_rot;
		public ImageButton y_tile_rot;
		public ImageButton xy_tile_rot;

		public Button newQuestionButton;
		public Button refreshButton;
		public Button checkButton;

		public int numberOfVariables = 0;

		public Boolean isFirstAnswerCorrect = false;
		public Boolean isSecondAnswerCorrect = false;
		public Boolean isThirdAnswerCorrect = false;

		public AlgeTilesRelativeLayout upperLeftGrid;
		public AlgeTilesRelativeLayout upperRightGrid;
		public AlgeTilesRelativeLayout lowerLeftGrid;
		public AlgeTilesRelativeLayout lowerRightGrid;

		public GridLayout upperMiddleGrid;
		public GridLayout middleLeftGrid;
		public GridLayout middleRightGrid;
		public GridLayout lowerMiddleGrid;

		//Four outer grids
		public GridValue upperLeftGV;
		public GridValue upperRightGV;
		public GridValue lowerLeftGV;
		public GridValue lowerRightGV;

		//Four center grids
		public GridValue midUpGV;
		public GridValue midLowGV;
		public GridValue midLeftGV;
		public GridValue midRightGV;

		public List<int> vars = new List<int>();
		public List<int> expandedVars = new List<int>();
		public List<ViewGroup> innerGridLayoutList = new List<ViewGroup>();
		public List<ViewGroup> outerGridLayoutList = new List<ViewGroup>();
		public List<GridValue> gridValueList = new List<GridValue>();

		public MediaPlayer correct;
		public MediaPlayer incorrect;

		public EditText x2ET;
		public EditText y2ET;
		public EditText xyET;
		public EditText xET;
		public EditText yET;
		public EditText oneET;

		public List<EditText> editTextList = new List<EditText>();

		public ScrollView sv;

		public bool isFirstTime = false;

		public int heightInPx = 0;
		public int widthInPx = 0;

		public List<RectTile> upperRightRectTileList = new List<RectTile>();
		public List<RectTile> upperLeftRectTileList = new List<RectTile>();
		public List<RectTile> lowerRightRectTileList = new List<RectTile>();
		public List<RectTile> lowerLeftRectTileList = new List<RectTile>();

		public List<List<RectTile>> rectTileListList = new List<List<RectTile>>();

		public Listeners listeners;
		public ISharedPreferences prefs;
		public string UserName { get; set; }
		internal Context PackageContext { get; set; }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(getLayoutResourceId());
		}

		protected abstract int getLayoutResourceId();

		protected abstract void setupQuestionString(List<int> vars);

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

		public void checkAnswers()
		{
			if (!isFirstAnswerCorrect)
			{
				GridValue[] gvArr = { midUpGV, midLowGV, midLeftGV, midRightGV };
				if (AlgorithmUtilities.isFirstAnswerCorrect(vars, gvArr, numberOfVariables))
				{
					isFirstAnswerCorrect = true;

					//Change color of draggable areas to signify "Done/Correct"
					upperLeftGrid.Drag += listeners.GridLayout_Drag;
					upperRightGrid.Drag += listeners.GridLayout_Drag;

					lowerLeftGrid.Drag += listeners.GridLayout_Drag;
					lowerRightGrid.Drag += listeners.GridLayout_Drag;

					//Shade red the other grids
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
						outerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.shape);

					for (int i = 0; i < innerGridLayoutList.Count; ++i)
					{
						innerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.ok);
						innerGridLayoutList[i].Drag -= listeners.GridLayout_Drag;
						for (int j = 0; j < innerGridLayoutList[i].ChildCount; ++j)
						{
							var iv = innerGridLayoutList[i].GetChildAt(j) as AlgeTilesTextView;
							iv.LongClick -= listeners.clonedImageView_Touch;
						}
					}

					expandedVars = AlgorithmUtilities.expandingVars(vars);
					foreach (var i in expandedVars)
						Log.Debug(TAG, "Expanded in activity: " + i);
					Toast.MakeText(Application.Context, "1:correct", ToastLength.Short).Show();

					if (Constants.ONE_VAR == numberOfVariables)
					{
						x2ET.Enabled = true;
						xET.Enabled = true;
						oneET.Enabled = true;
					}
					else
					{
						x2ET.Enabled = true;
						y2ET.Enabled = true;
						xyET.Enabled = true;
						xET.Enabled = true;
						yET.Enabled = true;
						oneET.Enabled = true;
					}

					if (!muteToggle.Checked)
						correct.Start();

					TileUtilities.generateInnerLayoutTileArrays(heightInPx, widthInPx, innerGridLayoutList, rectTileListList);
					upperRightGrid.drawRects(upperRightRectTileList);
					upperLeftGrid.drawRects(upperLeftRectTileList);
					lowerRightGrid.drawRects(lowerRightRectTileList);
					lowerLeftGrid.drawRects(lowerLeftRectTileList);
				}
				else
				{
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
						outerGridLayoutList[i].Drag -= listeners.GridLayout_Drag;

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
					}
					else
					{
						if (lowerLeftGV.xVal != 0)
						{
							posX = lowerLeftGV.xVal;
							posXVG = lowerLeftGrid;
						}
					}

					int posY = 0;
					ViewGroup posYVG = null;
					if (upperRightGV.yVal != 0)
					{
						posY = upperRightGV.yVal;
						posYVG = upperRightGrid;
					}
					else
					{
						if (lowerLeftGV.yVal != 0)
						{
							posY = lowerLeftGV.yVal;
							posYVG = lowerLeftGrid;
						}
					}

					int posXY = 0;
					ViewGroup posXYVG = null;
					if (upperRightGV.xyVal != 0)
					{
						posXY = upperRightGV.xyVal;
						posXYVG = upperRightGrid;
					}
					else
					{
						if (lowerLeftGV.xyVal != 0)
						{
							posXY = lowerLeftGV.xyVal;
							posXYVG = lowerLeftGrid;
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

					int negY = 0;
					ViewGroup negYVG = null;
					if (upperLeftGV.yVal != 0)
					{
						negY = upperLeftGV.yVal;
						negYVG = upperLeftGrid;
					}
					else
					{
						if (lowerRightGV.yVal != 0)
						{
							negY = lowerRightGV.yVal;
							negYVG = lowerRightGrid;
						}
					}

					int negXY = 0;
					ViewGroup negXYVG = null;
					if (upperLeftGV.xyVal != 0)
					{
						negXY = upperLeftGV.xyVal;
						negXYVG = upperLeftGrid;
					}
					else
					{
						if (lowerRightGV.xyVal != 0)
						{
							negXY = lowerRightGV.xyVal;
							negXYVG = lowerRightGrid;
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

					if (posY != 0 && negY != 0)
					{
						Log.Debug(TAG, "Cancelling out: " + posY + ", " + negY);
						int yToRemove = posY > negY ? negY : posY;
						Log.Debug(TAG, "To remove: " + yToRemove);
						List<AlgeTilesTextView> tobeRemoved = new List<AlgeTilesTextView>();
						for (int j = 0; j < posYVG.ChildCount; ++j)
						{
							AlgeTilesTextView alIV = posYVG.GetChildAt(j) as AlgeTilesTextView;
							if (alIV.getTileType().Equals(Constants.Y_TILE) ||
								alIV.getTileType().Equals(Constants.Y_TILE_ROT))
							{
								tobeRemoved.Add(alIV);
							}
						}

						List<AlgeTilesTextView> negTobeRemoved = new List<AlgeTilesTextView>();
						for (int j = 0; j < negYVG.ChildCount; ++j)
						{
							AlgeTilesTextView negalIV = negYVG.GetChildAt(j) as AlgeTilesTextView;
							if (negalIV.getTileType().Equals(Constants.Y_TILE) ||
								negalIV.getTileType().Equals(Constants.Y_TILE_ROT))
							{
								negTobeRemoved.Add(negalIV);
							}
						}

						for (int j = 0; j < yToRemove; ++j)
						{
							posYVG.RemoveView(tobeRemoved[j]);
							negYVG.RemoveView(negTobeRemoved[j]);
						}
					}

					if (posXY != 0 && negXY != 0)
					{
						Log.Debug(TAG, "Cancelling out: " + posXY + ", " + negXY);
						int xyToRemove = posXY > negXY ? negXY : posXY;
						Log.Debug(TAG, "To remove: " + xyToRemove);
						List<AlgeTilesTextView> tobeRemoved = new List<AlgeTilesTextView>();
						for (int j = 0; j < posXYVG.ChildCount; ++j)
						{
							AlgeTilesTextView alIV = posXYVG.GetChildAt(j) as AlgeTilesTextView;
							if (alIV.getTileType().Equals(Constants.XY_TILE) ||
								alIV.getTileType().Equals(Constants.XY_TILE_ROT))
							{
								tobeRemoved.Add(alIV);
							}
						}

						List<AlgeTilesTextView> negTobeRemoved = new List<AlgeTilesTextView>();
						for (int j = 0; j < negXYVG.ChildCount; ++j)
						{
							AlgeTilesTextView negalIV = negXYVG.GetChildAt(j) as AlgeTilesTextView;
							if (negalIV.getTileType().Equals(Constants.XY_TILE) ||
								negalIV.getTileType().Equals(Constants.XY_TILE_ROT))
							{
								negTobeRemoved.Add(negalIV);
							}
						}

						for (int j = 0; j < xyToRemove; ++j)
						{
							posXYVG.RemoveView(tobeRemoved[j]);
							negXYVG.RemoveView(negTobeRemoved[j]);
						}
					}
					//End Cancelling out
					Toast.MakeText(Application.Context, "2:correct", ToastLength.Short).Show();
					if (!muteToggle.Checked)
						correct.Start();

					//Loop through inner and prevent deletions by removing: listeners.clonedImageView_Touch
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
					{
						outerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.ok);
						outerGridLayoutList[i].Drag -= listeners.GridLayout_Drag;
						for (int j = 0; j < outerGridLayoutList[i].ChildCount; ++j)
						{
							var iv = outerGridLayoutList[i].GetChildAt(j) as AlgeTilesTextView;
							iv.LongClick -= listeners.clonedImageView_Touch;
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
				if (Constants.ONE_VAR == numberOfVariables)
				{
					int[] answer = new int[3];
					int temp = 0;
					answer[0] = int.TryParse(x2ET.Text, out temp) ? temp : 0;
					answer[1] = int.TryParse(xET.Text, out temp) ? temp : 0;
					answer[2] = int.TryParse(oneET.Text, out temp) ? temp : 0;

					if ((Math.Abs(answer[0]) +
						Math.Abs(answer[1]) +
						Math.Abs(answer[2])) == 0)
					{
						isThirdAnswerCorrect = false;
					}
					else
					{
						if (expandedVars[0] == answer[0] &&
							expandedVars[1] == answer[1] &&
							expandedVars[2] == answer[2])
							isThirdAnswerCorrect = true;
					}
				}
				else
				{
					int[] answer = new int[6];
					int temp = 0;
					answer[0] = int.TryParse(x2ET.Text, out temp) ? temp : 0;
					answer[1] = int.TryParse(y2ET.Text, out temp) ? temp : 0;
					answer[2] = int.TryParse(xyET.Text, out temp) ? temp : 0;
					answer[3] = int.TryParse(xET.Text, out temp) ? temp : 0;
					answer[4] = int.TryParse(yET.Text, out temp) ? temp : 0;
					answer[5] = int.TryParse(oneET.Text, out temp) ? temp : 0;

					foreach (int i in answer)
						Log.Debug(TAG, "answer:" + i);

					foreach (int i in expandedVars)
						Log.Debug(TAG, "expandedVars:" + i);

					if ((Math.Abs(answer[0]) +
						Math.Abs(answer[1]) +
						Math.Abs(answer[2]) +
						Math.Abs(answer[3]) +
						Math.Abs(answer[4]) +
						Math.Abs(answer[5])) == 0)
					{
						isThirdAnswerCorrect = false;
					}
					else
					{
						if (expandedVars[0] == answer[0] &&
							expandedVars[1] == answer[1] &&
							expandedVars[2] == answer[2] &&
							expandedVars[3] == answer[3] &&
							expandedVars[4] == answer[4] &&
							expandedVars[5] == answer[5])
							isThirdAnswerCorrect = true;
					}
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

		protected void refreshScreen(string ActivityType, List<GridValue> gvList, List<ViewGroup> inGLList, List<ViewGroup> outGLList)
		{
			foreach (List<RectTile> rList in rectTileListList)
				rList.Clear();

			for (int i = 0; i < editTextList.Count; ++i)
			{
				editTextList[i].SetBackgroundResource(Resource.Drawable.shape);
				editTextList[i].Enabled = false;
				editTextList[i].Text = "";
			}

			isFirstAnswerCorrect = false;
			isSecondAnswerCorrect = false;
			isThirdAnswerCorrect = false;

			for (int i = 0; i < inGLList.Count; ++i)
			{
				inGLList[i].SetBackgroundResource(Resource.Drawable.shape);
				inGLList[i].Drag -= listeners.GridLayout_Drag;
			}

			for (int i = 0; i < outGLList.Count; ++i)
			{
				outGLList[i].SetBackgroundResource(Resource.Drawable.shape);
				outGLList[i].Drag -= listeners.GridLayout_Drag;
				AlgeTilesRelativeLayout a = (AlgeTilesRelativeLayout)outGLList[i];
				a.clearRects(heightInPx, widthInPx);
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
					inGLList[i].Drag -= listeners.GridLayout_Drag;
				}

				for (int i = 0; i < outGLList.Count; ++i)
				{
					outGLList[i].SetBackgroundResource(Resource.Drawable.shape);
					outGLList[i].Drag += listeners.GridLayout_Drag;
				}
			}
			else
			{
				for (int i = 0; i < inGLList.Count; ++i)
				{
					inGLList[i].SetBackgroundResource(Resource.Drawable.shape);
					inGLList[i].Drag += listeners.GridLayout_Drag;
				}

				for (int i = 0; i < outGLList.Count; ++i)
				{
					outGLList[i].SetBackgroundResource(Resource.Drawable.unavailable);
					outGLList[i].Drag -= listeners.GridLayout_Drag;
				}
			}
		}

	}
}
