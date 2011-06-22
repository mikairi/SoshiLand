using System;
using SWM=System.Windows.Media;
using System.Diagnostics;
using ExEnSilver.Effects;

namespace Microsoft.Xna.Framework.Graphics
{
	public struct ImageSourceId : IEquatable<ImageSourceId>
	{
		public const uint OpaqueWhiteMask = 0xffffff00;
		public const uint AdditiveBlendMask = 0x80;
		public const uint CacheMask = 0x40;
		public const uint DynamicColorMask = 0x20;
		public const uint DynamicRectangleMask = 0x10;
		public const uint OpaqueBlendMask = 0x08;
		public const uint NonDynamicMask = OpaqueWhiteMask | AdditiveBlendMask | OpaqueBlendMask;


		public static uint FlagForTintEffectMode(TintEffectMode tintEffectMode)
		{
			switch(tintEffectMode)
			{
				case TintEffectMode.Additive: return AdditiveBlendMask;
				case TintEffectMode.Opaque: return OpaqueBlendMask;
				default: return 0;
			}
		}



		#region Data

		// Packing Information (from MSB to LSB)
		//
		// colorAndFlags:
		// 8 bits: Blue   }
		// 8 bits: Green  }- Matches the packing order of Color
		// 8 bits: Red    }
		// 1 bit: Additive Blend
		// 1 bit: Cache surface
		// 1 bit: Dynamic color
		// 1 bit: dynamic rectangle
		// 1 bit: Opaque Blend
		// 3 bits: UNUSED
		//
		// sourceOrigin:
		// 16 bits: Y
		// 16 bits: X
		//
		// sourceSize:
		// 16 bits: Height
		// 16 bits: Width
		//

		public readonly uint colorAndFlags;
		public readonly uint sourceOrigin;
		public readonly uint sourceSize;

		#endregion


		#region Data Unpacking

		public int SourceWidth { get { return (int)(sourceSize & 0xFFFF); } }
		public int SourceHeight { get { return (int)(sourceSize >> 16); } }
		public int SourceX { get { return (int)(sourceOrigin & 0xFFFF); } }
		public int SourceY { get { return (int)(sourceOrigin >> 16); } }

		public byte Red { get { return (byte)(colorAndFlags >> 8); } }
		public byte Green { get { return (byte)(colorAndFlags >> 16); } }
		public byte Blue { get { return (byte)(colorAndFlags >> 24); } }

		public bool Cache { get { return (colorAndFlags & CacheMask) != 0; } }
		public bool DynamicColor { get { return (colorAndFlags & DynamicColorMask) != 0; } }
		public bool DynamicRectangle { get { return (colorAndFlags & DynamicRectangleMask) != 0; } }

		public TintEffectMode TintEffectMode
		{
			get
			{
				if((colorAndFlags & AdditiveBlendMask) != 0)
					return TintEffectMode.Additive;
				if((colorAndFlags & OpaqueBlendMask) != 0)
					return TintEffectMode.Opaque;
				else
					return TintEffectMode.Normal;
			}
		}

		#endregion


		#region Construction

		private static uint PackPoint(int x, int y)
		{
			return ((uint)x & 0xFFFF) | ((uint)y << 16);
		}


		private ImageSourceId(uint color, uint sourceOrigin, uint sourceSize)
		{
			this.colorAndFlags = color;
			this.sourceOrigin = sourceOrigin;
			this.sourceSize = sourceSize;
		}

		public ImageSourceId(byte r, byte g, byte b, Rectangle? sourceRectangle, TintEffectMode tintEffectMode,
				bool cache, bool dynamicColor, bool dynamicRectangle)
		{
			if(dynamicColor)
				r = g = b = 255; // Dynamic color sources are always white (although can still be additive or not)

			this.colorAndFlags = ((uint)r << 8) | ((uint)g << 16) | ((uint)b << 24)
					| FlagForTintEffectMode(tintEffectMode)
					| (cache ? CacheMask : 0)
					| (dynamicColor ? DynamicColorMask : 0)
					| (dynamicRectangle ? DynamicRectangleMask : 0);

			if(sourceRectangle.HasValue && !dynamicRectangle)
			{
				sourceOrigin = PackPoint(sourceRectangle.Value.X, sourceRectangle.Value.Y);
				sourceSize = PackPoint(sourceRectangle.Value.Width, sourceRectangle.Value.Height);
			}
			else
			{
				sourceOrigin = 0;
				sourceSize = 0;
			}
		}

		#endregion


		/// <summary>Texture sources do not have dynamic properties or a cache mode. Remove them before texture lookup.</summary>
		public ImageSourceId ForTextureLookup
		{
			get { return new ImageSourceId(colorAndFlags & NonDynamicMask, sourceOrigin, sourceSize); }
		}

		/// <summary>The texture lookup should return the original image.</summary>
		public bool TextureLookupOriginal
		{
			get { return sourceSize == 0 && (colorAndFlags & NonDynamicMask) == OpaqueWhiteMask; }
		}

		public bool UseOriginalDimentions { get { return sourceSize == 0; } }
		

		/// <summary>Does the surface source image need to be tinted?</summary>
		public bool WantsPreTint { get { return (colorAndFlags & NonDynamicMask) != OpaqueWhiteMask; } }

		public SWM.Color ColorForPreTint
		{
			get { return SWM.Color.FromArgb(255, Red, Green, Blue); }
		}


		


		#region Comparisons

		public override int GetHashCode()
		{
			return (int)(sourceOrigin ^ sourceSize ^ colorAndFlags);
		}

		public bool Equals(ImageSourceId other)
		{
			return (this.colorAndFlags == other.colorAndFlags
					&& this.sourceOrigin == other.sourceOrigin
					&& this.sourceSize == other.sourceSize);
		}

		public static bool operator==(ImageSourceId id1, ImageSourceId id2)
		{
			return id1.Equals(id2);
		}

		public static bool operator!=(ImageSourceId id1, ImageSourceId id2)
		{
			return !id1.Equals(id2);
		}

		public override bool Equals(object obj)
		{
			return (obj is ImageSourceId) && this.Equals((ImageSourceId)obj);
		}

		#endregion

	}
}
