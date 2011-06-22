using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SWM=System.Windows.Media;

namespace ExEnSilver.Renderer
{
	public abstract class Sprite
	{
		protected Canvas canvas = new Canvas();
		internal Canvas AddToRoot { get { return canvas; } }

		internal int usedOnFrame = -1;


		#region Setup

		// Results from Benchmarking:
		//
		// Animated Colour Changes:
		//    - No Cache
		//      (Silverlight will *automatically* uncache image if it has an effect, can use no-colour-change path. yay!)
		//    - Transform on image
		//    - Image on canvas
		//    - Effect on canvas
		//
		// No Colour Changes:
		//    - Optional Cache Image (will depend on if this will split an intermediate layer)
		//      (have an option to block caching, or disable it automatically in (pre-?)render pass based on surrounding objects)
		//    - Tranform on Image (could be on canvas, as long as layout and transform are on the same object, no difference)
		//    - Image on canvas (may have tiny framerate impact, negligble and lets me have identical codepath)
		//
		// Fix Colour
		//    - Same as No Colour Changes
		//    - Set image source to writable bitmap copy of image
		//    - Requires hint call
		//

		// Insulating the element from the parent canvas with another canvas
		// seems to result in a *significant* performance boost (perhaps to do with layout)

		#region Options For Benchmarking
		// Some of these are no longer in use - for documentation only
		//
		//internal protected UIElement ApplyTransformTo { get { return element; } }
		//internal protected UIElement ApplyLayoutOffsetTo { get { return element; } }
		//internal protected UIElement ApplyEffectTo { get { return canvas; } }
		//internal protected UIElement ApplyCacheTo { get { return element; } }
		//internal protected UIElement ApplyOpacityTo { get { return element; } }
		//internal protected UIElement AddToRoot { get { return canvas; } }
		//
		#endregion

		protected void Create(UIElement element, bool wantsCache, bool wantsTransform)
		{
			if(wantsTransform)
				element.RenderTransform = transform = new SWM.MatrixTransform();

			if(wantsCache)
				element.CacheMode = new SWM.BitmapCache();

			canvas.Children.Add(element);
		}

		#endregion


		#region Sprite Placement

		protected SWM.MatrixTransform transform;

		public void Transform(Vector2 position, Vector2 scale, Vector2 origin,
				Vector2 sourceOrigin, Vector2 sourceSize, float rotation,
				SpriteEffects spriteEffects, ref Matrix matrix)
		{
			// Performance thoughts:
			// These conditionals could be removed by bit-twiddling to integer and then casting to double
			// But how does that compare to the performance hit of converting int to double?
			// (Does it matter? Doesn't seem to...)
			double reflectX = ((spriteEffects & SpriteEffects.FlipHorizontally) != 0) ? -1 : 1;
			double reflectY = ((spriteEffects & SpriteEffects.FlipVertically)   != 0) ? -1 : 1;
			double reflectTranslateX = ((spriteEffects & SpriteEffects.FlipHorizontally) != 0) ? -sourceSize.X : 0;
			double reflectTranslateY = ((spriteEffects & SpriteEffects.FlipVertically)   != 0) ? -sourceSize.Y : 0;

			// Matrix Operations:
			//Transform2D t = Transform2D.Identity;
			//t *= Transform2D.CreateTranslate(new Vector2(reflectTranslateX - sourceOrigin.X, reflectTranslateY - sourceOrigin.Y));
			//t *= Transform2D.CreateScale(new Vector2(reflectX, reflectY));
			//t *= Transform2D.CreateTranslateScaleRotate(-origin, new Vector2(scale.X, scale.Y), rotation);
			//t *= Transform2D.CreateTranslate(position);
			//transform.Matrix = t.AsSilverlightMatrix;
			

			// Inlined Version (Rough maths is in Book 2010A)
			double cos = Math.Cos(rotation);
			double sin = Math.Sin(rotation);

			double A =  cos * scale.X; double B = sin * scale.X;
			double C = -sin * scale.Y; double D = cos * scale.Y;
			double pX = (reflectX * (reflectTranslateX - sourceOrigin.X) - origin.X);
			double pY = (reflectY * (reflectTranslateY - sourceOrigin.Y) - origin.Y);

			double M11 = reflectX * A;
			double M12 = reflectX * B;
			double M21 = reflectY * C;
			double M22 = reflectY * D;
			double OffsetX = pX * A + pY * C + position.X;
			double OffsetY = pX * B + pY * D + position.Y;

			// Multiply with transform matrix (assume it is Affine 2D, this is Asserted in SpriteBatch.Begin)
			SWM.Matrix m = new System.Windows.Media.Matrix();
			m.M11 = M11 * matrix.M11 + M12 * matrix.M21;
			m.M12 = M11 * matrix.M12 + M12 * matrix.M22;
			m.M21 = M21 * matrix.M11 + M22 * matrix.M21;
			m.M22 = M21 * matrix.M12 + M22 * matrix.M22;
			m.OffsetX = OffsetX * matrix.M11 + OffsetY * matrix.M21 + matrix.M41;
			m.OffsetY =	OffsetX * matrix.M12 + OffsetY * matrix.M22 + matrix.M42;
			transform.Matrix = m;
		}

		#endregion

	}
}
