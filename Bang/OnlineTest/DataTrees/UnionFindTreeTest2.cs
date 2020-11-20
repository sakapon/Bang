using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmLab.DataTrees;

namespace OnlineTest.DataTrees
{
	// Test: https://atcoder.jp/contests/abl/tasks/abl_c
	class UnionFindTreeTest2
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static void Main()
		{
			var h = Read();
			int n = h[0], m = h[1];
			var es = Array.ConvertAll(new bool[m], _ => Read());

			var uf = new QuickFind(n + 1);
			foreach (var e in es)
				uf.Unite(e[0], e[1]);
			Console.WriteLine(uf.GroupsCount - 2);
		}
	}
}
