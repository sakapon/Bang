using System;
using System.Collections.Generic;
using AlgorithmLab.DataTrees;
using AlgorithmLab.Graphs0;

namespace OnlineTest.Graphs0.Spp0Test
{
	// Test: https://codeforces.com/contest/20/problem/C
	// https://codeforces.com/contest/20/submission/101973330
	class Dijkstra_CFR020_C1
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static Edge[] ReadEdges(int count) => Array.ConvertAll(new bool[count], _ => new Edge(Read()));
		static void Main()
		{
			var (n, m) = Read2();
			var es = ReadEdges(m);

			var r = ShortestPath1.Dijkstra(n + 1, es, false, 1, n);
			if (!r.IsConnected(n)) { Console.WriteLine(-1); return; }

			var path = r.GetPathVertexes(n);
			Console.WriteLine(string.Join(" ", path));
		}
	}
}
