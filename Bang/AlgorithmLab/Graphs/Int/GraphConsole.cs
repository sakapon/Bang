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
			return new UnweightedMap(vertexesCount, ReadEdges(edgesCount), directed);
		}

		public static WeightedMap ReadWeightedMap(int vertexesCount, int edgesCount, bool directed)
		{
			return new WeightedMap(vertexesCount, ReadEdges(edgesCount), directed);
		}
	}
}
