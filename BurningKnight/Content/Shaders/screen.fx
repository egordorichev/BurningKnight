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

float2 resolution;

sampler2D SpriteTextureSampler = sampler_state {
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput {
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR {
	float4 color = tex2D(s0, input.TextureCoordinates);
	float2 position = input.TextureCoordinates.xy - float2(0.5f, 0.5f);
	float len = length(position);
	float vignette = smoothstep(0.75f, 0.3f, len);
	
	color.rgb = color.rgb * vignette;

	return color;
}

technique SpriteDrawing {
	pass P0 {
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};