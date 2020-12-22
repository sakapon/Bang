﻿using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
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
	}

	public struct UnweightedEdge<T>
	{
		public T From { get; }
		public T To { get; }
		public UnweightedEdge(T from, T to) { From = from; To = to; }
		public override string ToString() => $"{{{From}}} {{{To}}}";
		public UnweightedEdge<T> Reverse() => new UnweightedEdge<T>(To, From);
	}

	public struct WeightedEdge<T>
	{
		public T From { get; }
		public T To { get; }
		public long Cost { get; }
		public WeightedEdge(T from, T to, long cost) { From = from; To = to; Cost = cost; }
		public override string ToString() => $"{{{From}}} {{{To}}} {Cost}";
		public WeightedEdge<T> Reverse() => new WeightedEdge<T>(To, From, Cost);
	}

	public static class EdgeHelper
	{
		public static UnweightedEdge<int> Unweighted(int[] e) => new UnweightedEdge<int>(e[0], e[1]);
		public static UnweightedEdge<int> Unweighted(long[] e) => new UnweightedEdge<int>((int)e[0], (int)e[1]);
		public static WeightedEdge<int> Weighted(int[] e) => new WeightedEdge<int>(e[0], e[1], e.Length > 2 ? e[2] : 0);
		public static WeightedEdge<int> Weighted(long[] e) => new WeightedEdge<int>((int)e[0], (int)e[1], e.Length > 2 ? e[2] : 0);
	}

	public abstract class Map<TKey, TValue>
	{
		public abstract TValue this[TKey key] { get; set; }
	}

	public abstract class ReadOnlyMap<TKey, TValue>
	{
		public abstract TValue this[TKey key] { get; }
	}

	public abstract class ListMap<TKey, TValue> : ReadOnlyMap<TKey, TValue[]>
	{
		public abstract void Add(TKey key, TValue value);
	}

	public class IntMap<TValue> : Map<int, TValue>
	{
		TValue[] a;
		public IntMap(int count, TValue iv)
		{
			a = Array.ConvertAll(new bool[count], _ => iv);
		}
		public override TValue this[int key] { get => a[key]; set => a[key] = value; }
	}

	public class GridMap<TValue> : Map<(int i, int j), TValue>
	{
		TValue[][] a;
		public GridMap(int height, int width, TValue iv)
		{
			a = Array.ConvertAll(new bool[height], _ => Array.ConvertAll(new bool[width], __ => iv));
		}
		public override TValue this[(int i, int j) key] { get => a[key.i][key.j]; set => a[key.i][key.j] = value; }
	}

	public class HashMap<TKey, TValue> : Map<TKey, TValue>
	{
		TValue[] a;
		Func<TKey, int> ToHash;
		public HashMap(int count, TValue iv, Func<TKey, int> toHash)
		{
			a = Array.ConvertAll(new bool[count], _ => iv);
			ToHash = toHash;
		}
		public override TValue this[TKey key] { get => a[ToHash(key)]; set => a[ToHash(key)] = value; }
	}

	public class FuncReadOnlyMap<TKey, TValue> : ReadOnlyMap<TKey, TValue>
	{
		Func<TKey, TValue> GetValue;
		public FuncReadOnlyMap(Func<TKey, TValue> getValue)
		{
			GetValue = getValue;
		}
		public override TValue this[TKey key] => GetValue(key);
	}

	public class IntListMap<TValue> : ListMap<int, TValue>
	{
		List<TValue>[] map;
		public IntListMap(int count)
		{
			map = Array.ConvertAll(new bool[count], _ => new List<TValue>());
		}
		public override TValue[] this[int key] => map[key].ToArray();
		public override void Add(int key, TValue value) => map[key].Add(value);
	}

	public class GridListMap<TValue> : ListMap<(int i, int j), TValue>
	{
		List<TValue>[][] map;
		public GridListMap(int height, int width)
		{
			map = Array.ConvertAll(new bool[height], _ => Array.ConvertAll(new bool[width], __ => new List<TValue>()));
		}
		public override TValue[] this[(int i, int j) key] => map[key.i][key.j].ToArray();
		public override void Add((int i, int j) key, TValue value) => map[key.i][key.j].Add(value);
	}

	public class HashListMap<TKey, TValue> : ListMap<TKey, TValue>
	{
		List<TValue>[] map;
		Func<TKey, int> ToHash;
		public HashListMap(int count, Func<TKey, int> toHash)
		{
			map = Array.ConvertAll(new bool[count], _ => new List<TValue>());
			ToHash = toHash;
		}
		public override TValue[] this[TKey key] => map[ToHash(key)].ToArray();
		public override void Add(TKey key, TValue value) => map[ToHash(key)].Add(value);
	}
}
