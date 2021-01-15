using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs.Grid
{
	public static class GridHelper
	{
		public static Point[] GetPoints(int height, int width)
		{
			var ps = new List<Point>();
			for (int i = 0, j = 0; i < height;)
			{
				ps.Add(new Point(i, j));
				if (++j == width) { ++i; j = 0; }
			}
			return ps.ToArray();
		}

		public static Point FindValue<T>(GridMap<T> map, T value)
		{
			var ec = EqualityComparer<T>.Default;
			var (h, w) = (map.Height, map.Width);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (ec.Equals(map[i, j], value)) return new Point(i, j);
			return new Point(-1, -1);
		}

		// 負値を指定できます。
		public static void Enclose<T>(ref int height, ref int width, ref GridMap<T> map, T value, int delta = 1)
		{
			var (h, w) = (height + 2 * delta, width + 2 * delta);
			var (li, ri) = (Math.Max(0, -delta), Math.Min(height, height + delta));
			var (lj, rj) = (Math.Max(0, -delta), Math.Min(width, width + delta));

			var t = Array.ConvertAll(new bool[h], _ => Array.ConvertAll(new bool[w], __ => value));
			for (int i = li; i < ri; ++i)
				for (int j = lj; j < rj; ++j)
					t[delta + i][delta + j] = map[i, j];
			(height, width, map) = (h, w, new JaggedGridMap<T>(t));
		}
	}
}
