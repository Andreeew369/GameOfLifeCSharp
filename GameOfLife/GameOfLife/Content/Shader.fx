/*
BEGIN_SHADER_DECLARATIONS
{
    "Shaders": [
        {
            "ShaderName": "MainPS",
            "ShaderCompiler": "fxc",
            "ShaderType": "ps",
            "ShaderModel": "4_0",
            "EntryPoint": "MainPS",
            "Defines": [],
            "Optimization": "3",
            "AdditionalArgs": []
        }
    ]
}
END_SHADER_DECLARATIONS
*/


//input color
float4 CircleColor : register(c0);
float4 position : register(c1);

float4 MainPS() : SV_Target {
    float2 center = float2(0.5, 0.5);
    float radius = 0.5;
    float2 pixelCoords = position.xy;

    float distance = length(pixelCoords - center);

    if (distance <= radius) 
        return CircleColor;
    else
        return float4(0,0,0,0);
}

technique Technique1 {
    pass Pass1 {
        PixelShader = compile ps_4_0_level_9_1 MainPS();
    }
}