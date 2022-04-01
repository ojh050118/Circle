#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;
uniform lowp sampler2D m_Sampler;

void main(void)
{
	vec4 u_xlat0;
	vec4 u_xlat1;
	vec4 u_xlat2;
	vec4 u_xlat3;
	vec4 u_xlat4;
	vec4 u_xlat5;
	vec4 u_xlat6;

	u_xlat0.x = 0.001;
	u_xlat1 = toSRGB(texture2D(m_Sampler, v_TexCoord - u_xlat0.xx));
	u_xlat0.yz = -u_xlat0.xx;
	u_xlat2 = u_xlat0.yxxx + v_TexCoord.xyxy;
	u_xlat3 = toSRGB(texture2D(m_Sampler, u_xlat2.xy));
	u_xlat2 = toSRGB(texture2D(m_Sampler, u_xlat2.zw));
	u_xlat4 = u_xlat3 - u_xlat1;
	u_xlat0.w = 0.0;
	u_xlat5 = u_xlat0.zwwx + v_TexCoord.xyxy;
	u_xlat6 = u_xlat0.xwxz + v_TexCoord.xyxy;
	u_xlat0.xy = u_xlat0.wx * vec2(1.0, -1.0) + v_TexCoord;
	u_xlat1 += toSRGB(texture2D(m_Sampler, u_xlat5.xy)) * vec4(2.0) + u_xlat3 - u_xlat2;
	u_xlat0 = toSRGB(texture2D(m_Sampler, u_xlat5.zw)) * vec4(2.0) + u_xlat4 - toSRGB(texture2D(m_Sampler, u_xlat0.xy)) * vec4(2.0) + u_xlat2;
	u_xlat2 = toSRGB(texture2D(m_Sampler, u_xlat6.zw));
	u_xlat1 -= toSRGB(texture2D(m_Sampler, u_xlat6.xy)) * vec4(2.0) + u_xlat2;
	u_xlat0 -= u_xlat2;
	u_xlat0 *= u_xlat0;
	u_xlat0 += u_xlat1 * u_xlat1;
	gl_FragColor = vec4(sqrt(u_xlat0.xyz), 1.0);
}