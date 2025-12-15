<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

// Day 12 from AOC 2024 was helpful aswell as Day 18 AOC 2023.
// 2D Engine part from good old game dev practices, was also quite helpful
void Main()
{
	string fileName = Path.GetFileName(Util.CurrentQueryPath).Split('.')[0];
	string scriptDirectory = Path.GetDirectoryName(Util.CurrentQueryPath);
	var parentDirectory = Directory.GetParent(scriptDirectory).FullName;
	var inputDirectory = $@"{parentDirectory}\Inputs";
	var inputFile = $@"{inputDirectory}\{fileName}.txt";
	var lines = File.ReadAllLines(inputFile);
	//Brute - force over all vertex pairs, treat each pair as opposite corners of an
	// axis-aligned rectangle, compute its inclusive grid/tile area, and take the max.
	TimeIt("Part 1", () =>
	{
		var poly = ParsePolygonVertices(lines);

		long best = 0;
		for (int i = 0; i < poly.Count; i++)
		{
			for (int j = i + 1; j < poly.Count; j++)
			{
				var p1 = poly[i];
				var p2 = poly[j];
				best = Math.Max(best, GetGridArea(p1.X, p1.Y, p2.X, p2.Y));
			}
		}

		Console.WriteLine(best);
	});
	//   1) Coordinate-compress X and Y using all vertex coordinates and +1 offsets.
	//   2) For each compressed cell, test its center point with point-in-polygon; if inside, that
	//      cell contributes (cellWidth * cellHeight) to an "allowed area" grid.
	//   3) Build a 2D sum over allowed-cell-area so any rectangle’s allowed area can be tested.
	//   4) For each vertex-pair rectangle candidate, check allowed area; if it equals the rectangle’s area,
	//      then every covered cell is inside = rectangle fits.
	TimeIt("Part 2", () =>
	{
		var poly = ParsePolygonVertices(lines);

		// Compress coordinates so the polygon is representable as a small grid of variable-sized cells.
		var (xs, ys) = BuildCompressedAxes(poly);

		// Prefix-sum grid of "allowed area" (area of cells whose center lies inside polygon).
		var allowedPS = BuildAllowedPrefixSum(poly, xs, ys);

		long best = 0;

		// Enumerate candidate rectangles using pairs of vertices as opposite corners.
		for (int i = 0; i < poly.Count; i++)
		{
			for (int j = i + 1; j < poly.Count; j++)
			{
				var a = poly[i];
				var b = poly[j];

				if (a.X == b.X || a.Y == b.Y) continue;

				var r = Rect(a, b);
				long area = RectAreaTilesCount(r);
				if (area <= best) continue;

				// if "allowed area" == "rectangle area", rectangle is fully inside the polygon.
				if (RectFitsPolygonAllowed(r, xs, ys, allowedPS))
					best = area;
			}
		}

		Console.WriteLine(best);
	});
}

static (long[] xs, long[] ys) BuildCompressedAxes(IReadOnlyList<PointL> points)
{
	// Include both coordinate and coordinate+1 so compressed *cell boundaries* match integer tile boundaries.
	var xSet = new HashSet<long>();
	var ySet = new HashSet<long>();

	foreach (var p in points)
	{
		xSet.Add(p.X);
		xSet.Add(p.X + 1);
		ySet.Add(p.Y);
		ySet.Add(p.Y + 1);
	}

	var xs = xSet.ToArray();
	var ys = ySet.ToArray();
	Array.Sort(xs);
	Array.Sort(ys);
	return (xs, ys);
}

static int LowerBound(long[] a, long value)
{
	int lo = 0, hi = a.Length;
	while (lo < hi)
	{
		int mid = lo + (hi - lo) / 2;
		if (a[mid] < value) lo = mid + 1;
		else hi = mid;
	}
	return lo;
}

static bool IsPointInPolygon(double px, double py, IReadOnlyList<PointL> poly, bool includeBoundary = true)
{
	// Shoot a ray to +infinity on X and toggle "inside" every time we cross an edge.
	// Boundary handling is done explicitly for axis-aligned edges.
	bool inside = false;

	for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
	{
		var a = poly[j];
		var b = poly[i];

		if (includeBoundary)
		{
			if (a.X == b.X)
			{
				if (Math.Abs(px - a.X) < 1e-12 &&
					py >= Math.Min(a.Y, b.Y) &&
					py <= Math.Max(a.Y, b.Y))
					return true;
			}
			else if (a.Y == b.Y)
			{
				if (Math.Abs(py - a.Y) < 1e-12 &&
					px >= Math.Min(a.X, b.X) &&
					px <= Math.Max(a.X, b.X))
					return true;
			}
		}

		bool intersects =
			((a.Y > py) != (b.Y > py)) &&
			(px < (double)(b.X - a.X) * (py - a.Y) / (double)(b.Y - a.Y) + a.X);

		if (intersects) inside = !inside;
	}

	return inside;
}

static long[,] BuildAllowedPrefixSum(IReadOnlyList<PointL> polygon, long[] xs, long[] ys)
{
	// Build a weighted prefix sum where each compressed cell contributes its actual area if it's inside.
	int w = xs.Length - 1;
	int h = ys.Length - 1;

	var ps = new long[h + 1, w + 1];

	for (int y = 0; y < h; y++)
	{
		double cy = (ys[y] + ys[y + 1]) * 0.5;
		long cellH = ys[y + 1] - ys[y];

		long rowRunning = 0;
		for (int x = 0; x < w; x++)
		{
			double cx = (xs[x] + xs[x + 1]) * 0.5;
			long cellW = xs[x + 1] - xs[x];

			bool inside = IsPointInPolygon(cx, cy, polygon, true);
			long add = inside ? (cellW * cellH) : 0;

			rowRunning += add;
			ps[y + 1, x + 1] = ps[y, x + 1] + rowRunning;
		}
	}

	return ps;
}

static bool RectFitsPolygonAllowed((long minX, long maxX, long minY, long maxY) r, long[] xs, long[] ys, long[,] allowedPS)
{
	// Convert rectangle bounds into compressed grid indices, then query prefix sum.
	long area = RectAreaTilesCount(r);

	int x0 = LowerBound(xs, r.minX);
	int x1 = LowerBound(xs, r.maxX + 1);
	int y0 = LowerBound(ys, r.minY);
	int y1 = LowerBound(ys, r.maxY + 1);

	long allowedArea = QueryRect(allowedPS, x0, y0, x1, y1);
	return allowedArea == area;
}

static long QueryRect(long[,] ps, int x0, int y0, int x1, int y1)
{
	// Standard 2D prefix sum query over [x0,x1) x [y0,y1)
	return ps[y1, x1] - ps[y0, x1] - ps[y1, x0] + ps[y0, x0];
}

List<PointL> ParsePolygonVertices(string[] lines)
{
	var vertices = new List<PointL>();
	foreach (var line in lines)
	{
		var (x, y) = ParsePairs(line);
		vertices.Add(new PointL(x, y));
	}

	if (vertices.Count < 3)
		throw new ArgumentException("Polygon needs at least 3 vertices.");

	return vertices;
}

(long x, long y) ParsePairs(string line)
{
	var longs = line.Split(',').Select(s => long.Parse(s)).ToList();
	return (longs[0], longs[1]);
}

long GetGridArea(long x1, long y1, long x2, long y2)
{
	// Inclusive “tile/grid” area between corners.
	long width = Math.Abs(x2 - x1) + 1;
	long height = Math.Abs(y2 - y1) + 1;
	return width * height;
}

void TimeIt(string label, Action action)
{
	var sw = System.Diagnostics.Stopwatch.StartNew();
	action();
	sw.Stop();
	Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");
}

static (long minX, long maxX, long minY, long maxY) Rect(PointL a, PointL b)
{
	return (Math.Min(a.X, b.X), Math.Max(a.X, b.X),
			Math.Min(a.Y, b.Y), Math.Max(a.Y, b.Y));
}

static long RectAreaTilesCount((long minX, long maxX, long minY, long maxY) r)
{
	return (r.maxX - r.minX + 1) * (r.maxY - r.minY + 1);
}

public class PointL : IEquatable<PointL>
{
	public PointL(long x, long y) { X = x; Y = y; }
	public long X { get; set; }
	public long Y { get; set; }

	public bool Equals(PointL other)
		=> !(other is null) && X == other.X && Y == other.Y;

	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 31 + X.GetHashCode();
		hash = hash * 31 + Y.GetHashCode();
		return hash;
	}
}
