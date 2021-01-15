using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs.Grid;

namespace OnlineTest.Graphs.Grid.SppTest
{
	// Test: https://atcoder.jp/contests/atc002/tasks/abc007_3
	class Bfs_ATC002_A
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (h, w) = Read2();
			// 1-indexed に注意
			var sv = Read2();
			var ev = Read2();
			var s = GraphConsole.ReadEnclosedGrid(ref h, ref w);

			var r = ShortestPathCore.Bfs(h, w, v => Array.FindAll(v.Nexts(), nv => s[nv] != '#'), sv, ev);
			return r[ev];
		}

		static object Solve2()
		{
			var (h, w) = Read2();
			// 1-indexed に注意
			var sv = Read2();
			var ev = Read2();
			var s = GraphConsole.ReadEnclosedGrid(ref h, ref w);

			Point ud = (-1, 0);
			Point ld = (0, -1);

			var es = new List<Edge>();
			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
				{
					if (s[i, j] == '#') continue;
					Point v = new Point(i, j), nv;
					if (s[nv = v + ud] != '#') es.Add(new Edge(v, nv));
					if (s[nv = v + ld] != '#') es.Add(new Edge(v, nv));
				}

			var map = new UnweightedMap(h, w, es.ToArray(), false);
			var r = map.Bfs(sv, ev);
			return r[ev];
		}
	}
}
