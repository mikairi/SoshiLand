using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoshiLandSilverlight
{
    public static class SoshiLandGameFunctions
    {
        public static void startNextPlayerTurn(List<Player> ListOfPlayers)
        {
            if (SoshilandGame.currentTurnsPlayers != null)
                Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + SoshilandGame.currentTurnsPlayers.getName + "\"'s " + " turn ends");

            int previousPlayersTurn = ListOfPlayers.IndexOf(SoshilandGame.currentTurnsPlayers);
            int nextPlayersTurn;

            // Checks if the player is at the end of the list
            if (previousPlayersTurn == ListOfPlayers.Count - 1)
                nextPlayersTurn = 0;
            else
                nextPlayersTurn = previousPlayersTurn + 1;

            SoshilandGame.currentTurnsPlayers = ListOfPlayers.ElementAt(nextPlayersTurn);

            Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + SoshilandGame.currentTurnsPlayers.getName + "\"'s " + " turn begins");

            // Set phase to Pre Roll Phase
            SoshilandGame.turnPhase = 0;
        }

        public static void MovePlayerToJail(Player p)
        {
            Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + p.getName + "\"" + " goes to jail!");
            // Set jail flag for player
            p.inJail = true;
            MovePlayer(p, 12);

            // Set phase to Post Roll Phase
            SoshilandGame.turnPhase = 2;
        }

        public static void MovePlayer(Player p, int position)
        {
            // Update the player's current position to the new position
            p.CurrentBoardPosition = position;
            Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + p.getName + "\"" + " moves to Tile \"" + SoshilandGame.Tiles[position].getName + "\"");
        }

        public static void MovePlayerDiceRoll(Player p, int roll)
        {
            int currentPosition = p.CurrentBoardPosition;
            int newPosition = currentPosition + roll;

            // If player passes or lands on Go
            if (newPosition > 47)
            {
                newPosition = Math.Abs(newPosition - 48);           // Get absolute value of the difference and move player to that new Tile
                p.BankPaysPlayer(200);                              // Pay player $200 for passing Go
            }
            // Move player to the new position
            SoshiLandGameFunctions.MovePlayer(p, newPosition);
        }

        public static void RollDice(Player p)
        {
            SoshilandGame.DoublesRolled = false;
            int dice1Int = SoshilandGame.die.Next(1, 6);
            int dice2Int = SoshilandGame.die.Next(1, 6);

            int total = dice1Int + dice2Int;

            SoshilandGame.currentDiceRoll = total;                // Set the global dice roll variable

            if (dice1Int == dice2Int && SoshilandGame.gameInitialized)
            {
                SoshilandGame.DoublesRolled = true;
                // Check if it's the third consecutive double roll
                if (SoshilandGame.numberOfDoubles == 2)
                    // Move player to jail
                    SoshiLandGameFunctions.MovePlayerToJail(p);
                else
                    // Increment number of doubles
                    SoshilandGame.numberOfDoubles++;
            }

            Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + p.getName + "\"" + " rolls dice: " + dice1Int + " and " + dice2Int + ". Total: " + total);
            if (SoshilandGame.DoublesRolled)
                Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + p.getName + "\"" + " rolled doubles!");

            // Only move if the player is not in jail
            if ((!p.inJail) && SoshilandGame.gameInitialized)
                SoshiLandGameFunctions.MovePlayerDiceRoll(p, total);
        }
    }
}
