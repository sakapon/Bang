using System;

namespace AlgorithmLab.Graphs0
{
	/// <summary>
	/// 最短経路を求めるための静的メソッドを提供します。
	/// </summary>
	public static class ShortestPath1
	{
		public static UnweightedResult Bfs(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = GraphConvert1.UnweightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore1.Bfs(vertexesCount, v => map[v].ToArray(), startVertexId, endVertexId);
		}

		public static WeightedResult Dijkstra(int vertexesCount, Edge[] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = GraphConvert1.WeightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore1.Dijkstra(vertexesCount, v => map[v].ToArray(), startVertexId, endVertexId);
		}

		public static WeightedResult Dijkstra(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = GraphConvert1.WeightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore1.Dijkstra(vertexesCount, v => map[v].ToArray(), startVertexId, endVertexId);
		}
	}
}
