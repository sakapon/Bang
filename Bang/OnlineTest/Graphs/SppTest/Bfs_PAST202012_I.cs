using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
{
	// Test: https://atcoder.jp/contests/past202012-open/tasks/past202012_i
	class Bfs_PAST202012_I
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static void Main()
		{
			var (n, m, k) = Read3();
			var h = Read();
			var c = Read();
			var es = Array.ConvertAll(new bool[m], _ => Read());

			var spp = ShortestPath.ForInt(n + 1).ForUnweightedMap();
			foreach (var v in c)
			{
				spp.AddEdge(0, v, true);
			}
			foreach (var e in es)
			{
				if (h[e[0] - 1] > h[e[1] - 1]) (e[0], e[1]) = (e[1], e[0]);
				spp.AddEdge(e[0], e[1], true);
			}
			spp.Bfs(0, -1);

			Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false });
			for (int v = 1; v <= n; v++)
				Console.WriteLine(spp.IsConnected(v) ? spp[v] - 1 : -1);
			Console.Out.Flush();
		}
	}
}
