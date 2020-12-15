using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	public static class ShortestPath
	{
	}

	public static class ShortestPathCore
	{
		public static long[] Bfs(int vertexesCount, Func<int, IEnumerable<int>> getNextVertexes, int startVertexId, int endVertexId = -1)
		{
			var costs = Array.ConvertAll(new bool[vertexesCount], _ => long.MaxValue);
			var q = new Queue<int>();
			costs[startVertexId] = 0;
			q.Enqueue(startVertexId);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				var nc = costs[v] + 1;

				foreach (var nv in getNextVertexes(v))
				{
					if (costs[nv] <= nc) continue;
					costs[nv] = nc;
					if (nv == endVertexId) return costs;
					q.Enqueue(nv);
				}
			}
			return costs;
		}

		public static long[] Bfs(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(e[1]);
				if (!directed) map[e[1]].Add(e[0]);
			}
			return Bfs(vertexesCount, v => map[v], startVertexId, endVertexId);
		}
	}
}
