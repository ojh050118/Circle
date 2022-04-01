#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;

uniform lowp sampler2D m_Sampler;
uniform mediump float intensity;
uniform mediump float time;

void main(void)
{
	vec3 u_xlat0;

	u_xlat0.xy = v_TexCoord + vec2(4.0);
	u_xlat0.x *= u_xlat0.y * time;
	u_xlat0.xyz = u_xlat0.xxx * vec3(10.0, 0.769230783, 0.08130081);
	u_xlat0.xy = u_xlat0.xx - floor(u_xlat0.yz) * vec2(13.0, 123.0) + vec2(1.0);
	u_xlat0.x *= u_xlat0.y;
	u_xlat0.x -= floor(u_xlat0.x * 100.0) * 0.01 + 0.005;
	gl_FragColor = vec4(u_xlat0.xxx * intensity * 32.0 + toSRGB(texture2D(m_Sampler, v_TexCoord)).rgb, 1.0);
}