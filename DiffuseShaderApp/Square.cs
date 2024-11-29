using EduGraf;
using EduGraf.Geometries;
using EduGraf.Lighting;
using EduGraf.Shapes;
using EduGraf.Tensors;
using Geometry = Utils.Geometry;

namespace DiffuseShaderApp;

public struct Square
{
    private Vector3[] Vertices { get; set; }
    float[][] Sides => [
        Geometry.Rectangle([Vertices[0], Vertices[1], Vertices[2], Vertices[3]]),
        Geometry.Rectangle([Vertices[0], Vertices[1], Vertices[5], Vertices[4]]),
        Geometry.Rectangle([Vertices[1], Vertices[2], Vertices[6], Vertices[5]]),
        Geometry.Rectangle([Vertices[2], Vertices[3], Vertices[7], Vertices[6]]),
        Geometry.Rectangle([Vertices[3], Vertices[0], Vertices[4], Vertices[7]]),
        Geometry.Rectangle([Vertices[4], Vertices[5], Vertices[6], Vertices[7]]),
    ];

    public float[] Positions => Geometry.Unroll([
            Vertices[0], Vertices[1], Vertices[2], Vertices[3],
            Vertices[0], Vertices[1], Vertices[5], Vertices[4],
            Vertices[1], Vertices[2], Vertices[6], Vertices[5],
            Vertices[2], Vertices[3], Vertices[7], Vertices[6],
            Vertices[3], Vertices[0], Vertices[4], Vertices[7],
            Vertices[4], Vertices[5], Vertices[6], Vertices[7],
    ]);

    public ushort[] Triangles => Enumerable.Range(0, 6).SelectMany(sideIndex => new ushort[6]
    {
        (ushort)(0 + 4 * sideIndex),
        (ushort)(1 + 4 * sideIndex),
        (ushort)(2 + 4 * sideIndex),
        (ushort)(2 + 4 * sideIndex),
        (ushort)(3 + 4 * sideIndex),
        (ushort)(0 + 4 * sideIndex)

    }).ToArray();
    
    public float[] TextureUv { get; } = Enumerable.Repeat<float[]>(new float[8]
    {
        0.0f,
        0.0f,
        1f,
        0.0f,
        1f,
        1f,
        0.0f,
        1f
    }, 6).SelectMany<float[], float>((Func<float[], IEnumerable<float>>) (side => (IEnumerable<float>) side)).ToArray<float>();

    public Square(Point3 position, int size)
    {
        Vertices = [
            position.Vector,
            new Vector3(position.X + size, position.Y, position.Z),
            new Vector3(position.X + size, position.Y, position.Z + size),
            new Vector3(position.X, position.Y, position.Z + size),
            
            new Vector3(position.X, position.Y + size, position.Z),
            new Vector3(position.X + size, position.Y + size, position.Z),
            new Vector3(position.X + size, position.Y + size, position.Z + size),
            new Vector3(position.X, position.Y + size, position.Z + size),
        ];
        
    }

    public static IGeometry CreateWithUV(Point3 position, int size)
    {
        var square = new Square(position, size);
        var positions = square.Positions;
        var triangles = square.Triangles;
        var textureUvs = square.TextureUv;
        return EduGraf.Geometries.Geometry.CreateWithUv(positions, positions, textureUvs, triangles);
    }
    
}