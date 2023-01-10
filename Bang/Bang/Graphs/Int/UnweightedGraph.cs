using System;
using System.Collections.Generic;

// 頂点 id は整数 [0, n) とします。
// GetEdges を抽象化します。
// 実行結果は入力グラフから分離されます。
namespace Bang.Graphs.Int
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

		// 8-adjacency
		public virtual ListUnweightedGraph ToListGraph8()
		{
			var g = ToListGraph();
			for (int i = 1; i < h; ++i)
				for (int j = 1; j < w; ++j)
				{
					var v = w * i + j;
					g.AddEdge(v, v - w - 1, true);
					g.AddEdge(v - w, v - 1, true);
				}
			return g;
		}
	}

	public class CharUnweightedGrid : UnweightedGrid
	{
		protected readonly char[][] s;
		protected readonly char wall;
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

		// 8-adjacency
		public override ListUnweightedGraph ToListGraph8()
		{
			var g = ToListGraph();
			for (int i = 1; i < h; ++i)
				for (int j = 1; j < w; ++j)
				{
					var v = w * i + j;
					if (s[i][j] != wall && s[i - 1][j - 1] != wall)
						g.AddEdge(v, v - w - 1, true);
					if (s[i - 1][j] != wall && s[i][j - 1] != wall)
						g.AddEdge(v - w, v - 1, true);
				}
			return g;
		}
	}
}
