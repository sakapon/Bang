using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;

namespace OnlineTest2022.Graphs.Int.SPPs.Dijkstra
{
	// Test: https://atcoder.jp/contests/past202004-open/tasks/past202004_h
	class PAST202004_H3
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (h, w) = Read2();
			var s = Array.ConvertAll(new bool[h], _ => Console.ReadLine());

			var grid = new CharWeightedGrid(s);
			var sp = grid.FindCell('S');
			var ep = grid.FindCell('G');
			var sv = grid.FindVertexId('S');
			var ev = grid.FindVertexId('G');

			var map = Array.ConvertAll(new bool[1 << 7], _ => new List<(int i, int j)>());
			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
					map[s[i][j]].Add((i, j));
			map['0'] = map['S'];
			map[':'] = map['G'];

			var g = new ListWeightedGraph(grid.VertexesCount);
			for (char c = '0'; c < ':'; c++)
				foreach (var p in map[c])
					foreach (var q in map[c + 1])
						g.AddEdge(grid.ToVertexId(p.i, p.j), grid.ToVertexId(q.i, q.j), false, Distance(p, q));

			var r = g.ShortestForDAG(map['0'..':'].SelectMany(l => l.Select(p => grid.ToVertexId(p.i, p.j))).ToArray());
			return r[ev] != long.MaxValue ? r[ev] : -1;
		}

		static int Distance((int i, int j) p, (int i, int j) q) => Math.Abs(p.i - q.i) + Math.Abs(p.j - q.j);
	}

	public static class WeightedPath_H3
	{
		// DAG の場合
		public static long[] ShortestForDAG(this WeightedGraph graph, int[] order)
		{
			var costs = Array.ConvertAll(new bool[graph.VertexesCount], _ => long.MaxValue);
			costs[order[0]] = 0;

			foreach (var v in order)
			{
				if (costs[v] == long.MaxValue) continue;
				foreach (var (nv, cost) in graph.GetEdges(v))
				{
					var nc = costs[v] + cost;
					if (costs[nv] > nc) costs[nv] = nc;
				}
			}
			return costs;
		}
	}
}
