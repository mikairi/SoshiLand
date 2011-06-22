using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
	public partial class GraphicsDeviceManager : IGraphicsDeviceService, IDisposable, IGraphicsDeviceManager
	{
		public static readonly int DefaultBackBufferWidth = 800;
		public static readonly int DefaultBackBufferHeight = 600;


		public GraphicsDevice GraphicsDevice { get; private set; }


		#region Device Setup

		public void CreateDevice()
		{
			GraphicsDevice = new GraphicsDevice(game);
			OnDeviceCreated(this, EventArgs.Empty);
			ApplyChanges();
		}

		public void ApplyChanges()
		{
			GraphicsDevice.Viewport = new Viewport()
			{
				X = 0,
				Y = 0,
				Width = PreferredBackBufferWidth,
				Height = PreferredBackBufferHeight,
			};
			GraphicsDevice.Root.Width = PreferredBackBufferWidth;
			GraphicsDevice.Root.Height = PreferredBackBufferHeight;
			GraphicsDevice.UpdatePresentationParameters();
		}

		#endregion


		#region Drawing

		public bool BeginDraw()
		{
			return true;
		}

		public void EndDraw()
		{
			GraphicsDevice.EndSpriteFrame();
		}

		#endregion

	}
}
