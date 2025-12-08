internal class Program
{
    private static void Main(string[] args)
    {
        var input = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt")); // sorry

        var emptyLineIndex = Array.FindIndex(input, string.IsNullOrWhiteSpace);

        List<(long, long)> ranges = input
            .Take(emptyLineIndex)
            .Select(line =>
            {
                var parts = line.Split('-');
                return (long.Parse(parts[0]), long.Parse(parts[1]));
            })
            .ToList();

        var products = input
            .Skip(emptyLineIndex + 1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(long.Parse)
            .ToList();

        StageOne(ranges, products);

        StageTwo(ranges);
    }

    private static void StageOne(List<(long, long)> ranges, List<long> products)
    {
        int freshCount = 0;

        foreach (var item in products)
        {
            bool isFresh = ranges.Any(ranges => item >= ranges.Item1 && item <= ranges.Item2);

            if (isFresh)
                freshCount++;
        }

        Console.WriteLine(freshCount);
    }

    private static void StageTwo(List<(long, long)> ranges)
    {
        long freshCount = 0;

        ranges = ranges
            .OrderBy(r => r.Item1)
            .ToList();

        List<(long, long)> mergedRanges = new List<(long, long)>();

        foreach (var range in ranges)
        {
            if (mergedRanges.Count == 0)
            {
                mergedRanges.Add(range);
            }
            else
            {
                var lastRange = mergedRanges.Last();
                if (range.Item1 <= lastRange.Item2 + 1)
                {
                    mergedRanges[mergedRanges.Count - 1] = (lastRange.Item1, Math.Max(lastRange.Item2, range.Item2));
                }
                else
                {
                    mergedRanges.Add(range);
                }
            }
        }

        foreach (var range in mergedRanges)
        {
            freshCount += range.Item2 - range.Item1 + 1;
        }

        Console.WriteLine(freshCount);
    }
}
