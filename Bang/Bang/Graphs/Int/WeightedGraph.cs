using System;
using System.Collections.Generic;

// 頂点 id は整数 [0, n) とします。
// GetEdges を抽象化します。
// 実行結果は入力グラフから分離されます。
namespace Bang.Graphs.Int
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
			// 8-adjacency
			//for (int i = 1; i < h; ++i)
			//	for (int j = 1; j < w; ++j)
			//	{
			//		var v = w * i + j;
			//		g.AddEdge(v, v - w - 1, true, 1);
			//		g.AddEdge(v - w, v - 1, true, 1);
			//	}
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
			// 8-adjacency
			//for (int i = 1; i < h; ++i)
			//	for (int j = 1; j < w; ++j)
			//	{
			//		var v = w * i + j;
			//		g.AddEdge(v, v - w - 1, false, s[i - 1][j - 1]);
			//		g.AddEdge(v - w - 1, v, false, s[i][j]);
			//		g.AddEdge(v - w, v - 1, false, s[i][j - 1]);
			//		g.AddEdge(v - 1, v - w, false, s[i - 1][j]);
			//	}
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
			// 8-adjacency
			//for (int i = 1; i < h; ++i)
			//	for (int j = 1; j < w; ++j)
			//	{
			//		var v = w * i + j;
			//		if (s[i][j] != wall && s[i - 1][j - 1] != wall)
			//		{
			//			g.AddEdge(v, v - w - 1, false, s[i - 1][j - 1] - '0');
			//			g.AddEdge(v - w - 1, v, false, s[i][j] - '0');
			//		}
			//		if (s[i - 1][j] != wall && s[i][j - 1] != wall)
			//		{
			//			g.AddEdge(v - w, v - 1, false, s[i][j - 1] - '0');
			//			g.AddEdge(v - 1, v - w, false, s[i - 1][j] - '0');
			//		}
			//	}
			return g;
		}
	}
}
