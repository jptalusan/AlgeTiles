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

		public Button tutorialButton;
		
		public ToggleButton removeToggle;
		public ToggleButton dragToggle;
		public ToggleButton rotateToggle;
		public ToggleButton muteToggle;

		public AlgeTilesTextView tile_1;
		public AlgeTilesTextView x_tile;
		public AlgeTilesTextView y_tile;
		public AlgeTilesTextView x2_tile;
		public AlgeTilesTextView y2_tile;
		public AlgeTilesTextView xy_tile;

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
		public Dialog settingsDialog;

		public Space space1;
		public Space space2;
		public Space space3;
		public Space space4;
		public Space space5;

		public TextView x2TV;
		public TextView y2TV;

		public Boolean oneTile_Clicked;
		public Boolean xTile_Clicked;
		public Boolean x2Tile_Clicked;
		public Boolean xyTile_Clicked;
		public Boolean yTile_Clicked;
		public Boolean y2Tile_Clicked;
		
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(getLayoutResourceId());
		}

		protected abstract int getLayoutResourceId();

		public void resetBGColors(List<ViewGroup> vL)
 		{
 			foreach (ViewGroup v in vL)
 			{
 				if (v is AlgeTilesRelativeLayout)
 				{
 					AlgeTilesRelativeLayout temp = v as AlgeTilesRelativeLayout;
 					temp.resetColor();
 				}
 				else
 				{
 					v.SetBackgroundResource(Resource.Drawable.shape);
 				}
 			}
 		}

		public void resetBGColors(ViewGroup v)
 		{
 			if (v is AlgeTilesRelativeLayout)
 			{
 				AlgeTilesRelativeLayout temp = v as AlgeTilesRelativeLayout;
 				temp.resetColor();
 			}
 			else
 			{
 				v.SetBackgroundResource(Resource.Drawable.shape);
 			}
 		}

		protected abstract void setupQuestionString(List<int> vars);

		public async void incorrectPrompt(List<EditText> gvList)
		{
			if (!muteToggle.Checked)
				incorrect.Start();
			for (int i = 0; i < gvList.Count; ++i)
				gvList[i].SetBackgroundResource(Resource.Drawable.notok);
			await Task.Delay(Constants.DELAY);
			for (int i = 0; i < gvList.Count; ++i)
			{
				gvList[i].SetBackgroundResource(Resource.Drawable.shape);
			}
		}

		public async void incorrectPrompt(List<ViewGroup> gvList)
		{
			if (!muteToggle.Checked)
				incorrect.Start();
			for (int i = 0; i < gvList.Count; ++i)
				gvList[i].SetBackgroundResource(Resource.Drawable.notok);
			await Task.Delay(Constants.DELAY);
			for (int i = 0; i < gvList.Count; ++i)
			{
				resetBGColors(gvList);
			}
		}

		public async void checkAnswers(AlgeTilesActivity multiplyActivity)
		{
			if (!isFirstAnswerCorrect)
			{
				GridValue[] gvArr = { midUpGV, midLowGV, midLeftGV, midRightGV };
				if (AlgorithmUtilities.isFirstAnswerCorrect(vars, gvArr, numberOfVariables))
				{
					isFirstAnswerCorrect = true;

					//Change color of draggable areas to signify "Done/Correct"
					upperLeftGrid.Touch += listeners.Layout_Touch;
					upperRightGrid.Touch += listeners.Layout_Touch;

					lowerLeftGrid.Touch += listeners.Layout_Touch;
					lowerRightGrid.Touch += listeners.Layout_Touch;

					//Shade red the other grids
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
					{
						resetBGColors(outerGridLayoutList);
					}

					for (int i = 0; i < innerGridLayoutList.Count; ++i)
					{
						innerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.ok);
						innerGridLayoutList[i].Touch -= listeners.Layout_Touch;
						for (int j = 0; j < innerGridLayoutList[i].ChildCount; ++j)
						{
							var iv = innerGridLayoutList[i].GetChildAt(j) as AlgeTilesTextView;
							iv.LongClick -= listeners.clonedImageView_Touch;
						}
					}

					expandedVars = AlgorithmUtilities.expandingVars(vars);
					foreach (var i in expandedVars)
						Log.Debug(TAG, "Expanded in activity: " + i);

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

					new AlertDialog.Builder(this)
						.SetPositiveButton(Constants.PROCEED_TO_MULTIP, (sender, args) =>
						{
							// User pressed yes
						})
						.SetMessage(Constants.CORRECT_PLACEMENT)
						.SetTitle(Constants.CORRECT)
						.Show();
				}
				else
				{
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
					{
						outerGridLayoutList[i].Touch -= listeners.Layout_Touch;
					}

					incorrectPrompt(innerGridLayoutList);

					Toast.MakeText(Application.Context, Constants.WRONG + Constants.MULTIPLICATION, ToastLength.Short).Show();
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

					int xToRemove = posX > negX ? negX : posX;
					List<AlgeTilesTextView> tobeRemovedX = new List<AlgeTilesTextView>();
					List<AlgeTilesTextView> negTobeRemovedX = new List<AlgeTilesTextView>();
					if (posX != 0 && negX != 0)
					{
						Log.Debug(TAG, "Cancelling out: " + posX + ", " + negX);
						Log.Debug(TAG, "To remove: " + xToRemove);
						for (int j = 0; j < posXVG.ChildCount; ++j)
						{
							AlgeTilesTextView alIV = posXVG.GetChildAt(j) as AlgeTilesTextView;
							if (alIV.getTileType().Equals(Constants.X_TILE) ||
								alIV.getTileType().Equals(Constants.X_TILE_ROT))
							{
								tobeRemovedX.Add(alIV);
							}
						}

						for (int j = 0; j < negXVG.ChildCount; ++j)
						{
							AlgeTilesTextView negalIV = negXVG.GetChildAt(j) as AlgeTilesTextView;
							if (negalIV.getTileType().Equals(Constants.X_TILE) ||
								negalIV.getTileType().Equals(Constants.X_TILE_ROT))
							{
								negTobeRemovedX.Add(negalIV);
							}
						}
					}

					int yToRemove = posY > negY ? negY : posY;
					List<AlgeTilesTextView> tobeRemovedY = new List<AlgeTilesTextView>();
					List<AlgeTilesTextView> negTobeRemovedY = new List<AlgeTilesTextView>();
					if (posY != 0 && negY != 0)
					{
						Log.Debug(TAG, "Cancelling out: " + posY + ", " + negY);
						Log.Debug(TAG, "To remove: " + yToRemove);
						for (int j = 0; j < posYVG.ChildCount; ++j)
						{
							AlgeTilesTextView alIV = posYVG.GetChildAt(j) as AlgeTilesTextView;
							if (alIV.getTileType().Equals(Constants.Y_TILE) ||
								alIV.getTileType().Equals(Constants.Y_TILE_ROT))
							{
								tobeRemovedY.Add(alIV);
							}
						}

						for (int j = 0; j < negYVG.ChildCount; ++j)
						{
							AlgeTilesTextView negalIV = negYVG.GetChildAt(j) as AlgeTilesTextView;
							if (negalIV.getTileType().Equals(Constants.Y_TILE) ||
								negalIV.getTileType().Equals(Constants.Y_TILE_ROT))
							{
								negTobeRemovedY.Add(negalIV);
							}
						}
					}

					int xyToRemove = posXY > negXY ? negXY : posXY;
					List<AlgeTilesTextView> tobeRemovedXY = new List<AlgeTilesTextView>();
					List<AlgeTilesTextView> negTobeRemovedXY = new List<AlgeTilesTextView>();
					if (posXY != 0 && negXY != 0)
					{
						Log.Debug(TAG, "Cancelling out: " + posXY + ", " + negXY);
						Log.Debug(TAG, "To remove: " + xyToRemove);

						for (int j = 0; j < posXYVG.ChildCount; ++j)
						{
							AlgeTilesTextView alIV = posXYVG.GetChildAt(j) as AlgeTilesTextView;
							if (alIV.getTileType().Equals(Constants.XY_TILE) ||
								alIV.getTileType().Equals(Constants.XY_TILE_ROT))
							{
								tobeRemovedXY.Add(alIV);
							}
						}


						for (int j = 0; j < negXYVG.ChildCount; ++j)
						{
							AlgeTilesTextView negalIV = negXYVG.GetChildAt(j) as AlgeTilesTextView;
							if (negalIV.getTileType().Equals(Constants.XY_TILE) ||
								negalIV.getTileType().Equals(Constants.XY_TILE_ROT))
							{
								negTobeRemovedXY.Add(negalIV);
							}
						}
					}

					//Added changing color of tiles to be cancelled and 2 second delay before cancelling out
					for (int j = 0; j < xToRemove; ++j)
					{
						tobeRemovedX[j].SetBackgroundResource(Resource.Drawable.cancelling);
						negTobeRemovedX[j].SetBackgroundResource(Resource.Drawable.cancelling);
					}

					for (int j = 0; j < yToRemove; ++j)
					{
						tobeRemovedY[j].SetBackgroundResource(Resource.Drawable.cancelling);
						negTobeRemovedY[j].SetBackgroundResource(Resource.Drawable.cancelling);
					}


					for (int j = 0; j < xyToRemove; ++j)
					{
						tobeRemovedXY[j].SetBackgroundResource(Resource.Drawable.cancelling);
						negTobeRemovedXY[j].SetBackgroundResource(Resource.Drawable.cancelling);
					}

					if (xToRemove + yToRemove + xyToRemove > 0)
						await Task.Delay(Constants.CANCELOUT_DELAY);

					for (int j = 0; j < xToRemove; ++j)
					{
						posXVG.RemoveView(tobeRemovedX[j]);
						negXVG.RemoveView(negTobeRemovedX[j]);
					}

					for (int j = 0; j < yToRemove; ++j)
					{
						posYVG.RemoveView(tobeRemovedY[j]);
						negYVG.RemoveView(negTobeRemovedY[j]);
					}


					for (int j = 0; j < xyToRemove; ++j)
					{
						posXYVG.RemoveView(tobeRemovedXY[j]);
						negXYVG.RemoveView(negTobeRemovedXY[j]);
					}
					//End Cancelling out

					if (!muteToggle.Checked)
						correct.Start();

					//Loop through inner and prevent deletions by removing: listeners.clonedImageView_Touch
					for (int i = 0; i < outerGridLayoutList.Count; ++i)
					{
						outerGridLayoutList[i].SetBackgroundResource(Resource.Drawable.ok);
						outerGridLayoutList[i].Touch -= listeners.Layout_Touch;
						for (int j = 0; j < outerGridLayoutList[i].ChildCount; ++j)
						{
							var iv = outerGridLayoutList[i].GetChildAt(j) as AlgeTilesTextView;
							iv.LongClick -= listeners.clonedImageView_Touch;
						}
					}

					//Removing outer grid after 2nd correct (10-28-2016)
					upperRightGrid.clearRects(heightInPx, widthInPx);
					upperLeftGrid.clearRects(heightInPx, widthInPx);
					lowerRightGrid.clearRects(heightInPx, widthInPx);
					lowerLeftGrid.clearRects(heightInPx, widthInPx);

					upperRightRectTileList.Clear();
					upperLeftRectTileList.Clear();
					lowerRightRectTileList.Clear();
					lowerLeftRectTileList.Clear();

					isSecondAnswerCorrect = true;
					new AlertDialog.Builder(this)
						.SetPositiveButton(Constants.PROCEED + Constants.COEFFICIENTS, (sender, args) =>
						{
							// User pressed yes
						})
						.SetMessage(Constants.CORRECT_MULTIP)
						.SetTitle(Constants.CORRECT)
						.Show();

					for (int i = 0; i < innerGridLayoutList.Count; ++i)
					{
						for (int j = 0; j < innerGridLayoutList[i].ChildCount; ++j)
						{
							View v = innerGridLayoutList[i].GetChildAt(j);
							innerGridLayoutList[i].RemoveAllViews();
						}
					}
				}
				else
				{

					Toast.MakeText(Application.Context, Constants.WRONG + " " + Constants.FACTOR, ToastLength.Short).Show();
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
					for (int i = 0; i < editTextList.Count; ++i)
					{
						editTextList[i].SetBackgroundResource(Resource.Drawable.ok);
						editTextList[i].Enabled = false;
					}
					if (!muteToggle.Checked)
						correct.Start();


					new AlertDialog.Builder(this)
						//.SetPositiveButton("New Question", (sender, args) =>
						//{
						//	setupNewQuestion(numberOfVariables);
						//	refreshScreen(Constants.FACTOR, gridValueList, innerGridLayoutList, outerGridLayoutList);
						//})
						.SetNegativeButton("OK", (sender, args) =>
						{

						})
						.SetMessage(Constants.CORRECT + Constants.COEFFICIENTS)
						.SetTitle(Constants.CORRECT)
						.Show();
				}
				else
				{
					Toast.MakeText(Application.Context, Constants.WRONG + " " + Constants.COEFFICIENTS, ToastLength.Short).Show();
					incorrectPrompt(editTextList);
				}
			}
		}

		protected void setupNewQuestion(int numberOfVariables)
		{
			isFirstAnswerCorrect = false;
			vars = AlgorithmUtilities.RNG(Constants.MULTIPLY, numberOfVariables);

			//(ax + b)(cx + d)
			//if (Constants.ONE_VAR == numberOfVariables)
			//{
				for (int i = 0; i < gridValueList.Count; ++i)
				{
					gridValueList[i].init();
				}

				setupQuestionString(vars);
			//}

			foreach (int i in vars)
			{
				Log.Debug(TAG, i + "");
			}
		}

		protected void refreshScreen(string ActivityType, List<GridValue> gvList, List<ViewGroup> inGLList, List<ViewGroup> outGLList)
		{
			foreach (List<RectTile> rList in rectTileListList)
			{
				rList.Clear();
			}

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
				resetBGColors(inGLList);
				inGLList[i].Touch -= listeners.Layout_Touch;
			}

			for (int i = 0; i < outGLList.Count; ++i)
			{
				resetBGColors(outGLList);
				outGLList[i].Touch -= listeners.Layout_Touch;
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
					inGLList[i].Touch -= listeners.Layout_Touch;
				}

				for (int i = 0; i < outGLList.Count; ++i)
				{
					resetBGColors(outGLList);
					outGLList[i].Touch += listeners.Layout_Touch;
				}
			}
			else
			{
				for (int i = 0; i < inGLList.Count; ++i)
				{
					resetBGColors(inGLList);
					inGLList[i].Touch += listeners.Layout_Touch;
				}

				for (int i = 0; i < outGLList.Count; ++i)
				{
					outGLList[i].SetBackgroundResource(Resource.Drawable.unavailable);
					outGLList[i].Touch -= listeners.Layout_Touch;
				}
			}
		}
	}
}
