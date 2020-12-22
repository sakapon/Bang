using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs0.Spp2
{
	public struct UnweightedEdge<T>
	{
		public T From { get; }
		public T To { get; }
		public UnweightedEdge(T from, T to) { From = from; To = to; }
		public UnweightedEdge<T> Reverse() => new UnweightedEdge<T>(To, From);
	}

	public struct WeightedEdge<T>
	{
		public T From { get; }
		public T To { get; }
		public long Cost { get; }
		public WeightedEdge(T from, T to, long cost) { From = from; To = to; Cost = cost; }
		public WeightedEdge<T> Reverse() => new WeightedEdge<T>(To, From, Cost);
	}

	public static class EdgeHelper
	{
		public static UnweightedEdge<int> Unweighted(int[] e) => new UnweightedEdge<int>(e[0], e[1]);
		public static UnweightedEdge<int> Unweighted(long[] e) => new UnweightedEdge<int>((int)e[0], (int)e[1]);
		public static WeightedEdge<int> Weighted(int[] e) => new WeightedEdge<int>(e[0], e[1], e.Length > 2 ? e[2] : 0);
		public static WeightedEdge<int> Weighted(long[] e) => new WeightedEdge<int>((int)e[0], (int)e[1], e.Length > 2 ? e[2] : 0);
	}

	public abstract class Map<TKey, TValue>
	{
		public abstract TValue this[TKey key] { get; set; }
	}

	public abstract class ReadOnlyMap<TKey, TValue>
	{
		public abstract TValue this[TKey key] { get; }
	}

	public abstract class ListMap<TKey, TValue> : ReadOnlyMap<TKey, TValue[]>
	{
		public abstract void Add(TKey key, TValue value);
	}

	public class IntMap<TValue> : Map<int, TValue>
	{
		TValue[] a;
		public IntMap(int count, TValue iv)
		{
			a = Array.ConvertAll(new bool[count], _ => iv);
		}
		public override TValue this[int key] { get => a[key]; set => a[key] = value; }
	}

	public class GridMap<TValue> : Map<(int i, int j), TValue>
	{
		TValue[][] a;
		public GridMap(int height, int width, TValue iv)
		{
			a = Array.ConvertAll(new bool[height], _ => Array.ConvertAll(new bool[width], __ => iv));
		}
		public override TValue this[(int i, int j) key] { get => a[key.i][key.j]; set => a[key.i][key.j] = value; }
	}

	public class HashMap<TKey, TValue> : Map<TKey, TValue>
	{
		TValue[] a;
		Func<TKey, int> ToHash;
		public HashMap(int count, TValue iv, Func<TKey, int> toHash)
		{
			a = Array.ConvertAll(new bool[count], _ => iv);
			ToHash = toHash;
		}
		public override TValue this[TKey key] { get => a[ToHash(key)]; set => a[ToHash(key)] = value; }
	}

	public class FuncReadOnlyMap<TKey, TValue> : ReadOnlyMap<TKey, TValue>
	{
		Func<TKey, TValue> GetValue;
		public FuncReadOnlyMap(Func<TKey, TValue> getValue)
		{
			GetValue = getValue;
		}
		public override TValue this[TKey key] => GetValue(key);
	}

	public class IntListMap<TValue> : ListMap<int, TValue>
	{
		List<TValue>[] map;
		public IntListMap(int count)
		{
			map = Array.ConvertAll(new bool[count], _ => new List<TValue>());
		}
		public override TValue[] this[int key] => map[key].ToArray();
		public override void Add(int key, TValue value) => map[key].Add(value);
	}

	public class GridListMap<TValue> : ListMap<(int i, int j), TValue>
	{
		List<TValue>[][] map;
		public GridListMap(int height, int width)
		{
			map = Array.ConvertAll(new bool[height], _ => Array.ConvertAll(new bool[width], __ => new List<TValue>()));
		}
		public override TValue[] this[(int i, int j) key] => map[key.i][key.j].ToArray();
		public override void Add((int i, int j) key, TValue value) => map[key.i][key.j].Add(value);
	}

	public class HashListMap<TKey, TValue> : ListMap<TKey, TValue>
	{
		List<TValue>[] map;
		Func<TKey, int> ToHash;
		public HashListMap(int count, Func<TKey, int> toHash)
		{
			map = Array.ConvertAll(new bool[count], _ => new List<TValue>());
			ToHash = toHash;
		}
		public override TValue[] this[TKey key] => map[ToHash(key)].ToArray();
		public override void Add(TKey key, TValue value) => map[ToHash(key)].Add(value);
	}

	public static class GraphConsole
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

		public static UnweightedEdge<int>[] ReadUnweightedEdges(int count)
		{
			return Array.ConvertAll(new bool[count], _ => EdgeHelper.Unweighted(Read()));
		}

		public static WeightedEdge<int>[] ReadWeightedEdges(int count)
		{
			return Array.ConvertAll(new bool[count], _ => EdgeHelper.Weighted(Read()));
		}

		public static string[] ReadGrid(int h)
		{
			return Array.ConvertAll(new bool[h], _ => Console.ReadLine());
		}

		public static char[][] ReadGridAsChar(int h)
		{
			return Array.ConvertAll(new bool[h], _ => Console.ReadLine().ToCharArray());
		}

		public static int[][] ReadGridAsInt(int h)
		{
			return Array.ConvertAll(new bool[h], _ => Read());
		}

		public static string[] ReadEnclosedGrid(ref int h, ref int w, char c = '#')
		{
			var s = new string[h + 2];
			s[h + 1] = s[0] = new string(c, w += 2);
			for (int i = 1; i <= h; ++i) s[i] = c + Console.ReadLine() + c;
			h += 2;
			return s;
		}
	}

	public static class GridHelper
	{
		const char Road = '.';
		const char Wall = '#';

		// 2 次元配列に 2 次元インデックスでアクセスします。
		public static T GetByP<T>(this T[][] a, (int i, int j) p) => a[p.i][p.j];
		public static void SetByP<T>(this T[][] a, (int i, int j) p, T value) => a[p.i][p.j] = value;
		public static char GetByP(this string[] s, (int i, int j) p) => s[p.i][p.j];

		public static void EncloseGrid(ref int h, ref int w, ref string[] s, char c = Wall)
		{
			var t = new string[h + 2];
			t[h + 1] = t[0] = new string(c, w += 2);
			for (int i = 1; i <= h; ++i) t[i] = c + s[i - 1] + c;
			h += 2;
			s = t;
		}

		public static (int i, int j) FindChar(string[] s, char c)
		{
			var (h, w) = (s.Length, s[0].Length);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (s[i][j] == c) return (i, j);
			return (-1, -1);
		}

		public static int ToHash((int i, int j) p, int w) => p.i * w + p.j;
		public static (int i, int j) FromHash(int id, int w) => (id / w, id % w);

		public static readonly (int i, int j)[] NextsByDelta = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
		public static (int i, int j)[] Nexts((int i, int j) v)
		{
			var (i, j) = v;
			return new[] { (i - 1, j), (i + 1, j), (i, j - 1), (i, j + 1) };
		}
	}

	/// <summary>
	/// 頂点を表すデータの種類に応じて、<see cref="SppFactory{TVertex}"/> オブジェクトを取得します。
	/// </summary>
	public static class ShortestPath
	{
		/// <summary>
		/// 整数値が頂点を表すグラフを使用します。<br/>
		/// この値は <c>0 &lt;= v &lt; <paramref name="vertexesCount"/></c> に制限されます。
		/// </summary>
		/// <param name="vertexesCount">頂点の個数。</param>
		/// <returns>整数値に対する Factory オブジェクト。</returns>
		/// <example>
		/// 有向グラフ上での典型的な Dijkstra:
		/// <code>
		/// var r = ShortestPath.WithInt(n)
		/// 	.WithWeighted(es, true)
		/// 	.Dijkstra(sv, ev);
		/// </code>
		/// </example>
		public static IntSppFactory WithInt(int vertexesCount) => new IntSppFactory(vertexesCount);

		/// <summary>
		/// 2 次元グリッド上の点が頂点を表すグラフを使用します。
		/// </summary>
		/// <param name="height">高さ。</param>
		/// <param name="width">幅。</param>
		/// <returns>2 次元グリッド上の点に対する Factory オブジェクト。</returns>
		/// <example>
		/// 無向グリッド上での典型的な BFS:
		/// <code>
		/// var r = ShortestPath.WithGrid(h, w)
		/// 	.WithUnweighted(v => Array.FindAll(GridHelper.Nexts(v), v => s.GetByP(v) != '#'))
		/// 	.Bfs(sv, ev);
		/// </code>
		/// </example>
		public static GridSppFactory WithGrid(int height, int width) => new GridSppFactory(height, width);
		public static HashSppFactory<TVertex> WithHash<TVertex>(int vertexesCount, Func<TVertex, int> toHash, TVertex invalid) => new HashSppFactory<TVertex>(vertexesCount, toHash, invalid);
	}

	/// <summary>
	/// 実装による内部データ構造の違いを吸収します。
	/// </summary>
	/// <typeparam name="TVertex">頂点を表すオブジェクトの型。</typeparam>
	public abstract class SppFactory<TVertex>
	{
		public abstract TVertex Invalid { get; }
		public abstract Map<TVertex, TValue> CreateMap<TValue>(TValue iv);
		public abstract ListMap<TVertex, TValue> CreateListMap<TValue>();

		/// <summary>
		/// 隣接頂点を動的に取得するための関数を指定します。
		/// </summary>
		/// <param name="getNextVertexes">指定された頂点からの行先となる頂点を取得するための関数。</param>
		/// <returns>アルゴリズムを実行するためのオブジェクト。</returns>
		public UnweightedSppContext<TVertex> WithUnweighted(Func<TVertex, TVertex[]> getNextVertexes)
		{
			var map = new FuncReadOnlyMap<TVertex, TVertex[]>(getNextVertexes);
			return new UnweightedSppContext<TVertex>(this, map);
		}

		/// <summary>
		/// 隣接辺を動的に取得するための関数を指定します。
		/// </summary>
		/// <param name="getNextEdges">指定された頂点からの出辺を取得するための関数。</param>
		/// <returns>アルゴリズムを実行するためのオブジェクト。</returns>
		public WeightedSppContext<TVertex> WithWeighted(Func<TVertex, WeightedEdge<TVertex>[]> getNextEdges)
		{
			var map = new FuncReadOnlyMap<TVertex, WeightedEdge<TVertex>[]>(getNextEdges);
			return new WeightedSppContext<TVertex>(this, map);
		}

		/// <summary>
		/// 重みなし辺のリストを静的に指定します。
		/// </summary>
		/// <param name="edges">辺のリスト。</param>
		/// <param name="directed">有向グラフかどうかを示す値。</param>
		/// <returns>アルゴリズムを実行するためのオブジェクト。</returns>
		public UnweightedSppContext<TVertex> WithUnweighted(UnweightedEdge<TVertex>[] edges, bool directed)
		{
			var map = UnweightedEdgesToMap(edges, directed);
			return new UnweightedSppContext<TVertex>(this, map);
		}

		/// <summary>
		/// 重み付き辺のリストを静的に指定します。
		/// </summary>
		/// <param name="edges">辺のリスト。</param>
		/// <param name="directed">有向グラフかどうかを示す値。</param>
		/// <returns>アルゴリズムを実行するためのオブジェクト。</returns>
		public WeightedSppContext<TVertex> WithWeighted(WeightedEdge<TVertex>[] edges, bool directed)
		{
			var map = WeightedEdgesToMap(edges, directed);
			return new WeightedSppContext<TVertex>(this, map);
		}

		public ListMap<TVertex, TVertex> UnweightedEdgesToMap(UnweightedEdge<TVertex>[] edges, bool directed)
		{
			var map = CreateListMap<TVertex>();
			foreach (var e in edges)
			{
				map.Add(e.From, e.To);
				if (!directed) map.Add(e.To, e.From);
			}
			return map;
		}

		public ListMap<TVertex, WeightedEdge<TVertex>> WeightedEdgesToMap(WeightedEdge<TVertex>[] edges, bool directed)
		{
			var map = CreateListMap<WeightedEdge<TVertex>>();
			foreach (var e in edges)
			{
				map.Add(e.From, e);
				if (!directed) map.Add(e.To, e.Reverse());
			}
			return map;
		}
	}

	public class IntSppFactory : SppFactory<int>
	{
		public int VertexesCount { get; }
		public IntSppFactory(int vertexesCount)
		{
			VertexesCount = vertexesCount;
		}

		public override int Invalid => -1;

		public override Map<int, TValue> CreateMap<TValue>(TValue iv)
		{
			return new IntMap<TValue>(VertexesCount, iv);
		}

		public override ListMap<int, TValue> CreateListMap<TValue>()
		{
			return new IntListMap<TValue>(VertexesCount);
		}

		public UnweightedSppContext<int> WithUnweighted(int[][] edges, bool directed)
		{
			return WithUnweighted(Array.ConvertAll(edges, EdgeHelper.Unweighted), directed);
		}

		public WeightedSppContext<int> WithWeighted(int[][] edges, bool directed)
		{
			return WithWeighted(Array.ConvertAll(edges, EdgeHelper.Weighted), directed);
		}
	}

	public class GridSppFactory : SppFactory<(int i, int j)>
	{
		public int Height { get; }
		public int Width { get; }
		public GridSppFactory(int height, int width)
		{
			Height = height;
			Width = width;
		}

		public override (int i, int j) Invalid => (-1, -1);

		public override Map<(int i, int j), TValue> CreateMap<TValue>(TValue iv)
		{
			return new GridMap<TValue>(Height, Width, iv);
		}

		public override ListMap<(int i, int j), TValue> CreateListMap<TValue>()
		{
			return new GridListMap<TValue>(Height, Width);
		}
	}

	public class HashSppFactory<TVertex> : SppFactory<TVertex>
	{
		public int VertexesCount { get; }
		public Func<TVertex, int> ToHash { get; }
		public HashSppFactory(int vertexesCount, Func<TVertex, int> toHash, TVertex invalid)
		{
			VertexesCount = vertexesCount;
			ToHash = toHash;
			Invalid = invalid;
		}

		public override TVertex Invalid { get; }

		public override Map<TVertex, TValue> CreateMap<TValue>(TValue iv)
		{
			return new HashMap<TVertex, TValue>(VertexesCount, iv, ToHash);
		}

		public override ListMap<TVertex, TValue> CreateListMap<TValue>()
		{
			return new HashListMap<TVertex, TValue>(VertexesCount, ToHash);
		}
	}

	public class UnweightedSppContext<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		public SppFactory<TVertex> Factory { get; }
		public ReadOnlyMap<TVertex, TVertex[]> NextVertexesMap { get; }

		public UnweightedSppContext(SppFactory<TVertex> factory, ReadOnlyMap<TVertex, TVertex[]> nextVertexesMap)
		{
			Factory = factory;
			NextVertexesMap = nextVertexesMap;
		}

		public Map<TVertex, long> Costs { get; private set; }
		public Map<TVertex, TVertex> InVertexes { get; private set; }
		public TVertex StartVertex { get; private set; }
		public TVertex EndVertex { get; private set; }

		/// <summary>
		/// 幅優先探索により、始点から各頂点への最短経路を求めます。<br/>
		/// 辺のコストはすべて 1 として扱われます。
		/// </summary>
		/// <param name="startVertex">始点。</param>
		/// <param name="endVertex">終点。終点を指定しない場合、<c>Factory.Invalid</c>。</param>
		/// <returns>現在のオブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。
		/// </remarks>
		public UnweightedSppContext<TVertex> Bfs(TVertex startVertex, TVertex endVertex)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;

			Costs = Factory.CreateMap(long.MaxValue);
			InVertexes = Factory.CreateMap(Factory.Invalid);
			var q = new Queue<TVertex>();
			Costs[startVertex] = 0;
			q.Enqueue(startVertex);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				var nc = Costs[v] + 1;

				foreach (var nv in NextVertexesMap[v])
				{
					if (Costs[nv] <= nc) continue;
					Costs[nv] = nc;
					InVertexes[nv] = v;
					if (TEquals(nv, endVertex)) return this;
					q.Enqueue(nv);
				}
			}
			return this;
		}

		public long this[TVertex vertex] => Costs[vertex];
		public bool IsConnected(TVertex vertex) => Costs[vertex] != long.MaxValue;

		public TVertex[] GetPathVertexes(TVertex endVertex)
		{
			if (InVertexes == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<TVertex>();
			for (var v = endVertex; !TEquals(v, Factory.Invalid); v = InVertexes[v])
				path.Push(v);
			return path.ToArray();
		}
	}

	public class WeightedSppContext<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		public SppFactory<TVertex> Factory { get; }
		public ReadOnlyMap<TVertex, WeightedEdge<TVertex>[]> NextEdgesMap { get; }

		public WeightedSppContext(SppFactory<TVertex> factory, ReadOnlyMap<TVertex, WeightedEdge<TVertex>[]> nextEdgesMap)
		{
			Factory = factory;
			NextEdgesMap = nextEdgesMap;
		}

		public Map<TVertex, long> Costs { get; private set; }
		public Map<TVertex, WeightedEdge<TVertex>> InEdges { get; private set; }
		public TVertex StartVertex { get; private set; }
		public TVertex EndVertex { get; private set; }

		/// <summary>
		/// Dijkstra 法により、始点から各頂点への最短経路を求めます。<br/>
		/// 辺のコストは非負でなければなりません。
		/// </summary>
		/// <param name="startVertex">始点。</param>
		/// <param name="endVertex">終点。終点を指定しない場合、<c>Factory.Invalid</c>。</param>
		/// <returns>現在のオブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。
		/// </remarks>
		public WeightedSppContext<TVertex> Dijkstra(TVertex startVertex, TVertex endVertex)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;

			Costs = Factory.CreateMap(long.MaxValue);
			InEdges = Factory.CreateMap(new WeightedEdge<TVertex>(Factory.Invalid, Factory.Invalid, -1));
			var q = PriorityQueue<TVertex>.CreateWithKey(v => Costs[v]);
			Costs[startVertex] = 0;
			q.Push(startVertex);

			while (q.Any)
			{
				var (v, c) = q.Pop();
				if (TEquals(v, endVertex)) break;
				if (Costs[v] < c) continue;

				// IEnumerable<T>, List<T>, T[] の順に高速になります。
				foreach (var e in NextEdgesMap[v])
				{
					var (nv, nc) = (e.To, c + e.Cost);
					if (Costs[nv] <= nc) continue;
					Costs[nv] = nc;
					InEdges[nv] = e;
					q.Push(nv);
				}
			}
			return this;
		}

		public long this[TVertex vertex] => Costs[vertex];
		public bool IsConnected(TVertex vertex) => Costs[vertex] != long.MaxValue;

		public TVertex[] GetPathVertexes(TVertex endVertex)
		{
			if (InEdges == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<TVertex>();
			for (var v = endVertex; !TEquals(v, Factory.Invalid); v = InEdges[v].From)
				path.Push(v);
			return path.ToArray();
		}

		public WeightedEdge<TVertex>[] GetPathEdges(TVertex endVertex)
		{
			if (InEdges == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<WeightedEdge<TVertex>>();
			for (var e = InEdges[endVertex]; !TEquals(e.From, Factory.Invalid); e = InEdges[e.From])
				path.Push(e);
			return path.ToArray();
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
