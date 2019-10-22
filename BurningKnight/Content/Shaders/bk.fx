#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler s0;

sampler2D SpriteTextureSampler = sampler_state {
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput {
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float time;
float a;
float2 pos;
float2 size;

float4 MainPS(VertexShaderOutput input) : COLOR {
	float2 cof = float2(1.0 / size.x, 1.0 / size.y);
    float x = (pos.x - input.TextureCoordinates.x) * cof.x;
    float y = (pos.y - input.TextureCoordinates.y) * cof.y;
    float v = floor(sin(time * 4.0 + y * 8.0));

	float4 color = tex2D(s0, float2(
        clamp(input.TextureCoordinates.x + v / (cof.x * 16.0), pos.x, pos.x + size.x),
        clamp(input.TextureCoordinates.y + floor(sin(time * 2.0 + x)) / (cof.y * 24.0), pos.y, pos.y + size.y)
    ));

    if (color.r > 0.7) {
        color.a -= 0.5;
    }

    color.a = min(color.a, a);

	return color;
}

technique SpriteDrawing {
	pass P0 {
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};