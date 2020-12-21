﻿using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	public class UnweightedSppContext<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		SppFactory<TVertex> Factory;
		ReadOnlyMap<TVertex, TVertex[]> NextVertexesMap;

		public UnweightedSppContext(SppFactory<TVertex> factory, ReadOnlyMap<TVertex, TVertex[]> nextVertexesMap)
		{
			Factory = factory;
			NextVertexesMap = nextVertexesMap;
		}

		public Map<TVertex, long> Costs { get; private set; }
		public Map<TVertex, TVertex> InVertexes { get; private set; }
		public TVertex StartVertex { get; private set; }
		public TVertex EndVertex { get; private set; }

		public void Bfs(TVertex startVertex, TVertex endVertex)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;

			Costs = Factory.CreateMap(long.MaxValue);
			InVertexes = Factory.CreateMap(Factory.Invalid);
			var q = new Queue<TVertex>();
			Costs[startVertex] = 0;
			q.Enqueue(startVertex);

			while (q.Count > 0)
			{
				var v = q.Dequeue();
				var nc = Costs[v] + 1;

				foreach (var nv in NextVertexesMap[v])
				{
					if (Costs[nv] <= nc) continue;
					Costs[nv] = nc;
					InVertexes[nv] = v;
					if (TEquals(nv, endVertex)) return;
					q.Enqueue(nv);
				}
			}
		}

		public long this[TVertex vertex] => Costs[vertex];
		public bool IsConnected(TVertex vertex) => Costs[vertex] != long.MaxValue;

		public TVertex[] GetPathVertexes(TVertex endVertex)
		{
			if (InVertexes == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<TVertex>();
			for (var v = endVertex; !TEquals(v, Factory.Invalid); v = InVertexes[v])
				path.Push(v);
			return path.ToArray();
		}
	}
}