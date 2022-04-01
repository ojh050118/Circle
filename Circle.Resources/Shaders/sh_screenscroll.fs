#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;
varying vec4 v_TexRect;

uniform lowp sampler2D m_Sampler;
uniform mediump float startX;
uniform mediump float startY;
uniform mediump float speedX;
uniform mediump float speedY;
uniform mediump float startTime;
uniform mediump float time;

void main(void)
{
	vec2 resolution = vec2(v_TexRect[2] - v_TexRect[0], v_TexRect[1] - v_TexRect[3]);
	vec2 pixelPos = v_TexCoord/resolution;

	float timeOffset = max(0, time - startTime) / 1000.0;
	float xOffset = pixelPos.x - startX + timeOffset * speedX / 100.0;
	float yOffset = pixelPos.y - startY + timeOffset * speedY / 100.0;

	gl_FragColor = toSRGB(texture2D(m_Sampler, fract(vec2(xOffset, yOffset)) * resolution));
}
