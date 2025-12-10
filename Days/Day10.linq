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
	TimeIt("Part 1", () =>
	{

		var total = 0L;
		foreach (var line in lines)
		{

			var indicatorButtons = ParseMachineLine(line);

			var presses = MinPressesForMachine(indicatorButtons.indicatorString, indicatorButtons.buttonStrings, indicatorButtons.joltageRequirementString);

			if (presses > 0)
			{
				total += presses;
			}
		}
		Console.WriteLine(total);
	});

	TimeIt("Part 2", () =>
	{

		Console.WriteLine(0);
	});
}

long MinPressesForMachine(string indicator, string[] buttonStrings, string joltageRequirementString)
{
	var (targetState, n) = ParseIndicator(indicator);
	int startState = 0;

	if (targetState == startState)
		return 0;

	// Convert buttons to bitmasks
	var buttons = new List<int>();
	foreach (var b in buttonStrings)
	{
		buttons.Add(ParseButton(b));
	}

	int maxStates = 1 << n;
	int[] dist = new int[maxStates];
	for (int i = 0; i < maxStates; i++)
		dist[i] = int.MaxValue;

	var queue = new Queue<int>();
	dist[startState] = 0;
	queue.Enqueue(startState);

	while (queue.Count > 0)
	{
		int state = queue.Dequeue();

		if (state == targetState)
			return dist[state];

		// Try pressing every button once
		foreach (int mask in buttons)
		{
			// XOR toggles lights:
			// - if mask has bit i = 1, then light i flips (0 -> 1, 1 -> 0)
			// - if mask has bit i = 0, light i stays unchanged
			int nextState = state ^ mask;

			// If we haven't visited nextState yet, store distance and queue it
			if (dist[nextState] == int.MaxValue)
			{
				// dist[state] is the number of button presses needed to reach "state"
				// So dist[state] + 1 is the cost to press one more button
				dist[nextState] = dist[state] + 1;

				// BFS queue ensures shortest paths are processed first
				queue.Enqueue(nextState);
			}
		}
	}
	throw new InvalidOperationException("No solution found for machine.");
}

(int target, int length) ParseIndicator(string diagram)
{

	var s = diagram.Trim().Replace("[", "").Replace("]", "").Trim();

	int target = 0;

	for (var i = 0; i < s.Length; i++)
	{
		if (s[i] == '#')
		{
			target |= (1 << i);
		}
	}
	return (target, s.Length);
}
int ParseButton(string button)
{
	var s = button.Replace("(", "").Replace(")", "").Trim();

	int mask = 0;
	foreach (var p in s.Split(','))
	{
		var idx = int.Parse(p);
		mask |= (1 << idx); // note the cast to int for shift count
	}
	return mask;
}
(string indicatorString, string[] buttonStrings, string joltageRequirementString) ParseMachineLine(string line)
{
	var parts = line.Split(' ');
	var buttons = new List<string>();
	var indicator = string.Empty;
	var joltageRequirement = string.Empty;
	foreach (var p in parts)
	{
		if (p.StartsWith("[") && p.EndsWith("]"))
		{
			indicator = p;
		}
		else if (p.StartsWith("(") && p.EndsWith(")"))
		{
			buttons.Add(p);
		}
		else if (p.StartsWith("{") && p.EndsWith("}"))
		{
			joltageRequirement = p;
		}
	}
	return (indicator, buttons.ToArray(), joltageRequirement);
}
void TimeIt(string label, Action action)
{
	var sw = System.Diagnostics.Stopwatch.StartNew();
	action();
	sw.Stop();

	Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");
}