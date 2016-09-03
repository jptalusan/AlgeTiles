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
using System.Threading.Tasks;
using Android.Graphics;
using Android.Content.Res;

namespace AlgeTiles
{
	public class LocalImageService
	{
		public static async Task<Bitmap> LoadDrawableAsync(Resources resources, int drawableId, int width = 100, int heigth = 100)
		{
			var options = await GetDrawableOptions(resources, drawableId);
			options.InSampleSize = CalculateInSampleSize(options, width, heigth);
			options.InJustDecodeBounds = false;

			return await BitmapFactory.DecodeResourceAsync(resources, drawableId, options);
		}

		public static async Task<Bitmap> LoadImageAsync(string filePath, int width, int height)
		{

			var options = await GetImageOptions(filePath);
			options.InSampleSize = CalculateInSampleSize(options, width, height);

			options.InJustDecodeBounds = false;

			return await BitmapFactory.DecodeFileAsync(filePath, options);
		}

		private static async Task<BitmapFactory.Options> GetImageOptions(string path)
		{
			var options = new BitmapFactory.Options { InJustDecodeBounds = true, InPurgeable = true, InInputShareable = true };
			await BitmapFactory.DecodeFileAsync(path, options);

			return options;
		}

		private static async Task<BitmapFactory.Options> GetDrawableOptions(Resources resources, int drawableId)
		{
			var options = new BitmapFactory.Options
			{
				InJustDecodeBounds = true
			};

			await BitmapFactory.DecodeResourceAsync(resources, drawableId, options);

			return options;
		}

		private static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			var height = options.OutHeight;
			var width = options.OutWidth;
			var inSampleSize = 1D;

			if (height > reqHeight || width > reqWidth)
			{
				int halfHeight = (int)(height / 2);
				int halfWidth = (int)(width / 2);


				while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
				{
					inSampleSize *= 2;
				}
			}

			return (int)inSampleSize;
		}
	}
}