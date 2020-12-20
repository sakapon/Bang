﻿using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	public static class GraphConvert
	{
		public static List<int>[] UnweightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<int>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e[0]].Add(e[1]);
				if (!directed) map[e[1]].Add(e[0]);
			}
			return map;
		}

		public static List<WeightedEdge>[] WeightedEdgesToMap(int vertexesCount, WeightedEdge[] edges, bool directed)
		{
			var map = Array.ConvertAll(new bool[vertexesCount], _ => new List<WeightedEdge>());
			foreach (var e in edges)
			{
				// 入力チェックは省略。
				map[e.From].Add(e);
				if (!directed) map[e.To].Add(e.Reverse());
			}
			return map;
		}

		public static List<WeightedEdge>[] WeightedEdgesToMap(int vertexesCount, int[][] edges, bool directed)
		{
			return WeightedEdgesToMap(vertexesCount, Array.ConvertAll(edges, e => new WeightedEdge(e)), directed);
		}

		public static List<WeightedEdge>[] WeightedEdgesToMap<T>(int vertexesCount, WeightedEdge<T>[] edges, bool directed, Func<T, int> toId)
		{
			return WeightedEdgesToMap(vertexesCount, Array.ConvertAll(edges, e => e.Untype(toId)), directed);
		}
	}
}