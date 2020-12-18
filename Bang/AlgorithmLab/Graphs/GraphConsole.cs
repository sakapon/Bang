using System;

namespace AlgorithmLab.Graphs
{
	public static class GraphConsole
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

		// Unweighted も含まれます。
		public static WeightedEdge[] ReadEdges(int count)
		{
			return Array.ConvertAll(new bool[count], _ => new WeightedEdge(Read()));
		}

		public static string[] ReadGrid(int h)
		{
			return Array.ConvertAll(new bool[h], _ => Console.ReadLine());
		}

		public static string[] ReadEnclosedGrid(ref int h, ref int w, char c = '#')
		{
			var s = new string[h + 2];
			s[h + 1] = s[0] = new string(c, w += 2);
			for (int i = 1; i <= h; ++i) s[i] = c + Console.ReadLine() + c;
			h += 2;
			return s;
		}
	}
}
