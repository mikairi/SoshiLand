using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using ExEnSilver.Graphics;
using System.Windows.Controls;
using ExEnSilver.Effects;
using SWM=System.Windows.Media;
using ExEnSilver.Renderer;

namespace Microsoft.Xna.Framework.Graphics
{
	public class Texture2D : IDisposable
	{
		public GraphicsDevice GraphicsDevice { get; private set; }
		
		public BitmapImage ImageSource { get; protected set; }

		int width, height;

		public Texture2D(Stream source, GraphicsDevice device)
		{
			GraphicsDevice = device;
			ImageSource = ImageLoader.GetBitmapImage(source);

			width = ImageSource.PixelWidth;
			height = ImageSource.PixelHeight;
		}



		#region Pre-processed surfaces

		private Dictionary<ImageSourceId, WriteableBitmap> specialSources;

		WriteableBitmap CreateSourceFor(ImageSourceId id)
		{
			BitmapSource original = ImageSource;

			// Note: need to recreate temporaryImage each time, because Silverlight
			//       has some very strange ideas about when to update the image dimensions
			Image temporaryImage = new Image();

			TintEffect temporaryTintEffect = null;
			if(id.WantsPreTint)
			{
				temporaryTintEffect = TintEffect.Create(id.TintEffectMode);
				temporaryImage.Effect = temporaryTintEffect;
				temporaryTintEffect.Color = id.ColorForPreTint;
			}

			temporaryImage.Width = original.PixelWidth;
			temporaryImage.Height = original.PixelHeight;
			temporaryImage.Source = original;


			int width, height;
			SWM.TranslateTransform transform;

			if(id.UseOriginalDimentions)
			{
				width = original.PixelWidth;
				height = original.PixelHeight;
				transform = null;
			}
			else
			{
				width = id.SourceWidth;
				height = id.SourceHeight;

				transform = new SWM.TranslateTransform();
				transform.X = -id.SourceX;
				transform.Y = -id.SourceY;
			}

			WriteableBitmap output = new WriteableBitmap(width, height);
			output.Render(temporaryImage, transform);
			output.Invalidate();

			if(temporaryTintEffect != null)
			{
				temporaryImage.Effect = null;
				temporaryTintEffect.Release();
			}

			return output;
		}

		public BitmapSource GetSpecialSource(ImageSourceId id)
		{
			if(id.TextureLookupOriginal)
				return ImageSource;

			WriteableBitmap output = null;

			if(specialSources == null)
				specialSources = new Dictionary<ImageSourceId, WriteableBitmap>();
			else
				specialSources.TryGetValue(id, out output);
			
			if(output == null)
			{
				output = CreateSourceFor(id);
				specialSources.Add(id, output);
			}

			return output;
		}

		public void ClearSpecialSources()
		{
			if(specialSources != null)
				specialSources.Clear();
		}

		#endregion



		#region Sprites Using This Texture

		internal Dictionary<ImageSourceId, List<ImageSprite>> sprites = new Dictionary<ImageSourceId, List<ImageSprite>>();

		internal ImageSprite GetSprite(ImageSourceId id)
		{
			List<ImageSprite> spriteList;
			sprites.TryGetValue(id, out spriteList);

			if(spriteList == null)
			{
				spriteList = new List<ImageSprite>();
				sprites.Add(id, spriteList);
			}

			int frame = GraphicsDevice.CurrentSpriteFrameNumber;
			foreach(ImageSprite sprite in spriteList)
			{
				if(sprite.usedOnFrame != frame)
					return sprite;
			}

			// No suitable sprite found, add one:
			ImageSprite createSprite = new ImageSprite(this, id);
			GraphicsDevice.AddSprite(createSprite);
			spriteList.Add(createSprite);
			return createSprite;
		}

		#endregion



		#region XNA API

		public int Width { get { return width; } }
		public int Height { get { return height; } }

		public void Dispose()
		{
		}

		#endregion

	}
}
