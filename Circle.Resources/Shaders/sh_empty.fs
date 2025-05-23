﻿#ifndef EMPTY_FS
#define EMPTY_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;

layout(set = 0, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 0, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 colour;

void main(void)
{
    colour = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord);
}

#endif
