using EduGraf;
using EduGraf.Cameras;
using EduGraf.Geometries;
using EduGraf.Lighting;
using EduGraf.OpenGL.OpenTK;
using EduGraf.Tensors;
using EduGraf.UI;
using Utils;
using Geometry = EduGraf.Geometries.Geometry;

public class TriangleRendering(Graphic graphic, OrbitCamera camera) 
    : Rendering(graphic, new Color3(0.2f, 0, 0.2f))
{
    private static readonly Vertex[] Positions = [
            new (-1, 2, 1),
            new (-1, 2, 5),
            new (1, 2, 3), 
            new (0, 9, 3)
    ];

    public override void OnLoad(Window window)
    {
        var material = new UniformMaterial(0.5f, 0.1f, new Color3(1, 0, 0));
        var light = new AmbientLight(new Color3(1, 1, 1));
        var shading = Graphic.CreateShading("emissive", material, light);
        var geometry = Geometry.Create(Utils.Geometry.Tetraeder(Positions));
        var surface = Graphic.CreateSurface(shading, geometry);
        var triangle = Graphic.CreateVisual("x", surface);
        Scene.Add(triangle);
    }
}
public class CuboidRendering(Graphic graphic, OrbitCamera camera) 
    : Rendering(graphic, new Color3(0.2f, 0, 0.2f))
{
    public override void OnLoad(Window window)
    {
        var material = new UniformMaterial(0.5f, 0.1f, new Color3(1, 0, 0));
        var light = new AmbientLight(new Color3(1, 1, 1));
        var shading = graphic.CreateShading("emissive", material, light);
        var geometry = Geometry.Create(Utils.Geometry.Cuboid(Positions));
        var surface = Graphic.CreateSurface(shading, geometry);
        var triangle = Graphic.CreateVisual("", surface);
        Scene.Add(triangle);
    }

    private static readonly Vertex[] Positions = [
        new (1, 1, 1),
        new (3, 1, 1),
        new (3, 1, 3), 
        new (1, 1, 3),
            
        new (1, 3, 1),
        new (3, 3, 1),
        new (3, 3, 3), 
        new (1, 3, 3)
    ];
}

public class CircleRendering(Graphic graphic) 
    : Rendering(graphic, new Color3(0.2f, 0, 0.2f))
{
    public override void OnLoad(Window window)
    {
        var light = new AmbientLight(new(1, 1, 1));
        var material = new UniformMaterial(0, 0, new Color3(1, 0, 0));
        var shading = Graphic.CreateShading("emissive", material, light);
        var geometry = Geometry.Create(Utils.Geometry.Circle(2, (0, 0, 0), (2, 1, 1)));
        var surface = Graphic.CreateSurface(shading, geometry);
        var triangle = Graphic.CreateVisual("circle", surface);
        Scene.Add(triangle);
    }
}

static class Program
{
    static void Main()
    {
        var graphic = new OpenTkGraphic();
        var camera = new OrbitCamera(new Point3(1, -2, -2),  Point3.Origin);
        var rendering = new CircleRendering(graphic);
        using var window = new OpenTkWindow("U01", graphic, 700, 700, camera.Handle);
        window.Show(rendering, camera);
    }
}