using System;

namespace AlgorithmLab.Graphs
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
		/// var r = ShortestPath.WithInt(n)
		/// 	.WithWeighted(es, true)
		/// 	.Dijkstra(sv, ev);
		/// </code>
		/// </example>
		public static IntSppFactory WithInt(int vertexesCount) => new IntSppFactory(vertexesCount);

		/// <summary>
		/// 頂点が 2 次元グリッド上の点で表されるグラフを使用します。
		/// </summary>
		/// <param name="height">高さ。</param>
		/// <param name="width">幅。</param>
		/// <returns>2 次元グリッド上の点に対する Factory オブジェクト。</returns>
		/// <example>
		/// 無向グリッド上での典型的な BFS:
		/// <code>
		/// var r = ShortestPath.WithGrid(h, w)
		/// 	.WithUnweighted(v => Array.FindAll(v.Nexts(), nv => s.GetValue(nv) != '#'))
		/// 	.Bfs(sv, ev);
		/// </code>
		/// </example>
		public static GridSppFactory WithGrid(int height, int width) => new GridSppFactory(height, width);

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
		/// var r = ShortestPath.WithHash(h * w, GridHelper.CreateToHash(w), (-1, -1))
		/// 	.WithUnweighted(v => Array.FindAll(v.Nexts(), nv => s.GetValue(nv) != '#'))
		/// 	.Bfs(sv, ev);
		/// </code>
		/// </example>
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
		public UnweightedFuncMapSpp<TVertex> WithUnweighted(Func<TVertex, TVertex[]> getNextVertexes)
		{
			var map = new FuncReadOnlyMap<TVertex, TVertex[]>(getNextVertexes);
			return new UnweightedFuncMapSpp<TVertex>(this, map);
		}

		/// <summary>
		/// 隣接辺を動的に取得するための関数を指定します。
		/// </summary>
		/// <param name="getNextEdges">指定された頂点からの出辺を取得するための関数。</param>
		/// <returns>アルゴリズムを実行するためのオブジェクト。</returns>
		public WeightedFuncMapSpp<TVertex> WithWeighted(Func<TVertex, Edge<TVertex>[]> getNextEdges)
		{
			var map = new FuncReadOnlyMap<TVertex, Edge<TVertex>[]>(getNextEdges);
			return new WeightedFuncMapSpp<TVertex>(this, map);
		}

		public UnweightedListMapSpp<TVertex> WithUnweighted()
		{
			var map = CreateListMap<TVertex>();
			return new UnweightedListMapSpp<TVertex>(this, map);
		}

		public WeightedListMapSpp<TVertex> WithWeighted()
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
		public UnweightedListMapSpp<TVertex> WithUnweighted(Edge<TVertex>[] edges, bool directed)
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
		public WeightedListMapSpp<TVertex> WithWeighted(Edge<TVertex>[] edges, bool directed)
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

		public UnweightedListMapSpp<int> WithUnweighted(int[][] edges, bool directed)
		{
			return WithUnweighted(Array.ConvertAll(edges, EdgeHelper.ToEdge), directed);
		}

		public WeightedListMapSpp<int> WithWeighted(int[][] edges, bool directed)
		{
			return WithWeighted(Array.ConvertAll(edges, EdgeHelper.ToEdge), directed);
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
}
