using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

// Int, Typed
namespace OnlineTest2022.Graphs.Int.SPPs.Dijkstra
{
	// Test: https://judge.yosupo.jp/problem/shortest_path
	class YLC_SPP
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int, int) Read3() { var a = Read(); return (a[0], a[1], a[2]); }
		static (int, int, int, int) Read4() { var a = Read(); return (a[0], a[1], a[2], a[3]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (n, m, s, t) = Read4();
			var es = Array.ConvertAll(new bool[m], _ => Read3());

			var graph = new ListWeightedGraph(n, es, false);
			var r = graph.DijkstraTree(s, t);
			if (!r[t].IsConnected) return -1;

			var path = r[t].GetPathEdges();
			return $"{r[t].Cost} {path.Length}\n" + string.Join("\n", path.Select(e => $"{e.from} {e.to}"));
		}
	}
}
