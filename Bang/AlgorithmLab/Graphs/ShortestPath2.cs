using System;

namespace AlgorithmLab.Graphs
{
	public static class ShortestPath2
	{

	}

	/// <summary>
	/// 実装による内部データ構造の違いを吸収します。
	/// </summary>
	/// <typeparam name="TVertex">頂点を表すオブジェクトの型。</typeparam>
	public abstract class SppFactory<TVertex>
	{
		public abstract TVertex Invalid { get; }
		public abstract Map<TVertex, TValue> CreateMap<TValue>(TValue v0);
		public abstract ListMap<TVertex, TValue> CreateListMap<TValue>();

		public UnweightedSppContext<TVertex> CreateUnweighted(Func<TVertex, TVertex[]> getNextVertexes)
		{
			var map = new FuncReadOnlyMap<TVertex, TVertex[]>(getNextVertexes);
			return new UnweightedSppContext<TVertex>(this, map);
		}

		public UnweightedSppContext<TVertex> CreateUnweighted(UnweightedEdge<TVertex>[] edges, bool directed)
		{
			var map = UnweightedEdgesToMap(edges, directed);
			return new UnweightedSppContext<TVertex>(this, map);
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

		public override Map<int, TValue> CreateMap<TValue>(TValue v0)
		{
			return new IntMap<TValue>(VertexesCount, v0);
		}

		public override ListMap<int, TValue> CreateListMap<TValue>()
		{
			return new IntListMap<TValue>(VertexesCount);
		}

		public UnweightedSppContext<int> CreateUnweighted(int[][] edges, bool directed)
		{
			return CreateUnweighted(Array.ConvertAll(edges, EdgeHelper.Unweighted), directed);
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

		public override Map<(int i, int j), TValue> CreateMap<TValue>(TValue v0)
		{
			return new GridMap<TValue>(Height, Width, v0);
		}

		public override ListMap<(int i, int j), TValue> CreateListMap<TValue>()
		{
			return new GridListMap<TValue>(Height, Width);
		}
	}

	public class MappingSppFactory<TVertex> : SppFactory<TVertex>
	{
		public int VertexesCount { get; }
		public Func<TVertex, int> ToId { get; }
		public MappingSppFactory(int vertexesCount, Func<TVertex, int> toId, TVertex invalid)
		{
			VertexesCount = vertexesCount;
			ToId = toId;
			Invalid = invalid;
		}

		public override TVertex Invalid { get; }

		public override Map<TVertex, TValue> CreateMap<TValue>(TValue v0)
		{
			return new MappingMap<TVertex, TValue>(VertexesCount, v0, ToId);
		}

		public override ListMap<TVertex, TValue> CreateListMap<TValue>()
		{
			return new MappingListMap<TVertex, TValue>(VertexesCount, ToId);
		}
	}
}
