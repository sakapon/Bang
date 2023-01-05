using System;
using System.Collections.Generic;

namespace Bang.Graphs.Int.SPPs
{
	public static class UnweightedPath2
	{
		// 最短経路とは限りません。
		// 連結性のみを判定する場合は、DFS、BFS または Union-Find を利用します。
		public static PathResult DFSTree(this UnweightedGraph graph, int sv, int ev = -1)
		{
			var r = new PathResult(graph.VertexesCount);
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			DFS(sv);
			return r;

			bool DFS(int v)
			{
				if (v == ev) return true;
				var vo = vs[v];

				foreach (var nv in graph.GetEdges(v))
				{
					var nvo = vs[nv];
					if (nvo.Cost != long.MaxValue) continue;
					nvo.Cost = vo.Cost + 1;
					nvo.Parent = vo;
					if (DFS(nv)) return true;
				}
				return false;
			}
		}

		public static PathResult BFSTree(this UnweightedGraph graph, int sv, int ev = -1)
		{
			var r = new PathResult(graph.VertexesCount);
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var q = new Queue<int>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (v == ev) return r;
				var vo = vs[v];
				var nc = vo.Cost + 1;

				foreach (var nv in graph.GetEdges(v))
				{
					var nvo = vs[nv];
					if (nvo.Cost <= nc) continue;
					nvo.Cost = nc;
					nvo.Parent = vo;
					q.Enqueue(nv);
				}
			}
			return r;
		}
	}
}
