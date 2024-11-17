using EduGraf.OpenGL;
using EduGraf.Tensors;

namespace Utils;

public record TextureParameter(GlTextureHandle Handle) {}
public class PlanetColorTextureShading : GlShading
{
    private static GlShadingAspect[] MapParams(TextureParameter[] parameters)
    {
        return parameters.Select((parameter, index) =>
        {
            var name = index == 0 ? "mapTextureUnit" : "lightsTextureUnit";
            return new GlNamedTextureShadingAspect(name, parameter.Handle);
        }).ToArray();
    }
    
    private static string GetFragShader(TextureParameter[] parameters)
    {
        switch (parameters.Length)
        {
            case(1): return FragShader;
            default: return FragShaderLights;
        }
    }

    public PlanetColorTextureShading(GlGraphic graphic, params TextureParameter[] textures)
        : base("color_texture", graphic, VertShader, GetFragShader(textures), MapParams(textures))
    {
        DoInContext(() =>
        {
            Set("lightPosition", Point3.Origin);
            Set("lightColor", new Color3(1, 1, 1));
        });
    }

    private const string VertShader = @"
    #version 410

    in vec3 Position;

    uniform mat4 Model;
    uniform mat4 View;
    uniform mat4 Projection;
    out vec3 surfacePosition;

    in vec2 TextureUv;
    out vec2 textureUv;

    in vec3 Normal;
    out vec3 worldNormal;

    void main(void)
    {
	    vec4 worldPos = vec4(Position, 1.0) * Model;
        worldNormal = Normal * mat3(Model);
	    gl_Position = worldPos * View * Projection;
        surfacePosition = vec3(worldPos);
        textureUv = TextureUv;
    }";

    
    private const string FragShader = @"
    #version 410

    in vec3 worldNormal;
    in vec2 textureUv;
    in vec3 surfacePosition;
    uniform vec3 lightPosition;
    uniform vec3 lightColor;
    uniform sampler2D mapTextureUnit;
    out vec4 fragment;

    void main(void)
    {
        vec3 normDir = normalize(worldNormal);
        vec3 lightDir = normalize(lightPosition - surfacePosition);
        float i = max(dot(normDir, lightDir), 0.0);
        if (i > 0) fragment = i * texture(mapTextureUnit, textureUv);
        else fragment = vec4(0, 0, 0, 1);
    }";
    
    private const string FragShaderLights = @"
    #version 410

    in vec3 worldNormal;
    in vec2 textureUv;
    in vec3 surfacePosition;
    uniform vec3 lightPosition;
    uniform vec3 lightColor;
    uniform sampler2D mapTextureUnit;
    uniform sampler2D lightsTextureUnit;
    out vec4 fragment;

    void main(void)
    {
        vec3 normDir = normalize(worldNormal);
        vec3 lightDir = normalize(lightPosition - surfacePosition);
        float i = max(dot(normDir, lightDir), 0.0);
        if (i > 0) fragment = i * texture(mapTextureUnit, textureUv);
        else fragment = vec4(0, 0, 0, 1);
        if (i <= 0.5f) {
            fragment = min(vec4(1, 1, 1, 1), fragment + (-0.8 * pow(i + 0.6, 2) + 1) *texture(lightsTextureUnit, textureUv));
        }
    }";

}