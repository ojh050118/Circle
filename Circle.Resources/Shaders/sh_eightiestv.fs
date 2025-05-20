#ifndef EIGHTIESTV_FS
#define EIGHTIESTV_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;
layout (location = 3) in mediump vec4 v_TexRect;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float time;
    mediump vec2 resolution;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
	vec4 u_xlat0 = vec4(0.0);
    vec4 u_xlat1;
    bvec2 u_xlatb1;
    vec3 u_xlat3;
    bvec2 u_xlatb7;
    float u_xlat9;

	u_xlat3.xyz = v_TexCoord.yyx * vec3(21.0, 29.0, 16.0);
	u_xlat0.x = time * 0.33 + 0.3 + v_TexCoord.y * 31.0;
	u_xlat0.yz = vec2(time) * vec2(0.3, 0.7) + u_xlat3.xy;
	u_xlat0.xyz = sin(u_xlat0.xyz);
	u_xlat9 = u_xlat3.z * v_TexCoord.y;
	u_xlat0.x *= u_xlat0.z * u_xlat0.y;
	u_xlat1.z = u_xlat0.x * 0.0017 + v_TexCoord.x + 0.001;
	u_xlat0.x = u_xlat0.x * 0.0017 + 0.01875;
	u_xlat0.y = -0.015;
	u_xlat1.xyw = v_TexCoord.xyy + vec3(0.001);
	u_xlat0.xy += u_xlat1.xy;
	u_xlat0.xyz = texture(sampler2D(m_Texture, m_Sampler), u_xlat0.xy).xyz * vec3(0.08, 0.05, 0.08) + texture(sampler2D(m_Texture, m_Sampler), u_xlat1.zw).xyz + vec3(0.05);
	u_xlat0.xyz = clamp(u_xlat0.xyz * vec3(0.6) + u_xlat0.xyz * u_xlat0.xyz * vec3(0.4), 0.0, 1.0);
	u_xlat1.xy = vec2(1.0) - v_TexCoord;
	u_xlat0.xyz *= vec3(exp2(log2(u_xlat9 * u_xlat1.x * u_xlat1.y) * 0.3));
	u_xlat0.w = log2(sin(time * 3.5 + v_TexCoord.y * resolution.y * 1.5) * 0.35 + 0.35);
	u_xlat0 *= vec4(3.66, 2.94, 2.66, 1.7);
	u_xlat0.xyz *= vec3(exp2(u_xlat0.w) * 0.7 + 0.4);
	u_xlat1.xy = vec2(time) * vec2(2.6, 110.0);
	u_xlat0.xyz *= vec3(min(floor(sin(v_TexCoord.y * 6.0 + u_xlat1.x) + 1.95), 1.1) * 0.25 + (sin(u_xlat1.y) * 0.01 + 1.0));
	u_xlatb1.xy = lessThan(v_TexCoord.xyxx, vec4(0.0)).xy;
	u_xlatb7.xy = lessThan(vec4(1.0), v_TexCoord.xyxy).xy;

	o_Colour = vec4(u_xlat0.xyz * (1.0 - float(u_xlatb7.y || u_xlatb1.y || u_xlatb7.x || u_xlatb1.x)), 1.0);
}

#endif
