#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;
varying vec4 v_TexRect;

uniform lowp sampler2D m_Sampler;
uniform mediump float tilingX;
uniform mediump float tilingY;

void main(void)
{
    vec2 resolution = vec2(v_TexRect[2] - v_TexRect[0], v_TexRect[1] - v_TexRect[3]);
    vec2 pixelPos = v_TexCoord / resolution;
    
    gl_FragColor = toSRGB(texture2D(m_Sampler, fract(pixelPos * vec2(tilingX, tilingY)) * resolution));
}
