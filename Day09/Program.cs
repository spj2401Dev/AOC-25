internal class Program
{
    private static void Main(string[] args)
    {
        var input = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "input.txt")); // sorry

        var points = input
            .Select(line =>
            {
                var parts = line.Split(',');
                return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
            })
            .ToArray();

        StageOne(points);
        StageTwo(points);
    }

    private static void StageOne(Point[] points)
    {
        long maxArea = 0;

        for (int i = 0; i < points.Length; i++)
        {
            for (int j = i + 1; j < points.Length; j++)
            {
                long area = points[i].RectArea(points[j]);
                maxArea = Math.Max(maxArea, area);
            }
        }

        Console.WriteLine($"Part 1: {maxArea}");
    }

    private static void StageTwo(Point[] points)
    {
        var polygon = new Polygon(points);
        long maxArea = 0;

        for (int i = 0; i < points.Length; i++)
        {
            var p1 = points[i];

            for (int j = i + 1; j < points.Length; j++)
            {
                var p2 = points[j];

                // Testing for intersections
                double x1 = Math.Min(p1.X, p2.X) + 0.5;
                double x2 = Math.Max(p1.X, p2.X) - 0.5;
                double y1 = Math.Min(p1.Y, p2.Y) + 0.5;
                double y2 = Math.Max(p1.Y, p2.Y) - 0.5;

                var rect = new Polygon(new[]
                {
                    new Point(x1, y1),
                    new Point(x2, y1),
                    new Point(x2, y2),
                    new Point(x1, y2)
                });

                if (!rect.Edges.Any(edge => polygon.Intersects(edge)))
                {
                    long area = p1.RectArea(p2);
                    maxArea = Math.Max(maxArea, area);
                }
            }
        }

        Console.WriteLine($"Part 2: {maxArea}");
    }
}

class Point
{
    public double X { get; }
    public double Y { get; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public long RectArea(Point that)
    {
        long width = (long)Math.Abs(X - that.X) + 1;
        long height = (long)Math.Abs(Y - that.Y) + 1;
        return width * height;
    }
}

class Edge
{
    public bool Horizontal { get; }
    public Point P1 { get; }
    public Point P2 { get; }

    public Edge(Point p1, Point p2)
    {
        Horizontal = p1.Y == p2.Y;
        if (Horizontal)
        {
            P1 = p1.X < p2.X ? p1 : p2;
            P2 = p1.X < p2.X ? p2 : p1;
        }
        else
        {
            P1 = p1.Y < p2.Y ? p1 : p2;
            P2 = p1.Y < p2.Y ? p2 : p1;
        }
    }

    public bool Intersects(Edge that)
    {
        if (Horizontal == that.Horizontal)
        {
            return false;
        }

        var horizontal = Horizontal ? this : that;
        var vertical = Horizontal ? that : this;

        return vertical.P1.X > horizontal.P1.X && vertical.P1.X < horizontal.P2.X &&
               horizontal.P1.Y > vertical.P1.Y && horizontal.P1.Y < vertical.P2.Y;
    }
}

class Polygon
{
    public Point[] Points { get; }
    public Edge[] Edges { get; }

    public Polygon(Point[] points)
    {
        Points = points;
        Edges = new Edge[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            var next = points[(i + 1) % points.Length];
            Edges[i] = new Edge(points[i], next);
        }
    }

    public bool Intersects(Edge edge)
    {
        return Edges.Any(e => e.Intersects(edge));
    }
}