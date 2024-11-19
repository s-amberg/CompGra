using EduGraf.Tensors;

namespace Utils;

public class MovingLight(Point3 position, Color3 ambient, Color3 diffuse, Color3 specular)
{
    public Point3 Position = position;
    public Color3 Ambient = ambient;
    public Color3 Diffuse = diffuse;
    public Color3 Specular = specular;
    
}