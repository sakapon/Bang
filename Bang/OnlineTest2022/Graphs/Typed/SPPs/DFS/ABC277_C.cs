using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Typed.SPPs.Unweighted.v1_0_2;

// ShortestByBFS, ConnectivityByDFS, BFSTree, DFSTree
namespace OnlineTest2022.Graphs.Typed.SPPs.DFS
{
	// Test: https://atcoder.jp/contests/abc277/tasks/abc277_c
	class ABC277_C
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var n = int.Parse(Console.ReadLine());
			var es = Array.ConvertAll(new bool[n], _ => Read2());

			var graph = new ListUnweightedGraph<int>(es, true);
			graph.AddVertex(1);
			var r = graph.ConnectivityByDFS(1, -1);
			return r.Where(p => p.Value).Max(p => p.Key);
		}
	}
}
