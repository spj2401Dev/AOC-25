internal class Program
{
    private static void Main(string[] args)
    {
        var input = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt")); // sorry

        var grid = new HashSet<(int row, int col)>();

        for (int row = 0; row < input.Length; row++)
        {
            for (int col = 0; col < input[row].Length; col++)
            {
                if (input[row][col] == '@')
                {
                    grid.Add((row, col));
                }
            }
        }

        StageOne(grid);
        StageTwo(grid);
    }

    private static IEnumerable<(int row, int col)> GetNeighbors((int row, int col) pos)
    {
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr != 0 || dc != 0)
                {
                    yield return (pos.row + dr, pos.col + dc);
                }
            }
        }
    }

    private static bool IsAccessible(HashSet<(int row, int col)> grid, (int row, int col) pos)
    {
        int neighborCount = GetNeighbors(pos).Count(neighbor => grid.Contains(neighbor));
        return neighborCount < 4;
    }

    private static void StageOne(HashSet<(int row, int col)> initialGrid)
    {
        int accessibleCount = initialGrid.Count(pos => IsAccessible(initialGrid, pos));
        Console.WriteLine(accessibleCount);
    }

    private static void StageTwo(HashSet<(int row, int col)> initialGrid)
    {
        var grid = new HashSet<(int row, int col)>(initialGrid);
        long total = 0;

        while (true)
        {
            var toRemove = grid.Where(pos => IsAccessible(grid, pos)).ToHashSet();
            
            if (toRemove.Count == 0)
                break;
            
            total += toRemove.Count;
            grid.ExceptWith(toRemove);
        }

        Console.WriteLine(total);
    }
}