using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
{
	// Test: https://atcoder.jp/contests/abc176/tasks/abc176_d
	class Dijkstra_ABC176_D
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int i, int j) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (h, w) = Read2();
			// 1-indexed に注意
			var sv = GraphConsole.ReadPoint() + (1, 1);
			var ev = GraphConsole.ReadPoint() + (1, 1);
			var s = GraphConsole.ReadEnclosedGrid(ref h, ref w, delta: 2);

			// WeightedEdge<T>[] を静的に構築する方法では TLE。
			var r = ShortestPath.WithGrid(h, w)
				.WithWeighted(v =>
				{
					var nids = new List<WeightedEdge<Point>>();

					foreach (var nv in v.Nexts())
					{
						if (s.GetValue(nv) == '#') continue;
						nids.Add((v, nv, 0));
					}

					for (int i = -2; i <= 2; i++)
						for (int j = -2; j <= 2; j++)
						{
							var nv = v + (i, j);
							if (s.GetValue(nv) == '#') continue;
							nids.Add((v, nv, 1));
						}
					return nids.ToArray();
				})
				.Dijkstra(sv, ev);

			Console.WriteLine(r.IsConnected(ev) ? r[ev] : -1);
		}
	}
}
