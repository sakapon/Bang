﻿using System;
using System.Collections.Generic;
using AlgorithmLab.DataTrees;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs0.Spp0Test
{
	// Test: https://codeforces.com/contest/20/problem/C
	class Dijkstra_CFR020_C1
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (n, m) = Read2();
			var es = GraphConsole.ReadEdges(m);

			var r = ShortestPath.Dijkstra(n + 1, es, false, 1, n);
			if (!r.IsConnected(n)) { Console.WriteLine(-1); return; }

			var path = r.GetPathVertexes(n);
			Console.WriteLine(string.Join(" ", path));
		}
	}
}
