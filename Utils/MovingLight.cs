using EduGraf;
using EduGraf.Lighting;
using EduGraf.Tensors;

namespace Utils;

public class MovingLight
{
    private readonly Matrix4 _baseTransform;
    public readonly MovingLightInfo Light;
    public readonly VisualPart Sphere;
    public MovingLight(MovingLightInfo light,  Graphic graphic, Matrix4? baseTransform)
    {
        Light = light;
        _baseTransform = baseTransform ?? Matrix4.Identity;
        Sphere = CreateLightOrb(light, graphic);
        Translate(Vector3.Zero);
    }
    
    private static VisualPart CreateLightOrb(MovingLightInfo info, Graphic graphic)
    {
        var color = new Color3(
            ((info.Ambient.R + 0.5f) + (info.Diffuse.R + 0.5f) + 0.5f / 2.5f),
            ((info.Ambient.G + 0.5f) + (info.Diffuse.G + 0.5f) + 0.5f / 2.5f),
            ((info.Ambient.B + 0.5f) + (info.Diffuse.B + 0.5f) + 0.5f / 2.5f)
            );
        var light = new AmbientLight(color);
        var shading = graphic.CreateShading("light", new UniformMaterial(1, 1, light.Color), light);
        var positions = EduGraf.Shapes.Sphere.GetPositions(10, 10);
        var triangles = EduGraf.Shapes.Sphere.GetTriangles(10, 10);
        var geometry = EduGraf.Geometries.Geometry.Create(positions, triangles);
        
        return graphic.CreateVisual("sphere", graphic.CreateSurface(shading, geometry));
    }

    

    public void Translate(Vector3 vec)
    {
        Light.Position += vec;
        Sphere.Transform =  _baseTransform * Matrix4.Translation(Light.Position.Vector);
    }
}

public class MovingLightInfo (Point3 position, Color3 ambient, Color3 diffuse, Color3 specular)
{
    public Point3 Position { get; set; } = position;
    public Color3 Ambient { get; } = ambient;
    public Color3 Diffuse { get; } = diffuse;
    public Color3 Specular { get; } = specular;
}