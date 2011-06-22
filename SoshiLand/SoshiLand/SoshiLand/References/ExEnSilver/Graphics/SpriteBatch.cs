using Microsoft.Xna.Framework;
using System;
using System.Text;
using System.Diagnostics;
using ExEnSilver.Renderer;
using ExEnSilver.Effects;

namespace Microsoft.Xna.Framework.Graphics
{
	[Flags]
	public enum SpriteEffects
	{
		None = 0,
		FlipHorizontally = 1,
		FlipVertically = 256
	}

	public enum SpriteSortMode
	{
		Deferred,

		// These are not supported and currently behave as Deferred
		Immediate,
		Texture,
		BackToFront,
		FrontToBack,
	}


	public class SpriteBatch : IDisposable
	{
		public string Name { get; set; }
		public object Tag { get; set; }
		public GraphicsDevice GraphicsDevice { get; private set; }

		public SpriteBatch(GraphicsDevice graphicsDevice)
		{
			this.GraphicsDevice = graphicsDevice;
		}


		#region Disposable

		public bool IsDisposed { get; private set; }

		public event EventHandler Disposing;

		public void Dispose()
		{
			Dispose(true);
			IsDisposed = true;
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing && Disposing != null)
			{
				Disposing(this, EventArgs.Empty);
			}
		}

		#endregion


		#region In-Batch State

		internal Matrix currentMatrix = Matrix.Identity;
		FixedBlendState fixedBlendState;
		TintEffectMode tintEffectMode;

		#endregion


		#region XNA API Overloads

		#region Begin()

		public void Begin()
		{
			Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);
		}

		public void Begin(SpriteSortMode sortMode, BlendState blendState)
		{
			Begin(sortMode, blendState, null, null, null, null, Matrix.Identity);
		}

		public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
		{
			Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, null, Matrix.Identity);
		}

		public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
		{
			Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, Matrix.Identity);
		}

		public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
		{
			if(blendState != null)
			{
				fixedBlendState = blendState.fixedBlendState;
				tintEffectMode = blendState.tintEffectMode;
			}
			else
			{
				fixedBlendState = FixedBlendState.AlphaBlend;
				tintEffectMode = TintEffectMode.Normal;
			}

			Debug.Assert(samplerState == null);
			Debug.Assert(depthStencilState == null);
			Debug.Assert(rasterizerState == null);

			if(effect != null)
				throw new NotSupportedException("Effect on SpriteBatch is not supported by ExEnSilver");

			currentMatrix = transformMatrix;

			#region Matrix Check
			// Sprite.Transform assumes that the matrix is affine, check that:
			Debug.Assert(transformMatrix.M13 == 0f);
			Debug.Assert(transformMatrix.M14 == 0f);
			Debug.Assert(transformMatrix.M23 == 0f);
			Debug.Assert(transformMatrix.M23 == 0f);
			Debug.Assert(transformMatrix.M31 == 0f);
			Debug.Assert(transformMatrix.M32 == 0f);
			Debug.Assert(transformMatrix.M33 == 1f);
			Debug.Assert(transformMatrix.M34 == 0f);
			Debug.Assert(transformMatrix.M43 == 0f);
			Debug.Assert(transformMatrix.M44 == 1f);
			#endregion
		}

		#endregion

		#region Draw()

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
		{
			if(texture == null) throw new ArgumentNullException("texture");

			if(destinationRectangle.Width == 0 || destinationRectangle.Height == 0)
				return;

			Vector2 scale = ScaleForDestination(destinationRectangle.Width,
					destinationRectangle.Height, texture.Width, texture.Height);
			Vector2 position = new Vector2(destinationRectangle.X, destinationRectangle.Y);

			InternalDraw(texture, position, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		}

		public void Draw(Texture2D texture, Vector2 position, Color color)
		{
			if(texture == null) throw new ArgumentNullException("texture");

			InternalDraw(texture, position, null, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
		}

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
		{
			if(texture == null) throw new ArgumentNullException("texture");

			if(destinationRectangle.Width == 0 || destinationRectangle.Height == 0)
				return;

			Vector2 scale;
			if(sourceRectangle.HasValue)
				scale = ScaleForDestination(destinationRectangle.Width, destinationRectangle.Height, sourceRectangle.Value.Width, sourceRectangle.Value.Height);
			else
				scale = ScaleForDestination(destinationRectangle.Width, destinationRectangle.Height, texture.Width, texture.Height);
			Vector2 position = new Vector2(destinationRectangle.X, destinationRectangle.Y);

			InternalDraw(texture, position, sourceRectangle, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
		{
			if(texture == null) throw new ArgumentNullException("texture");

			InternalDraw(texture, position, sourceRectangle, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
		}

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
		{
			if(texture == null) throw new ArgumentNullException("texture");

			if(destinationRectangle.Width == 0 || destinationRectangle.Height == 0)
				return;

			Vector2 scale;
			if(sourceRectangle.HasValue)
				scale = ScaleForDestination(destinationRectangle.Width, destinationRectangle.Height, sourceRectangle.Value.Width, sourceRectangle.Value.Height);
			else
				scale = ScaleForDestination(destinationRectangle.Width, destinationRectangle.Height, texture.Width, texture.Height);
			Vector2 position = new Vector2(destinationRectangle.X, destinationRectangle.Y);

			InternalDraw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			InternalDraw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			InternalDraw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale), effects, layerDepth);
		}

		#endregion

		#region DrawString()

		public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
		{
			spriteFont.InternalDrawString(this, text, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
		}

		public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
		{
			spriteFont.InternalDrawString(this, text, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
		}

		public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			spriteFont.InternalDrawString(this, text, position, color, rotation, origin, new Vector2(scale), effects, 0);
		}

		public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			spriteFont.InternalDrawString(this, text, position, color, rotation, origin, scale, effects, 0);
		}

		public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			spriteFont.InternalDrawString(this, text, position, color, rotation, origin, new Vector2(scale), effects, 0);
		}

		public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			spriteFont.InternalDrawString(this, text, position, color, rotation, origin, scale, effects, 0);
		}

		#endregion

		public void End()
		{

		}

		#endregion


		#region Silverlight-specific hints

		internal bool hintUseCache = true;
		public void HintDisableCache(bool disable)
		{
			hintUseCache = !disable;
		}

		bool hintDynamicColor = false;
		public void HintDynamicColor(bool enabled)
		{
			hintDynamicColor = enabled;
		}

		bool hintDynamicRectangle = false;
		public void HintDynamicRectangle(bool enable)
		{
			hintDynamicRectangle = enable;
		}

		#endregion


		#region Internal Rendering

		private Vector2 ScaleForDestination(int destWidth, int destHeight, int srcWidth, int srcHeight)
		{
			return new Vector2((float)destWidth  / (float)srcWidth,
							   (float)destHeight / (float)srcHeight);
		}

		internal void InternalDraw(Texture2D texture, Vector2 position,
				Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin,
				Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			byte r, g, b, a;

			switch(fixedBlendState)
			{
				default:
				case FixedBlendState.AlphaBlend: // One, 1-SrcAlpha
					r = color.cR;
					g = color.cG;
					b = color.cB;
					a = color.A;
					// TODO: Emit warning if premultiplied R,G or B is greater than A.
					//       This would normally cause additive blending. Currently not handled by ExEnSilver.
					break;

				case FixedBlendState.NonPremultiplied: // SrcAlpha, 1-SrcAlpha
					r = color.R;
					g = color.G;
					b = color.B;
					a = color.A;
					break;

				case FixedBlendState.Additive: // SrcAlpha, One
					r = color.R;
					g = color.G;
					b = color.B;
					a = color.A;
					break;

				case FixedBlendState.Opaque: // One, Zero
					r = color.R;
					g = color.G;
					b = color.B;
					a = 255;
					break;
			}

			if(a == 0) // Nothing to draw
				return;

			// Dynamic rectangle only used if there's actually a source rectangle!
			bool dynamicRectangle = hintDynamicRectangle && sourceRectangle.HasValue;

			ImageSourceId id = new ImageSourceId(r, g, b, sourceRectangle, tintEffectMode,
					hintUseCache, hintDynamicColor, dynamicRectangle);
			ImageSprite sprite = texture.GetSprite(id);

			if(hintDynamicColor)
				sprite.UpdateDynamicColor(r, g, b);
			if(dynamicRectangle)
				sprite.UpdateDynamicRectangle(sourceRectangle.Value);

			Vector2 sourceSize = sourceRectangle.HasValue ?
					new Vector2(sourceRectangle.Value.Width, sourceRectangle.Value.Height) : 
					new Vector2(texture.Width, texture.Height);
			Vector2 sourceOrigin = dynamicRectangle ?
					new Vector2(sourceRectangle.Value.X, sourceRectangle.Value.Y) : 
					Vector2.Zero;

			sprite.Transform(position, scale, origin, sourceOrigin, sourceSize, rotation, effects, ref currentMatrix);
			sprite.SetOpacity(a / 255f);

			GraphicsDevice.DrawSprite(sprite);
		}

		#endregion

	}
}
