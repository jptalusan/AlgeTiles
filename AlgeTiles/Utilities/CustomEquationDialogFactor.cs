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

//http://valokafor.com/custom-dialog-xamarin-android/

namespace AlgeTiles
{
	public class CustomEquationDialogFactor : DialogFragment
	{
		//Create class properties
		protected Spinner equationsSpinner;
		public event DialogEventHandler Dismissed;
		protected string SelectedCategory = "";

		public static CustomEquationDialogFactor NewInstance()
		{
			var dialogFragment = new CustomEquationDialogFactor();
			return dialogFragment;
		}

		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			//Begin building a new dialog.
			var builder = new AlertDialog.Builder(Activity);

			//Get the layout inflater
			var inflater = Activity.LayoutInflater;

			//Inflate the layout for this dialog
			var dialogView = inflater.Inflate(Resource.Layout.CustomQuestionFactor, null);

			if (dialogView != null)
			{
				equationsSpinner = dialogView.FindViewById<Spinner>(Resource.Id.equationsSpinner);
				//Initialize the properties
				builder.SetView(dialogView);
				builder.SetPositiveButton("Ok", HandlePositiveButtonClick);
				builder.SetNegativeButton("Cancel", HandleNegativeButtonClick);

				LoadSpinnerData();
				equationsSpinner.ItemSelected += spinner_ItemSelected;
			}

			var dialog = builder.Create();
			return dialog;
		}

		private void LoadSpinnerData()
		{
			//Should parse here:
			var equationsAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleSpinnerItem, formatEquations(Constants.EQUATIONS));
			equationsAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			equationsSpinner.Adapter = equationsAdapter;
		}

		private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			var spinner = (Spinner)sender;
			SelectedCategory = string.Format("{0}", spinner.GetItemAtPosition(e.Position));
		}

		private void HandlePositiveButtonClick(object sender, DialogClickEventArgs e)
		{
			var dialog = (AlertDialog)sender;
			if (null != Dismissed)
				Dismissed(this, new DialogEventArgs { index = equationsSpinner.SelectedItemPosition });

			dialog.Dismiss();
		}

		private void HandleNegativeButtonClick(object sender, DialogClickEventArgs e)
		{
			var dialog = (AlertDialog)sender;
			Console.WriteLine("Canceled");
			dialog.Dismiss();
		}

		public class DialogEventArgs : EventArgs
		{
			public int index { get; set; }
		}

		public delegate void DialogEventHandler(object sender, DialogEventArgs args);

		private List<string> formatEquations(List<string> input)
		{
			List<string> output = new List<string>();
			int cnt = 1;
			foreach (string s in input)
			{
				string[] vals = s.Split(',');
				List<int> temp = new List<int>();
				foreach (string val in vals)
				{
					temp.Add(int.Parse(val));
				}
				List<int> expanded = AlgorithmUtilities.expandingVars(temp);
				string equation = "[" + cnt + "] " + setupQuestionString(expanded);//expanded[0] + "x\xB2 + " + expanded[1] + "x + " + expanded[2];
				output.Add(equation);
				++cnt;
			}
			return output;
		}

		private string setupQuestionString(List<int> vars)
		{
			string output = "";
			//vars = (ax^2 + bx + c)
			int ax2 = vars[0];
			int bx = vars[1];
			int c = vars[2];

			if (ax2 != 0)
				output += ax2 + "x\xB2";

			if (bx != 0)
				output += "+" + bx + "x";
			if (c != 0)
				output += "+" + c;

			output = output.Replace(" ", "");
			output = output.Replace("+-", "-");
			output = output.Replace("+", " + ");
			output = output.Replace("-", " - ");
			return output;
		}
	}
}