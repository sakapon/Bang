using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs0.Spp1
{
	/// <summary>
	/// 最短経路を求めるための静的メソッドを提供します。
	/// </summary>
	public static class ShortestPath1
	{
		public static UnweightedResult Bfs(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = GraphConvert1.UnweightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore1.Bfs(vertexesCount, v => map[v].ToArray(), startVertexId, endVertexId);
		}

		public static WeightedResult Dijkstra(int vertexesCount, Edge[] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = GraphConvert1.WeightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore1.Dijkstra(vertexesCount, v => map[v].ToArray(), startVertexId, endVertexId);
		}

		public static WeightedResult Dijkstra(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = GraphConvert1.WeightedEdgesToMap(vertexesCount, edges, directed);
			return ShortestPathCore1.Dijkstra(vertexesCount, v => map[v].ToArray(), startVertexId, endVertexId);
		}
	}

	/// <summary>
	/// 最短経路アルゴリズムの核となる機能を提供します。
	/// ここでは整数型の ID を使用します。
	/// </summary>
	public static class ShortestPathCore1
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

	public static class GraphConvert1
	{
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

		public static List<Edge>[] WeightedEdgesToMap(int vertexesCount, Edge[] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<Edge>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e.From].Add(e);
				if (!directed) map[e.To].Add(e.Reverse());
			}
			return map;
		}

		public static List<Edge>[] WeightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			return WeightedEdgesToMap(vertexesCount, Array.ConvertAll(edges, e => new Edge(e)), directed);
		}
	}

	/// <summary>
	/// 優先度付きキューを表します。
	/// </summary>
	/// <typeparam name="T">オブジェクトの型。</typeparam>
	/// <remarks>
	/// 二分ヒープによる実装です。<br/>
	/// 内部では 1-indexed のため、raw array を直接ソートする用途では使われません。
	/// </remarks>
	public class PriorityQueue<T>
	{
		public static PriorityQueue<T> Create(bool descending = false)
		{
			var c = Comparer<T>.Default;
			return descending ?
				new PriorityQueue<T>((x, y) => c.Compare(y, x)) :
				new PriorityQueue<T>(c.Compare);
		}

		public static PriorityQueue<T> Create<TKey>(Func<T, TKey> keySelector, bool descending = false)
		{
			if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

			var c = Comparer<TKey>.Default;
			return descending ?
				new PriorityQueue<T>((x, y) => c.Compare(keySelector(y), keySelector(x))) :
				new PriorityQueue<T>((x, y) => c.Compare(keySelector(x), keySelector(y)));
		}

		public static PriorityQueue<T, TKey> CreateWithKey<TKey>(Func<T, TKey> keySelector, bool descending = false)
		{
			var c = Comparer<TKey>.Default;
			return descending ?
				new PriorityQueue<T, TKey>(keySelector, (x, y) => c.Compare(y.key, x.key)) :
				new PriorityQueue<T, TKey>(keySelector, (x, y) => c.Compare(x.key, y.key));
		}

		List<T> l = new List<T> { default };
		Comparison<T> c;

		public T First
		{
			get
			{
				if (l.Count <= 1) throw new InvalidOperationException("The heap is empty.");
				return l[1];
			}
		}

		public int Count => l.Count - 1;
		public bool Any => l.Count > 1;

		internal PriorityQueue(Comparison<T> comparison)
		{
			c = comparison ?? throw new ArgumentNullException(nameof(comparison));
		}

		// x の親: x/2
		// x の子: 2x, 2x+1
		void UpHeap(int i)
		{
			for (int j; (j = i >> 1) > 0 && c(l[j], l[i]) > 0; i = j)
				(l[i], l[j]) = (l[j], l[i]);
		}

		void DownHeap(int i)
		{
			for (int j; (j = i << 1) < l.Count; i = j)
			{
				if (j + 1 < l.Count && c(l[j], l[j + 1]) > 0) j++;
				if (c(l[i], l[j]) > 0) (l[i], l[j]) = (l[j], l[i]); else break;
			}
		}

		public void Push(T value)
		{
			l.Add(value);
			UpHeap(l.Count - 1);
		}

		public void PushRange(IEnumerable<T> values)
		{
			if (values != null) foreach (var v in values) Push(v);
		}

		public T Pop()
		{
			if (l.Count <= 1) throw new InvalidOperationException("The heap is empty.");

			var r = l[1];
			l[1] = l[l.Count - 1];
			l.RemoveAt(l.Count - 1);
			DownHeap(1);
			return r;
		}
	}

	// キーをキャッシュすることにより、キーが不変であることを保証します。
	public class PriorityQueue<T, TKey> : PriorityQueue<(T value, TKey key)>
	{
		Func<T, TKey> KeySelector;

		internal PriorityQueue(Func<T, TKey> keySelector, Comparison<(T value, TKey key)> comparison) : base(comparison)
		{
			KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
		}

		public void Push(T value)
		{
			Push((value, KeySelector(value)));
		}

		public void PushRange(IEnumerable<T> values)
		{
			if (values != null) foreach (var v in values) Push(v);
		}
	}
}
