#ifndef BLURFOCUS_FS
#define BLURFOCUS_FS

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
    vec3 u_xlat1;
    vec4 u_xlat2;
    vec2 u_xlat3;
    float u_xlat9;
    float u_xlat10;
    vec4 col;

    u_xlat3.xy = vec2(0.5);
    u_xlat3.xy = (-u_xlat3.xy) + v_TexCoord;
    u_xlat9 = dot(u_xlat3.xy, u_xlat3.xy);
    u_xlat3.xy = vec2(u_xlat9) * u_xlat3.xy;
    u_xlat1.xyz = vec3(0.0);
    u_xlat9 = 0.0;

    while (u_xlat9 < intensity)
    {
        u_xlat10 = u_xlat9 / 2.0;
        u_xlat2.xy = u_xlat3.xy * vec2(u_xlat10) + v_TexCoord;
        u_xlat2 = samplePP(m_Texture, m_Sampler, v_TexRect, u_xlat2.xy);
        u_xlat1.xyz = u_xlat1.xyz + u_xlat2.xyz;
        u_xlat9 += 0.2;
    }

    col.rgb = u_xlat1.xyz / vec3(intensity * 5.0);
    col.a = 1.0;

    o_Colour = col;
}

#endif