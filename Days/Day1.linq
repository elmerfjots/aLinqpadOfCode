<Query Kind="Program" />

void Main()
{
	string fileName = Path.GetFileName(Util.CurrentQueryPath).Split('.')[0];
	string scriptDirectory = Path.GetDirectoryName(Util.CurrentQueryPath);
	var parentDirectory = Directory.GetParent(scriptDirectory).FullName;
	var inputDirectory = $@"{parentDirectory}\Inputs";
	var inputFile = $@"{inputDirectory}\{fileName}.txt";
	
	var lines = File.ReadAllLines(inputFile);
	var codeDial = new CodeDial(50);
	foreach(var input in lines){
		var dir = input[0] == 'R' ? Direction.R : Direction.L;
		var s = input.Substring(1);
		codeDial.MoveDial(dir,Convert.ToInt32(s));
	}
	Console.WriteLine(codeDial.PointZeroCnt);
	Console.WriteLine(codeDial.ZeroEndCnt);
	
}
class CodeDial{
	private DialDigit[] Dial{get;set;}
	public DialDigit CurrentDigit{get;set;}
	public int PointZeroCnt{get;set;}
	public int ZeroEndCnt{get;set;}
	public CodeDial(int startIdx){
		Dial = new DialDigit[100];
		for(int i = 0;i<100;i++){
			var iLeft = i == 0 ? 99 : i-1;
			var iRight = i == 99 ? 0 : i+1;
			Dial[i] = new DialDigit{
				IndexLeft = iLeft,
				Index = i,
				IndexRight = iRight
			};
		}
		CurrentDigit = Dial[startIdx];
	}
	public void MoveDial(Direction d, int amount){
		var moveCount = 0;
		while(moveCount < amount){
			var nextIndex = d == Direction.L ? CurrentDigit.IndexLeft : CurrentDigit.IndexRight;
			CurrentDigit = Dial[nextIndex];
			if(CurrentDigit.Index == 0){PointZeroCnt++;}
			moveCount++;
		}
		if(CurrentDigit.Index == 0){ZeroEndCnt++;}
	}
}
class DialDigit{
	public int IndexLeft{get;set;}
	public int IndexRight{get;set;}
	public int Index{get;set;}
}
enum Direction{
	L,
	R
}
// Define other methods and classes here