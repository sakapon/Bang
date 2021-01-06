using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs0
{
	public static class ShortestPath2
	{
	}

	public class HashSppFactory<TVertex>
	{
		public int VertexesCount { get; }
		public Func<TVertex, int> ToHash { get; }
		public TVertex Invalid { get; }
		public HashSppFactory(int vertexesCount, Func<TVertex, int> toHash, TVertex invalid)
		{
			VertexesCount = vertexesCount;
			ToHash = toHash;
			Invalid = invalid;
		}

		public HashMap<TVertex, TValue> CreateMap<TValue>(TValue iv)
		{
			return new HashMap<TVertex, TValue>(VertexesCount, iv, ToHash);
		}

		public UnweightedGraph<TVertex> CreateUnweightedGraph()
		{
			return new UnweightedGraph<TVertex>(VertexesCount, ToHash);
		}

		public UnweightedResult<TVertex> Bfs(Func<TVertex, TVertex[]> getNextVertexes, TVertex startVertex, TVertex endVertex)
		{
			return ShortestPathCore2<TVertex>.Bfs(this, getNextVertexes, startVertex, endVertex);
		}

		public UnweightedResult<TVertex> Bfs(UnweightedGraph<TVertex> graph, TVertex startVertex, TVertex endVertex)
		{
			return ShortestPathCore2<TVertex>.Bfs(this, v => graph[v], startVertex, endVertex);
		}
	}

	public class IntSpp : HashSppFactory<int>
	{
		public IntSpp(int vertexesCount) : base(vertexesCount, v => v, -1) { }
	}

	public class GridSpp : HashSppFactory<Point>
	{
		public GridSpp(int height, int width) : base(height * width, v => v.i * width + v.j, (-1, -1)) { }
	}

	public class CompressionSpp : HashSppFactory<int>
	{
		public CompressionSpp(int[] a) : base(a.Length, CreateToHash(a), int.MinValue) { }

		static Func<int, int> CreateToHash(int[] a)
		{
			var hs = new HashSet<int>();
			foreach (var v in a) hs.Add(v);
			var fromHash = new int[hs.Count];
			hs.CopyTo(fromHash);
			Array.Sort(fromHash);
			var d = new Dictionary<int, int>();
			for (int i = 0; i < fromHash.Length; i++) d[fromHash[i]] = i;
			return v => d[v];
		}
	}

	public class UnweightedGraph<TVertex>
	{
		List<TVertex>[] map;
		Func<TVertex, int> ToHash;
		public UnweightedGraph(int count, Func<TVertex, int> toHash)
		{
			map = Array.ConvertAll(new bool[count], _ => new List<TVertex>());
			ToHash = toHash;
		}

		public TVertex[] this[TVertex key] => map[ToHash(key)].ToArray();
		void Add(TVertex key, TVertex value) => map[ToHash(key)].Add(value);

		public void AddEdge(Edge<TVertex> edge, bool directed)
		{
			Add(edge.From, edge.To);
			if (!directed) Add(edge.To, edge.From);
		}

		public void AddEdge(TVertex from, TVertex to, bool directed)
		{
			Add(from, to);
			if (!directed) Add(to, from);
		}

		public void AddEdges(Edge<TVertex>[] edges, bool directed)
		{
			foreach (var e in edges)
			{
				Add(e.From, e.To);
				if (!directed) Add(e.To, e.From);
			}
		}
	}
}
