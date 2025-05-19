#ifndef GRAIN_FS
#define GRAIN_FS

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
	vec3 u_xlat0;

	u_xlat0.xy = v_TexCoord + vec2(4.0);
	u_xlat0.x *= u_xlat0.y * time;
	u_xlat0.xyz = u_xlat0.xxx * vec3(10.0, 0.769230783, 0.08130081);
	u_xlat0.xy = u_xlat0.xx - floor(u_xlat0.yz) * vec2(13.0, 123.0) + vec2(1.0);
	u_xlat0.x *= u_xlat0.y;
	u_xlat0.x -= floor(u_xlat0.x * 100.0) * 0.01 + 0.005;
	colour = vec4(u_xlat0.xxx * intensity * 32.0 + texture(sampler2D(m_Texture, m_Sampler), v_TexCoord).rgb, 1.0);
}

#endif