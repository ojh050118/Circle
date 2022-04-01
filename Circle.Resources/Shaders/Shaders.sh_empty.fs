#include "sh_Utils.h"

varying vec2 v_TexCoord;

uniform sampler2D m_Sampler;

void main(void)
{
    gl_FragColor = toSRGB(texture2D(m_Sampler, v_TexCoord));
}
