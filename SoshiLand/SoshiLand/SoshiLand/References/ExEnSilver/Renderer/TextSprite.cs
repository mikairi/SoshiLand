using System;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Controls;
using SWM=System.Windows.Media;
using Microsoft.Xna.Framework;

namespace ExEnSilver.Renderer
{
	public class TextSprite : Sprite
	{
		TextBlock textBlock;
		SWM.SolidColorBrush brush;

		public TextSprite(SpriteFontTTF font, bool wantsCache)
		{
			textBlock = new TextBlock();
			textBlock.FontSource = font.FontSource;
			textBlock.FontFamily = font.FontFamily;
			textBlock.FontSize = font.FontSize;
			textBlock.Foreground = brush = new SWM.SolidColorBrush(SWM.Color.FromArgb(255, 0, 0, 0));

			base.Create(textBlock, wantsCache, true); // TODO: optional cache
		}


		public Vector2 NaturalSize { get; private set; }

		internal void SetText(string text)
		{
			textBlock.Text = text;
			NaturalSize = new Vector2((float)textBlock.ActualWidth, (float)textBlock.ActualHeight);
		}


		Color actualColor = Color.Black;
		internal void SetColor(Color color)
		{
			if(color.A != actualColor.A)
				textBlock.Opacity = color.A / 255d;
			if((color.PackedValue & Color.OpaqueMask) != (actualColor.PackedValue & Color.OpaqueMask))
				brush.Color = color.ToOpaqueSilverlightColor();
			actualColor = color;
		}

	}
}
