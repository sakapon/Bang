using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	public struct WeightedEdge
	{
		public static WeightedEdge Invalid { get; } = new WeightedEdge(-1, -1, -1);

		public int From { get; }
		public int To { get; }
		public long Cost { get; }

		public WeightedEdge(int from, int to, long cost)
		{
			From = from;
			To = to;
			Cost = cost;
		}
		public WeightedEdge(int[] e) : this(e[0], e[1], e.Length > 2 ? e[2] : 0) { }
		public WeightedEdge(long[] e) : this((int)e[0], (int)e[1], e.Length > 2 ? e[2] : 0) { }

		public WeightedEdge Reverse() => new WeightedEdge(To, From, Cost);
	}

	public struct WeightedEdge<T>
	{
		public T From { get; }
		public T To { get; }
		public long Cost { get; }

		public WeightedEdge(T from, T to, long cost)
		{
			From = from;
			To = to;
			Cost = cost;
		}
		public WeightedEdge(WeightedEdge e, Func<int, T> fromId) : this(fromId(e.From), fromId(e.To), e.Cost) { }

		public WeightedEdge Untype(Func<T, int> toId) => new WeightedEdge(toId(From), toId(To), Cost);
		public WeightedEdge<T> Reverse() => new WeightedEdge<T>(To, From, Cost);
	}

	public class UnweightedResult
	{
		public long[] RawCosts { get; }
		public int[] RawInVertexes { get; }
		public long this[int vertexId] => RawCosts[vertexId];
		public bool IsConnected(int vertexId) => RawCosts[vertexId] != long.MaxValue;

		public UnweightedResult(long[] costs, int[] inVertexes)
		{
			RawCosts = costs;
			RawInVertexes = inVertexes;
		}

		public int[] GetPathVertexes(int endVertexId)
		{
			var path = new Stack<int>();
			for (var v = endVertexId; v != -1; v = RawInVertexes[v])
				path.Push(v);
			return path.ToArray();
		}
	}

	public class WeightedResult
	{
		public long[] RawCosts { get; }
		public WeightedEdge[] RawInEdges { get; }
		public long this[int vertexId] => RawCosts[vertexId];
		public bool IsConnected(int vertexId) => RawCosts[vertexId] != long.MaxValue;

		public WeightedResult(long[] costs, WeightedEdge[] inEdges)
		{
			RawCosts = costs;
			RawInEdges = inEdges;
		}

		public int[] GetPathVertexes(int endVertexId)
		{
			var path = new Stack<int>();
			for (var v = endVertexId; v != -1; v = RawInEdges[v].From)
				path.Push(v);
			return path.ToArray();
		}

		public WeightedEdge[] GetPathEdges(int endVertexId)
		{
			var path = new Stack<WeightedEdge>();
			for (var e = RawInEdges[endVertexId]; e.From != -1; e = RawInEdges[e.From])
				path.Push(e);
			return path.ToArray();
		}
	}

	public class UnweightedResult<T> : UnweightedResult
	{
		Func<T, int> ToId;
		Func<int, T> FromId;
		public long this[T vertex] => RawCosts[ToId(vertex)];
		public bool IsConnected(T vertex) => RawCosts[ToId(vertex)] != long.MaxValue;

		public UnweightedResult(UnweightedResult result, Func<T, int> toId, Func<int, T> fromId) : base(result.RawCosts, result.RawInVertexes)
		{
			ToId = toId;
			FromId = fromId;
		}

		public T[] GetPathVertexes(T endVertex)
		{
			return Array.ConvertAll(GetPathVertexes(ToId(endVertex)), id => FromId(id));
		}
	}

	public class WeightedResult<T> : WeightedResult
	{
		Func<T, int> ToId;
		Func<int, T> FromId;
		public long this[T vertex] => RawCosts[ToId(vertex)];
		public bool IsConnected(T vertex) => RawCosts[ToId(vertex)] != long.MaxValue;

		public WeightedResult(WeightedResult result, Func<T, int> toId, Func<int, T> fromId) : base(result.RawCosts, result.RawInEdges)
		{
			ToId = toId;
			FromId = fromId;
		}

		public T[] GetPathVertexes(T endVertex)
		{
			return Array.ConvertAll(GetPathVertexes(ToId(endVertex)), id => FromId(id));
		}

		public WeightedEdge<T>[] GetPathEdges(T endVertex)
		{
			return Array.ConvertAll(GetPathEdges(ToId(endVertex)), e => new WeightedEdge<T>(e, FromId));
		}
	}
}
