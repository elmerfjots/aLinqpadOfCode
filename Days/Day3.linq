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

	var totalJoltagePart1 = lines.Select(line => PickMaxDigits(line, 2)).Sum(x=>int.Parse(x));
	var totalJoltagePart2 = lines.Select(line => PickMaxDigits(line, 12)).Sum(x=>long.Parse(x));

	Console.WriteLine($"Part 1: {totalJoltagePart1}");
	Console.WriteLine($"Part 2: {totalJoltagePart2}");
}
string PickMaxDigits(string line, int digitsToPick)
{
	int lineLength = line.Length;
	int start = 0;
	var result = new StringBuilder(digitsToPick);

	for (int picked = 0; picked < digitsToPick; picked++)
	{
		int remainingToPick = digitsToPick - picked;

		// Last index we are allowed to consider for this digit
		int end = lineLength - remainingToPick;
		//ASCII/Unicode, the characters '0' through '9' are stored in increasing numeric order
		char bestDigit = '0';
		int bestIndex = start;
		for (int i = start; i <= end; i++)
		{
			char c = line[i];
			if (c > bestDigit)
			{
				bestDigit = c;
				bestIndex = i;
				if (bestDigit == '9')
				{
					break;
				}
			}
		}

		result.Append(bestDigit);
		start = bestIndex + 1;
	}

	return result.ToString();
}
// Define other methods and classes here