using System;
using System.Linq;

// 401
namespace Bang.Data.Int
{
	/// <summary>
	/// 素集合データ構造を表します。
	/// </summary>
	[System.Diagnostics.DebuggerDisplay(@"ItemsCount = {ItemsCount}, SetsCount = {SetsCount}")]
	public class UnionFind
	{
		readonly int[] parents, sizes;
		int setsCount;

		public UnionFind(int n)
		{
			parents = new int[n];
			Array.Fill(parents, -1);
			sizes = new int[n];
			Array.Fill(sizes, 1);
			setsCount = n;
		}

		public int ItemsCount => parents.Length;
		public int SetsCount => setsCount;

		// path compression
		public int Find(int x) => parents[x] == -1 ? x : parents[x] = Find(parents[x]);
		public bool AreSame(int x, int y) => Find(x) == Find(y);
		public int GetSize(int x) => sizes[Find(x)];

		public bool Union(int x, int y)
		{
			if ((x = Find(x)) == (y = Find(y))) return false;

			// union by size
			if (sizes[x] < sizes[y]) (x, y) = (y, x);

			// x を根とします。
			parents[y] = x;
			sizes[x] += sizes[y];
			--setsCount;
			return true;
		}

		public ILookup<int, int> ToSets() => Enumerable.Range(0, parents.Length).ToLookup(Find);
	}
}
