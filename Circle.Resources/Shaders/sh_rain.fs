#include "sh_Utils.h"
#include "sh_YosoUtils.h"

varying mediump vec2 v_TexCoord;
varying mediump vec4 v_TexRect;

uniform lowp sampler2D m_Sampler;
uniform lowp sampler2D s_Sampler1;
uniform mediump vec4 s_TexRect1;

uniform mediump float intensity;
uniform mediump float time;

void main(void)
{
	vec2 frameResolution = vec2(v_TexRect[2] - v_TexRect[0], v_TexRect[3] - v_TexRect[1]);
	vec2 framePixelPos = v_TexCoord/frameResolution;
	vec2 texResolution = vec2(s_TexRect1[1] - s_TexRect1[0], s_TexRect1[3] - s_TexRect1[2]);
	vec2 s_topLeft = vec2(s_TexRect1[1], s_TexRect1[3]);

	vec4 u_xlat0;
	vec4 u_xlat1;
	vec4 u_xlat2;
	vec4 u_xlat3;
	vec2 u_xlat4;

	u_xlat0.xy = v_TexCoord * vec2(1.5);
	u_xlat0.x += 0.12 * u_xlat0.y;
	u_xlat1.x = u_xlat0.x * 3.0 + 0.1;
	u_xlat2.x = u_xlat1.x * 0.65 + 0.1;
	u_xlat0.x = time * 0.275;
	u_xlat3.xyz = u_xlat0.xxx * vec3(0.8, 1.2, 0.4);
	u_xlat1.y = u_xlat0.y * 3.0 + u_xlat3.x;
	u_xlat2.y = u_xlat1.y * 0.65 + u_xlat0.x;
	u_xlat0 = toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat1.xy, texResolution, s_topLeft)));
	u_xlat0.x *= intensity;
	u_xlat4.x = u_xlat2.x * 0.65 + 0.1;
	u_xlat4.y = u_xlat2.y * 0.65 + u_xlat3.y;
	u_xlat0.x = u_xlat0.x * 0.3 + toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat2.xy, texResolution, s_topLeft))).x * intensity * 0.5;
	u_xlat2.x = u_xlat4.x * 0.5 + 0.1;
	u_xlat2.y = u_xlat4.y * 0.5 + u_xlat3.y;
	u_xlat0.x += toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat4, texResolution, s_topLeft))).x * intensity * 0.7;
	u_xlat1 = toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat2.xy, texResolution, s_topLeft)));
	u_xlat4.x = u_xlat2.x * 0.4 + 0.1;
	u_xlat4.y = u_xlat2.y * 0.4 + u_xlat3.y;
	u_xlat2.x = v_TexCoord.x * 0.001 + u_xlat3.z;
	u_xlat0.x += u_xlat1.x * intensity * 0.9 + toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat4, texResolution, s_topLeft))).x * intensity * 0.9;
	u_xlat0 = u_xlat0.xxxx + toSRGB(texture2D(m_Sampler, u_xlat0.xx * vec2(0.05) + v_TexCoord));
	u_xlat1 = toSRGB(texture2D(m_Sampler, v_TexCoord));
	u_xlat2.y = 0.0;
	u_xlat2 = toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat2.xy, texResolution, s_topLeft)));
	u_xlat0 += vec4(u_xlat2.y * intensity * 0.1) * vec4(0.9) - u_xlat1;

	gl_FragColor = vec4(u_xlat0.rgb + u_xlat1.rgb, 1.0);
}