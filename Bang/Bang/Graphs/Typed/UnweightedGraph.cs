using System;
using System.Collections.Generic;

// GetEdges を抽象化します。
// 実行結果は入力グラフから分離されます。
namespace Bang.Graphs.Typed
{
	[System.Diagnostics.DebuggerDisplay(@"VertexesCount = {VertexesCount}")]
	public abstract class UnweightedGraph<T>
	{
		public abstract int VertexesCount { get; }
		public abstract IEnumerable<T> GetVertexes();
		public abstract List<T> GetEdges(T v);
	}

	public class ListUnweightedGraph<T> : UnweightedGraph<T>
	{
		protected readonly Dictionary<T, List<T>> map;
		public Dictionary<T, List<T>> AdjacencyList => map;
		public override int VertexesCount => map.Count;
		public override IEnumerable<T> GetVertexes() => map.Keys;
		public override List<T> GetEdges(T v) => map[v];

		public ListUnweightedGraph(Dictionary<T, List<T>> map) { this.map = map ?? new Dictionary<T, List<T>>(); }
		public ListUnweightedGraph() : this(null) { }
		public ListUnweightedGraph(IEnumerable<(T from, T to)> edges, bool twoWay) : this()
		{
			foreach (var (from, to) in edges) AddEdge(from, to, twoWay);
		}

		public bool AddVertex(T v)
		{
			if (map.ContainsKey(v)) return false;
			map[v] = new List<T>();
			return true;
		}

		public void AddEdge(T from, T to, bool twoWay)
		{
			if (!map.TryGetValue(from, out var edges)) map[from] = edges = new List<T>();
			edges.Add(to);
			if (twoWay)
			{
				if (!map.TryGetValue(to, out edges)) map[to] = edges = new List<T>();
				edges.Add(from);
			}
		}
	}
}
