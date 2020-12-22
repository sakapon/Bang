using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmLab.DataTrees;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
{
	// Test: https://judge.yosupo.jp/problem/shortest_path
	class Dijkstra_YLC_SPP
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static void Main()
		{
			var h = Read();
			int n = h[0], m = h[1], s = h[2], t = h[3];
			var es = GraphConsole.ReadWeightedEdges(m);

			var r = ShortestPath.WithInt(n)
				.WithWeighted(es, true)
				.Dijkstra(s, t);
			if (!r.IsConnected(t)) { Console.WriteLine(-1); return; }

			var path = r.GetPathEdges(t);
			Console.WriteLine($"{r[t]} {path.Length}");
			Console.WriteLine(string.Join("\n", path.Select(e => $"{e.From} {e.To}")));
		}
	}
}
