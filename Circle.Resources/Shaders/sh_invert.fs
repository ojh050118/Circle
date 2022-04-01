#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;

uniform lowp sampler2D m_Sampler;

vec4 invert(vec4 original)
{
	return vec4(vec3(1.0) - original.rgb, 1.0);
}

void main(void)
{
	gl_FragColor = invert(toSRGB(texture2D(m_Sampler, v_TexCoord)));
}