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

		public static UnweightedEdge<int>[] ReadUnweightedEdges(int count)
		{
			return Array.ConvertAll(new bool[count], _ => EdgeHelper.Unweighted(Read()));
		}

		public static WeightedEdge<int>[] ReadWeightedEdges(int count)
		{
			return Array.ConvertAll(new bool[count], _ => EdgeHelper.Weighted(Read()));
		}

		public static string[] ReadGrid(int h)
		{
			return Array.ConvertAll(new bool[h], _ => Console.ReadLine());
		}

		public static char[][] ReadGridAsChar(int h)
		{
			return Array.ConvertAll(new bool[h], _ => Console.ReadLine().ToCharArray());
		}

		public static int[][] ReadGridAsInt(int h)
		{
			return Array.ConvertAll(new bool[h], _ => Read());
		}

		public static string[] ReadEnclosedGrid(ref int height, ref int width, char c = '#', int delta = 1)
		{
			var cl = new string(c, width += 2 * delta);
			var cd = new string(c, delta);

			var s = new string[height + 2 * delta];
			for (int i = 0; i < delta; ++i) s[delta + height + i] = s[i] = cl;
			for (int i = 0; i < height; ++i) s[delta + i] = cd + Console.ReadLine() + cd;
			height = s.Length;
			return s;
		}
	}
}
