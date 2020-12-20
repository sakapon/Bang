﻿using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	/// <summary>
	/// 最短経路を求めるための静的メソッドを提供します。
	/// </summary>
	public static class ShortestPath
	{
		// 無向グリッド上での典型的な BFS の例
		// ev: 終点を指定しない場合、(-1, -1) など
		// 
		//var r = ShortestPath.Bfs(h * w,
		//	v => GridHelper.ToId(v, w),
		//	id => GridHelper.FromId(id, w),
		//	v => Array.FindAll(GridHelper.Nexts(v), v => s.GetByP(v) != '#'),
		//	sv, ev);

		public static UnweightedResult<T> Bfs<T>(int vertexesCount, Func<T, int> toId, Func<int, T> fromId, Func<T, T[]> getNextVertexes, T startVertex, T endVertex)
		{
			var r = ShortestPathCore.Bfs(vertexesCount, id => Array.ConvertAll(getNextVertexes(fromId(id)), v => toId(v)), toId(startVertex), toId(endVertex));
			return new UnweightedResult<T>(r, toId, fromId);
		}

		public static UnweightedResult Bfs(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = GraphConvert.UnweightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore.Bfs(vertexesCount, v => map[v].ToArray(), startVertexId, endVertexId);
		}

		public static WeightedResult Dijkstra(int vertexesCount, WeightedEdge[] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = GraphConvert.WeightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore.Dijkstra(vertexesCount, v => map[v].ToArray(), startVertexId, endVertexId);
		}

		public static WeightedResult Dijkstra(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			return Dijkstra(vertexesCount, Array.ConvertAll(edges, e => new WeightedEdge(e)), directed, startVertexId, endVertexId);
		}

		public static WeightedResult<T> Dijkstra<T>(int vertexesCount, WeightedEdge<T>[] edges, bool directed, T startVertex, T endVertex, Func<T, int> toId, Func<int, T> fromId)
		{
			var r = Dijkstra(vertexesCount, Array.ConvertAll(edges, e => e.Untype(toId)), directed, toId(startVertex), toId(endVertex));
			return new WeightedResult<T>(r, toId, fromId);
		}

		public static WeightedResult<T> Dijkstra<T>(int vertexesCount, Func<T, WeightedEdge<T>[]> getNextEdges, T startVertex, T endVertex, Func<T, int> toId, Func<int, T> fromId)
		{
			var r = ShortestPathCore.Dijkstra(vertexesCount, id => Array.ConvertAll(getNextEdges(fromId(id)), e => e.Untype(toId)), toId(startVertex), toId(endVertex));
			return new WeightedResult<T>(r, toId, fromId);
		}
	}
}
