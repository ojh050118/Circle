#ifndef PIXELSNOW_FS
#define PIXELSNOW_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;
layout(location = 3) in mediump vec2 v_TexRect;

layout(std140, set = 0, binding = 0) uniform m_FilterParameters
{
    mediump float intensity;
    mediump float time;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
    vec4 u_xlat0;
    vec4 u_xlat1;
    vec4 u_xlat2;
    vec3 u_xlat3;
    vec4 u_xlat4;
    vec4 u_xlat5;
    float u_xlat6;
    vec3 u_xlat7;
    vec2 u_xlat13;
    float u_xlat18;
    float u_xlat19;

    float _Value2 = 64.0;

    u_xlat0.xy = vec2(time) * vec2(0.02, 0.501);
    u_xlat1.zy = v_TexCoord;
    u_xlat1.x = u_xlat1.z * 2.0;
    u_xlat0 = u_xlat1.xyxy * vec4(1.01, 2.02, 1.07, 2.14) + u_xlat0.xyxy;
    u_xlat2 = floor(u_xlat0);
    u_xlat0 -= u_xlat2;
    u_xlat0 *= vec4(_Value2);
    u_xlat0 = floor(u_xlat0);
    u_xlat0.z = dot(u_xlat0.zw, vec2(12.9898005, 78.2330017));
    u_xlat0.x = dot(u_xlat0.xy, vec2(12.9898005, 78.2330017));
    u_xlat0.xy = fract(sin(u_xlat0.xz) * vec2(13758.5449, 13758.5449));
    u_xlat0.x = u_xlat0.x * intensity - 0.9;
    u_xlat0.y = u_xlat0.y * intensity - 0.9;
    u_xlat0.xy *= vec2(10.0);
    u_xlat0.xy = clamp(u_xlat0.xy, 0.0, 1.0);
    u_xlat6 = u_xlat0.y * u_xlat0.y * (3.0 - u_xlat0.y * 2.0);
    u_xlat0.x *= u_xlat0.x * (3.0 - u_xlat0.x * 2.0);
    u_xlat2 = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord);
    u_xlat3.xyz = vec3(1.0) - u_xlat2.xyz;
    u_xlat0.xzw = u_xlat0.xxx * u_xlat3.xyz + u_xlat2.xyz;
    u_xlat3.xyz = vec3(0.1, 1.0, 1.0) - u_xlat0.xzw;
    u_xlat0.xyz = vec3(u_xlat6) * u_xlat3.xyz + u_xlat0.xzw;
    u_xlat3.xyz = vec3(1.0) - u_xlat0.xyz;
    u_xlat1.w = u_xlat1.y * 2.0;
    u_xlat13.xy = vec2(time) * vec2(0.05, 0.5) + u_xlat1.xw;
    u_xlat4.xy = floor(u_xlat13.xy);
    u_xlat13.xy = floor((u_xlat13.xy - u_xlat4.xy) * vec2(_Value2));
    u_xlat18 = clamp((fract(sin(dot(u_xlat13.xy, vec2(12.9898005, 78.2330017))) * 13758.5449) * intensity - 0.9) * 10.0, 0.0, 1.0);
    u_xlat13.x = 3.0 - u_xlat18 * 2.0;
    u_xlat18 *= u_xlat18;
    u_xlat18 *= u_xlat13.x;
    u_xlat0.xyz += vec3(u_xlat18) * u_xlat3.xyz;
    u_xlat3.xyz = vec3(1.0) - u_xlat0.xyz;
    u_xlat4 = vec4(time) * vec4(0.02, 0.51, 0.07, 0.493);
    u_xlat4 = u_xlat1.xyxy * vec4(0.9, 1.8, 0.75, 1.5) + u_xlat4;
    u_xlat5 = floor(u_xlat4);
    u_xlat4 -= u_xlat5;
    u_xlat4 *= vec4(_Value2);
    u_xlat4 = floor(u_xlat4);
    u_xlat18 = dot(u_xlat4.xy, vec2(12.9898005, 78.2330017));
    u_xlat13.x = clamp((fract(sin(dot(u_xlat4.zw, vec2(12.9898005, 78.2330017))) * 13758.5449) * intensity - 0.9) * 10.0, 0.0, 1.0);
    u_xlat18 = clamp((fract(sin(u_xlat18) * 13758.5449) * intensity - 0.9) * 10.0, 0.0, 1.0);
    u_xlat19 = 3.0 - u_xlat18 * 2.0;
    u_xlat18 *= u_xlat18;
    u_xlat18 *= u_xlat19;
    u_xlat0.xyz += vec3(u_xlat18) * u_xlat3.xyz;
    u_xlat3.xyz = vec3(1.0) - u_xlat0.xyz;
    u_xlat18 = 3.0 - u_xlat13.x * 2.0;
    u_xlat13.x *= u_xlat13.x;
    u_xlat18 *= u_xlat13.x;
    u_xlat0.xyz += vec3(u_xlat18) * u_xlat3.xyz;
    u_xlat3.xyz = vec3(1.0) - u_xlat0.xyz;
    u_xlat4 = vec4(time) * vec4(0.03, 0.504, 0.02, 0.497);
    u_xlat4 += u_xlat1.xyxy * vec4(0.5, 1.0, 0.3, 0.6);
    u_xlat5 = floor(u_xlat4);
    u_xlat4 -= u_xlat5;
    u_xlat4 *= vec4(_Value2);
    u_xlat4 = floor(u_xlat4);
    u_xlat18 = dot(u_xlat4.xy, vec2(12.9898005, 78.2330017));
    u_xlat13.x = dot(u_xlat4.zw, vec2(12.9898005, 78.2330017));
    u_xlat13.x = clamp((fract(sin(u_xlat13.x) * 13758.5449) * intensity - 0.95) * 20.0, 0.0, 1.0);
    u_xlat18 = clamp((fract(sin(u_xlat18) * 13758.5449) * intensity - 0.94) * 16.666666, 0.0, 1.0);
    u_xlat19 = 3.0 - u_xlat18 * 2.0;
    u_xlat18 *= u_xlat18;
    u_xlat18 *= u_xlat19;
    u_xlat0.xyz += vec3(u_xlat18) * u_xlat3.xyz;
    u_xlat3.xyz = vec3(1.0) - u_xlat0.xyz;
    u_xlat18 = 3.0 - u_xlat13.x * 2.0;
    u_xlat13.x *= u_xlat13.x;
    u_xlat18 *= u_xlat13.x;
    u_xlat0.xyz += vec3(u_xlat18) * u_xlat3.xyz;
    u_xlat3.xyz = vec3(1.0) - u_xlat0.xyz;
    u_xlat4 = vec4(time) * vec4(0.0, 0.51, 0.0, 0.523);
    u_xlat1 = u_xlat1.xyxy * vec4(0.1, 0.2, 0.03, 0.06) + u_xlat4;
    u_xlat4 = floor(u_xlat1);
    u_xlat1 -= u_xlat4;
    u_xlat1 *= vec4(_Value2);
    u_xlat1 = floor(u_xlat1);
    u_xlat18 = dot(u_xlat1.xy, vec2(12.9898005, 78.2330017));
    u_xlat1.x = clamp((fract(sin(dot(u_xlat1.zw, vec2(12.9898005, 78.2330017))) * 13758.5449) * intensity - 0.99) * 100.0, 0.0, 1.0);
    u_xlat18 = clamp((fract(sin(u_xlat18) * 13758.5449) * intensity - 0.96) * 25.0, 0.0, 1.0);
    u_xlat7.x = 3.0 - u_xlat18 * 2.0;
    u_xlat18 *= u_xlat18;
    u_xlat18 *= u_xlat7.x;
    u_xlat0.xyz += vec3(u_xlat18) * u_xlat3.xyz;
    u_xlat7.xyz = vec3(1.0) - u_xlat0.xyz;
    u_xlat18 = 3.0 - u_xlat1.x * 2.0;
    u_xlat1.x *= u_xlat1.x;
    u_xlat18 *= u_xlat1.x;
    u_xlat0.xyz += vec3(u_xlat18) * u_xlat7.xyz;
    u_xlat0.xyz -= u_xlat2.xyz;

    o_Colour = vec4(u_xlat0.xyz + u_xlat2.xyz, 1.0);
}

#endif