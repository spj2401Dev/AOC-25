using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var input = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt")); // sorry

        List<(long, long)> values = input[0]
            .Split(',')
            .Select(range => range.Split('-'))
            .Select(parts => (long.Parse(parts[0]), long.Parse(parts[1])))
            .ToList();

        StageOne(values);

        var sw = Stopwatch.StartNew();
        StageTwo(values);
        sw.Stop();

        Console.WriteLine($"Stage Two Time: {sw.ElapsedMilliseconds} ms");

        var swOptimised = Stopwatch.StartNew();
        StageTwo_Optimised(values);
        swOptimised.Stop();

        Console.WriteLine($"Stage Two Optimised Time: {swOptimised.ElapsedMilliseconds} ms");
    }

    private static void StageOne(List<(long, long)> values)
    {
        long invalididSum = 0;

        foreach (var (start, end) in values)
        {
            for (long id = start; id <= end; id++)
            {
                string idAsString = id.ToString();
                int lenght = idAsString.Length;

                var firstHalf = idAsString.Substring(0, lenght / 2);
                var secondHalf = idAsString.Substring(lenght / 2, lenght / 2);

                if (lenght % 2 == 0 && firstHalf == secondHalf)
                {
                    invalididSum += id;
                }
            }
        }

        Console.WriteLine(invalididSum);
    }

    private static void StageTwo(List<(long, long)> values)
    {
        long invalidSum = 0;

        foreach (var (start, end) in values)
        {
            for (long id = start; id <= end; id++)
            {
                string idAsString = id.ToString();
                int length = idAsString.Length;

                bool isInvalid = false;

                for (int patternLength = 1; patternLength <= length / 2; patternLength++)
                {
                    if (length % patternLength == 0)
                    {
                        string pattern = idAsString.Substring(0, patternLength);
                        int repetitions = length / patternLength;

                        string constructed = string.Concat(Enumerable.Repeat(pattern, repetitions));

                        if (constructed == idAsString)
                        {
                            isInvalid = true;
                            break;
                        }
                    }
                }
                if (isInvalid)
                {
                    invalidSum += id;
                }
            }
        }

        Console.WriteLine(invalidSum);
    }

    private static void StageTwo_Optimised(List<(long, long)> values)
    {
        long invalidSum = 0;

        var partialSums = new long[values.Count];
        
        Parallel.For(0, values.Count, i =>
        {
            var (start, end) = values[i];
            long localSum = 0;

            int minLength = start >= 10 ? (int)Math.Log10(start) + 1 : 1;
            int maxLength = end >= 10 ? (int)Math.Log10(end) + 1 : 1;

            for (int length = minLength; length <= maxLength; length++)
            {
                long rangeStart = Math.Max(start, (long)Math.Pow(10, length - 1));
                long rangeEnd = Math.Min(end, (long)Math.Pow(10, length) - 1);

                if (rangeStart > rangeEnd) continue;

                localSum += GenerateCandidatesForLength(rangeStart, rangeEnd, length);
            }

            partialSums[i] = localSum;
        });

        invalidSum = partialSums.Sum();
        Console.WriteLine(invalidSum);
    }

    private static long GenerateCandidatesForLength(long start, long end, int length)
    {
        long sum = 0;
        Span<char> buffer = stackalloc char[length];

        for (int patternLength = 1; patternLength <= length / 2; patternLength++)
        {
            if (length % patternLength != 0) continue;

            int repetitions = length / patternLength;
            long patternMin = (long)Math.Pow(10, patternLength - 1);
            long patternMax = (long)Math.Pow(10, patternLength) - 1;

            for (long pattern = patternMin; pattern <= patternMax; pattern++)
            {
                Span<char> patternStr = stackalloc char[patternLength];
                pattern.TryFormat(patternStr, out _);

                for (int rep = 0; rep < repetitions; rep++)
                {
                    patternStr.CopyTo(buffer.Slice(rep * patternLength, patternLength));
                }

                if (long.TryParse(buffer, out long candidate))
                {
                    if (candidate >= start && candidate <= end)
                    {
                        if (IsSmallestPattern(buffer, patternLength))
                        {
                            sum += candidate;
                        }
                    }
                }
            }
        }

        return sum;
    }

    private static bool IsSmallestPattern(Span<char> number, int patternLength)
    {
        int length = number.Length;
        
        for (int smallerPattern = 1; smallerPattern < patternLength; smallerPattern++)
        {
            if (length % smallerPattern == 0)
            {
                bool matches = true;
                for (int i = smallerPattern; i < length; i++)
                {
                    if (number[i] != number[i % smallerPattern])
                    {
                        matches = false;
                        break;
                    }
                }
                if (matches) return false;
            }
        }
        return true;
    }
}
