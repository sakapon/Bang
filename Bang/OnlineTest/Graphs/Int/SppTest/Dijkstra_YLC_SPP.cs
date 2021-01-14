using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmLab.Graphs.Int;

namespace OnlineTest.Graphs.Int.SppTest
{
	// Test: https://judge.yosupo.jp/problem/shortest_path
	class Dijkstra_YLC_SPP
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int, int, int) Read4() { var a = Read(); return (a[0], a[1], a[2], a[3]); }
		static void Main()
		{
			var (n, m, s, t) = Read4();
			var map = GraphConsole.ReadWeightedMap(n, m, true);

			var r = map.Dijkstra(s, t);
			if (!r.IsConnected(t)) { Console.WriteLine(-1); return; }

			var path = r.GetPathEdges(t);
			Console.WriteLine($"{r[t]} {path.Length}");
			Console.WriteLine(string.Join("\n", path.Select(e => $"{e.From} {e.To}")));
		}
	}
}
