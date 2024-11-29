using EduGraf;
using EduGraf.Lighting;
using EduGraf.Shapes;
using EduGraf.Tensors;

namespace Utils;

public class MovingLight
{
    private readonly Matrix4 _baseTransform;
    public readonly MovingLightInfo light;
    public readonly VisualPart sphere;
    public MovingLight(MovingLightInfo light, string name,  Graphic graphic, Matrix4? baseTransform)
    {
        this.light = light;
        _baseTransform = baseTransform ?? Matrix4.Identity;
        sphere = CreateLightOrb(light, graphic);
        this.Translate(Vector3.Zero);
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
        var positions = Sphere.GetPositions(10, 10);
        var triangles = Sphere.GetTriangles(10, 10);
        var geometry = EduGraf.Geometries.Geometry.Create(positions, triangles);
        
        return graphic.CreateVisual("sphere", graphic.CreateSurface(shading, geometry));
    }

    

    public void Translate(Vector3 vec)
    {
        light.Position += vec;
        sphere.Transform =  _baseTransform * Matrix4.Translation(light.Position.Vector);
    }
}

public class MovingLightInfo (Point3 Position, Color3 Ambient, Color3 Diffuse, Color3 Specular)
{
    public Point3 Position { get; set; } = Position;
    public Color3 Ambient { get; } = Ambient;
    public Color3 Diffuse { get; } = Diffuse;
    public Color3 Specular { get; } = Specular;
}