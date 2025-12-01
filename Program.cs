internal class Program
{
    private static void Main(string[] args)
    {
        var input = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt")); // sorry

        StageOne(input);
        StageTwo(input);
    }

    private static void StageOne(string[] input)
    {
        int dialPosition = 50;
        int zeroCount = 0;

        foreach (var line in input)
        {
            char direction = line[0];
            int distance = int.Parse(line[1..]);

            if (direction == 'L')
            {
                dialPosition = (dialPosition - distance + 100) % 100;
            }
            else if (direction == 'R')
            {
                dialPosition = (dialPosition + distance) % 100;
            }

            if (dialPosition == 0)
            {
                zeroCount++;
            }
        }

        Console.WriteLine(zeroCount);
    }

    private static void StageTwo(string[] input)
    {
        int dialPosition = 50;
        int zeroCount = 0;

        foreach (var line in input)
        {
            char direction = line[0];
            int distance = int.Parse(line[1..]);

            if (direction == 'L')
            {
                int distanceToZero = dialPosition;

                if (distance >= distanceToZero && distanceToZero > 0)
                {
                    zeroCount++;
                    int remainingDistance = distance - distanceToZero;
                    zeroCount += remainingDistance / 100;
                }
                else if (distanceToZero == 0)
                {
                    zeroCount += distance / 100;
                }

                dialPosition = (dialPosition - distance % 100 + 100) % 100;
            }
            else if (direction == 'R')
            {
                int distanceToZero = 100 - dialPosition;

                if (distance >= distanceToZero && distanceToZero > 0)
                {
                    zeroCount++;
                    int remainingDistance = distance - distanceToZero;
                    zeroCount += remainingDistance / 100;
                }
                else if (distanceToZero == 0)
                {
                    zeroCount += distance / 100;
                }

                dialPosition = (dialPosition + distance % 100) % 100;
            }
        }

        Console.WriteLine(zeroCount);
    }
}