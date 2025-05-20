#ifndef TUNNEL_FS
#define TUNNEL_FS

#include "sh_Utils.h"
#include "sh_CircleUtils.h"

#undef PI
#define PI 3.1415926538

layout(location = 2) in mediump vec2 v_TexCoord;
layout (location = 3) in mediump vec4 v_TexRect;

layout(set = 0, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 0, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

float angleBetween(vec2 l1p1, vec2 l1p2, vec2 l2p1, vec2 l2p2)
{
    return atan(l1p2.y - l1p1.y, l1p2.x - l1p1.x) - atan(l2p2.y - l2p1.y, l2p2.x - l2p1.x);
}

void main(void)
{
    vec2 resolution = v_TexRect.zw - v_TexRect.xy;
    vec2 pixelPos = (v_TexCoord - v_TexRect.xy) / resolution;

    float angle = angleBetween(vec2(0.5, 0.0), vec2(0.5, 1.0), vec2(0.5), pixelPos) + PI / 2.0; // in rad
    vec2 sampleTarget = vec2(fract((distance(pixelPos, vec2(0.5)) * 1.1 + 0.45) * 0.9), 1.0 - angle / 2.0 / PI);

    o_Colour = samplePP(m_Texture, m_Sampler, v_TexRect, sampleTarget * resolution);
}

#endif
