#ifndef OILPAINT_FS
#define OILPAINT_FS

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
    vec4 u_xlat2;
    vec4 u_xlat3;
    vec4 u_xlat4;
    vec4 u_xlat5;
    vec4 u_xlat6;
    vec4 u_xlat7;
    vec4 u_xlat8;
    vec3 u_xlat9;
    bool u_xlatb9;
    float u_xlat18;
    vec2 u_xlat19;

    u_xlat0 = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord);
    u_xlat1.xy = vec2(intensity) / resolution;
    u_xlat2 = u_xlat1.xyxy * vec4(1.0, 0.0, -2.0, -2.0) + v_TexCoord.xyxy;
    u_xlat3 = texture(sampler2D(m_Texture, m_Sampler), u_xlat2.xy);
    u_xlat2 = texture(sampler2D(m_Texture, m_Sampler), u_xlat2.zw);
    u_xlat4.xyz = u_xlat3.xyz * u_xlat3.xyz;
    u_xlat3.xyz = u_xlat0.xyz + u_xlat3.xyz;
    u_xlat0.xyz = u_xlat0.xyz * u_xlat0.xyz + u_xlat4.xyz;
    u_xlat4 = u_xlat1.xyxy * vec4(2.0, 0.0, -4.0, -3.0) + v_TexCoord.xyxy;
    u_xlat5 = texture(sampler2D(m_Texture, m_Sampler), u_xlat4.xy);
    u_xlat4 = texture(sampler2D(m_Texture, m_Sampler), u_xlat4.zw);
    u_xlat0.xyz = u_xlat5.xyz * u_xlat5.xyz + u_xlat0.xyz;
    u_xlat3.xyz = u_xlat3.xyz + u_xlat5.xyz;
    u_xlat5 = u_xlat1.xyxy * vec4(0.0, 1.0, -2.0, -3.0) + v_TexCoord.xyxy;
    u_xlat6 = texture(sampler2D(m_Texture, m_Sampler), u_xlat5.xy);
    u_xlat5 = texture(sampler2D(m_Texture, m_Sampler), u_xlat5.zw);
    u_xlat0.xyz = u_xlat6.xyz * u_xlat6.xyz + u_xlat0.xyz;
    u_xlat3.xyz = u_xlat3.xyz + u_xlat6.xyz;
    u_xlat19.xy = u_xlat1.xy + v_TexCoord;
    u_xlat6 = texture(sampler2D(m_Texture, m_Sampler), u_xlat19.xy);
    u_xlat0.xyz = u_xlat6.xyz * u_xlat6.xyz + u_xlat0.xyz;
    u_xlat3.xyz = u_xlat3.xyz + u_xlat6.xyz;
    u_xlat6 = u_xlat1.xyxy * vec4(2.0, 1.0, -4.0, -2.0) + v_TexCoord.xyxy;
    u_xlat7 = texture(sampler2D(m_Texture, m_Sampler), u_xlat6.xy);
    u_xlat6 = texture(sampler2D(m_Texture, m_Sampler), u_xlat6.zw);
    u_xlat0.xyz = u_xlat7.xyz * u_xlat7.xyz + u_xlat0.xyz;
    u_xlat3.xyz = u_xlat3.xyz + u_xlat7.xyz;
    u_xlat7 = u_xlat1.xyxy * vec4(0.0, 2.0, -3.0, -2.0) + v_TexCoord.xyxy;
    u_xlat8 = texture(sampler2D(m_Texture, m_Sampler), u_xlat7.xy);
    u_xlat7 = texture(sampler2D(m_Texture, m_Sampler), u_xlat7.zw);
    u_xlat0.xyz = u_xlat8.xyz * u_xlat8.xyz + u_xlat0.xyz;
    u_xlat3.xyz = u_xlat3.xyz + u_xlat8.xyz;
    u_xlat19.xy = u_xlat1.xy * vec2(1.0, 2.0) + v_TexCoord.xy;
    u_xlat8 = texture(sampler2D(m_Texture, m_Sampler), u_xlat19.xy);
    u_xlat0.xyz = u_xlat8.xyz * u_xlat8.xyz + u_xlat0.xyz;
    u_xlat3.xyz = u_xlat3.xyz + u_xlat8.xyz;
    u_xlat19.xy = u_xlat1.xy * vec2(2.0, 2.0) + v_TexCoord.xy;
    u_xlat8 = u_xlat1.xyxy * vec4(-4.0, -4.0, -3.0, -3.0) + v_TexCoord.xyxy;
    u_xlat1 = texture(sampler2D(m_Texture, m_Sampler), u_xlat19.xy);
    u_xlat0.xyz = u_xlat1.xyz * u_xlat1.xyz + u_xlat0.xyz;
    u_xlat1.xyz = u_xlat1.xyz + u_xlat3.xyz;
    u_xlat1.xyz = u_xlat1.xyz * vec3(0.111111111);
    u_xlat3.xyz = u_xlat1.xyz * u_xlat1.xyz;
    u_xlat0.xyz = u_xlat0.xyz * vec3(0.111111111) + (-u_xlat3.xyz);
    u_xlat0.x = abs(u_xlat0.y) + abs(u_xlat0.x);
    u_xlat0.x = abs(u_xlat0.z) + u_xlat0.x;
    u_xlat3 = texture(sampler2D(m_Texture, m_Sampler), u_xlat8.xy);
    u_xlat8 = texture(sampler2D(m_Texture, m_Sampler), u_xlat8.zw);
    u_xlat9.xyz = u_xlat8.xyz * u_xlat8.xyz;
    u_xlat9.xyz = u_xlat3.xyz * u_xlat3.xyz + u_xlat9.xyz;
    u_xlat3.xyz = u_xlat3.xyz + u_xlat8.xyz;
    u_xlat3.xyz = u_xlat2.xyz + u_xlat3.xyz;
    u_xlat9.xyz = u_xlat2.xyz * u_xlat2.xyz + u_xlat9.xyz;
    u_xlat9.xyz = u_xlat4.xyz * u_xlat4.xyz + u_xlat9.xyz;
    u_xlat2.xyz = u_xlat4.xyz + u_xlat3.xyz;
    u_xlat2.xyz = u_xlat8.xyz + u_xlat2.xyz;
    u_xlat9.xyz = u_xlat8.xyz * u_xlat8.xyz + u_xlat9.xyz;
    u_xlat9.xyz = u_xlat5.xyz * u_xlat5.xyz + u_xlat9.xyz;
    u_xlat9.xyz = u_xlat6.xyz * u_xlat6.xyz + u_xlat9.xyz;
    u_xlat9.xyz = u_xlat7.xyz * u_xlat7.xyz + u_xlat9.xyz;
    u_xlat9.xyz = u_xlat5.xyz * u_xlat5.xyz + u_xlat9.xyz;
    u_xlat2.xyz = u_xlat5.xyz + u_xlat2.xyz;
    u_xlat2.xyz = u_xlat6.xyz + u_xlat2.xyz;
    u_xlat2.xyz = u_xlat7.xyz + u_xlat2.xyz;
    u_xlat2.xyz = u_xlat5.xyz + u_xlat2.xyz;
    u_xlat2.xyz = u_xlat2.xyz * vec3(0.111111111);
    u_xlat3.xyz = u_xlat2.xyz * u_xlat2.xyz;
    u_xlat9.xyz = u_xlat9.xyz * vec3(0.111111111) + (-u_xlat3.xyz);
    u_xlat9.x = abs(u_xlat9.y) + abs(u_xlat9.x);
    u_xlat9.x = abs(u_xlat9.z) + u_xlat9.x;
    u_xlat18 = min(u_xlat9.x, 100.0);
    u_xlatb9 = u_xlat9.x < 100.0;
    u_xlatb0 = u_xlat0.x < u_xlat18;
    u_xlat2.w = 1.0;
    u_xlat2 = u_xlatb9 ? u_xlat2 : vec4(0.0);
    u_xlat1.w = 1.0;
    o_Colour = u_xlatb0 ? u_xlat1 : u_xlat2;
}

#endif