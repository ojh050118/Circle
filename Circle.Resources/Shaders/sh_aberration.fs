#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;

uniform lowp sampler2D m_Sampler;
uniform mediump float intensity;

void main(void)
{
	vec4 u_xlat0;
	vec4 u_xlat1;
	vec4 u_xlat2;

	u_xlat0.y = intensity;
	u_xlat0 = u_xlat0.yxxy + v_TexCoord.xyxy;
	u_xlat1 = toSRGB(texture2D(m_Sampler, u_xlat0.xy));
	u_xlat0 = toSRGB(texture2D(m_Sampler, u_xlat0.zw));
	u_xlat2 = toSRGB(texture2D(m_Sampler, v_TexCoord.xy));

	gl_FragColor = vec4(u_xlat1.x, u_xlat2.y, u_xlat0.z, u_xlat2.w);
}