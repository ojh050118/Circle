#ifndef BLUR_FS
#define BLUR_FS

#include "sh_Utils.h"
#include "sh_CircleUtils.h"

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
    vec2 u_xlat0;
    vec3 u_xlat1 = vec3(0.0);
    vec4 u_xlat2;
    ivec2 u_xlati2;
    vec2 u_xlat3;
    vec3 u_xlat4;
    vec4 u_xlat5;
    vec2 u_xlat9;
    int u_xlati12 = 0;
    ivec2 u_xlati15;
    float u_xlat18;
    int u_xlati18;
    bool u_xlatb18;
    float u_xlat19;
    bool u_xlatb19;
    vec3 ImmCB_0_0_0[4];

    ImmCB_0_0_0[0] = vec3(1.0, 0.0, 0.0);
    ImmCB_0_0_0[1] = vec3(0.0, 1.0, 0.0);
    ImmCB_0_0_0[2] = vec3(0.0, 0.0, 1.0);
    ImmCB_0_0_0[3] = vec3(0.0, 0.0, 0.0);

    u_xlat0 = vec2(1.0) / resolution * vec2(intensity);

    while (true)
    {
        u_xlatb18 = u_xlati12 >= 3;

        if (u_xlatb18) {break;}

        u_xlati2.xy = ivec2(u_xlati12) + ivec2(int(0xFFFFFFFFu), 1);
        u_xlat18 = float(u_xlati2.x);
        u_xlat3.x = u_xlat0.x * u_xlat18;
        u_xlat4.x = dot(vec3(1.0, 2.0, 1.0), ImmCB_0_0_0[u_xlati12].xyz);
        u_xlat4.y = dot(vec3(2.0, 4.0, 2.0), ImmCB_0_0_0[u_xlati12].xyz);
        u_xlat4.z = dot(vec3(1.0, 2.0, 1.0), ImmCB_0_0_0[u_xlati12].xyz);
        u_xlat2.xzw = u_xlat1.xyz;
        u_xlati18 = 0;
        while (true)
        {
            u_xlatb19 = u_xlati18 >= 3;

            if (u_xlatb19) {break;}

            u_xlati15.xy = ivec2(u_xlati18) + ivec2(int(0xFFFFFFFFu), 1);
            u_xlat19 = float(u_xlati15.x);
            u_xlat3.y = u_xlat0.y * u_xlat19;
            u_xlat9.xy = u_xlat3.xy + v_TexCoord;
            u_xlat19 = dot(u_xlat4.xyz, ImmCB_0_0_0[u_xlati18].xyz);
            u_xlat5 = samplePP(m_Texture, m_Sampler, v_TexRect, u_xlat9.xy);
            u_xlat2.xzw = vec3(u_xlat19) * u_xlat5.xyz + u_xlat2.xzw;
            u_xlati18 = u_xlati15.y;
        }
        u_xlat1.xyz = u_xlat2.xzw;
        u_xlati12 = u_xlati2.y;
    }

    o_Colour = vec4(u_xlat1.xyz * vec3(0.0625, 0.0625, 0.0625), 1.0);
}

#endif