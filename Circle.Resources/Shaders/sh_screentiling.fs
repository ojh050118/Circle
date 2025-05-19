#ifndef SCREENTILING_FS
#define SCREENTILING_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;
layout(location = 3) in mediump vec2 v_TexRect;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float tilingX;
    mediump float tilingY;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
    vec2 resolution = vec2(v_TexRect[2] - v_TexRect[0], v_TexRect[1] - v_TexRect[3]);
    vec2 pixelPos = v_TexCoord / resolution;
    
    o_Colour = texture(sampler2D(m_Texture, m_Sampler), fract(pixelPos * vec2(tilingX, tilingY)) * resolution);
}

#endif