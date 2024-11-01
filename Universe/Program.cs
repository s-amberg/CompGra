using System.Reflection;
using EduGraf;
using EduGraf.Cameras;
using EduGraf.Lighting;
using EduGraf.OpenGL.OpenTK;
using EduGraf.Shapes;
using EduGraf.Tensors;
using EduGraf.UI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Utils;
using Geometry = EduGraf.Geometries.Geometry;

namespace Universe;

public class UniverseRendering : Rendering
{
    const int Scale = 2;
    private const float Velocity = 2 * MathF.PI / (60 * TimeSpan.TicksPerSecond);

    private VisualPart _sphere;
    private Planet _planet;
    private float _rotation;
    private Point3 _spherePosition;
    private Graphic _graphic;
    private Camera _camera;
    private long _lastUpdate = DateTime.Now.Ticks;
        
    public UniverseRendering(Graphic graphic, Camera camera)
        : base(graphic, new Color3(0, 0, 0))
    {
        _graphic = graphic;
        _camera = camera;
    }
    
    public override void OnLoad(Window window)
    {
        _planet = GetEarth(_graphic, _camera);        
        _sphere = _planet._body;
        var plane = GetPlane(_graphic, _camera);
       
        Scene.AddRange(
        [
          _sphere,
          plane.Scale(10 * Scale)
        ]);
    }
    
    private Planet GetEarth(Graphic graphic, Camera camera) {
        _spherePosition = new Point3(-4, Scale, 0);
        var transformation = Matrix4.Scale(Scale) * Matrix4.Translation(_spherePosition.Vector);
        Assembly myAssembly = Assembly.GetExecutingAssembly();
        Stream myStream = myAssembly.GetManifestResourceStream( "Universe.resources.earthmap.jpg" );
        var bmp = Image.Load<Rgba32>( myStream );
        bmp.Mutate(context => context.Flip(FlipMode.Vertical)); // switch orientation, if necessary
        var texture = Graphic.CreateTexture(bmp);
        var material = new ColorTextureMaterial(0, 0, texture);
        return new Planet(graphic, camera, transformation, (float)Math.PI/8, (float)Math.PI/6, material);
    }
        
    private static VisualPart GetPlane(Graphic graphic, Camera camera)
    {
        var color = new Color3(0.2f, 0.2f, 0.3f);
        var material = new UniformMaterial(0.5f, 0.1f, color);
        var light = new AmbientLight(new Color3(1, 1, 1));
        var shading = graphic.CreateShading("emissive", material, light);
        var positions = Patch.GetPositions(1, 1, (_, _) => 0);
        var triangles = Patch.GetTriangles(1, 1);
        var geometry = Geometry.Create(positions, triangles);
        var surface = graphic.CreateSurface(shading, geometry);
        var plane = graphic.CreateVisual("plane", surface);
        return plane;
    }


    protected override void OnUpdateFrame(Window window)
    {
        base.OnUpdateFrame(window);


        long now = DateTime.Now.Ticks;
        var deltaAngle = Velocity * (now - _lastUpdate);
        _lastUpdate = now;
        _rotation += deltaAngle;
        Console.WriteLine("velocity: " + Velocity + " deltaAngle: " + deltaAngle + "rotation: " + _rotation);

        _planet.Transform(Matrix4.RotationY(_rotation));
    }
    
}


public static class Program
{
    public static void Main()
    {
        var graphic = new OpenTkGraphic();
        var camera = new OrbitCamera(new Point3(-10, 10, -10), Point3.Origin);
        using var window = new OpenTkWindow("Universe", graphic, 1580, 920, camera.Handle);
        var rendering = new UniverseRendering(graphic, camera);
        window.Show(rendering, camera);
    }
}