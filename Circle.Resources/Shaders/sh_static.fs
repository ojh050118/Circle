#ifndef STATIC_FS
#define STATIC_FS

#include "sh_Utils.h"
#include "sh_CircleUtils.h"

layout(location = 2) in mediump vec2 v_TexCoord;
layout (location = 3) in mediump vec4 v_TexRect;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
    mediump float time;
	mediump vec4 s_TexRect1;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(set = 2, binding = 0) uniform mediump texture2D s_Texture1;
layout(set = 2, binding = 1) uniform mediump sampler s_Sampler1;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
	vec2 frameResolution = v_TexRect.zw - v_TexRect.xy;
	vec2 framePixelPos = (v_TexCoord - v_TexRect.xy) / frameResolution;
	framePixelPos = vec2(framePixelPos.x, 1.0 - framePixelPos.y);
	vec2 texResolution = s_TexRect1.zw - s_TexRect1.xy;
	vec2 s_topLeft = s_TexRect1.xy;

	vec4 u_xlat0 = texture(sampler2D(s_Texture1, s_Sampler1), getShaderTexturePosition(framePixelPos + vec2(time) * vec2(10.0, 0.5), texResolution, s_topLeft));
	vec4 u_xlat1 = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord);
	o_Colour = vec4(intensity) * (u_xlat0 - u_xlat1) + u_xlat1;
}

#endif