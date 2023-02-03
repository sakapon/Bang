using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Data.Int;

namespace OnlineTest2022.Data.Int.UF
{
	// Test: https://atcoder.jp/contests/abc177/tasks/abc177_d
	class ABC177_D
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (n, m) = Read2();
			var es = Array.ConvertAll(new bool[m], _ => Read2());

			var uf = new UnionFind(n + 1);
			foreach (var (a, b) in es)
			{
				uf.Union(a, b);
			}
			return Enumerable.Range(1, n).Max(uf.GetSize);
		}
	}
}
