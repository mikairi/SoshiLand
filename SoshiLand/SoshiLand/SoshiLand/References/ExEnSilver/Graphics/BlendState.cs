using System;
using ExEnSilver.Effects;

namespace Microsoft.Xna.Framework.Graphics
{
	internal enum FixedBlendState
	{
		AlphaBlend,
		NonPremultiplied,
		Additive,
		Opaque
	}


	public class BlendState
	{
		#region Static blend states

		public static readonly BlendState Additive;
		public static readonly BlendState AlphaBlend;
		public static readonly BlendState NonPremultiplied;
		public static readonly BlendState Opaque;

		static BlendState()
		{
			AlphaBlend = new BlendState(FixedBlendState.AlphaBlend, TintEffectMode.Normal, "BlendState.AlphaBlend");
			NonPremultiplied = new BlendState(FixedBlendState.NonPremultiplied, TintEffectMode.Normal, "BlendState.NonPremultiplied");
			Additive = new BlendState(FixedBlendState.Additive, TintEffectMode.Additive, "BlendState.Additive");
			Opaque = new BlendState(FixedBlendState.Opaque, TintEffectMode.Opaque, "BlendState.Opaque");
		}

		#endregion

		private BlendState(FixedBlendState fixedBlendState, TintEffectMode mode, string name)
		{
			this.fixedBlendState = fixedBlendState;
			this.tintEffectMode = mode;
			this.Name = name;
		}

		internal readonly FixedBlendState fixedBlendState;
		internal readonly TintEffectMode tintEffectMode;

		public string Name { get; private set; }

	}
}
