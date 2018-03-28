sampler RenderTarget:	register(s0);
sampler TilePalette:	register(s1);
sampler EntityPalette:	register(s2);
sampler TilePalette2:	register(s3);
sampler EntityPalette2:	register(s4);

float TileRatio;
float EntityRatio;

static const float2 OFFSET = float2(0.5, 0.5);
static const float2 IMAGE_SIZE = float2(256, 256);
static const float3 IMAGE_SIZE_M_1 = float3(255, 255, 255);

float4 PixelShaderFunction(float4 colorize: COLOR0, float2 texCoord : TEXCOORD0) : COLOR0 {
	float4 color = tex2D(RenderTarget, texCoord);
	if (color.r == 1.0 && color.g == 1.0 && color.b == 1.0 && color.a == 1.0) {
		color = colorize;
		colorize = float4(1.0, 1.0, 1.0, 1.0);
	}
	int a = color.a * 255;
	if (a == 254) {
		color.rgb *= IMAGE_SIZE_M_1;
		float2 uv = (color.rg + OFFSET) / IMAGE_SIZE;
		if (color.b == 0) {
			color = tex2D(TilePalette, uv);
			if (TileRatio > 0.0)
				color = lerp(color, tex2D(TilePalette2, uv), TileRatio);
		}
		else if (color.b > 0) { // color.b == 1 does not work in Release
			color = tex2D(EntityPalette, uv);
			if (EntityRatio > 0.0)
				color = lerp(color, tex2D(EntityPalette2, uv), EntityRatio);
		}
	}
	return color * colorize;
}

technique Technique1 {
	pass Pass1 {
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
