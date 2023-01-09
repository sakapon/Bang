using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.Dijkstra
{
	// Test: https://atcoder.jp/contests/past202010-open/tasks/past202010_j
	class PAST202010_J
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (n, m) = Read2();
			var (xab, xac, xbc) = Read3();
			var s = Console.ReadLine();
			var es = Array.ConvertAll(new bool[m], _ => Read3());

			var g = new ListWeightedGraph(n + 7, es, true);

			var (vab, vac, vbc) = (n + 1, n + 2, n + 3);
			var (vba, vca, vcb) = (n + 4, n + 5, n + 6);

			for (int v = 1; v <= n; v++)
			{
				if (s[v - 1] == 'A')
				{
					g.AddEdge(v, vab, false, xab);
					g.AddEdge(v, vac, false, xac);
					g.AddEdge(vba, v, false, 0);
					g.AddEdge(vca, v, false, 0);
				}
				else if (s[v - 1] == 'B')
				{
					g.AddEdge(v, vba, false, xab);
					g.AddEdge(v, vbc, false, xbc);
					g.AddEdge(vab, v, false, 0);
					g.AddEdge(vcb, v, false, 0);
				}
				else
				{
					g.AddEdge(v, vcb, false, xbc);
					g.AddEdge(v, vca, false, xac);
					g.AddEdge(vbc, v, false, 0);
					g.AddEdge(vac, v, false, 0);
				}
			}
			var r = g.Dijkstra(1, n);
			return r[n];
		}
	}
}
