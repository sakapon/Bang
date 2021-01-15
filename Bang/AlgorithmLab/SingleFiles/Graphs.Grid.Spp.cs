using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Grid.Spp
{
	public struct Point : IEquatable<Point>
	{
		public int i, j;
		public Point(int i, int j) { this.i = i; this.j = j; }
		public void Deconstruct(out int i, out int j) { i = this.i; j = this.j; }
		public override string ToString() => $"{i} {j}";
		public static Point Parse(string s) => Array.ConvertAll(s.Split(), int.Parse);

		public static implicit operator Point(int[] v) => (v[0], v[1]);
		public static explicit operator int[](Point v) => new[] { v.i, v.j };
		public static implicit operator Point((int i, int j) v) => new Point(v.i, v.j);
		public static explicit operator (int, int)(Point v) => (v.i, v.j);

		public bool Equals(Point other) => i == other.i && j == other.j;
		public static bool operator ==(Point v1, Point v2) => v1.Equals(v2);
		public static bool operator !=(Point v1, Point v2) => !v1.Equals(v2);
		public override bool Equals(object obj) => obj is Point v && Equals(v);
		public override int GetHashCode() => (i, j).GetHashCode();

		public static Point operator -(Point v) => new Point(-v.i, -v.j);
		public static Point operator +(Point v1, Point v2) => new Point(v1.i + v2.i, v1.j + v2.j);
		public static Point operator -(Point v1, Point v2) => new Point(v1.i - v2.i, v1.j - v2.j);

		public bool IsInRange(int height, int width) => 0 <= i && i < height && 0 <= j && j < width;
		public Point[] Nexts() => new[] { new Point(i - 1, j), new Point(i + 1, j), new Point(i, j - 1), new Point(i, j + 1) };
		public static Point[] NextsByDelta { get; } = new[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };

		public int NormL1 => Math.Abs(i) + Math.Abs(j);
		public double Norm => Math.Sqrt(i * i + j * j);
	}

	public static class GridMap
	{
		public static GridMap<TValue> Create<TValue>(int height, int width, TValue iv) => new JaggedGridMap<TValue>(height, width, iv);
		public static GridMap<TValue> Create<TValue>(int height, int width, Func<TValue> getIV) => new JaggedGridMap<TValue>(height, width, getIV);
	}

	public abstract class GridMap<TValue>
	{
		public abstract int Height { get; }
		public abstract int Width { get; }
		public abstract TValue this[Point key] { get; set; }
		public abstract TValue this[int i, int j] { get; set; }
	}

	public class JaggedGridMap<TValue> : GridMap<TValue>
	{
		TValue[][] a;
		public override int Height => a.Length;
		public override int Width => a[0].Length;
		public JaggedGridMap(TValue[][] a) { this.a = a; }
		public JaggedGridMap(int height, int width, TValue iv)
		{
			a = Array.ConvertAll(new bool[height], _ => Array.ConvertAll(new bool[width], __ => iv));
		}
		public JaggedGridMap(int height, int width, Func<TValue> getIV)
		{
			a = Array.ConvertAll(new bool[height], _ => Array.ConvertAll(new bool[width], __ => getIV()));
		}
		public override TValue this[Point key] { get => a[key.i][key.j]; set => a[key.i][key.j] = value; }
		public override TValue this[int i, int j] { get => a[i][j]; set => a[i][j] = value; }
	}

	public class RectGridMap<TValue> : GridMap<TValue>
	{
		TValue[,] a;
		public override int Height => a.GetLength(0);
		public override int Width => a.GetLength(1);
		public RectGridMap(TValue[,] a) { this.a = a; }
		public RectGridMap(int height, int width, TValue iv)
		{
			a = new TValue[height, width];
			for (int i = 0; i < height; ++i)
				for (int j = 0; j < width; ++j)
					a[i, j] = iv;
		}
		public RectGridMap(int height, int width, Func<TValue> getIV)
		{
			a = new TValue[height, width];
			for (int i = 0; i < height; ++i)
				for (int j = 0; j < width; ++j)
					a[i, j] = getIV();
		}
		public override TValue this[Point key] { get => a[key.i, key.j]; set => a[key.i, key.j] = value; }
		public override TValue this[int i, int j] { get => a[i, j]; set => a[i, j] = value; }
	}

	public struct Edge
	{
		public static Edge Invalid { get; } = new Edge((-1, -1), (-1, -1), long.MinValue);

		public Point From { get; }
		public Point To { get; }
		public long Cost { get; }

		public Edge(Point from, Point to, long cost = 1) { From = from; To = to; Cost = cost; }
		public void Deconstruct(out Point from, out Point to) { from = From; to = To; }
		public void Deconstruct(out Point from, out Point to, out long cost) { from = From; to = To; cost = Cost; }
		public override string ToString() => $"{{{From}}} {{{To}}} {Cost}";

		public static implicit operator Edge((Point from, Point to) v) => new Edge(v.from, v.to);
		public static implicit operator Edge((Point from, Point to, long cost) v) => new Edge(v.from, v.to, v.cost);

		public Edge Reverse() => new Edge(To, From, Cost);
	}

	public class CostResult
	{
		protected static readonly Point InvalidVertex = (-1, -1);

		public GridMap<long> RawCosts { get; }
		public CostResult(GridMap<long> costs) { RawCosts = costs; }

		public long this[Point vertex] => RawCosts[vertex];
		public bool IsConnected(Point vertex) => RawCosts[vertex] != long.MaxValue;
		public long GetCost(Point vertex, long invalid = -1) => IsConnected(vertex) ? RawCosts[vertex] : invalid;
	}

	public class UnweightedResult : CostResult
	{
		public GridMap<Point> RawInVertexes { get; }

		public UnweightedResult(GridMap<long> costs, GridMap<Point> inVertexes) : base(costs)
		{
			RawInVertexes = inVertexes;
		}

		public Point[] GetPathVertexes(Point endVertex)
		{
			var path = new Stack<Point>();
			for (var v = endVertex; v != InvalidVertex; v = RawInVertexes[v])
				path.Push(v);
			return path.ToArray();
		}

		public Edge[] GetPathEdges(Point endVertex)
		{
			var path = new Stack<Edge>();
			for (var v = endVertex; RawInVertexes[v] != InvalidVertex; v = RawInVertexes[v])
				path.Push(new Edge(RawInVertexes[v], v));
			return path.ToArray();
		}
	}

	public class WeightedResult : CostResult
	{
		public GridMap<Edge> RawInEdges { get; }

		public WeightedResult(GridMap<long> costs, GridMap<Edge> inEdges) : base(costs)
		{
			RawInEdges = inEdges;
		}

		public Point[] GetPathVertexes(Point endVertex)
		{
			var path = new Stack<Point>();
			for (var v = endVertex; v != InvalidVertex; v = RawInEdges[v].From)
				path.Push(v);
			return path.ToArray();
		}

		public Edge[] GetPathEdges(Point endVertex)
		{
			var path = new Stack<Edge>();
			for (var e = RawInEdges[endVertex]; e.From != InvalidVertex; e = RawInEdges[e.From])
				path.Push(e);
			return path.ToArray();
		}
	}

	public static class GraphConsole
	{
		const char Road = '.';
		const char Wall = '#';

		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

		public static Point ReadPoint()
		{
			return Point.Parse(Console.ReadLine());
		}

		public static GridMap<char> ReadGrid(int height)
		{
			return new JaggedGridMap<char>(Array.ConvertAll(new bool[height], _ => Console.ReadLine().ToCharArray()));
		}

		public static string[] ReadGridAsString(int height)
		{
			return Array.ConvertAll(new bool[height], _ => Console.ReadLine());
		}

		public static char[][] ReadGridAsChar(int height)
		{
			return Array.ConvertAll(new bool[height], _ => Console.ReadLine().ToCharArray());
		}

		public static int[][] ReadGridAsInt(int height)
		{
			return Array.ConvertAll(new bool[height], _ => Read());
		}

		public static GridMap<char> ReadEnclosedGrid(ref int height, ref int width, char c = '#', int delta = 1)
		{
			var h = height + 2 * delta;
			var w = width + 2 * delta;

			var s = Array.ConvertAll(new bool[h], _ => Array.ConvertAll(new bool[w], __ => c));
			for (int i = 0; i < height; ++i)
			{
				var di = delta + i;
				var l = Console.ReadLine();
				for (int j = 0; j < width; ++j)
					s[di][delta + j] = l[j];
			}

			(height, width) = (h, w);
			return new JaggedGridMap<char>(s);
		}
	}

	public static class GraphConvert
	{
		public static void UnweightedEdgesToMap(GridMap<List<Point>> map, Edge[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map[e.From].Add(e.To);
				if (!directed) map[e.To].Add(e.From);
			}
		}

		public static void WeightedEdgesToMap(GridMap<List<Edge>> map, Edge[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map[e.From].Add(e);
				if (!directed) map[e.To].Add(e.Reverse());
			}
		}
	}

	public static class GridHelper
	{
		public static Point[] GetPoints(int height, int width)
		{
			var ps = new List<Point>();
			for (int i = 0, j = 0; i < height;)
			{
				ps.Add(new Point(i, j));
				if (++j == width) { ++i; j = 0; }
			}
			return ps.ToArray();
		}

		public static Point FindValue<T>(GridMap<T> map, T value)
		{
			var ec = EqualityComparer<T>.Default;
			var (h, w) = (map.Height, map.Width);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (ec.Equals(map[i, j], value)) return new Point(i, j);
			return new Point(-1, -1);
		}

		// 負値を指定できます。
		public static void Enclose<T>(ref int height, ref int width, ref GridMap<T> map, T value, int delta = 1)
		{
			var (h, w) = (height + 2 * delta, width + 2 * delta);
			var (li, ri) = (Math.Max(0, -delta), Math.Min(height, height + delta));
			var (lj, rj) = (Math.Max(0, -delta), Math.Min(width, width + delta));

			var t = Array.ConvertAll(new bool[h], _ => Array.ConvertAll(new bool[w], __ => value));
			for (int i = li; i < ri; ++i)
				for (int j = lj; j < rj; ++j)
					t[delta + i][delta + j] = map[i, j];
			(height, width, map) = (h, w, new JaggedGridMap<T>(t));
		}
	}

	/// <summary>
	/// 最短経路アルゴリズムの核となる機能を提供します。
	/// ここでは <see cref="Point"/> 型の ID を使用します。
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

	public class UnweightedMap
	{
		public int Height { get; }
		public int Width { get; }
		GridMap<List<Point>> map;
		public GridMap<List<Point>> RawMap => map;
		public Point[] this[Point vertex] => map[vertex].ToArray();

		public UnweightedMap(int height, int width, GridMap<List<Point>> map)
		{
			Height = height;
			Width = width;
			this.map = map;
		}

		public UnweightedMap(int height, int width)
		{
			Height = height;
			Width = width;
			map = GridMap.Create(height, width, () => new List<Point>());
		}

		public UnweightedMap(int height, int width, Edge[] edges, bool directed) : this(height, width)
		{
			AddEdges(edges, directed);
		}

		public void AddEdges(Edge[] edges, bool directed)
		{
			GraphConvert.UnweightedEdgesToMap(map, edges, directed);
		}

		public void AddEdge(Edge edge, bool directed)
		{
			map[edge.From].Add(edge.To);
			if (!directed) map[edge.To].Add(edge.From);
		}

		public void AddEdge(Point from, Point to, bool directed)
		{
			map[from].Add(to);
			if (!directed) map[to].Add(from);
		}

		public UnweightedResult Bfs(Point startVertex, Point endVertex)
		{
			return ShortestPathCore.Bfs(Height, Width, v => this[v], startVertex, endVertex);
		}
	}

	public class WeightedMap
	{
		public int Height { get; }
		public int Width { get; }
		GridMap<List<Edge>> map;
		public GridMap<List<Edge>> RawMap => map;
		public Edge[] this[Point vertex] => map[vertex].ToArray();

		public WeightedMap(int height, int width, GridMap<List<Edge>> map)
		{
			Height = height;
			Width = width;
			this.map = map;
		}

		public WeightedMap(int height, int width)
		{
			Height = height;
			Width = width;
			map = GridMap.Create(height, width, () => new List<Edge>());
		}

		public WeightedMap(int height, int width, Edge[] edges, bool directed) : this(height, width)
		{
			AddEdges(edges, directed);
		}

		public void AddEdges(Edge[] edges, bool directed)
		{
			GraphConvert.WeightedEdgesToMap(map, edges, directed);
		}

		public void AddEdge(Edge edge, bool directed)
		{
			map[edge.From].Add(edge);
			if (!directed) map[edge.To].Add(edge.Reverse());
		}

		public void AddEdge(Point from, Point to, long cost, bool directed)
		{
			map[from].Add(new Edge(from, to, cost));
			if (!directed) map[to].Add(new Edge(to, from, cost));
		}

		public WeightedResult Dijkstra(Point startVertex, Point endVertex)
		{
			return ShortestPathCore.Dijkstra(Height, Width, v => this[v], startVertex, endVertex);
		}

		public WeightedResult BfsMod(int m, Point startVertex, Point endVertex)
		{
			return ShortestPathCore.BfsMod(m, Height, Width, v => this[v], startVertex, endVertex);
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
