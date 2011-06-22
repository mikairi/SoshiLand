using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ExEnCore;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{
	public partial class Game : Canvas, IDisposable
	{
		public Game()
		{
			TextOptions.SetTextHintingMode(this, TextHintingMode.Animated);

			this.Window = new GameWindow();
			this.Content = new ContentManager(services);

			// Wait until we're loaded and then do the startup sequence:
			this.Loaded += new RoutedEventHandler(StartGame);
		}

		void StartGame(object sender, RoutedEventArgs e)
		{
			if(graphicsDeviceManager == null)
				throw new InvalidOperationException("Game requires that a GraphicsDeviceManager is created in the constructor");
			graphicsDeviceManager.CreateDevice();

			Window.ClientBounds = new Rectangle(0, 0, (int)Width, (int)Height);

			Initialize();
			BeginRun();

			InitializeGameLoop();

			Input.Keyboard.SetEventHandlersOn(Application.Current.RootVisual as FrameworkElement);
			Input.Mouse.SetEventHandlersOn(Application.Current.RootVisual as FrameworkElement, this);
		}



		#region Game Loop

		GameLoop gameLoop;
		GameTime updateGameTime = new GameTime();
		GameTime drawGameTime = new GameTime();

		EventHandler renderingEvent;

		DateTime lastUpdate;

		void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			DateTime now = DateTime.Now;
			gameLoop.Tick(now - lastUpdate);
			lastUpdate = now;
		}

		void InitializeGameLoop()
		{
			if(gameLoop == null)
			{
				gameLoop = new GameLoop();
				gameLoop.Update = gameLoop_Update;
				gameLoop.Draw = gameLoop_Draw;
				gameLoop.ForcePump = ForceDrawPump;

				lastUpdate = DateTime.Now;
				renderingEvent = new EventHandler(CompositionTarget_Rendering);
				CompositionTarget.Rendering += renderingEvent;
			}
		}

		#endregion


		#region Game Loop Event Handlers

		void ForceDrawPump()
		{
			Visibility = Visibility.Visible; // Force redraw to always pump
		}

		void gameLoop_Draw(TimeSpan time)
		{
			drawGameTime.Update(time);
			DoDraw(drawGameTime);
		}

		void gameLoop_Update(TimeSpan time)
		{
			if(!playing)
				return;

			updateGameTime.Update(time);
			Update(updateGameTime);
		}

		#endregion


		#region Silverlight "Play" Function

		// Normally XNA has a "Run" function that you call to enter the main game execution
		// and it does not return until the execution is complete.
		//
		// ExEnSilver, on the other hand, will start up by itself.
		// The "Play" function is provided so you can hold off starting the Update loop
		// until the user is ready (for example: a "Play" button on the Silverlight control).

		bool playing = false;
		public void Play()
		{
			playing = true;
		}

		#endregion

	}
}
