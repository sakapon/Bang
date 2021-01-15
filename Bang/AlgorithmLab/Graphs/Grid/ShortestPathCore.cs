using System;
using System.Collections.Generic;
using AlgorithmLab.DataTrees;

namespace AlgorithmLab.Graphs.Grid
{
	/// <summary>
	/// 最短経路アルゴリズムの核となる機能を提供します。
	/// ここでは整数型の ID を使用します。
	/// </summary>
	public static class ShortestPathCore
	{
		/// <summary>
		/// 幅優先探索により、始点から各頂点への最短経路を求めます。<br/>
		/// 辺のコストはすべて 1 として扱われます。
		/// </summary>
		/// <param name="height">高さ。</param>
		/// <param name="width">幅。</param>
		/// <param name="getNextVertexes">指定された頂点からの行先を取得するための関数。</param>
		/// <param name="startVertex">始点。</param>
		/// <param name="endVertex">終点。終点を指定しない場合、<c>(-1, -1)</c>。</param>
		/// <returns>探索結果を表す <see cref="UnweightedResult"/> オブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。
		/// </remarks>
		public static UnweightedResult Bfs(int height, int width, Func<Point, Point[]> getNextVertexes, Point startVertex, Point endVertex)
		{
			var costs = GridMap.Create(height, width, long.MaxValue);
			var inVertexes = GridMap.Create(height, width, new Point(-1, -1));
			var q = new Queue<Point>();
			costs[startVertex] = 0;
			q.Enqueue(startVertex);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				var nc = costs[v] + 1;

				// IEnumerable<T>, List<T>, T[] の順に高速になります。
				foreach (var nv in getNextVertexes(v))
				{
					if (costs[nv] <= nc) continue;
					costs[nv] = nc;
					inVertexes[nv] = v;
					if (nv == endVertex) return new UnweightedResult(costs, inVertexes);
					q.Enqueue(nv);
				}
			}
			return new UnweightedResult(costs, inVertexes);
		}

		/// <summary>
		/// Dijkstra 法により、始点から各頂点への最短経路を求めます。<br/>
		/// 辺のコストは非負でなければなりません。
		/// </summary>
		/// <param name="height">高さ。</param>
		/// <param name="width">幅。</param>
		/// <param name="getNextEdges">指定された頂点からの出辺を取得するための関数。</param>
		/// <param name="startVertex">始点。</param>
		/// <param name="endVertex">終点。終点を指定しない場合、<c>(-1, -1)</c>。</param>
		/// <returns>探索結果を表す <see cref="WeightedResult"/> オブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。
		/// </remarks>
		public static WeightedResult Dijkstra(int height, int width, Func<Point, Edge[]> getNextEdges, Point startVertex, Point endVertex)
		{
			var costs = GridMap.Create(height, width, long.MaxValue);
			var inEdges = GridMap.Create(height, width, Edge.Invalid);
			var q = PriorityQueue<Point>.CreateWithKey(v => costs[v]);
			costs[startVertex] = 0;
			q.Push(startVertex);

			while (q.Any)
			{
				var (v, c) = q.Pop();
				if (v == endVertex) break;
				if (costs[v] < c) continue;

				// IEnumerable<T>, List<T>, T[] の順に高速になります。
				foreach (var e in getNextEdges(v))
				{
					var (nv, nc) = (e.To, c + e.Cost);
					if (costs[nv] <= nc) continue;
					costs[nv] = nc;
					inEdges[nv] = e;
					q.Push(nv);
				}
			}
			return new WeightedResult(costs, inEdges);
		}

		/// <summary>
		/// 幅優先探索の拡張により、始点から各頂点への最短経路を求めます。<br/>
		/// 例えば <paramref name="m"/> = 3 のとき、012-BFS を表します。<br/>
		/// 辺のコストの範囲は [0, <paramref name="m"/>) です。
		/// </summary>
		/// <param name="m">辺のコストの候補となる数。</param>
		/// <param name="height">高さ。</param>
		/// <param name="width">幅。</param>
		/// <param name="getNextEdges">指定された頂点からの出辺を取得するための関数。</param>
		/// <param name="startVertex">始点。</param>
		/// <param name="endVertex">終点。終点を指定しない場合、<c>(-1, -1)</c>。</param>
		/// <returns>探索結果を表す <see cref="WeightedResult"/> オブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。
		/// </remarks>
		public static WeightedResult BfsMod(int m, int height, int width, Func<Point, Edge[]> getNextEdges, Point startVertex, Point endVertex)
		{
			var costs = GridMap.Create(height, width, long.MaxValue);
			var inEdges = GridMap.Create(height, width, Edge.Invalid);
			var qs = Array.ConvertAll(new bool[m], _ => new Queue<Point>());
			costs[startVertex] = 0;
			qs[0].Enqueue(startVertex);

			for (long c = 0; Array.Exists(qs, q => q.Count > 0); ++c)
			{
				var q = qs[c % m];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					if (v == endVertex) return new WeightedResult(costs, inEdges);
					if (costs[v] < c) continue;

					foreach (var e in getNextEdges(v))
					{
						var (nv, nc) = (e.To, c + e.Cost);
						if (costs[nv] <= nc) continue;
						costs[nv] = nc;
						inEdges[nv] = e;
						qs[nc % m].Enqueue(nv);
					}
				}
			}
			return new WeightedResult(costs, inEdges);
		}
	}
}
