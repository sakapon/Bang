using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
{
	// Test: https://atcoder.jp/contests/past202012-open/tasks/past202012_h
	class Bfs_PAST202012_H
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (h, w) = Read2();
			Point sv = Read2();
			var s = GraphConsole.ReadEnclosedGrid(ref h, ref w);

			const string arrows = "v^><";
			var spp = ShortestPath.ForGrid(h, w)
				.ForUnweightedMap(v =>
				{
					var nvs = new List<Point>();
					var nva = v.Nexts();

					for (int k = 0; k < 4; k++)
					{
						var (i, j) = nva[k];
						if (s[i][j] == '.' || s[i][j] == arrows[k]) nvs.Add(nva[k]);
					}
					return nvs.ToArray();
				})
				.Bfs(sv, (-1, -1));

			Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false });
			for (int i = 1; i < h - 1; i++)
			{
				for (int j = 1; j < w - 1; j++)
					Console.Write(s[i][j] == '#' ? '#' : spp.IsConnected((i, j)) ? 'o' : 'x');
				Console.WriteLine();
			}
			Console.Out.Flush();
		}
	}
}
