using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using Microsoft.Xna.Framework;

namespace BlankGame
{
	[Register("AppDelegate")]
	class Program : ExEnEmTouchApplication
	{
		public override void FinishedLaunching(UIApplication application)
		{
			game = new BlankGameGame();
			game.Run();
		}

		static void Main(string[] args)
		{
			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}

