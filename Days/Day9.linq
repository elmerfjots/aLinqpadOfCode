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
	var coordinates = lines.Select(ParsePairs).ToList();
	TimeIt("Part 1", () =>
	{
		var areas = new HashSet<(long area, (long x1, long y1), (long x2, long y2))>();
		for (var i = 0; i < coordinates.Count(); i++)
		{
			var p1 = coordinates[i];
			for (var k = i + 1; k < coordinates.Count(); k++)
			{
				var p2 = coordinates[k];
				var area = GetGridArea(p1.x, p1.y, p2.x, p2.y);
				areas.Add((area, (p1.x, p1.y), (p2.x, p2.y)));
			}
		}
		var ordered = areas.OrderByDescending(x=>x.area);
		Console.WriteLine(ordered.First().area);
	});

	TimeIt("Part 2", () =>
	{

		Console.WriteLine(0);
	});
}
long GetGridArea(long x1, long y1, long x2, long y2)
{
	// Number of columns covered (inclusive)
	var width = Math.Abs(x2 - x1) + 1;
	// Number of rows covered (inclusive)
	var height = Math.Abs(y2 - y1) + 1;
	// Total number of grid points (or cells covered if counting inclusively)
	var area = width * height;
	return area;
}
(long x, long y) ParsePairs(string line)
{
	var longs = line.Split(',').Select(x => long.Parse(x)).ToList();
	return (longs[0], longs[1]);
}
void TimeIt(string label, Action action)
{
	var sw = System.Diagnostics.Stopwatch.StartNew();
	action();
	sw.Stop();

	Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");
}