#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

static const float iterations = 255;

float2 complexSquare(float2 v) {
    return float2(
		v.x * v.x - v.y * v.y,
		2 * v.x * v.y
	);
}

float compute(float2 coord) {
    float2 z = float2(0, 0);
    for (int i = 1; i < iterations; i++)
    {
        float sqrLen = z.x * z.x + z.y * z.y;
        if (sqrLen > 4)
            return i;
        z = complexSquare(z) + coord;
    }
    return iterations;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coord = float2(
        lerp(-2.2, 1.2, input.TextureCoordinates.x),
        lerp(-1.2, 1.2, input.TextureCoordinates.y));
    float convergenceValue = compute(coord);
    float v = min(convergenceValue / 40.0, 1.0);
    return float4(v, v, v, 1);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};