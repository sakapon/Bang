using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
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

		public static Func<T, long> Bfs<T>(int vertexesCount, Func<T, int> toId, Func<int, T> fromId, Func<T, T[]> getNextVertexes, T startVertex, T endVertex)
		{
			var r = ShortestPathCore.Bfs(vertexesCount, id => Array.ConvertAll(getNextVertexes(fromId(id)), v => toId(v)), toId(startVertex), toId(endVertex));
			return v => r[toId(v)];
		}

		public static Func<T, long> Bfs<T>(int vertexesCount, Func<T, int> toId, T[][] edges, bool directed, T startVertex, T endVertex)
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
			return v => r[toId(v)];
		}
	}

	public static class ShortestPathCore
	{
		/// <summary>
		/// 幅優先探索により、始点からの最短経路を求めます。
		/// </summary>
		/// <param name="vertexesCount">頂点の個数。これ未満の値を ID として使用できます。</param>
		/// <param name="getNextVertexes">指定された頂点からの行先を取得するための関数。</param>
		/// <param name="startVertexId">始点の ID。</param>
		/// <param name="endVertexId">終点の ID。終点を指定しない場合、-1。</param>
		/// <returns>頂点ごとの最小コスト。到達不可能の場合、<see cref="long.MaxValue"/>。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。したがって、1-indexed でも利用できます。<br/>
		/// 辺のコストはすべて 1 です。
		/// </remarks>
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
