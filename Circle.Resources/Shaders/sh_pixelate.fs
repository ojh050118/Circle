#include "sh_Utils.h"

varying vec2 v_TexCoord;
varying vec4 v_TexRect;

uniform sampler2D m_Sampler;
uniform float intensity;

void main(void)
{
    vec3 u_xlat0 = vec3(128.0 / intensity, 512.0, 512.0);
    gl_FragColor = toSRGB(texture2D(m_Sampler, floor(u_xlat0.yz * floor(u_xlat0.xx * v_TexCoord) / u_xlat0.xx) / u_xlat0.yz));
}
