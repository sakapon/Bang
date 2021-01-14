using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Int
{
	public class WeightedMap
	{
		public int VertexesCount { get; }
		List<Edge>[] map;
		public List<Edge>[] RawMap => map;
		public Edge[] this[int vertex] => map[vertex].ToArray();

		public WeightedMap(int vertexesCount, List<Edge>[] map)
		{
			VertexesCount = vertexesCount;
			this.map = map;
		}

		public WeightedMap(int vertexesCount)
		{
			VertexesCount = vertexesCount;
			map = Array.ConvertAll(new bool[vertexesCount], _ => new List<Edge>());
		}

		public WeightedMap(int vertexesCount, Edge[] edges, bool directed) : this(vertexesCount)
		{
			AddEdges(edges, directed);
		}

		public WeightedMap(int vertexesCount, int[][] edges, bool directed) : this(vertexesCount)
		{
			AddEdges(edges, directed);
		}

		public void AddEdges(Edge[] edges, bool directed)
		{
			GraphConvert.WeightedEdgesToMap(map, edges, directed);
		}

		public void AddEdges(int[][] edges, bool directed)
		{
			GraphConvert.WeightedEdgesToMap(map, edges, directed);
		}

		public void AddEdge(Edge edge, bool directed)
		{
			map[edge.From].Add(edge);
			if (!directed) map[edge.To].Add(edge.Reverse());
		}

		public void AddEdge(int from, int to, long cost, bool directed)
		{
			map[from].Add(new Edge(from, to, cost));
			if (!directed) map[to].Add(new Edge(to, from, cost));
		}

		public WeightedResult Dijkstra(int startVertex, int endVertex = -1)
		{
			return ShortestPathCore.Dijkstra(VertexesCount, v => this[v], startVertex, endVertex);
		}
	}
}
