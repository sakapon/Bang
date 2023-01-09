using System;
using System.Collections.Generic;

// https://github.com/sakapon/Bang
namespace Bang.Graphs.Typed.SPPs.Unweighted.v1_0_2
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
			if (!map.TryGetValue(to, out edges)) map[to] = edges = new List<T>();
			if (twoWay) edges.Add(from);
		}
	}

	public static class UnweightedPath1
	{
		static Dictionary<T, TValue> CreateVertexes<T, TValue>(this UnweightedGraph<T> graph, TValue value)
		{
			var d = new Dictionary<T, TValue>();
			foreach (var v in graph.GetVertexes()) d[v] = value;
			return d;
		}

		// 最短経路とは限りません。
		// 連結性のみを判定する場合は、DFS、BFS または Union-Find を利用します。
		public static Dictionary<T, bool> ConnectivityByDFS<T>(this UnweightedGraph<T> graph, T sv, T ev)
		{
			var u = graph.CreateVertexes(false);
			u[sv] = true;
			DFS(sv);
			return u;

			bool DFS(T v)
			{
				if (u.Comparer.Equals(v, ev)) return true;
				foreach (var nv in graph.GetEdges(v))
				{
					if (u[nv]) continue;
					u[nv] = true;
					if (DFS(nv)) return true;
				}
				return false;
			}
		}

		public static Dictionary<T, long> ShortestByBFS<T>(this UnweightedGraph<T> graph, T sv, T ev)
		{
			var costs = graph.CreateVertexes(long.MaxValue);
			costs[sv] = 0;
			var q = new Queue<T>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (costs.Comparer.Equals(v, ev)) return costs;
				var nc = costs[v] + 1;

				foreach (var nv in graph.GetEdges(v))
				{
					if (costs[nv] <= nc) continue;
					costs[nv] = nc;
					q.Enqueue(nv);
				}
			}
			return costs;
		}
	}

	public static class UnweightedPath2
	{
		// 最短経路とは限りません。
		// 連結性のみを判定する場合は、DFS、BFS または Union-Find を利用します。
		public static PathResult<T> DFSTree<T>(this UnweightedGraph<T> graph, T sv, T ev)
		{
			var r = new PathResult<T>(graph.GetVertexes());
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			DFS(sv);
			return r;

			bool DFS(T v)
			{
				if (vs.Comparer.Equals(v, ev)) return true;
				var vo = vs[v];

				foreach (var nv in graph.GetEdges(v))
				{
					var nvo = vs[nv];
					if (nvo.Cost != long.MaxValue) continue;
					nvo.Cost = vo.Cost + 1;
					nvo.Parent = vo;
					if (DFS(nv)) return true;
				}
				return false;
			}
		}

		public static PathResult<T> BFSTree<T>(this UnweightedGraph<T> graph, T sv, T ev)
		{
			var r = new PathResult<T>(graph.GetVertexes());
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var q = new Queue<T>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (vs.Comparer.Equals(v, ev)) return r;
				var vo = vs[v];
				var nc = vo.Cost + 1;

				foreach (var nv in graph.GetEdges(v))
				{
					var nvo = vs[nv];
					if (nvo.Cost <= nc) continue;
					nvo.Cost = nc;
					nvo.Parent = vo;
					q.Enqueue(nv);
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
