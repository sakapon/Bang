using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
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
		public GridListMap(int h, int w)
		{
			map = Array.ConvertAll(new bool[h], _ => Array.ConvertAll(new bool[w], __ => new List<TValue>()));
		}
		public override TValue[] this[(int i, int j) key] => map[key.i][key.j].ToArray();
		public override void Add((int i, int j) key, TValue value) => map[key.i][key.j].Add(value);
	}

	public class MappingListMap<TKey, TValue> : ListMap<TKey, TValue>
	{
		List<TValue>[] map;
		Func<TKey, int> ToId;
		public MappingListMap(int count, Func<TKey, int> toId)
		{
			map = Array.ConvertAll(new bool[count], _ => new List<TValue>());
			ToId = toId;
		}
		public override TValue[] this[TKey key] => map[ToId(key)].ToArray();
		public override void Add(TKey key, TValue value) => map[ToId(key)].Add(value);
	}
}
