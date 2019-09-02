#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

Texture2D SpriteTexture;
sampler s0;
float black;
float bx;
float by;

sampler2D SpriteTextureSampler = sampler_state {
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput {
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR {
  if (black < 1.0f) { 
		float dx = input.TextureCoordinates.x - bx;
		float dy = (input.TextureCoordinates.y - by) * 0.5625f;
	
		if (sqrt(dx * dx + dy * dy) > black) {
			return float4(0, 0, 0, 1);
		}
	}

	return tex2D(s0, float2(input.TextureCoordinates.x, input.TextureCoordinates.y));
}

technique SpriteDrawing {
	pass P0 {
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};