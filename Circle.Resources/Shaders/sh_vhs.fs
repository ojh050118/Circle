#include "sh_Utils.h"
#include "sh_YosoUtils.h"

varying mediump vec2 v_TexCoord;
varying mediump vec4 v_TexRect;

uniform lowp sampler2D m_Sampler;

uniform lowp sampler2D s_Sampler1;
uniform mediump vec4 s_TexRect1;

uniform lowp sampler2D s_Sampler2;
uniform mediump vec4 s_TexRect2;

uniform mediump float intensity;
uniform mediump float time;

void main(void)
{
	vec2 frameResolution = vec2(v_TexRect[2] - v_TexRect[0], v_TexRect[3] - v_TexRect[1]);
	vec2 framePixelPos = v_TexCoord/frameResolution;
	vec2 texResolution = vec2(s_TexRect1[1] - s_TexRect1[0], s_TexRect1[3] - s_TexRect1[2]);
	vec2 s_topLeft = vec2(s_TexRect1[1], s_TexRect1[3]);
	vec2 texResolution2 = vec2(s_TexRect2[1] - s_TexRect2[0], s_TexRect2[3] - s_TexRect2[2]);
	vec2 s_topLeft2 = vec2(s_TexRect2[1], s_TexRect2[3]);

	float adjustedTime = time / 15.0;

	vec4 u_xlat0;
	vec4 u_xlat1;
	bvec4 u_xlatb1;
	vec4 u_xlat2;
	vec3 u_xlat3;
	vec4 u_xlat4;
	vec2 u_xlat5;
	vec3 u_xlat6;
	vec3 u_xlat7;
	float u_xlat10;
	bvec2 u_xlatb12;
	float u_xlat15;
	float u_xlat16;

	u_xlat0 = adjustedTime.xxxx * vec4(12.0, 64.0, 48.0, 20.0);
	u_xlat15 = dot(u_xlat0.ww, vec2(12.9898005, 78.2330017)) * 0.318471313;
	u_xlatb1.x = u_xlat15 >= -u_xlat15;
	u_xlat15 = fract(abs(u_xlat15));
	u_xlat15 = fract(sin((u_xlatb1.x ? u_xlat15 : -u_xlat15) * 3.14) * 43758.5469);
	u_xlat1.x = u_xlat15 * 15.0 + 0.06;
	u_xlat15 = clamp((v_TexCoord.y - u_xlat15 * 15.0) * 33.3333359, 0.0, 1.0);
	u_xlat1.x = clamp((v_TexCoord.y - u_xlat1.x) * 33.3333282, 0.0, 1.0);
	u_xlat6.x = 3.0 - u_xlat1.x * 2.0;
	u_xlat1.x *= u_xlat1.x * u_xlat6.x;
	u_xlat6.x = 3.0 - u_xlat15 * 2.0;
	u_xlat15 *= u_xlat15;
	u_xlat15 = u_xlat6.x * u_xlat15 - u_xlat1.x;
	u_xlat1.xyz = sin(v_TexCoord.yyy * vec3(512.0, 512.0, 150.0) + u_xlat0.xyz);
	u_xlat0.x = v_TexCoord.y - 0.05;
	u_xlat10 = u_xlat0.x * u_xlat1.x + v_TexCoord.x;
	u_xlatb1.xw = lessThan(v_TexCoord.yyyy, vec4(0.025, 0.0, 0.0, 0.015)).xw;
	u_xlat10 = u_xlatb1.x ? u_xlat10 : v_TexCoord.x;
	u_xlat0.x = u_xlat0.x * u_xlat1.y + u_xlat10;
	u_xlat1.x = u_xlat1.z * 0.015625;
	u_xlat0.x = (u_xlatb1.w ? u_xlat0.x : u_xlat10) + u_xlat15 * u_xlat1.x;
	u_xlat10 = 1.0 - u_xlat0.x;
	u_xlat1.y = adjustedTime.x * 0.4;
	u_xlat15 = floor(v_TexCoord.y * 288.0);
	u_xlat2.xw = vec2(u_xlat15) * vec2(0.00347222225, 0.00145833334);
	u_xlat1.z = u_xlat2.x;
	u_xlat6.x = sin(fract(sin(abs(fract(abs(dot(u_xlat1.yz, vec2(12.9898, 78.233)) * 0.318471313))) * 3.14) * 43758.5469));
	u_xlat2.xyz = adjustedTime.xxx * vec3(128.0, 16.0, 0.013);
	u_xlat6.z = dot(u_xlat2.zw, vec2(12.9898005, 78.233));
	u_xlat6.xz *= vec2(0.005, 0.318471313);
	u_xlat16 = fract(abs(u_xlat6.z));
	u_xlat6.x += fract(sin((u_xlat6.z >= -u_xlat6.z ? u_xlat16 : -u_xlat16) * 3.14) * 43758.5469) * 0.004;
	u_xlat3.x = u_xlat6.x * u_xlat10 + u_xlat0.x;
	u_xlat1.x = floor(u_xlat3.x * 52.0) * 0.0192307699;
	u_xlat4.y = 0.0078125;
	u_xlat0.x = sin(u_xlat2.x);
	u_xlat2.x = cos(u_xlat2.x);
	u_xlat5.y = u_xlat15 * 0.0486111119 + u_xlat2.y;
	u_xlat5.x = u_xlat15 * 0.0833333358 + u_xlat0.y;
	u_xlat5.xy = max(sin(u_xlat5.xy), vec2(0.0));
	u_xlat5.x = (u_xlat5.x + u_xlat5.y) * 0.5 + 0.5;
	u_xlat4.x = u_xlat0.x * 0.0078125;
	u_xlat2.y = u_xlat2.x * u_xlat4.x;
	u_xlat2.x = u_xlat0.x * 0.0078125 - 0.01;
	u_xlat0.xz = u_xlat2.xy * u_xlat4.xy + u_xlat1.xz;
	u_xlat1 = toSRGB(texture2D(m_Sampler, u_xlat0.xz));
	u_xlat0.x = u_xlat1.x * -0.147 - u_xlat1.y * 0.289;
	u_xlat2.yw = u_xlat1.zz * vec2(0.436) + u_xlat0.xx;
	u_xlat0.x = u_xlat1.x * 0.615 - u_xlat1.y * 0.514999986;
	u_xlat2.xz = u_xlat0.xx - u_xlat1.zz * vec2(0.1);
	u_xlat1 = u_xlat2;
	u_xlat0 = u_xlat5.xxxx * u_xlat1;
	u_xlat3.y = v_TexCoord.y;
	u_xlat1 = toSRGB(texture2D(m_Sampler, u_xlat3.xy));
	u_xlat1.xyz = min(max(u_xlat1.xyz, vec3(0.08)), vec3(0.95));
	u_xlat1.x = dot(u_xlat1.xyz, vec3(0.299, 0.587, 0.114));
	u_xlat5.x = u_xlat1.x - u_xlat0.y * 0.395;
	u_xlat1.xzw = u_xlat0.xwx * vec3(1.14, 2.032, 1.14) + u_xlat1.xxx;
	u_xlat1.y = u_xlat5.x - u_xlat0.z * 0.581;
	u_xlat0 = min(max(u_xlat1, vec4(0.08)), vec4(0.95));
	u_xlatb1.xyz = lessThan(u_xlat0.wyzw, vec4(0.476190507, 0.476190507, 0.476190507, 0.0)).xyz;
	u_xlat0.w *= 2.1;
	u_xlat2.xy = adjustedTime.xx * vec2(30.0, 3.75);
	u_xlatb12.xy = greaterThanEqual(u_xlat2.xyxy, (-u_xlat2.xyxy)).xy;
	u_xlat2.xy = fract(abs(u_xlat2.xy));

	vec4 hlslcc_movcTemp = u_xlat2;
	hlslcc_movcTemp.x = (u_xlatb12.x) ? u_xlat2.x : (-u_xlat2.x);
	hlslcc_movcTemp.y = (u_xlatb12.y) ? u_xlat2.y : (-u_xlat2.y);
	u_xlat2 = hlslcc_movcTemp;

	u_xlat2.xy *= vec2(8.0);
	u_xlat2.xy = floor(u_xlat2.xy);
	u_xlat16 = u_xlat2.x * 0.125;
	u_xlat2.x = 1.0 - u_xlat2.y * 0.125;
	u_xlat3.x = v_TexCoord.x * 0.125 + u_xlat16;
	u_xlat3.y = v_TexCoord.y * 0.125 + u_xlat2.x;
	u_xlat4 = toSRGB(texture2D(s_Sampler1, getShaderTexturePosition(u_xlat3.xy, texResolution, s_topLeft)));
	u_xlat7.xyz = vec3(1.0) - u_xlat0.xyz * vec3(1.05);
	u_xlat0.xy = u_xlat0.yz * vec2(2.1);
	u_xlat0.xyw *= u_xlat4.yzx;
	u_xlat7.xyz = vec3(1.0) - (u_xlat7.xyz + u_xlat7.xyz) * (vec3(1.0) - u_xlat4.xyz);
	u_xlat4.x = u_xlatb1.x ? u_xlat0.w : u_xlat7.x;
	u_xlat4.y = u_xlatb1.y ? u_xlat0.x : u_xlat7.y;
	u_xlat4.z = u_xlatb1.z ? u_xlat0.y : u_xlat7.z;
	u_xlat3.z = 1.0 - v_TexCoord.y * 0.125 + u_xlat2.x;

	gl_FragColor = vec4(vec3((1.0 - v_TexCoord.y) * intensity * 0.212) * toSRGB(texture2D(s_Sampler2, getShaderTexturePosition(u_xlat3.xz, texResolution2, s_topLeft2))).xyz + u_xlat4.xyz, 1.0);
}