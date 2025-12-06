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
			// How many numeric columns exist (max per row)
			//Behandle det som strenge så længe som muligt. Det skal paddes afhængig af hvilken side whitespace er på.
			var colCount = GetColCount(lines);
			var linesWithPadding = GetLinesWithPadding(lines, colCount);
			var resultForColumn = BuildRightToLeftCumulative(linesWithPadding.Take(linesWithPadding.Count - 1).ToList(), operators.Length - 1);
			var grandTotal = 0L;
			foreach (var r in resultForColumn)
			{
				try
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
				catch (Exception ex)
				{

				}

			}
			Console.WriteLine(grandTotal);
		});

}

List<string> GetLinesWithPadding(string[] lines, int colCount)
{
	var sb = new StringBuilder();
	var linesWithPadding = new List<string>();

	for (var k = 0; k < lines[0].Length; k++)
	{
		var notWhitespaceFound = false;
		for (var i = 1; i < lines.Length; i++)
		{
			var current = lines[i][k];
			
			if(notWhitespaceFound && current == ' ' || ){
				sb.Append('#');
			}
			if(current != ' '){
				notWhitespaceFound = true;
				sb.Append(current);
			}
			if(notWhitespaceFound == false && i == lines.Length-1){
				
			}
		}
	}
	
	
	
	
	foreach (var line in lines)
	{
		var cnt = colCount;
		for (var i = 0; i < line.Length; i++)
		{
			var current = line[i];
			if (cnt == 0)
			{
				sb.Append(' ');
				cnt = colCount;
				continue;
			}
			else
			{
				if (line[i] == ' ')
				{
					sb.Append("#");
				}
				else
				{
					sb.Append(current);
				}
			}
			cnt--;
		}
		linesWithPadding.Add(sb.ToString());
		sb.Clear();
	}
	return linesWithPadding;
}
static List<(List<long>, int)> BuildRightToLeftCumulative(List<string> paddedNums, int opCount)
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
			if (current == '#')
			{
				continue;
			}
			else if (current == ' ')
			{
				result.Add((l, oc));
				l = new List<long>();
				oc--;
			}
			else
			{
				sb.Append(current);
			}

		}
		if (sb.ToString() == string.Empty)
		{
			continue;
		}
		l.Add(long.Parse(sb.ToString()));
		sb.Clear();
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
int GetColCount(string[] lines)
{
	var rows = lines
		   .Select(line => Regex.Matches(line, @"\d+")
								.Cast<Match>()
								.Select(m => m.Value)
								.ToList())
		   .ToList();
	var max = 0;
	foreach (var row in rows)
	{
		if (row.Any() == false) { continue; }
		var m = row.Max(x => x.Length);
		max = m > max ? m : max;
	}
	return max;
}
void TimeIt(string label, Action action)
{
	var sw = System.Diagnostics.Stopwatch.StartNew();
	action();
	sw.Stop();

	Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");
}