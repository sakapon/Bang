using System;
using System.Collections.Generic;
using AlgorithmLab.DataTrees;

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
		/// <returns>探索結果を表す <see cref="UnweightedResult"/> オブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。したがって、1-indexed でも利用できます。<br/>
		/// 辺のコストはすべて 1 として扱われます。
		/// </remarks>
		public static UnweightedResult Bfs(int vertexesCount, Func<int, int[]> getNextVertexes, int startVertexId, int endVertexId = -1)
		{
			var costs = Array.ConvertAll(new bool[vertexesCount], _ => long.MaxValue);
			var inVertexes = Array.ConvertAll(costs, _ => -1);
			var q = new Queue<int>();
			costs[startVertexId] = 0;
			q.Enqueue(startVertexId);

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
					if (nv == endVertexId) return new UnweightedResult(costs, inVertexes);
					q.Enqueue(nv);
				}
			}
			return new UnweightedResult(costs, inVertexes);
		}

		/// <summary>
		/// Dijkstra 法により、始点から各頂点への最短経路を求めます。
		/// </summary>
		/// <param name="vertexesCount">頂点の個数。これ未満の値を ID として使用できます。</param>
		/// <param name="getNextEdges">指定された頂点からの出辺を取得するための関数。</param>
		/// <param name="startVertexId">始点の ID。</param>
		/// <param name="endVertexId">終点の ID。終点を指定しない場合、-1。</param>
		/// <returns>探索結果を表す <see cref="WeightedResult"/> オブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。したがって、1-indexed でも利用できます。<br/>
		/// 辺のコストは非負でなければなりません。
		/// </remarks>
		public static WeightedResult Dijkstra(int vertexesCount, Func<int, Edge[]> getNextEdges, int startVertexId, int endVertexId = -1)
		{
			var costs = Array.ConvertAll(new bool[vertexesCount], _ => long.MaxValue);
			var inEdges = Array.ConvertAll(costs, _ => Edge.Invalid);
			var q = PriorityQueue<int>.CreateWithKey(v => costs[v]);
			costs[startVertexId] = 0;
			q.Push(startVertexId);

			while (q.Any)
			{
				var (v, c) = q.Pop();
				if (v == endVertexId) break;
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
	}

	public struct Edge
	{
		public static Edge Invalid { get; } = new Edge(-1, -1, -1);

		public int From { get; }
		public int To { get; }
		public long Cost { get; }

		public Edge(int from, int to, long cost)
		{
			From = from;
			To = to;
			Cost = cost;
		}
		public Edge(int[] e) : this(e[0], e[1], e.Length > 2 ? e[2] : 0) { }
		public Edge(long[] e) : this((int)e[0], (int)e[1], e.Length > 2 ? e[2] : 0) { }

		public Edge Reverse() => new Edge(To, From, Cost);
	}

	public class UnweightedResult
	{
		public long[] RawCosts { get; }
		public int[] RawInVertexes { get; }
		public long this[int vertexId] => RawCosts[vertexId];
		public bool IsConnected(int vertexId) => RawCosts[vertexId] != long.MaxValue;

		public UnweightedResult(long[] costs, int[] inVertexes)
		{
			RawCosts = costs;
			RawInVertexes = inVertexes;
		}

		public int[] GetPathVertexes(int endVertexId)
		{
			var path = new Stack<int>();
			for (var v = endVertexId; v != -1; v = RawInVertexes[v])
				path.Push(v);
			return path.ToArray();
		}
	}

	public class WeightedResult
	{
		public long[] RawCosts { get; }
		public Edge[] RawInEdges { get; }
		public long this[int vertexId] => RawCosts[vertexId];
		public bool IsConnected(int vertexId) => RawCosts[vertexId] != long.MaxValue;

		public WeightedResult(long[] costs, Edge[] inEdges)
		{
			RawCosts = costs;
			RawInEdges = inEdges;
		}

		public int[] GetPathVertexes(int endVertexId)
		{
			var path = new Stack<int>();
			for (var v = endVertexId; v != -1; v = RawInEdges[v].From)
				path.Push(v);
			return path.ToArray();
		}

		public Edge[] GetPathEdges(int endVertexId)
		{
			var path = new Stack<Edge>();
			for (var e = RawInEdges[endVertexId]; e.From != -1; e = RawInEdges[e.From])
				path.Push(e);
			return path.ToArray();
		}
	}
}
