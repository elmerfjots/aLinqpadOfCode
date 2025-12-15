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
		var tree = ParseNodesAndConnections(lines);
		var pathsCount = CountPaths(tree, "you", "out");
		Console.WriteLine(pathsCount);
	});

	TimeIt("Part 2", () =>
	{
		var tree = ParseNodesAndConnections(lines);
		var pathsCount = CountPaths(tree, "svr", "out","fft","dac");
		Console.WriteLine(pathsCount);
	});
}
static long CountPaths(
    Dictionary<string, List<string>> tree,
    string start,
    string end,
    string required1 = null,
    string required2 = null)
{
    var currentPath = new List<string>();
    var onPath = new HashSet<string>();
    long count = 0;

    Dfs(start, hasR1: false, hasR2: false);
    return count;

    void Dfs(string node, bool hasR1, bool hasR2)
    {
        currentPath.Add(node);
        onPath.Add(node);

        if (node == required1) hasR1 = true;
        if (node == required2) hasR2 = true;

        if (node == end)
        {
            bool r1Ok = required1 == null || hasR1;
            bool r2Ok = required2 == null || hasR2;

            if (r1Ok && r2Ok)
                count++;
        }
        else if (tree.TryGetValue(node, out var neighbors))
        {
            foreach (var next in neighbors)
            {
                if (!onPath.Contains(next))
                {
                    Dfs(next, hasR1, hasR2);
                }
            }
        }

        onPath.Remove(node);
        currentPath.RemoveAt(currentPath.Count - 1);
    }
}
Dictionary<string, List<string>> ParseNodesAndConnections(string[] lines)
{
	var tree = new Dictionary<string, List<string>>();
	for (var i = 0; i < lines.Length; i++)
	{
		var parts = lines[i].Split(' ').ToList();
		var key = string.Empty;
		foreach (var part in parts)
		{

			if (part.Contains(':'))
			{
				key = part.Replace(":", "").Trim();
				continue;
			}
			InsertInDicionaryTree(key, part, ref tree);
		}
	}
	return tree;
}
void InsertInDicionaryTree(string key, string part, ref Dictionary<string, List<string>> tree)
{
	if (tree.ContainsKey(key) == false)
	{
		tree.Add(key, new List<String>());
	}
	if (tree.ContainsKey(part) == false)
	{
		tree.Add(part, new List<String>());
	}
	tree[key].Add(part);
}
void TimeIt(string label, Action action)
{
	var sw = System.Diagnostics.Stopwatch.StartNew();
	action();
	sw.Stop();

	Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");
}