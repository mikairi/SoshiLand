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
        public static GameState currentGameState = GameState.EnterUserName;
        public static MainPage mPage;

		protected override void SetupMainPage(MainPage mainPage)
		{
            mPage = mainPage;
            FontSource fontSource = new FontSource(Application.GetResourceStream(new Uri("SoshiLandSilverLight;component/Content/nobile.ttf", UriKind.Relative)).Stream);
            FontFamily fontFamily = new FontFamily("Nobile");

            ContentManager.SilverlightFontTranslation("SpriteFont1", new SpriteFontTTF(fontSource, fontFamily, 14));

            // Start the app by prompting User Name
            ChangeGameState(GameState.EnterUserName);
		}

        public static void ChangeGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.EnterUserName:
                    var startPage = new StartPage();
                    mPage.Children.Add(startPage);
                    break;
                case GameState.ChatRoom:
                    mPage.Children.Clear();     // Clear all Children

                    var chatRoom = new ChatRoom();  // Create new chatroom instance
                    mPage.Children.Add(chatRoom);   // Add chatroom as a new child
                    break;
                case GameState.InGame:
                    mPage.Children.Clear();     // Clear all Children

                    var game = new Game1(ChatRoom.chatroomListOfPlayers);     // Create a new instance of a SoshiLand game
			        mPage.Children.Add(game);   // Add the game as a child of the Silverlight App
			        game.Play();                // Starts the game logic
                    break;
            }

        }
	}
}
