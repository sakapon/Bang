using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Grid
{
	public static class GraphConvert
	{
		public static void UnweightedEdgesToMap(List<int>[] map, Edge[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map[e.From].Add(e.To);
				if (!directed) map[e.To].Add(e.From);
			}
		}

		public static void UnweightedEdgesToMap(List<int>[] map, int[][] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map[e[0]].Add(e[1]);
				if (!directed) map[e[1]].Add(e[0]);
			}
		}

		public static void WeightedEdgesToMap(List<Edge>[] map, Edge[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map[e.From].Add(e);
				if (!directed) map[e.To].Add(e.Reverse());
			}
		}

		public static void WeightedEdgesToMap(List<Edge>[] map, int[][] edges, bool directed)
		{
			foreach (var e0 in edges)
			{
				Edge e = e0;
				map[e.From].Add(e);
				if (!directed) map[e.To].Add(e.Reverse());
			}
		}

		public static List<int>[] UnweightedEdgesToMap(int vertexesCount, Edge[] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
			UnweightedEdgesToMap(map, edges, directed);
			return map;
		}

		public static List<int>[] UnweightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
			UnweightedEdgesToMap(map, edges, directed);
			return map;
		}

		public static List<Edge>[] WeightedEdgesToMap(int vertexesCount, Edge[] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<Edge>());
			WeightedEdgesToMap(map, edges, directed);
			return map;
		}

		public static List<Edge>[] WeightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<Edge>());
			WeightedEdgesToMap(map, edges, directed);
			return map;
		}
	}
}
