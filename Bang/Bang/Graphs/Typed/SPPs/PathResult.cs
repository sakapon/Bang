using System;
using System.Collections.Generic;

namespace Bang.Graphs.Typed.SPPs
{
	[System.Diagnostics.DebuggerDisplay(@"VertexesCount = {VertexesCount}")]
	public class PathResult<T>
	{
		[System.Diagnostics.DebuggerDisplay(@"\{{Id}: Cost = {Cost}\}")]
		public class Vertex
		{
			public T Id { get; }
			public long Cost { get; set; } = long.MaxValue;
			public bool IsConnected => Cost != long.MaxValue;
			public Vertex Parent { get; set; }
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

		public Dictionary<T, Vertex> Vertexes { get; }
		public int VertexesCount => Vertexes.Count;
		public Vertex this[T v] => Vertexes[v];
		public PathResult(Dictionary<T, Vertex> vertexes) { Vertexes = vertexes; }
		public PathResult(IEnumerable<T> vertexes) : this(CreateVertexes(vertexes)) { }

		static Dictionary<T, Vertex> CreateVertexes(IEnumerable<T> vertexes)
		{
			var vs = new Dictionary<T, Vertex>();
			foreach (var v in vertexes) vs[v] = new Vertex(v);
			return vs;
		}
	}
}
