using System;
using System.Collections.Generic;

namespace Bang.Graphs.Typed.SPPs
{
	public static class UnweightedPath2
	{
		// 最短経路とは限りません。
		// 連結性のみを判定する場合は、DFS、BFS または Union-Find を利用します。
		public static PathResult<T> DFSTree<T>(this UnweightedGraph<T> graph, T sv, T ev)
		{
			var r = new PathResult<T>(graph.GetVertexes());
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			DFS(sv);
			return r;

			bool DFS(T v)
			{
				if (vs.Comparer.Equals(v, ev)) return true;
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

		public static PathResult<T> BFSTree<T>(this UnweightedGraph<T> graph, T sv, T ev)
		{
			var r = new PathResult<T>(graph.GetVertexes());
			var vs = r.Vertexes;
			vs[sv].Cost = 0;
			var q = new Queue<T>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (vs.Comparer.Equals(v, ev)) return r;
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
