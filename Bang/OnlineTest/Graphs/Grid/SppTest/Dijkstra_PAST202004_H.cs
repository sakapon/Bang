﻿using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs.Grid;

namespace OnlineTest.Graphs.Grid.SppTest
{
	// Test: https://atcoder.jp/contests/past202004-open/tasks/past202004_h
	class Dijkstra_PAST202004_H
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (n, m) = Read2();
			var s = GraphConsole.ReadGrid(n);

			Point sv = default, ev = default;
			var map = Array.ConvertAll(new bool[10], _ => new List<Point>());

			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
				{
					var c = s[i, j];
					var v = new Point(i, j);
					if (c == 'S') sv = v;
					else if (c == 'G') ev = v;
					else map[c - '0'].Add(v);
				}

			var r = ShortestPathCore.Dijkstra(n, m,
				v =>
				{
					var c = s[v];
					if (c == '9') return new[] { new Edge(v, ev, (ev - v).NormL1) };
					return Array.ConvertAll(map[c == 'S' ? 1 : c - '0' + 1].ToArray(), nv => new Edge(v, nv, (nv - v).NormL1));
				},
				sv, ev);
			Console.WriteLine(r.GetCost(ev));
		}
	}
}
