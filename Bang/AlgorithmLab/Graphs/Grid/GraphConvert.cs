using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Grid
{
	public static class GraphConvert
	{
		public static void UnweightedEdgesToMap(GridMap<List<Point>> map, Edge[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map[e.From].Add(e.To);
				if (!directed) map[e.To].Add(e.From);
			}
		}

		public static void WeightedEdgesToMap(GridMap<List<Edge>> map, Edge[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map[e.From].Add(e);
				if (!directed) map[e.To].Add(e.Reverse());
			}
		}
	}
}
