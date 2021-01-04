using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
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
			var es = GraphConsole.ReadEdges(m);

			var (vab, vac, vbc) = (n + 1, n + 2, n + 3);
			var (vba, vca, vcb) = (n + 4, n + 5, n + 6);

			var spp = ShortestPath.ForInt(n + 7).ForWeightedMap();
			foreach (var e in es)
			{
				spp.AddEdge(e, false);
			}

			for (int v = 1; v <= n; v++)
			{
				if (s[v - 1] == 'A')
				{
					spp.AddEdge(v, vab, xab, true);
					spp.AddEdge(v, vac, xac, true);
					spp.AddEdge(vba, v, 0, true);
					spp.AddEdge(vca, v, 0, true);
				}
				else if (s[v - 1] == 'B')
				{
					spp.AddEdge(v, vba, xab, true);
					spp.AddEdge(v, vbc, xbc, true);
					spp.AddEdge(vab, v, 0, true);
					spp.AddEdge(vcb, v, 0, true);
				}
				else
				{
					spp.AddEdge(v, vcb, xbc, true);
					spp.AddEdge(v, vca, xac, true);
					spp.AddEdge(vbc, v, 0, true);
					spp.AddEdge(vac, v, 0, true);
				}
			}
			spp.Dijkstra(1, n);
			Console.WriteLine(spp[n]);
		}
	}
}
