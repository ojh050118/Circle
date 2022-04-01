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
    float u_xlat4;
    vec3 u_xlat5;
    vec2 u_xlat8;
    float u_xlat13;

	u_xlat4 = intensity * time;
	u_xlat8.xy = vec2(sin(u_xlat4) * u_xlat4) * vec2(0.0625, 0.5) + vec2(1.0);
	u_xlat1.x = u_xlat8.x + v_TexCoord.x;
	u_xlat1.x -= sin(u_xlat4 * 0.0625 + u_xlat1.x) * 0.0625 + u_xlat4;
	u_xlat8.x *= u_xlat1.x;
	u_xlat1.y = u_xlat8.x * 0.03125 + u_xlat4 + v_TexCoord.y;
	u_xlat1 = toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat1.xy, texResolution, s_topLeft)));
	u_xlat0.xz = u_xlat4 * vec2(0.5, 0.333333343);
	u_xlat5.xy = sin(u_xlat0.xz) * u_xlat0.xz * vec2(0.25, 0.333333343) + vec2(1.0);
	u_xlat1.x = max(u_xlat1.x * sin(u_xlat5.y * 0.1), 0.0);
	u_xlat5.x += v_TexCoord.x;
	u_xlat2.x = u_xlat5.x - (sin(u_xlat4 * 0.0625 + u_xlat5.x) * 0.125 + u_xlat0.x);
	u_xlat2.y = u_xlat4 * 0.5 + v_TexCoord.y;
	u_xlat2 = toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat2.xy, texResolution, s_topLeft)));
	u_xlat1.x += u_xlat2.y;
	u_xlat13 = v_TexCoord.x * 2.0 + u_xlat8.y;
	u_xlat5.x = v_TexCoord.x * 0.5 + u_xlat5.y;
	u_xlat2.x = sin(u_xlat4 * 0.0833333358 + u_xlat13) * 0.125 + u_xlat4;
	u_xlat3.x = u_xlat5.x - (sin(u_xlat4 * 0.0555555597 + u_xlat5.x) * 0.0833333358 + u_xlat0.z);
	u_xlat3.y = v_TexCoord.y * 0.5 + u_xlat0.z;
	u_xlat3 = toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat3.xy, texResolution, s_topLeft)));
	u_xlat2.x = u_xlat13 - u_xlat2.x;
	u_xlat2.y = u_xlat8.y * u_xlat2.x * 0.015625 + v_TexCoord.y;
	u_xlat0 = toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat2.xy, texResolution, s_topLeft)));

	gl_FragColor = vec4(vec3(max(sin(u_xlat5.y * 0.015625 + 2.0) * u_xlat0.z, 0.0) + u_xlat1.x + u_xlat3.y * 2.0) * vec3(0.25) + toSRGB(texture2D(m_Sampler, v_TexCoord)).xyz, 1.0);
}