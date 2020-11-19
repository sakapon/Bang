using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs0;

namespace OnlineTest.Graphs0
{
	// Test: https://codeforces.com/contest/20/problem/C
	class Dijkstra0Test2
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static void Main()
		{
			var h = Read();
			int n = h[0], m = h[1];
			var es = Array.ConvertAll(new bool[m], _ => Read());

			var (c, inEdges) = ShortestPath0.Dijkstra(n + 1, es, false, 1, n);
			if (inEdges[n] == null) { Console.WriteLine(-1); return; }

			var path = ShortestPath0.GetPathVertexes(inEdges, n);
			Console.WriteLine(string.Join(" ", path));
		}
	}
}
