using System;
using System.Collections.Generic;
using AlgorithmLab.Graphs;

namespace OnlineTest.Graphs.SppTest
{
	// Test: https://atcoder.jp/contests/abc184/tasks/abc184_e
	class Bfs_ABC184_E
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (h, w) = Read2();
			var s = GraphConsole.ReadEnclosedGrid(ref h, ref w);

			var sv = GridHelper.FindValue(s, 'S');
			var ev = GridHelper.FindValue(s, 'G');

			var map = new MultiMap<char, Point>();
			for (int i = 0; i < h; i++)
				for (int j = 0; j < w; j++)
				{
					var c = s[i][j];
					if (!char.IsLower(c)) continue;
					map.Add(c, new Point(i, j));
				}

			var r = ShortestPath.WithGrid(h, w)
				.WithUnweighted(v =>
				{
					var nvs = Array.FindAll(v.Nexts(), nv => s.GetValue(nv) != '#');
					var c = s.GetValue(v);
					if (!map.ContainsKey(c)) return nvs;

					var nvl = new List<Point>(nvs);
					nvl.AddRange(map[c]);
					map.Remove(c);
					return nvl.ToArray();
				})
				.Bfs(sv, ev);
			Console.WriteLine(r.IsConnected(ev) ? r[ev] : -1);
		}
	}

	class MultiMap<TK, TV> : Dictionary<TK, List<TV>>
	{
		static List<TV> empty = new List<TV>();

		public void Add(TK key, TV v)
		{
			if (ContainsKey(key)) this[key].Add(v);
			else this[key] = new List<TV> { v };
		}

		public List<TV> ReadValues(TK key) => ContainsKey(key) ? this[key] : empty;
	}
}
