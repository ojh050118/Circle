#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;

uniform lowp sampler2D m_Sampler;
uniform mediump float intensity;

void main(void)
{
	vec3 pixelCol = texture2D(m_Sampler, v_TexCoord).rgb;
	gl_FragColor = toSRGB(vec4(mix(pixelCol, vec3(dot(pixelCol, vec3(0.222, 0.707, 0.071))), intensity), 1.0));
}