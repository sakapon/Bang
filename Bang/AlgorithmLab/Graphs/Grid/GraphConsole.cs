using System;

namespace AlgorithmLab.Graphs.Grid
{
	public static class GraphConsole
	{
		const char Road = '.';
		const char Wall = '#';

		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

		public static Point ReadPoint()
		{
			return Point.Parse(Console.ReadLine());
		}

		public static string[] ReadGridAsString(int height)
		{
			return Array.ConvertAll(new bool[height], _ => Console.ReadLine());
		}

		public static char[][] ReadGridAsCharArray(int height)
		{
			return Array.ConvertAll(new bool[height], _ => Console.ReadLine().ToCharArray());
		}

		public static int[][] ReadGridAsIntArray(int height)
		{
			return Array.ConvertAll(new bool[height], _ => Read());
		}

		public static GridMap<char> ReadGrid(int height)
		{
			return new JaggedGridMap<char>(ReadGridAsCharArray(height));
		}

		public static GridMap<int> ReadIntGrid(int height)
		{
			return new JaggedGridMap<int>(ReadGridAsIntArray(height));
		}

		public static GridMap<char> ReadEnclosedGrid(ref int height, ref int width, char c = '#', int delta = 1)
		{
			var h = height + 2 * delta;
			var w = width + 2 * delta;

			var s = Array.ConvertAll(new bool[h], _ => Array.ConvertAll(new bool[w], __ => c));
			for (int i = 0; i < height; ++i)
			{
				var si = s[delta + i];
				var l = Console.ReadLine();
				for (int j = 0; j < width; ++j)
					si[delta + j] = l[j];
			}

			(height, width) = (h, w);
			return new JaggedGridMap<char>(s);
		}
	}
}
