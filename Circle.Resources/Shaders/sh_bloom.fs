#include "sh_Utils.h"

#define PI 3.1415926538

varying mediump vec2 v_TexCoord;

uniform lowp sampler2D m_Sampler;
uniform mediump float intensity;
uniform mediump float threshold;
uniform mediump float r;
uniform mediump float g;
uniform mediump float b;
uniform mediump vec2 resolution;

vec4 thresholded(vec4 col)
{
    vec3 srgb = toSRGB(col).rgb;
    return col * float(dot(srgb, vec3(0.3, 0.59, 0.11)) > threshold);
}

void main(void)
{
    vec4 col = texture2D(m_Sampler, v_TexCoord);

    vec2 texel = 1.0 / resolution;

    float radius = 1.5;
    vec4 blur = vec4(0.0);

    for (float y = -radius; y < radius; y++)
    {
        float xr = sqrt(radius * radius - y * y);
        
        for (float x = -xr; x < xr; x++)
            blur += thresholded(texture2D(m_Sampler, v_TexCoord + vec2(x, y) * texel));
    }

    blur /= PI * radius * radius;

    vec4 bloom = blur * vec4(r, g, b, 1.0) * intensity;
    
    gl_FragColor = toSRGB(col + bloom);
}

