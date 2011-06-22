using System;
using SWI=System.Windows.Input;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.Xna.Framework.Input
{
	public static class Mouse
	{
		#region Silverlight Mouse State Tracking

		static MouseState currentState;


		// Note from the docs:
		// Panel elements do not receive mouse or stylus events if a Background is not defined.
		// If you need to handle mouse or stylus events but do not want a background for your Panel, use Transparent. 

		internal static void SetEventHandlersOn(FrameworkElement element, Game game)
		{
			element.MouseLeftButtonDown += (o, e) =>
			{
				currentState.LeftButton = ButtonState.Pressed;
				if(o is FrameworkElement)
					(o as FrameworkElement).CaptureMouse();
			};

			element.MouseLeftButtonUp += (o, e) =>
			{
				currentState.LeftButton = ButtonState.Released;
				if(o is FrameworkElement)
					(o as FrameworkElement).ReleaseMouseCapture();
			};

			element.MouseMove += (o, e) =>
			{
				var p = e.GetPosition(game as UIElement);
				currentState.X = (int)p.X;
				currentState.Y = (int)p.Y;
			};

			element.MouseWheel += (o, e) =>
			{
				// TODO: this behaviour isn't confirmed...
				currentState.ScrollWheelValue += e.Delta;
			};
		}

		#endregion


		#region XNA API

		public static MouseState GetState()
		{
			return currentState;
		}

		#endregion
	}
}
