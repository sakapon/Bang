using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

// ShortestByModBFS, Dijkstra
namespace OnlineTest2022.Graphs.Int.SPPs.ModBFS
{
	// Test: https://atcoder.jp/contests/abc277/tasks/abc277_e
	class ABC277_E
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (n, m, k) = Read3();
			var es = Array.ConvertAll(new bool[m], _ => Read3());
			var s = k == 0 ? new int[0] : Read();

			var graph = new ListWeightedGraph(2 * n + 1);
			foreach (var (u, v, a) in es)
			{
				graph.AddEdge(u + n * a, v + n * a, true, 1);
			}
			foreach (var v in s)
			{
				graph.AddEdge(v, v + n, true, 0);
			}
			var r = graph.ShortestByModBFS(2, 1 + n);

			var min = Math.Min(r[n], r[n + n]);
			return min != long.MaxValue ? min : -1;
		}
	}
}
