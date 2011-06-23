using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ExEnSilver;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace SoshiLandSilverlight
{
	public class App : ExEnSilverApplication
	{
		protected override void SetupMainPage(MainPage mainPage)
		{
            FontSource fontSource = new FontSource(Application.GetResourceStream(new Uri("SoshiLandSilverLight;component/Content/nobile.ttf", UriKind.Relative)).Stream);
            FontFamily fontFamily = new FontFamily("Nobile");

            ContentManager.SilverlightFontTranslation("SpriteFont1", new SpriteFontTTF(fontSource, fontFamily, 16));

            var game = new Game1();
			mainPage.Children.Add(game);
			game.Play();
		}
	}
}
