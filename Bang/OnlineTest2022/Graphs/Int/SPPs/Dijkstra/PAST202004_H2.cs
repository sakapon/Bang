using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Graphs.Int;

namespace OnlineTest2022.Graphs.Int.SPPs.Dijkstra
{
	// Test: https://atcoder.jp/contests/past202004-open/tasks/past202004_h
	class PAST202004_H2
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

			var r = g.ShortestByBFS(sv, ev);
			return r[ev] != long.MaxValue ? r[ev] : -1;
		}

		static int Distance((int i, int j) p, (int i, int j) q) => Math.Abs(p.i - q.i) + Math.Abs(p.j - q.j);
	}

	public static class WeightedPath_H2
	{
		// 木など、BFS の訪問順でよい場合
		public static long[] ShortestByBFS(this WeightedGraph graph, int sv, int ev = -1)
		{
			var costs = Array.ConvertAll(new bool[graph.VertexesCount], _ => long.MaxValue);
			costs[sv] = 0;
			var q = new Queue<int>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (v == ev) return costs;

				foreach (var (nv, cost) in graph.GetEdges(v))
				{
					var nc = costs[v] + cost;
					if (costs[nv] == long.MaxValue) q.Enqueue(nv);
					if (costs[nv] > nc) costs[nv] = nc;
				}
			}
			return costs;
		}
	}
}
