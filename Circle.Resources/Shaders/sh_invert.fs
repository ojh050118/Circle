#ifndef INVERT_FS
#define INVERT_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 colour;

vec4 invert(vec4 original)
{
	return vec4(vec3(1.0) - original.rgb, 1.0);
}

void main(void)
{
	colour = invert(texture(sampler2D(m_Texture, m_Sampler), v_TexCoord));
}

#endif