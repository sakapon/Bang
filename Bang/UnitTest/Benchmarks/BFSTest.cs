using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Benchmarks
{
	[TestClass]
	public class BFSTest
	{
		const int n = 200000;
		static (int, int)[] Edges;

		[ClassInitialize]
		public static void Initialize(TestContext context)
		{
			var random = new Random();
			Edges = Enumerable.Range(0, 2 * n).Select(_ => (random.Next(n), random.Next(n))).ToArray();
		}

		[TestMethod]
		public void ForGraph()
		{
			var g = new ListUnweightedGraph(n);
			foreach (var (u, v) in Edges)
				g.AddEdge(u, v, true);
			var r = g.ShortestByBFS(0);
			Assert.AreEqual(0, r[0]);
		}

		[TestMethod]
		public void ForGraphE()
		{
			var g = new ListUnweightedGraphE(n);
			foreach (var (u, v) in Edges)
				g.AddEdge(u, v, true);
			var r = g.ShortestByBFS(0);
			Assert.AreEqual(0, r[0]);
		}
	}
}
