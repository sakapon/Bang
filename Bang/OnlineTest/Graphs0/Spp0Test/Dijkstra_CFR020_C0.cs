using System;
using System.Collections.Generic;
using AlgorithmLab.DataTrees;
using AlgorithmLab.Graphs0;

namespace OnlineTest.Graphs0.Spp0Test
{
	// Test: https://codeforces.com/contest/20/problem/C
	// https://codeforces.com/contest/20/submission/101975965
	class Dijkstra_CFR020_C0
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (n, m) = Read2();
			var es = Array.ConvertAll(new bool[m], _ => Read());

			var (c, inEdges) = ShortestPath0.Dijkstra(n + 1, es, false, 1, n);
			if (inEdges[n] == null) { Console.WriteLine(-1); return; }

			var path = ShortestPath0.GetPathVertexes(inEdges, n);
			Console.WriteLine(string.Join(" ", path));
		}
	}
}
