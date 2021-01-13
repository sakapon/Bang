using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs.Int;

namespace OnlineTest.Graphs.SppTest.Int
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
			var es = GraphConsole.ReadEdges(m);

			// 0: 超頂点
			var map = new UnweightedMap(n + 1);
			foreach (var v in c)
				map.AddEdge(0, v, true);
			foreach (var e in es)
				map.AddEdge(h[e.From - 1] < h[e.To - 1] ? e : e.Reverse(), true);
			var r = map.Bfs(0);

			Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false });
			for (int v = 1; v <= n; v++)
				Console.WriteLine(r.IsConnected(v) ? r[v] - 1 : -1);
			Console.Out.Flush();
		}
	}
}
