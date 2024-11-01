using System.Reflection;
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
using SixLabors.ImageSharp.Processing;
using Utils;
using Geometry = EduGraf.Geometries.Geometry;

namespace Universe;

public class UniverseRendering : Rendering
{
    const int Scale = 2;
    private const float Velocity = 2 * MathF.PI / (60 * TimeSpan.TicksPerSecond);

    private Planet _earth;
    private Planet _moon;
    private Visual _earthSystem;
    private float _rotation;
    private GlGraphic _graphic;
    private Camera _camera;
    private long _lastUpdate = DateTime.Now.Ticks;
    private Parameters _parameters;
        
    public UniverseRendering(GlGraphic graphic, Camera camera, Parameters parameters)
        : base(graphic, new Color3(0, 0, 0))
    {
        _graphic = graphic;
        _camera = camera;
        _parameters = parameters;
    }
    
    public override void OnLoad(Window window)
    {
        _earth = GetEarth(_graphic, _camera);        
        _moon = GetMoon(_graphic, _camera);        
        var plane = GetPlane(_graphic, _camera);
       
        Scene.AddRange(
        [
            _earth._body,
          plane.Scale(10 * Scale)
        ]);
        
        _earthSystem = Graphic.CreateGroup("system");
        Scene.Add(_earthSystem);
        _earthSystem.Add(_earth._body);
        _earthSystem.Add(_moon._body);
        
    }
    
    private Planet GetEarth(GlGraphic graphic, Camera camera) {
        float center = (float)(PlanetCalc.CenterDistance(Constants.Moon.distance - Constants.Earth.distance, Constants.Earth, Constants.Moon) / Constants.Earth.diameter);

        var spherePosition = new Point3(-center, 0, 0);
        var transformation = Matrix4.Scale(Scale) * Matrix4.Translation(spherePosition.Vector);
        Image<Rgba32>? earthMap = TextureLoader.LoadImage("Universe.resources.earthmap.jpg");
        Image<Rgba32>? cities = TextureLoader.LoadImage("Universe.resources.cities.png");
        if (earthMap == null || cities == null) throw new Exception("texture not found");
        earthMap.Mutate(context => context.Flip(FlipMode.Vertical)); // switch orientation, if necessary
        cities.Mutate(context => context.Flip(FlipMode.Vertical)); // switch orientation, if necessary
        GlTextureHandle earthMapTexture = Graphic.CreateTexture(earthMap) as GlTextureHandle;
        GlTextureHandle citiesTexture = Graphic.CreateTexture(cities) as GlTextureHandle;
        return new Planet(graphic, camera, transformation, Constants.Earth, null, new ColorTextureShading(graphic, earthMapTexture, citiesTexture));
    }
    private Planet GetMoon(GlGraphic graphic, Camera camera)
    {
        float distance = (float)(Constants.Moon.distance / Constants.Earth.diameter);
        float center = (float)(PlanetCalc.CenterDistance(Constants.Moon.distance - Constants.Earth.distance, Constants.Earth, Constants.Moon) / Constants.Earth.diameter);
        var scale = (float)(Constants.Moon.diameter / Constants.Earth.diameter);
        var spherePosition = new Point3((distance - center), 0, 0);
        var transformation = Matrix4.Scale(scale * Scale) * Matrix4.Translation(spherePosition.Vector);

        var material = new UniformMaterial(0, 0, new Color3(0.2f, 0.2f, 0.2f));
        return new Planet(graphic, camera, transformation, Constants.Moon, material);
    }
        
    private static VisualPart GetPlane(Graphic graphic, Camera camera)
    {
        var color = new Color3(0.2f, 0.2f, 0.3f);
        var material = new UniformMaterial(0.5f, 0.1f, color);
        var light = new AmbientLight(new Color3(1, 1, 1));
        var shading = graphic.CreateShading("emissive", material, light);
        var positions = Patch.GetPositions(1, 1, (_, _) => -0.5f);
        var triangles = Patch.GetTriangles(1, 1);
        var geometry = Geometry.Create(positions, triangles);
        var surface = graphic.CreateSurface(shading, geometry);
        var plane = graphic.CreateVisual("plane", surface);
        return plane;
    }


    protected override void OnUpdateFrame(Window window)
    {
        base.OnUpdateFrame(window);

        var currentVelocity = _parameters.Speed * Velocity;
        
        long now = DateTime.Now.Ticks;
        var deltaAngle = currentVelocity * (now - _lastUpdate);
        _lastUpdate = now;
        _rotation += deltaAngle;

        _earth.Transform(Matrix4.RotationY(_rotation));

        _earthSystem?.RotateY((float)(1 / 27.3 * deltaAngle));
    }
    
}

public class Parameters
{
    public float Speed = 1;
    public Parameters(float speed)
    {
        Speed = speed;
    }
}

public class App
{
    
    private Parameters parameters = new Parameters(1);
    
    public void Start()
    {
        var graphic = new OpenTkGraphic();
        var camera = new OrbitCamera(new Point3(-10, 10, -10), Point3.Origin);
        using var window = new OpenTkWindow("Universe", graphic, 1580, 920, camera.Handle, OnEvent);
        var rendering = new UniverseRendering(graphic, camera, parameters);
        window.Show(rendering, camera);
    }

    private void OnEvent(InputEvent evt)
    {
        if (typeof(EduGraf.UI.KeyInputEvent) == evt.GetType())
        {
            var keyEvent = evt as KeyInputEvent;
            
            if (keyEvent.Key == ConsoleKey.UpArrow)
            {
                parameters.Speed++;
            }
            else if (keyEvent.Key == ConsoleKey.DownArrow)
            {
                parameters.Speed--;
            }
            else if (keyEvent.Key == ConsoleKey.Backspace)
            {
                parameters.Speed = 1;
            }
            
        }
    } 
}


public class Program
{
    public static void Main()
    {
        new App().Start();
    }
    
}