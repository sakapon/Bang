using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Grid
{
	public abstract class GridMap<TValue>
	{
		public abstract TValue this[Point key] { get; set; }
		public abstract TValue this[int i, int j] { get; set; }
	}

	public class JaggedGridMap<TValue> : GridMap<TValue>
	{
		TValue[][] a;
		public JaggedGridMap(int height, int width, TValue iv)
		{
			a = Array.ConvertAll(new bool[height], _ => Array.ConvertAll(new bool[width], __ => iv));
		}
		public JaggedGridMap(int height, int width, Func<TValue> getIV)
		{
			a = Array.ConvertAll(new bool[height], _ => Array.ConvertAll(new bool[width], __ => getIV()));
		}
		public override TValue this[Point key] { get => a[key.i][key.j]; set => a[key.i][key.j] = value; }
		public override TValue this[int i, int j] { get => a[i][j]; set => a[i][j] = value; }
	}

	public class RectGridMap<TValue> : GridMap<TValue>
	{
		TValue[,] a;
		public RectGridMap(int height, int width, TValue iv)
		{
			a = new TValue[height, width];
			for (int i = 0; i < height; ++i)
				for (int j = 0; j < width; ++j)
					a[i, j] = iv;
		}
		public RectGridMap(int height, int width, Func<TValue> getIV)
		{
			a = new TValue[height, width];
			for (int i = 0; i < height; ++i)
				for (int j = 0; j < width; ++j)
					a[i, j] = getIV();
		}
		public override TValue this[Point key] { get => a[key.i, key.j]; set => a[key.i, key.j] = value; }
		public override TValue this[int i, int j] { get => a[i, j]; set => a[i, j] = value; }
	}

	public struct Edge
	{
		public static Edge Invalid { get; } = new Edge((-1, -1), (-1, -1), long.MinValue);

		public Point From { get; }
		public Point To { get; }
		public long Cost { get; }

		public Edge(Point from, Point to, long cost = 1) { From = from; To = to; Cost = cost; }
		public void Deconstruct(out Point from, out Point to) { from = From; to = To; }
		public void Deconstruct(out Point from, out Point to, out long cost) { from = From; to = To; cost = Cost; }
		public override string ToString() => $"{{{From}}} {{{To}}} {Cost}";

		public static implicit operator Edge((Point from, Point to) v) => new Edge(v.from, v.to);
		public static implicit operator Edge((Point from, Point to, long cost) v) => new Edge(v.from, v.to, v.cost);

		public Edge Reverse() => new Edge(To, From, Cost);
	}

	public class CostResult
	{
		protected static readonly Point InvalidVertex = (-1, -1);

		public GridMap<long> RawCosts { get; }
		public CostResult(GridMap<long> costs) { RawCosts = costs; }

		public long this[Point vertex] => RawCosts[vertex];
		public bool IsConnected(Point vertex) => RawCosts[vertex] != long.MaxValue;
		public long GetCost(Point vertex, long invalid = -1) => IsConnected(vertex) ? RawCosts[vertex] : invalid;
	}

	public class UnweightedResult : CostResult
	{
		public GridMap<Point> RawInVertexes { get; }

		public UnweightedResult(GridMap<long> costs, GridMap<Point> inVertexes) : base(costs)
		{
			RawInVertexes = inVertexes;
		}

		public Point[] GetPathVertexes(Point endVertex)
		{
			var path = new Stack<Point>();
			for (var v = endVertex; v != InvalidVertex; v = RawInVertexes[v])
				path.Push(v);
			return path.ToArray();
		}

		public Edge[] GetPathEdges(Point endVertex)
		{
			var path = new Stack<Edge>();
			for (var v = endVertex; RawInVertexes[v] != InvalidVertex; v = RawInVertexes[v])
				path.Push(new Edge(RawInVertexes[v], v));
			return path.ToArray();
		}
	}

	public class WeightedResult : CostResult
	{
		public GridMap<Edge> RawInEdges { get; }

		public WeightedResult(GridMap<long> costs, GridMap<Edge> inEdges) : base(costs)
		{
			RawInEdges = inEdges;
		}

		public Point[] GetPathVertexes(Point endVertex)
		{
			var path = new Stack<Point>();
			for (var v = endVertex; v != InvalidVertex; v = RawInEdges[v].From)
				path.Push(v);
			return path.ToArray();
		}

		public Edge[] GetPathEdges(Point endVertex)
		{
			var path = new Stack<Edge>();
			for (var e = RawInEdges[endVertex]; e.From != InvalidVertex; e = RawInEdges[e.From])
				path.Push(e);
			return path.ToArray();
		}
	}
}
