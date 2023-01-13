using System;
using System.Collections.Generic;

namespace UnitTest.Benchmarks
{
	public class Edge
	{
		public int From, To;
		public Edge(int from, int to) { From = from; To = to; }
	}

	[System.Diagnostics.DebuggerDisplay(@"VertexesCount = {VertexesCount}")]
	public abstract class UnweightedGraphE
	{
		protected readonly int n;
		public int VertexesCount => n;
		public abstract List<Edge> GetEdges(int v);
		protected UnweightedGraphE(int n) { this.n = n; }
	}

	public class ListUnweightedGraphE : UnweightedGraphE
	{
		protected readonly List<Edge>[] map;
		public List<Edge>[] AdjacencyList => map;
		public override List<Edge> GetEdges(int v) => map[v];

		public ListUnweightedGraphE(int n) : base(n)
		{
			map = Array.ConvertAll(new bool[n], _ => new List<Edge>());
		}

		public void AddEdge(int from, int to, bool twoWay)
		{
			map[from].Add(new Edge(from, to));
			if (twoWay) map[to].Add(new Edge(to, from));
		}
	}

	public static class UnweightedPath1E
	{
		public static long[] ShortestByBFS(this UnweightedGraphE graph, int sv, int ev = -1)
		{
			var costs = Array.ConvertAll(new bool[graph.VertexesCount], _ => long.MaxValue);
			costs[sv] = 0;
			var q = new Queue<int>();
			q.Enqueue(sv);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (v == ev) return costs;
				var nc = costs[v] + 1;

				foreach (var e in graph.GetEdges(v))
				{
					var nv = e.To;
					if (costs[nv] <= nc) continue;
					costs[nv] = nc;
					q.Enqueue(nv);
				}
			}
			return costs;
		}
	}
}
