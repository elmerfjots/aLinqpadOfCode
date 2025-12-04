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
	var r = GetAccessible(lines);
	Console.WriteLine($"Part 1: {r.Item1}");
	var r2 = GetAccessible2(r.Item2,r.Item1);
	Console.WriteLine($"Part 2: {r2}");
}
public int GetAccessible2(string[] grid,int accessible){
	var lAccessible = accessible;
	var currentMap = grid;
	var removed = accessible;
	while(lAccessible > 0){
		var a = GetAccessible(currentMap);
		lAccessible = a.Item1;
		currentMap = a.Item2;
		removed += lAccessible;
	}
	return removed;
}
public (int,string[]) GetAccessible(string[] grid){
	string[] cMap = (string[])grid.Clone();
	var accessible = 0;
	var rows = grid.Length;
	var cols = grid[0].Length;
	
	for(int y = 0;y<grid.Length;y++){
		for(int x = 0;x<grid[0].Length;x++){
			var cVal = grid[y][x];
			
			if(cVal != '@'){
				continue;
			}
			var neighbors = 0;
			foreach(var dir in Directions){
				var ny = y+dir.Item1;
				var nx = x+dir.Item2;
				if (0 <= ny && ny < rows && 0 <= nx && nx < cols)
				{
					if (grid[ny][nx] == '@')
					{
						neighbors++;
					}
				}
			}
			if(neighbors < 4){
				accessible++;
				var t = cMap[y].ToCharArray();
				t[x] = 'x';
				cMap[y] = new string(t);
			}
		}
	}
	
	return (accessible,cMap);
}
public List<(int,int)> Directions = new List<(int,int)>{(0,-1),(0,1),(-1,-1),(1,-1),(1,0),(-1,0),(-1,1),(1,1)};