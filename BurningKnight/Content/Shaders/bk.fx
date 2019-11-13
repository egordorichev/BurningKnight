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

	float2 ps = float2(
        clamp(input.TextureCoordinates.x + v / (cof.x * 10.0), pos.x, pos.x + size.x),
        clamp(input.TextureCoordinates.y + floor(sin(time * 2.0 + x)) / (cof.y * 20.0), pos.y, pos.y + size.y)
    );
    
    float4 color = tex2D(s0, ps);
    
    float xx = (ps.x - pos.x) * cof.x;
    float yy = (ps.y - pos.y) * cof.y;

    float dx = (xx - 0.5);
    float dy = (yy + 0.5);
    float d = sqrt(dx * dx + dy * dy);

    float sum = max(0.0, 1.41 - d * 2.0 + cos(time * 1.4));

    color.r = min(1.0, sum + color.r);
    color.g = min(1.0, sum + color.g);
    color.b = min(1.0, sum + color.b);

    return float4(1.0, abs(cos(time * 2.0) / 2.5) + color.g, color.b * 0.5, color.a * a);
}

technique SpriteDrawing {
	pass P0 {
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};