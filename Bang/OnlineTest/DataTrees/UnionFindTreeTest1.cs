using System;
using System.Collections.Generic;
using System.Linq;
using AlgorithmLab.DataTrees;

namespace OnlineTest.DataTrees
{
	// Test: https://atcoder.jp/contests/practice2/tasks/practice2_a
	class UnionFindTreeTest1
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static void Main()
		{
			Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false });
			var h = Read();
			int n = h[0], qc = h[1];
			var qs = Array.ConvertAll(new bool[qc], _ => Read());

			var uf = new UnionFindTree0(n);

			foreach (var q in qs)
				if (q[0] == 0)
					uf.Unite(q[1], q[2]);
				else
					Console.WriteLine(uf.AreUnited(q[1], q[2]) ? 1 : 0);
			Console.Out.Flush();
		}
	}
}
