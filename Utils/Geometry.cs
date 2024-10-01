using SixLabors.ImageSharp;

namespace Utils;

public class Geometry
{

    public static T[] Unroll<T>(T[][] lists) {
        return lists.SelectMany(list => list).ToArray();
    }

    public static float[] Unroll(Vertex[] lists) {
        return lists.SelectMany(list => list.Unroll()).ToArray();
    }

    public static float[] Tetraeder(Vertex[] vertices) {
        if (vertices.Length != 4) throw new ArgumentException("exactly 4 verties required");

        Vertex[][] combinations = [
            [vertices[0], vertices[1], vertices[2]],
            [vertices[0], vertices[1], vertices[3]],
            [vertices[0], vertices[2], vertices[3]],
            [vertices[1], vertices[2], vertices[3]],
        ];
        return Unroll(Unroll(combinations));
    }

    public static float[] Rectangle(Vertex[] vertices) {
        if (vertices.Length != 4) throw new ArgumentException("exactly 4 verties required");

        Vertex[][] triangles = [
            [vertices[0], vertices[1], vertices[3]],
            [vertices[1], vertices[2], vertices[3]]
        ];
        return Unroll(Unroll(triangles));
    }

    //vertices must be in order of bot left, counterclockwise, top left, counterclockwise
    public static float[] Cuboid(Vertex[] vertices) {
        if (vertices.Length != 8) throw new ArgumentException("exactly 8 verties required");

        float[][] sides = [
            Rectangle([vertices[0], vertices[1], vertices[2], vertices[3]]),
            Rectangle([vertices[0], vertices[1], vertices[5], vertices[4]]),
            Rectangle([vertices[1], vertices[2], vertices[6], vertices[5]]),
            Rectangle([vertices[2], vertices[3], vertices[7], vertices[6]]),
            Rectangle([vertices[3], vertices[0], vertices[4], vertices[7]]),
            Rectangle([vertices[4], vertices[5], vertices[6], vertices[7]]),
        ];

        return Unroll(sides);
    }

    public static (float, float, float) PointAddition((float, float, float) point, (float, float, float) vector) {

        return (point.Item1 + vector.Item1, point.Item2 + vector.Item2, point.Item3);
    }
    public static (float, float, float) VectorRotate((float, float, float) point, (float, float, float) vector, (float, float, float) norm, float angle) {

        Math.Sin(angle) * 
        return (point.Item1 + vector.Item1, point.Item2 + vector.Item2, point.Item3);
    }

    public static float[] Circle(float radius, (float, float, float) center, (float, float, float) norm) {

        // y component always 0
        // x * norm[0] + z * norm[2] = 0;
        // x = Math.Sqrt(Math.Pow(radius) - Math.Pow(z, 2));

        // Math.Sqrt(Math.Pow(-norm.Item3 * scale, 2) + Math.Pow(norm.Item1 * scale, 2)) = radius;
        // (Math.Pow(-norm.Item3, 2) * Math.Pow(scale, 2) + Math.Pow(norm.Item1, 2) * Math.Pow(scale, 2)) = Math.Sqrt(radius);
        // Math.Pow(scale, 2) * ((Math.Pow(-norm.Item3, 2) + Math.Pow(norm.Item1, 2)) = Math.Sqrt(radius);
        float scale = (float)Math.Sqrt(Math.Sqrt(radius) / ((Math.Pow(-norm.Item3, 2) + Math.Pow(norm.Item1, 2))));

        (float, float, float) p0 = (-norm.Item3 * scale, 0, norm.Item1 * scale);
        (float, float, float) p1 = (0, -norm.Item3 * scale, norm.Item2 * scale);
        (float, float, float) p2 = (norm.Item3 * scale, 0, -norm.Item1 * scale);
        (float, float, float) p3 = (0, norm.Item3 * scale, -norm.Item2 * scale);

        (float, float, float)[] points = [
            PointAddition(center, p0), PointAddition(center, p1), PointAddition(center, p2), PointAddition(center, p3)
        ];
        Vertex[] rolled = points.Select<(float, float, float), Vertex>(p => 
            new Vertex(p.Item1, p.Item2, p.Item3)
        ).ToArray();
        Vertex centerV = new Vertex(center.Item1, center.Item2, center.Item3);

        IEnumerable<Vertex[]> combinations = [];
        
        for(int i = 0; i < rolled.Length; i++) {
            combinations = combinations.Append([centerV, rolled[i], rolled[(i + 1) % rolled.Length]]);
        };
        return Unroll(Unroll(combinations.ToArray()));
    }
}

public record Vertex
{
    private readonly float x, y, z = 0;

    public Vertex(float xi, float yi, float zi) {
        (x, y, z) = (xi, yi, zi);
    }
    public float[] Unroll() {
        return [x, y, z];
    }
}

public class Vector
{
    private readonly float x, y, z = 0;

    public Vector(float xi, float yi, float zi) {
        (x, y, z) = (xi, yi, zi);
    }
    public float[] Unroll() {
        return [x, y, z];
    }
    
    public static Vector Rotate(Vertex point, Vector vector, Vector norm, float angle) {

        vector * ((float)Math.Cos(angle)) + Math.Sin(angle) * 
            
        return (point.Item1 + vector.Item1, point.Item2 + vector.Item2, point.Item3);
    }
    
    public static Vector operator *(Vector a, float scalar) {

        return new(
            a.x * scalar,
            a.y * scalar,
            a.z * scalar
        );
    }
    
    public static Vector operator +(Vector a, Vector b) {

        return new(
            a.x + b.x,
            a.y + b.y,
            a.z + b.z
        );
    }
    public static Vector Crossproduct(Vector a, Vector b) {

        return new(
            a.x * b.x,
            a.y * b.y,
            a.z + b.z
        );
    }
}