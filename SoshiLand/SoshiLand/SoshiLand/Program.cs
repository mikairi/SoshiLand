using System;

namespace SoshiLand
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            string[] listOfPlayers = new string[7];

            listOfPlayers[0] = "Mark";
            listOfPlayers[1] = "Addy";
            listOfPlayers[2] = "Yook";
            listOfPlayers[3] = "Wooski";
            listOfPlayers[4] = "Skylar";
            listOfPlayers[5] = "Colby";
            listOfPlayers[6] = "Mako";

            using (Game1 game = new Game1(listOfPlayers))
            {
                game.Run();
            }
        }
    }
#endif
}

