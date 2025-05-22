#ifndef WATERDROP_FS
#define WATERDROP_FS

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

layout (set = 2, binding = 0) uniform mediump texture2D m_Texture1;
layout (set = 2, binding = 1) uniform mediump sampler m_Sampler1;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
	vec2 frameResolution = v_TexRect.zw - v_TexRect.xy;
	vec2 framePixelPos = (v_TexCoord - v_TexRect.xy) / frameResolution;
	framePixelPos = vec2(framePixelPos.x, 1.0 - framePixelPos.y);
	vec2 texResolution = TexRect1.zw - TexRect1.xy;
	vec2 s_topLeft = TexRect1.xy;

	vec2 m_TexCoord = v_TexCoord;
	m_TexCoord.y = v_TexRect.w - (m_TexCoord.y - v_TexRect.y);

	vec4 u_xlat0;
	vec4 u_xlat1;
	vec4 u_xlat2;
	float u_xlat3;
	vec2 u_xlat6;
	float u_xlat9;

	u_xlat3 = 0.0;
	u_xlat0.x = 1.0;
	u_xlat0.x = u_xlat0.x * time;
	u_xlat0.xyz = u_xlat0.xxx * vec3(0.125, 0.25, 0.025);
	u_xlat9 = m_TexCoord.y * 0.5;
	u_xlat1.zw = vec2(u_xlat9) * vec2(1.4, 1.1) + u_xlat0.xy;
	u_xlat0.y = m_TexCoord.y * 0.5 + u_xlat0.z;
	u_xlat6.xy = v_TexCoord.xx * vec2(1.3, 1.15);
	u_xlat1.xy = u_xlat6.xy * vec2(1.0);
	u_xlat6.xy = u_xlat1.yw + vec2(-0.1, 0.0);
	u_xlat1 = samplePP(m_Texture1, m_Sampler1, TexRect1, getShaderTexturePosition(u_xlat1.xz, texResolution, s_topLeft));
	u_xlat1.xy = u_xlat1.xy / vec2(intensity);
	u_xlat2 = samplePP(m_Texture1, m_Sampler1, TexRect1, getShaderTexturePosition(u_xlat6.xy, texResolution, s_topLeft));
	u_xlat6.xy = u_xlat2.xy / vec2(intensity);
	u_xlat6.xy = (-u_xlat6.xy) + u_xlat1.xy;
	u_xlat0.x = v_TexCoord.x - 0.2;
	u_xlat1 = samplePP(m_Texture1, m_Sampler1, TexRect1, getShaderTexturePosition(u_xlat0.xy, texResolution, s_topLeft));
	u_xlat0.xy = u_xlat1.xy / vec2(intensity);
	u_xlat0.xy = (-u_xlat0.xy) + u_xlat6.xy;
	u_xlat0.xy = (-u_xlat0.xy) * vec2(0.333333333) + v_TexCoord.xy;
	u_xlat0 = texture(sampler2D(m_Texture, m_Sampler), u_xlat0.xy);

	o_Colour = vec4(u_xlat0.xyz, 1.0);
}

#endif