using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Diagnostics;
using IgorO.ExposedObjectProject;
using System.IO;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Drawing;
using Color=Microsoft.Xna.Framework.Color;
using Rectangle=Microsoft.Xna.Framework.Rectangle;
using System.Drawing.Imaging;
using Nuclex.Fonts.Content;
using SpriteFontContent=Microsoft.Xna.Framework.Content.Pipeline.Processors.SpriteFontContent;
using NuclexSpriteFontContent=Nuclex.Fonts.Content.SpriteFontContent;
using System.Text;

namespace ExEnFontShim
{
	internal static class ExtensionMethods
	{
		public static bool WriteBoolean(this BinaryWriter bw, bool value)
		{
			bw.Write(value);
			return value;
		}

		public static void Write(this BinaryWriter bw, Rectangle value)
		{
			bw.Write(value.X);
			bw.Write(value.Y);
			bw.Write(value.Width);
			bw.Write(value.Height);
		}

		public static void Write(this BinaryWriter bw, Vector3 value)
		{
			bw.Write(value.X);
			bw.Write(value.Y);
			bw.Write(value.Z);
		}
	}

	internal static class FontDescriptionExtensions
	{
		public static void BecomeAt2x(this FontDescription fontDescription)
		{
			fontDescription.Size = fontDescription.Size * 2;
			fontDescription.Spacing = fontDescription.Spacing * 2;
		}
	}


	internal static class ExEnFontWriter
	{
		private static string GetOutputDirectory(ContentIdentity identity)
		{
			return Path.GetDirectoryName(identity.SourceFilename);
		}

		internal static void CreateOutputDirectory(ContentIdentity identity)
		{
			// Directory.CreateDirectory(GetOutputDirectory(identity));
		}

		internal static string AssetOutputFilename(ContentIdentity identity, ContentProcessorContext context, string suffixDotExtension)
		{
			string assetName = Path.GetFileNameWithoutExtension(context.OutputFilename);
			return Path.Combine(GetOutputDirectory(identity), assetName + suffixDotExtension);
		}


		internal static void WriteTexture(object spriteFontContent, bool alphaOnly, ContentProcessorContext context, string filename)
		{
			dynamic sfc = ExposedObject.From(spriteFontContent);

			// Get a copy of the texture in Color format
			Texture2DContent originalTexture = sfc.Texture;
			BitmapContent originalBitmap = originalTexture.Mipmaps[0];
			PixelBitmapContent<Color> colorBitmap = new PixelBitmapContent<Color>(
					originalBitmap.Width, originalBitmap.Height);
			BitmapContent.Copy(originalBitmap, colorBitmap);


			Bitmap bitmap = new Bitmap(colorBitmap.Width, colorBitmap.Height, PixelFormat.Format32bppArgb);
			for(int x = 0; x < colorBitmap.Width; x++) for(int y = 0; y < colorBitmap.Height; y++)
			{
				Color c = colorBitmap.GetPixel(x, y);
				if(alphaOnly)
				{
					c.R = 255; c.G = 255; c.B = 255; // Undo premultiplication
				}
				bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B));
			}
			bitmap.Save(filename, ImageFormat.Png);
			bitmap.Dispose();

			context.AddOutputFile(filename);
		}


		internal static void WriteMetrics(object spriteFontContent, ContentProcessorContext context, string filename)
		{
			dynamic sfc = ExposedObject.From(spriteFontContent);

			using(FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write))
			{
				using(BinaryWriter bw = new BinaryWriter(fs, Encoding.Unicode))
				{
					// Identifier and version:
					bw.Write((int)0x6E457845); // ExEn
					bw.Write((int)0x746E6F46); // Font
					bw.Write((int)0);

					// Write common properties
					bw.Write((int)sfc.LineSpacing);
					bw.Write((int)sfc.Spacing);
					if(bw.WriteBoolean(((char?)sfc.DefaultCharacter).HasValue))
						bw.Write(((char?)sfc.DefaultCharacter).Value);

					// Write glyph list:
					int count = sfc.CharacterMap.Count;
					bw.Write(count);
					for(int i = 0; i < count; i++)
					{
						bw.Write((char)sfc.CharacterMap[i]);
						bw.Write((Rectangle)sfc.Glyphs[i]);
						bw.Write((Rectangle)sfc.Cropping[i]);
						bw.Write((Vector3)sfc.Kerning[i]);
					}
				}				
			}

			context.AddDependency(filename);
		}

	}



	[ContentProcessor(DisplayName = "ExEn Font Shim - Sprite Font Description")]
	public class FontDescriptionProcessorShim : FontDescriptionProcessor
	{
		private void CreateExEnOutput(SpriteFontContent spriteFontContent, FontDescription input, ContentProcessorContext context)
		{
			ExEnFontWriter.CreateOutputDirectory(input.Identity);

			ExEnFontWriter.WriteTexture(spriteFontContent, true, context,
					ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont.png"));
			ExEnFontWriter.WriteMetrics(spriteFontContent, context,
					ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont.exenfont"));

			// Retina Display
			input.BecomeAt2x();
			SpriteFontContent spriteFontContentAt2x = base.Process(input, context);
			ExEnFontWriter.WriteTexture(spriteFontContentAt2x, true, context,
					ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont@2x.png"));
			ExEnFontWriter.WriteMetrics(spriteFontContentAt2x, context,
					ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont@2x.exenfont"));
		}

		public override SpriteFontContent Process(FontDescription input, ContentProcessorContext context)
		{
			SpriteFontContent sfc = base.Process(input, context);

			if(context.TargetPlatform == TargetPlatform.Windows)
				CreateExEnOutput(sfc, input, context);

			return sfc;
		}
	}

	
	[ContentProcessor(DisplayName = "ExEn Font Shim - Nuclex Sprite Font")]
	public class NuclexFontDescriptionProcessorShim : NuclexSpriteFontDescriptionProcessor
	{
		private void CreateExEnOutput(NuclexSpriteFontContent spriteFontContent, FontDescription input, ContentProcessorContext context)
		{
			ExEnFontWriter.CreateOutputDirectory(input.Identity);

			ExEnFontWriter.WriteTexture(spriteFontContent, true, context,
					ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont.png"));
			ExEnFontWriter.WriteMetrics(spriteFontContent, context,
					ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont.exenfont"));

			// Retina Display
			input.BecomeAt2x();
			NuclexSpriteFontContent spriteFontContentAt2x = base.Process(input, context);
			ExEnFontWriter.WriteTexture(spriteFontContentAt2x, true, context,
					ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont@2x.png"));
			ExEnFontWriter.WriteMetrics(spriteFontContentAt2x, context,
					ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont@2x.exenfont"));
		}

		public override NuclexSpriteFontContent Process(FontDescription input, ContentProcessorContext context)
		{
			NuclexSpriteFontContent sfc = base.Process(input, context);

			if(context.TargetPlatform == TargetPlatform.Windows)
				CreateExEnOutput(sfc, input, context);

			return sfc;
		}
	}

	
	[ContentProcessor(DisplayName = "ExEn Font Shim - Sprite Font Texture")]
	public class FontTextureProcessorShim : FontTextureProcessor
	{
		private void CreateExEnOutput(Texture2DContent input, ContentProcessorContext context)
		{
			ExEnFontWriter.CreateOutputDirectory(input.Identity);

			// Put the processor in a format suitable for outputting to PNG
			var originalPremultiply = PremultiplyAlpha;
			var originalFormat = TextureFormat;
			PremultiplyAlpha = false;
			TextureFormat = TextureProcessorOutputFormat.Color;

			// Build normal size:
			SpriteFontContent spriteFontContent = base.Process(input, context);
			ExEnFontWriter.WriteTexture(spriteFontContent, false, context,
					ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont.png"));
			ExEnFontWriter.WriteMetrics(spriteFontContent, context,
					ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont.exenfont"));
			
			// Check for retina size:
			string pathAt2x = Path.Combine(Path.GetDirectoryName(input.Identity.SourceFilename),
					Path.GetFileNameWithoutExtension(input.Identity.SourceFilename) + "@2x"
					+ Path.GetExtension(input.Identity.SourceFilename));
			if(File.Exists(pathAt2x))
			{
				var textureImporter = new TextureImporter();
				var textureAt2x = (Texture2DContent)textureImporter.Import(pathAt2x, null);
				context.AddDependency(pathAt2x);

				var spriteFontContentAt2x = base.Process(textureAt2x, context);
				ExEnFontWriter.WriteTexture(spriteFontContentAt2x, false, context,
						ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont@2x.png"));
				ExEnFontWriter.WriteMetrics(spriteFontContentAt2x, context,
						ExEnFontWriter.AssetOutputFilename(input.Identity, context, "-exenfont@2x.exenfont"));
			}

			PremultiplyAlpha = originalPremultiply;
			TextureFormat = originalFormat;
		}

		public override SpriteFontContent Process(Texture2DContent input, ContentProcessorContext context)
		{
			if(context.TargetPlatform == TargetPlatform.Windows)
				CreateExEnOutput(input, context);

			return base.Process(input, context);
		}
	}
	
}
