using DiffuseShaderApp;
using EduGraf;
using EduGraf.Cameras;
using EduGraf.Geometries;
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

class SquareRendering(GlGraphic graphic)
    : Rendering(graphic, new Color3(0.2f, 0, 0.2f))
{
    private static readonly Square Square = new (Point3.Origin, 2);
    
    private GlShading GetShading() {

        Image<Rgba32>? texture = TextureLoader.LoadImage("DiffuseShaderApp.resources.containerDiffuse.png");
        if (texture == null) throw new Exception("texture not found");
        GlTextureHandle textureHandle = Graphic.CreateTexture(texture) as GlTextureHandle;
        return new DiffuseColorTextureShading(graphic, textureHandle, new(0.1f, 0.1f, 0.1f), new(1, 1, 1));
    }
    
    private VisualPart CreateSquare(Shading? shader = null)
    {
        var shading = shader;
        var positions = Cube.Positions;
        var triangles = Cube.Triangles;
        var textureUvs = Cube.TextureUv;
        var geometry = Geometry.CreateWithUv(positions, positions, textureUvs, triangles);
        
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



static class Program
{
    static void Main()
    {
        var graphic = new OpenTkGraphic();
        var camera = new OrbitCamera(new Point3(1, -2, -2),  Point3.Origin);
        var rendering = new SquareRendering(graphic);
        using var window = new OpenTkWindow("DiffuseShaderApp", graphic, 1200, 700, camera.Handle);
        window.Show(rendering, camera);
    }
}