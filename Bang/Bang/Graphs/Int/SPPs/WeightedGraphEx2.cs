using System;
using System.Collections.Generic;

namespace Bang.Graphs.Int.SPPs
{
	public static class WeightedGraphEx2
	{
		static Vertex[] CreateVertexes(int n)
		{
			var vs = new Vertex[n];
			for (int v = 0; v < n; ++v) vs[v] = new Vertex(v);
			return vs;
		}

		public static Vertex[] DijkstraTree(this WeightedGraph graph, int sv, int ev = -1)
		{
			var vs = CreateVertexes(graph.VertexesCount);
			vs[sv].Cost = 0;
			var q = new SortedSet<(long, int)> { (0, sv) };

			while (q.Count > 0)
			{
				var (c, v) = q.Min;
				q.Remove((c, v));
				if (v == ev) return vs;
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
			return vs;
		}

		// Dijkstra 法の特別な場合です。
		public static Vertex[] ModBFSTree(this WeightedGraph graph, int mod, int sv, int ev = -1)
		{
			var vs = CreateVertexes(graph.VertexesCount);
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
					if (v == ev) return vs;
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
			return vs;
		}
	}
}
