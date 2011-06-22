using System;
using ExEnSilver;

namespace SoshiLandSilverlight
{
	public class App : ExEnSilverApplication
	{
		protected override void SetupMainPage(MainPage mainPage)
		{
            var game = new SoshiLandSilverlightGame();
			mainPage.Children.Add(game);
			game.Play();
		}
	}
}
