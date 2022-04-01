#include "sh_Utils.h"

varying mediump vec2 v_TexCoord;

uniform lowp sampler2D m_Sampler;
uniform mediump float time;

void main(void)
{
	vec3 u_xlat0;
	vec3 u_xlat1;
	vec3 u_xlat2;
	vec2 u_xlat3;
	vec2 u_xlat4;
	ivec2 u_xlati4;
	float u_xlat8;
	int u_xlati8;
	float u_xlat12;
	int u_xlati12;

	u_xlat0.x = v_TexCoord.y * 16.0;
	u_xlat0.yz = vec2(time) * vec2(4.0, 8.0);
	u_xlat0.xyz = floor(u_xlat0.xyz);
	u_xlat4.xy = u_xlat0.yz * vec2(37.5, 0.125);
	u_xlat0.x = u_xlat0.x * 0.0625 + u_xlat4.x;
	u_xlat0.y = fract(sin(dot(u_xlat4.yy, vec2(12.9898, 78.233))) * 43758.5469);
	u_xlat0.x = fract(sin(dot(u_xlat0.xx, vec2(12.9898, 78.233))) * 43758.5469) * 16.0;
	u_xlat0.x = floor(u_xlat0.x * time) / u_xlat0.x * 5.0 + v_TexCoord.y;
	u_xlat0.xz = floor(u_xlat0.xx * vec2(11.0, 7.0)) * vec2(0.0909090936, 0.142857149);
	u_xlat0.z = dot(u_xlat0.zz, vec2(12.9898, 78.233));
	u_xlat0.x = dot(u_xlat0.xx, vec2(12.9898, 78.233));
	u_xlat0.xz = fract(sin(u_xlat0.xz) * vec2(43758.5469, 43758.5469));
	u_xlat8 = u_xlat0.z * 0.5;
	u_xlat0.x = (u_xlat0.x * 0.5 + u_xlat8) * 2.0 - 1.0;
	u_xlati4.xy = ivec2(uvec2(lessThan(vec4(0.99, 0.0, 0.0, 0.0), u_xlat0.yxyy).xy) * 0xFFFFFFFFu);
	u_xlati12 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
	u_xlat0.x = max((abs(u_xlat0.x) - 0.6) * 2.5, 0.0);
	u_xlati8 = u_xlati12 - u_xlati4.y;
	u_xlat8 = float(u_xlati8);
	u_xlat12 = clamp(0.5 - u_xlat8 * u_xlat0.x, 0.0, 1.0);
	u_xlat1.x = u_xlat0.x * u_xlat8;
	u_xlat0.x = clamp(u_xlat8 * u_xlat0.x - 0.5, 0.0, 1.0);
	u_xlat8 = abs(u_xlat1.x) * 3.0;
	u_xlat1.xy = u_xlat1.xx * vec2(0.1, 0.125);
	u_xlat1.z = 0.0;
	u_xlat2.xy = clamp(u_xlat1.xz + v_TexCoord, 0.0, 1.0);
	u_xlat2.z = u_xlat2.y;
	u_xlat2 = toSRGB(texture2D(m_Sampler, u_xlat2.xz)).xyz;
	u_xlat3.x = dot(u_xlat2, vec3(-0.147129998, -0.28886, 0.436)) / (1.0 - u_xlat8 * u_xlat12);
	u_xlat3.y = u_xlat1.y * u_xlat0.x + dot(u_xlat2, vec3(0.615, -0.514989972, -0.1));
	u_xlat4.x = dot(u_xlat2, vec3(0.299, 0.587, 0.114));
	
	vec4 finalCol = vec4(1.0);
	finalCol.y = dot(vec2(-0.39465, -0.5806), u_xlat3.xy) + u_xlat4.x;
	finalCol.xz = u_xlat3.yx * vec2(1.13989, 2.03211) + u_xlat4.xx;

	gl_FragColor = finalCol;
}