﻿using System;
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

	public struct UnweightedEdge<T>
	{
		public T From { get; }
		public T To { get; }
		public UnweightedEdge(T from, T to) { From = from; To = to; }
		public UnweightedEdge<T> Reverse() => new UnweightedEdge<T>(To, From);
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

	public static class EdgeHelper
	{
		public static UnweightedEdge<int> Unweighted(int[] e) => new UnweightedEdge<int>(e[0], e[1]);
		public static UnweightedEdge<int> Unweighted(long[] e) => new UnweightedEdge<int>((int)e[0], (int)e[1]);
		public static WeightedEdge<int> Weighted(int[] e) => new WeightedEdge<int>(e[0], e[1], e.Length > 2 ? e[2] : 0);
		public static WeightedEdge<int> Weighted(long[] e) => new WeightedEdge<int>((int)e[0], (int)e[1], e.Length > 2 ? e[2] : 0);
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

	public abstract class Map<TKey, TValue>
	{
		public abstract TValue this[TKey key] { get; set; }
	}

	public abstract class ReadOnlyMap<TKey, TValue>
	{
		public abstract TValue this[TKey key] { get; }
	}

	public class IntMap<TValue> : Map<int, TValue>
	{
		TValue[] a;
		public IntMap(int count, TValue v0)
		{
			a = Array.ConvertAll(new bool[count], _ => v0);
		}
		public override TValue this[int key] { get => a[key]; set => a[key] = value; }
	}

	public class GridMap<TValue> : Map<(int i, int j), TValue>
	{
		TValue[][] a;
		public GridMap(int h, int w, TValue v0)
		{
			a = Array.ConvertAll(new bool[h], _ => Array.ConvertAll(new bool[w], __ => v0));
		}
		public override TValue this[(int i, int j) key] { get => a[key.i][key.j]; set => a[key.i][key.j] = value; }
	}

	public class MappingMap<TKey, TValue> : Map<TKey, TValue>
	{
		TValue[] a;
		Func<TKey, int> ToId;
		public MappingMap(int count, TValue v0, Func<TKey, int> toId)
		{
			a = Array.ConvertAll(new bool[count], _ => v0);
			ToId = toId;
		}
		public override TValue this[TKey key] { get => a[ToId(key)]; set => a[ToId(key)] = value; }
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

	public class IntListMap<TValue> : ReadOnlyMap<int, TValue[]>
	{
		List<TValue>[] map;
		public IntListMap(int count)
		{
			map = Array.ConvertAll(new bool[count], _ => new List<TValue>());
		}
		public override TValue[] this[int key] => map[key].ToArray();
		public void Add(int key, TValue value) => map[key].Add(value);
	}

	public class GridListMap<TValue> : ReadOnlyMap<(int i, int j), TValue[]>
	{
		List<TValue>[][] map;
		public GridListMap(int h, int w)
		{
			map = Array.ConvertAll(new bool[h], _ => Array.ConvertAll(new bool[w], __ => new List<TValue>()));
		}
		public override TValue[] this[(int i, int j) key] => map[key.i][key.j].ToArray();
		public void Add((int i, int j) key, TValue value) => map[key.i][key.j].Add(value);
	}

	public class MappingListMap<TKey, TValue> : ReadOnlyMap<TKey, TValue[]>
	{
		List<TValue>[] map;
		Func<TKey, int> ToId;
		public MappingListMap(int count, Func<TKey, int> toId)
		{
			map = Array.ConvertAll(new bool[count], _ => new List<TValue>());
			ToId = toId;
		}
		public override TValue[] this[TKey key] => map[ToId(key)].ToArray();
		public void Add(TKey key, TValue value) => map[ToId(key)].Add(value);
	}
}
