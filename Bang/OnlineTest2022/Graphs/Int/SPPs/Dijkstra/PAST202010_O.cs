using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.Dijkstra
{
	// Test: https://atcoder.jp/contests/past202010-open/tasks/past202010_o
	class PAST202010_O
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static long[] ReadL() => Array.ConvertAll(Console.ReadLine().Split(), long.Parse);
		static void Main()
		{
			var (n, m) = Read2();
			var a = ReadL();
			var es = Array.ConvertAll(new bool[m], _ => { var e = Read(); return (e[0] - 1, e[1], e[2]); });

			var g = new ListWeightedGraph(n + 1, es, false);

			for (int i = 0; i < n; i++)
			{
				g.AddEdge(i, i + 1, false, a[i]);
				g.AddEdge(i + 1, i, false, 0);
			}
			var r = g.Dijkstra(0, n);
			Console.WriteLine(a.Sum() - r[n]);
		}
	}
}
