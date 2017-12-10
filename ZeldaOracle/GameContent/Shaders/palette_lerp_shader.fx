sampler RenderTarget:	register(s0);
sampler TilePalette:	register(s1);
sampler EntityPalette:	register(s2);
sampler TilePalette2:	register(s3);
sampler EntityPalette2:	register(s4);

float TileRatio;
float EntityRatio;

static const float2 OFFSET = float2(0.5, 0.5);
static const float2 IMAGE_SIZE = float2(256, 256);
static const float2 IMAGE_SIZE_M_1 = float2(255, 255);

float4 PixelShaderFunction(float4 colorize: COLOR0, float2 texCoord : TEXCOORD0) : COLOR0{
	float4 color = tex2D(RenderTarget, texCoord);
	int a = color.a * 255;
	if (a == 254) {
		color.rg *= IMAGE_SIZE_M_1;
		float2 uv = (color.rg + OFFSET) / IMAGE_SIZE;
		if (color.b == 1) {
			color = tex2D(EntityPalette, uv);
			if (EntityRatio > 0.0)
				color = lerp(color, tex2D(EntityPalette2, uv), EntityRatio);
		}
		else if (color.b == 0) {
			color = tex2D(TilePalette, uv);
			if (TileRatio > 0.0)
				color = lerp(color, tex2D(TilePalette2, uv), TileRatio);
		}
	}
	return color * colorize;
}

technique Technique1 {
	pass Pass1 {
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
