using System;
using System.Collections.Generic;
using AlgorithmLab.DataTrees;

namespace AlgorithmLab.Graphs0
{
	/// <summary>
	/// 最短経路アルゴリズムの核となる機能を提供します。
	/// </summary>
	public static class ShortestPathCore2<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		public static UnweightedResult<TVertex> Bfs(HashSppFactory<TVertex> factory, Func<TVertex, TVertex[]> getNextVertexes, TVertex startVertex, TVertex endVertex)
		{
			var costs = factory.CreateMap(long.MaxValue);
			var inVertexes = factory.CreateMap(factory.Invalid);
			var q = new Queue<TVertex>();
			costs[startVertex] = 0;
			q.Enqueue(startVertex);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				var nc = costs[v] + 1;

				foreach (var nv in getNextVertexes(v))
				{
					if (costs[nv] <= nc) continue;
					costs[nv] = nc;
					inVertexes[nv] = v;
					if (TEquals(nv, endVertex)) return new UnweightedResult<TVertex>(startVertex, endVertex, costs, inVertexes);
					q.Enqueue(nv);
				}
			}
			return new UnweightedResult<TVertex>(startVertex, endVertex, costs, inVertexes);
		}

		public static WeightedResult<TVertex> Dijkstra(HashSppFactory<TVertex> factory, Func<TVertex, Edge<TVertex>[]> getNextEdges, TVertex startVertex, TVertex endVertex)
		{
			var costs = factory.CreateMap(long.MaxValue);
			var inEdges = factory.CreateMap(new Edge<TVertex>(factory.Invalid, factory.Invalid, long.MinValue));
			var q = PriorityQueue<TVertex>.CreateWithKey(v => costs[v]);
			costs[startVertex] = 0;
			q.Push(startVertex);

			while (q.Any)
			{
				var (v, c) = q.Pop();
				if (TEquals(v, endVertex)) break;
				if (costs[v] < c) continue;

				foreach (var e in getNextEdges(v))
				{
					var (nv, nc) = (e.To, c + e.Cost);
					if (costs[nv] <= nc) continue;
					costs[nv] = nc;
					inEdges[nv] = e;
					q.Push(nv);
				}
			}
			return new WeightedResult<TVertex>(startVertex, endVertex, costs, inEdges);
		}
	}

	public class UnweightedResult<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		public TVertex StartVertex { get; }
		public TVertex EndVertex { get; }
		public HashMap<TVertex, long> Costs { get; }
		public HashMap<TVertex, TVertex> InVertexes { get; }

		public long this[TVertex vertex] => Costs[vertex];
		public bool IsConnected(TVertex vertex) => Costs[vertex] != long.MaxValue;

		public UnweightedResult(TVertex startVertex, TVertex endVertex, HashMap<TVertex, long> costs, HashMap<TVertex, TVertex> inVertexes)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;
			Costs = costs;
			InVertexes = inVertexes;
		}

		public TVertex[] GetPathVertexes(TVertex endVertex)
		{
			var path = new Stack<TVertex>();
			for (var v = endVertex; ; v = InVertexes[v])
			{
				path.Push(v);
				if (TEquals(v, StartVertex)) break;
			}
			return path.ToArray();
		}
	}

	public class WeightedResult<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		public TVertex StartVertex { get; }
		public TVertex EndVertex { get; }
		public HashMap<TVertex, long> Costs { get; }
		public HashMap<TVertex, Edge<TVertex>> InEdges { get; }

		public long this[TVertex vertex] => Costs[vertex];
		public bool IsConnected(TVertex vertex) => Costs[vertex] != long.MaxValue;

		public WeightedResult(TVertex startVertex, TVertex endVertex, HashMap<TVertex, long> costs, HashMap<TVertex, Edge<TVertex>> inEdges)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;
			Costs = costs;
			InEdges = inEdges;
		}

		public TVertex[] GetPathVertexes(TVertex endVertex)
		{
			var path = new Stack<TVertex>();
			for (var v = endVertex; ; v = InEdges[v].From)
			{
				path.Push(v);
				if (TEquals(v, StartVertex)) break;
			}
			return path.ToArray();
		}

		public Edge<TVertex>[] GetPathEdges(TVertex endVertex)
		{
			var path = new Stack<Edge<TVertex>>();
			for (var e = InEdges[endVertex]; ; e = InEdges[e.From])
			{
				path.Push(e);
				if (TEquals(e.From, StartVertex)) break;
			}
			return path.ToArray();
		}
	}
}
