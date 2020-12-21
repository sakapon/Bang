using System;
using System.Collections.Generic;

namespace OnlineTest.Graphs0.Spp0Test
{
	// Test: https://atcoder.jp/contests/atc002/tasks/abc007_3
	static class Bfs_ATC002_A0
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main() => Console.WriteLine(Solve());
		static object Solve()
		{
			var (h, w) = Read2();
			// 1-indexed に注意
			var sv = Read2();
			var ev = Read2();
			var s = ReadEnclosedGrid(ref h, ref w);

			var cs = Array.ConvertAll(new bool[h], _ => Array.ConvertAll(new bool[w], __ => int.MaxValue));
			var q = new Queue<(int i, int j)>();
			cs.SetByP(sv, 0);
			q.Enqueue(sv);

			while (q.TryDequeue(out var v))
			{
				var nc = cs.GetByP(v) + 1;
				foreach (var nv in Nexts(v))
				{
					// NextsByDelta を使う場合
					//var nv = (v.i + dv.i, v.j + dv.j);

					// 壁で囲まれているため、範囲チェックは不要です。
					if (s.GetByP(nv) == '#') continue;
					if (cs.GetByP(nv) <= nc) continue;
					cs.SetByP(nv, nc);
					if (nv == ev) return nc;
					q.Enqueue(nv);
				}
			}
			return -1;
		}

		static (int i, int j)[] NextsByDelta = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
		static (int i, int j)[] Nexts((int i, int j) v)
		{
			var (i, j) = v;
			return new[] { (i - 1, j), (i + 1, j), (i, j - 1), (i, j + 1) };
		}

		static string[] ReadEnclosedGrid(ref int h, ref int w, char c = '#')
		{
			var s = new string[h + 2];
			s[h + 1] = s[0] = new string(c, w += 2);
			for (int i = 1; i <= h; ++i) s[i] = c + Console.ReadLine() + c;
			h += 2;
			return s;
		}

		static (int i, int j) FindChar(string[] s, char c)
		{
			var (h, w) = (s.Length, s[0].Length);
			for (int i = 0; i < h; ++i)
				for (int j = 0; j < w; ++j)
					if (s[i][j] == c) return (i, j);
			return (-1, -1);
		}

		static T GetByP<T>(this T[][] a, (int i, int j) p) => a[p.i][p.j];
		static void SetByP<T>(this T[][] a, (int i, int j) p, T value) => a[p.i][p.j] = value;
		static char GetByP(this string[] s, (int i, int j) p) => s[p.i][p.j];
	}
}
