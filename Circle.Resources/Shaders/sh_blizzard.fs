#ifndef BLIZZARD_FS
#define BLIZZARD_FS

#include "sh_Utils.h"
#include "sh_CircleUtils.h"

layout(location = 2) in mediump vec2 v_TexCoord;
layout (location = 3) in mediump vec4 v_TexRect;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
    mediump float time;
	mediump vec4 TexRect1;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout (set = 2, binding = 0) uniform lowp texture2D m_Texture1;
layout (set = 2, binding = 1) uniform lowp sampler m_Sampler1;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
	vec2 frameResolution = v_TexRect.zw - v_TexRect.xy;
	vec2 framePixelPos = (v_TexCoord - v_TexRect.xy) / frameResolution;
	framePixelPos = vec2(framePixelPos.x, 1.0 - framePixelPos.y);
	vec2 texResolution = TexRect1.zw - TexRect1.xy;
	vec2 s_topLeft = TexRect1.xy;

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
	u_xlat1 = samplePP(m_Texture1, m_Sampler1, TexRect1, getShaderTexturePosition(u_xlat1.xy, texResolution, s_topLeft));
	u_xlat0.xz = u_xlat4 * vec2(0.5, 0.333333343);
	u_xlat5.xy = sin(u_xlat0.xz) * u_xlat0.xz * vec2(0.25, 0.333333343) + vec2(1.0);
	u_xlat1.x = max(u_xlat1.x * sin(u_xlat5.y * 0.1), 0.0);
	u_xlat5.x += v_TexCoord.x;
	u_xlat2.x = u_xlat5.x - (sin(u_xlat4 * 0.0625 + u_xlat5.x) * 0.125 + u_xlat0.x);
	u_xlat2.y = u_xlat4 * 0.5 + v_TexCoord.y;
	u_xlat2 = samplePP(m_Texture1, m_Sampler1, TexRect1, getShaderTexturePosition(u_xlat2.xy, texResolution, s_topLeft));
	u_xlat1.x += u_xlat2.y;
	u_xlat13 = v_TexCoord.x * 2.0 + u_xlat8.y;
	u_xlat5.x = v_TexCoord.x * 0.5 + u_xlat5.y;
	u_xlat2.x = sin(u_xlat4 * 0.0833333358 + u_xlat13) * 0.125 + u_xlat4;
	u_xlat3.x = u_xlat5.x - (sin(u_xlat4 * 0.0555555597 + u_xlat5.x) * 0.0833333358 + u_xlat0.z);
	u_xlat3.y = v_TexCoord.y * 0.5 + u_xlat0.z;
	u_xlat3 = samplePP(m_Texture1, m_Sampler1, TexRect1, getShaderTexturePosition(u_xlat3.xy, texResolution, s_topLeft));
	u_xlat2.x = u_xlat13 - u_xlat2.x;
	u_xlat2.y = u_xlat8.y * u_xlat2.x * 0.015625 + v_TexCoord.y;
	u_xlat0 = samplePP(m_Texture1, m_Sampler1, TexRect1, getShaderTexturePosition(u_xlat2.xy, texResolution, s_topLeft));
	o_Colour = vec4(vec3(max(sin(u_xlat5.y * 0.015625 + 2.0) * u_xlat0.z, 0.0) + u_xlat1.x + u_xlat3.y * 2.0) * vec3(0.25) + texture(sampler2D(m_Texture, m_Sampler), v_TexCoord).rgb, 1.0);
}

#endif