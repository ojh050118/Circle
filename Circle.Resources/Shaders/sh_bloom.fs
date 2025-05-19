#ifndef BLOOM_FS
#define BLOOM_FS

#include "sh_Utils.h"

#undef PI
#define PI 3.1415926538

layout(location = 2) in mediump vec2 v_TexCoord;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
    mediump float threshold;
    mediump float r;
    mediump float g;
    mediump float b;
    mediump vec2 resolution;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 colour;

vec4 thresholded(vec4 col)
{
    vec3 srgb = col.rgb;
    return col * float(dot(srgb, vec3(0.3, 0.59, 0.11)) > threshold);
}

void main(void)
{
    vec4 col = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord);

    vec2 texel = 1.0 / resolution;

    float radius = 1.5;
    vec4 blur = vec4(0.0);

    for (float y = -radius; y < radius; y++)
    {
        float xr = sqrt(radius * radius - y * y);
        
        for (float x = -xr; x < xr; x++)
            blur += thresholded(texture(sampler2D(m_Texture, m_Sampler), v_TexCoord + vec2(x, y) * texel));
    }

    blur /= PI * radius * radius;

    vec4 bloom = blur * vec4(r, g, b, 1.0) * intensity;
    
    colour = col + bloom;
}

#endif

