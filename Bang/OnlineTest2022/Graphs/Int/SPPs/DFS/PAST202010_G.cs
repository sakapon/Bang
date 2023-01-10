using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.DFS
{
	// Test: https://atcoder.jp/contests/past202010-open/tasks/past202010_g
	class PAST202010_G
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (h, w) = Read2();
			var s = Array.ConvertAll(new bool[h], _ => Console.ReadLine());

			const char Road = '.';
			const char Wall = '#';
			var g = new CharUnweightedGrid(s);
			var count = 0;

			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
				{
					if (g[i][j] != Wall) continue;

					g[i][j] = Road;
					if (IsAllConnected()) count++;
					g[i][j] = Wall;
				}
			return count;

			bool IsAllConnected()
			{
				var sv = g.FindVertexId(Road);
				var r = g.BFSTree(sv);

				for (int i = 0; i < h; i++)
					for (int j = 0; j < w; j++)
					{
						if (g[i][j] == Wall) continue;
						if (!r[g.ToVertexId(i, j)].IsConnected) return false;
					}
				return true;
			}
		}
	}
}
