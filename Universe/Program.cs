using System;
using System.Reflection;
using ClassLibrary1;
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

    private Planet _sun;
    private Planet _mercury;
    private Planet _venus;
    private Planet _earth;
    private Planet _moon;
    private Planet _mars;
    private CelestialSystem _earthSystem;
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
    
    private Planet GetEarth(GlGraphic graphic, Camera camera) {
        float center = (float) PlanetCalc.CenterDistance(PlanetCalc.ToScale(Constants.Moon.distance - Constants.Earth.distance), Constants.Earth, Constants.Moon);

        var spherePosition = new Point3(-center / 10, 0, 0);
        var transformation = Matrix4.Scale(Scale) * Matrix4.Translation(spherePosition.Vector);
        Image<Rgba32>? earthMap = TextureLoader.LoadImage("Universe.resources.earthmap.jpg");
        Image<Rgba32>? cities = TextureLoader.LoadImage("Universe.resources.cities.png");
        if (earthMap == null || cities == null) throw new Exception("texture not found");
        earthMap.Mutate(context => context.Flip(FlipMode.Vertical)); // switch orientation, if necessary
        cities.Mutate(context => context.Flip(FlipMode.Vertical)); // switch orientation, if necessary
        GlTextureHandle earthMapTexture = Graphic.CreateTexture(earthMap) as GlTextureHandle;
        GlTextureHandle citiesTexture = Graphic.CreateTexture(cities) as GlTextureHandle;
        return new Planet(graphic, camera, transformation, Constants.Earth, null, new PlanetColorTextureShading(graphic, new(earthMapTexture), new(citiesTexture)));
    }
    
    private Planet GetMercury(GlGraphic graphic, Camera camera) {

        var spherePosition = new Point3(PlanetCalc.ToScaleDistance(Constants.Mercury.distance), 0, 0);
        var orbitTilt = Matrix4.RotationZ(float.DegreesToRadians((float)Constants.Mercury.orbitTilt));
        var transformation = Matrix4.Scale(PlanetCalc.ToScale(Constants.Mercury.diameter) * Scale) * orbitTilt * Matrix4.Translation(spherePosition.Vector);
        Image<Rgba32>? earthMap = TextureLoader.LoadImage("Universe.resources.mercurymap.jpg");
        if (earthMap == null) throw new Exception("texture not found");
        earthMap.Mutate(context => context.Flip(FlipMode.Vertical)); // switch orientation, if necessary
        GlTextureHandle earthMapTexture = Graphic.CreateTexture(earthMap) as GlTextureHandle;
        return new Planet(graphic, camera, transformation, Constants.Mercury, null, new PlanetColorTextureShading(graphic, new TextureParameter(earthMapTexture)));
    }
    
    private Planet GetVenus(GlGraphic graphic, Camera camera) {

        var spherePosition = new Point3(PlanetCalc.ToScaleDistance(Constants.Venus.distance), 0, 0);
        var orbitTilt = Matrix4.RotationZ(float.DegreesToRadians((float)Constants.Venus.orbitTilt));
        var transformation = Matrix4.Scale(PlanetCalc.ToScale(Constants.Venus.diameter) * Scale) * orbitTilt * Matrix4.Translation(spherePosition.Vector);
        Image<Rgba32>? earthMap = TextureLoader.LoadImage("Universe.resources.venusmap.jpg");
        if (earthMap == null) throw new Exception("texture not found");
        earthMap.Mutate(context => context.Flip(FlipMode.Vertical)); // switch orientation, if necessary
        GlTextureHandle earthMapTexture = Graphic.CreateTexture(earthMap) as GlTextureHandle;
        return new Planet(graphic, camera, transformation, Constants.Venus, null, new PlanetColorTextureShading(graphic, new TextureParameter(earthMapTexture)));
    }
    
    private Planet GetMars(GlGraphic graphic, Camera camera) {
        var spherePosition = new Point3(PlanetCalc.ToScaleDistance(Constants.Mars.distance), 0, 0);
        var orbitTilt = Matrix4.RotationZ(float.DegreesToRadians((float)Constants.Mars.orbitTilt));
        var transformation = Matrix4.Scale(PlanetCalc.ToScale(Constants.Mars.diameter) * Scale) * orbitTilt * Matrix4.Translation(spherePosition.Vector);
        Image<Rgba32>? earthMap = TextureLoader.LoadImage("Universe.resources.marsmap.jpg");
        if (earthMap == null) throw new Exception("texture not found");
        earthMap.Mutate(context => context.Flip(FlipMode.Vertical)); // switch orientation, if necessary
        GlTextureHandle earthMapTexture = Graphic.CreateTexture(earthMap) as GlTextureHandle;
        return new Planet(graphic, camera, transformation, Constants.Mars, null, new PlanetColorTextureShading(graphic, new TextureParameter(earthMapTexture)));
    }
    
    private Planet GetSun(GlGraphic graphic, Camera camera) {
        var spherePosition = new Point3(0, 0, 0);
        var scale = PlanetCalc.ToScale(Constants.Sun.diameter) / 100; //todo: fix scale
        var transformation = Matrix4.Scale(scale * Scale) * Matrix4.Translation(spherePosition.Vector);
        Image<Rgba32>? earthMap = TextureLoader.LoadImage("Universe.resources.sunmap.jpg");
        if (earthMap == null) throw new Exception("texture not found");
        earthMap.Mutate(context => context.Flip(FlipMode.Vertical)); // switch orientation, if necessary
        GlTextureHandle earthMapTexture = Graphic.CreateTexture(earthMap) as GlTextureHandle;

        return new Planet(graphic, camera, transformation, Constants.Sun, null, new ColorTextureShading(graphic, new TextureParameter(earthMapTexture)));
    }
    
    
    private Planet GetMoon(GlGraphic graphic, Camera camera)
    {
        double distance = Constants.Moon.distance - Constants.Earth.distance;
        float center = (float) PlanetCalc.CenterDistance(PlanetCalc.ToScale(distance), Constants.Earth, Constants.Moon);
        var scale = PlanetCalc.ToScale(Constants.Moon.diameter);
        var spherePosition = new Point3((PlanetCalc.ToScale(distance) - center) / 10, 0, 0); //todo: adjust to /200 = scale when increasing scale
        var transformation = Matrix4.Scale(scale * Scale) * Matrix4.Translation(spherePosition.Vector);

        var material = new UniformMaterial(0, 0, new Color3(0.2f, 0.2f, 0.2f));
        return new Planet(graphic, camera, transformation, Constants.Moon, material);
    }

    private static VisualPart GetGrid(Graphic graphic, Camera camera, float[] positions)
    {
        var color = new Color3(0.2f, 0.2f, 0.3f);
        var material = new UniformMaterial(0.5f, 0.1f, color);
        var light = new AmbientLight(new Color3(1, 1, 1));
        var shading = graphic.CreateShading("emissive", material, light);
        var geometry = Geometry.Create(positions);
        var surface = graphic.CreateSurface(shading, geometry);
        var plane = graphic.CreateVisual("plane", surface);
        return plane;
    }
    
    public override void OnLoad(Window window)
    {
        _sun = GetSun(_graphic, _camera);
        _mercury = GetMercury(_graphic, _camera);
        _venus = GetVenus(_graphic, _camera);
        _earth = GetEarth(_graphic, _camera);
        _moon = GetMoon(_graphic, _camera);
        _mars = GetMars(_graphic, _camera);
        var planeX = GetGrid(_graphic, _camera, Utils.Geometry.Rectangle([new Vertex(10, -1, 0), new Vertex(10, 0, 0), new Vertex(-10, -1, 0), new Vertex(-10, 0, 0)]));
        var planeZ = GetGrid(_graphic, _camera, Utils.Geometry.Rectangle([new Vertex( 0, -1, 10), new Vertex(0, 0, 10), new Vertex( 0, -1, -10), new Vertex(0, 0, -10)]));
       
        Scene.AddRange(
        [
            _sun._body,
            // planeX, planeZ,
            _mercury._body,
            _venus._body,
            _mars._body
        ]);
        
        var earthSystem = Graphic.CreateGroup("earth_system");
        Scene.Add(earthSystem);
        earthSystem.Add(_earth._body);
        earthSystem.Add(_moon._body);
        _earthSystem = new CelestialSystem(earthSystem);

    }

    protected override void OnUpdateFrame(Window window)
    {
        base.OnUpdateFrame(window);

        var currentVelocity = _parameters.Speed * Velocity;
        
        long now = DateTime.Now.Ticks;
        var deltaAngle = currentVelocity * (now - _lastUpdate);
        _lastUpdate = now;
        _rotation += deltaAngle;
        
        
        var translateEarthSystem = Matrix4.Translation(new Vector3(PlanetCalc.ToScaleDistance(Constants.Earth.distance), 0, 0));
        var rotateEarthSystem = Matrix4.RotationY(_earth.CelestialSystem.Orbit(1 / Constants.Earth.orbitPeriod * deltaAngle));
        _moon.TransformBefore( Matrix4.Translation(Vector3.Zero) );
        _moon.TransformAfter(Matrix4.RotationY(_earthSystem.Rotate(1 / Constants.Moon.orbitPeriod * deltaAngle))
                                * translateEarthSystem
                                * rotateEarthSystem);
        
        _earth.TransformBefore(Matrix4.RotationY(_earth.CelestialSystem.Rotate(deltaAngle)));
        _earth.TransformAfter(translateEarthSystem * rotateEarthSystem);
        
        _mercury.TransformBefore(Matrix4.RotationY(_mercury.CelestialSystem.Rotate(1 / _mercury.Info.rotationPeriod * deltaAngle)));
        _mercury.TransformAfter(Matrix4.RotationY(_mercury.CelestialSystem.Orbit(1 / _mercury.Info.orbitPeriod * deltaAngle)));
                
        _venus.TransformBefore(Matrix4.RotationY(_venus.CelestialSystem.Rotate(1 / _venus.Info.rotationPeriod * deltaAngle)));
        _venus.TransformAfter(Matrix4.RotationY(_venus.CelestialSystem.Orbit(1 / _venus.Info.orbitPeriod * deltaAngle)));
                      
        _mars.TransformBefore(Matrix4.RotationY(_mars.CelestialSystem.Rotate(1 / _mars.Info.rotationPeriod * deltaAngle)));
        _mars.TransformAfter(Matrix4.RotationY(_mars.CelestialSystem.Orbit(1 / _mars.Info.orbitPeriod * deltaAngle)));
                                 
        
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