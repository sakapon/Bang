using System;
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

		public static UnweightedResult<T> Bfs<T>(int vertexesCount, Func<T, int> toId, Func<int, T> fromId, T[][] edges, bool directed, T startVertex, T endVertex)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
			foreach (var e in edges)
			{
				var id0 = toId(e[0]);
				var id1 = toId(e[1]);
				map[id0].Add(id1);
				if (!directed) map[id1].Add(id0);
			}
			var r = ShortestPathCore.Bfs(vertexesCount, id => map[id], toId(startVertex), toId(endVertex));
			return new UnweightedResult<T>(r, toId, fromId);
		}

		public static UnweightedResult Bfs(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = UnweightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore.Bfs(vertexesCount, v => map[v], startVertexId, endVertexId);
		}

		public static WeightedResult Dijkstra(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = WeightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore.Dijkstra(vertexesCount, v => map[v], startVertexId, endVertexId);
		}

		#region Adjacent List

		public static List<int>[] UnweightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(e[1]);
				if (!directed) map[e[1]].Add(e[0]);
			}
			return map;
		}

		public static List<int[]>[] WeightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int[]>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(new[] { e[0], e[1], e[2] });
				if (!directed) map[e[1]].Add(new[] { e[1], e[0], e[2] });
			}
			return map;
		}
		#endregion
	}

	public class UnweightedResult<T> : UnweightedResult
	{
		Func<T, int> ToId;
		Func<int, T> FromId;
		public long this[T vertex] => RawCosts[ToId(vertex)];

		public UnweightedResult(UnweightedResult result, Func<T, int> toId, Func<int, T> fromId) : base(result.RawCosts, result.RawInVertexes)
		{
			ToId = toId;
			FromId = fromId;
		}

		public T[] GetPathVertexes(T endVertex)
		{
			return Array.ConvertAll(GetPathVertexes(ToId(endVertex)), id => FromId(id));
		}
	}
}
