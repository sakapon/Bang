using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.Dijkstra
{
	// Test: https://atcoder.jp/contests/past201912-open/tasks/past201912_j
	class PAST201912_J
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (h, w) = Read2();
			var a = Array.ConvertAll(new bool[h], _ => Read());

			var g = new IntWeightedGrid(a);
			var r1 = g.Dijkstra(g.ToVertexId(h - 1, 0));
			var r2 = g.Dijkstra(g.ToVertexId(h - 1, w - 1));
			var r3 = g.Dijkstra(g.ToVertexId(0, w - 1));

			return Enumerable.Range(0, g.VertexesCount).Min(v => r1[v] + r2[v] + r3[v] - 2 * a[v / w][v % w]);
		}
	}
}
