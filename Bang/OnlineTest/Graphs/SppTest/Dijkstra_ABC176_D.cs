using System;
using System.Collections.Generic;
using AlgorithmLab.DataTrees;
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
			var sv = Read2();
			var ev = Read2();
			var s = GridHelper.ReadEnclosedGrid(ref h, ref w);

			sv = (sv.i + 1, sv.j + 1);
			ev = (ev.i + 1, ev.j + 1);
			GridHelper.EncloseGrid(ref h, ref w, ref s);

			var r = ShortestPath.Dijkstra(h * w,
				v => GridHelper.ToId(v, w),
				id => GridHelper.FromId(id, w),
				v =>
				{
					var nids = new List<WeightedEdge<(int, int)>>();

					foreach (var nv in GridHelper.Nexts(v))
					{
						if (s.GetByP(nv) == '#') continue;
						nids.Add(new WeightedEdge<(int, int)>(v, nv, 0));
					}

					for (int i = -2; i <= 2; i++)
						for (int j = -2; j <= 2; j++)
						{
							var nv = (v.i + i, v.j + j);
							if (s.GetByP(nv) == '#') continue;
							nids.Add(new WeightedEdge<(int, int)>(v, nv, 1));
						}
					return nids.ToArray();
				},
				sv, ev);

			Console.WriteLine(r.IsConnected(ev) ? r[ev] : -1);
		}
	}
}
