using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmLab.DataTrees
{
	// 基本的な実装です。
	public class UnionFindTree0
	{
		int[] p;
		public UnionFindTree0(int n) => p = Enumerable.Range(0, n).ToArray();
		public int GetRoot(int x) => p[x] == x ? x : p[x] = GetRoot(p[x]);
		public bool AreUnited(int x, int y) => GetRoot(x) == GetRoot(y);
		public void Unite(int x, int y) { if (!AreUnited(x, y)) p[p[y]] = p[x]; }
	}

	public class QuickFind
	{
		int[] p;
		List<int>[] g;
		public int GroupsCount { get; private set; }
		public QuickFind(int n)
		{
			p = Enumerable.Range(0, n).ToArray();
			g = Array.ConvertAll(p, x => new List<int> { x });
			GroupsCount = n;
		}
		public int GetRoot(int x) => p[x];
		public int GetSize(int x) => g[p[x]].Count;
		public List<int> GetGroup(int x) => g[p[x]];
		public bool AreUnited(int x, int y) => p[x] == p[y];
		public bool Unite(int x, int y)
		{
			if ((x = p[x]) == (y = p[y])) return false;
			if (g[x].Count < g[y].Count) (x, y) = (y, x);
			foreach (var z in g[y]) p[z] = x;
			g[x].AddRange(g[y]);
			--GroupsCount;
			return true;
		}
	}
}
