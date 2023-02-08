using System;
using System.Linq;

// 501
namespace Bang.Data.Int
{
	public class UnionFindMap<TValue>
	{
		readonly int[] parents, sizes;
		int setsCount;
		readonly TValue[] values;
		readonly Func<TValue, TValue, TValue> mergeValues;

		public UnionFindMap(TValue[] values, Func<TValue, TValue, TValue> mergeValues)
		{
			var n = values.Length;
			parents = new int[n];
			Array.Fill(parents, -1);
			sizes = new int[n];
			Array.Fill(sizes, 1);
			setsCount = n;
			this.values = values;
			this.mergeValues = mergeValues;
		}

		public UnionFindMap(int n, Func<TValue, TValue, TValue> mergeValues) : this(new TValue[n], mergeValues) { }

		public int ItemsCount => parents.Length;
		public int SetsCount => setsCount;
		public int Find(int x) => parents[x] == -1 ? x : parents[x] = Find(parents[x]);
		public bool AreSame(int x, int y) => Find(x) == Find(y);
		public int GetSize(int x) => sizes[Find(x)];

		public TValue this[int x]
		{
			get => values[Find(x)];
			set => values[Find(x)] = value;
		}

		public bool Union(int x, int y)
		{
			if ((x = Find(x)) == (y = Find(y))) return false;

			// 左右の順序を保って値をマージします。
			var v = mergeValues(values[x], values[y]);

			if (sizes[x] < sizes[y]) (x, y) = (y, x);
			parents[y] = x;
			sizes[x] += sizes[y];
			--setsCount;
			values[x] = v;
			return true;
		}

		public ILookup<int, int> ToGroups() => Enumerable.Range(0, parents.Length).ToLookup(Find);
	}
}
