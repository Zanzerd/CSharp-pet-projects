using System;
using System.Collections.Generic;
using System.Text;

namespace ZanzerdBot
{
	public class Image
	{
		public Image(int width, int height)
		{
			this.Height = height;
			this.Width = width;
			data = new Pixel[width, height];
		}

		public ref Pixel this[int x, int y]
		{
			get { return ref data[x, y]; }

			//set { data[x, y] = value; }
		}

		public int Height { get; private set; }
		public int Width { get; private set; }
		public readonly Pixel[,] data;
	}
}
