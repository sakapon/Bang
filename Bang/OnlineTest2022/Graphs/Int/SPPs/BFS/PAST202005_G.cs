using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.BFS
{
	// Test: https://atcoder.jp/contests/past202005-open/tasks/past202005_g
	class PAST202005_G
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (n, x, y) = Read3();
			var ps = Array.ConvertAll(new bool[n], _ => Read2());

			var grid = new UnweightedGridG(ps);
			var sv = grid.ToVertexId(0, 0);
			var ev = grid.ToVertexId(x, y);

			var r = grid.ShortestByBFS(sv, ev);
			return r[ev] != long.MaxValue ? r[ev] : -1;
		}
	}

	public class UnweightedGridG : UnweightedGraph
	{
		const int h = 403, w = 403;
		const int oi = 201, oj = 201;
		public int Height => h;
		public int Width => w;

		readonly bool[] pset;

		public UnweightedGridG((int, int)[] ps) : base(h * w)
		{
			pset = new bool[n];
			foreach (var (i, j) in ps)
			{
				pset[ToVertexId(i, j)] = true;
			}
		}

		public int ToVertexId(int i, int j) => w * (i + oi) + j + oj;
		public (int i, int j) FromVertexId(int v) => (v / w - oi, v % w - oj);

		public static (int di, int dj)[] NextsDelta { get; } = new[] { (0, -1), (0, 1), (-1, 0), (1, 0), (-1, 1), (1, 1) };

		public override List<int> GetEdges(int v)
		{
			var (i, j) = (v / w, v % w);
			var l = new List<int>();
			foreach (var (di, dj) in NextsDelta)
			{
				var (ni, nj) = (i + di, j + dj);
				if (0 <= ni && ni < h && 0 <= nj && nj < w && !pset[w * ni + nj]) l.Add(w * ni + nj);
			}
			return l;
		}
	}
}
