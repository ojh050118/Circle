#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;

uniform lowp sampler2D m_Sampler;
uniform mediump float intensity;
uniform mediump float time;

void main(void)
{
	vec2 u_xlat0;
	vec2 u_xlat1;

	u_xlat0.x = intensity - 10.0;
	u_xlat1.xy = vec2(time) * vec2(0.07, 0.1) + v_TexCoord.yy;
	u_xlat0.x *= u_xlat1.y;
	u_xlat0.y = u_xlat1.x * intensity;
	u_xlat0.xy = sin(u_xlat0.xy);
	u_xlat0.x *= 0.005;
	u_xlat0.x += u_xlat0.y * 0.009 + v_TexCoord.x;
	u_xlat0.y = v_TexCoord.y;
	gl_FragColor = toSRGB(vec4(texture2D(m_Sampler, u_xlat0.xy).rgb, 1.0));
}