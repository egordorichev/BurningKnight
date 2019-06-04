#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VertexShaderOutput {
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

Texture2D SpriteTexture;
sampler s0;

float time;
float cx;
float cy;
float tx;
float ty;

#define mod(x,y) (x-y*floor(x/y))

sampler2D SpriteTextureSampler = sampler_state {
	Texture = <SpriteTexture>;
};

float4 MainPS(VertexShaderOutput input) : COLOR {
  float v = (tex2D(s0, float2(
    mod(input.TextureCoordinates.x + cx + tx * time, 1.0), 
    mod(input.TextureCoordinates.y + cy + ty * time, 1.0)
  )).r * cos(time * 100.0 + tex2D(s0, float2(
    mod(input.TextureCoordinates.x + cx + tx * time, 1.0),
    mod(input.TextureCoordinates.y + cy + ty * time, 1.0)
  )).r * 10.0) * 0.5 + 0.5);
      
  return float4(v, v, v, v * 0.2f);//v * (1 - smoothstep(0.75f, 0.3f, length(input.TextureCoordinates - float2(0.5f, 0.5f)))));
}

technique SpriteDrawing {
	pass P0 {
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};