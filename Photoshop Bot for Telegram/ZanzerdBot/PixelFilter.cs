
using System;

namespace ZanzerdBot
{
	public class PixelFilter<Tparam> : ParametrizedFilter<Tparam>
		where Tparam : IParameters, new()
	{
		private Func<Pixel, Tparam, Pixel> Filter { get; set; }
		private string FilterName { get; set; }
		public PixelFilter(string filterName, Func<Pixel, Tparam, Pixel> filter) //фильтр принимает пиксель и параметры, выдаёт преобразованный пиксель
        {
			FilterName = filterName;
			Filter = filter;
        }

		public override Image Process(Image original, Tparam parameters)
		{
			var result = new Image(original.Width, original.Height);

			for (int x = 0; x < result.Width; x++)
				for (int y = 0; y < result.Height; y++)
				{
					result[x, y] = Filter(original[x, y], parameters);
				}
			return result;
		}
	}
}
