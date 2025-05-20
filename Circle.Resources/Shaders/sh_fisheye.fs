#ifndef FISHEYE_FS
#define FISHEYE_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;
layout (location = 3) in mediump vec4 v_TexRect;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
	vec4 u_xlat0 = vec4(0.0);
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
	
	o_Colour = texture(sampler2D(m_Texture, m_Sampler), u_xlat9.xy);
}

#endif
