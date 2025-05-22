#ifndef HEXAGONBLACK_FS
#define HEXAGONBLACK_FS

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
    vec4 u_xlat0;
    bool u_xlatb0;
    vec4 u_xlat1;
    bvec4 u_xlatb1;
    vec4 u_xlat2;
    vec4 u_xlat3;
    vec3 u_xlat4;
    float u_xlat5;
    float u_xlat10;
    vec2 u_xlat11;
    float u_xlat15;
    bool u_xlatb15;
    bool u_xlatb16;

    u_xlat0.xy = v_TexCoord * resolution;
    u_xlat10 = resolution.x * intensity;
    u_xlat1.xyz = vec3(u_xlat10) * vec3(0.0108253174, 0.00937500037, 0.0108253174);
    u_xlat0.xyw = u_xlat0.yxy / u_xlat1.yxy;
    u_xlatb1.xw = greaterThanEqual(u_xlat0.yyyw, (-u_xlat0.yyyw)).xw;
    u_xlat2.xy = fract(abs(u_xlat0.yw));
    u_xlat3.xyz = floor(u_xlat0.xyw);
    u_xlat0.x = (u_xlatb1.x) ? u_xlat2.x : (-u_xlat2.x);
    u_xlat0.y = (u_xlatb1.w) ? u_xlat2.y : (-u_xlat2.y);
    u_xlat0.xy = u_xlat1.zy * u_xlat0.xy;
    u_xlat1.xy = vec2(u_xlat10) * vec2(0.00312500005, 0.00541265868);
    u_xlat15 = u_xlat1.x / u_xlat1.y;
    u_xlat1.z = u_xlat15 * u_xlat0.x;
    u_xlat15 = u_xlat0.x * u_xlat15 + (-u_xlat1.x);
    u_xlatb15 = u_xlat0.y < u_xlat15;
    u_xlat1.xw = vec2(u_xlat10) * vec2(0.00312500005, 0.00625000009) + (-u_xlat1.zz);
    u_xlatb1.xzw = lessThan(u_xlat0.yyyy, u_xlat1.xxzw).xzw;
    u_xlatb0 = u_xlat1.y < u_xlat0.x;
    u_xlat2 = u_xlat3.yzyz + vec4(-1.0, -1.0, -1.0, 0.0);
    u_xlat4.z = (u_xlatb1.w) ? u_xlat2.y : u_xlat3.z;
    u_xlat4.xy = u_xlat3.yz + vec2(0.0, -1.0);
    u_xlat11.xy = (u_xlatb1.z) ? u_xlat4.xy : u_xlat2.zw;
    u_xlat0.xy = (bool(u_xlatb0)) ? u_xlat4.xz : u_xlat11.xy;
    u_xlat3.w = (u_xlatb15) ? u_xlat2.y : u_xlat3.z;
    u_xlat1.xz = (u_xlatb1.x) ? u_xlat2.xy : u_xlat3.yw;
    u_xlat15 = u_xlat3.x * 0.5;
    u_xlatb16 = u_xlat15 >= (-u_xlat15);
    u_xlat15 = fract(abs(u_xlat15));
    u_xlat15 = (u_xlatb16) ? u_xlat15 : (-u_xlat15);
    u_xlatb15 = 0.0 < u_xlat15;
    u_xlat0.xy = (bool(u_xlatb15)) ? u_xlat1.xz : u_xlat0.xy;
    u_xlat15 = u_xlat0.y * 0.5;
    u_xlatb1.x = u_xlat15 >= (-u_xlat15);
    u_xlat15 = fract(abs(u_xlat15));
    u_xlat15 = (u_xlatb1.x) ? u_xlat15 : (-u_xlat15);
    u_xlat15 = dot(vec2(u_xlat15), u_xlat1.yy);
    u_xlat0.x = dot(u_xlat0.xx, u_xlat1.yy);
    u_xlat1.x = (-u_xlat15) + u_xlat0.x;
    u_xlat0.x = u_xlat10 * 0.00937500037;
    u_xlat1.y = u_xlat0.x * u_xlat0.y;
    u_xlat0.xy = vec2(u_xlat10) * vec2(0.0108253174, 0.00625000009) + u_xlat1.xy;
    u_xlat1.xy = (-v_TexCoord) * resolution + u_xlat0.xy;
    u_xlat0.xy = u_xlat0.xy / resolution;
    u_xlat2 = texture(sampler2D(m_Texture, m_Sampler), v_TexRect, u_xlat0.xy);
    u_xlat0.x = dot(abs(u_xlat1.xy), vec2(0.5, 0.866025388));
    u_xlat0.x = max(abs(u_xlat1.x), u_xlat0.x);
    u_xlat5 = u_xlat10 * 0.00625000009 + -1.0;
    u_xlat10 = u_xlat10 * 0.00625000009 + (-u_xlat5);
    u_xlat0.x = u_xlat0.x * 1.15470052 + (-u_xlat5);
    u_xlat5 = float(1.0) / u_xlat10;
    u_xlat0.x = u_xlat5 * u_xlat0.x;
    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
    u_xlat5 = u_xlat0.x * -2.0 + 3.0;
    u_xlat0.x = u_xlat0.x * u_xlat0.x;
    u_xlat0.x = (-u_xlat5) * u_xlat0.x + 1.0;

    o_Colour = vec4(u_xlat0.xxx * u_xlat2.xyz, 1.0);
}

#endif