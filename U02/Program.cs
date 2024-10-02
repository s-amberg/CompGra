using EduGraf.Cameras;
using EduGraf.OpenGL.OpenTK;
using EduGraf.Tensors;

namespace U02;

public static class Program
{
    public static void Main()
    {
        var graphic = new OpenTkGraphic();
        var camera = new OrbitCamera(new Point3(0, 0, 5), Point3.Origin);
        using var window = new OpenTkWindow("S3A7", graphic, 1024, 768, camera.Handle);
        var rendering = new RollingSphereRendering(graphic, camera);
        window.Show(rendering);
    }
}