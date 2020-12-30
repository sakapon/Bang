using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
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

			var r = ShortestPath.ForGrid(h, w)
				.ForUnweightedMap(v => Array.FindAll(v.Nexts(), nv => s.GetValue(nv) != '#'))
				.Bfs(sv, ev);
			return r.IsConnected(ev);
		}

		static bool Solve2()
		{
			var (h, w) = Read2();
			var s = GraphConsole.ReadEnclosedGrid(ref h, ref w);

			var sv = GridHelper.FindValue(s, 's');
			var ev = GridHelper.FindValue(s, 'g');

			var es = new List<Edge<Point>>();
			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
				{
					if (s[i][j] == '#') continue;
					Point v = new Point(i, j), nv;
					if (s.GetValue(nv = v + (-1, 0)) != '#') es.Add(new Edge<Point>(v, nv));
					if (s.GetValue(nv = v + (0, -1)) != '#') es.Add(new Edge<Point>(v, nv));
				}

			var r = ShortestPath.ForGrid(h, w)
				.ForUnweightedMap(es.ToArray(), false)
				.Bfs(sv, ev);
			return r.IsConnected(ev);
		}
	}
}
