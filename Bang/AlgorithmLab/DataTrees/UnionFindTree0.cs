using System;
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
}
