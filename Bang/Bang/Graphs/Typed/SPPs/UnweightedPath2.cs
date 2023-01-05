using System;
using System.Collections.Generic;

// GetEdges を抽象化します。
// 実行結果は入力グラフから分離されます。
namespace Bang.Graphs.Typed.SPPs
{
	[System.Diagnostics.DebuggerDisplay(@"\{{Id}: Cost = {Cost}\}")]
	public class Vertex<T>
	{
		public T Id { get; }
		public long Cost { get; set; } = long.MaxValue;
		public bool IsConnected => Cost != long.MaxValue;
		public Vertex<T> Parent { get; set; }
		public Vertex(T id) { Id = id; }

		public T[] GetPathVertexes()
		{
			var path = new Stack<T>();
			for (var v = this; v != null; v = v.Parent)
				path.Push(v.Id);
			return path.ToArray();
		}

		public (T, T)[] GetPathEdges()
		{
			var path = new Stack<(T, T)>();
			for (var v = this; v.Parent != null; v = v.Parent)
				path.Push((v.Parent.Id, v.Id));
			return path.ToArray();
		}
	}

	public static class UnweightedGraphEx
	{
		static Dictionary<T, Vertex<T>> CreateVertexes<T>(this UnweightedGraph<T> graph)
		{
			var vs = new Dictionary<T, Vertex<T>>();
			foreach (var v in graph.GetVertexes()) vs[v] = new Vertex<T>(v);
			return vs;
		}

		// 最短経路とは限りません。
		// 連結性のみを判定する場合は、DFS、BFS または Union-Find を利用します。
		public static Dictionary<T, Vertex<T>> ConnectivityByDFS<T>(this UnweightedGraph<T> graph, T sv, T ev)
		{
			var vs = graph.CreateVertexes();
			vs[sv].Cost = 0;
			DFS(sv);
			return vs;

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

		public static Dictionary<T, Vertex<T>> ShortestByBFS<T>(this UnweightedGraph<T> graph, T sv, T ev)
		{
			var vs = graph.CreateVertexes();
			vs[sv].Cost = 0;
			var q = new Queue<T>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (vs.Comparer.Equals(v, ev)) return vs;
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
