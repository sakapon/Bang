using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	public static class ShortestPath
	{
		// 無向グリッド上での典型的な BFS の例
		// ev: 終点を指定しない場合、(-1, -1) など
		// 
		//var r = ShortestPath.Bfs(h * w,
		//	v => GridHelper.ToId(v, w),
		//	id => GridHelper.FromId(id, w),
		//	v => Array.FindAll(GridHelper.Nexts(v), v => s.GetByP(v) != '#'),
		//	sv, ev);

		public static Func<T, long> Bfs<T>(int vertexesCount, Func<T, int> toId, Func<int, T> fromId, Func<T, T[]> getNextVertexes, T startVertex, T endVertex)
		{
			var r = ShortestPathCore.Bfs(vertexesCount, id => Array.ConvertAll(getNextVertexes(fromId(id)), v => toId(v)), toId(startVertex), toId(endVertex));
			return v => r[toId(v)];
		}

		public static Func<T, long> Bfs<T>(int vertexesCount, Func<T, int> toId, T[][] edges, bool directed, T startVertex, T endVertex)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
			foreach (var e in edges)
			{
				var id0 = toId(e[0]);
				var id1 = toId(e[1]);
				map[id0].Add(id1);
				if (!directed) map[id1].Add(id0);
			}
			var r = ShortestPathCore.Bfs(vertexesCount, id => map[id], toId(startVertex), toId(endVertex));
			return v => r[toId(v)];
		}

		public static long[] Bfs(int vertexesCount, int[][] edges, bool directed, int startVertexId, int endVertexId = -1)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(e[1]);
				if (!directed) map[e[1]].Add(e[0]);
			}
			return ShortestPathCore.Bfs(vertexesCount, v => map[v], startVertexId, endVertexId);
		}
	}
}
