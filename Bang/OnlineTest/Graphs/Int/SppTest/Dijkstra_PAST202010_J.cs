using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs.Int;

namespace OnlineTest.Graphs.Int.SppTest
{
	// Test: https://atcoder.jp/contests/past202010-open/tasks/past202010_j
	class Dijkstra_PAST202010_J
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static void Main()
		{
			var (n, m) = Read2();
			var (xab, xac, xbc) = Read3();
			var s = Console.ReadLine();
			var map = GraphConsole.ReadWeightedMap(n + 7, m, false);

			var (vab, vac, vbc) = (n + 1, n + 2, n + 3);
			var (vba, vca, vcb) = (n + 4, n + 5, n + 6);

			for (int v = 1; v <= n; v++)
			{
				if (s[v - 1] == 'A')
				{
					map.AddEdge(v, vab, xab, true);
					map.AddEdge(v, vac, xac, true);
					map.AddEdge(vba, v, 0, true);
					map.AddEdge(vca, v, 0, true);
				}
				else if (s[v - 1] == 'B')
				{
					map.AddEdge(v, vba, xab, true);
					map.AddEdge(v, vbc, xbc, true);
					map.AddEdge(vab, v, 0, true);
					map.AddEdge(vcb, v, 0, true);
				}
				else
				{
					map.AddEdge(v, vcb, xbc, true);
					map.AddEdge(v, vca, xac, true);
					map.AddEdge(vbc, v, 0, true);
					map.AddEdge(vac, v, 0, true);
				}
			}
			var r = map.Dijkstra(1, n);
			Console.WriteLine(r[n]);
		}
	}
}
