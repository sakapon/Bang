using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	public static class GraphConvert2
	{
		public static List<int>[] UnweightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
			foreach (var e in edges)
			{
				map[e[0]].Add(e[1]);
				if (!directed) map[e[1]].Add(e[0]);
			}
			return map;
		}

		public static IntListMap<WeightedEdge<int>> WeightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			return WeightedEdgesToMap(vertexesCount, Array.ConvertAll(edges, EdgeHelper.Weighted), directed);
		}

		public static IntListMap<WeightedEdge<int>> WeightedEdgesToMap(int vertexesCount, WeightedEdge<int>[] edges, bool directed)
		{
			var map = new IntListMap<WeightedEdge<int>>(vertexesCount);
			foreach (var e in edges)
			{
				map.Add(e.From, e);
				if (!directed) map.Add(e.To, e.Reverse());
			}
			return map;
		}

		public static GridListMap<WeightedEdge<(int i, int j)>> WeightedEdgesToMap(int h, int w, WeightedEdge<(int i, int j)>[] edges, bool directed)
		{
			var map = new GridListMap<WeightedEdge<(int i, int j)>>(h, w);
			foreach (var e in edges)
			{
				map.Add(e.From, e);
				if (!directed) map.Add(e.To, e.Reverse());
			}
			return map;
		}

		public static MappingListMap<T, WeightedEdge<T>> WeightedEdgesToMap<T>(int vertexesCount, WeightedEdge<T>[] edges, bool directed, Func<T, int> toId)
		{
			var map = new MappingListMap<T, WeightedEdge<T>>(vertexesCount, toId);
			foreach (var e in edges)
			{
				map.Add(e.From, e);
				if (!directed) map.Add(e.To, e.Reverse());
			}
			return map;
		}
	}
}
