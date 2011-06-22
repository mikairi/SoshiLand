using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ExEnSilver.Renderer;

namespace Microsoft.Xna.Framework.Graphics
{
	public sealed class SpriteFontTTF : SpriteFont
	{
		// Due to a bug in Silverlight, setting the FontSource via the FontFamily is unreliable
		// So use FontSource to specify the source of any external font files
		public readonly FontFamily FontFamily;
		public readonly FontSource FontSource;

		/// <summary>Font size in pixels.</summary>
		public readonly double FontSize;

		// TODO: font sizes do not match XNA exactly
		const double pointsToPixels = 96.0 / 72.0;


		/// <param name="fontSize">Font size in points.</param>
		public SpriteFontTTF(FontSource source, FontFamily family, int fontSize)
		{
			this.FontFamily = family;
			this.FontSource = source;
			this.FontSize = fontSize * pointsToPixels;

			measuringTextBlock = new TextBlock()
			{
				FontSize = FontSize,
				FontFamily = FontFamily,
				FontSource = FontSource
			};
		}


		#region Measuring Text

		TextBlock measuringTextBlock;

		public override Vector2 MeasureString(string text)
		{
			// Try and find an existing, matching TextSprite, to save allocation and rendering
			for(int i = 0; i < sprites.Count; i++)
			{
				if(ReferenceEquals(sprites[i].key, text))
				{
					// Strings are immutable, so size will not have changed
					return sprites[i].value.NaturalSize;
				}
			}

			measuringTextBlock.Text = text;
			return new Vector2((float)measuringTextBlock.ActualWidth, (float)measuringTextBlock.ActualHeight);
		}

		public override Vector2 MeasureString(StringBuilder text)
		{
			// Try and find an existing, matching TextSprite, to save allocation and rendering
			for(int i = 0; i < sprites.Count; i++)
			{
				if(ReferenceEquals(sprites[i].key, text))
				{
					if(!HasStringBuilderChanged(text, sprites[i].renderedStringBuilder))
					{
						return sprites[i].value.NaturalSize;
					}

					// Actual content of StringBuilder did not match
					// and writing code to check if it can be safely modified is hard
					// don't want to keep looking through list, so...
					break;
				}
			}

			measuringTextBlock.Text = text.ToString();
			return new Vector2((float)measuringTextBlock.ActualWidth, (float)measuringTextBlock.ActualHeight);
		}

		#endregion


		#region Public XNA Properties

		public static ReadOnlyCollection<char> defaultCharacterList = new ReadOnlyCollection<char>(Enumerable.Range(32, 95).Select(i => (char)i).ToList());

		// These are not really possible to implement using the tools Silverlight provides,
		// (Plus do we really want an entire font worth of Characters?)
		// So simply do a good enough job of implementing the getter functions:
		public override ReadOnlyCollection<char> Characters { get { return defaultCharacterList; } }
		public override char? DefaultCharacter { get { return '_'; } set { } }
		public override int LineSpacing { get { return (int)MeasureString("lj").Y; } set { } }
		public override float Spacing { get { return 0; } set { } }

		#endregion


		#region Sprites using this font

		// TODO: this could probably be packed better
		struct TextSpriteInstance
		{
			public object key;
			public TextSprite value;
			public string renderedStringBuilder;
			public int foundOnFrame;
			public bool cached;
		}

		List<TextSpriteInstance> sprites = new List<TextSpriteInstance>();

		private TextSprite CreateSprite(object key, string renderedStringBuilder,
				int frameNumber, bool cached, GraphicsDevice device)
		{
			// First try to find an existing sprite to reuse
			for(int i = 0; i < sprites.Count; i++)
			{	
				// If it was last used before the immediately previous frame, assume it is free to use:
				if(sprites[i].cached == cached && sprites[i].foundOnFrame < frameNumber-1)
				{
					TextSpriteInstance instance = sprites[i];
					instance.key = key;
					instance.renderedStringBuilder = renderedStringBuilder;
					instance.foundOnFrame = frameNumber;
					sprites[i] = instance;
					return instance.value;
				}
			}

			// If no sprite to reuse was found...
			TextSprite sprite = new TextSprite(this, cached);
			sprites.Add(new TextSpriteInstance()
			{
				key = key,
				value = sprite,
				renderedStringBuilder = renderedStringBuilder,
				foundOnFrame = frameNumber,
				cached = cached
			});
			device.AddSprite(sprite);
			return sprite;
		}

		private static bool HasStringBuilderChanged(StringBuilder sb, string rendered)
		{
			if(rendered.Length != sb.Length)
				return true;

			for(int c = rendered.Length - 1; c >= 0; c--)
			{
				if(sb[c] != rendered[c])
				{
					return true;
				}
			}

			return false;
		}

		private TextSprite UpdateStringBuilderInstance(StringBuilder text, int frameNumber, int i)
		{
			TextSpriteInstance instance = sprites[i];

			// If the value of the StringBuilder has changed, re-render it:
			if(HasStringBuilderChanged(text, instance.renderedStringBuilder))
			{
				instance.renderedStringBuilder = text.ToString(); // Allocates memory
				instance.value.SetText(instance.renderedStringBuilder); // Causes rendering
			}

			// Always update the frame number, then insert it back into the list of sprites
			instance.foundOnFrame = frameNumber;
			sprites[i] = instance;

			return sprites[i].value;
		}


		private TextSprite GetSprite(StringBuilder text, bool cached, GraphicsDevice device)
		{
			int frameNumber = device.CurrentSpriteFrameNumber;

			// Find the TextSprite instance that goes with this StringBuilder
			for(int i = 0; i < sprites.Count; i++)
			{
				if(ReferenceEquals(sprites[i].key, text)
						&& sprites[i].cached == cached && sprites[i].foundOnFrame != frameNumber)
				{
					return UpdateStringBuilderInstance(text, frameNumber, i);
				}
			}

			// No match found, create one:
			string renderedText = text.ToString();
			TextSprite sprite = CreateSprite(text, renderedText, frameNumber, cached, device);
			sprite.SetText(renderedText);
			return sprite;
		}

		private TextSprite GetSprite(string text, bool cached, GraphicsDevice device)
		{
			int frameNumber = device.CurrentSpriteFrameNumber;

			// Strings are immutable, if an instance is matched: use it directly
			for(int i = 0; i < sprites.Count; i++)
			{
				if(ReferenceEquals(sprites[i].key, text)
						&& sprites[i].cached == cached && sprites[i].foundOnFrame != frameNumber)
				{
					TextSpriteInstance instance = sprites[i];
					instance.foundOnFrame = frameNumber;
					sprites[i] = instance;
					return instance.value;
				}
			}

			// No match found, create one:
			TextSprite sprite = CreateSprite(text, null, frameNumber, cached, device);
			sprite.SetText(text);
			return sprite;
		}

		#endregion



		#region Internal Drawing

		private void DrawTextSprite(SpriteBatch sb, TextSprite sprite, Vector2 position, Color color,
				float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
		{
			sprite.SetColor(color);
			sprite.Transform(position, scale, origin, Vector2.Zero, sprite.NaturalSize, rotation, effects,
					ref sb.currentMatrix);

			sb.GraphicsDevice.DrawSprite(sprite);
		}

		internal override void InternalDrawString(SpriteBatch sb, StringBuilder text,
				Vector2 position, Color color, float rotation, Vector2 origin,
				Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			TextSprite sprite = GetSprite(text, sb.hintUseCache, sb.GraphicsDevice);
			DrawTextSprite(sb, sprite, position, color, rotation, origin, scale, effects);
		}

		internal override void InternalDrawString(SpriteBatch sb, string text,
				Vector2 position, Color color, float rotation, Vector2 origin,
				Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			TextSprite sprite = GetSprite(text, sb.hintUseCache, sb.GraphicsDevice);
			DrawTextSprite(sb, sprite, position, color, rotation, origin, scale, effects);
		}

		#endregion

	}
}
