using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Grid
{
	public class UnweightedMap
	{
		public int Height { get; }
		public int Width { get; }
		GridMap<List<Point>> map;
		public GridMap<List<Point>> RawMap => map;
		public Point[] this[Point vertex] => map[vertex].ToArray();

		public UnweightedMap(int height, int width, GridMap<List<Point>> map)
		{
			Height = height;
			Width = width;
			this.map = map;
		}

		public UnweightedMap(int height, int width)
		{
			Height = height;
			Width = width;
			map = GridMap.Create(height, width, () => new List<Point>());
		}

		public UnweightedMap(int height, int width, Edge[] edges, bool directed) : this(height, width)
		{
			AddEdges(edges, directed);
		}

		public void AddEdges(Edge[] edges, bool directed)
		{
			GraphConvert.UnweightedEdgesToMap(map, edges, directed);
		}

		public void AddEdge(Edge edge, bool directed)
		{
			map[edge.From].Add(edge.To);
			if (!directed) map[edge.To].Add(edge.From);
		}

		public void AddEdge(Point from, Point to, bool directed)
		{
			map[from].Add(to);
			if (!directed) map[to].Add(from);
		}

		public UnweightedResult Bfs(Point startVertex, Point endVertex)
		{
			return ShortestPathCore.Bfs(Height, Width, v => this[v], startVertex, endVertex);
		}
	}
}
