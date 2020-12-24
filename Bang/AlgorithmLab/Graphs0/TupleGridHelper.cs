using System;
using System.Collections.Generic;

namespace AlgorithmLab.Graphs0
{
	public static class TupleGridHelper
	{
		public static T GetValue<T>(this T[,] a, (int i, int j) p) => a[p.i, p.j];
		public static void SetValue<T>(this T[,] a, (int i, int j) p, T value) => a[p.i, p.j] = value;
		public static T GetValue<T>(this T[][] a, (int i, int j) p) => a[p.i][p.j];
		public static void SetValue<T>(this T[][] a, (int i, int j) p, T value) => a[p.i][p.j] = value;
		public static char GetValue(this string[] s, (int i, int j) p) => s[p.i][p.j];

		public static (int i, int j) FindValue<T>(T[][] a, T value)
		{
			var ec = EqualityComparer<T>.Default;
			var (h, w) = (a.Length, a[0].Length);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (ec.Equals(a[i][j], value)) return (i, j);
			return (-1, -1);
		}

		public static (int i, int j) FindValue(string[] s, char value)
		{
			var (h, w) = (s.Length, s[0].Length);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (s[i][j] == value) return (i, j);
			return (-1, -1);
		}

		public static int ToHash((int i, int j) p, int width) => p.i * width + p.j;
		public static (int i, int j) FromHash(int hash, int width) => (hash / width, hash % width);
		public static Func<(int i, int j), int> CreateToHash(int width) => p => p.i * width + p.j;
		public static Func<int, (int i, int j)> CreateFromHash(int width) => hash => (hash / width, hash % width);

		public static (int i, int j)[] NextsByDelta { get; } = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
		public static (int i, int j)[] Nexts((int i, int j) v)
		{
			var (i, j) = v;
			return new[] { (i - 1, j), (i + 1, j), (i, j - 1), (i, j + 1) };
		}
	}
}
