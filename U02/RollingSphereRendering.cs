using EduGraf;
using EduGraf.Cameras;
using EduGraf.Geometries;
using EduGraf.Lighting;
using EduGraf.Shapes;
using EduGraf.Tensors;
using EduGraf.UI;

namespace U02;

public class RollingSphereRendering : Rendering
{
    const int Scale = 2;
    private const float Velocity = 0.02f;

    private readonly VisualPart _sphere;
    private float _sphereAngle;
    private Point3 _spherePosition;

    public RollingSphereRendering(Graphic graphic, Camera camera)
        : base(graphic, new Color3(0, 0, 0))
    {
        var plane = GetPlane(graphic, camera);
        _sphere = GetSphere(graphic, camera);
        _spherePosition = new Point3(-4, Scale, 0);
        Scene.AddRange(
        [
            _sphere
                .Scale(Scale)
                .Translate(_spherePosition.Vector),
            plane.Scale(10 * Scale)
        ]);
    }

    private VisualPart GetPlane(Graphic graphic, Camera camera)
    {
        var color = new Color3(0.2f, 0.2f, 0.3f);
        var light = new AmbientLight(new(1, 1, 1));
        var material = new UniformMaterial(0, 0, color);
        var shading = Graphic.CreateShading("emissive", material, light);
        var positions = Patch.GetPositions(1, 1, (_, _) => 0);
        var triangles = Patch.GetTriangles(1, 1);
        var geometry = Geometry.Create(positions, triangles);
        var surface = graphic.CreateSurface(shading, geometry);
        var plane = graphic.CreateVisual("plane", surface);
        return plane;
    }

    private VisualPart GetSphere(Graphic graphic, Camera camera)
    {
        var color = new Color3(0.8f, 1, 0.8f);
        var light = new AmbientLight(new(1, 1, 1));
        var material = new UniformMaterial(0, 0, color);
        var shading = Graphic.CreateShading("emissive", material, light);
        var positions = Sphere.GetPositions(10, 10);
        var triangles = Sphere.GetTriangles(10, 10);
        var geometry = Geometry.Create(positions, triangles);
        var surface = graphic.CreateSurface(shading, geometry);
        return graphic.CreateVisual("sphere", surface);
    }

    protected override void OnUpdateFrame(Window window)
    {
        base.OnUpdateFrame(window);

        // _sphereAngle += Velocity / Scale;
        // _spherePosition += new Vector3(-Velocity, 0, 0);
        float rotationDegrees = Velocity * 0.5f;
        _sphereAngle += rotationDegrees;
        _spherePosition += new Vector3(-(float)(Math.Tan(rotationDegrees) * Scale), 0, 0);

        _sphere.Transform = Matrix4.Scale(Scale) *
                            Matrix4.RotationZ(_sphereAngle)*
                            Matrix4.Translation(_spherePosition.Vector);
    }
}