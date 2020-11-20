using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmLab.DataTrees
{
	// 基本的な実装です。
	public class UnionFindTree0
	{
		readonly int[] p;
		public UnionFindTree0(int n) => p = Enumerable.Range(0, n).ToArray();
		public int GetRoot(int x) => p[x] == x ? x : p[x] = GetRoot(p[x]);
		public bool AreUnited(int x, int y) => GetRoot(x) == GetRoot(y);
		public void Unite(int x, int y) { if (!AreUnited(x, y)) p[p[y]] = p[x]; }
	}

	public class QuickFind
	{
		readonly int[] p;
		readonly List<int>[] g;
		public QuickFind(int n)
		{
			p = Enumerable.Range(0, n).ToArray();
			g = Array.ConvertAll(p, x => new List<int> { x });
		}
		public int GetRoot(int x) => p[x];
		public bool AreUnited(int x, int y) => p[x] == p[y];
		public void Unite(int x, int y)
		{
			(x, y) = (p[x], p[y]);
			if (x == y) return;
			if (g[x].Count < g[y].Count) (x, y) = (y, x);
			foreach (var z in g[y]) p[z] = x;
			g[x].AddRange(g[y]);
		}
	}
}
