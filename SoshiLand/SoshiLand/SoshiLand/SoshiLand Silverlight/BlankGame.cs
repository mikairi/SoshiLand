using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SoshiLandSilverlight
{
	public class SoshiLandSilverlightGame : Game
	{
		protected GraphicsDeviceManager graphics;

        public SoshiLandSilverlightGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 320;
			graphics.PreferredBackBufferHeight = 480;

			IsMouseVisible = true;
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			base.Draw(gameTime);
		}

	}
}
