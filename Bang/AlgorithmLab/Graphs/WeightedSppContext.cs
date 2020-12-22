using System;
using System.Collections.Generic;
using AlgorithmLab.DataTrees;

namespace AlgorithmLab.Graphs
{
	public class WeightedSppContext<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		SppFactory<TVertex> Factory;
		ReadOnlyMap<TVertex, WeightedEdge<TVertex>[]> NextEdgesMap;

		public WeightedSppContext(SppFactory<TVertex> factory, ReadOnlyMap<TVertex, WeightedEdge<TVertex>[]> nextEdgesMap)
		{
			Factory = factory;
			NextEdgesMap = nextEdgesMap;
		}

		public Map<TVertex, long> Costs { get; private set; }
		public Map<TVertex, WeightedEdge<TVertex>> InEdges { get; private set; }
		public TVertex StartVertex { get; private set; }
		public TVertex EndVertex { get; private set; }

		public WeightedSppContext<TVertex> Dijkstra(TVertex startVertex, TVertex endVertex)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;

			Costs = Factory.CreateMap(long.MaxValue);
			InEdges = Factory.CreateMap(new WeightedEdge<TVertex>(Factory.Invalid, Factory.Invalid, -1));
			var q = PriorityQueue<TVertex>.CreateWithKey(v => Costs[v]);
			Costs[startVertex] = 0;
			q.Push(startVertex);

			while (q.Any)
			{
				var (v, c) = q.Pop();
				if (TEquals(v, endVertex)) break;
				if (Costs[v] < c) continue;

				// IEnumerable<T>, List<T>, T[] の順に高速になります。
				foreach (var e in NextEdgesMap[v])
				{
					var (nv, nc) = (e.To, c + e.Cost);
					if (Costs[nv] <= nc) continue;
					Costs[nv] = nc;
					InEdges[nv] = e;
					q.Push(nv);
				}
			}
			return this;
		}

		public long this[TVertex vertex] => Costs[vertex];
		public bool IsConnected(TVertex vertex) => Costs[vertex] != long.MaxValue;

		public TVertex[] GetPathVertexes(TVertex endVertex)
		{
			if (InEdges == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<TVertex>();
			for (var v = endVertex; !TEquals(v, Factory.Invalid); v = InEdges[v].From)
				path.Push(v);
			return path.ToArray();
		}

		public WeightedEdge<TVertex>[] GetPathEdges(TVertex endVertex)
		{
			if (InEdges == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<WeightedEdge<TVertex>>();
			for (var e = InEdges[endVertex]; !TEquals(e.From, Factory.Invalid); e = InEdges[e.From])
				path.Push(e);
			return path.ToArray();
		}
	}
}
