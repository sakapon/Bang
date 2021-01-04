using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Spp
{
	/// <summary>
	/// 頂点を表すデータの種類に応じて、<see cref="SppFactory{TVertex}"/> オブジェクトを取得します。
	/// </summary>
	public static class ShortestPath
	{
		/// <summary>
		/// 頂点が整数値で表されるグラフを使用します。<br/>
		/// この値の範囲は [0, <paramref name="vertexesCount"/>) です。
		/// </summary>
		/// <param name="vertexesCount">頂点の個数。</param>
		/// <returns>整数値に対する Factory オブジェクト。</returns>
		/// <example>
		/// 有向グラフ上での典型的な Dijkstra:
		/// <code>
		/// var r = ShortestPath.ForInt(n)
		/// 	.ForWeightedMap(es, true)
		/// 	.Dijkstra(sv, ev);
		/// </code>
		/// </example>
		public static IntSppFactory ForInt(int vertexesCount) => new IntSppFactory(vertexesCount);

		/// <summary>
		/// 頂点が 2 次元グリッド上の点で表されるグラフを使用します。
		/// </summary>
		/// <param name="height">高さ。</param>
		/// <param name="width">幅。</param>
		/// <returns>2 次元グリッド上の点に対する Factory オブジェクト。</returns>
		/// <example>
		/// 無向グリッド上での典型的な BFS:
		/// <code>
		/// var r = ShortestPath.ForGrid(h, w)
		/// 	.ForUnweightedMap(v => Array.FindAll(v.Nexts(), nv => s.GetValue(nv) != '#'))
		/// 	.Bfs(sv, ev);
		/// </code>
		/// </example>
		public static GridSppFactory ForGrid(int height, int width) => new GridSppFactory(height, width);

		/// <summary>
		/// ハッシュ関数により、頂点が任意の値で表されるグラフを使用します。<br/>
		/// ハッシュ値の範囲は [0, <paramref name="vertexesCount"/>) です。
		/// </summary>
		/// <typeparam name="TVertex">頂点を表すオブジェクトの型。</typeparam>
		/// <param name="vertexesCount">頂点の個数。</param>
		/// <param name="toHash">ハッシュ関数。</param>
		/// <param name="invalid">無効な頂点を表す値。</param>
		/// <returns>ハッシュ関数を使用する場合の Factory オブジェクト。</returns>
		/// <example>
		/// 無向グリッド上での典型的な BFS:
		/// <code>
		/// var r = ShortestPath.ForHash(h * w, GridHelper.CreateToHash(w), (-1, -1))
		/// 	.ForUnweightedMap(v => Array.FindAll(v.Nexts(), nv => s.GetValue(nv) != '#'))
		/// 	.Bfs(sv, ev);
		/// </code>
		/// </example>
		public static HashSppFactory<TVertex> ForHash<TVertex>(int vertexesCount, Func<TVertex, int> toHash, TVertex invalid) => new HashSppFactory<TVertex>(vertexesCount, toHash, invalid);
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
		public UnweightedFuncMapSpp<TVertex> ForUnweightedMap(Func<TVertex, TVertex[]> getNextVertexes)
		{
			var map = new FuncReadOnlyMap<TVertex, TVertex[]>(getNextVertexes);
			return new UnweightedFuncMapSpp<TVertex>(this, map);
		}

		/// <summary>
		/// 隣接辺を動的に取得するための関数を指定します。
		/// </summary>
		/// <param name="getNextEdges">指定された頂点からの出辺を取得するための関数。</param>
		/// <returns>アルゴリズムを実行するためのオブジェクト。</returns>
		public WeightedFuncMapSpp<TVertex> ForWeightedMap(Func<TVertex, Edge<TVertex>[]> getNextEdges)
		{
			var map = new FuncReadOnlyMap<TVertex, Edge<TVertex>[]>(getNextEdges);
			return new WeightedFuncMapSpp<TVertex>(this, map);
		}

		public UnweightedListMapSpp<TVertex> ForUnweightedMap()
		{
			var map = CreateListMap<TVertex>();
			return new UnweightedListMapSpp<TVertex>(this, map);
		}

		public WeightedListMapSpp<TVertex> ForWeightedMap()
		{
			var map = CreateListMap<Edge<TVertex>>();
			return new WeightedListMapSpp<TVertex>(this, map);
		}

		/// <summary>
		/// 重みなし辺のリストを静的に指定します。
		/// </summary>
		/// <param name="edges">辺のリスト。</param>
		/// <param name="directed">有向グラフかどうかを示す値。</param>
		/// <returns>アルゴリズムを実行するためのオブジェクト。</returns>
		public UnweightedListMapSpp<TVertex> ForUnweightedMap(Edge<TVertex>[] edges, bool directed)
		{
			var map = CreateListMap<TVertex>();
			GraphConvert.UnweightedEdgesToMap(map, edges, directed);
			return new UnweightedListMapSpp<TVertex>(this, map);
		}

		/// <summary>
		/// 重み付き辺のリストを静的に指定します。
		/// </summary>
		/// <param name="edges">辺のリスト。</param>
		/// <param name="directed">有向グラフかどうかを示す値。</param>
		/// <returns>アルゴリズムを実行するためのオブジェクト。</returns>
		public WeightedListMapSpp<TVertex> ForWeightedMap(Edge<TVertex>[] edges, bool directed)
		{
			var map = CreateListMap<Edge<TVertex>>();
			GraphConvert.WeightedEdgesToMap(map, edges, directed);
			return new WeightedListMapSpp<TVertex>(this, map);
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

		public UnweightedListMapSpp<int> ForUnweightedMap(int[][] edges, bool directed)
		{
			return ForUnweightedMap(Array.ConvertAll(edges, EdgeHelper.ToEdge), directed);
		}

		public WeightedListMapSpp<int> ForWeightedMap(int[][] edges, bool directed)
		{
			return ForWeightedMap(Array.ConvertAll(edges, EdgeHelper.ToEdge), directed);
		}
	}

	public class GridSppFactory : SppFactory<Point>
	{
		public int Height { get; }
		public int Width { get; }
		public GridSppFactory(int height, int width)
		{
			Height = height;
			Width = width;
		}

		public override Point Invalid => (-1, -1);

		public override Map<Point, TValue> CreateMap<TValue>(TValue iv)
		{
			return new GridMap<TValue>(Height, Width, iv);
		}

		public override ListMap<Point, TValue> CreateListMap<TValue>()
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

	public abstract class UnweightedMapSpp<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		public SppFactory<TVertex> Factory { get; }
		public abstract ReadOnlyMap<TVertex, TVertex[]> NextVertexesMap { get; }

		protected UnweightedMapSpp(SppFactory<TVertex> factory)
		{
			Factory = factory;
		}

		public TVertex StartVertex { get; private set; }
		public TVertex EndVertex { get; private set; }
		public Map<TVertex, long> Costs { get; private set; }
		public Map<TVertex, TVertex> InVertexes { get; private set; }
		public long this[TVertex vertex] => Costs[vertex];
		public bool IsConnected(TVertex vertex) => Costs[vertex] != long.MaxValue;

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
		public UnweightedMapSpp<TVertex> Bfs(TVertex startVertex, TVertex endVertex)
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

		public TVertex[] GetPathVertexes(TVertex endVertex)
		{
			if (InVertexes == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<TVertex>();
			for (var v = endVertex; !TEquals(v, Factory.Invalid); v = InVertexes[v])
				path.Push(v);
			return path.ToArray();
		}
	}

	public class UnweightedFuncMapSpp<TVertex> : UnweightedMapSpp<TVertex>
	{
		public override ReadOnlyMap<TVertex, TVertex[]> NextVertexesMap { get; }

		public UnweightedFuncMapSpp(SppFactory<TVertex> factory, FuncReadOnlyMap<TVertex, TVertex[]> nextVertexesMap) : base(factory)
		{
			NextVertexesMap = nextVertexesMap;
		}
	}

	public class UnweightedListMapSpp<TVertex> : UnweightedMapSpp<TVertex>
	{
		ListMap<TVertex, TVertex> map;
		public override ReadOnlyMap<TVertex, TVertex[]> NextVertexesMap => map;

		public UnweightedListMapSpp(SppFactory<TVertex> factory, ListMap<TVertex, TVertex> nextVertexesMap) : base(factory)
		{
			map = nextVertexesMap;
		}

		public void AddEdge(Edge<TVertex> edge, bool directed)
		{
			map.Add(edge.From, edge.To);
			if (!directed) map.Add(edge.To, edge.From);
		}

		public void AddEdge(TVertex from, TVertex to, bool directed)
		{
			map.Add(from, to);
			if (!directed) map.Add(to, from);
		}
	}

	public abstract class WeightedMapSpp<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		public SppFactory<TVertex> Factory { get; }
		public abstract ReadOnlyMap<TVertex, Edge<TVertex>[]> NextEdgesMap { get; }

		protected WeightedMapSpp(SppFactory<TVertex> factory)
		{
			Factory = factory;
		}

		public TVertex StartVertex { get; private set; }
		public TVertex EndVertex { get; private set; }
		public Map<TVertex, long> Costs { get; private set; }
		public Map<TVertex, Edge<TVertex>> InEdges { get; private set; }
		public long this[TVertex vertex] => Costs[vertex];
		public bool IsConnected(TVertex vertex) => Costs[vertex] != long.MaxValue;

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
		public WeightedMapSpp<TVertex> Dijkstra(TVertex startVertex, TVertex endVertex)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;

			Costs = Factory.CreateMap(long.MaxValue);
			InEdges = Factory.CreateMap(new Edge<TVertex>(Factory.Invalid, Factory.Invalid, long.MinValue));
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

		/// <summary>
		/// 幅優先探索の拡張により、始点から各頂点への最短経路を求めます。<br/>
		/// 例えば <paramref name="m"/> = 3 のとき、012-BFS を表します。<br/>
		/// 辺のコストの範囲は [0, <paramref name="m"/>) です。
		/// </summary>
		/// <param name="m">辺のコストの候補となる数。</param>
		/// <param name="startVertex">始点。</param>
		/// <param name="endVertex">終点。終点を指定しない場合、<c>Factory.Invalid</c>。</param>
		/// <returns>現在のオブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。
		/// </remarks>
		public WeightedMapSpp<TVertex> BfsMod(int m, TVertex startVertex, TVertex endVertex)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;

			Costs = Factory.CreateMap(long.MaxValue);
			InEdges = Factory.CreateMap(new Edge<TVertex>(Factory.Invalid, Factory.Invalid, long.MinValue));
			var qs = Array.ConvertAll(new bool[m], _ => new Queue<TVertex>());
			Costs[startVertex] = 0;
			qs[0].Enqueue(startVertex);

			for (long c = 0; Array.Exists(qs, q => q.Count > 0); ++c)
			{
				var q = qs[c % m];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					if (TEquals(v, endVertex)) return this;
					if (Costs[v] < c) continue;

					foreach (var e in NextEdgesMap[v])
					{
						var (nv, nc) = (e.To, c + e.Cost);
						if (Costs[nv] <= nc) continue;
						Costs[nv] = nc;
						InEdges[nv] = e;
						qs[nc % m].Enqueue(nv);
					}
				}
			}
			return this;
		}

		public TVertex[] GetPathVertexes(TVertex endVertex)
		{
			if (InEdges == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<TVertex>();
			for (var v = endVertex; !TEquals(v, Factory.Invalid); v = InEdges[v].From)
				path.Push(v);
			return path.ToArray();
		}

		public Edge<TVertex>[] GetPathEdges(TVertex endVertex)
		{
			if (InEdges == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<Edge<TVertex>>();
			for (var e = InEdges[endVertex]; !TEquals(e.From, Factory.Invalid); e = InEdges[e.From])
				path.Push(e);
			return path.ToArray();
		}
	}

	public class WeightedFuncMapSpp<TVertex> : WeightedMapSpp<TVertex>
	{
		public override ReadOnlyMap<TVertex, Edge<TVertex>[]> NextEdgesMap { get; }

		public WeightedFuncMapSpp(SppFactory<TVertex> factory, FuncReadOnlyMap<TVertex, Edge<TVertex>[]> nextEdgesMap) : base(factory)
		{
			NextEdgesMap = nextEdgesMap;
		}
	}

	public class WeightedListMapSpp<TVertex> : WeightedMapSpp<TVertex>
	{
		ListMap<TVertex, Edge<TVertex>> map;
		public override ReadOnlyMap<TVertex, Edge<TVertex>[]> NextEdgesMap => map;

		public WeightedListMapSpp(SppFactory<TVertex> factory, ListMap<TVertex, Edge<TVertex>> nextEdgesMap) : base(factory)
		{
			map = nextEdgesMap;
		}

		public void AddEdge(Edge<TVertex> edge, bool directed)
		{
			map.Add(edge.From, edge);
			if (!directed) map.Add(edge.To, edge.Reverse());
		}

		public void AddEdge(TVertex from, TVertex to, long cost, bool directed)
		{
			map.Add(from, new Edge<TVertex>(from, to, cost));
			if (!directed) map.Add(to, new Edge<TVertex>(to, from, cost));
		}
	}

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

	public struct Edge<T>
	{
		public T From { get; }
		public T To { get; }
		public long Cost { get; }
		public Edge(T from, T to, long cost = 1) { From = from; To = to; Cost = cost; }
		public override string ToString() => $"{{{From}}} {{{To}}} {Cost}";
		public static implicit operator Edge<T>((T from, T to) v) => new Edge<T>(v.from, v.to);
		public static implicit operator Edge<T>((T from, T to, long cost) v) => new Edge<T>(v.from, v.to, v.cost);
		public Edge<T> Reverse() => new Edge<T>(To, From, Cost);
	}

	public static class EdgeHelper
	{
		public static Edge<int> ToEdge(int[] e) => new Edge<int>(e[0], e[1], e.Length > 2 ? e[2] : 1);
		public static Edge<int> ToEdge(long[] e) => new Edge<int>((int)e[0], (int)e[1], e.Length > 2 ? e[2] : 1);
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

	public class GridMap<TValue> : Map<Point, TValue>
	{
		TValue[][] a;
		public GridMap(int height, int width, TValue iv)
		{
			a = Array.ConvertAll(new bool[height], _ => Array.ConvertAll(new bool[width], __ => iv));
		}
		public override TValue this[Point key] { get => a[key.i][key.j]; set => a[key.i][key.j] = value; }
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

	public class GridListMap<TValue> : ListMap<Point, TValue>
	{
		List<TValue>[][] map;
		public GridListMap(int height, int width)
		{
			map = Array.ConvertAll(new bool[height], _ => Array.ConvertAll(new bool[width], __ => new List<TValue>()));
		}
		public override TValue[] this[Point key] => map[key.i][key.j].ToArray();
		public override void Add(Point key, TValue value) => map[key.i][key.j].Add(value);
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
		const char Road = '.';
		const char Wall = '#';

		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

		public static Point ReadPoint()
		{
			return Point.Parse(Console.ReadLine());
		}

		public static Edge<int>[] ReadEdges(int count)
		{
			return Array.ConvertAll(new bool[count], _ => EdgeHelper.ToEdge(Read()));
		}

		public static string[] ReadGrid(int height)
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

		public static string[] ReadEnclosedGrid(ref int height, ref int width, char c = '#', int delta = 1)
		{
			var h = height + 2 * delta;
			var w = width + 2 * delta;
			var cw = new string(c, w);
			var cd = new string(c, delta);

			var s = new string[h];
			for (int i = 0; i < delta; ++i) s[delta + height + i] = s[i] = cw;
			for (int i = 0; i < height; ++i) s[delta + i] = cd + Console.ReadLine() + cd;
			(height, width) = (h, w);
			return s;
		}
	}

	public static class GraphConvert
	{
		public static void UnweightedEdgesToMap<TVertex>(ListMap<TVertex, TVertex> map, Edge<TVertex>[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map.Add(e.From, e.To);
				if (!directed) map.Add(e.To, e.From);
			}
		}

		public static void WeightedEdgesToMap<TVertex>(ListMap<TVertex, Edge<TVertex>> map, Edge<TVertex>[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				map.Add(e.From, e);
				if (!directed) map.Add(e.To, e.Reverse());
			}
		}
	}

	public static class GridHelper
	{
		public static T GetValue<T>(this T[,] a, Point p) => a[p.i, p.j];
		public static void SetValue<T>(this T[,] a, Point p, T value) => a[p.i, p.j] = value;
		public static T GetValue<T>(this T[][] a, Point p) => a[p.i][p.j];
		public static void SetValue<T>(this T[][] a, Point p, T value) => a[p.i][p.j] = value;
		public static char GetValue(this string[] s, Point p) => s[p.i][p.j];

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

		public static Point FindValue<T>(T[][] a, T value)
		{
			var ec = EqualityComparer<T>.Default;
			var (h, w) = (a.Length, a[0].Length);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (ec.Equals(a[i][j], value)) return new Point(i, j);
			return new Point(-1, -1);
		}

		public static Point FindValue(string[] s, char c)
		{
			var (h, w) = (s.Length, s[0].Length);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (s[i][j] == c) return new Point(i, j);
			return new Point(-1, -1);
		}

		public static int ToHash(Point p, int width) => p.i * width + p.j;
		public static Point FromHash(int hash, int width) => new Point(hash / width, hash % width);
		public static Func<Point, int> CreateToHash(int width) => p => p.i * width + p.j;
		public static Func<int, Point> CreateFromHash(int width) => hash => new Point(hash / width, hash % width);

		// 負値を指定できます。
		public static void Enclose<T>(ref int height, ref int width, ref T[][] a, T value, int delta = 1)
		{
			var (h, w) = (height + 2 * delta, width + 2 * delta);
			var (li, ri) = (Math.Max(0, -delta), Math.Min(height, height + delta));
			var (lj, rj) = (Math.Max(0, -delta), Math.Min(width, width + delta));

			var t = Array.ConvertAll(new bool[h], _ => Array.ConvertAll(new bool[w], __ => value));
			for (int i = li; i < ri; ++i)
				for (int j = lj; j < rj; ++j)
					t[delta + i][delta + j] = a[i][j];
			(height, width, a) = (h, w, t);
		}

		// 負値を指定できます。
		public static void Enclose(ref int height, ref int width, ref string[] s, char c = '#', int delta = 1)
		{
			var (h, w) = (height + 2 * delta, width + 2 * delta);
			var (li, ri) = (Math.Max(0, -delta), Math.Min(height, height + delta));
			var cw = new string(c, w);
			var cd = new string(c, Math.Max(0, delta));

			var t = new string[h];
			for (int i = 0; i < delta; ++i) t[delta + height + i] = t[i] = cw;
			for (int i = li; i < ri; ++i) t[delta + i] = delta >= 0 ? cd + s[i] + cd : s[i][-delta..(width + delta)];
			(height, width, s) = (h, w, t);
		}

		public static T[][] Rotate180<T>(T[][] a)
		{
			var (h, w) = (a.Length, a[0].Length);
			var r = Array.ConvertAll(new bool[h], _ => new T[w]);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					r[i][j] = a[h - 1 - i][w - 1 - j];
			return r;
		}

		public static T[][] RotateLeft<T>(T[][] a)
		{
			var (h, w) = (a.Length, a[0].Length);
			var r = Array.ConvertAll(new bool[w], _ => new T[h]);
			for (int i = 0; i < w; ++i)
				for (int j = 0; j < h; ++j)
					r[i][j] = a[j][w - 1 - i];
			return r;
		}

		public static T[][] RotateRight<T>(T[][] a)
		{
			var (h, w) = (a.Length, a[0].Length);
			var r = Array.ConvertAll(new bool[w], _ => new T[h]);
			for (int i = 0; i < w; ++i)
				for (int j = 0; j < h; ++j)
					r[i][j] = a[h - 1 - j][i];
			return r;
		}

		public static string[] Rotate180(string[] s)
		{
			var h = s.Length;
			var r = new string[h];
			for (int i = 0; i < h; ++i)
			{
				var cs = s[h - 1 - i].ToCharArray();
				Array.Reverse(cs);
				r[i] = new string(cs);
			}
			return r;
		}

		public static string[] RotateLeft(string[] s)
		{
			var (h, w) = (s.Length, s[0].Length);
			var r = new string[w];
			for (int i = 0; i < w; ++i)
			{
				var cs = new char[h];
				for (int j = 0; j < h; ++j)
					cs[j] = s[j][w - 1 - i];
				r[i] = new string(cs);
			}
			return r;
		}

		public static string[] RotateRight(string[] s)
		{
			var (h, w) = (s.Length, s[0].Length);
			var r = new string[w];
			for (int i = 0; i < w; ++i)
			{
				var cs = new char[h];
				for (int j = 0; j < h; ++j)
					cs[j] = s[h - 1 - j][i];
				r[i] = new string(cs);
			}
			return r;
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
