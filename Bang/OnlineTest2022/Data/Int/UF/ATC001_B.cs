using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bang.Data.Int;

namespace OnlineTest2022.Data.Int.UF
{
	// Test: https://atcoder.jp/contests/atc001/tasks/unionfind_a
	// Test: https://atcoder.jp/contests/practice2/tasks/practice2_a
	class ATC001_B
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (n, qc) = Read2();
			var qs = Array.ConvertAll(new bool[qc], _ => Read());
			var sb = new StringBuilder();

			var uf = new UnionFind(n);

			foreach (var q in qs)
			{
				if (q[0] == 0)
				{
					uf.Union(q[1], q[2]);
				}
				else
				{
					var b = uf.AreSame(q[1], q[2]);
					sb.AppendLine(b ? "Yes" : "No");
					//sb.Append(b ? 1 : 0).AppendLine();
				}
			}
			Console.Write(sb);
		}
	}
}
