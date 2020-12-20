using System;

namespace AlgorithmLab.Graphs
{
	public static class GraphConvert2
	{
		static void AddUnweightedEdges<T>(ListMap<T, T> map, UnweightedEdge<T>[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map.Add(e.From, e.To);
				if (!directed) map.Add(e.To, e.From);
			}
		}

		static void AddWeightedEdges<T>(ListMap<T, WeightedEdge<T>> map, WeightedEdge<T>[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map.Add(e.From, e);
				if (!directed) map.Add(e.To, e.Reverse());
			}
		}

		public static IntListMap<int> UnweightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			return UnweightedEdgesToMap(vertexesCount, Array.ConvertAll(edges, EdgeHelper.Unweighted), directed);
		}

		public static IntListMap<int> UnweightedEdgesToMap(int vertexesCount, UnweightedEdge<int>[] edges, bool directed)
		{
			var map = new IntListMap<int>(vertexesCount);
			AddUnweightedEdges(map, edges, directed);
			return map;
		}

		public static GridListMap<(int i, int j)> UnweightedEdgesToMap(int h, int w, UnweightedEdge<(int i, int j)>[] edges, bool directed)
		{
			var map = new GridListMap<(int i, int j)>(h, w);
			AddUnweightedEdges(map, edges, directed);
			return map;
		}

		public static MappingListMap<T, T> UnweightedEdgesToMap<T>(int vertexesCount, UnweightedEdge<T>[] edges, bool directed, Func<T, int> toId)
		{
			var map = new MappingListMap<T, T>(vertexesCount, toId);
			AddUnweightedEdges(map, edges, directed);
			return map;
		}

		public static IntListMap<WeightedEdge<int>> WeightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			return WeightedEdgesToMap(vertexesCount, Array.ConvertAll(edges, EdgeHelper.Weighted), directed);
		}

		public static IntListMap<WeightedEdge<int>> WeightedEdgesToMap(int vertexesCount, WeightedEdge<int>[] edges, bool directed)
		{
			var map = new IntListMap<WeightedEdge<int>>(vertexesCount);
			AddWeightedEdges(map, edges, directed);
			return map;
		}

		public static GridListMap<WeightedEdge<(int i, int j)>> WeightedEdgesToMap(int h, int w, WeightedEdge<(int i, int j)>[] edges, bool directed)
		{
			var map = new GridListMap<WeightedEdge<(int i, int j)>>(h, w);
			AddWeightedEdges(map, edges, directed);
			return map;
		}

		public static MappingListMap<T, WeightedEdge<T>> WeightedEdgesToMap<T>(int vertexesCount, WeightedEdge<T>[] edges, bool directed, Func<T, int> toId)
		{
			var map = new MappingListMap<T, WeightedEdge<T>>(vertexesCount, toId);
			AddWeightedEdges(map, edges, directed);
			return map;
		}
	}
}
