using System;
using System.Collections.Generic;

namespace Bang.Graphs.Int.SPPs
{
	public static class PathEx1
	{
		// 最短経路とは限りません。
		// 連結性のみを判定する場合は、DFS、BFS または Union-Find を利用します。
		public static bool[] ConnectivityByDFS(this UnweightedGraph graph, int sv, int ev = -1)
		{
			var u = new bool[graph.VertexesCount];
			u[sv] = true;
			DFS(sv);
			return u;

			bool DFS(int v)
			{
				if (v == ev) return true;
				foreach (var nv in graph.GetEdges(v))
				{
					if (u[nv]) continue;
					u[nv] = true;
					if (DFS(nv)) return true;
				}
				return false;
			}
		}

		public static long[] ShortestByBFS(this UnweightedGraph graph, int sv, int ev = -1)
		{
			var costs = Array.ConvertAll(new bool[graph.VertexesCount], _ => long.MaxValue);
			costs[sv] = 0;
			var q = new Queue<int>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (v == ev) return costs;
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

		public static long[] Dijkstra(this WeightedGraph graph, int sv, int ev = -1)
		{
			var costs = Array.ConvertAll(new bool[graph.VertexesCount], _ => long.MaxValue);
			costs[sv] = 0;
			var q = new SortedSet<(long, int)> { (0, sv) };

			while (q.Count > 0)
			{
				var (c, v) = q.Min;
				q.Remove((c, v));
				if (v == ev) return costs;

				foreach (var (nv, cost) in graph.GetEdges(v))
				{
					var nc = c + cost;
					if (costs[nv] <= nc) continue;
					if (costs[nv] != long.MaxValue) q.Remove((costs[nv], nv));
					q.Add((nc, nv));
					costs[nv] = nc;
				}
			}
			return costs;
		}

		// Dijkstra 法の特別な場合です。
		public static long[] ShortestByModBFS(this WeightedGraph graph, int mod, int sv, int ev = -1)
		{
			var costs = Array.ConvertAll(new bool[graph.VertexesCount], _ => long.MaxValue);
			costs[sv] = 0;
			var qs = Array.ConvertAll(new bool[mod], _ => new Queue<int>());
			qs[0].Enqueue(sv);
			var qc = 1;

			for (long c = 0; qc > 0; ++c)
			{
				var q = qs[c % mod];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					--qc;
					if (v == ev) return costs;
					if (costs[v] < c) continue;

					foreach (var (nv, cost) in graph.GetEdges(v))
					{
						var nc = c + cost;
						if (costs[nv] <= nc) continue;
						costs[nv] = nc;
						qs[nc % mod].Enqueue(nv);
						++qc;
					}
				}
			}
			return costs;
		}
	}
}
