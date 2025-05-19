#ifndef PIXELATE_FS
#define PIXELATE_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;
layout(location = 3) in mediump vec2 v_TexRect;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
    vec3 u_xlat0 = vec3(128.0 / intensity, 512.0, 512.0);
    o_Colour = texture(sampler2D(m_Texture, m_Sampler), floor(u_xlat0.yz * floor(u_xlat0.xx * v_TexCoord) / u_xlat0.xx) / u_xlat0.yz);
}

#endif
