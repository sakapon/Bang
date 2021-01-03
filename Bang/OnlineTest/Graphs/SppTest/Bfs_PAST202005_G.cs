using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
{
	// Test: https://atcoder.jp/contests/past202005-open/tasks/past202005_g
	class Bfs_PAST202005_G
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static void Main()
		{
			var (n, x, y) = Read3();
			var ps = Array.ConvertAll(new bool[n], _ => GraphConsole.ReadPoint());

			var (h, w) = (403, 403);
			Point d = (202, 202);
			var ev = (x, y) + d;

			var s = NewArray2<bool>(h, w);
			GridHelper.Enclose(ref h, ref w, ref s, true);
			foreach (var p in ps)
				s.SetValue(p + d, true);

			var r = ShortestPath.ForGrid(h, w)
				.ForUnweightedMap(v =>
				{
					var nvs = Array.ConvertAll(NextsByDelta, nd => v + nd);
					nvs = Array.FindAll(nvs, nv => !s.GetValue(nv));
					return nvs;
				})
				.Bfs(d, ev);
			Console.WriteLine(r.IsConnected(ev) ? r[ev] : -1);
		}

		static T[][] NewArray2<T>(int n1, int n2, T v = default) => Array.ConvertAll(new bool[n1], _ => Array.ConvertAll(new bool[n2], __ => v));
		public static Point[] NextsByDelta { get; } = new[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1), (-1, 1), (1, 1) };
	}
}
