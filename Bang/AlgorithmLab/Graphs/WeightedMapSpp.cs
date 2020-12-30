using System;
using System.Collections.Generic;
using AlgorithmLab.DataTrees;

namespace AlgorithmLab.Graphs
{
	public abstract class WeightedMapSpp<TVertex>
	{
		static readonly Func<TVertex, TVertex, bool> TEquals = EqualityComparer<TVertex>.Default.Equals;

		public SppFactory<TVertex> Factory { get; }
		public abstract ReadOnlyMap<TVertex, Edge<TVertex>[]> NextEdgesMap { get; }

		protected WeightedMapSpp(SppFactory<TVertex> factory)
		{
			Factory = factory;
		}

		public TVertex StartVertex { get; private set; }
		public TVertex EndVertex { get; private set; }
		public Map<TVertex, long> Costs { get; private set; }
		public Map<TVertex, Edge<TVertex>> InEdges { get; private set; }
		public long this[TVertex vertex] => Costs[vertex];
		public bool IsConnected(TVertex vertex) => Costs[vertex] != long.MaxValue;

		/// <summary>
		/// Dijkstra 法により、始点から各頂点への最短経路を求めます。<br/>
		/// 辺のコストは非負でなければなりません。
		/// </summary>
		/// <param name="startVertex">始点。</param>
		/// <param name="endVertex">終点。終点を指定しない場合、<c>Factory.Invalid</c>。</param>
		/// <returns>現在のオブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。
		/// </remarks>
		public WeightedMapSpp<TVertex> Dijkstra(TVertex startVertex, TVertex endVertex)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;

			Costs = Factory.CreateMap(long.MaxValue);
			InEdges = Factory.CreateMap(new Edge<TVertex>(Factory.Invalid, Factory.Invalid, long.MinValue));
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

		/// <summary>
		/// 幅優先探索の拡張により、始点から各頂点への最短経路を求めます。<br/>
		/// 例えば <paramref name="m"/> = 3 のとき、012-BFS を表します。<br/>
		/// 辺のコストの範囲は [0, <paramref name="m"/>) です。
		/// </summary>
		/// <param name="m">辺のコストの候補となる数。</param>
		/// <param name="startVertex">始点。</param>
		/// <param name="endVertex">終点。終点を指定しない場合、<c>Factory.Invalid</c>。</param>
		/// <returns>現在のオブジェクト。</returns>
		/// <remarks>
		/// グラフの有向性、連結性、多重性、開閉を問いません。
		/// </remarks>
		public WeightedMapSpp<TVertex> BfsMod(int m, TVertex startVertex, TVertex endVertex)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;

			Costs = Factory.CreateMap(long.MaxValue);
			InEdges = Factory.CreateMap(new Edge<TVertex>(Factory.Invalid, Factory.Invalid, long.MinValue));
			var qs = Array.ConvertAll(new bool[m], _ => new Queue<TVertex>());
			Costs[startVertex] = 0;
			qs[0].Enqueue(startVertex);

			for (long c = 0; Array.Exists(qs, q => q.Count > 0); ++c)
			{
				var q = qs[c % m];
				while (q.Count > 0)
				{
					var v = q.Dequeue();
					if (TEquals(v, endVertex)) return this;
					if (Costs[v] < c) continue;

					foreach (var e in NextEdgesMap[v])
					{
						var (nv, nc) = (e.To, c + e.Cost);
						if (Costs[nv] <= nc) continue;
						Costs[nv] = nc;
						InEdges[nv] = e;
						qs[nc % m].Enqueue(nv);
					}
				}
			}
			return this;
		}

		public TVertex[] GetPathVertexes(TVertex endVertex)
		{
			if (InEdges == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<TVertex>();
			for (var v = endVertex; !TEquals(v, Factory.Invalid); v = InEdges[v].From)
				path.Push(v);
			return path.ToArray();
		}

		public Edge<TVertex>[] GetPathEdges(TVertex endVertex)
		{
			if (InEdges == null) throw new InvalidOperationException("No Result.");

			var path = new Stack<Edge<TVertex>>();
			for (var e = InEdges[endVertex]; !TEquals(e.From, Factory.Invalid); e = InEdges[e.From])
				path.Push(e);
			return path.ToArray();
		}
	}

	public class WeightedFuncMapSpp<TVertex> : WeightedMapSpp<TVertex>
	{
		public override ReadOnlyMap<TVertex, Edge<TVertex>[]> NextEdgesMap { get; }

		public WeightedFuncMapSpp(SppFactory<TVertex> factory, FuncReadOnlyMap<TVertex, Edge<TVertex>[]> nextEdgesMap) : base(factory)
		{
			NextEdgesMap = nextEdgesMap;
		}
	}

	public class WeightedListMapSpp<TVertex> : WeightedMapSpp<TVertex>
	{
		ListMap<TVertex, Edge<TVertex>> map;
		public override ReadOnlyMap<TVertex, Edge<TVertex>[]> NextEdgesMap => map;

		public WeightedListMapSpp(SppFactory<TVertex> factory, ListMap<TVertex, Edge<TVertex>> nextEdgesMap) : base(factory)
		{
			map = nextEdgesMap;
		}

		public void AddEdge(Edge<TVertex> edge, bool directed)
		{
			map.Add(edge.From, edge);
			if (!directed) map.Add(edge.To, edge.Reverse());
		}

		public void AddEdge(TVertex from, TVertex to, long cost, bool directed)
		{
			map.Add(from, new Edge<TVertex>(from, to, cost));
			if (!directed) map.Add(to, new Edge<TVertex>(to, from, cost));
		}
	}
}
