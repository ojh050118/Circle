#ifndef RAIN_FS
#define RAIN_FS

#include "sh_Utils.h"
#include "sh_CircleUtils.h"

layout(location = 2) in mediump vec2 v_TexCoord;
layout(location = 3) in mediump vec2 v_TexRect;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
    mediump float time;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(set = 2, binding = 0) uniform mediump texture2D s_Texture1;
layout(set = 2, binding = 1) uniform mediump sampler s_Sampler1;
layout(set = 2, binding = 2) uniform mediump vec4 s_TexRect1;

layout(location = 0) out vec4 o_Colour;

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
	u_xlat0 = texture(sampler2D(s_Texture1, s_Sampler1), getShaderTexturePosition(u_xlat1.xy, texResolution, s_topLeft));
	u_xlat0.x *= intensity;
	u_xlat4.x = u_xlat2.x * 0.65 + 0.1;
	u_xlat4.y = u_xlat2.y * 0.65 + u_xlat3.y;
	u_xlat0.x = u_xlat0.x * 0.3 + texture(sampler2D(s_Texture1, s_Sampler1), getShaderTexturePosition(u_xlat2.xy, texResolution, s_topLeft)).x * intensity * 0.5;
	u_xlat2.x = u_xlat4.x * 0.5 + 0.1;
	u_xlat2.y = u_xlat4.y * 0.5 + u_xlat3.y;
	u_xlat0.x += texture(sampler2D(s_Texture1, s_Sampler1), getShaderTexturePosition(u_xlat4, texResolution, s_topLeft)).x * intensity * 0.7;
	u_xlat1 = texture(sampler2D(s_Texture1, s_Sampler1), getShaderTexturePosition(u_xlat2.xy, texResolution, s_topLeft));
	u_xlat4.x = u_xlat2.x * 0.4 + 0.1;
	u_xlat4.y = u_xlat2.y * 0.4 + u_xlat3.y;
	u_xlat2.x = v_TexCoord.x * 0.001 + u_xlat3.z;
	u_xlat0.x += u_xlat1.x * intensity * 0.9 + texture(sampler2D(s_Texture1, s_Sampler1), getShaderTexturePosition(u_xlat4, texResolution, s_topLeft)).x * intensity * 0.9;
	u_xlat0 = u_xlat0.xxxx + texture(sampler2D(m_Texture, m_Sampler), u_xlat0.xx * vec2(0.05) + v_TexCoord));
	u_xlat1 = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord);
	u_xlat2.y = 0.0;
	u_xlat2 = texture(sampler2D(s_Texture1, s_Sampler1), getShaderTexturePosition(u_xlat2.xy, texResolution, s_topLeft));
	u_xlat0 += vec4(u_xlat2.y * intensity * 0.1) * vec4(0.9) - u_xlat1;

	o_Colour = vec4(u_xlat0.rgb + u_xlat1.rgb, 1.0);
}

#endif