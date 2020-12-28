using System;

namespace AlgorithmLab.Graphs
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

		public static Edge<int>[] ReadEdges(int count)
		{
			return Array.ConvertAll(new bool[count], _ => EdgeHelper.ToEdge(Read()));
		}

		public static string[] ReadGrid(int height)
		{
			return Array.ConvertAll(new bool[height], _ => Console.ReadLine());
		}

		public static char[][] ReadGridAsChar(int height)
		{
			return Array.ConvertAll(new bool[height], _ => Console.ReadLine().ToCharArray());
		}

		public static int[][] ReadGridAsInt(int height)
		{
			return Array.ConvertAll(new bool[height], _ => Read());
		}

		public static string[] ReadEnclosedGrid(ref int height, ref int width, char c = '#', int delta = 1)
		{
			var h = height + 2 * delta;
			var w = width + 2 * delta;
			var cw = new string(c, w);
			var cd = new string(c, delta);

			var s = new string[h];
			for (int i = 0; i < delta; ++i) s[delta + height + i] = s[i] = cw;
			for (int i = 0; i < height; ++i) s[delta + i] = cd + Console.ReadLine() + cd;
			(height, width) = (h, w);
			return s;
		}
	}
}
