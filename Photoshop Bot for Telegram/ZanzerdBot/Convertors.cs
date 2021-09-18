﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ZanzerdBot
{
	public static class Convertors
	{
		public static Image Bitmap2Photo(Bitmap bmp)
		{
			var image = new Image(bmp.Width, bmp.Height);
			for (int x = 0; x < bmp.Width; x++)
				for (int y = 0; y < bmp.Height; y++)
				{
					var pixel = bmp.GetPixel(x, y);
					//photo[x, y] = new Pixel { };
					image[x, y].R = (double)pixel.R / 255;
					image[x, y].G = (double)pixel.G / 255;
					image[x, y].B = (double)pixel.B / 255;
				}
			return image;
		}

		static int ToChannel(double val)
		{
			if (val < 0 || val > 1)
				throw new Exception(string.Format("Wrong channel value {0} (the value must be between 0 and 1", val));
			return (int)(val * 255);
		}

		public static Bitmap Photo2Bitmap(Image image)
		{
			var bmp = new Bitmap(image.Width, image.Height);
			for (int x = 0; x < bmp.Width; x++)
				for (int y = 0; y < bmp.Height; y++)
					bmp.SetPixel(x, y, Color.FromArgb(
						ToChannel(image[x, y].R),
						ToChannel(image[x, y].G),
						ToChannel(image[x, y].B)));

			return bmp;
		}
	}
}
