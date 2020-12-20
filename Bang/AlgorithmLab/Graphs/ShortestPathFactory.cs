using System;

namespace AlgorithmLab.Graphs
{
	/// <summary>
	/// データ型による内部実装の違いを吸収します。
	/// </summary>
	/// <typeparam name="TVertex">頂点を表すオブジェクトの型。</typeparam>
	public abstract class SppFactory<TVertex>
	{
		public abstract TVertex Invalid { get; }
		public abstract Map<TVertex, TValue> CreateMap<TValue>(TValue v0);

		public UnweightedSppContext<TVertex> CreateUnweighted(Func<TVertex, TVertex[]> getNextVertexes)
		{
			var map = new FuncReadOnlyMap<TVertex, TVertex[]>(getNextVertexes);
			return new UnweightedSppContext<TVertex>(this, map);
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
	}
}
