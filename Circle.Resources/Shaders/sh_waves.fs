#ifndef WAVES_FS
#define WAVES_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
    mediump float time;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 colour;

void main(void)
{
	vec2 u_xlat0;
	vec2 u_xlat1;

	u_xlat0.x = intensity - 10.0;
	u_xlat1.xy = vec2(time) * vec2(0.07, 0.1) + v_TexCoord.yy;
	u_xlat0.x *= u_xlat1.y;
	u_xlat0.y = u_xlat1.x * intensity;
	u_xlat0.xy = sin(u_xlat0.xy);
	u_xlat0.x *= 0.005;
	u_xlat0.x += u_xlat0.y * 0.009 + v_TexCoord.x;
	u_xlat0.y = v_TexCoord.y;
	colour = vec4(texture(sampler2D(m_Texture, m_Sampler), u_xlat0.xy).rgb, 1.0);
}

#endif