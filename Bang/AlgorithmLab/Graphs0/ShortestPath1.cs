using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs0
{
	/// <summary>
	/// 最短経路を求めるための静的メソッドを提供します。
	/// </summary>
	public static class ShortestPath1
	{
		// 無向グリッド上での典型的な BFS の例
		// ev: 終点を指定しない場合、(-1, -1) など
		// 
		//var r = ShortestPath.Bfs(h * w,
		//	v => GridHelper.ToId(v, w),
		//	id => GridHelper.FromId(id, w),
		//	v => Array.FindAll(GridHelper.Nexts(v), v => s.GetByP(v) != '#'),
		//	sv, ev);

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
			return Dijkstra(vertexesCount, Array.ConvertAll(edges, e => new Edge(e)), directed, startVertexId, endVertexId);
		}
	}
}
