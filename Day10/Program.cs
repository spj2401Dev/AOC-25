using Microsoft.Z3;

internal class Program
{
    private static void Main(string[] args)
    {
        var machines = ParseInput(File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt")));
        
        Console.WriteLine(machines.Sum(m => SolvePart1(m)));
        Console.WriteLine(machines.Sum(m => SolvePart2(m)));
    }

    private static List<Machine> ParseInput(string[] input)
    {
        var machines = new List<Machine>();

        foreach (var line in input.Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            var parts = line.Split(' ');
            
            var pattern = parts[0][1..^1];
            var targetState = pattern.Select(c => c == '#').ToArray();

            var buttons = new List<HashSet<int>>();
            int idx = 1;
            while (idx < parts.Length && parts[idx].StartsWith('('))
            {
                buttons.Add(parts[idx][1..^1].Split(',').Select(int.Parse).ToHashSet());
                idx++;
            }

            var joltages = idx < parts.Length && parts[idx].StartsWith('{')
                ? parts[idx][1..^1].Split(',').Select(int.Parse).ToArray()
                : [];

            machines.Add(new Machine(targetState, buttons, joltages));
        }

        return machines;
    }

    private static int SolvePart1(Machine machine)
    {
        for (int pressCount = 0; pressCount <= machine.Buttons.Count; pressCount++)
        {
            if (TryFindSolution(machine, pressCount, 0, 0, new bool[machine.TargetState.Length]))
                return pressCount;
        }
        return 0;
    }

    private static bool TryFindSolution(Machine machine, int target, int start, int depth, bool[] state)
    {
        if (depth == target)
            return state.SequenceEqual(machine.TargetState);

        for (int i = start; i <= machine.Buttons.Count - (target - depth); i++)
        {
            ToggleLights(machine.Buttons[i], state);
            if (TryFindSolution(machine, target, i + 1, depth + 1, state))
                return true;
            ToggleLights(machine.Buttons[i], state);
        }
        return false;
    }

    private static void ToggleLights(HashSet<int> lights, bool[] state)
    {
        foreach (int idx in lights.Where(idx => idx < state.Length))
            state[idx] = !state[idx];
    }

    private static long SolvePart2(Machine machine)
    {
        using var ctx = new Context();
        using var opt = ctx.MkOptimize();
        
        var presses = Enumerable.Range(0, machine.Buttons.Count)
            .Select(i => ctx.MkIntConst($"p{i}"))
            .ToArray();
        
        foreach (var press in presses)
            opt.Add(ctx.MkGe(press, ctx.MkInt(0)));
        
        for (int i = 0; i < machine.Joltages.Length; i++)
        {
            var affecting = presses.Where((_, j) => machine.Buttons[j].Contains(i)).ToArray();
            if (affecting.Length > 0)
            {
                var sum = affecting.Length == 1 ? affecting[0] : ctx.MkAdd(affecting);
                opt.Add(ctx.MkEq(sum, ctx.MkInt(machine.Joltages[i])));
            }
        }
        
        opt.MkMinimize(presses.Length == 1 ? presses[0] : ctx.MkAdd(presses));
        opt.Check();
        
        var model = opt.Model;
        return presses.Sum(p => ((IntNum)model.Evaluate(p, true)).Int64);
    }

    private record Machine(bool[] TargetState, List<HashSet<int>> Buttons, int[] Joltages);
}