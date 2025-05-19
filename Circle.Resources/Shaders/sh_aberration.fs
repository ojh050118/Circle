#ifndef ABERRATION_FS
#define ABERRATION_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 colour;

void main(void)
{
	vec4 u_xlat0 = vec4(0.0);
	vec4 u_xlat1;
	vec4 u_xlat2;

	u_xlat0.y = intensity;
	u_xlat0 = u_xlat0.yxxy + v_TexCoord.xyxy;
	u_xlat1 = texture(sampler2D(m_Texture, m_Sampler), u_xlat0.xy);
	u_xlat0 = texture(sampler2D(m_Texture, m_Sampler), u_xlat0.zw);
	u_xlat2 = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord.xy);

	colour = vec4(u_xlat1.x, u_xlat2.y, u_xlat0.z, u_xlat2.w);
}

#endif