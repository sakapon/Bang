using System;
using System.Collections.Generic;

namespace Bang.Graphs.Typed.SPPs.Weighted.v1_0_2
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
			if (!map.TryGetValue(to, out edges)) map[to] = edges = new List<(T to, long cost)>();
			if (twoWay) edges.Add((from, cost));
		}
	}

	public static class WeightedPath1
	{
		static Dictionary<T, TValue> CreateVertexes<T, TValue>(this WeightedGraph<T> graph, TValue value)
		{
			var d = new Dictionary<T, TValue>();
			foreach (var v in graph.GetVertexes()) d[v] = value;
			return d;
		}

		public static Dictionary<T, long> Dijkstra<T>(this WeightedGraph<T> graph, T sv, T ev)
		{
			var costs = graph.CreateVertexes(long.MaxValue);
			costs[sv] = 0;
			var q = new SortedSet<(long, T)> { (0, sv) };

			while (q.Count > 0)
			{
				var (c, v) = q.Min;
				q.Remove((c, v));
				if (costs.Comparer.Equals(v, ev)) return costs;

				foreach (var (nv, cost) in graph.GetEdges(v))
				{
					var nc = c + cost;
					if (costs[nv] <= nc) continue;
					if (costs[nv] != long.MaxValue) q.Remove((costs[nv], nv));
					q.Add((nc, nv));
					costs[nv] = nc;
				}
			}
			return costs;
		}

		// Dijkstra 法の特別な場合です。
		public static Dictionary<T, long> ShortestByModBFS<T>(this WeightedGraph<T> graph, int mod, T sv, T ev)
		{
			var costs = graph.CreateVertexes(long.MaxValue);
			costs[sv] = 0;
			var qs = Array.ConvertAll(new bool[mod], _ => new Queue<T>());
			qs[0].Enqueue(sv);
			var qc = 1;

			for (long c = 0; qc > 0; ++c)
			{
				var q = qs[c % mod];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					--qc;
					if (costs.Comparer.Equals(v, ev)) return costs;
					if (costs[v] < c) continue;

					foreach (var (nv, cost) in graph.GetEdges(v))
					{
						var nc = c + cost;
						if (costs[nv] <= nc) continue;
						costs[nv] = nc;
						qs[nc % mod].Enqueue(nv);
						++qc;
					}
				}
			}
			return costs;
		}
	}

	public static class WeightedPath2
	{
		public static PathResult<T> DijkstraTree<T>(this WeightedGraph<T> graph, T sv, T ev)
		{
			var r = new PathResult<T>(graph.GetVertexes());
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var q = new SortedSet<(long, T)> { (0, sv) };

			while (q.Count > 0)
			{
				var (c, v) = q.Min;
				q.Remove((c, v));
				if (vs.Comparer.Equals(v, ev)) return r;
				var vo = vs[v];

				foreach (var (nv, cost) in graph.GetEdges(v))
				{
					var nvo = vs[nv];
					var nc = c + cost;
					if (nvo.Cost <= nc) continue;
					if (nvo.Cost != long.MaxValue) q.Remove((nvo.Cost, nv));
					q.Add((nc, nv));
					nvo.Cost = nc;
					nvo.Parent = vo;
				}
			}
			return r;
		}

		// Dijkstra 法の特別な場合です。
		public static PathResult<T> ModBFSTree<T>(this WeightedGraph<T> graph, int mod, T sv, T ev)
		{
			var r = new PathResult<T>(graph.GetVertexes());
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var qs = Array.ConvertAll(new bool[mod], _ => new Queue<T>());
			qs[0].Enqueue(sv);
			var qc = 1;

			for (long c = 0; qc > 0; ++c)
			{
				var q = qs[c % mod];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					--qc;
					if (vs.Comparer.Equals(v, ev)) return r;
					var vo = vs[v];
					if (vo.Cost < c) continue;

					foreach (var (nv, cost) in graph.GetEdges(v))
					{
						var nvo = vs[nv];
						var nc = c + cost;
						if (nvo.Cost <= nc) continue;
						nvo.Cost = nc;
						nvo.Parent = vo;
						qs[nc % mod].Enqueue(nv);
						++qc;
					}
				}
			}
			return r;
		}
	}

	[System.Diagnostics.DebuggerDisplay(@"VertexesCount = {VertexesCount}")]
	public class PathResult<T>
	{
		[System.Diagnostics.DebuggerDisplay(@"\{{Id}: Cost = {Cost}\}")]
		public class Vertex
		{
			public T Id { get; }
			public long Cost { get; set; } = long.MaxValue;
			public bool IsConnected => Cost != long.MaxValue;
			public Vertex Parent { get; set; }
			public Vertex(T id) { Id = id; }

			public T[] GetPathVertexes()
			{
				var path = new Stack<T>();
				for (var v = this; v != null; v = v.Parent)
					path.Push(v.Id);
				return path.ToArray();
			}

			public (T from, T to)[] GetPathEdges()
			{
				var path = new Stack<(T, T)>();
				for (var v = this; v.Parent != null; v = v.Parent)
					path.Push((v.Parent.Id, v.Id));
				return path.ToArray();
			}
		}

		public Dictionary<T, Vertex> Vertexes { get; }
		public int VertexesCount => Vertexes.Count;
		public Vertex this[T v] => Vertexes[v];
		public PathResult(Dictionary<T, Vertex> vertexes) { Vertexes = vertexes; }
		public PathResult(IEnumerable<T> vertexes) : this(CreateVertexes(vertexes)) { }

		static Dictionary<T, Vertex> CreateVertexes(IEnumerable<T> vertexes)
		{
			var vs = new Dictionary<T, Vertex>();
			foreach (var v in vertexes) vs[v] = new Vertex(v);
			return vs;
		}
	}
}
