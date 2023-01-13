using System;
using System.Collections.Generic;

namespace UnitTest.Benchmarks
{
	[System.Diagnostics.DebuggerDisplay(@"VertexesCount = {VertexesCount}")]
	public abstract class UnweightedGraph
	{
		protected readonly int n;
		public int VertexesCount => n;
		public abstract List<int> GetEdges(int v);
		protected UnweightedGraph(int n) { this.n = n; }
	}

	public class ListUnweightedGraph : UnweightedGraph
	{
		protected readonly List<int>[] map;
		public List<int>[] AdjacencyList => map;
		public override List<int> GetEdges(int v) => map[v];

		public ListUnweightedGraph(int n) : base(n)
		{
			map = Array.ConvertAll(new bool[n], _ => new List<int>());
		}

		public void AddEdge(int from, int to, bool twoWay)
		{
			map[from].Add(to);
			if (twoWay) map[to].Add(from);
		}
	}

	public static class UnweightedPath1
	{
		public static long[] ShortestByBFS(this UnweightedGraph graph, int sv, int ev = -1)
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

				foreach (var nv in graph.GetEdges(v))
				{
					if (costs[nv] <= nc) continue;
					costs[nv] = nc;
					q.Enqueue(nv);
				}
			}
			return costs;
		}
	}
}
