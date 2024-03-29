﻿#include "sh_Utils.h"

#define PI 3.1415926538

varying vec2 v_TexCoord;
varying vec4 v_TexRect;

uniform sampler2D m_Sampler;

float angleBetween(vec2 l1p1, vec2 l1p2, vec2 l2p1, vec2 l2p2)
{
    return atan(l1p2.y - l1p1.y, l1p2.x - l1p1.x) - atan(l2p2.y - l2p1.y, l2p2.x - l2p1.x);
}

void main(void)
{
    vec2 resolution = vec2(v_TexRect[2] - v_TexRect[0], v_TexRect[1] - v_TexRect[3]);
    vec2 pixelPos = v_TexCoord/resolution;

    float angle = angleBetween(vec2(0.5, 0.0), vec2(0.5, 1.0), vec2(0.5), pixelPos) + PI / 2.0; // in rad

    vec2 sample = vec2(fract((distance(pixelPos, vec2(0.5)) * 1.1 + 0.45) * 0.9), 1.0 - angle / 2.0 / PI);
    
    gl_FragColor = toSRGB(texture2D(m_Sampler, sample * resolution));
}
