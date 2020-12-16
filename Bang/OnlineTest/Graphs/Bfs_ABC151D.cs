using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs
{
	// Test: https://atcoder.jp/contests/abc151/tasks/abc151_d
	class Bfs_ABC151D
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (h, w) = Read2();
			var s = GridHelper.ReadEnclosedGrid(ref h, ref w);

			var M = 0L;

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
				{
					if (s[i][j] == '#') continue;

					var r = ShortestPath.Bfs(h * w,
						v => GridHelper.ToId(v, w),
						id => GridHelper.FromId(id, w),
						v => Array.FindAll(GridHelper.Nexts(v), v => s.GetByP(v) != '#'),
						(i, j), (0, -1));

					M = Math.Max(M, r.RawCosts.Where(x => x < long.MaxValue).Max());
				}
			Console.WriteLine(M);
		}
	}
}
