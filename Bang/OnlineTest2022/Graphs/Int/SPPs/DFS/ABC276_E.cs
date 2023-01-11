using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.DFS
{
	class ABC276_E
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main() => Console.WriteLine(Solve() ? "Yes" : "No");
		static bool Solve()
		{
			var (h, w) = Read2();
			var s = Array.ConvertAll(new bool[h], _ => Console.ReadLine());

			var grid = new CharUnweightedGrid(s);
			var (si, sj) = grid.FindCell('S');
			var sv = grid.ToVertexId(si, sj);
			grid[si][sj] = '#';

			var svs = grid.GetEdges(sv);

			for (int i = 0; i < svs.Count - 1; i++)
			{
				var r = grid.ShortestByBFS(svs[i]);

				for (int j = i + 1; j < svs.Count; j++)
				{
					if (r[svs[j]] != long.MaxValue) return true;
				}
			}
			return false;
		}
	}
}
