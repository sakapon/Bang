using System;
using System.Collections.Generic;
using Bang.Graphs.Int;
using Bang.Graphs.Int.SPPs;

namespace OnlineTest2022.Graphs.Int.SPPs.BFS
{
	// Test: https://atcoder.jp/contests/past202012-open/tasks/past202012_h
	class PAST202012_H
	{
		static int[] Read() => Array.ConvertAll(Console.ReadLine().Split(), int.Parse);
		static (int, int) Read2() { var a = Read(); return (a[0], a[1]); }
		static void Main()
		{
			var (h, w) = Read2();
			var (si, sj) = Read2();
			si--; sj--;
			var s = Array.ConvertAll(new bool[h], _ => Console.ReadLine());

			var g = new CharUnweightedGridH(s);
			var sv = g.ToVertexId(si, sj);
			var r = g.BFSTree(sv);

			var c = g.Cells;
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					if (c[i][j] != '#') c[i][j] = r[g.ToVertexId(i, j)].IsConnected ? 'o' : 'x';
				}
			}

			Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false });
			foreach (var cs in c)
			{
				Console.WriteLine(new string(cs));
			}
			Console.Out.Flush();
		}
	}

	public class CharUnweightedGridH : CharUnweightedGrid
	{
		public CharUnweightedGridH(string[] s, char wall = '#') : base(s, wall) { }

		const string Arrows = "><v^";
		public override List<int> GetEdges(int v)
		{
			var (i, j) = (v / w, v % w);
			var l = new List<int>();
			for (int k = 0; k < 4; k++)
			{
				var (di, dj) = NextsDelta[k];
				var (ni, nj) = (i + di, j + dj);
				if (0 <= ni && ni < h && 0 <= nj && nj < w && (s[ni][nj] == '.' || s[ni][nj] == Arrows[k])) l.Add(w * ni + nj);
			}
			return l;
		}
	}
}
