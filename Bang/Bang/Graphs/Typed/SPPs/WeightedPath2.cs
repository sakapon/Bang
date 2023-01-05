using System;
using System.Collections.Generic;

namespace Bang.Graphs.Typed.SPPs
{
	public static class WeightedPath2
	{
		public static PathResult<T> DijkstraTree<T>(this WeightedGraph<T> graph, T sv, T ev)
		{
			var r = new PathResult<T>(graph.GetVertexes());
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var q = new SortedSet<(long, T)> { (0, sv) };

			while (q.Count > 0)
			{
				var (c, v) = q.Min;
				q.Remove((c, v));
				if (vs.Comparer.Equals(v, ev)) return r;
				var vo = vs[v];

				foreach (var (nv, cost) in graph.GetEdges(v))
				{
					var nvo = vs[nv];
					var nc = c + cost;
					if (nvo.Cost <= nc) continue;
					if (nvo.Cost != long.MaxValue) q.Remove((nvo.Cost, nv));
					q.Add((nc, nv));
					nvo.Cost = nc;
					nvo.Parent = vo;
				}
			}
			return r;
		}

		// Dijkstra 法の特別な場合です。
		public static PathResult<T> ModBFSTree<T>(this WeightedGraph<T> graph, int mod, T sv, T ev)
		{
			var r = new PathResult<T>(graph.GetVertexes());
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
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
					if (vs.Comparer.Equals(v, ev)) return r;
					var vo = vs[v];
					if (vo.Cost < c) continue;

					foreach (var (nv, cost) in graph.GetEdges(v))
					{
						var nvo = vs[nv];
						var nc = c + cost;
						if (nvo.Cost <= nc) continue;
						nvo.Cost = nc;
						nvo.Parent = vo;
						qs[nc % mod].Enqueue(nv);
						++qc;
					}
				}
			}
			return r;
		}
	}
}
