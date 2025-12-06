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
	var operators = lines[lines.Length - 1].Replace(" ", "").ToCharArray();
	TimeIt("Part 1", () =>
	{
		var parsedInput = ParseDay1(lines);
		var grandTotal = 0L;
		for (var k = 0; k < operators.Length; k++)
		{
			var op = operators[k];
			var calculation = parsedInput[0][k];
			for (var i = 1; i < parsedInput.Count; i++)
			{
				var b = parsedInput[i][k];
				calculation = CalculateInput(calculation, b, op);
			}
			grandTotal += calculation;
		}

		Console.WriteLine(grandTotal);
	});
	TimeIt("Part 2", () =>
		{
			var resultForColumn = BuildRightToLeft(lines.Take(lines.Length - 1).ToList(), operators.Length - 1);
			var grandTotal = 0L;
			foreach (var r in resultForColumn)
			{
				if (r.Item1.Any() == false) { continue; }
				var op = operators[r.Item2];
				var calculation = r.Item1[0];
				for (var i = 1; i < r.Item1.Count; i++)
				{
					var b = r.Item1[i];
					calculation = CalculateInput(calculation, b, op);
				}
				grandTotal += calculation;
			}
			Console.WriteLine(grandTotal);
		});

}
static List<(List<long>, int)> BuildRightToLeft(List<string> paddedNums, int opCount)
{
	var result = new List<(List<long>, int)>();
	var oc = opCount;
	var l = new List<long>();
	for (int j = paddedNums[0].Length - 1; j >= 0; j--)
	{
		var sb = new StringBuilder();
		for (int i = 0; i < paddedNums.Count; i++)
		{
			var current = paddedNums[i][j];
			if (current != ' ')
			{
				sb.Append(current);
				continue;
			}
			if (i == paddedNums.Count - 1 && sb.Length > 0)
			{
				l.Add(long.Parse(sb.ToString()));
				sb.Clear();
			}
			else if (i == paddedNums.Count - 1 && sb.Length == 0)
			{
				result.Add((l, oc));
				l = new List<long>();
				oc--;
			}
		}
		if (sb.Length > 0)
		{
			l.Add(long.Parse(sb.ToString()));
			sb.Clear();
		}

	}
	result.Add((l, opCount));

	return result;
}
List<List<long>> ParseDay1(string[] lines)
{
	List<List<long>> parsedInput = new List<List<long>>();
	foreach (var line in lines.Take(lines.Length - 1))
	{
		var l = new List<long>();
		var matches = Regex.Matches(line, @"\d+");
		foreach (Match m in matches)
		{
			l.Add(long.Parse(m.Value));
		}
		parsedInput.Add(l);
	}
	return parsedInput;
}
long CalculateInput(long a, long b, char op)
{
	switch (op)
	{
		case '+':
			return a + b;
		case '*':
			return a * b;
		case '/':
			return a / b;
		default:
			throw new NotImplementedException();
	}


}
void TimeIt(string label, Action action)
{
	var sw = System.Diagnostics.Stopwatch.StartNew();
	action();
	sw.Stop();

	Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");
}