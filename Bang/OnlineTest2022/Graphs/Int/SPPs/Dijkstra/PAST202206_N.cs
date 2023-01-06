using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.Dijkstra
{
	// Test: https://atcoder.jp/contests/past202206-open/tasks/past202206_n
	class PAST202206_N
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static long[] ReadL() => Array.ConvertAll(Console.ReadLine().Split(), long.Parse);
		static void Main()
		{
			var (h, w) = Read2();
			var s = Array.ConvertAll(new bool[h], _ => ReadL());

			s[0][0] = 1L << 50;
			s[^1][^1] = 1L << 50;

			var n = h * w;
			var graph = new ListWeightedGraph(n + 2);

			for (int i = 0; i < h; ++i)
				for (int j = 1; j < w; ++j)
				{
					var v = w * i + j;
					graph.AddEdge(v, v - 1, false, s[i][j - 1]);
					graph.AddEdge(v - 1, v, false, s[i][j]);
				}
			for (int j = 0; j < w; ++j)
				for (int i = 1; i < h; ++i)
				{
					var v = w * i + j;
					graph.AddEdge(v, v - w, false, s[i - 1][j]);
					graph.AddEdge(v - w, v, false, s[i][j]);
				}
			for (int i = 1; i < h; ++i)
				for (int j = 1; j < w; ++j)
				{
					var v = w * i + j;
					graph.AddEdge(v, v - w - 1, false, s[i - 1][j - 1]);
					graph.AddEdge(v - w - 1, v, false, s[i][j]);
					graph.AddEdge(v - w, v - 1, false, s[i][j - 1]);
					graph.AddEdge(v - 1, v - w, false, s[i - 1][j]);
				}

			for (int i = 0; i < h; i++)
			{
				graph.AddEdge(n, w * i, false, s[i][0]);
				graph.AddEdge(w * i + (w - 1), n + 1, false, 0);
			}
			for (int j = 1; j < w; j++)
			{
				graph.AddEdge(n, w * (h - 1) + j, false, s[h - 1][j]);
				graph.AddEdge(j, n + 1, false, 0);
			}

			var r = graph.DijkstraTree(n, n + 1);
			var path = r[n + 1].GetPathVertexes();

			var g = NewArray2(h, w, '.');
			foreach (var v in path[1..^1])
			{
				var (i, j) = FromVertexId(v);
				g[i][j] = '#';
			}

			Console.WriteLine(r[n + 1].Cost);
			foreach (var cs in g)
			{
				Console.WriteLine(new string(cs));
			}

			int ToVertexId(int i, int j) => w * i + j;
			(int i, int j) FromVertexId(int v) => (v / w, v % w);
		}

		static T[][] NewArray2<T>(int n1, int n2, T v = default) => Array.ConvertAll(new bool[n1], _ => Array.ConvertAll(new bool[n2], __ => v));
	}
}
