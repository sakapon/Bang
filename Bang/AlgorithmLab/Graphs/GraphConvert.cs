using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	public static class GraphConvert
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

		public static List<WeightedEdge>[] WeightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<WeightedEdge>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(new WeightedEdge(e[0], e[1], e[2]));
				if (!directed) map[e[1]].Add(new WeightedEdge(e[1], e[0], e[2]));
			}
			return map;
		}

		public static List<WeightedEdge>[] WeightedEdgesToMap(int vertexesCount, WeightedEdge[] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<WeightedEdge>());
			foreach (var e in edges)
			{
				map[e.From].Add(e);
				if (!directed) map[e.To].Add(e.Reverse());
			}
			return map;
		}

		public static List<WeightedEdge>[] WeightedEdgesToMap<T>(int vertexesCount, WeightedEdge<T>[] edges, bool directed, Func<T, int> toId)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<WeightedEdge>());
			foreach (var te in edges)
			{
				var e = te.Untype(toId);
				map[e.From].Add(e);
				if (!directed) map[e.To].Add(e.Reverse());
			}
			return map;
		}
	}
}
