using System;
using ExEnSilver;

namespace SoshiLandSilverlight
{
	public class App : ExEnSilverApplication
	{
		protected override void SetupMainPage(MainPage mainPage)
		{
            var game = new Game1();
			mainPage.Children.Add(game);
			game.Play();
		}
	}
}
