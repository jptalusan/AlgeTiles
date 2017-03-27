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
	public class CustomEquationDialogTwoVar : DialogFragment
	{
		//Create class properties
		protected EditText x_value_1;
		protected EditText y_value_1;
		protected EditText one_value_1;

		protected EditText x_value_2;
		protected EditText y_value_2;
		protected EditText one_value_2;
		//protected Button ok;
		//protected Button cancel;
		public event DialogEventHandler Dismissed;

		public static CustomEquationDialogTwoVar NewInstance()
		{
			var dialogFragment = new CustomEquationDialogTwoVar();
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
			var dialogView = inflater.Inflate(Resource.Layout.CustomQuestionTwoVar, null);

			if (dialogView != null)
			{
				//Initialize the properties
				x_value_1 = dialogView.FindViewById<EditText>(Resource.Id.x_value_1);
				y_value_1 = dialogView.FindViewById<EditText>(Resource.Id.y_value_1);
				one_value_1 = dialogView.FindViewById<EditText>(Resource.Id.one_value_1);

				x_value_2 = dialogView.FindViewById<EditText>(Resource.Id.x_value_2);
				y_value_2 = dialogView.FindViewById<EditText>(Resource.Id.y_value_2);
				one_value_2 = dialogView.FindViewById<EditText>(Resource.Id.one_value_2);

				builder.SetView(dialogView);
				builder.SetPositiveButton("Ok", HandlePositiveButtonClick);
				builder.SetNegativeButton("Cancel", HandleNegativeButtonClick);
			}

			var dialog = builder.Create();
			return dialog;
		}

		private void HandlePositiveButtonClick(object sender, DialogClickEventArgs e)
		{
			var dialog = (AlertDialog)sender;
			int temp = 0;
			int?[] questions = new int?[6];
			string test = "TEST";

			if (int.TryParse(x_value_1.Text.ToString(), out temp) && !String.IsNullOrEmpty(x_value_1.Text.ToString().Trim()))
			{
				questions[0] = int.Parse(x_value_1.Text.ToString());
			}

			if (int.TryParse(y_value_1.Text.ToString(), out temp) && !String.IsNullOrEmpty(y_value_1.Text.ToString().Trim()))
			{
				questions[1] = int.Parse(y_value_1.Text.ToString());
			}

			if (int.TryParse(one_value_1.Text.ToString(), out temp) && !String.IsNullOrEmpty(one_value_1.Text.ToString().Trim()))
			{
				questions[2] = int.Parse(one_value_1.Text.ToString());
			}

			if (int.TryParse(x_value_2.Text.ToString(), out temp) && !String.IsNullOrEmpty(x_value_2.Text.ToString().Trim()))
			{
				questions[3] = int.Parse(x_value_2.Text.ToString());
			}

			if (int.TryParse(y_value_2.Text.ToString(), out temp) && !String.IsNullOrEmpty(y_value_2.Text.ToString().Trim()))
			{
				questions[4] = int.Parse(y_value_2.Text.ToString());
			}

			if (int.TryParse(one_value_2.Text.ToString(), out temp) && !String.IsNullOrEmpty(one_value_2.Text.ToString().Trim()))
			{
				questions[5] = int.Parse(one_value_2.Text.ToString());
			}

			if (questions[0].HasValue &&
				questions[1].HasValue &&
				questions[2].HasValue &&
				questions[3].HasValue &&
				questions[4].HasValue &&
				questions[5].HasValue)
			{
				if (null != Dismissed)
					Dismissed(this, new DialogEventArgs { vars = questions });

				dialog.Dismiss();
			} else
			{
				Toast.MakeText(Activity, "Invalid, please enter values again.", ToastLength.Short).Show();
			}
		}

		private void HandleNegativeButtonClick(object sender, DialogClickEventArgs e)
		{
			var dialog = (AlertDialog)sender;
			Console.WriteLine("Canceled");
			dialog.Dismiss();
		}

		public class DialogEventArgs : EventArgs
		{
			public int?[] vars { get; set; }
		}

		public delegate void DialogEventHandler(object sender, DialogEventArgs args);
	}
}