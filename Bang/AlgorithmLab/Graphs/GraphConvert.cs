using System;

namespace AlgorithmLab.Graphs
{
	public static class GraphConvert
	{
		public static void UnweightedEdgesToMap<TVertex>(ListMap<TVertex, TVertex> map, Edge<TVertex>[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map.Add(e.From, e.To);
				if (!directed) map.Add(e.To, e.From);
			}
		}

		public static void WeightedEdgesToMap<TVertex>(ListMap<TVertex, Edge<TVertex>> map, Edge<TVertex>[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map.Add(e.From, e);
				if (!directed) map.Add(e.To, e.Reverse());
			}
		}
	}
}
