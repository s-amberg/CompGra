using EduGraf.OpenGL;

namespace Universe;

public class ColorTextureShading : GlShading
{
    public ColorTextureShading(GlGraphic graphic, GlTextureHandle handle, GlTextureHandle lights)
        : base("color_texture", graphic, VertShader, FragShader, 
            new GlNamedTextureShadingAspect("mapTextureUnit", handle), 
            new GlNamedTextureShadingAspect("lightsTextureUnit", lights)
            )
    {
        DoInContext(() =>
        {
            
        });
    }

    private const string VertShader = @"
    #version 410

    in vec3 Position;

    uniform mat4 Model;
    uniform mat4 View;
    uniform mat4 Projection;

    in vec2 TextureUv;
    out vec2 textureUv;

    in vec3 Normal;
    out vec3 worldNormal;

    void main(void)
    {
	    vec4 worldPos = vec4(Position, 1.0) * Model;
        worldNormal = Normal * mat3(Model);
	    gl_Position = worldPos * View * Projection;
        textureUv = TextureUv;
    }";

    private const string FragShader = @"
    #version 410

    in vec3 worldNormal;
    in vec2 textureUv;
    uniform sampler2D mapTextureUnit;
    uniform sampler2D lightsTextureUnit;
    out vec4 fragment;

    void main(void)
    {
        float i = normalize(worldNormal).z;
        if (i > 0) fragment = i * texture(mapTextureUnit, textureUv);
        else fragment = vec4(0, 0, 0, 1);
        if (i <= 0.25f) {
            fragment = min(vec4(1, 1, 1, 1), fragment + (1/abs((i+0.25f)*3))*texture(lightsTextureUnit, textureUv));
        }
        //if (i <= 0.25f) {
        //    if(vec4(1, 1, 1, 1) >=  fragment + (1/i)*texture(lightsTextureUnit, textureUv)) {
        //        fragment = fragment + (1/i)*texture(lightsTextureUnit, textureUv);
        //    }
        //    else fragment = vec4(1, 1, 1, 1);
        //}
    }";

}