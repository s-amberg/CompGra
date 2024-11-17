using System;
using System.IO;
using System.Reflection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Utils;

public static class TextureLoader
{
    public static Image<Rgba32>? LoadImage(string assemblyName)
    {
        Assembly assembly = Assembly.GetCallingAssembly();
        Stream stream = assembly.GetManifestResourceStream( assemblyName );
        if (stream is null) throw new ArgumentException("image: " + assemblyName + "not found");
        return Image.Load<Rgba32>( stream );
    }
}