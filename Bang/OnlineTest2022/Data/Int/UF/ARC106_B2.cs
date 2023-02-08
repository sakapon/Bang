using System;
using System.Collections.Generic;
using System.Linq;
using Bang.Data.Int;

namespace OnlineTest2022.Data.Int.UF
{
	// Test: https://atcoder.jp/contests/arc106/tasks/arc106_b
	class ARC106_B2
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static long[] ReadL() => Array.ConvertAll(Console.ReadLine().Split(), long.Parse);
		static void Main() => Console.WriteLine(Solve() ? "Yes" : "No");
		static bool Solve()
		{
			var (n, m) = Read2();
			var a = ReadL();
			var b = ReadL();
			var es = Array.ConvertAll(new bool[m], _ => Read2());

			var uf = new UnionFindMap<(long av, long bv)>(a.Zip(b).ToArray(), (x, y) => (x.av + y.av, x.bv + y.bv));
			foreach (var (c, d) in es)
			{
				uf.Union(c - 1, d - 1);
			}

			return Enumerable.Range(0, n).Select(i => uf[i]).All(p => p.av == p.bv);
		}
	}
}
