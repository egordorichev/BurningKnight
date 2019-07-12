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

bool enabled;
float2 tilePosition;
float2 edgePosition;
float flow;
float time;
float sy;
float h;

#define mod(x,y) (x-y*floor(x/y))

sampler2D SpriteTextureSampler = sampler_state {
	Texture = <SpriteTexture>;
};

float4 MainPS(VertexShaderOutput input) : COLOR {
	float cy = input.TextureCoordinates.y;
		
	if (flow > 0.1f) {
		cy -= tilePosition.y;
		cy -= time * flow - sy;
		cy -= h * floor(cy / h);
		cy += tilePosition.y;
	}

	float4 color = tex2D(s0, float2(input.TextureCoordinates.x, cy));

	if (enabled == true) {
		float4 mask = tex2D(s0, float2(
			input.TextureCoordinates.x - tilePosition.x + edgePosition.x,
      input.TextureCoordinates.y - tilePosition.y + edgePosition.y
		));
		
		if (mask.r == 1 && mask.g == 0 && mask.b == 0 && mask.a == 1) {
			if (flow > 0.6f) {
				color.a = 0.5f;
			}
			
			return color;
		}
		
		return mask;
	}

	return color;
}

technique SpriteDrawing {
	pass P0 {
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};