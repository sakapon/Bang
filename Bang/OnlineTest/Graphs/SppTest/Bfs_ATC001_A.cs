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

			var sv = TupleGridHelper.FindChar(s, 's');
			var ev = TupleGridHelper.FindChar(s, 'g');

			var r = ShortestPath.WithGrid(h, w)
				.WithUnweighted(v => Array.FindAll(TupleGridHelper.Nexts(v), v => s.GetValue(v) != '#'))
				.Bfs(sv, ev);
			return r.IsConnected(ev);
		}
	}
}
