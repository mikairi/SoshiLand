using System;
using System.Windows.Controls;
using Microsoft.Xna.Framework.Graphics;
using ExEnSilver.Effects;
using SWM=System.Windows.Media;
using Microsoft.Xna.Framework;

namespace ExEnSilver.Renderer
{
	public class ImageSprite : Sprite
	{
		Image image;

		public ImageSprite(Texture2D texture, ImageSourceId sourceId)
		{
			image = new Image();
			var source = texture.GetSpecialSource(sourceId.ForTextureLookup);
			image.Source = source;
			image.Width = source.PixelWidth;
			image.Height = source.PixelHeight;

			base.Create(image, sourceId.Cache, true);

			if(sourceId.DynamicColor)
			{
				// Note: This tint effect does not apply additive blending
				//       (because theoretically the non-additive version should be faster).
				//       Additive-ness is already applied at the image source.
				//       (Ditto for Opaque blending)
				canvas.Effect = tintEffect = TintEffect.Create(TintEffectMode.Normal);
			}

			if(sourceId.DynamicRectangle)
			{
				image.Clip = clipGeometry = new SWM.RectangleGeometry();
			}
		}


		#region Dynamic Color

		private TintEffect tintEffect;

		public void UpdateDynamicColor(byte r, byte g, byte b)
		{
			if(tintEffect == null)
				throw new InvalidOperationException();

			tintEffect.Color = System.Windows.Media.Color.FromArgb(255, r, g, b);
		}

		#endregion


		#region Dynamic Rectangle

		SWM.RectangleGeometry clipGeometry;

		public void UpdateDynamicRectangle(Rectangle rectangle)
		{
			clipGeometry.Rect = new System.Windows.Rect(rectangle.X,
					rectangle.Y, rectangle.Width, rectangle.Height);
		}

		#endregion



		public void SetOpacity(float opacity)
		{
			image.Opacity = opacity;
		}


		// TODO: dynamic source rectangle
	}
}
