using EduGraf;
using EduGraf.Tensors;

namespace ClassLibrary1;

public class CelestialSystem
{
    private double _rotation = 0;
    private double _orbitRotation = 0;
    public Visual Body { get; set; }

    public CelestialSystem(Visual body)
    {
        Body = body;
    }

    public float Rotate(double radians)
    {
        _rotation += radians;
        return (float) _rotation;
    }
    public float Orbit(double radians)
    {
        _orbitRotation += radians;
        return (float) _orbitRotation;
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