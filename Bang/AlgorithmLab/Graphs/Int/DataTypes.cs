using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Int
{
	public struct Edge
	{
		public static Edge Invalid { get; } = new Edge(-1, -1, long.MinValue);

		public int From { get; }
		public int To { get; }
		public long Cost { get; }

		public Edge(int from, int to, long cost = 1) { From = from; To = to; Cost = cost; }
		public void Deconstruct(out int from, out int to) { from = From; to = To; }
		public void Deconstruct(out int from, out int to, out long cost) { from = From; to = To; cost = Cost; }
		public override string ToString() => $"{From} {To} {Cost}";

		public static implicit operator Edge(int[] e) => new Edge(e[0], e[1], e.Length > 2 ? e[2] : 1);
		public static implicit operator Edge(long[] e) => new Edge((int)e[0], (int)e[1], e.Length > 2 ? e[2] : 1);
		public static implicit operator Edge((int from, int to) v) => new Edge(v.from, v.to);
		public static implicit operator Edge((int from, int to, long cost) v) => new Edge(v.from, v.to, v.cost);

		public Edge Reverse() => new Edge(To, From, Cost);
	}

	public class CostResult
	{
		protected const int InvalidVertex = -1;

		public long[] RawCosts { get; }
		public CostResult(long[] costs) { RawCosts = costs; }

		public long this[int vertex] => RawCosts[vertex];
		public bool IsConnected(int vertex) => RawCosts[vertex] != long.MaxValue;
		public long GetCost(int vertex, long invalid = -1) => IsConnected(vertex) ? RawCosts[vertex] : invalid;
	}

	public class UnweightedResult : CostResult
	{
		public int[] RawInVertexes { get; }

		public UnweightedResult(long[] costs, int[] inVertexes) : base(costs)
		{
			RawInVertexes = inVertexes;
		}

		public int[] GetPathVertexes(int endVertex)
		{
			var path = new Stack<int>();
			for (var v = endVertex; v != InvalidVertex; v = RawInVertexes[v])
				path.Push(v);
			return path.ToArray();
		}

		public Edge[] GetPathEdges(int endVertex)
		{
			var path = new Stack<Edge>();
			for (var v = endVertex; RawInVertexes[v] != InvalidVertex; v = RawInVertexes[v])
				path.Push(new Edge(RawInVertexes[v], v));
			return path.ToArray();
		}
	}

	public class WeightedResult : CostResult
	{
		public Edge[] RawInEdges { get; }

		public WeightedResult(long[] costs, Edge[] inEdges) : base(costs)
		{
			RawInEdges = inEdges;
		}

		public int[] GetPathVertexes(int endVertex)
		{
			var path = new Stack<int>();
			for (var v = endVertex; v != InvalidVertex; v = RawInEdges[v].From)
				path.Push(v);
			return path.ToArray();
		}

		public Edge[] GetPathEdges(int endVertex)
		{
			var path = new Stack<Edge>();
			for (var e = RawInEdges[endVertex]; e.From != InvalidVertex; e = RawInEdges[e.From])
				path.Push(e);
			return path.ToArray();
		}
	}
}
