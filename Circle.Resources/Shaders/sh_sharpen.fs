#ifndef SHARPEN_FS
#define SHARPEN_FS

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
    vec4 u_xlat1;
    vec4 u_xlat2;
    vec4 u_xlat3;
    vec4 u_xlat4;
    vec4 u_xlat5;
    vec4 u_xlat6;
    float u_xlat7;

    u_xlat0.x = 37.0;
    u_xlat7 = 36.0;
    u_xlat1.y = intensity / resolution.x;
    u_xlat1.x = 0.0;
    u_xlat2 = u_xlat1.yxxy + v_TexCoord.xyxy;
    u_xlat1 = v_TexCoord.xyxy - u_xlat1.yxxy;
    u_xlat3 = texture(sampler2D(m_Texture, m_Sampler), u_xlat2.xy);
    u_xlat2 = texture(sampler2D(m_Texture, m_Sampler), u_xlat2.zw);
    u_xlat4 = texture(sampler2D(m_Texture, m_Sampler), u_xlat1.xy);
    u_xlat1 = texture(sampler2D(m_Texture, m_Sampler), u_xlat1.zw);
    u_xlat5 = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord);
    u_xlat6 = u_xlat4 + u_xlat5;
    u_xlat6 = u_xlat3 + u_xlat6;
    u_xlat6 = u_xlat1 + u_xlat6;
    u_xlat6 = u_xlat2 + u_xlat6;
    u_xlat6 = vec4(u_xlat7) * u_xlat6;
    u_xlat6 = u_xlat6 * vec4(0.2);
    u_xlat0 = u_xlat0.xxxx * u_xlat5 - u_xlat6;
    u_xlat6 = min(u_xlat4, u_xlat5);
    u_xlat4 = max(u_xlat4, u_xlat5);
    u_xlat4 = max(u_xlat3, u_xlat4);
    u_xlat3 = min(u_xlat3, u_xlat6);
    u_xlat3 = min(u_xlat1, u_xlat3);
    u_xlat1 = max(u_xlat1, u_xlat4);
    u_xlat1 = max(u_xlat2, u_xlat1);
    u_xlat2 = min(u_xlat2, u_xlat3);
    u_xlat0 = max(u_xlat0, u_xlat2);

    o_Colour = min(u_xlat1, u_xlat0);
}

#endif