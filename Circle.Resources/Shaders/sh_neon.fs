#ifndef NEON_FS
#define NEON_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 colour;

void main(void)
{
	vec4 u_xlat0;
	vec4 u_xlat1;
	vec4 u_xlat2;
	vec4 u_xlat3;
	vec4 u_xlat4;
	vec4 u_xlat5;
	vec4 u_xlat6;

	u_xlat0.x = 0.001;
	u_xlat1 = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord - u_xlat0.xx);
	u_xlat0.yz = -u_xlat0.xx;
	u_xlat2 = u_xlat0.yxxx + v_TexCoord.xyxy;
	u_xlat3 = texture(sampler2D(m_Texture, m_Sampler), u_xlat2.xy);
	u_xlat2 = texture(sampler2D(m_Texture, m_Sampler), u_xlat2.zw);
	u_xlat4 = u_xlat3 - u_xlat1;
	u_xlat0.w = 0.0;
	u_xlat5 = u_xlat0.zwwx + v_TexCoord.xyxy;
	u_xlat6 = u_xlat0.xwxz + v_TexCoord.xyxy;
	u_xlat0.xy = u_xlat0.wx * vec2(1.0, -1.0) + v_TexCoord;
	u_xlat1 += texture(sampler2D(m_Texture, m_Sampler), u_xlat5.xy) * vec4(2.0) + u_xlat3 - u_xlat2;
	u_xlat0 = texture(sampler2D(m_Texture, m_Sampler), u_xlat5.zw) * vec4(2.0) + u_xlat4 - texture(sampler2D(m_Texture, m_Sampler), u_xlat0.xy) * vec4(2.0) + u_xlat2;
	u_xlat2 = texture(sampler2D(m_Texture, m_Sampler), u_xlat6.zw);
	u_xlat1 -= texture(sampler2D(m_Texture, m_Sampler), u_xlat6.xy) * vec4(2.0) + u_xlat2;
	u_xlat0 -= u_xlat2;
	u_xlat0 *= u_xlat0;
	u_xlat0 += u_xlat1 * u_xlat1;
	colour = vec4(sqrt(u_xlat0.xyz), 1.0);
}

#endif