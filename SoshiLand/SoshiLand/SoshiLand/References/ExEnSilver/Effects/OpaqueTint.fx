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
	float4 s = Color * tex2D(implicitInputSampler, uv);
	s.a = 1.0;
	return s;
}
