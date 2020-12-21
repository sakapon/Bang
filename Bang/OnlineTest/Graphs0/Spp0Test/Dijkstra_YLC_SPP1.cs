using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmLab.DataTrees;
using AlgorithmLab.Graphs0;

namespace OnlineTest.Graphs0.Spp0Test
{
	// Test: https://judge.yosupo.jp/problem/shortest_path
	// https://judge.yosupo.jp/submission/33187
	class Dijkstra_YLC_SPP1
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static Edge[] ReadEdges(int count) => Array.ConvertAll(new bool[count], _ => new Edge(Read()));
		static void Main()
		{
			var h = Read();
			int n = h[0], m = h[1], s = h[2], t = h[3];
			var es = ReadEdges(m);

			var r = ShortestPath1.Dijkstra(n, es, true, s, t);
			if (!r.IsConnected(t)) { Console.WriteLine(-1); return; }

			var path = r.GetPathEdges(t);
			Console.WriteLine($"{r[t]} {path.Length}");
			Console.WriteLine(string.Join("\n", path.Select(e => $"{e.From} {e.To}")));
		}
	}
}
