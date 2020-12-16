using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	/// <summary>
	/// 最短経路アルゴリズムの核となる機能を提供します。
	/// ここでは整数型の ID を使用します。
	/// 整数型以外の ID を使用するには、<see cref="ShortestPath"/> クラスを呼び出します。
	/// </summary>
	public static class ShortestPathCore
	{
		/// <summary>
		/// 幅優先探索により、始点から各頂点への最短経路を求めます。
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
	}
}
