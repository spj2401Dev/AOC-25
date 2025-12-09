internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));
        
        var (splits, totalBeams) = SimulateTachyonBeams(lines);
        
        Console.WriteLine($"Part 1: {splits}");
        Console.WriteLine($"Part 2: {totalBeams}");
    }

    private static (long splits, long totalBeams) SimulateTachyonBeams(string[] lines)
    {
        int startPos = lines[0].IndexOf('S');
        
        // Dictionary tracking beam positions -> count of beams at that position
        var beams = new Dictionary<int, long> { [startPos] = 1 };
        
        long totalSplits = 0;
        
        foreach (var row in lines.Skip(1).Where(line => line.Contains('^')))
        {
            var nextBeams = new Dictionary<int, long>();
            
            foreach (var (position, count) in beams)
            {
                if (position >= 0 && position < row.Length && row[position] == '^')
                {
                    totalSplits++;
                    
                    int left = position - 1;
                    if (!nextBeams.TryAdd(left, count))
                        nextBeams[left] += count;
                    
                    int right = position + 1;
                    if (!nextBeams.TryAdd(right, count))
                        nextBeams[right] += count;
                }
                else
                {
                    if (!nextBeams.TryAdd(position, count))
                        nextBeams[position] += count;
                }
            }
            
            beams = nextBeams;
        }
        
        // for part 2
        long totalBeams = beams.Values.Sum();
        
        return (totalSplits, totalBeams);
    }
}