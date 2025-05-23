#ifndef CIRCLEUTILS_H
#define CIRCLEUTILS_H

#undef PI
#define PI 3.1415926538

vec2 rotate(float length, vec2 origin, float angle)
{
    float rad = angle * PI / 180.0;
    return vec2(cos(rad), sin(rad)) * length + origin;
}

vec2 rotateAround(vec2 point, vec2 origin, float angle)
{
    float rad = angle * PI / 180.0;
    float s = sin(rad);
    float c = cos(rad);

    point -= origin;
    
    return vec2(point.x * c - point.y * s, point.x * s + point.y * c) + origin;
}

float distanceToLine(vec2 pt1, vec2 pt2, vec2 point)
{
    vec2 a = (pt2 - pt1) / distance(pt2, pt1);
    vec2 closest = clamp(dot(a, point - pt1), 0.0, distance(pt2, pt1)) * a + pt1;
    return distance(closest, point);
}

float map(float value, float minValue, float maxValue, float minEndValue, float maxEndValue)
{
    return (value - minValue) / (maxValue - minValue) * (maxEndValue - minEndValue) + minEndValue;
}

bool almostEqual(float v1, float v2)
{
    return abs(v1 - v2) < 0.00001;
}

bool almostEqual(vec2 p1, vec2 p2)
{
    return almostEqual(p1.x, p2.x) && almostEqual(p1.y, p2.y);
}

vec2 getShaderTexturePosition(vec2 value, vec2 texRes, vec2 topLeft)
{
    return topLeft + fract(value) * texRes;
}

vec2 getTileTexturePosition(vec2 pixelPos, vec2 texRes, vec2 topLeft, vec2 offset)
{
    return topLeft + abs(texRes) * 0.1 - fract(pixelPos + offset) * 0.8 * texRes;
}

bool hasInnerGlow(int style)
{
    return style == 1 || style == 2;
}

bool hasOuterShadow(int style)
{
    return style < 3;
}

float getShadowColour(int style)
{
    // 0,3 -> 0.0
    // rest -> 1.0
    return float(style != 0 && style != 3);
}

float getBorderColour(int style)
{
    // 0 -> 0.5
    // 3 -> 0.0
    // rest -> 1.0
    float styleIs0 = float(style == 0);
    float styleIs3 = float(style == 3);
    return styleIs0 * 0.5 + float(styleIs0 + styleIs3 == 0);
}

float getInnerColour(int style)
{
    // 1 -> 0.0
    // 2 -> 0.5
    // rest -> 1.0
    float styleIs1 = float(style == 1);
    float styleIs2 = float(style == 2);
    return styleIs2 * 0.5 + float(styleIs1 + styleIs2 == 0);
}

vec4 samplePP(lowp texture2D m_Texture, lowp sampler m_Sampler, highp vec4 rect, highp vec2 pos)
{
    return texture(sampler2D(m_Texture, m_Sampler), vec2(pos.x, rect.w - (pos.y - rect.y)));
}

#endif
