using EduGraf.Cameras;
using EduGraf.OpenGL;
using EduGraf.Tensors;

namespace Utils;

public class DiffuseColorTextureShading : GlShading
{
    private MovingLight _light;

    public DiffuseColorTextureShading(GlGraphic graphic, GlTextureHandle mapTextureUnit, MovingLight light, Camera camera)
        : base("color_texture", graphic, VertShader, FragShader, new GlNamedTextureShadingAspect("mapTextureUnit", mapTextureUnit))
    {
        _light = light;
        
        DoInContext(() =>
        {
            Console.WriteLine(camera.View.Position);

            Set("lightPosition", _light.Position);
            Set("lightAmbient", _light.Ambient);
            Set("lightDiffuse", _light.Diffuse);
            Set("lightSpecular", _light.Specular);
            Set("viewPos", camera.View.Position);
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
    uniform vec3 lightAmbient;
    uniform vec3 lightDiffuse;
    uniform vec3 lightSpecular;
    uniform sampler2D mapTextureUnit;
    uniform vec3 viewPos;
    out vec4 fragment;

    void main(void)
    {
        vec3 normDir = normalize(worldNormal);
        vec3 lightDir = normalize(lightPosition - surfacePosition);

        vec3 mapTexture = texture(mapTextureUnit, textureUv).rgb;
        vec3 ambientColor = lightAmbient * mapTexture;
        float diff = max(dot(normDir, lightDir), 0.0);
        vec3 diffuseColor = lightDiffuse * diff * mapTexture;

        // specular
        vec3 viewDir = normalize(viewPos - surfacePosition);
        vec3 reflectDir = reflect(-lightDir, normDir);  
        float matShininess = 64;
        vec3 matSpecular = vec3(0.5f, 0.5f, 0.5f);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), matShininess);
        vec3 specularColor = lightSpecular * (spec * matSpecular);  

        vec3 result = ambientColor + diffuseColor + specularColor;
        fragment = vec4(result, 1.0);

    }";

}