using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs.Grid;

namespace OnlineTest.Graphs.Grid.SppTest
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
			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
				{
					if (s[i, j] == '#') continue;

					var r = ShortestPathCore.Bfs(h, w,
						v => Array.FindAll(v.Nexts(), nv => s[nv] != '#'),
						(i, j), (-1, -1));

					for (int x = 0; x < h; x++)
						for (int y = 0; y < w; y++)
						{
							if (s[x, y] == '#') continue;

							M = Math.Max(M, r[x, y]);
						}
				}
			Console.WriteLine(M);
		}
	}
}
