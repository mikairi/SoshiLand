//
// Constants
//
float4 Color : register(C2);

//
// Sampler Inputs (Brushes, including ImplicitInput)
//
sampler2D implicitInputSampler : register(S0);


//
// Pixel Shader
//
float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 s = tex2D(implicitInputSampler, uv);
	
	//
	// This is a work-around for Silverlight's software renderer, which takes some
	// (probably integer-maths) shortcuts when calculating lower opacities.
	// In the software renderer, an alpha value of zero will not render at all
	// without an opacity of one, so compromise with a small alpha value where
	// it normally should be zero. Modulate by the intensity of the original image
	// so that the black backgorund of an additive image will have no effect.
	//
	// This does ignore the original alpha value of the image.
	//
	
	float i = (s.r + s.g + s.b)/3.0;
	s.a = min(i/4.0, 8.0/256.0);

	return Color * s;
}
