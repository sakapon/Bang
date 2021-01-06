using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs0
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

	public struct Edge<T>
	{
		public T From { get; }
		public T To { get; }
		public long Cost { get; }
		public Edge(T from, T to, long cost = 1) { From = from; To = to; Cost = cost; }
		public override string ToString() => $"{{{From}}} {{{To}}} {Cost}";
		public static implicit operator Edge<T>((T from, T to) v) => new Edge<T>(v.from, v.to);
		public static implicit operator Edge<T>((T from, T to, long cost) v) => new Edge<T>(v.from, v.to, v.cost);
		public Edge<T> Reverse() => new Edge<T>(To, From, Cost);
	}

	public static class EdgeHelper
	{
		public static Edge<int> ToEdge(int[] e) => new Edge<int>(e[0], e[1], e.Length > 2 ? e[2] : 1);
		public static Edge<int> ToEdge(long[] e) => new Edge<int>((int)e[0], (int)e[1], e.Length > 2 ? e[2] : 1);
	}

	public class HashMap<TKey, TValue>
	{
		TValue[] a;
		Func<TKey, int> ToHash;
		public HashMap(int count, TValue iv, Func<TKey, int> toHash)
		{
			a = Array.ConvertAll(new bool[count], _ => iv);
			ToHash = toHash;
		}
		public TValue this[TKey key] { get => a[ToHash(key)]; set => a[ToHash(key)] = value; }
	}
}
