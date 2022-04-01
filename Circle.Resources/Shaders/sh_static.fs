#include "sh_Utils.h"
#include "sh_YosoUtils.h"

varying mediump vec2 v_TexCoord;
varying mediump vec4 v_TexRect;

uniform lowp sampler2D m_Sampler;
uniform lowp sampler2D s_Sampler1;
uniform mediump vec4 s_TexRect1;

uniform mediump float intensity;
uniform mediump float time;

void main(void)
{
	vec2 frameResolution = vec2(v_TexRect[2] - v_TexRect[0], v_TexRect[3] - v_TexRect[1]);
	vec2 framePixelPos = v_TexCoord/frameResolution;
	vec2 texResolution = vec2(s_TexRect1[1] - s_TexRect1[0], s_TexRect1[3] - s_TexRect1[2]);
	vec2 s_topLeft = vec2(s_TexRect1[1], s_TexRect1[3]);

	vec4 u_xlat0 = toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(framePixelPos + vec2(time) * vec2(10.0, 0.5), texResolution, s_topLeft)));
	vec4 u_xlat1 = toSRGB(texture2D(m_Sampler, v_TexCoord));
	gl_FragColor = vec4(intensity) * (u_xlat0 - u_xlat1) + u_xlat1;
}