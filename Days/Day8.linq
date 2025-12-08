<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	string fileName = Path.GetFileName(Util.CurrentQueryPath).Split('.')[0];
	string scriptDirectory = Path.GetDirectoryName(Util.CurrentQueryPath);
	var parentDirectory = Directory.GetParent(scriptDirectory).FullName;
	var inputDirectory = $@"{parentDirectory}\Inputs";
	var inputFile = $@"{inputDirectory}\{fileName}.txt";
	var lines = File.ReadAllLines(inputFile);
	var coordinates = lines.Select(StringToPoint).ToList();
	TimeIt("Part 1", () =>
	{
		// Generate every pair (i,j) of points and compute squared distances.
		var edges = GenerateEdges(coordinates);

		// Sort edges from shortest -> longest (Kruskal-style).
		var edgesOrdered = edges.OrderBy(e => e.dist);

		// Initialize DSU, each point starts in its own circuit.
		var dsu = new DSU(coordinates.Count);

		// First 1000 shortest edges.
		const int stepsToConsider = 1000;
		int steps = 0;

		// Process edges in increasing order of distance.
		foreach (var e in edgesOrdered)
		{
			steps++;

			// Attempt to union the circuits containing endpoints a and b.
			// Whether the union actually merges or not, this counts as 1 step.
			dsu.Union(e.a, e.b);

			// Stop once we've "considered" the first 1000 shortest edges.
			if (steps == stepsToConsider)
				break;
		}

		// Build a dictionary: {root -> sizeOfcircuit}
		var sizes = dsu.GetCircuitSizes()
					   .Values
					   .OrderByDescending(s => s)
					   .ToList();

		long result = (long)sizes[0] * sizes[1] * sizes[2];
		Console.WriteLine(result);
	});

	TimeIt("Part 2", () =>
	{
		// Generate all edges and sort them shortest -> longest.
		var edges = GenerateEdges(coordinates);
		var edgesOrdered = edges.OrderBy(e => e.dist).ToList();

		// Start with each point in its own circuit.
		var dsu = new DSU(coordinates.Count);
		int circuits = coordinates.Count;    // how many separate circuits exist

		long result = 0;

		// Keep merging the closest pairs until all points become 1 circuit.
		foreach (var e in edgesOrdered)
		{
			// Only when we successfully merge two previously separated circuits
			// do we reduce the circuit count.
			if (dsu.Union(e.a, e.b))
			{
				circuits--;
				// When circuit count hits 1, this edge is the first one
				// that must make the entire graph connected.
				if (circuits == 1)
				{
					var p1 = coordinates[e.a];
					var p2 = coordinates[e.b];
					result = (long)p1.X * (long)p2.X;
					break;
				}
			}
		}

		Console.WriteLine(result);
	});
}
public class DSU
{
	private readonly int[] parent; // parent[i] = parent of node i
	private readonly int[] size;   // size[i] = size of the tree whose root is i

	public DSU(int n)
	{
		parent = new int[n];
		size = new int[n];

		// Initialize each element to be its own parent (root of itself)
		// and size 1 (a single-node circuit).
		for (int i = 0; i < n; i++)
		{
			parent[i] = i;
			size[i] = 1;
		}
	}

	// Find the root of x.
	// Path compression rewires each visited node to point directly to the root,
	// flattening the structure.
	public int Find(int x)
	{
		if (parent[x] != x)   
			parent[x] = Find(parent[x]); // recurse, and compress path
		return parent[x];
	}

	// Merge the sets containing a and b.
	// Returns true if a merge happened (circuits were separate),
	// false if they were already in the same set.
	public bool Union(int a, int b)
	{
		int ra = Find(a); // find root of a
		int rb = Find(b); // find root of b

		if (ra == rb)
			return false; // already in same circuit -> nothing changes

		// Union by size: attach smaller tree under bigger tree.
		if (size[ra] < size[rb])
		{
			(ra, rb) = (rb, ra); // swap roots so ra is always larger
		}

		// Attach rb under ra.
		parent[rb] = ra;

		// Update size of the resulting circuit.
		size[ra] += size[rb];

		return true;
	}

	// Return the size of the circuit that x belongs to.
	public int CircuitSize(int x) => size[Find(x)];

	// Build a dictionary of circuit sizes: root -> size.
	public Dictionary<int, int> GetCircuitSizes()
	{
		var result = new Dictionary<int, int>();

		for (int i = 0; i < parent.Length; i++)
		{
			int root = Find(i);        // find root
			if (!result.ContainsKey(root))
				result[root] = 0;
			result[root]++; // count point in its circuit
		}

		return result;
	}
}
// Build a list of edges between all pairs of points.
// Each edge: (distance, index a, index b)
List<(long dist, int a, int b)> GenerateEdges(List<Point> points)
{
	var edges = new List<(long dist, int a, int b)>();

	// Double loop over all unique pairs i < k.
	for (int i = 0; i < points.Count; i++)
	{
		for (int k = i + 1; k < points.Count; k++)
		{
			// Compute deltas between coordinates.
			long dx = (long)points[i].X - points[k].X;
			long dy = (long)points[i].Y - points[k].Y;
			long dz = (long)points[i].Z - points[k].Z;

			// Squared Euclidean distance.
			long dist2 = dx * dx + dy * dy + dz * dz;
			edges.Add((dist2, i, k));
		}
	}

	return edges;
}
// Convert a line into a Point object.
Point StringToPoint(string line)
{
	var sSplit = line.Split(',');
	return new Point(int.Parse(sSplit[0]), int.Parse(sSplit[1]), int.Parse(sSplit[2]));
}

class Point
{
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }

	public Point(int x, int y, int z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
	}
}
void TimeIt(string label, Action action)
{
	var sw = System.Diagnostics.Stopwatch.StartNew();
	action();
	sw.Stop();

	Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");
}