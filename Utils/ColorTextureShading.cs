using EduGraf.OpenGL;
using EduGraf.Tensors;

namespace Utils;

public class ColorTextureShading : GlShading
{

    public ColorTextureShading(GlGraphic graphic, TextureParameter texture)
        : base("color_texture", graphic, VertShader, FragShader, 
            new GlNamedTextureShadingAspect("mapTextureUnit", texture.Handle))
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

    void main(void)
    {
	    vec4 worldPos = vec4(Position, 1.0) * Model;
	    gl_Position = worldPos * View * Projection;
        textureUv = TextureUv;
    }";

    
    private const string FragShader = @"
    #version 410

    in vec2 textureUv;
    uniform sampler2D mapTextureUnit;
    out vec4 fragment;

    void main(void)
    {
        fragment = texture(mapTextureUnit, textureUv);
    }";
    

}