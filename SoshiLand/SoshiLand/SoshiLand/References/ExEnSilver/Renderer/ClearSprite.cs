using System;
using SWM=System.Windows.Media;
using SWS=System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExEnSilver.Renderer
{
	public class ClearSprite : Sprite
	{
		public ClearSprite(bool hardware, int width, int height)
		{
			// If hardware: use a 1-pixel rectangle rendered into a hardware surface and scaled full-screen
			// If software: use a full-screen rectangle

			rectangle = new SWS.Rectangle();
			rectangle.Width = hardware ? 1 : width;
			rectangle.Height = hardware ? 1 : height;
			rectangle.Fill = brush = new SWM.SolidColorBrush(SWM.Color.FromArgb(255, 128, 0, 128));

			base.Create(rectangle, hardware, hardware);

			if(hardware)
			{
				// Scaling matrix to take the one pixel rectangle to screen-size
				transform.Matrix = new SWM.Matrix(width, 0, 0, height, 0, 0);
			}
		}

		SWS.Rectangle rectangle;
		SWM.SolidColorBrush brush;

		Color lastColor;

		public void SetColor(Color color)
		{
			if(color != lastColor)
			{
				brush.Color = color.ToOpaqueSilverlightColor();
				lastColor = color;
			}
		}
		
	}
}
