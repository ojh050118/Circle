#include "sh_Utils.h"

varying vec2 v_TexCoord;
varying vec4 v_TexRect;

uniform sampler2D m_Sampler;
uniform mediump float intensity;
uniform mediump float time;

void main(void)
{
    vec4 u_xlat0;
    float u_xlat1;

	u_xlat1 = floor(time * 12.0);
	u_xlat0 = floor(v_TexCoord.xyxy * vec4(24.0, 19.0, 38.0, 14.0)) * vec4(4.0) * u_xlat1;
	u_xlat1 = fract(sin(dot(u_xlat1 * vec2(2.0, 1.0), vec2(127.1, 311.7))) * 43758.5469);
	u_xlat0.x = dot(u_xlat0.xy, vec2(127.1, 311.7));
	u_xlat0.y = dot(u_xlat0.zw, vec2(127.1, 311.7));
	u_xlat0.xy = fract(sin(u_xlat0.xy) * vec2(43758.5469));
	u_xlat0.x = u_xlat0.x * u_xlat0.x * u_xlat0.x * u_xlat0.y * u_xlat0.y * u_xlat0.y * intensity * 0.06 * u_xlat1;
	u_xlat0.y = 0.0;

	gl_FragColor = toSRGB(texture2D(m_Sampler, v_TexCoord + u_xlat0.xy));
}
