#ifndef SCREENSCROLL_FS
#define SCREENSCROLL_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;
layout (location = 3) in mediump vec4 v_TexRect;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float startTime;
    mediump float time;
    mediump float startX;
    mediump float startY;
    mediump float speedX;
    mediump float speedY;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
	vec2 resolution = vec2(v_TexRect[2] - v_TexRect[0], v_TexRect[1] - v_TexRect[3]);
	vec2 pixelPos = v_TexCoord/resolution;

	float timeOffset = max(0, time - startTime) / 1000.0;
	float xOffset = pixelPos.x - startX + timeOffset * speedX / 100.0;
	float yOffset = pixelPos.y - startY + timeOffset * speedY / 100.0;

	o_Colour = texture(sampler2D(m_Texture, m_Sampler), fract(vec2(xOffset, yOffset)) * resolution);
}

#endif
