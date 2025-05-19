#ifndef LED_FS
#define LED_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
    mediump float time;
    mediump vec2 resolution;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 colour;

void main(void)
{
    vec4 u_xlat0;
    vec4 u_xlat1;
    vec3 u_xlat2;
    bvec2 u_xlatb2;
    vec4 u_xlat3;
    vec3 u_xlat4;
    bvec2 u_xlatb7;
    vec2 u_xlat12;
    float u_xlat15;

	u_xlat0.xw = v_TexCoord * resolution;
	u_xlat1.xw = u_xlat0.xw / vec2(intensity);
	u_xlatb2.xy = greaterThanEqual(u_xlat1.xwxx, -u_xlat1.xwxx).xy;
	u_xlat12.x = fract(abs(u_xlat1.x));
	u_xlat12.y = fract(abs(u_xlat1.w));
	u_xlat1.x = floor(u_xlat1.x);
	u_xlat1.w = floor(u_xlat1.w);
	u_xlat1.xw *= vec2(intensity) / resolution;
	u_xlat3 = texture(sampler2D(m_Texture, m_Sampler), u_xlat1.xw);
	u_xlat1.x = u_xlatb2.x ? u_xlat12.x : -u_xlat12.x;
	u_xlat1.w = u_xlatb2.y ? u_xlat12.y : -u_xlat12.y;
	u_xlat1.xw *= vec2(intensity);
	u_xlat2.xyz = vec3(time, intensity, intensity) * vec3(5.6, 0.333333333, 0.666666666);
	u_xlatb7.xy = lessThan(u_xlat1.xxxx, u_xlat2.yzyy).xy;
	u_xlat15 = min(sin(u_xlat0.w * 6.0 + u_xlat2.x) + 1.25, 1.0) * 0.5;
	u_xlat1.yz = u_xlatb7.y ? vec2(u_xlat3.y, 0.0) : vec2(0.0, u_xlat3.z);
	u_xlat4.yz = vec2(0.0);
	u_xlat1.x = 0.0;
	u_xlat4.x = u_xlat3.x;
	u_xlat0.xyz = (u_xlat1.w < intensity ? (u_xlatb7.x ? u_xlat4.xyz : u_xlat1.xyz) : vec3(0.0)) + u_xlat3.xyz;
	u_xlat1.xyz = max(u_xlat0.xyz - vec3(0.2), vec3(0.0)); // u_xlat0.xyz - vec3(0.2);
	u_xlat0.xyz -= u_xlat1.xyz;
	colour = vec4(vec3(u_xlat15) * u_xlat0.xyz + u_xlat1.xyz, 1.0);
}

#endif
