#include "sh_Utils.h"

varying vec2 v_TexCoord;

uniform sampler2D m_Sampler;
uniform mediump float time;
uniform mediump vec2 resolution;

void main(void)
{
    vec4 u_xlat0;
    vec4 u_xlat1;
    bvec2 u_xlatb1;
    vec3 u_xlat2;
    float u_xlat3;
    vec3 u_xlat4;
    float u_xlat6;
    bvec2 u_xlatb7;
    float u_xlat9;

	u_xlat0.xy = v_TexCoord * vec2(2.2) - vec2(1.1);
	u_xlat6 = abs(u_xlat0.y) * 0.2;
	u_xlat4.x = (u_xlat6 * u_xlat6 + 1.0) * u_xlat0.x;
	u_xlat0.x = abs(u_xlat4.x) * 0.25;
	u_xlat0.x = u_xlat0.x * u_xlat0.x + 1.0;
	u_xlat4.yz = u_xlat0.xx * u_xlat0.yy;
	u_xlat0.xyz = u_xlat4.xyz * vec3(0.5) + vec3(0.5);
	u_xlat9 = u_xlat0.x * 0.92;
	u_xlat1.yzw = u_xlat0.zxy * vec3(0.92) + vec3(0.041, 0.04, 0.04);
	u_xlat0.xy = sin(u_xlat1.ww * vec2(21.0, 29.0) + vec2(time) * vec2(0.3, 0.7));
	u_xlat0.x *= u_xlat0.y * sin(time * 0.33 + 0.3 + u_xlat1.w * 31.0);
	u_xlat1.x = u_xlat0.x * 0.0017 + u_xlat1.z + 0.001;
	u_xlat2 = toSRGB(texture2D(m_Sampler, u_xlat1.xy)).xyz + vec3(0.05);
	u_xlat1.x = (u_xlat0.x * 0.0017 + 0.025) * 0.75 + u_xlat9;
	u_xlat0.xyz = toSRGB(texture2D(m_Sampler, u_xlat1.xy + vec2(0.041, -0.015))).xyz * vec3(0.08, 0.05, 0.08) + u_xlat2;
	u_xlat0.xyz = clamp(u_xlat0.xyz * vec3(0.6) + u_xlat0.xyz * u_xlat0.xyz * vec3(0.4), 0.0, 1.0);
	u_xlat9 = u_xlat1.z * u_xlat1.w * 16.0;
	u_xlat1.xy = vec2(1.0) - u_xlat1.zw;
	u_xlat9 *= u_xlat1.x * u_xlat1.y;
	u_xlat0.xyz *= vec3(exp2(log2(u_xlat9) * 0.3));
	u_xlat0.w = log2(sin(u_xlat1.w * resolution.y * 1.5 + time * 3.5) * 0.35 + 0.35);
	u_xlat0 *= vec4(2.66, 2.94, 2.66, 1.7);
	u_xlat0.xyz *= vec3(exp2(u_xlat0.w) * 0.7 + 0.4) * vec3(sin(time * 110.0) * 0.01 + 1.0);
	u_xlatb1.xy = lessThan(u_xlat1.zwzz, vec4(0.0)).xy;
	u_xlatb7.xy = lessThan(vec4(1.0), u_xlat1.zwzw).xy;

	gl_FragColor = vec4(u_xlat0.xyz * (1.0 - float(u_xlatb7.y || u_xlatb1.y || u_xlatb7.x || u_xlatb1.x)), 1.0);
}
