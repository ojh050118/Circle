#ifndef SUPERDOT_FS
#define SUPERDOT_FS

#include "sh_Utils.h"
#include "sh_CircleUtils.h"

layout (location = 2) in mediump vec2 v_TexCoord;
layout (location = 3) in mediump vec4 v_TexRect;

layout (std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump vec2 resolution;
};

layout (set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout (set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout (location = 0) out vec4 o_Colour;

void main(void)
{
    vec2 u_xlat0;
    vec4 u_xlat1;
    vec2 u_xlat2;
    vec2 u_xlat6;
    vec2 u_xlat8;

    u_xlat0.xy = v_TexCoord * resolution;
    u_xlat0.xy = u_xlat0.xy * vec2(0.125);
    u_xlat0.xy = floor(u_xlat0.xy);
    u_xlat0.xy = u_xlat0.xy * vec2(8.0);
    u_xlat6.xy = v_TexCoord * resolution - u_xlat0.xy;
    u_xlat0.xy = u_xlat0.xy / resolution;
    u_xlat1 = texture(Sampler2D(m_Texture, m_Sampler), u_xlat0.xy);
    u_xlat0.xy = u_xlat6.xy * vec2(0.125);
    u_xlat6.xy = u_xlat6.xy * u_xlat0.xy;
    u_xlat2.xy = u_xlat0.xy * u_xlat6.xy;
    u_xlat0.xy = u_xlat0.xy * u_xlat0.xy;
    u_xlat2.xy = u_xlat2.xy * vec2(0.0625);
    u_xlat8.xy = u_xlat6.xy * vec2(0.03125);
    u_xlat0.xy = u_xlat0.xy * u_xlat8.xy + (-u_xlat2.xy);
    u_xlat0.xy = u_xlat6.xy * vec2(0.03125) + u_xlat0.xy;
    u_xlat0.xy = u_xlat0.xy * vec2(32.0) + vec2(0.5);
    u_xlat1 = u_xlat0.xxxx * u_xlat1;

    o_Colour = vec4(u_xlat0.yyy * u_xlat1.rgb, 1.0);
}

#endif
