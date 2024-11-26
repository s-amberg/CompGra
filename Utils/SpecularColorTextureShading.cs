using EduGraf.OpenGL;

namespace Utils;

public class SpecularColorTextureShading : GlShading
{
    public SpecularColorTextureShading(GlGraphic graphic, GlTextureHandle mapTextureUnit, GlTextureHandle specularTextureUnit, MovingLight light)
        : base("color_texture", graphic, VertShader, FragShader, new GlNamedTextureShadingAspect("mapTextureUnit", mapTextureUnit), new GlNamedTextureShadingAspect("specularTextureUnit", specularTextureUnit))
    {
        DoInContext(() =>
        {
            Set("lightPosition", light.Position);
            Set("lightAmbient", light.Ambient);
            Set("lightDiffuse", light.Diffuse);
            Set("lightSpecular", light.Specular);
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
    uniform vec3 CameraPosition;
    uniform vec3 lightPosition;
    uniform vec3 lightAmbient;
    uniform vec3 lightDiffuse;
    uniform vec3 lightSpecular;

    uniform sampler2D mapTextureUnit;
    uniform sampler2D specularTextureUnit;

    out vec4 fragment;

    void main(void)
    {
        vec3 normDir = normalize(worldNormal);
        vec3 lightDir = normalize(lightPosition - surfacePosition);

        vec3 mapTexture = vec3(texture(mapTextureUnit, textureUv));
        vec3 ambientColor = lightAmbient * mapTexture;
        float diff = max(dot(normDir, lightDir), 0.0);
        vec3 diffuseColor = lightDiffuse * diff * mapTexture;

        // specular
        vec3 viewDir = normalize(CameraPosition - surfacePosition);
        vec3 reflectDir = reflect(-lightDir, normDir);  
        float matShininess = 16;
        float spec = pow(max(dot(viewDir, reflectDir), 0.0f), matShininess);
        vec3 specularColor = lightSpecular * spec * vec3(texture(specularTextureUnit, textureUv));  

        vec3 result = ambientColor + diffuseColor + specularColor;
        fragment = vec4(result, 1.0);

    }";

}