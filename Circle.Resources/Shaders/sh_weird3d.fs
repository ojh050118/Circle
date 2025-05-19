#ifndef WEIRD3D_FS
#define WEIRD3D_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 colour;

void main(void)
{
	vec4 u_xlat0;
	vec4 u_xlat1;
	vec3 u_xlat2;
	vec4 u_xlat3;
	vec4 u_xlat4;
	vec2 u_xlat13;
	float u_xlat18;

	u_xlat0.xyz = v_TexCoord.xyx * vec3(500.0);
	u_xlat0.x = floor(u_xlat0.x);
	u_xlat0.y = floor(u_xlat0.y);
	u_xlat0.z = floor(u_xlat0.z);
	u_xlat1.xy = u_xlat0.xy * vec2(0.002);

	for (int i = 0; i < 80; i++)
	{
		u_xlat18 = u_xlat1.y - float(i) * 0.002;
		u_xlat1.w = u_xlat18 + u_xlat18;
		u_xlat3.xy = u_xlat1.xw + vec2(0.75, 0.375);
		u_xlat13.xy = u_xlat3.xy * vec2(0.4);
		u_xlat4 = texture(sampler2D(m_Texture, m_Sampler), u_xlat13.xy);
		u_xlat3.z = u_xlat4.x * 0.2;
		u_xlat18 += u_xlat4.x * 0.2;
		u_xlat2.xyz = u_xlat1.y < u_xlat18 ? u_xlat3.xyz : u_xlat2.xyz;
	}

	u_xlat1 = texture(sampler2D(m_Texture, m_Sampler), u_xlat2.xy * vec2(0.4));
	u_xlat3 = (u_xlat2.xyxy + vec4(0.0, 0.004, 0.0, -0.004)) * vec4(0.4);
	u_xlat4 = texture(sampler2D(m_Texture, m_Sampler), u_xlat3.xy);
	u_xlat3 = texture(sampler2D(m_Texture, m_Sampler), u_xlat3.zw);
	u_xlat3.y = max(u_xlat4.x * 0.2 - u_xlat3.x * 0.2, 0.003);
	u_xlat4 = (u_xlat2.xyxy + vec4(0.004, 0.0, -0.004, 0.0)) * vec4(0.4);
	u_xlat3.x = max(texture(sampler2D(m_Texture, m_Sampler), u_xlat4.xy).x * 0.2 - texture(sampler2D(m_Texture, m_Sampler), u_xlat4.zw).x * 0.2, 0.003);
	u_xlat3.xy *= vec2(0.008);
	u_xlat3.z = -0.0000064;
	u_xlat0.x = dot(vec2(inversesqrt(dot(u_xlat3.xyz, u_xlat3.xyz))) * u_xlat3.xz, vec2(0.707106769, -0.707106769));
	u_xlat1.xyz = vec3(max(u_xlat0.x, 0.2)) * u_xlat1.xyz;
	u_xlat0.x *= u_xlat0.x;
	u_xlat1.xyz *= vec3(u_xlat0.x * u_xlat0.x * u_xlat0.x + 1.0);
	u_xlat2.xy = u_xlat0.zy * vec2(0.0016, 0.002) - vec2(0.4, 0.0);
	u_xlat0.x = dot(u_xlat2.xyz, u_xlat2.xyz);
	
	colour = vec4(u_xlat1.xyz * vec3(1.0 - u_xlat0.x) + vec3(u_xlat0.x * 0.8), 1.0);
}

#endif