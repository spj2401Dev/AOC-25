using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var input = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt")); // sorry

        List<(int X, int Y, int Z)> values = input
            .Select(line => line.Split(','))
            .Select(parts => (int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])))
            .ToList();

        var sw = Stopwatch.StartNew();
        StageOne(values);
        sw.Stop();
        Console.WriteLine($"Stage 1 took: {sw.ElapsedMilliseconds}ms");

        sw.Restart();
        StageTwo(values);
        sw.Stop();
        Console.WriteLine($"Stage 2 took: {sw.ElapsedMilliseconds}ms");
    }

    /// <summary>
    /// Implements Kruskals algorithm for Minimum Spanning Forest using Union Find (Disjoint Set Union)
    /// with path compression and union by rank optimizations.
    /// https://en.wikipedia.org/wiki/Disjoint-set_data_structure
    /// 
    /// Algorithm complexity:
    /// - O(n²) for generating all pairwise distances (~500k edges for n≈1000 points)
    /// - O(E log E) for sorting edges by distance
    /// - O(E · α(n)) for union-find operations, where α is the inverse Ackermann function (nearly constant)
    /// </summary>
    private static void StageOne(List<(int X, int Y, int Z)> values)
    {
        // Generate pairwise squared Euclidean distances O(n²) combinations
        var distances = new List<(long Distance, int IndexA, int IndexB)>(500000);
        
        for (int i = 0; i < values.Count; i++)
        {
            for (int j = i + 1; j < values.Count; j++)
            {
                var dist = DistanceSquared(values[i], values[j]);
                distances.Add((dist, i, j));
            }
        }

        // Sort edges by distance (Kruskal's greedy approach)
        distances.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        var topPairs = distances.Take(1000).ToList();

        // Initialize Union Find (DSU) data structure
        var parent = new int[values.Count];
        var rank = new int[values.Count];
        
        for (int i = 0; i < values.Count; i++)
        {
            parent[i] = i;
            rank[i] = 0;
        }

        foreach (var (_, indexA, indexB) in topPairs)
        {
            Union(parent, rank, indexA, indexB);
        }

        var networkSizes = new Dictionary<int, int>();
        for (int i = 0; i < values.Count; i++)
        {
            int root = Find(parent, i);
            if (!networkSizes.ContainsKey(root))
                networkSizes[root] = 0;
            networkSizes[root]++;
        }

        var largestThree = networkSizes.Values
            .OrderByDescending(x => x)
            .Take(3)
            .ToList();

        long result = (long)largestThree[0] * largestThree[1] * largestThree[2];
        Console.WriteLine($"Stage 1: {result}");
    }

    /// <summary>
    /// Continues Kruskal's algorithm until all points form a single connected component.
    /// Returns the product of X coordinates of the final edge that completes the spanning tree.
    /// </summary>
    private static void StageTwo(List<(int X, int Y, int Z)> values)
    {
        // Generate pairwise squared Euclidean distances. O(n²) combinations
        var distances = new List<(long Distance, int IndexA, int IndexB)>(500000);
        
        for (int i = 0; i < values.Count; i++)
        {
            for (int j = i + 1; j < values.Count; j++)
            {
                var dist = DistanceSquared(values[i], values[j]);
                distances.Add((dist, i, j));
            }
        }

        // Sort edges by distance (Kruskal's greedy approach)
        distances.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        var parent = new int[values.Count];
        var rank = new int[values.Count];
        int componentCount = values.Count;
        
        for (int i = 0; i < values.Count; i++)
        {
            parent[i] = i;
            rank[i] = 0;
        }

        foreach (var (_, indexA, indexB) in distances)
        {
            int rootA = Find(parent, indexA);
            int rootB = Find(parent, indexB);
            
            if (rootA != rootB)
            {
                Union(parent, rank, indexA, indexB);
                componentCount--;
                
                if (componentCount == 1)
                {
                    long result = (long)values[indexA].X * values[indexB].X;
                    Console.WriteLine($"Stage 2: {result}");
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Calculates squared Euclidean distance between two 3D points.
    /// </summary>
    private static long DistanceSquared((int X, int Y, int Z) p, (int X, int Y, int Z) q)
    {
        long dx = p.X - q.X;
        long dy = p.Y - q.Y;
        long dz = p.Z - q.Z;
        return dx * dx + dy * dy + dz * dz;
    }

    private static int Find(int[] parent, int x)
    {
        if (parent[x] != x)
            parent[x] = Find(parent, parent[x]);
        return parent[x];
    }

    private static void Union(int[] parent, int[] rank, int x, int y)
    {
        int rootX = Find(parent, x);
        int rootY = Find(parent, y);

        if (rootX == rootY)
            return;

        if (rank[rootX] < rank[rootY])
            parent[rootX] = rootY;
        else if (rank[rootX] > rank[rootY])
            parent[rootY] = rootX;
        else
        {
            parent[rootY] = rootX;
            rank[rootX]++;
        }
    }
}