using EduGraf;
using EduGraf.Cameras;
using EduGraf.OpenGL;
using EduGraf.OpenGL.OpenTK;
using EduGraf.Shapes;
using EduGraf.Tensors;
using EduGraf.UI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Utils;

namespace DiffuseShaderApp;

class SquareRendering(GlGraphic graphic)
    : Rendering(graphic, new Color3(0.2f, 0, 0.2f))
{
    public MovingLight Light;
    private UpdateShader _shader;

    public static Point3 SquarePos = new(10, 10, 10);
    public static Point3 InitialLightPos = SquarePos + new Vector3(3, 1, 3);
    
    private UpdateShader GetShading(MovingLightInfo light) {

        Image<Rgba32>? texture = TextureLoader.LoadImage("DiffuseShaderApp.resources.containerDiffuse.png");
        if (texture == null) throw new Exception("texture not found");
        GlTextureHandle textureHandle = Graphic.CreateTexture(texture) as GlTextureHandle;
        Image<Rgba32>? textureSpecular = TextureLoader.LoadImage("DiffuseShaderApp.resources.containerSpecular.png");
        if (textureSpecular == null) throw new Exception("texture not found");
        GlTextureHandle textureSpecularHandle = Graphic.CreateTexture(textureSpecular) as GlTextureHandle;
        return new SpecularColorTextureShading(graphic, textureHandle, textureSpecularHandle, light, 16);
    }
    
    private VisualPart CreateSquare(Shading? shader = null)
    {
        var shading = shader;
        var positions = Cube.Positions;
        var triangles = Cube.Triangles;
        var textureUvs = Cube.TextureUv;
        var geometry = EduGraf.Geometries.Geometry.CreateWithUv(positions, positions, textureUvs, triangles);
        var surface = graphic.CreateSurface(shading, geometry);
        var sphere = graphic.CreateVisual("sphere", surface);
        sphere.Scale(1);
        sphere.Translate(SquarePos.Vector);
        return sphere;
    }
    private MovingLight CreateLightOrb()
    {
        var light = new MovingLightInfo(InitialLightPos, new(0.1f, 0.1f, 0.1f), new(0.7f, 0.7f, 0.7f), new(0.9f, 0.9f, 0.9f));
        var movingLight = new MovingLight(light, "lightOrb", graphic, Matrix4.Scale(0.1f));
        return movingLight;
    }

    public override void OnLoad(Window window)
    {
        Light = CreateLightOrb();
        _shader = GetShading(Light.light);
        var box = CreateSquare(_shader);
        Scene.Add(box);
        Scene.Add(Light.sphere);
    }

    protected override void OnUpdateFrame(Window window)
    {
        _shader.OnUpdate();
        base.OnUpdateFrame(window);
    }
}

public class App(float velocity)
{
    
    private SquareRendering rendering;
    
    public void Start()
    {
        var graphic = new OpenTkGraphic();
        rendering = new SquareRendering(graphic);
        var camera = new OrbitCamera(SquareRendering.SquarePos + new Vector3(4.5f, 3, 5.5f),  SquareRendering.SquarePos + new Vector3(1, 1, 1));
        using var window = new OpenTkWindow("DiffuseShaderApp", graphic, 1200, 700, camera.Handle, OnEvent);
        window.Show(rendering, camera);
    }

    private Vector3 MovementDelta(ConsoleKey key)
    {
        Vector3 GetDirectionVector(ConsoleKey key) {
            switch (key)
            {
                case ConsoleKey.UpArrow: return Vector3.UnitZ;
                case ConsoleKey.Add: return Vector3.UnitZ;
                case ConsoleKey.DownArrow: return -Vector3.UnitZ;
                case ConsoleKey.Subtract: return -Vector3.UnitZ;
                case ConsoleKey.RightArrow: return Vector3.UnitX;
                case ConsoleKey.LeftArrow: return -Vector3.UnitX;
                case ConsoleKey.PageUp: return Vector3.UnitY;
                case ConsoleKey.PageDown: return -Vector3.UnitY;
                default: return Vector3.Zero;
            }
        }

        return GetDirectionVector(key) * velocity;
    }
    private void OnEvent(InputEvent evt)
    {
        if (typeof(EduGraf.UI.KeyInputEvent) == evt.GetType())
        {
            var keyEvent = evt as KeyInputEvent;
            
            if (keyEvent.Key == ConsoleKey.Backspace)
            {
                rendering.Light.Translate(new(-rendering.Light.light.Position.X, -rendering.Light.light.Position.Y, -rendering.Light.light.Position.Z));
            }
            else
            {
                var delta = MovementDelta(keyEvent.Key);
                rendering.Light.Translate(delta);
            }
        }
    } 
}

static class Program
{
    static void Main()
    {
        new App(0.25f).Start();
    }
}