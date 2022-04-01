#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;

uniform lowp sampler2D m_Sampler;
uniform mediump float intensity;

void main(void)
{
	vec4 pixelCol = toSRGB(texture2D(m_Sampler, v_TexCoord));
	vec3 finalCol = vec3(dot(pixelCol.rgb, vec3(0.222, 0.707, 0.071))) + vec3(0.437, 0.171, 0.078);
	gl_FragColor = vec4(mix(pixelCol.rgb, finalCol, intensity), 1.0);
}
