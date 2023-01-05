using System;
using System.Collections.Generic;

namespace Bang.Graphs.Int.SPPs
{
	public static class WeightedPath2
	{
		public static PathResult DijkstraTree(this WeightedGraph graph, int sv, int ev = -1)
		{
			var r = new PathResult(graph.VertexesCount);
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var q = new SortedSet<(long, int)> { (0, sv) };

			while (q.Count > 0)
			{
				var (c, v) = q.Min;
				q.Remove((c, v));
				if (v == ev) return r;
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
		public static PathResult ModBFSTree(this WeightedGraph graph, int mod, int sv, int ev = -1)
		{
			var r = new PathResult(graph.VertexesCount);
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var qs = Array.ConvertAll(new bool[mod], _ => new Queue<int>());
			qs[0].Enqueue(sv);
			var qc = 1;

			for (long c = 0; qc > 0; ++c)
			{
				var q = qs[c % mod];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					--qc;
					if (v == ev) return r;
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
