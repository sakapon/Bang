using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs.Grid;

namespace OnlineTest.Graphs.Grid.SppTest
{
	// Test: https://atcoder.jp/contests/atc001/tasks/dfs_a
	class Bfs_ATC001_A
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main() => Console.WriteLine(Solve() ? "Yes" : "No");
		static bool Solve()
		{
			var (h, w) = Read2();
			var s = GraphConsole.ReadEnclosedGrid(ref h, ref w);

			var sv = GridHelper.FindValue(s, 's');
			var ev = GridHelper.FindValue(s, 'g');

			var r = ShortestPathCore.Bfs(h, w, v => Array.FindAll(v.Nexts(), nv => s[nv] != '#'), sv, ev);
			return r.IsConnected(ev);
		}

		static bool Solve2()
		{
			var (h, w) = Read2();
			var s = GraphConsole.ReadEnclosedGrid(ref h, ref w);

			var sv = GridHelper.FindValue(s, 's');
			var ev = GridHelper.FindValue(s, 'g');
			Point ud = (-1, 0);
			Point ld = (0, -1);

			var map = new UnweightedMap(h, w);
			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
				{
					if (s[i, j] == '#') continue;
					Point v = new Point(i, j), nv;
					if (s[nv = v + ud] != '#') map.AddEdge(v, nv, false);
					if (s[nv = v + ld] != '#') map.AddEdge(v, nv, false);
				}

			var r = map.Bfs(sv, ev);
			return r.IsConnected(ev);
		}
	}
}
