#ifndef CONTRAST_FS
#define CONTRAST_FS

#include "sh_Utils.h"
#include "sh_CircleUtils.h"

layout (location = 2) in highp vec2 v_TexCoord;
layout (location = 3) in highp vec4 v_TexRect;

layout (std140, set = 0, binding = 0) uniform m_FilterParameters
{
    float intensity;
};

layout (set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout (set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout (location = 0) out vec4 o_Colour;

void main(void)
{
    vec3 col = samplePP(m_Texture, m_Sampler, v_TexRect, v_TexCoord).rgb - vec3(0.5);
    o_Colour = vec4(vec3(intensity) * col + vec3(0.5), 1.0);
}

#endif