#ifndef LIGHTWATER_FS
#define LIGHTWATER_FS

#include "sh_Utils.h"

layout (location = 2) in highp vec2 v_TexCoord;
layout (location = 3) in highp vec4 v_TexRect;

layout (std140, set = 0, binding = 0) uniform m_FilterParameters
{
    float intensity;
    float time;
};

layout (set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout (set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout (location = 0) out vec4 o_Colour;

void main(void)
{
    float u_xlat0;
    vec4 u_xlat1;
    int u_xlati1;
    bool u_xlatb1;
    vec2 u_xlat2;
    bool u_xlatb2;
    float u_xlat3;
    vec2 u_xlat4;
    float u_xlat5;
    bool u_xlatb5;
    float u_xlat6;
    float u_xlat8;
    float u_xlat9;
    float u_xlat10;
    float u_xlat12;
    bool u_xlatb12;
    int u_xlati13;

    u_xlat0 = time * 1.3;
    u_xlat4.xy = vec2(u_xlat0) * vec2(0.2, 0.3);
    u_xlat12 = 0.0;

    for (int i = 0; i < 8; i++)
    {
        u_xlat5 = float(i);
        u_xlat5 = u_xlat5 * 0.897597909;
        u_xlat2.x = sin(u_xlat5);
        u_xlat3 = cos(u_xlat5);
        u_xlat5 = u_xlat0 * u_xlat3;
        u_xlat5 = u_xlat5 * 0.2 + u_xlat4.x;
        u_xlat5 = u_xlat5 + v_TexCoord.x;
        u_xlat9 = u_xlat0 * u_xlat2.x;
        u_xlat9 = u_xlat9 * 0.2 + (-u_xlat4.y);
        u_xlat9 = (-u_xlat9) + v_TexCoord.y;
        u_xlat9 = u_xlat2.x * u_xlat9;
        u_xlat5 = u_xlat5 * u_xlat3 + (-u_xlat9);
        u_xlat5 = u_xlat5 * 6.0;
        u_xlat5 = cos(u_xlat5);
        u_xlat12 = u_xlat5 * intensity + u_xlat12;
    }

    u_xlat12 = cos(u_xlat12);
    u_xlat1.xy = v_TexCoord + vec2(8.53);
    u_xlat9 = float(0.0);

    for (int i = 0; i < 8; i++)
    {
        u_xlat2.x = float(i);
        u_xlat2.x = u_xlat2.x * 0.897597909;
        u_xlat3 = cos(u_xlat2.x);
        u_xlat2.x = sin(u_xlat2.x);
        u_xlat6 = u_xlat0 * u_xlat3;
        u_xlat6 = u_xlat6 * 0.2 + u_xlat4.x;
        u_xlat6 = u_xlat1.x + u_xlat6;
        u_xlat10 = u_xlat0 * u_xlat2.x;
        u_xlat10 = u_xlat10 * 0.2 + (-u_xlat4.y);
        u_xlat10 = (-u_xlat10) + v_TexCoord.y;
        u_xlat2.x = u_xlat2.x * u_xlat10;
        u_xlat2.x = u_xlat6 * u_xlat3 + (-u_xlat2.x);
        u_xlat2.x = u_xlat2.x * 6.0;
        u_xlat2.x = cos(u_xlat2.x);
        u_xlat9 = u_xlat2.x * intensity + u_xlat9;
    }

    u_xlat1.x = cos(u_xlat9);
    u_xlat1.x = u_xlat12 + (-u_xlat1.x);
    u_xlat9 = float(0.0);

    for (int i = 0; i < 8; i++)
    {
        u_xlat2.x = float(i);
        u_xlat2.x = u_xlat2.x * 0.897597909;
        u_xlat3 = cos(u_xlat2.x);
        u_xlat2.x = sin(u_xlat2.x);
        u_xlat6 = u_xlat0 * u_xlat3;
        u_xlat6 = u_xlat6 * 0.2 + u_xlat4.x;
        u_xlat6 = u_xlat6 + v_TexCoord.x;
        u_xlat10 = u_xlat0 * u_xlat2.x;
        u_xlat10 = u_xlat10 * 0.2 + (-u_xlat4.y);
        u_xlat10 = u_xlat1.y + (-u_xlat10);
        u_xlat2.x = u_xlat2.x * u_xlat10;
        u_xlat2.x = u_xlat6 * u_xlat3 + (-u_xlat2.x);
        u_xlat2.x = u_xlat2.x * 6.0;
        u_xlat2.x = cos(u_xlat2.x);
        u_xlat9 = u_xlat2.x * intensity + u_xlat9;
    }

    u_xlat0 = cos(u_xlat9);
    u_xlat0 = (-u_xlat0) + u_xlat12;
    u_xlat4.x = u_xlat1.x * 0.00833333377;
    u_xlat2.x = u_xlat1.x * 0.0166666675 + v_TexCoord.x;
    u_xlat8 = u_xlat0 * 0.00833333377;
    u_xlat2.y = u_xlat0 * 0.0166666675 + v_TexCoord.y;
    u_xlat4.x = u_xlat8 * u_xlat4.x;
    u_xlat4.x = u_xlat4.x * 700.0 + 1.0;
    u_xlat8 = u_xlat1.x * 0.00833333377 + -0.0120000001;
    u_xlat0 = u_xlat0 * 0.00833333377 + -0.0120000001;
    u_xlatb12 = u_xlat8 > 0.0;
    u_xlatb1 = u_xlat0 > 0.0;
    u_xlatb12 = u_xlatb12 && u_xlatb1;
    u_xlat0 = u_xlat0 * u_xlat8;
    u_xlat0 = u_xlat0 * 200000.0;
    u_xlat8 = log2(u_xlat4.x);
    u_xlat0 = u_xlat8 * u_xlat0;
    u_xlat0 = exp2(u_xlat0);
    u_xlat0 = u_xlatb12 ? u_xlat0 : u_xlat4.x;
    u_xlat1 = texture(sampler2D(m_Texture, m_Sampler), u_xlat2.xy);

    o_Colour = vec4(u_xlat0) * u_xlat1;
}

#endif