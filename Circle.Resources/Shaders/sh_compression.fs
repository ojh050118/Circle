#ifndef COMPRESSION_FS
#define COMPRESSION_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
    mediump float time;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 colour;

void main(void)
{
    vec4 u_xlat0;
    float u_xlat1;

	u_xlat1 = floor(time * 12.0);
	u_xlat0 = floor(v_TexCoord.xyxy * vec4(24.0, 19.0, 38.0, 14.0)) * vec4(4.0) * u_xlat1;
	u_xlat1 = fract(sin(dot(u_xlat1 * vec2(2.0, 1.0), vec2(127.1, 311.7))) * 43758.5469);
	u_xlat0.x = dot(u_xlat0.xy, vec2(127.1, 311.7));
	u_xlat0.y = dot(u_xlat0.zw, vec2(127.1, 311.7));
	u_xlat0.xy = fract(sin(u_xlat0.xy) * vec2(43758.5469));
	u_xlat0.x = u_xlat0.x * u_xlat0.x * u_xlat0.x * u_xlat0.y * u_xlat0.y * u_xlat0.y * intensity * 0.06 * u_xlat1;
	u_xlat0.y = 0.0;

	colour = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord + u_xlat0.xy);
}

#endif
