using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs.Grid;

namespace OnlineTest.Graphs.Grid.SppTest
{
	// Test: https://atcoder.jp/contests/past201912-open/tasks/past201912_j
	class Dijkstra_PAST201912_J
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (h, w) = Read2();
			var a = GraphConsole.ReadIntGrid(h);

			WeightedResult Dijkstra(Point sv) => ShortestPathCore.Dijkstra(h, w,
				v => Array.ConvertAll(Array.FindAll(v.Nexts(), nv => nv.IsInRange(h, w)), nv => new Edge(v, nv, a[nv])),
				sv, (-1, -1));

			var r1 = Dijkstra((h - 1, 0));
			var r2 = Dijkstra((h - 1, w - 1));
			var r3 = Dijkstra((0, w - 1));

			var m = long.MaxValue;
			foreach (var v in GridHelper.GetPoints(h, w))
				m = Math.Min(m, r1[v] + r2[v] + r3[v] - 2 * a[v]);
			Console.WriteLine(m);
		}
	}
}
