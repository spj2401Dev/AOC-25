using System.Numerics;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System;

internal class Program
{
    private static void Main(string[] args)
    {
        var input = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt")); // sorry

        var data = input
            .Select(line => line.Select(c => short.Parse(c.ToString())).ToList())
            .ToList();

        StageOne(data);

        Console.Write("StageTwo result: ");
        StageTwo(data);

        Console.Write("StageTwo_Optimised2 result: ");
        StageTwo_Optimised(input, printResult: true);

        // one run is too fast
        const int iterations = 10000;
        
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
            StageTwo(data, printResult: false);
        sw.Stop();
        Console.WriteLine($"StageTwo (original): {sw.ElapsedMilliseconds}ms for {iterations} iterations");

        sw.Restart();
        for (int i = 0; i < iterations; i++)
            StageTwo_Optimised(input, printResult: false);
        sw.Stop();
        Console.WriteLine($"StageTwo_Optimised2: {sw.ElapsedMilliseconds}ms for {iterations} iterations");
    }

    private static void StageOne(List<List<short>> input)
    {
        int totalJoltage = 0;

        foreach (var bank in input)
        {
            short maxJoltage = 0;
            bool foundMax = false;
            
            for (int i = 0; i < bank.Count - 1 && !foundMax; i++)
            {
                for (int j = i + 1; j < bank.Count; j++)
                {
                    short joltage = (short)(bank[i] * 10 + bank[j]);
                    if (joltage > maxJoltage)
                    {
                        maxJoltage = joltage;
                        if (maxJoltage == 99)
                        {
                            foundMax = true;
                            break;
                        }
                    }
                }
            }
            totalJoltage += maxJoltage;
        }

        Console.WriteLine(totalJoltage);
    }

    private static void StageTwo(List<List<short>> input, bool printResult = true)
    {
        BigInteger totalJoltage = 0;

        foreach (var bank in input)
        {
            int batteriesNeeded = 12;
            var selected = new List<short>();
            int startIndex = 0;

            for (int digit = 0; digit < batteriesNeeded; digit++)
            {
                int digitsStillNeeded = batteriesNeeded - digit;
                int maxIndex = bank.Count - digitsStillNeeded;

                short maxDigit = bank[startIndex];
                int maxPos = startIndex;
                for (int i = startIndex; i <= maxIndex; i++)
                {
                    if (bank[i] > maxDigit)
                    {
                        maxDigit = bank[i];
                        maxPos = i;
                    }
                }

                selected.Add(maxDigit);
                startIndex = maxPos + 1;
            }

            BigInteger joltage = 0;
            foreach (var digit in selected)
            {
                joltage = joltage * 10 + digit;
            }

            totalJoltage += joltage;
        }

        if (printResult)
            Console.WriteLine(totalJoltage);
    }

    private static void StageTwo_Optimised(string[] input, bool printResult = true)
    {
        const int batteriesNeeded = 12;
        UInt128 totalJoltage = 0;

        int maxLen = 0;
        foreach (var line in input)
            if (line.Length > maxLen)
                maxLen = line.Length;

        byte[] suffixMax = new byte[maxLen];
        int[] suffixMaxPos = new int[maxLen];

        foreach (var line in input)
        {
            ReadOnlySpan<char> digits = line.AsSpan();
            int n = digits.Length;

            suffixMax[n - 1] = (byte)digits[n - 1];
            suffixMaxPos[n - 1] = n - 1;
            
            for (int i = n - 2; i >= 0; i--)
            {
                byte curr = (byte)digits[i];
                if (curr >= suffixMax[i + 1])
                {
                    suffixMax[i] = curr;
                    suffixMaxPos[i] = i;
                }
                else
                {
                    suffixMax[i] = suffixMax[i + 1];
                    suffixMaxPos[i] = suffixMaxPos[i + 1];
                }
            }

            long joltage = 0;
            int startIndex = 0;

            for (int digit = 0; digit < batteriesNeeded; digit++)
            {
                int digitsStillNeeded = batteriesNeeded - digit;
                int maxIndex = n - digitsStillNeeded;

                int suffixPos = suffixMaxPos[startIndex];
                int maxPos;
                byte maxChar;

                if (suffixPos <= maxIndex)
                {
                    maxPos = suffixPos;
                    maxChar = suffixMax[startIndex];
                }
                else
                {
                    maxPos = startIndex;
                    maxChar = (byte)digits[startIndex];
                    for (int i = startIndex + 1; i <= maxIndex; i++)
                    {
                        byte c = (byte)digits[i];
                        if (c > maxChar)
                        {
                            maxChar = c;
                            maxPos = i;
                            if (maxChar == '9') break;
                        }
                    }
                }

                joltage = joltage * 10 + (maxChar - '0');
                startIndex = maxPos + 1;
            }

            totalJoltage += (ulong)joltage;
        }

        if (printResult)
            Console.WriteLine(totalJoltage);
    }
}
