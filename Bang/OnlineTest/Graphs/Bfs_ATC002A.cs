using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs
{
	// Test: https://atcoder.jp/contests/atc002/tasks/abc007_3
	class Bfs_ATC002A
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
			var s = GridHelper.ReadEnclosedGrid(ref h, ref w);

			var r = ShortestPath.Bfs(h * w,
				v => GridHelper.ToId(v, w),
				id => GridHelper.FromId(id, w),
				v => Array.FindAll(GridHelper.Nexts(v), v => s.GetByP(v) != '#'),
				sv, ev);
			return r[ev];
		}
	}
}
