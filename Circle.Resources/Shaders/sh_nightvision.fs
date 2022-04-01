#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;

uniform lowp sampler2D m_Sampler;
uniform mediump float time;

void main(void)
{
	vec4 u_xlat0;
	vec4 u_xlat1;
	float u_xlat2;
	vec2 u_xlat3;
	float u_xlat6;

	float _Linecount = 90.0; //(1-150)

	u_xlat0.x = v_TexCoord.y * _Linecount;
	u_xlat6 = fract(abs(u_xlat0.x));
	u_xlat1.y = floor(u_xlat0.x) / _Linecount;
	u_xlat0.x = log2(1.0 - abs((u_xlat0.x >= -u_xlat0.x ? u_xlat6 : -u_xlat6) * 2.0 - 1.0));
	u_xlat1.x = v_TexCoord.x;
	u_xlat2 = toSRGB(texture2D(m_Sampler,vec2(time) * vec2(9.0, 7.0) + u_xlat1.xy)).x;
	u_xlat0.w = u_xlat2 * 0.3 + 0.7;
	u_xlat3.x = dot(toSRGB(texture2D(m_Sampler, u_xlat2 * vec2(0.01, 0.0) + u_xlat1.xy)).xyz, vec3(0.2, 0.5, 0.3));
	u_xlat3.y = (1.0 - u_xlat3.x * u_xlat0.w) * 2.0 + 0.5;
	u_xlat0.xy = u_xlat0.xw * u_xlat3.yx;
	u_xlat0.x = clamp(exp2(u_xlat0.x) * u_xlat0.y, 0.0, 1.0);
	u_xlat1 = u_xlat0.xxxx * vec4(0.4, 1.0, 0.2, 2.0) + vec4(1.0 - u_xlat0.x * 2.0) * vec4(0.0, 0.1, 0.0, 1.0);
	u_xlat3.x = u_xlat0.x * 2.0 - 1.0;
	u_xlat0 = u_xlat0.x < 0.5 ? u_xlat1 : (u_xlat3.xxxx * vec4(0.9, 1.0, 0.6, 1.0) + vec4(1.0 - u_xlat3.x) * vec4(0.2, 0.5, 0.1, 1.0));
	u_xlat1.xy = abs(v_TexCoord - vec2(0.5)) * vec2(1.3);
	
	gl_FragColor = vec4(u_xlat0.rgb * vec3(1.0 - u_xlat1.x * u_xlat1.x - u_xlat1.y * u_xlat1.y), 1.0);
}