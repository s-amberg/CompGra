using EduGraf;
using EduGraf.Lighting;
using EduGraf.Shapes;
using EduGraf.Tensors;
using Utils;

namespace DiffuseShaderApp;

public struct Square
{
    private Vector3[] Vertices { get; set; }
    float[][] Sides {
        get => [
            Geometry.Rectangle([Vertices[0], Vertices[1], Vertices[2], Vertices[3]]),
            Geometry.Rectangle([Vertices[0], Vertices[1], Vertices[5], Vertices[4]]),
            Geometry.Rectangle([Vertices[1], Vertices[2], Vertices[6], Vertices[5]]),
            Geometry.Rectangle([Vertices[2], Vertices[3], Vertices[7], Vertices[6]]),
            Geometry.Rectangle([Vertices[3], Vertices[0], Vertices[4], Vertices[7]]),
            Geometry.Rectangle([Vertices[4], Vertices[5], Vertices[6], Vertices[7]]),
        ];
    }
    public float[] Positions {
       get => Geometry.Unroll(Sides);
    }
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
    
}