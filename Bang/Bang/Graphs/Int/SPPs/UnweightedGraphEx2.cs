using System;
using System.Collections.Generic;

namespace Bang.Graphs.Int.SPPs
{
	[System.Diagnostics.DebuggerDisplay(@"\{{Id}: Cost = {Cost}\}")]
	public class Vertex
	{
		public int Id { get; }
		public long Cost { get; set; } = long.MaxValue;
		public bool IsConnected => Cost != long.MaxValue;
		public Vertex Parent { get; set; }
		public Vertex(int id) { Id = id; }

		public int[] GetPathVertexes()
		{
			var path = new Stack<int>();
			for (var v = this; v != null; v = v.Parent)
				path.Push(v.Id);
			return path.ToArray();
		}

		public (int, int)[] GetPathEdges()
		{
			var path = new Stack<(int, int)>();
			for (var v = this; v.Parent != null; v = v.Parent)
				path.Push((v.Parent.Id, v.Id));
			return path.ToArray();
		}
	}

	public static class UnweightedGraphEx2
	{
		static Vertex[] CreateVertexes(this UnweightedGraph graph)
		{
			var vs = new Vertex[graph.VertexesCount];
			for (int v = 0; v < vs.Length; ++v) vs[v] = new Vertex(v);
			return vs;
		}

		// 最短経路とは限りません。
		// 連結性のみを判定する場合は、DFS、BFS または Union-Find を利用します。
		public static Vertex[] ConnectivityByDFS(this UnweightedGraph graph, int sv, int ev = -1)
		{
			var vs = graph.CreateVertexes();
			vs[sv].Cost = 0;
			DFS(sv);
			return vs;

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

		public static Vertex[] ShortestByBFS(this UnweightedGraph graph, int sv, int ev = -1)
		{
			var vs = graph.CreateVertexes();
			vs[sv].Cost = 0;
			var q = new Queue<int>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (v == ev) return vs;
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
			return vs;
		}
	}
}
