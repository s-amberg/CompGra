using ClassLibrary1;
using EduGraf;
using EduGraf.Cameras;
using EduGraf.Geometries;
using EduGraf.Lighting;
using EduGraf.OpenGL;
using EduGraf.Shapes;
using EduGraf.Tensors;

namespace Utils;

public class Planet
{
    // planet axis defined as tilted from Z-Axis
    public readonly PlanetInfo Info;
    private Matrix4 _baseTransformation;
    
    private Matrix4 _rotateToOrigin;
    private Matrix4 _rotateFromOrigin;

    public VisualPart _body;
    public CelestialSystem CelestialSystem;
    private GlGraphic graphic;
    private Camera camera;

    private VisualPart CreateSphere(Material? material = null, Shading? shader = null)
    {

        var resolution = 20;
        var color = new Color3(0.2f, 1f, 0.3f);
        var planetMaterial = material ?? new UniformMaterial(0.5f, 0.1f, color);
        var sunlightSun = new PointLight(new Color3(1, 1, 1), 100, new(0, 0, 0));
        var shading = shader ?? graphic.CreateShading("emissive", planetMaterial, sunlightSun);;
        var positions = Sphere.GetPositions(resolution, resolution);
        var triangles = Sphere.GetTriangles(resolution, resolution);
        var textureUvs = Sphere.GetTextureUvs(resolution, resolution);
        var geometry = EduGraf.Geometries.Geometry.CreateWithUv(positions, positions, textureUvs, triangles);
        
        var surface = graphic.CreateSurface(shading, geometry);
        var sphere = graphic.CreateVisual("sphere", surface);
        return sphere;
    }

    public Planet(GlGraphic graphic, Camera camera, Matrix4 transformation, PlanetInfo info, Material? material = null, Shading? shader = null) {
        (this.graphic, this.camera, Info, _baseTransformation) = (graphic, camera, info, transformation);

        _body = CreateSphere(material, shader);
        CelestialSystem = new CelestialSystem(_body);
        _rotateToOrigin = Matrix4.RotationX(- Info.xTilt) * Matrix4.RotationZ(- Info.zTilt);
        _rotateFromOrigin = Matrix4.RotationX(Info.xTilt) * Matrix4.RotationZ(Info.zTilt);

        _body.Transform = (_baseTransformation * _rotateFromOrigin);
    }


    public void TransformBefore(Matrix4 transformation) {
        _body.Transform = transformation * _rotateFromOrigin * _baseTransformation;
    }
    
    public void TransformAfter(Matrix4 transformation)
    {
        _body.Transform = _body.Transform * transformation;
    }
    
    
}