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

namespace AlgeTiles
{
	class Number
	{
		//Add type (x, x2, 1)
		//Add group?
		int mPositiveValue = 0;
		int mNegativeValue = 0;

		// Declare an Age property of type int:
		public int posVal
		{
			get
			{
				return mPositiveValue;
			}
			set
			{
				mPositiveValue = value;
			}
		}

		// Declare an Age property of type int:
		public int negVal
		{
			get
			{
				return mNegativeValue;
			}
			set
			{
				mNegativeValue = value;
			}
		}

		public Number(int posVal, int negVal)
		{
			this.mPositiveValue = posVal;
			this.mNegativeValue = negVal;
		}
	}
}