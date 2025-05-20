#ifndef EDGEBLACKLINE_FS
#define EDGEBLACKLINE_FS

#include "sh_Utils.h"
#include "sh_CircleUtils.h"

layout (location = 2) in highp vec2 v_TexCoord;
layout (location = 3) in highp vec4 v_TexRect;

layout (set = 0, binding = 0) uniform lowp texture2D m_Texture;
layout (set = 0, binding = 1) uniform lowp sampler m_Sampler;

layout (location = 0) out vec4 o_Colour;

void main(void)
{
    vec4 u_xlat0;
    float u_xlat1;
    int u_xlati1;
    vec2 u_xlat2;
    vec4 u_xlat3;
    vec4 u_xlat4;
    ivec3 u_xlati6;
    vec3 u_xlat7;
    float u_xlat12;
    int u_xlati12;
    vec4 TempArray0[25];

    TempArray0[0].x = -1.0;
    TempArray0[1].x = -1.0;
    TempArray0[2].x = -1.0;
    TempArray0[3].x = -1.0;
    TempArray0[4].x = -1.0;
    TempArray0[5].x = -1.0;
    TempArray0[6].x = -1.0;
    TempArray0[7].x = -1.0;
    TempArray0[8].x = -1.0;
    TempArray0[9].x = -1.0;
    TempArray0[10].x = -1.0;
    TempArray0[11].x = -1.0;
    TempArray0[13].x = -1.0;
    TempArray0[14].x = -1.0;
    TempArray0[15].x = -1.0;
    TempArray0[16].x = -1.0;
    TempArray0[17].x = -1.0;
    TempArray0[18].x = -1.0;
    TempArray0[19].x = -1.0;
    TempArray0[20].x = -1.0;
    TempArray0[21].x = -1.0;
    TempArray0[22].x = -1.0;
    TempArray0[23].x = -1.0;
    TempArray0[24].x = -1.0;
    TempArray0[12].x = 24.0;
    u_xlat0 = vec4(0.0);
    u_xlati1 = 0;
    while (u_xlati1 < 5)
    {
        u_xlati6.xy = ivec2(u_xlati1) + ivec2(int(0xFFFFFFFEu), 1);
        u_xlat2.x = float(u_xlati6.x);
        u_xlat3 = u_xlat0;
        u_xlati6.x = 0;
        while (u_xlati6.x < 5)
        {
            u_xlati12 = u_xlati1 * 5 + u_xlati6.x;
            u_xlat12 = TempArray0[u_xlati12].x;
            u_xlati6.xz = u_xlati6.xx + ivec2(1, int(0xFFFFFFFEu));
            u_xlat2.y = float(u_xlati6.z);
            u_xlat7.xz = u_xlat2.xy * vec2(0.01) + v_TexCoord;
            u_xlat4 = samplePP(m_Texture, m_Sampler, v_TexRect, u_xlat7.xz);
            u_xlat3 = vec4(u_xlat12) * u_xlat4 + u_xlat3;
        }
        u_xlat0 = u_xlat3;
        u_xlati1 = u_xlati6.y;
    }
    u_xlat1 = u_xlat0.y + u_xlat0.x;
    u_xlat1 = u_xlat0.z + u_xlat1;

    o_Colour = (u_xlat1 < 2.4) ? vec4(vec3(0.0), 1.0) : vec4(u_xlat0.rgb, 1.0);
}

#endif