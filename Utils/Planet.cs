using EduGraf;
using EduGraf.Cameras;
using EduGraf.Geometries;
using EduGraf.Lighting;
using EduGraf.Shapes;
using EduGraf.Tensors;

namespace Utils;

public class Planet
{
    // planet axis defined as tilted from Z-Axis
    public readonly PlanetInfo Info;
    private float rotationDays = 365;
    private Matrix4 _baseTransformation;
    
    private Matrix4 _rotateToOrigin;
    private Matrix4 _rotateFromOrigin;

    public VisualPart _body;
    private Graphic graphic;
    private Camera camera;

    private VisualPart CreateSphere(Material? material = null)
    {

        var resolution = 20;
        var color = new Color3(0.2f, 1f, 0.3f);
        var planetMaterial = material ?? new UniformMaterial(0.5f, 0.1f, color);
        var sunlight = new ParallelLight(new Color3(1, 1, 1), new(0, 0, -1000));
        var shading = graphic.CreateShading("emissive", planetMaterial, sunlight);
        var positions = Sphere.GetPositions(resolution, resolution);
        var triangles = Sphere.GetTriangles(resolution, resolution);
        var textureUvs = Sphere.GetTextureUvs(resolution, resolution);
        var geometry = EduGraf.Geometries.Geometry.CreateWithUv(positions, positions, textureUvs, triangles);
        
        var surface = graphic.CreateSurface(shading, geometry);
        var plane = graphic.CreateVisual("plane", surface);
        return plane;
    }

    public Planet(Graphic graphic, Camera camera, Matrix4 transformation, PlanetInfo info, Material? material = null) {
        (this.graphic, this.camera, Info, _baseTransformation) = (graphic, camera, info, transformation);

        _body = CreateSphere(material);
        _rotateToOrigin = Matrix4.RotationX(- Info.xTilt) * Matrix4.RotationZ(- Info.zTilt);
        _rotateFromOrigin = Matrix4.RotationX(Info.xTilt) * Matrix4.RotationZ(Info.zTilt);

        _body.Transform = (_baseTransformation * _rotateFromOrigin);
    }


    public void Transform(Matrix4 transformation) {
        _body.Transform = transformation * _rotateFromOrigin * _baseTransformation;
    }
    
    
}