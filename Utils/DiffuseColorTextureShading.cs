using EduGraf.OpenGL;
using EduGraf.Tensors;

namespace Utils;

public class DiffuseColorTextureShading : GlShading
{

    public DiffuseColorTextureShading(GlGraphic graphic, GlTextureHandle mapTextureUnit, Color3 lightAmbient, Color3 lightDiffuse)
        : base("color_texture", graphic, VertShader, FragShader, new GlNamedTextureShadingAspect("mapTextureUnit", mapTextureUnit))
    {
        DoInContext(() =>
        {
            Set("lightPosition", Point3.Origin);
            Set("lightAmbient", lightAmbient);
            Set("lightDiffuse", lightDiffuse);
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
    uniform vec3 ambient;
    uniform vec3 lightPosition;
    uniform vec3 lightAmbient;
    uniform vec3 lightDiffuse;
    uniform sampler2D mapTextureUnit;
    out vec4 fragment;

    void main(void)
    {
        vec3 mapTexture = vec3(texture(mapTextureUnit, textureUv))
        vec3 ambientColor = lightAmbient + mapTexture;
        vec3 diffuse = lightDiffuse * diff * mapTexture;

        vec3 normDir = normalize(worldNormal);
        vec3 lightDir = normalize(lightPosition - surfacePosition);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuseColor = lightColor * (diff * material.diffuse);

        vec3 result = ambientColor + diffuseColor;
        fragment = vec4(result, 1.0);

    }";

}