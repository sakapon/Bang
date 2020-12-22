using System;

namespace AlgorithmLab.Graphs
{
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
}
