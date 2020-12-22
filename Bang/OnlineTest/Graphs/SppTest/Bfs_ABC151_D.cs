using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
{
	// Test: https://atcoder.jp/contests/abc151/tasks/abc151_d
	class Bfs_ABC151_D
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (h, w) = Read2();
			var s = GraphConsole.ReadEnclosedGrid(ref h, ref w);

			var M = 0L;
			var r = ShortestPath.WithGrid(h, w)
				.WithUnweighted(v => Array.FindAll(TupleGridHelper.Nexts(v), v => s.GetByP(v) != '#'));

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
				{
					if (s[i][j] == '#') continue;

					r.Bfs((i, j), r.Factory.Invalid);

					for (int x = 0; x < h; x++)
						for (int y = 0; y < w; y++)
						{
							if (s[x][y] == '#') continue;

							M = Math.Max(M, r[(x, y)]);
						}
				}
			Console.WriteLine(M);
		}
	}
}
