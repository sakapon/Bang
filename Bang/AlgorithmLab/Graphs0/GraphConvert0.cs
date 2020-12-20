using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs0
{
	public static class GraphConvert0
	{
		public static List<int>[] UnweightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(e[1]);
				if (!directed) map[e[1]].Add(e[0]);
			}
			return map;
		}

		public static List<int[]>[] WeightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int[]>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(new[] { e[0], e[1], e[2] });
				if (!directed) map[e[1]].Add(new[] { e[1], e[0], e[2] });
			}
			return map;
		}
	}
}
