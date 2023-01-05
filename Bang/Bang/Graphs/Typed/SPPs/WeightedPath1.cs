using System;
using System.Collections.Generic;

namespace Bang.Graphs.Typed.SPPs
{
	public static class WeightedPath1
	{
		static Dictionary<T, TValue> CreateVertexes<T, TValue>(this WeightedGraph<T> graph, TValue value)
		{
			var d = new Dictionary<T, TValue>();
			foreach (var v in graph.GetVertexes()) d[v] = value;
			return d;
		}

		public static Dictionary<T, long> Dijkstra<T>(this WeightedGraph<T> graph, T sv, T ev)
		{
			var costs = graph.CreateVertexes(long.MaxValue);
			costs[sv] = 0;
			var q = new SortedSet<(long, T)> { (0, sv) };

			while (q.Count > 0)
			{
				var (c, v) = q.Min;
				q.Remove((c, v));
				if (costs.Comparer.Equals(v, ev)) return costs;

				foreach (var (nv, cost) in graph.GetEdges(v))
				{
					var nc = c + cost;
					if (costs[nv] <= nc) continue;
					if (costs[nv] != long.MaxValue) q.Remove((costs[nv], nv));
					q.Add((nc, nv));
					costs[nv] = nc;
				}
			}
			return costs;
		}

		// Dijkstra 法の特別な場合です。
		public static Dictionary<T, long> ShortestByModBFS<T>(this WeightedGraph<T> graph, int mod, T sv, T ev)
		{
			var costs = graph.CreateVertexes(long.MaxValue);
			costs[sv] = 0;
			var qs = Array.ConvertAll(new bool[mod], _ => new Queue<T>());
			qs[0].Enqueue(sv);
			var qc = 1;

			for (long c = 0; qc > 0; ++c)
			{
				var q = qs[c % mod];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					--qc;
					if (costs.Comparer.Equals(v, ev)) return costs;
					if (costs[v] < c) continue;

					foreach (var (nv, cost) in graph.GetEdges(v))
					{
						var nc = c + cost;
						if (costs[nv] <= nc) continue;
						costs[nv] = nc;
						qs[nc % mod].Enqueue(nv);
						++qc;
					}
				}
			}
			return costs;
		}
	}
}
