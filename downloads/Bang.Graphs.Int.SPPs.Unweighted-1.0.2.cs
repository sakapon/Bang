using System;
using System.Collections.Generic;

// https://github.com/sakapon/Bang
namespace Bang.Graphs.Int.SPPs.Unweighted.v1_0_2
{
	[System.Diagnostics.DebuggerDisplay(@"VertexesCount = {VertexesCount}")]
	public abstract class UnweightedGraph
	{
		protected readonly int n;
		public int VertexesCount => n;
		public abstract List<int> GetEdges(int v);
		protected UnweightedGraph(int n) { this.n = n; }
	}

	public class ListUnweightedGraph : UnweightedGraph
	{
		protected readonly List<int>[] map;
		public List<int>[] AdjacencyList => map;
		public override List<int> GetEdges(int v) => map[v];

		public ListUnweightedGraph(List<int>[] map) : base(map.Length) { this.map = map; }
		public ListUnweightedGraph(int n) : base(n)
		{
			map = Array.ConvertAll(new bool[n], _ => new List<int>());
		}
		public ListUnweightedGraph(int n, IEnumerable<(int from, int to)> edges, bool twoWay) : this(n)
		{
			foreach (var (from, to) in edges) AddEdge(from, to, twoWay);
		}

		public void AddEdge(int from, int to, bool twoWay)
		{
			map[from].Add(to);
			if (twoWay) map[to].Add(from);
		}
	}

	public class UnweightedGrid : UnweightedGraph
	{
		protected readonly int h, w;
		public int Height => h;
		public int Width => w;
		public UnweightedGrid(int h, int w) : base(h * w) { this.h = h; this.w = w; }

		public int ToVertexId(int i, int j) => w * i + j;
		public (int i, int j) FromVertexId(int v) => (v / w, v % w);

		public static (int di, int dj)[] NextsDelta { get; } = new[] { (0, -1), (0, 1), (-1, 0), (1, 0) };
		public static (int di, int dj)[] NextsDelta8 { get; } = new[] { (0, -1), (0, 1), (-1, 0), (1, 0), (-1, -1), (-1, 1), (1, -1), (1, 1) };

		public override List<int> GetEdges(int v)
		{
			var (i, j) = (v / w, v % w);
			var l = new List<int>();
			foreach (var (di, dj) in NextsDelta)
			{
				var (ni, nj) = (i + di, j + dj);
				if (0 <= ni && ni < h && 0 <= nj && nj < w) l.Add(w * ni + nj);
			}
			return l;
		}

		public virtual ListUnweightedGraph ToListGraph()
		{
			var g = new ListUnweightedGraph(n);
			for (int i = 0; i < h; ++i)
				for (int j = 1; j < w; ++j)
				{
					var v = w * i + j;
					g.AddEdge(v, v - 1, true);
				}
			for (int j = 0; j < w; ++j)
				for (int i = 1; i < h; ++i)
				{
					var v = w * i + j;
					g.AddEdge(v, v - w, true);
				}
			return g;
		}
	}

	public class CharUnweightedGrid : UnweightedGrid
	{
		readonly char[][] s;
		readonly char wall;
		public char[][] Cells => s;
		public char[] this[int i] => s[i];
		public CharUnweightedGrid(char[][] s, char wall = '#') : base(s.Length, s[0].Length) { this.s = s; this.wall = wall; }
		public CharUnweightedGrid(string[] s, char wall = '#') : this(ToArrays(s), wall) { }

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

		public override List<int> GetEdges(int v)
		{
			var (i, j) = (v / w, v % w);
			var l = new List<int>();
			foreach (var (di, dj) in NextsDelta)
			{
				var (ni, nj) = (i + di, j + dj);
				if (0 <= ni && ni < h && 0 <= nj && nj < w && s[ni][nj] != wall) l.Add(w * ni + nj);
			}
			return l;
		}

		public override ListUnweightedGraph ToListGraph()
		{
			var g = new ListUnweightedGraph(n);
			for (int i = 0; i < h; ++i)
				for (int j = 1; j < w; ++j)
				{
					if (s[i][j] == wall || s[i][j - 1] == wall) continue;
					var v = w * i + j;
					g.AddEdge(v, v - 1, true);
				}
			for (int j = 0; j < w; ++j)
				for (int i = 1; i < h; ++i)
				{
					if (s[i][j] == wall || s[i - 1][j] == wall) continue;
					var v = w * i + j;
					g.AddEdge(v, v - w, true);
				}
			return g;
		}
	}

	public static class UnweightedPath1
	{
		// 最短経路とは限りません。
		// 連結性のみを判定する場合は、DFS、BFS または Union-Find を利用します。
		public static bool[] ConnectivityByDFS(this UnweightedGraph graph, int sv, int ev = -1)
		{
			var u = new bool[graph.VertexesCount];
			u[sv] = true;
			DFS(sv);
			return u;

			bool DFS(int v)
			{
				if (v == ev) return true;
				foreach (var nv in graph.GetEdges(v))
				{
					if (u[nv]) continue;
					u[nv] = true;
					if (DFS(nv)) return true;
				}
				return false;
			}
		}

		public static long[] ShortestByBFS(this UnweightedGraph graph, int sv, int ev = -1)
		{
			var costs = Array.ConvertAll(new bool[graph.VertexesCount], _ => long.MaxValue);
			costs[sv] = 0;
			var q = new Queue<int>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (v == ev) return costs;
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
		public static PathResult DFSTree(this UnweightedGraph graph, int sv, int ev = -1)
		{
			var r = new PathResult(graph.VertexesCount);
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			DFS(sv);
			return r;

			bool DFS(int v)
			{
				if (v == ev) return true;
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

		public static PathResult BFSTree(this UnweightedGraph graph, int sv, int ev = -1)
		{
			var r = new PathResult(graph.VertexesCount);
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var q = new Queue<int>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (v == ev) return r;
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
