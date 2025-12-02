<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Numerics</Namespace>
</Query>

void Main()
{
	string fileName = Path.GetFileName(Util.CurrentQueryPath).Split('.')[0];
	string scriptDirectory = Path.GetDirectoryName(Util.CurrentQueryPath);
	var parentDirectory = Directory.GetParent(scriptDirectory).FullName;
	var inputDirectory = $@"{parentDirectory}\Inputs";
	var inputFile = $@"{inputDirectory}\{fileName}.txt";
	var lines = File.ReadAllLines(inputFile);

	var ranges = ParseRanges(lines[0]);

	var invalidIds = new List<long>();

	foreach (var (start, end) in ranges)
	{
		invalidIds.AddRange(FindInvalidIdsInRange(start, end));
	}
	Console.WriteLine($"Ids added: {invalidIds.Sum()}");
	HashSet<long> output = new HashSet<long>();
	
	foreach (var (start, end) in ranges)
	{
		FindInvalidIdsInRange2(start, end,output);
	}
	Console.WriteLine($"Ids added: {output.Sum()}");
}

static List<(long start, long end)> ParseRanges(string input)
{
	var result = new List<(long start, long end)>();

	var parts = input.Split(',');
	foreach (var part in parts)
	{
		var trimmed = part.Trim();
		var bounds = trimmed.Split('-');
		if (bounds.Length != 2)
			throw new FormatException($"Invalid range segment: {part}");

		long start = long.Parse(bounds[0], CultureInfo.InvariantCulture);
		long end = long.Parse(bounds[1], CultureInfo.InvariantCulture);

		if (start > end)
			(start, end) = (end, start); // just in case

		result.Add((start, end));
	}

	return result;
}
/// <summary>
/// Finds all invalid IDs in [start, end], where an invalid ID is BB (some digits repeated twice).
/// </summary>
static IEnumerable<long> FindInvalidIdsInRange(long start, long end)
{
	if (start > end)
		yield break;

	// Max digits of the numbers in this range
	int maxDigits = CountDigits(end);

	// A repeated-block number has 2k digits, so k <= maxDigits / 2
	for (int k = 1; k <= maxDigits / 2; k++)
	{
		long pow10k = Pow10(k);
		long m = pow10k + 1; // multiplier so that N = B * (10^k + 1)

		long bMin = Pow10(k - 1);     // smallest k-digit number (no leading zero)
		long bMax = pow10k - 1;       // largest k-digit number

		// Find B such that start <= B * m <= end
		long bLow = CeilDiv(start, m);
		long bHigh = end / m;

		long bStart = Math.Max(bLow, bMin);
		long bEnd = Math.Min(bHigh, bMax);

		if (bStart > bEnd)
			continue;

		for (long b = bStart; b <= bEnd; b++)
		{
			long n = b * m;
			yield return n;
		}
	}
}
static void FindInvalidIdsInRange2(long start, long end, HashSet<long> output)
{
	if (start > end)
		return;

	int maxDigits = CountDigits(end);

	// Total digit count D = r * m, with r >= 2, m >= 1
	for (int D = 2; D <= maxDigits; D++)
	{
		// r must be a divisor of D, and at least 2
		for (int r = 2; r <= D; r++)
		{
			if (D % r != 0)
				continue;

			int m = D / r; // digits in base block B
			
			long pow10m = Pow10(m);
			long pow10rm = Pow10(D);

			// M(m, r) = (10^{rm} - 1) / (10^m - 1)
			long denominator = pow10m - 1;
			long numerator = pow10rm - 1;
			long M = numerator / denominator; // exact integer

			long bMin = Pow10(m - 1); // smallest m-digit number (no leading zero)
			long bMax = pow10m - 1;   // largest m-digit number

			// We want N = B * M in [start, end]
			long bLow = CeilDiv(start, M);
			long bHigh = end / M;

			long bStart = Math.Max(bLow, bMin);
			long bEnd = Math.Min(bHigh, bMax);

			if (bStart > bEnd)
				continue;

			for (long B = bStart; B <= bEnd; B++)
			{
				long N = B * M;
				if (N >= start && N <= end)
				{
					output.Add(N);
				}
			}
		}
	}
}
static int CountDigits(long n)
{
	if (n == 0) return 1;
	int count = 0;
	long x = Math.Abs(n);
	while (x > 0)
	{
		x /= 10;
		count++;
	}
	return count;
}

static long Pow10(int exp)
{
	long result = 1;
	for (int i = 0; i < exp; i++)
	{
		result *= 10;
	}
	return result;
}
static long CeilDiv(long a, long b)
{
	if (a <= 0) return 0;
	return (a + b - 1) / b;
}
// Define other methods and classes here