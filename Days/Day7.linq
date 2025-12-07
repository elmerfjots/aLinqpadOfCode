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
		var cloneLines = lines.Clone();
		var dripIndexes = new HashSet<int>();
		var splitCnt = 0;
		for (var row = 0; row < lines.Length; row++)
		{
			for (var col = 0; col < lines[0].Length; col++)
			{
				var current = lines[row][col];
				if (current == 'S')
				{
					dripIndexes.Add((col));
					continue;
				}
				else if (current == '|')
				{
					continue;
				}
				else if (current == '^' && dripIndexes.Contains(col))
				{
					dripIndexes.Remove(col);
					dripIndexes.Add(col - 1);
					dripIndexes.Add(col + 1);
					splitCnt++;
				}
			}
			if (row > 0)
			{
				foreach (var d in dripIndexes)
				{
					var s = lines[row].ToCharArray();
					s[d] = '|';
					lines[row] = new string(s);
				}
			}

		}
		Console.WriteLine(splitCnt);
	});
	TimeIt("Part 2", () =>
		{
			int rows = lines.Length;
			int cols = lines[0].Length;

			var s = FindS(lines);

			long[,] memo = new long[rows, cols];
			for (int r = 0; r < rows; r++)
				for (int c = 0; c < cols; c++)
					memo[r, c] = -1;

			long n = Dp(s.row, s.col, memo, lines, rows, cols);
			Console.WriteLine(n);
		});

}

private static bool IsTrack(char ch)
{
    // Any cell that can have the particle on it
    return ch == '|' || ch == '^' || ch == 'S';
}

private long Dp(int r, int c, long[,] memo, string[] grid, int rows, int cols)
{
    char ch = grid[r][c];

    // Not on a valid track cell. no paths
    if (!IsTrack(ch))
        return 0;

    // Reached bottom row on a valid track cell. 1 timeline
    if (r == rows - 1)
        return 1;

    if (memo[r, c] != -1)
        return memo[r, c];

    long total = 0;
    int nr = r + 1;

    if (ch == 'S' || ch == '|')
    {
        // Straight down only
        if (IsTrack(grid[nr][c]))
        {
            total += Dp(nr, c, memo, grid, rows, cols);
        }
    }
    else if (ch == '^')
    {
        // Splitter: down-left and down-right (if they are track)
        if (c - 1 >= 0 && IsTrack(grid[nr][c - 1]))
        {
            total += Dp(nr, c - 1, memo, grid, rows, cols);
		}

		if (c + 1 < cols && IsTrack(grid[nr][c + 1]))
		{
			total += Dp(nr, c + 1, memo, grid, rows, cols);
		}
	}

	memo[r, c] = total;
	return total;
}

private (int row, int col) FindS(string[] lines)
{
	for (int r = 0; r < lines.Length; r++)
	{
		for (int c = 0; c < lines[r].Length; c++)
		{
			if (lines[r][c] == 'S')
			{
				return (r, c);
			}
		}
	}
	throw new InvalidOperationException("No start 'S' found in grid.");
}

void TimeIt(string label, Action action)
{
	var sw = System.Diagnostics.Stopwatch.StartNew();
	action();
	sw.Stop();

	Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");
}