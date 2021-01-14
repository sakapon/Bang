using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Int
{
	public class UnweightedMap
	{
		public int VertexesCount { get; }
		List<int>[] map;
		public List<int>[] RawMap => map;
		public int[] this[int vertex] => map[vertex].ToArray();

		public UnweightedMap(int vertexesCount, List<int>[] map)
		{
			VertexesCount = vertexesCount;
			this.map = map;
		}

		public UnweightedMap(int vertexesCount)
		{
			VertexesCount = vertexesCount;
			map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
		}

		public UnweightedMap(int vertexesCount, Edge[] edges, bool directed) : this(vertexesCount)
		{
			AddEdges(edges, directed);
		}

		public UnweightedMap(int vertexesCount, int[][] edges, bool directed) : this(vertexesCount)
		{
			AddEdges(edges, directed);
		}

		public void AddEdges(Edge[] edges, bool directed)
		{
			GraphConvert.UnweightedEdgesToMap(map, edges, directed);
		}

		public void AddEdges(int[][] edges, bool directed)
		{
			GraphConvert.UnweightedEdgesToMap(map, edges, directed);
		}

		public void AddEdge(Edge edge, bool directed)
		{
			map[edge.From].Add(edge.To);
			if (!directed) map[edge.To].Add(edge.From);
		}

		public void AddEdge(int from, int to, bool directed)
		{
			map[from].Add(to);
			if (!directed) map[to].Add(from);
		}

		public UnweightedResult Bfs(int startVertex, int endVertex = -1)
		{
			return ShortestPathCore.Bfs(VertexesCount, v => this[v], startVertex, endVertex);
		}
	}
}
