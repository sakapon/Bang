using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.Dijkstra
{
	// Test: https://atcoder.jp/contests/past202203-open/tasks/past202203_k
	class PAST202203_K
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (n, m) = Read2();
			var es = Array.ConvertAll(new bool[m], _ => Read3());

			var g = new ListWeightedGraph(n + 1, es, false);
			var gr = new ListWeightedGraph(n + 1);

			foreach (var (u, v, w) in es)
			{
				gr.AddEdge(v, u, false, w);
			}

			var d = g.Dijkstra(1);
			var dr = gr.Dijkstra(n);

			return string.Join("\n", Enumerable.Range(1, n).Select(i => d[i] == long.MaxValue || dr[i] == long.MaxValue ? -1 : d[i] + dr[i]));
		}
	}
}
