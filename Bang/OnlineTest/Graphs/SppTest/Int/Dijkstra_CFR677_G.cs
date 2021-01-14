using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmLab.Graphs.Int;

namespace OnlineTest.Graphs.SppTest.Int
{
	// Test: https://codeforces.com/contest/1433/problem/G
	class Dijkstra_CFR677_G
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static void Main()
		{
			var (n, m, k) = Read3();
			var es = GraphConsole.ReadEdges(m);
			var rs = GraphConsole.ReadEdges(k);

			var map = new WeightedMap(n + 1, es, false);
			var costMap = Enumerable.Range(0, n + 1).Select(sv => map.Dijkstra(sv)).ToArray();

			long CostSum(Edge e) => rs.Sum(r =>
			{
				var (a, b) = (r.From, r.To);
				var m = costMap[a][b];
				m = Math.Min(m, costMap[a][e.From] + costMap[e.To][b]);
				m = Math.Min(m, costMap[a][e.To] + costMap[e.From][b]);
				return m;
			});
			Console.WriteLine(es.Min(CostSum));
		}
	}
}
