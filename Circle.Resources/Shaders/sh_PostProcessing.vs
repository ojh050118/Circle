﻿#ifndef POSTPROCESSING_FS
#define POSTPROCESSING_FS

#include "sh_Utils.h"

layout (location = 0) in highp vec2 m_Position;
layout (location = 1) in lowp vec4 m_Colour;
layout (location = 2) in highp vec2 m_TexCoord;
layout (location = 3) in highp vec4 m_TexRect;
layout (location = 4) in mediump vec2 m_BlendRange;
layout (location = 5) in highp float m_BackbufferDrawDepth;

layout (location = 0) out highp vec2 v_MaskingPosition;
layout (location = 1) out lowp vec4 v_Colour;
layout (location = 2) out highp vec2 v_TexCoord;
layout (location = 3) out highp vec4 v_TexRect;
layout (location = 4) out mediump vec2 v_BlendRange;

void main(void)
{
    v_TexCoord = vec2(m_TexCoord.x, m_TexRect.w - (m_TexCoord.y - m_TexRect.y));
    v_TexRect = m_TexRect;
    gl_Position = g_ProjMatrix * vec4(m_Position, 1.0, 1.0);
}

#endif
