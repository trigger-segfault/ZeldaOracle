sampler RenderTarget:	register(s0);

// The size of the render target in pixels
float2 TargetSize;
// The vertical pixel offset to start the sine wave at
float Offset;
// The pixel height of a full sine wave
float Height;
// The horizontal magnitude of the sine wave
float Magnitude;
// The background to use when the requested pixel is out of bounds
float4 Background;

static const float PI = 3.14159265;
static const float PI2 = 6.28318531;
//static const float2 OFFSET = float2(0.5, 0.5);

float4 PixelShaderFunction(float4 colorize: COLOR0, float2 texCoord : TEXCOORD0) : COLOR0{
	// Input settings are in pixel coordinates so get them in hlsl coordinates
    float offset = Offset / TargetSize.y;
    float height = Height / TargetSize.y;
    float sine = sin((offset + texCoord.y) / height * PI2);
	// 0.5 is to get the center of the pixel which
	// HLSL ignorantly doesn't aquire by default.
   // float shift = (round(sine * Strength) + 0.5) / TargetSize.x
    float shift = round(sine * Magnitude) / TargetSize.x;

	// Get the shifted coordinates to grab the pixel from
    float2 newCoord = float2(texCoord.x + shift, texCoord.y);

	// Check if we're out of bounds
    if (newCoord.x < 0 || newCoord.x > 1)
        return Background * colorize;

    float4 color = tex2D(RenderTarget, newCoord);
    return color * colorize;
}

technique Technique1 {
	pass Pass1 {
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
