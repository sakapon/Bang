using System;

namespace AlgorithmLab.Graphs
{
	public static class GridHelper
	{
		public static void EncloseGrid(ref int height, ref int width, ref string[] s, char c = '#', int delta = 1)
		{
			var cl = new string(c, width += 2 * delta);
			var cd = new string(c, delta);

			var t = new string[height + 2 * delta];
			for (int i = 0; i < delta; ++i) t[delta + height + i] = t[i] = cl;
			for (int i = 0; i < height; ++i) t[delta + i] = cd + s[i] + cd;
			height = t.Length;
			s = t;
		}

		public static T GetValue<T>(this T[,] a, Point p) => a[p.i, p.j];
		public static void SetValue<T>(this T[,] a, Point p, T value) => a[p.i, p.j] = value;
		public static T GetValue<T>(this T[][] a, Point p) => a[p.i][p.j];
		public static void SetValue<T>(this T[][] a, Point p, T value) => a[p.i][p.j] = value;
		public static char GetValue(this string[] s, Point p) => s[p.i][p.j];

		public static Point FindChar(string[] s, char c)
		{
			var (h, w) = (s.Length, s[0].Length);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (s[i][j] == c) return new Point(i, j);
			return new Point(-1, -1);
		}

		public static int ToHash(Point p, int width) => p.i * width + p.j;
		public static Point FromHash(int hash, int width) => new Point(hash / width, hash % width);
		public static Func<Point, int> CreateToHash(int width) => p => p.i * width + p.j;
		public static Func<int, Point> CreateFromHash(int width) => hash => new Point(hash / width, hash % width);
	}
}
