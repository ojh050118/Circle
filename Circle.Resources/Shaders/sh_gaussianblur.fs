#ifndef GAUSSIANBLUR_FS
#define GAUSSIANBLUR_FS

#include "sh_Utils.h"

layout (location = 2) in highp vec2 v_TexCoord;
layout (location = 3) in highp vec4 v_TexRect;

layout (std140, set = 0, binding = 0) uniform m_FilterParameters
{
    float intensity;
    vec2 resolution;
};

layout (set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout (set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout (location = 0) out vec4 o_Colour;

void main(void)
{
    vec3 u_xlat0 = vec3(0.0);
    float u_xlat1;
    ivec2 u_xlati1;
    bool u_xlatb1;
    vec2 u_xlat2;
    vec3 u_xlat3;
    vec4 u_xlat4;
    vec2 u_xlat7;
    float u_xlat11;
    int u_xlati11;
    float u_xlat12;
    ivec2 u_xlati12;
    int u_xlati15;
    float u_xlat16;
    bool u_xlatb16;
    vec4 TempArray0[6];

    TempArray0[5].x = 0.0;
    TempArray0[2].x = 0.4;
    TempArray0[1].x = 0.4;
    TempArray0[3].x = 0.4;
    TempArray0[0].x = 0.4;
    TempArray0[4].x = 0.4;
    u_xlati15 = int(int(0xFFFFFFFEu));
    while (true) {
        u_xlatb1 = 2 < u_xlati15;
        if (u_xlatb1) {break;}
        u_xlati1.xy = ivec2(u_xlati15) + ivec2(2, 1);
        u_xlat1 = TempArray0[u_xlati1.x].x;
        u_xlat11 = float(u_xlati15);
        u_xlat2.x = u_xlat11 * intensity;
        u_xlat3.xyz = u_xlat0.xyz;
        u_xlati11 = int(0xFFFFFFFEu);
        while (true) {
            u_xlatb16 = 2 < u_xlati11;
            if (u_xlatb16) {break;}
            u_xlati12.xy = ivec2(u_xlati11) + ivec2(2, 1);
            u_xlat16 = TempArray0[u_xlati12.x].x;
            u_xlat16 = u_xlat1 * u_xlat16;
            u_xlat12 = float(u_xlati11);
            u_xlat2.y = u_xlat12 * intensity;
            u_xlat7.xy = v_TexCoord * resolution + u_xlat2.xy;
            u_xlat7.xy = u_xlat7.xy / resolution;
            u_xlat4 = texture(sampler2D(m_Texture, m_Sampler), u_xlat7.xy);
            u_xlat3.xyz = vec3(u_xlat16) * u_xlat4.xyz + u_xlat3.xyz;
            u_xlati11 = u_xlati12.y;
        }
        u_xlat0.xyz = u_xlat3.xyz;
        u_xlati15 = u_xlati1.y;
    }

    o_Colour = vec4(u_xlat0 * vec3(0.25), 1.0);
}

#endif