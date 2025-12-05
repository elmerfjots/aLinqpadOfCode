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
	var ranges = new List<(long, long)>();
	var ingredientIndex = 0;
	TimeIt("Init ranges", () =>
	{
		ranges = InitRanges(lines, out ingredientIndex);
	});
	TimeIt("Part 1", () =>
	{
		var freshCount = lines.Skip(ingredientIndex).Select(x => IsIngredientFresh(long.Parse(x), ranges)).Count(x => x == true);
		Console.WriteLine(freshCount);
	});
	TimeIt("Part 2", () =>
		{
			var mergedRanges = MergeRanges(ranges);
			Console.WriteLine(mergedRanges.Sum(x => x.Item2 - x.Item1 + 1));
		});

}
public List<(long, long)> MergeRanges(List<(long, long)> ranges)
{
	var rangesSorted = (from x in ranges orderby x.Item1 select x).ToList();
	var currentRange = rangesSorted[0];
	var mergedRanges = new List<(long, long)>();
	foreach (var next in rangesSorted.Skip(1))
	{
		if (next.Item1 <= currentRange.Item2)
		{
			currentRange.Item2 = Math.Max(currentRange.Item2, next.Item2);
		}
		else
		{
			mergedRanges.Add(currentRange);
			currentRange = next;
		}
	}
	mergedRanges.Add(currentRange);
	return mergedRanges;
}
public bool IsIngredientFresh(long ingredientId, List<(long, long)> ranges)
{
	foreach (var r in ranges)
	{
		if (r.Item1 <= ingredientId && ingredientId <= r.Item2)
		{
			return true;
		}
	}

	return false;
}
public List<(long, long)> InitRanges(string[] lines, out int ingredientIndex)
{
	var ranges = new List<(long, long)>();
	ingredientIndex = 0;
	foreach (var line in lines)
	{
		if (line.Contains("-"))
		{
			var s = line.Split('-');
			ranges.Add((long.Parse(s[0]), long.Parse(s[1])));
			ingredientIndex++;
		}
		else
		{
			ingredientIndex = ingredientIndex + 1;
			break;
		}
	}

	return ranges;
}
void TimeIt(string label, Action action)
{
	var sw = System.Diagnostics.Stopwatch.StartNew();
	action();
	sw.Stop();

	Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");
}