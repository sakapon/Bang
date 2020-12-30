using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
{
	// Test: https://atcoder.jp/contests/past202010-open/tasks/past202010_g
	class Bfs_PAST202010_G
	{
		const char Road = '.';
		const char Wall = '#';
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (h, w) = Read2();
			var s = GraphConsole.ReadGridAsChar(h);
			GridHelper.EncloseGrid(ref h, ref w, ref s, Wall);

			var count = 0;
			var r = ShortestPath.ForGrid(h, w)
				.ForUnweightedMap(v => Array.FindAll(v.Nexts(), nv => s.GetValue(nv) != '#'));

			for (int i = 1; i < h - 1; i++)
				for (int j = 1; j < w - 1; j++)
				{
					if (s[i][j] == Road) continue;

					s[i][j] = Road;

					var sv = GridHelper.FindValue(s, Road);
					r.Bfs(sv, r.Factory.Invalid);
					if (IsAllConnected(r)) count++;

					s[i][j] = Wall;
				}
			Console.WriteLine(count);

			bool IsAllConnected(UnweightedMapSpp<Point> r)
			{
				for (int i = 0; i < h; i++)
					for (int j = 0; j < w; j++)
					{
						if (s[i][j] == Wall) continue;
						if (!r.IsConnected((i, j))) return false;
					}
				return true;
			}
		}
	}
}
