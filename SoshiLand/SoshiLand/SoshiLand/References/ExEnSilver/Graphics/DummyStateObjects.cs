using System;

namespace Microsoft.Xna.Framework.Graphics
{
	// Because these states can't really be implemented in Silverlight,
	// and so they aren't used, simply provide dummy versions that should
	// allow most code to compile.
	//
	// For code that creates states, it is recommended to wrap it in #if !SILVERLIGHT
	// If you use "object initializer syntax", then wrapping the contents of the
	// initializer block in #if !SILVERLIGHT looks reasonable.

	public class SamplerState
	{
		public static readonly SamplerState AnisotropicClamp = null;
		public static readonly SamplerState AnisotropicWrap = null;
		public static readonly SamplerState LinearClamp = null;
		public static readonly SamplerState LinearWrap = null;
		public static readonly SamplerState PointClamp = null;
		public static readonly SamplerState PointWrap = null;
	}

	public class DepthStencilState
	{
		public static readonly DepthStencilState Default = null;
		public static readonly DepthStencilState DepthRead = null;
		public static readonly DepthStencilState None = null;
	}

	public class RasterizerState
	{
		public static readonly RasterizerState CullClockwise = null;
		public static readonly RasterizerState CullCounterClockwise = null;
		public static readonly RasterizerState CullNone = null;
	}


	// Impossible to construct - always use "null" for Effect in SpriteBatch
	public class Effect { private Effect() { } }

}
