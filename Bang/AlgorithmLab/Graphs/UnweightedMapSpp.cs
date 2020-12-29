using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	public abstract class UnweightedMapSpp<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		public SppFactory<TVertex> Factory { get; }
		public abstract ReadOnlyMap<TVertex, TVertex[]> NextVertexesMap { get; }

		protected UnweightedMapSpp(SppFactory<TVertex> factory)
		{
			Factory = factory;
		}

		public TVertex StartVertex { get; private set; }
		public TVertex EndVertex { get; private set; }
		public Map<TVertex, long> Costs { get; private set; }
		public Map<TVertex, TVertex> InVertexes { get; private set; }
		public long this[TVertex vertex] => Costs[vertex];
		public bool IsConnected(TVertex vertex) => Costs[vertex] != long.MaxValue;

		/// <summary>
		/// 幅優先探索により、始点から各頂点への最短経路を求めます。<br/>
		/// 辺のコストはすべて 1 として扱われます。
		/// </summary>
		/// <param name="startVertex">始点。</param>
		/// <param name="endVertex">終点。終点を指定しない場合、<c>Factory.Invalid</c>。</param>
		/// <returns>現在のオブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。
		/// </remarks>
		public UnweightedMapSpp<TVertex> Bfs(TVertex startVertex, TVertex endVertex)
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
					if (TEquals(nv, endVertex)) return this;
					q.Enqueue(nv);
				}
			}
			return this;
		}

		public TVertex[] GetPathVertexes(TVertex endVertex)
		{
			if (InVertexes == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<TVertex>();
			for (var v = endVertex; !TEquals(v, Factory.Invalid); v = InVertexes[v])
				path.Push(v);
			return path.ToArray();
		}
	}

	public class UnweightedFuncMapSpp<TVertex> : UnweightedMapSpp<TVertex>
	{
		public override ReadOnlyMap<TVertex, TVertex[]> NextVertexesMap { get; }

		public UnweightedFuncMapSpp(SppFactory<TVertex> factory, FuncReadOnlyMap<TVertex, TVertex[]> nextVertexesMap) : base(factory)
		{
			NextVertexesMap = nextVertexesMap;
		}
	}

	public class UnweightedListMapSpp<TVertex> : UnweightedMapSpp<TVertex>
	{
		ListMap<TVertex, TVertex> map;
		public override ReadOnlyMap<TVertex, TVertex[]> NextVertexesMap => map;

		public UnweightedListMapSpp(SppFactory<TVertex> factory, ListMap<TVertex, TVertex> nextVertexesMap) : base(factory)
		{
			map = nextVertexesMap;
		}

		public void AddEdge(Edge<TVertex> edge, bool directed)
		{
			map.Add(edge.From, edge.To);
			if (!directed) map.Add(edge.To, edge.From);
		}

		public void AddEdge(TVertex from, TVertex to, bool directed)
		{
			map.Add(from, to);
			if (!directed) map.Add(to, from);
		}
	}
}
