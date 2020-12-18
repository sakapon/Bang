using System;

namespace AlgorithmLab.Graphs
{
	public static class GridHelper
	{
		const char Road = '.';
		const char Wall = '#';

		// 2 次元配列に 2 次元インデックスでアクセスします。
		public static T GetByP<T>(this T[][] a, (int i, int j) p) => a[p.i][p.j];
		public static void SetByP<T>(this T[][] a, (int i, int j) p, T value) => a[p.i][p.j] = value;
		public static char GetByP(this string[] s, (int i, int j) p) => s[p.i][p.j];

		public static void EncloseGrid(ref int h, ref int w, ref string[] s, char c = Wall)
		{
			var t = new string[h + 2];
			t[h + 1] = t[0] = new string(c, w += 2);
			for (int i = 1; i <= h; ++i) t[i] = c + s[i - 1] + c;
			h += 2;
			s = t;
		}

		public static (int i, int j) FindChar(string[] s, char c)
		{
			var (h, w) = (s.Length, s[0].Length);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (s[i][j] == c) return (i, j);
			return (-1, -1);
		}

		public static int ToId((int i, int j) p, int w) => p.i * w + p.j;
		public static (int i, int j) FromId(int id, int w) => (id / w, id % w);

		public static readonly (int i, int j)[] NextsByDelta = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
		public static (int i, int j)[] Nexts((int i, int j) v)
		{
			var (i, j) = v;
			return new[] { (i - 1, j), (i + 1, j), (i, j - 1), (i, j + 1) };
		}
	}
}
