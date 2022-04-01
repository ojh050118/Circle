#include "sh_Utils.h"

varying vec2 v_TexCoord;
varying vec4 v_TexRect;

uniform sampler2D m_Sampler;
uniform mediump vec2 resolution;
uniform mediump float time;

void main(void)
{
    vec4 u_xlat0;
    vec4 u_xlat1;
    bvec2 u_xlatb1;
    vec3 u_xlat2;
    vec3 u_xlat3;
    vec2 u_xlat4;
    bvec2 u_xlatb4;
    vec3 u_xlat5;
    vec2 u_xlat8;
    float u_xlat12;

	u_xlat0.xy = (v_TexCoord.xy - vec2(0.5)) * vec2(2.2);
	u_xlat8.x = abs(u_xlat0.y) * 0.2;
	u_xlat5.x = (u_xlat8.x * u_xlat8.x + 1.0) * u_xlat0.x;
	u_xlat0.x = abs(u_xlat5.x) * 0.25;
	u_xlat0.x = u_xlat0.x * u_xlat0.x + 1.0;
	u_xlat5.yz = u_xlat0.xx * u_xlat0.yy;
	u_xlat0.xyz = u_xlat5.xyz * vec3(0.5) + vec3(0.5);
	u_xlat12 = u_xlat0.x * 0.92;
	u_xlat1.yzw = u_xlat0.zxy * vec3(0.92) + vec3(0.041, 0.04, 0.04);
	u_xlat0.x = time * 0.33 + 0.3 + u_xlat1.w * 31.0;
	u_xlat2.xyz = u_xlat1.wwz * vec3(21.0, 29.0, 250.0);
	u_xlat0.yz = vec2(time) * vec2(0.3, 0.7) + u_xlat2.xy;
	u_xlat2.x = floor(u_xlat2.z);
	u_xlat0.xyz = sin(u_xlat0.xyz);
	u_xlat0.x *= u_xlat0.z * u_xlat0.y;
	u_xlat1.x = u_xlat0.x * 0.0017 + u_xlat1.z + 0.001;
	u_xlat3 = toSRGB(texture2D(m_Sampler, u_xlat1.xy)).xyz + vec3(0.05);
	u_xlat1.x = (u_xlat0.x * 0.0017 + 0.025) * 0.75 + u_xlat12;
	u_xlat0.xyz = toSRGB(texture2D(m_Sampler, u_xlat1.xy + vec2(0.041, -0.015))).xyz * vec3(0.08, 0.05, 0.08) + u_xlat3.xyz;
	u_xlat0.xyz = clamp(u_xlat0.xyz * vec3(0.6) + u_xlat0.xyz * u_xlat0.xyz * vec3(0.4), 0.0, 1.0);
	u_xlat12 = u_xlat1.z * u_xlat1.w * 16.0;
	u_xlat1.xy = vec2(1.0) - u_xlat1.zw;
	u_xlat12 *= u_xlat1.x * u_xlat1.y;
	u_xlat0.xyz *= vec3(exp2(log2(u_xlat12) * 0.3));
	u_xlat0.w = log2(sin(u_xlat1.w * resolution.y * 1.5 + time * 3.5) * 0.35 + 0.35);
	u_xlat0 *= vec4(3.66, 2.94, 2.66, 1.7);
	u_xlat0.xyz *= vec3(exp2(u_xlat0.w) * 0.7 + 0.4);
	u_xlat0.x = dot(u_xlat0.xyz, vec3(0.222, 0.707, 0.071));
	u_xlat2.y = floor(u_xlat1.w * 250.0);
	u_xlat4.x = fract(sin(dot(u_xlat2.xy * vec2(time) * vec2(0.001), vec2(12.9898005, 78.2330017))) * 43758.5469) + 0.25;
	u_xlat8.xy = vec2(time) * vec2(2.6, 110.0);
	u_xlat8.x += u_xlat1.w * 6.0;
	u_xlat8.xy = sin(u_xlat8.xy);
	u_xlat8.x += 1.95;
	u_xlat4.y = floor(u_xlat8.x);
	u_xlat4.xy = min(u_xlat4.xy, vec2(1.0, 1.1));
	u_xlat0.x *= u_xlat4.x * u_xlat4.y + (u_xlat8.y * 0.01 + 1.0);
	u_xlatb4.xy = lessThan(u_xlat1.zwzz, vec4(0.0)).xy;
	u_xlatb1.xy = lessThan(vec4(1.0, 1.0, 0.0, 0.0), u_xlat1.zwzz).xy;

	gl_FragColor = vec4(u_xlat0.xxx * (1.0 - float(u_xlatb4.y || u_xlatb1.y || u_xlatb4.x || u_xlatb1.x)), 1.0);
}
