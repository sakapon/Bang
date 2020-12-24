using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs
{
	public static class GridHelper
	{
		public static T GetValue<T>(this T[,] a, Point p) => a[p.i, p.j];
		public static void SetValue<T>(this T[,] a, Point p, T value) => a[p.i, p.j] = value;
		public static T GetValue<T>(this T[][] a, Point p) => a[p.i][p.j];
		public static void SetValue<T>(this T[][] a, Point p, T value) => a[p.i][p.j] = value;
		public static char GetValue(this string[] s, Point p) => s[p.i][p.j];

		public static Point FindValue<T>(T[][] a, T value)
		{
			var ec = EqualityComparer<T>.Default;
			var (h, w) = (a.Length, a[0].Length);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (ec.Equals(a[i][j], value)) return new Point(i, j);
			return new Point(-1, -1);
		}

		public static Point FindValue(string[] s, char value)
		{
			var (h, w) = (s.Length, s[0].Length);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (s[i][j] == value) return new Point(i, j);
			return new Point(-1, -1);
		}

		public static int ToHash(Point p, int width) => p.i * width + p.j;
		public static Point FromHash(int hash, int width) => new Point(hash / width, hash % width);
		public static Func<Point, int> CreateToHash(int width) => p => p.i * width + p.j;
		public static Func<int, Point> CreateFromHash(int width) => hash => new Point(hash / width, hash % width);

		public static void EncloseGrid<T>(ref int height, ref int width, ref T[][] a, T value, int delta = 1)
		{
			var h = height + 2 * delta;
			var w = width + 2 * delta;

			var t = Array.ConvertAll(new bool[h], _ => Array.ConvertAll(new bool[w], __ => value));
			for (int i = 0; i < height; ++i)
				for (int j = 0; j < width; ++j)
					t[delta + i][delta + j] = a[i][j];
			(height, width, a) = (h, w, t);
		}

		public static void EncloseGrid(ref int height, ref int width, ref string[] s, char c = '#', int delta = 1)
		{
			var h = height + 2 * delta;
			var w = width + 2 * delta;
			var cw = new string(c, w);
			var cd = new string(c, delta);

			var t = new string[h];
			for (int i = 0; i < delta; ++i) t[delta + height + i] = t[i] = cw;
			for (int i = 0; i < height; ++i) t[delta + i] = cd + s[i] + cd;
			(height, width, s) = (h, w, t);
		}
	}
}
