using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Grid
{
	public struct Point : IEquatable<Point>
	{
		public int i, j;
		public Point(int i, int j) { this.i = i; this.j = j; }
		public void Deconstruct(out int i, out int j) { i = this.i; j = this.j; }
		public override string ToString() => $"{i} {j}";
		public static Point Parse(string s) => Array.ConvertAll(s.Split(), int.Parse);

		public static implicit operator Point(int[] v) => (v[0], v[1]);
		public static explicit operator int[](Point v) => new[] { v.i, v.j };
		public static implicit operator Point((int i, int j) v) => new Point(v.i, v.j);
		public static explicit operator (int, int)(Point v) => (v.i, v.j);

		public bool Equals(Point other) => i == other.i && j == other.j;
		public static bool operator ==(Point v1, Point v2) => v1.Equals(v2);
		public static bool operator !=(Point v1, Point v2) => !v1.Equals(v2);
		public override bool Equals(object obj) => obj is Point v && Equals(v);
		public override int GetHashCode() => (i, j).GetHashCode();

		public static Point operator -(Point v) => new Point(-v.i, -v.j);
		public static Point operator +(Point v1, Point v2) => new Point(v1.i + v2.i, v1.j + v2.j);
		public static Point operator -(Point v1, Point v2) => new Point(v1.i - v2.i, v1.j - v2.j);

		public bool IsInRange(int height, int width) => 0 <= i && i < height && 0 <= j && j < width;
		public Point[] Nexts() => new[] { new Point(i - 1, j), new Point(i + 1, j), new Point(i, j - 1), new Point(i, j + 1) };
		public static Point[] NextsByDelta { get; } = new[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };

		public int NormL1 => Math.Abs(i) + Math.Abs(j);
		public double Norm => Math.Sqrt(i * i + j * j);
	}

	public static class GridMap
	{
		public static GridMap<TValue> Create<TValue>(int height, int width, TValue iv) => new JaggedGridMap<TValue>(height, width, iv);
		public static GridMap<TValue> Create<TValue>(int height, int width, Func<TValue> getIV) => new JaggedGridMap<TValue>(height, width, getIV);
	}

	public abstract class GridMap<TValue>
	{
		public abstract int Height { get; }
		public abstract int Width { get; }
		public abstract TValue this[Point key] { get; set; }
		public abstract TValue this[int i, int j] { get; set; }
	}

	public class JaggedGridMap<TValue> : GridMap<TValue>
	{
		TValue[][] a;
		public override int Height => a.Length;
		public override int Width => a[0].Length;
		public JaggedGridMap(TValue[][] a) { this.a = a; }
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
		public override int Height => a.GetLength(0);
		public override int Width => a.GetLength(1);
		public RectGridMap(TValue[,] a) { this.a = a; }
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
