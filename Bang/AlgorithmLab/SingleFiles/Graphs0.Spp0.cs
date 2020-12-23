using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs0.Spp0
{
	/// <summary>
	/// 最短経路を求めるための静的メソッドを提供します。
	/// </summary>
	/// <remarks>
	/// 配列を用いた基本的な実装です。
	/// </remarks>
	public static class ShortestPath0
	{
		/// <summary>
		/// 幅優先探索により、始点から各頂点への最短経路を求めます。
		/// </summary>
		/// <param name="vertexesCount">頂点の個数。これ未満の値を ID として使用できます。</param>
		/// <param name="getNextVertexes">指定された頂点からの行先を取得するための関数。</param>
		/// <param name="startVertexId">始点の ID。</param>
		/// <param name="endVertexId">終点の ID。終点を指定しない場合、-1。</param>
		/// <returns>
		/// 最小コストおよび入頂点。<br/>
		/// ある頂点に到達不可能の場合、最小コストは <see cref="long.MaxValue"/>、入頂点は <c>-1</c>。
		/// </returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。したがって、1-indexed でも利用できます。<br/>
		/// 辺のコストはすべて 1 として扱われます。
		/// </remarks>
		public static (long[] minCosts, int[] inVertexes) Bfs(int vertexesCount, Func<int, int[]> getNextVertexes, int startVertexId, int endVertexId = -1)
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
					if (nv == endVertexId) return (costs, inVertexes);
					q.Enqueue(nv);
				}
			}
			return (costs, inVertexes);
		}

		/// <summary>
		/// Dijkstra 法により、始点から各頂点への最短経路を求めます。
		/// </summary>
		/// <param name="vertexesCount">頂点の個数。これ未満の値を ID として使用できます。</param>
		/// <param name="edges">辺のリスト。edge: { from, to, cost }</param>
		/// <param name="directed">有向グラフかどうかを示す値。</param>
		/// <param name="startVertexId">始点の ID。</param>
		/// <param name="endVertexId">終点の ID。終点を指定しない場合、-1。</param>
		/// <returns>
		/// 最小コストおよび入辺。<br/>
		/// ある頂点に到達不可能の場合、最小コストは <see cref="long.MaxValue"/>、入辺は <see langword="null"/>。
		/// </returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。したがって、1-indexed でも利用できます。<br/>
		/// 辺のコストは非負でなければなりません。
		/// </remarks>
		public static (long[] minCosts, int[][] inEdges) Dijkstra(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var n = vertexesCount;
			if (edges == null) throw new ArgumentNullException(nameof(edges));

			var map = Array.ConvertAll(new bool[n], _ => new List<int[]>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(new[] { e[0], e[1], e[2] });
				if (!directed) map[e[1]].Add(new[] { e[1], e[0], e[2] });
			}

			var costs = Array.ConvertAll(new bool[n], _ => long.MaxValue);
			var inEdges = new int[n][];
			var q = PriorityQueue<int>.CreateWithKey(v => costs[v]);
			costs[startVertexId] = 0;
			q.Push(startVertexId);

			while (q.Any)
			{
				var (v, c) = q.Pop();
				if (v == endVertexId) break;
				if (costs[v] < c) continue;

				foreach (var e in map[v])
				{
					var (nv, nc) = (e[1], c + e[2]);
					if (costs[nv] <= nc) continue;
					costs[nv] = nc;
					inEdges[nv] = e;
					q.Push(nv);
				}
			}
			return (costs, inEdges);
		}

		/// <summary>
		/// 幅優先探索の拡張により、始点から各頂点への最短経路を求めます。<br/>
		/// 例えば <paramref name="m"/> = 3 のとき、012-BFS を表します。
		/// </summary>
		/// <param name="vertexesCount">頂点の個数。これ未満の値を ID として使用できます。</param>
		/// <param name="m">辺のコストの候補となる数。辺のコストはこれ未満の値に制限されます。</param>
		/// <param name="edges">辺のリスト。edge: { from, to, cost }</param>
		/// <param name="directed">有向グラフかどうかを示す値。</param>
		/// <param name="startVertexId">始点の ID。</param>
		/// <param name="endVertexId">終点の ID。終点を指定しない場合、-1。</param>
		/// <returns>
		/// 最小コストおよび入辺。<br/>
		/// ある頂点に到達不可能の場合、最小コストは <see cref="long.MaxValue"/>、入辺は <see langword="null"/>。
		/// </returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。したがって、1-indexed でも利用できます。<br/>
		/// 辺のコストは <c>0 &lt;= c &lt; m</c> を満たさなければなりません。
		/// </remarks>
		public static (long[] minCosts, int[][] inEdges) BfsM(int vertexesCount, int m, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var n = vertexesCount;
			if (edges == null) throw new ArgumentNullException(nameof(edges));

			var map = Array.ConvertAll(new bool[n], _ => new List<int[]>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(new[] { e[0], e[1], e[2] });
				if (!directed) map[e[1]].Add(new[] { e[1], e[0], e[2] });
			}

			var costs = Array.ConvertAll(new bool[n], _ => long.MaxValue);
			var inEdges = new int[n][];
			var qs = Array.ConvertAll(new bool[m], _ => new Queue<int>());
			costs[startVertexId] = 0;
			qs[0].Enqueue(startVertexId);

			for (long c = 0; Array.Exists(qs, q => q.Count > 0); ++c)
			{
				var q = qs[c % m];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					if (v == endVertexId) break;
					if (costs[v] < c) continue;

					foreach (var e in map[v])
					{
						var (nv, nc) = (e[1], c + e[2]);
						if (costs[nv] <= nc) continue;
						costs[nv] = nc;
						inEdges[nv] = e;
						qs[nc % m].Enqueue(nv);
					}
				}
			}
			return (costs, inEdges);
		}

		// priority queue ではなく、queue を使うほうが速いことがあります。
		[Obsolete("最悪計算量は O(VE) です。")]
		public static (long[] minCosts, int[][] inEdges) Dijklmna(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var n = vertexesCount;
			var map = Array.ConvertAll(new bool[n], _ => new List<int[]>());
			foreach (var e in edges)
			{
				map[e[0]].Add(new[] { e[0], e[1], e[2] });
				if (!directed) map[e[1]].Add(new[] { e[1], e[0], e[2] });
			}

			var costs = Array.ConvertAll(new bool[n], _ => long.MaxValue);
			var inEdges = new int[n][];
			var q = new Queue<int>();
			costs[startVertexId] = 0;
			q.Enqueue(startVertexId);

			while (q.Count > 0)
			{
				var v = q.Dequeue();

				foreach (var e in map[v])
				{
					var nc = costs[v] + e[2];
					if (costs[e[1]] <= nc) continue;
					costs[e[1]] = nc;
					inEdges[e[1]] = e;
					q.Enqueue(e[1]);
				}
			}
			return (costs, inEdges);
		}

		/// <summary>
		/// Bellman-Ford 法により、始点から各頂点への最短経路を求めます。
		/// </summary>
		/// <param name="vertexesCount">頂点の個数。これ未満の値を ID として使用できます。</param>
		/// <param name="directedEdges">有向辺のリスト。edge: { from, to, cost }</param>
		/// <param name="startVertexId">始点の ID。</param>
		/// <returns>
		/// 最小コストおよび入辺。<br/>
		/// 負閉路が存在するとき、ともに <see langword="null"/>。<br/>
		/// ある頂点に到達不可能の場合、最小コストは <see cref="long.MaxValue"/>、入辺は <see langword="null"/>。
		/// </returns>
		/// <remarks>
		/// 通常、負値を含む有向グラフに対して使われます。連結性、多重性、開閉を問いません。したがって、1-indexed でも利用できます。<br/>
		/// 負閉路が存在するときは検出されます。<br/>
		/// 計算量: O(VE)
		/// </remarks>
		public static (long[] minCosts, int[][] inEdges) BellmanFord(int vertexesCount, int[][] directedEdges, int startVertexId)
		{
			var n = vertexesCount;
			if (directedEdges == null) throw new ArgumentNullException(nameof(directedEdges));
			// 入力チェックは省略。

			var costs = Array.ConvertAll(new bool[n], _ => long.MaxValue);
			var inEdges = new int[n][];
			costs[startVertexId] = 0;

			// V-1 回後に true であっても、負閉路の有無は確定しません。
			var next = true;
			for (int k = 0; k < n && next; ++k)
			{
				next = false;
				foreach (var e in directedEdges)
				{
					if (costs[e[0]] == long.MaxValue) continue;
					var nc = costs[e[0]] + e[2];
					if (costs[e[1]] <= nc) continue;
					costs[e[1]] = nc;
					inEdges[e[1]] = e;
					next = true;
				}
			}
			if (next) return (null, null);
			return (costs, inEdges);
		}

		/// <summary>
		/// Warshall-Floyd 法により、すべての頂点の間の最短経路を求めます。
		/// </summary>
		/// <param name="vertexesCount">頂点の個数。これ未満の値を ID として使用できます。</param>
		/// <param name="edges">辺のリスト。edge: { from, to, cost }</param>
		/// <param name="directed">有向グラフかどうかを示す値。</param>
		/// <returns>
		/// 各始点と終点のペアに対する最小コストおよび中間点。<br/>
		/// 負閉路が存在するとき、ともに <see langword="null"/>。<br/>
		/// ある始点から終点へ到達不可能の場合、最小コストは <see cref="long.MaxValue"/>、中間点は -1。<br/>
		/// ただし到達可能であっても、中間点がない場合は -1。
		/// </returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。したがって、1-indexed でも利用できます。<br/>
		/// 負閉路が存在するときは検出されます。<br/>
		/// 計算量: O(V^3)
		/// </remarks>
		public static (long[][] minCosts, int[][] interVertexes) WarshallFloyd(int vertexesCount, int[][] edges, bool directed)
		{
			var n = vertexesCount;
			if (edges == null) throw new ArgumentNullException(nameof(edges));
			// 入力チェックは省略。

			var costs = Array.ConvertAll(new bool[n], _ => Array.ConvertAll(new bool[n], _ => long.MaxValue));
			var inters = Array.ConvertAll(new bool[n], _ => Array.ConvertAll(new bool[n], _ => -1));
			for (int i = 0; i < n; ++i) costs[i][i] = 0;

			foreach (var e in edges)
			{
				costs[e[0]][e[1]] = Math.Min(costs[e[0]][e[1]], e[2]);
				if (!directed) costs[e[1]][e[0]] = Math.Min(costs[e[1]][e[0]], e[2]);
			}

			for (int k = 0; k < n; ++k)
				for (int i = 0; i < n; ++i)
					for (int j = 0; j < n; ++j)
					{
						if (costs[i][k] == long.MaxValue || costs[k][j] == long.MaxValue) continue;
						var nc = costs[i][k] + costs[k][j];
						if (costs[i][j] <= nc) continue;
						costs[i][j] = nc;
						inters[i][j] = k;
					}
			for (int i = 0; i < n; ++i) if (costs[i][i] < 0) return (null, null);
			return (costs, inters);
		}

		public static int[] GetPathVertexes(int[] inVertexes, int endVertexId)
		{
			var path = new Stack<int>();
			for (var v = endVertexId; v != -1; v = inVertexes[v])
				path.Push(v);
			return path.ToArray();
		}

		public static int[] GetPathVertexes(int[][] inEdges, int endVertexId)
		{
			var path = new Stack<int>();
			path.Push(endVertexId);
			for (var e = inEdges[endVertexId]; e != null; e = inEdges[e[0]])
				path.Push(e[0]);
			return path.ToArray();
		}

		public static int[][] GetPathEdges(int[][] inEdges, int endVertexId)
		{
			var path = new Stack<int[]>();
			for (var e = inEdges[endVertexId]; e != null; e = inEdges[e[0]])
				path.Push(e);
			return path.ToArray();
		}

		// For Warshall-Floyd
		public static int[] GetPathVertexes(int[][] interVertexes, int startVertexId, int endVertexId)
		{
			var path = new List<int>();
			path.Add(startVertexId);
			Dfs(startVertexId, endVertexId);
			path.Add(endVertexId);
			return path.ToArray();

			void Dfs(int i, int j)
			{
				var k = interVertexes[i][j];
				if (k == -1) return;
				Dfs(i, k);
				path.Add(k);
				Dfs(k, j);
			}
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
