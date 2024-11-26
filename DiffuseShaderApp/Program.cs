using EduGraf;
using EduGraf.Cameras;
using EduGraf.Lighting;
using EduGraf.OpenGL;
using EduGraf.OpenGL.OpenTK;
using EduGraf.Shapes;
using EduGraf.Tensors;
using EduGraf.UI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utils;
using Geometry = EduGraf.Geometries.Geometry;

namespace DiffuseShaderApp;

class SquareRendering(GlGraphic graphic)
    : Rendering(graphic, new Color3(0.2f, 0, 0.2f))
{
    public MovingLight Light = new MovingLight(new(10, 10, -10), new(0.1f, 0.1f, 0.1f), new(0.5f, 0.5f, 0.5f), new(0.9f, 0.9f, 0.9f));
    
    private GlShading GetShading() {

        Image<Rgba32>? texture = TextureLoader.LoadImage("DiffuseShaderApp.resources.containerDiffuse.png");
        if (texture == null) throw new Exception("texture not found");
        GlTextureHandle textureHandle = Graphic.CreateTexture(texture) as GlTextureHandle;
        Image<Rgba32>? textureSpecular = TextureLoader.LoadImage("DiffuseShaderApp.resources.containerSpecular.png");
        if (textureSpecular == null) throw new Exception("texture not found");
        GlTextureHandle textureSpecularHandle = Graphic.CreateTexture(textureSpecular) as GlTextureHandle;
        return new SpecularColorTextureShading(graphic, textureHandle, textureSpecularHandle, Light);
    }
    
    private VisualPart CreateSquare(Shading? shader = null)
    {
        var shading = shader;
        var geometry = Square.CreateWithUV(Point3.Origin, 2);
        
        var surface = graphic.CreateSurface(shading, geometry);
        var sphere = graphic.CreateVisual("sphere", surface);
        return sphere;
    }
    private VisualPart CreateLightOrb()
    {
        var light = new AmbientLight(new Color3(1, 1, 1));
        var shading = Graphic.CreateShading("light", new UniformMaterial(1, 1, new(1, 1, 1)), light);
        var positions = Sphere.GetPositions(10, 10);
        var triangles = Sphere.GetTriangles(10, 10);
        var geometry = Geometry.Create(positions, triangles);
        
        var surface = graphic.CreateSurface(shading, geometry);
        var sphere = graphic.CreateVisual("sphere", surface);
        sphere.Translate(Light.Position.Vector);
        sphere.Scale(0.1f);
        return sphere;
    }

    public override void OnLoad(Window window)
    {
        var geometry = CreateSquare(GetShading());
        var light = CreateLightOrb();
        Scene.Add(geometry);
        Scene.Add(light);
    }
}

public class App
{
    
    private SquareRendering rendering;
    
    public void Start()
    {
        var graphic = new OpenTkGraphic();
        var camera = new OrbitCamera(new Point3(-3, 3, 3),  new(1, 1, 1));
        rendering = new SquareRendering(graphic);
        using var window = new OpenTkWindow("DiffuseShaderApp", graphic, 1200, 700, camera.Handle, OnEvent);
        window.Show(rendering, camera);
    }

    private Vector3 MovementDelta(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow: return Vector3.UnitZ;
            case ConsoleKey.DownArrow: return -Vector3.UnitZ;
            case ConsoleKey.RightArrow: return Vector3.UnitX;
            case ConsoleKey.LeftArrow: return -Vector3.UnitX;
            case ConsoleKey.PageUp: return Vector3.UnitY;
            case ConsoleKey.PageDown: return -Vector3.UnitY;
            default: return Vector3.Zero;
        }
    }
    private void OnEvent(InputEvent evt)
    {
        if (typeof(EduGraf.UI.KeyInputEvent) == evt.GetType())
        {
            var keyEvent = evt as KeyInputEvent;
            
            if (keyEvent.Key == ConsoleKey.Backspace)
            {
                rendering.Light.Position = Point3.Origin;
            }
            else
            {
                rendering.Light.Position = rendering.Light.Position + MovementDelta(keyEvent.Key);
            }
        }
    } 
}

static class Program
{
    static void Main()
    {
        new App().Start();
    }
}