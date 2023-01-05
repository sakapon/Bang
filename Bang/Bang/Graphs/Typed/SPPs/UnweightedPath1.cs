using System;
using System.Collections.Generic;

namespace Bang.Graphs.Typed.SPPs
{
	public static class UnweightedPath1
	{
		static Dictionary<T, TValue> CreateVertexes<T, TValue>(this UnweightedGraph<T> graph, TValue value)
		{
			var d = new Dictionary<T, TValue>();
			foreach (var v in graph.GetVertexes()) d[v] = value;
			return d;
		}

		// 最短経路とは限りません。
		// 連結性のみを判定する場合は、DFS、BFS または Union-Find を利用します。
		public static Dictionary<T, bool> ConnectivityByDFS<T>(this UnweightedGraph<T> graph, T sv, T ev)
		{
			var u = graph.CreateVertexes(false);
			u[sv] = true;
			DFS(sv);
			return u;

			bool DFS(T v)
			{
				if (u.Comparer.Equals(v, ev)) return true;
				foreach (var nv in graph.GetEdges(v))
				{
					if (u[nv]) continue;
					u[nv] = true;
					if (DFS(nv)) return true;
				}
				return false;
			}
		}

		public static Dictionary<T, long> ShortestByBFS<T>(this UnweightedGraph<T> graph, T sv, T ev)
		{
			var costs = graph.CreateVertexes(long.MaxValue);
			costs[sv] = 0;
			var q = new Queue<T>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (costs.Comparer.Equals(v, ev)) return costs;
				var nc = costs[v] + 1;

				foreach (var nv in graph.GetEdges(v))
				{
					if (costs[nv] <= nc) continue;
					costs[nv] = nc;
					q.Enqueue(nv);
				}
			}
			return costs;
		}
	}
}
