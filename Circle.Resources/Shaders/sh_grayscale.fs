#ifndef GRAYSCALE_FS
#define GRAYSCALE_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 colour;

void main(void)
{
	vec3 pixelCol = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord).rgb;
	colour = vec4(mix(pixelCol, vec3(dot(pixelCol, vec3(0.222, 0.707, 0.071))), intensity), 1.0);
}

#endif