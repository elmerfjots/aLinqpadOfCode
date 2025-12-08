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
		Console.WriteLine(0);
	});

	TimeIt("Part 2", () =>
	{
		
		Console.WriteLine(0);
	});
}
void TimeIt(string label, Action action)
{
	var sw = System.Diagnostics.Stopwatch.StartNew();
	action();
	sw.Stop();

	Console.WriteLine($"{label} took {sw.ElapsedMilliseconds} ms");
}