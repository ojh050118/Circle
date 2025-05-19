#ifndef DRAWING_FS
#define DRAWING_FS

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

	vec3 u_xlat0;
	vec4 u_xlat1;
	vec4 u_xlat2;
	vec4 u_xlat3;
	vec4 u_xlat4;
	vec4 u_xlat5;
	vec4 u_xlat6;
	vec3 u_xlat7;
	vec2 u_xlat14;
	float u_xlat16;

	float _Value1 = 0.0008; //pencil size

	u_xlat0.y = _Value1;
	u_xlat1 = u_xlat0.yxxy + v_TexCoord.xyxy;
	u_xlat0.x = time * 10.0;
	u_xlat0.y = cos(u_xlat0.x) * 0.02;
	u_xlat0.x = sin(u_xlat0.x) * 0.02;
	u_xlat0.xy = floor(u_xlat0.xy);
	u_xlat2 = texture(sampler2D(s_Texture1, s_Sampler1), getShaderTexturePosition(v_TexCoord, texResolution, s_topLeft));
	u_xlat16 = u_xlat2.z * 0.02;
	u_xlat3.x = u_xlat0.y * 0.0833333358 + u_xlat16;
	u_xlat3.y = u_xlat0.x * 0.0833333358 + u_xlat16;
	u_xlat1 = u_xlat3.xyxy * vec4(0.0078125) + u_xlat1;
	u_xlat4 = texture(sampler2D(m_Texture, m_Sampler), u_xlat1.xy);
	u_xlat1 = texture(sampler2D(m_Texture, m_Sampler), u_xlat1.zw);
	u_xlat5 = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord.xy);
	u_xlat4.xyz = max(vec3(1.0) - abs(u_xlat5.xyz - u_xlat4.xyz), vec3(0.0));
	u_xlat7.x = 19.0;
	u_xlat0.x = dot(vec4(exp2(log2(u_xlat4.z * u_xlat4.y * u_xlat4.x) * u_xlat7.x)), vec4(1.0));
	u_xlat14.y = -_Value1;
	u_xlat4 = u_xlat3.xyxy * vec4(0.0078125) + u_xlat14.yxxy + v_TexCoord.xyxy;
	u_xlat3 = texture(sampler2D(s_Texture1, s_Sampler1), getShaderTexturePosition(u_xlat3.xy + v_TexCoord, texResolution, s_topLeft));
	u_xlat6 = texture(sampler2D(m_Texture, m_Sampler), u_xlat4.xy);
	u_xlat4 = texture(sampler2D(m_Texture, m_Sampler), u_xlat4.zw);
	u_xlat3.xyw = max(vec3(1.0) - abs(u_xlat5.xyz - u_xlat4.xyz), vec3(0.0));
	u_xlat4.xyz = max(vec3(1.0) - abs(u_xlat5.xyz - u_xlat6.xyz), vec3(0.0));
	u_xlat1.xyz = max(vec3(1.0) - abs(u_xlat5.xyz - u_xlat1.xyz), vec3(0.0));
	u_xlat0.x *= dot(vec4(exp2(log2(u_xlat4.z * u_xlat4.y * u_xlat4.x) * u_xlat7.x)), vec4(1.0));
	u_xlat0.x *= dot(vec4(exp2(log2(u_xlat1.z * u_xlat1.y * u_xlat1.x) * u_xlat7.x)), vec4(1.0));
	u_xlat0.x = u_xlat3.z * (1.0 - min(dot(vec4(exp2(log2(u_xlat3.w * u_xlat3.y * u_xlat3.x) * u_xlat7.x)), vec4(1.0)) * u_xlat0.x, 1.0)) * 1.5 * (1.0 - (1.0 - u_xlat2.y) * 0.5);

	o_Colour = vec4(vec3(clamp(intensity, 0.0, 1.0)) * (u_xlat2.xxx - u_xlat5.xyz - u_xlat0.xxx * u_xlat2.xxx) + u_xlat5.xyz, 1.0);
}

#endif