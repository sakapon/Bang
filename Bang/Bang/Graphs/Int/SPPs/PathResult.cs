using System;
using System.Collections.Generic;

namespace Bang.Graphs.Int.SPPs
{
	[System.Diagnostics.DebuggerDisplay(@"VertexesCount = {VertexesCount}")]
	public class PathResult
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

		public Vertex[] Vertexes { get; }
		public int VertexesCount => Vertexes.Length;
		public Vertex this[int v] => Vertexes[v];
		public PathResult(Vertex[] vertexes) { Vertexes = vertexes; }
		public PathResult(int n) : this(CreateVertexes(n)) { }

		static Vertex[] CreateVertexes(int n)
		{
			var vs = new Vertex[n];
			for (int v = 0; v < n; ++v) vs[v] = new Vertex(v);
			return vs;
		}
	}
}
