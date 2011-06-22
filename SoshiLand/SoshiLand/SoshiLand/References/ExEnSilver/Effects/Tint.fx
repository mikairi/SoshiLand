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
	return Color * tex2D(implicitInputSampler, uv);
}
