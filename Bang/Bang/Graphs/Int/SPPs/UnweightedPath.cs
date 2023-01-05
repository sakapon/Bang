using System;
using System.Collections.Generic;

namespace Bang.Graphs.Int.SPPs
{
	public static class UnweightedPath
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
	}
}
