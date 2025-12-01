<Query Kind="Program" />

void Main()
{
	string fileName = Path.GetFileName(Util.CurrentQueryPath).Split('.')[0];
	string scriptDirectory = Path.GetDirectoryName(Util.CurrentQueryPath);
	var inputDirectory = $@"{scriptDirectory}\Inputs";
	var inputFile = $@"{inputDirectory}\{fileName}.txt";
	var inputLines = File.ReadAllLines(inputFile);
	
}
enum Direction{
	L,
	R
}
// Define other methods and classes here