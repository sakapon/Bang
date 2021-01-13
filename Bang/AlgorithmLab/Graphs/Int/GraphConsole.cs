using System;

namespace AlgorithmLab.Graphs.Int
{
	public static class GraphConsole
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);

		public static Edge[] ReadEdges(int count)
		{
			return Array.ConvertAll(new bool[count], _ => (Edge)Read());
		}

		public static UnweightedMap ReadUnweightedMap(int vertexesCount, int edgesCount, bool directed)
		{
			var map = new UnweightedMap(vertexesCount);
			for (int i = 0; i < edgesCount; ++i)
			{
				var e = Read();
				map.AddEdge(e[0], e[1], directed);
			}
			return map;
		}

		public static WeightedMap ReadWeightedMap(int vertexesCount, int edgesCount, bool directed)
		{
			var map = new WeightedMap(vertexesCount);
			for (int i = 0; i < edgesCount; ++i)
			{
				var e = Read();
				map.AddEdge(e[0], e[1], e[2], directed);
			}
			return map;
		}
	}
}
