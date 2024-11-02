using EduGraf;
using EduGraf.Tensors;

namespace ClassLibrary1;

public class CelestialSystem
{
    private double _rotation = 0;
    public Visual Body { get; set; }

    public CelestialSystem(Visual body)
    {
        Body = body;
    }

    public double Rotate(double radians)
    {
        _rotation += radians;
        Console.WriteLine(_rotation);
        return _rotation;
    }
    
    public void Transform(Matrix4 matrix)
    {
        Body.Transform = matrix;
    }
    public void RotateY(double radians)
    {
        Body.RotateY((float)radians);
    }
}