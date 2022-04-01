#include "sh_Utils.h"

varying vec2 v_TexCoord;
varying vec4 v_TexRect;

uniform sampler2D m_Sampler;

vec3 getClosest(vec3 original)
{
    int bestIndex = 0;
    float bestDst = 100000.0;
    
    vec3 gb_colors[4];

    gb_colors[0] = vec3(0.03, 0.16, 0.33);
    gb_colors[1] = vec3(0.13, 0.43, 0.37);
    gb_colors[2] = vec3(0.47, 0.69, 0.42);
    gb_colors[3] = vec3(0.68, 0.79, 0.27);
    
    for (int i = 0; i < 4; i++)
    {
        float dst = distance(gb_colors[i], original);

        if (dst < bestDst)
        {
            bestDst = dst;
            bestIndex = i;
        }
    }
    
    return gb_colors[bestIndex];
}

void main(void)
{
    vec3 u_xlat0 = vec3(256.0, 512.0, 512.0);
    u_xlat0 = texture2D(m_Sampler, floor(u_xlat0.yz * floor(u_xlat0.xx * v_TexCoord) / u_xlat0.xx) / u_xlat0.yz).xyz;
    gl_FragColor = toSRGB(vec4(getClosest(u_xlat0), 1.0));
}
