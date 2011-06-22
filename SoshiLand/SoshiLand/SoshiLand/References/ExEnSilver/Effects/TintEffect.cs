using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Effects;

namespace ExEnSilver.Effects
{
	public enum TintEffectMode
	{
		Normal,
		Additive,
		Opaque,
		Last
	}

	public class TintEffect : ShaderEffect
	{
		public static DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(System.Windows.Media.Color), typeof(TintEffect), new PropertyMetadata(new System.Windows.Media.Color(), PixelShaderConstantCallback(2)));

		public virtual System.Windows.Media.Color Color
		{
			get { return ((System.Windows.Media.Color)(GetValue(ColorProperty))); }
			set { SetValue(ColorProperty, value); }
		}


		#region Effect Pools

		static Stack<TintEffect>[] pools;

		static TintEffect()
		{
			pools = new Stack<TintEffect>[(int)TintEffectMode.Last];
			for(int i = 0; i < pools.Length; i++)
			{
				pools[i] = new Stack<TintEffect>();
			}
		}

		public static TintEffect Create(TintEffectMode mode)
		{
			Stack<TintEffect> pool = pools[(int)mode];
			if(pool.Count > 0)
				return pool.Pop();
			else
				return new TintEffect(mode);
		}

		#endregion


		TintEffectMode mode;

		public void Release()
		{
			Stack<TintEffect> pool = pools[(int)mode];
			pool.Push(this);
		}

		public TintEffect(TintEffectMode mode)
		{
			this.mode = mode;

			string url;
			switch(mode)
			{
				default:
				case TintEffectMode.Normal:
					url = @"/ExEnSilver;component/Effects/Tint.ps";
					break;
				case TintEffectMode.Additive:
					url = @"/ExEnSilver;component/Effects/AdditiveTint.ps";
					break;
				case TintEffectMode.Opaque:
					url = @"/ExEnSilver;component/Effects/OpaqueTint.ps";
					break;
			}

			Uri u = new Uri(url, UriKind.Relative);
			PixelShader = new PixelShader() { UriSource = u };
		}
	}
}
