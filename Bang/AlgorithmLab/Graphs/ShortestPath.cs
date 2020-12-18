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
			var r = ShortestPathCore.Bfs(vertexesCount, id => map[id].ToArray(), toId(startVertex), toId(endVertex));
			return new UnweightedResult<T>(r, toId, fromId);
		}

		public static UnweightedResult Bfs(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = UnweightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore.Bfs(vertexesCount, v => map[v].ToArray(), startVertexId, endVertexId);
		}

		public static WeightedResult Dijkstra(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = WeightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore.Dijkstra(vertexesCount, v => map[v].ToArray(), startVertexId, endVertexId);
		}

		public static WeightedResult<T> Dijkstra<T>(int vertexesCount, Func<T, int> toId, Func<int, T> fromId, Func<T, WeightedEdge<T>[]> getNextEdges, T startVertex, T endVertex)
		{
			var r = ShortestPathCore.Dijkstra(vertexesCount, id => Array.ConvertAll(getNextEdges(fromId(id)), e => e.Untype(toId)), toId(startVertex), toId(endVertex));
			return new WeightedResult<T>(r, toId, fromId);
		}

		public static WeightedResult<T> Dijkstra<T>(int vertexesCount, Func<T, int> toId, Func<int, T> fromId, WeightedEdge<T>[] edges, bool directed, T startVertex, T endVertex)
		{
			var map = WeightedEdgesToMap(vertexesCount, edges, directed, toId);
			var r = ShortestPathCore.Dijkstra(vertexesCount, id => map[id].ToArray(), toId(startVertex), toId(endVertex));
			return new WeightedResult<T>(r, toId, fromId);
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

		public static List<WeightedEdge>[] WeightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<WeightedEdge>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(new WeightedEdge(e[0], e[1], e[2]));
				if (!directed) map[e[1]].Add(new WeightedEdge(e[1], e[0], e[2]));
			}
			return map;
		}

		public static List<WeightedEdge>[] WeightedEdgesToMap(int vertexesCount, WeightedEdge[] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<WeightedEdge>());
			foreach (var e in edges)
			{
				map[e.From].Add(e);
				if (!directed) map[e.To].Add(e.Reverse());
			}
			return map;
		}

		public static List<WeightedEdge>[] WeightedEdgesToMap<T>(int vertexesCount, WeightedEdge<T>[] edges, bool directed, Func<T, int> toId)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<WeightedEdge>());
			foreach (var te in edges)
			{
				var e = te.Untype(toId);
				map[e.From].Add(e);
				if (!directed) map[e.To].Add(e.Reverse());
			}
			return map;
		}
		#endregion
	}

	public struct WeightedEdge<T>
	{
		public T From { get; }
		public T To { get; }
		public long Cost { get; }

		public WeightedEdge(T from, T to, long cost)
		{
			From = from;
			To = to;
			Cost = cost;
		}
		public WeightedEdge(WeightedEdge e, Func<int, T> fromId) : this(fromId(e.From), fromId(e.To), e.Cost) { }

		public WeightedEdge Untype(Func<T, int> toId) => new WeightedEdge(toId(From), toId(To), Cost);
		public WeightedEdge<T> Reverse() => new WeightedEdge<T>(To, From, Cost);
	}

	public class UnweightedResult<T> : UnweightedResult
	{
		Func<T, int> ToId;
		Func<int, T> FromId;
		public long this[T vertex] => RawCosts[ToId(vertex)];
		public bool IsConnected(T vertex) => RawCosts[ToId(vertex)] != long.MaxValue;

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

	public class WeightedResult<T> : WeightedResult
	{
		Func<T, int> ToId;
		Func<int, T> FromId;
		public long this[T vertex] => RawCosts[ToId(vertex)];
		public bool IsConnected(T vertex) => RawCosts[ToId(vertex)] != long.MaxValue;

		public WeightedResult(WeightedResult result, Func<T, int> toId, Func<int, T> fromId) : base(result.RawCosts, result.RawInEdges)
		{
			ToId = toId;
			FromId = fromId;
		}

		public T[] GetPathVertexes(T endVertex)
		{
			return Array.ConvertAll(GetPathVertexes(ToId(endVertex)), id => FromId(id));
		}

		public WeightedEdge<T>[] GetPathEdges(T endVertex)
		{
			return Array.ConvertAll(GetPathEdges(ToId(endVertex)), e => new WeightedEdge<T>(e, FromId));
		}
	}
}
