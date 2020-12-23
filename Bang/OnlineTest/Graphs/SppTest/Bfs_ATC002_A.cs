using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
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

			var r = ShortestPath.WithGrid(h, w)
				.WithUnweighted(v => Array.FindAll(v.Nexts(), nv => s.GetValue(nv) != '#'))
				.Bfs(sv, ev);
			return r[ev];
		}
	}
}
