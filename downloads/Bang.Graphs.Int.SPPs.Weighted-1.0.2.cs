using System;
using System.Collections.Generic;

// https://github.com/sakapon/Bang
namespace Bang.Graphs.Int.SPPs.Weighted.v1_0_2
{
	[System.Diagnostics.DebuggerDisplay(@"VertexesCount = {VertexesCount}")]
	public abstract class WeightedGraph
	{
		protected readonly int n;
		public int VertexesCount => n;
		public abstract List<(int to, long cost)> GetEdges(int v);
		protected WeightedGraph(int n) { this.n = n; }
	}

	public class ListWeightedGraph : WeightedGraph
	{
		protected readonly List<(int to, long cost)>[] map;
		public List<(int to, long cost)>[] AdjacencyList => map;
		public override List<(int to, long cost)> GetEdges(int v) => map[v];

		public ListWeightedGraph(List<(int to, long cost)>[] map) : base(map.Length) { this.map = map; }
		public ListWeightedGraph(int n) : base(n)
		{
			map = Array.ConvertAll(new bool[n], _ => new List<(int to, long cost)>());
		}
		public ListWeightedGraph(int n, IEnumerable<(int from, int to, int cost)> edges, bool twoWay) : this(n)
		{
			foreach (var (from, to, cost) in edges) AddEdge(from, to, twoWay, cost);
		}
		public ListWeightedGraph(int n, IEnumerable<(int from, int to, long cost)> edges, bool twoWay) : this(n)
		{
			foreach (var (from, to, cost) in edges) AddEdge(from, to, twoWay, cost);
		}

		public void AddEdge(int from, int to, bool twoWay, long cost)
		{
			map[from].Add((to, cost));
			if (twoWay) map[to].Add((from, cost));
		}
	}

	public class WeightedGrid : WeightedGraph
	{
		protected readonly int h, w;
		public int Height => h;
		public int Width => w;
		public WeightedGrid(int h, int w) : base(h * w) { this.h = h; this.w = w; }

		public int ToVertexId(int i, int j) => w * i + j;
		public (int i, int j) FromVertexId(int v) => (v / w, v % w);

		public static (int di, int dj)[] NextsDelta { get; } = new[] { (0, -1), (0, 1), (-1, 0), (1, 0) };
		public static (int di, int dj)[] NextsDelta8 { get; } = new[] { (0, -1), (0, 1), (-1, 0), (1, 0), (-1, -1), (-1, 1), (1, -1), (1, 1) };

		// コスト 1 の辺を設定します。
		public override List<(int to, long cost)> GetEdges(int v)
		{
			var (i, j) = (v / w, v % w);
			var l = new List<(int, long)>();
			foreach (var (di, dj) in NextsDelta)
			{
				var (ni, nj) = (i + di, j + dj);
				if (0 <= ni && ni < h && 0 <= nj && nj < w) l.Add((w * ni + nj, 1));
			}
			return l;
		}

		// コスト 1 の辺を設定します。
		public virtual ListWeightedGraph ToListGraph()
		{
			var g = new ListWeightedGraph(n);
			for (int i = 0; i < h; ++i)
				for (int j = 1; j < w; ++j)
				{
					var v = w * i + j;
					g.AddEdge(v, v - 1, true, 1);
				}
			for (int j = 0; j < w; ++j)
				for (int i = 1; i < h; ++i)
				{
					var v = w * i + j;
					g.AddEdge(v, v - w, true, 1);
				}
			return g;
		}
	}

	public class IntWeightedGrid : WeightedGrid
	{
		readonly int[][] s;
		public int[][] Cells => s;
		public int[] this[int i] => s[i];
		public IntWeightedGrid(int[][] s) : base(s.Length, s[0].Length) { this.s = s; }

		public override List<(int to, long cost)> GetEdges(int v)
		{
			var (i, j) = (v / w, v % w);
			var l = new List<(int, long)>();
			foreach (var (di, dj) in NextsDelta)
			{
				var (ni, nj) = (i + di, j + dj);
				if (0 <= ni && ni < h && 0 <= nj && nj < w) l.Add((w * ni + nj, s[ni][nj]));
			}
			return l;
		}

		public override ListWeightedGraph ToListGraph()
		{
			var g = new ListWeightedGraph(n);
			for (int i = 0; i < h; ++i)
				for (int j = 1; j < w; ++j)
				{
					var v = w * i + j;
					g.AddEdge(v, v - 1, false, s[i][j - 1]);
					g.AddEdge(v - 1, v, false, s[i][j]);
				}
			for (int j = 0; j < w; ++j)
				for (int i = 1; i < h; ++i)
				{
					var v = w * i + j;
					g.AddEdge(v, v - w, false, s[i - 1][j]);
					g.AddEdge(v - w, v, false, s[i][j]);
				}
			return g;
		}
	}

	public class CharWeightedGrid : WeightedGrid
	{
		readonly char[][] s;
		readonly char wall;
		public char[][] Cells => s;
		public char[] this[int i] => s[i];
		public CharWeightedGrid(char[][] s, char wall = '#') : base(s.Length, s[0].Length) { this.s = s; this.wall = wall; }
		public CharWeightedGrid(string[] s, char wall = '#') : this(ToArrays(s), wall) { }

		public static char[][] ToArrays(string[] s) => Array.ConvertAll(s, l => l.ToCharArray());

		public (int i, int j) FindCell(char c)
		{
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (s[i][j] == c) return (i, j);
			return (-1, -1);
		}

		public int FindVertexId(char c)
		{
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (s[i][j] == c) return w * i + j;
			return -1;
		}

		// 1 桁の整数が設定されている場合
		public override List<(int to, long cost)> GetEdges(int v)
		{
			var (i, j) = (v / w, v % w);
			var l = new List<(int, long)>();
			foreach (var (di, dj) in NextsDelta)
			{
				var (ni, nj) = (i + di, j + dj);
				if (0 <= ni && ni < h && 0 <= nj && nj < w && s[ni][nj] != wall) l.Add((w * ni + nj, s[ni][nj] - '0'));
			}
			return l;
		}

		// 1 桁の整数が設定されている場合
		public override ListWeightedGraph ToListGraph()
		{
			var g = new ListWeightedGraph(n);
			for (int i = 0; i < h; ++i)
				for (int j = 1; j < w; ++j)
				{
					if (s[i][j] == wall || s[i][j - 1] == wall) continue;
					var v = w * i + j;
					g.AddEdge(v, v - 1, false, s[i][j - 1] - '0');
					g.AddEdge(v - 1, v, false, s[i][j] - '0');
				}
			for (int j = 0; j < w; ++j)
				for (int i = 1; i < h; ++i)
				{
					if (s[i][j] == wall || s[i - 1][j] == wall) continue;
					var v = w * i + j;
					g.AddEdge(v, v - w, false, s[i - 1][j] - '0');
					g.AddEdge(v - w, v, false, s[i][j] - '0');
				}
			return g;
		}
	}

	public static class WeightedPath1
	{
		public static long[] Dijkstra(this WeightedGraph graph, int sv, int ev = -1)
		{
			var costs = Array.ConvertAll(new bool[graph.VertexesCount], _ => long.MaxValue);
			costs[sv] = 0;
			var q = new SortedSet<(long, int)> { (0, sv) };

			while (q.Count > 0)
			{
				var (c, v) = q.Min;
				q.Remove((c, v));
				if (v == ev) return costs;

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
		public static long[] ShortestByModBFS(this WeightedGraph graph, int mod, int sv, int ev = -1)
		{
			var costs = Array.ConvertAll(new bool[graph.VertexesCount], _ => long.MaxValue);
			costs[sv] = 0;
			var qs = Array.ConvertAll(new bool[mod], _ => new Queue<int>());
			qs[0].Enqueue(sv);
			var qc = 1;

			for (long c = 0; qc > 0; ++c)
			{
				var q = qs[c % mod];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					--qc;
					if (v == ev) return costs;
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
		public static PathResult DijkstraTree(this WeightedGraph graph, int sv, int ev = -1)
		{
			var r = new PathResult(graph.VertexesCount);
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var q = new SortedSet<(long, int)> { (0, sv) };

			while (q.Count > 0)
			{
				var (c, v) = q.Min;
				q.Remove((c, v));
				if (v == ev) return r;
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
		public static PathResult ModBFSTree(this WeightedGraph graph, int mod, int sv, int ev = -1)
		{
			var r = new PathResult(graph.VertexesCount);
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var qs = Array.ConvertAll(new bool[mod], _ => new Queue<int>());
			qs[0].Enqueue(sv);
			var qc = 1;

			for (long c = 0; qc > 0; ++c)
			{
				var q = qs[c % mod];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					--qc;
					if (v == ev) return r;
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
	public class PathResult
	{
		[System.Diagnostics.DebuggerDisplay(@"\{{Id}: Cost = {Cost}\}")]
		public class Vertex
		{
			public int Id { get; }
			public long Cost { get; set; } = long.MaxValue;
			public bool IsConnected => Cost != long.MaxValue;
			public Vertex Parent { get; set; }
			public Vertex(int id) { Id = id; }

			public int[] GetPathVertexes()
			{
				var path = new Stack<int>();
				for (var v = this; v != null; v = v.Parent)
					path.Push(v.Id);
				return path.ToArray();
			}

			public (int from, int to)[] GetPathEdges()
			{
				var path = new Stack<(int, int)>();
				for (var v = this; v.Parent != null; v = v.Parent)
					path.Push((v.Parent.Id, v.Id));
				return path.ToArray();
			}
		}

		public Vertex[] Vertexes { get; }
		public int VertexesCount => Vertexes.Length;
		public Vertex this[int v] => Vertexes[v];
		public PathResult(Vertex[] vertexes) { Vertexes = vertexes; }
		public PathResult(int n) : this(CreateVertexes(n)) { }

		static Vertex[] CreateVertexes(int n)
		{
			var vs = new Vertex[n];
			for (int v = 0; v < n; ++v) vs[v] = new Vertex(v);
			return vs;
		}
	}
}
