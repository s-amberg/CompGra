using EduGraf;
using EduGraf.Cameras;
using EduGraf.Lighting;
using EduGraf.OpenGL;
using EduGraf.OpenGL.OpenTK;
using EduGraf.Tensors;
using EduGraf.UI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utils;

namespace DiffuseShaderApp;

class SquareRendering(GlGraphic graphic, Camera camera)
    : Rendering(graphic, new Color3(0.2f, 0, 0.2f))
{
    public MovingLight Light = new MovingLight(new(5, 0, 0), new(0.2f, 0.2f, 0.2f), new(0.9f, 0.9f, 0.9f), new(0.5f, 0.5f, 0.5f));
    
    private GlShading GetShading() {

        Image<Rgba32>? texture = TextureLoader.LoadImage("DiffuseShaderApp.resources.containerDiffuse.png");
        if (texture == null) throw new Exception("texture not found");
        GlTextureHandle textureHandle = Graphic.CreateTexture(texture) as GlTextureHandle;
        return new DiffuseColorTextureShading(graphic, textureHandle, Light, camera);
    }
    
    private VisualPart CreateSquare(Shading? shader = null)
    {
        var shading = shader;
        var geometry = Square.CreateWithUV(Point3.Origin, 2);
        
        var surface = graphic.CreateSurface(shading, geometry);
        var sphere = graphic.CreateVisual("sphere", surface);
        return sphere;
    }

    public override void OnLoad(Window window)
    {
        var geometry = CreateSquare(GetShading());
        Scene.Add(geometry);
    }
}

public class App
{
    
    private SquareRendering rendering;
    
    public void Start()
    {
        var graphic = new OpenTkGraphic();
        var camera = new OrbitCamera(new Point3(5, -0.5f, -4),  Point3.Origin);
        rendering = new SquareRendering(graphic, camera);
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