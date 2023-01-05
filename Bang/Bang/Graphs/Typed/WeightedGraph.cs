using System;
using System.Collections.Generic;

// GetEdges を抽象化します。
// 実行結果は入力グラフから分離されます。
namespace Bang.Graphs.Typed
{
	[System.Diagnostics.DebuggerDisplay(@"VertexesCount = {VertexesCount}")]
	public abstract class WeightedGraph<T>
	{
		public abstract int VertexesCount { get; }
		public abstract IEnumerable<T> GetVertexes();
		public abstract List<(T to, long cost)> GetEdges(T v);
	}

	public class ListWeightedGraph<T> : WeightedGraph<T>
	{
		protected readonly Dictionary<T, List<(T to, long cost)>> map;
		public Dictionary<T, List<(T to, long cost)>> AdjacencyList => map;
		public override int VertexesCount => map.Count;
		public override IEnumerable<T> GetVertexes() => map.Keys;
		public override List<(T to, long cost)> GetEdges(T v) => map[v];

		public ListWeightedGraph(Dictionary<T, List<(T to, long cost)>> map) { this.map = map ?? new Dictionary<T, List<(T to, long cost)>>(); }
		public ListWeightedGraph() : this(null) { }
		public ListWeightedGraph(IEnumerable<(T from, T to, int cost)> edges, bool twoWay) : this()
		{
			foreach (var (from, to, cost) in edges) AddEdge(from, to, twoWay, cost);
		}
		public ListWeightedGraph(IEnumerable<(T from, T to, long cost)> edges, bool twoWay) : this()
		{
			foreach (var (from, to, cost) in edges) AddEdge(from, to, twoWay, cost);
		}

		public bool AddVertex(T v)
		{
			if (map.ContainsKey(v)) return false;
			map[v] = new List<(T to, long cost)>();
			return true;
		}

		public void AddEdge(T from, T to, bool twoWay, long cost)
		{
			if (!map.TryGetValue(from, out var edges)) map[from] = edges = new List<(T to, long cost)>();
			edges.Add((to, cost));
			if (twoWay)
			{
				if (!map.TryGetValue(to, out edges)) map[to] = edges = new List<(T to, long cost)>();
				edges.Add((from, cost));
			}
		}
	}
}
