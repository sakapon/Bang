using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.BFS
{
	// Test: https://atcoder.jp/contests/abc277/tasks/abc277_e
	class ABC277_E
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries), int.Parse);
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (n, m, k) = Read3();
			var es = Array.ConvertAll(new bool[m], _ => Read3());
			var s = Read().ToHashSet();

			var graph = new ListUnweightedGraph(2 * n + 1);
			foreach (var (u, v, a) in es)
			{
				// 距離 0 で連結している頂点を同値類とします。
				var u2 = a == 1 || s.Contains(u) ? u + n : u;
				var v2 = a == 1 || s.Contains(v) ? v + n : v;
				graph.AddEdge(u2, v2, true);
			}
			var r = graph.ShortestByBFS(1 + n);

			var min = Math.Min(r[n], r[n + n]);
			return min != long.MaxValue ? min : -1;
		}
	}
}
