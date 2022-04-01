#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;
varying vec4 v_TexRect;

uniform lowp sampler2D m_Sampler;
uniform mediump float intensity;

void main(void)
{
	vec4 u_xlat0;
	float u_xlat1;
	float u_xlat2;
	float u_xlat5;
	float u_xlat8;
	vec2 u_xlat9;
	float u_xlat12;

	u_xlat0.xy = v_TexCoord.xy - vec2(0.5);
	u_xlat8 = dot(u_xlat0.xy, u_xlat0.xy);
	u_xlat12 = sqrt(u_xlat8);
	u_xlat1 = intensity - 0.5;
	u_xlat5 = u_xlat1 * 4.44289351;

	if (u_xlat1 > 0.0)
	{
		u_xlat9.xy = u_xlat0.xy * vec2(inversesqrt(u_xlat8));
		u_xlat2 = u_xlat12 * u_xlat5;
		u_xlat9.xy *= vec2(sin(u_xlat2) / cos(u_xlat2) * 0.5);
		u_xlat2 = u_xlat1 * 2.22144675;
		u_xlat9.xy /= vec2(sin(u_xlat2) / cos(u_xlat2));
		u_xlat9.xy += vec2(0.5);
	}
	else
	{
		if (u_xlat1 < 0.0)
		{
			u_xlat0.xy *= vec2(inversesqrt(u_xlat8));
			u_xlat8 = u_xlat12 * (-u_xlat5) * 10.0;
			u_xlat12 = min(abs(u_xlat8), 1.0) * (1.0 / max(abs(u_xlat8), 1.0));
			u_xlat5 = u_xlat12 * u_xlat12;
			u_xlat5 = u_xlat5 * (((u_xlat5 * 0.0208350997 - 0.0851330012) * u_xlat5 + 0.180141002) * u_xlat5 - 0.330299497) + 1.0;
			u_xlat12 = u_xlat12 * u_xlat5 + (abs(u_xlat8) > 1.0 ? (1.57079637 - u_xlat12 * u_xlat5 * 2.0) : 0.0);
			u_xlat8 = min(u_xlat8, 1.0);
			u_xlat0.xy *= vec2((u_xlat8 < -u_xlat8 ? -u_xlat12 : u_xlat12) * 0.5);
			u_xlat8 = u_xlat1 * -22.214468;
			u_xlat12 = min(abs(u_xlat8), 1.0) * (1.0 / max(abs(u_xlat8), 1.0));
			u_xlat1 = u_xlat12 * u_xlat12;
			u_xlat1 = u_xlat1 * (u_xlat1 * (u_xlat1 * (u_xlat1 * 0.0208350997 - 0.0851330012) + 0.180141002) - 0.330299497) + 1.0;
			u_xlat12 = u_xlat12 * u_xlat1 + (1.0 < abs(u_xlat8) ? (1.57079637 - u_xlat12 * u_xlat1 * 2.0) : 0.0);
			u_xlat8 = min(u_xlat8, 1.0);
			u_xlat0.xy /= vec2(u_xlat8 < -u_xlat8 ? -u_xlat12 : u_xlat12);
			u_xlat9.xy = u_xlat0.xy + vec2(0.5);
		}
		else
		{
			u_xlat9.xy = v_TexCoord.xy;
		}
	}
	
	gl_FragColor = toSRGB(texture2D(m_Sampler, u_xlat9.xy));
}
