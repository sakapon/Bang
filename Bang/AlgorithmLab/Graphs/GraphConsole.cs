using System;

namespace AlgorithmLab.Graphs
{
	public static class GraphConsole
	{
		public static WeightedEdge[] ReadUnweightedEdges(int count)
		{
			var edges = new WeightedEdge[count];
			for (int i = 0; i < count; ++i)
			{
				var e = Console.ReadLine().Split();
				edges[i] = new WeightedEdge(int.Parse(e[0]), int.Parse(e[1]), 0);
			}
			return edges;
		}

		public static WeightedEdge[] ReadWeightedEdges(int count)
		{
			var edges = new WeightedEdge[count];
			for (int i = 0; i < count; ++i)
			{
				var e = Console.ReadLine().Split();
				edges[i] = new WeightedEdge(int.Parse(e[0]), int.Parse(e[1]), long.Parse(e[2]));
			}
			return edges;
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
