using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Grid
{
	public class WeightedMap
	{
		public int Height { get; }
		public int Width { get; }
		GridMap<List<Edge>> map;
		public GridMap<List<Edge>> RawMap => map;
		public Edge[] this[Point vertex] => map[vertex].ToArray();

		public WeightedMap(int height, int width, GridMap<List<Edge>> map)
		{
			Height = height;
			Width = width;
			this.map = map;
		}

		public WeightedMap(int height, int width)
		{
			Height = height;
			Width = width;
			map = GridMap.Create(height, width, () => new List<Edge>());
		}

		public WeightedMap(int height, int width, Edge[] edges, bool directed) : this(height, width)
		{
			AddEdges(edges, directed);
		}

		public void AddEdges(Edge[] edges, bool directed)
		{
			GraphConvert.WeightedEdgesToMap(map, edges, directed);
		}

		public void AddEdge(Edge edge, bool directed)
		{
			map[edge.From].Add(edge);
			if (!directed) map[edge.To].Add(edge.Reverse());
		}

		public void AddEdge(Point from, Point to, long cost, bool directed)
		{
			map[from].Add(new Edge(from, to, cost));
			if (!directed) map[to].Add(new Edge(to, from, cost));
		}

		public WeightedResult Dijkstra(Point startVertex, Point endVertex)
		{
			return ShortestPathCore.Dijkstra(Height, Width, v => this[v], startVertex, endVertex);
		}

		public WeightedResult BfsMod(int m, Point startVertex, Point endVertex)
		{
			return ShortestPathCore.BfsMod(m, Height, Width, v => this[v], startVertex, endVertex);
		}
	}
}
