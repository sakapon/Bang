using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.BFS
{
	// Test: https://atcoder.jp/contests/past202012-open/tasks/past202012_i
	class PAST202012_I
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static void Main()
		{
			var (n, m, k) = Read3();
			var h = Read();
			var c = Read();
			var es = Array.ConvertAll(new bool[m], _ => Read2());

			// 0: 超頂点
			var graph = new ListUnweightedGraph(n + 1);
			foreach (var v in c)
				graph.AddEdge(0, v, false);
			foreach (var e in es)
			{
				var (a, b) = e;
				if (h[a - 1] > h[b - 1]) (a, b) = (b, a);
				graph.AddEdge(a, b, false);
			}
			var r = graph.BFSTree(0);

			Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false });
			for (int v = 1; v <= n; v++)
				Console.WriteLine(r[v].IsConnected ? r[v].Cost - 1 : -1);
			Console.Out.Flush();
		}
	}
}
