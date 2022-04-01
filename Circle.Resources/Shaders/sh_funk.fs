#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;

uniform lowp sampler2D m_Sampler;
uniform mediump float time;

void main(void)
{
	vec4 u_xlat0;
	vec4 u_xlat1;
	vec2 u_xlat2;
	bool u_xlatb3;
	bvec2 u_xlatb6;

	u_xlat0 = vec4(time) * vec4(2.70440626, 2.12428117, 1.9024688, 2.43993759) + vec4(1.09960902, 0.455078006, 8.44726562, 610.460938);
	u_xlat0.y = u_xlat0.y - v_TexCoord.x * 10.0;
	u_xlat0.xzw += v_TexCoord.xyy * vec3(7.5, 5.0, 12.5);
	u_xlat0 = sin(u_xlat0);
	u_xlat0.x = fract((u_xlat0.x + u_xlat0.y + u_xlat0.z + u_xlat0.w) * 0.25 + 1.0) + dot(toSRGB(texture2D(m_Sampler, v_TexCoord.xy)).xzy, vec3(0.2, 0.2, 0.4));
	u_xlatb3 = u_xlat0.x >= -u_xlat0.x;
	u_xlat0.x = fract(abs(u_xlat0.x));
	u_xlat0.x = u_xlatb3 ? u_xlat0.x : -u_xlat0.x;
	u_xlatb6.xy = lessThan(u_xlat0.xxxx, vec4(0.333333343, 0.666666687, 0.333333343, 0.666666687)).xy;
	u_xlat1.xyz = vec3(2.0, 4.0, 6.0) - u_xlat0.xxx * vec3(6.0);
	u_xlat2.xy = clamp(u_xlat0.xx * vec2(6.0) - vec2(4.0, 2.0), 0.0, 1.0);
	u_xlat1.xy = clamp(u_xlat1.xy, 0.0, 1.0);
	
	gl_FragColor = vec4(u_xlat1.x + u_xlat2.x, u_xlatb6.x ? clamp(u_xlat0.x * 6.0, 0.0, 1.0) : u_xlat1.y, u_xlatb6.y ? u_xlat2.y : min(u_xlat1.z, 1.0), 1.0);
}