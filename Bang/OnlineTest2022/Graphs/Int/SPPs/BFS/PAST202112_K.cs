using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.BFS
{
	// Test: https://atcoder.jp/contests/past202112-open/tasks/past202112_k
	class PAST202112_K
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int u, int v) Read2() { var a = Read(); return (a[0], a[1]); }
		static (int, int, int, int) Read4() { var a = Read(); return (a[0], a[1], a[2], a[3]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (n, m, qc, k) = Read4();
			var a = Read();
			var es = Array.ConvertAll(new bool[m], _ => Read2());
			var qs = Array.ConvertAll(new bool[qc], _ => Read2());

			var g = new ListUnweightedGraph(n + 1, es, true);
			var dists = Array.ConvertAll(a, sv => g.ShortestByBFS(sv));

			return string.Join("\n", qs.Select(q => dists.Min(d => d[q.u] + d[q.v])));
		}
	}
}
