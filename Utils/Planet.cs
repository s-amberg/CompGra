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
    private readonly float _axisXTilt;
    private readonly float _axisZTilt;
    private float rotationDays = 365;
    private Matrix4 _baseTransformation;
    
    private Matrix4 _rotateToOrigin;
    private Matrix4 _rotateFromOrigin;

    public VisualPart _body;
    private Graphic graphic;
    private Camera camera;

    private VisualPart CreateSphere() {
        
        var color = new Color4(0.2f, 1f, 0.3f, 1);
        var shading = graphic.CreateShading([], [ new EmissiveUniformMaterial(color)], camera);
        var positions = Sphere.GetPositions(10, 10);
        var triangles = Sphere.GetTriangles(10, 10);
        var geometry = EduGraf.Geometries.Geometry.Create(positions, triangles);
        
        var surface = graphic.CreateSurface(shading, geometry);
        var plane = graphic.CreateVisual("plane", surface);
        return plane;
    }

    public Planet(Graphic graphic, Camera camera, Matrix4 transformation, float xTilt = 0, float zTilt = 0) {
        (this.graphic, this.camera, _axisXTilt, _axisZTilt, _baseTransformation) = (graphic, camera, xTilt, zTilt, transformation);
        _body = CreateSphere();
        _rotateToOrigin = Matrix4.RotationX(- _axisXTilt) * Matrix4.RotationZ(- _axisZTilt);
        _rotateFromOrigin = Matrix4.RotationX(_axisXTilt) * Matrix4.RotationZ(_axisZTilt);

        _body.Transform = (_baseTransformation * _rotateFromOrigin);
    }


    public void Transform(Matrix4 transformation) {
        _body.Transform = transformation * _rotateFromOrigin * _baseTransformation;
    }
    
    
}