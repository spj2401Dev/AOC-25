using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var input = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt"));

        var problems = ParseProblems(input);

        var sw = Stopwatch.StartNew();
        StageOne(problems);
        sw.Stop();
        Console.WriteLine($"Stage 1 took: {sw.ElapsedMilliseconds}ms");

        sw.Restart();
        StageTwo(problems);
        sw.Stop();
        Console.WriteLine($"Stage 2 took: {sw.ElapsedMilliseconds}ms");
    }

    private static List<(string[] rawSegments, bool isMultiply)> ParseProblems(string[] lines)
    {
        var operatorLine = lines[^1];
        var numberLines = lines[..^1];

        int maxLen = Math.Max(operatorLine.Length, numberLines.Max(l => l.Length));
        var paddedLines = numberLines.Select(l => l.PadRight(maxLen)).ToArray();

        var operators = new List<(int pos, bool isMultiply)>();
        for (int i = 0; i < operatorLine.Length; i++)
        {
            if (operatorLine[i] == '+' || operatorLine[i] == '*')
            {
                operators.Add((i, operatorLine[i] == '*'));
            }
        }

        var problems = new List<(string[] rawSegments, bool isMultiply)>();

        for (int i = 0; i < operators.Count; i++)
        {
            int startCol = operators[i].pos;
            int endCol = (i + 1 < operators.Count) ? operators[i + 1].pos - 1 : maxLen;

            bool isMultiply = operators[i].isMultiply;

            var rawSegments = new string[paddedLines.Length];
            for (int row = 0; row < paddedLines.Length; row++)
            {
                rawSegments[row] = paddedLines[row].Substring(startCol, endCol - startCol);
            }

            problems.Add((rawSegments, isMultiply));
        }

        return problems;
    }

    private static void StageOne(List<(string[] rawSegments, bool isMultiply)> problems)
    {
        long total = 0;

        foreach (var (rawSegments, isMultiply) in problems)
        {
            if (isMultiply)
            {
                long product = 1;
                foreach (var s in rawSegments)
                {
                    var trimmed = s.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                        product *= long.Parse(trimmed);
                }
                total += product;
            }
            else
            {
                long sum = 0;
                foreach (var s in rawSegments)
                {
                    var trimmed = s.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                        sum += long.Parse(trimmed);
                }
                total += sum;
            }
        }

        Console.WriteLine($"Stage 1: {total}");
    }

    private static void StageTwo(List<(string[] rawSegments, bool isMultiply)> problems)
    {
        long total = 0;

        foreach (var (rawSegments, isMultiply) in problems)
        {
            var columnNumbers = TransposeAndParse(rawSegments);

            if (isMultiply)
            {
                long product = 1;
                foreach (var n in columnNumbers)
                    product *= n;
                total += product;
            }
            else
            {
                long sum = 0;
                foreach (var n in columnNumbers)
                    sum += n;
                total += sum;
            }
        }

        Console.WriteLine($"Stage 2: {total}");
    }

    private static long[] TransposeAndParse(string[] rawSegments)
    {
        if (rawSegments.Length == 0 || rawSegments[0].Length == 0) return [];

        int numCols = rawSegments[0].Length;
        var result = new List<long>();

        for (int col = 0; col < numCols; col++)
        {
            var chars = new char[rawSegments.Length];
            for (int row = 0; row < rawSegments.Length; row++)
            {
                chars[row] = rawSegments[row][col];
            }

            var numStr = new string(chars).Replace(" ", "");
            if (!string.IsNullOrEmpty(numStr))
            {
                result.Add(long.Parse(numStr));
            }
        }

        return result.ToArray();
    }
}