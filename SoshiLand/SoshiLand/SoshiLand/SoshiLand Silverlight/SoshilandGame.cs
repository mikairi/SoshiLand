using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
// Required to read XML file
using System.Xml;


namespace SoshiLandSilverlight
{
    class SoshilandGame
    {
        public static List<Player> ListOfPlayers;                  // Contains the list of players in the game. This will be in the order from first to last player
        public static Player currentTurnsPlayers;             // Holds the Player of the current turn         
        public static Tile[] Tiles = new Tile[48];            // Array of Tiles

        private DeckOfCards ChanceCards = new DeckOfCards();          // Chance Cards Deck
        private DeckOfCards CommunityChestCards = new DeckOfCards();  // Community Chest Deck

        public static Random die = new Random();        // Need to create a static random die generator so it doesn't reuse the same seed over and over

        public static bool DoublesRolled;               // Flag to indicate doubles were rolled
        public static int currentDiceRoll;              // Global dice roll variable for special instances when we need to know (ie. determining player order)
        public static int numberOfDoubles;              // Keep track of the number of doubles rolled

        public static int Houses = 32;                  // Static Global variable for number of houses remaining
        public static int Hotels = 12;                  // Static Global variable for number of hotels remaining

        public static bool gameInitialized = false;     // Flag for when the game is officially started
        private bool optionsCalculated = false;         // Flag for when player options are ready to prompt

        private bool displayJailMessageOnce = true;     // Flag to display message only once

        // Player Options during turn
        private bool optionPurchaseOrAuctionProperty = false;
        private bool optionPurchaseOrAuctionUtility = false;
        private bool optionDevelopProperty = false;
        private bool optionPromptMortgageOrTrade = false;
        private bool optionPromptLuxuryTax = false;
        private bool optionShoppingSpree = false;

        private bool taxesMustPayTenPercent = false;
        private bool taxesMustPayTwoHundred = false;
        // Phase Flags
        
        // 0 = Pre Roll Phase
        // Player has option to trade, develop or mortgage / unmortgage.
        // If player is in jail, player has option to Pay to get out of jail, or roll doubles
        // Phase ends after player chooses to roll dice
        
        // 1 = Roll Phase
        // Player has landed on a Tile.
        // If tile is a property, Player is forced to purchase or auction
        // If tile is Chance / Community Chest, Player is forced to follow card instructions immediately
        // If tile is Taxes, Player is forced to pay immediately (or choose option between 10% or $200 for luxury tax)
        // If tile is Jail, move piece to jail

        // 2 = Post Roll Phase
        // Player has option to trade, develop or mortgage / unmortgage.
        // Phase ends after playing chooses to end his or her turn

        public static byte turnPhase = 0;                 

        private KeyboardState previousKeyboardInput;    

        // TEMPORARY
        Player[] playerArray;

        public SoshilandGame()
        {
            Initialization gameInitialization = new Initialization();

            gameInitialization.InitializeTiles(Tiles);      // Initialize Tiles on the board
            gameInitialization.InitializeCards(ChanceCards, CommunityChestCards);   // Initialize Chance and Community Chest cards

            // Temporary list of players
            Player player1 = new Player("Mark");
            Player player2 = new Player("Wooski");
            Player player3 = new Player("Yook");
            Player player4 = new Player("Addy");
            Player player5 = new Player("Colby");
            Player player6 = new Player("Skylar");
            Player player7 = new Player("Mako");

            playerArray = new Player[7];
            playerArray[0] = player1;
            playerArray[1] = player2;
            playerArray[2] = player3;
            playerArray[3] = player4;
            playerArray[4] = player5;
            playerArray[5] = player6;
            playerArray[6] = player7;

            gameInitialization.DeterminePlayerOrder(playerArray, ref ListOfPlayers);        // Determine order of players
            // Players choose pieces (this can be implemented later)


            gameInitialization.DistributeStartingMoney(ListOfPlayers);                      // Players are given starting money
            gameInitialization.PlaceAllPiecesOnGo(ListOfPlayers);                           // Place all Pieces on Go
            SoshiLandGameFunctions.startNextPlayerTurn(ListOfPlayers);                      // Start first player's turn
        }

        private void PlayerOptions(Player player)
        {
            int currentTile = player.CurrentBoardPosition;
            TileType currentTileType = Tiles[currentTile].getTileType;

            optionPurchaseOrAuctionProperty = false;
            optionDevelopProperty = false;

            // Determine Player Options and take any actions required
            switch (currentTileType)
            {
                case TileType.Property:
                    PropertyTile currentProperty = (PropertyTile)Tiles[currentTile];

                    if (currentProperty.Owner == null)                  // If the property is not owned yet
                        optionPurchaseOrAuctionProperty = true;
                    else if (currentProperty.Owner != player && !currentProperty.MortgageStatus)    // If the property is owned by another player and not mortgaged
                    {
                        if (player.getMoney >= currentProperty.getRent) // Check if the player has enough money to pay Rent
                        {
                            player.CurrentPlayerPaysPlayer(currentProperty.Owner, currentProperty.getRent);     // Pay rent
                            turnPhase = 2;          // Go to next phase
                        }
                        else
                            optionPromptMortgageOrTrade = true;         // Player must decide to mortgage or trade to get money
                    }

                    // Otherwise, player landed on his or her own property, so do nothing
                    break;

                case TileType.Utility:
                    UtilityTile currentUtility = (UtilityTile)Tiles[currentTile];
                    UtilityTile otherUtility;

                    if (currentTile == 15)
                        otherUtility = (UtilityTile)Tiles[33];
                    else
                        otherUtility = (UtilityTile)Tiles[15];

                    if (currentUtility.Owner == null)               // If the property is not owned yet
                        optionPurchaseOrAuctionUtility = true;

                    else if (currentUtility.Owner != player && !currentUtility.MortgageStatus)        // If the property is owned by another player            
                    {
                        int utilityRent;                           // Calculate the amount to pay for Utility Rent

                        if (currentUtility.Owner == otherUtility.Owner)     // Check if player owns both utilities
                            utilityRent = (int)currentDiceRoll * 10;
                        else
                            utilityRent = (int)currentDiceRoll * 4;


                        if (player.getMoney >= utilityRent)                 // Check if the player has enough money to pay Rent
                        {
                            player.CurrentPlayerPaysPlayer(currentUtility.Owner, utilityRent);  // Pay rent
                            turnPhase = 2;              // Go to next phase
                        }
                        else
                            optionPromptMortgageOrTrade = true;             // Player must decide to mortgage or trade to get money
                    }
                    break;

                case TileType.Chance:
                    Card drawnChanceCard = ChanceCards.drawCard();                              // Draw the Chance card
                    SoshiLandGameFunctions.FollowCardInstructions(drawnChanceCard, player, ListOfPlayers);
                    turnPhase = 2;
                    break;
                case TileType.CommunityChest:
                    Card drawnCommunityChestCard = CommunityChestCards.drawCard();              // Draw the Community Chest card
                    SoshiLandGameFunctions.FollowCardInstructions(drawnCommunityChestCard, player, ListOfPlayers);
                    turnPhase = 2;
                    break;
                case TileType.FanMeeting:
                    turnPhase = 2;              // Nothing happens, so go to last phase
                    break;
                case TileType.Jail:
                    turnPhase = 2;              // Nothing happens, so go to last phase
                    break;
                case TileType.ShoppingSpree:
                        currentTurnsPlayers.PlayerPaysBank(75); // Pay Bank taxes
                        turnPhase = 2;
                    break;
                case TileType.SpecialLuxuryTax:
                        Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + currentTurnsPlayers.getName + "\"" + " must choose to pay 10% of net worth, or $200");
                        Game1.debugMessageQueue.addMessageToQueue("Press K to pay 10% of net worth, or L to pay $200");
                        optionPromptLuxuryTax = true;
                    break;
                case TileType.GoToJail:
                    SoshiLandGameFunctions.MovePlayerToJail(player);
                    break;
                case TileType.Go:
                    turnPhase = 2;
                    break;
            }

            optionsCalculated = true;

            if (Game1.DEBUG)
            {
                string optionsMessage = "Options Available: Trade,";
                if (optionDevelopProperty)
                    optionsMessage = optionsMessage + " Develop,";
                if (optionPurchaseOrAuctionProperty || optionPurchaseOrAuctionUtility)
                    optionsMessage = optionsMessage + " Purchase/Auction";

                Game1.debugMessageQueue.addMessageToQueue(optionsMessage);
            }
        }
        
        public void PlayerInputUpdate()
        {
            KeyboardState kbInput = Keyboard.GetState();

            switch (turnPhase)
            {
                // Pre Roll Phase
                case 0:
                    // Check if player is in jail
                    if (currentTurnsPlayers.inJail)
                    {
                        if (Game1.DEBUG && displayJailMessageOnce)
                        {
                            Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + currentTurnsPlayers.getName + "\"" + " is currently in jail");
                            Game1.debugMessageQueue.addMessageToQueue("Press T to pay $50 to get out of jail, or R to try and roll doubles");
                            displayJailMessageOnce = false;
                        }

                        // Player decides to roll for doubles
                        if (kbInput.IsKeyDown(Keys.R) && previousKeyboardInput.IsKeyUp(Keys.R))
                        {
                            // Roll Dice
                            SoshiLandGameFunctions.RollDice(currentTurnsPlayers);

                            // Only move if doubles were rolled or if player has been in jail for the third turn
                            if (DoublesRolled || currentTurnsPlayers.turnsInJail == 2)
                            {
                                if (currentTurnsPlayers.turnsInJail == 2)
                                {
                                    Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + currentTurnsPlayers.getName + "\"" + " must pay $50 to get out of jail on third turn.");
                                    currentTurnsPlayers.PlayerPaysBank(50);             // Pay bank fine
                                }

                                currentTurnsPlayers.inJail = false;                 // Set player out of jail
                                Game1.debugMessageQueue.addMessageToQueue("Player is no longer in jail!");
                                currentTurnsPlayers.turnsInJail = 0;                // Set turns in jail back to zero

                                SoshiLandGameFunctions.MovePlayerDiceRoll(currentTurnsPlayers, currentDiceRoll);   // Move player piece
                                PlayerOptions(currentTurnsPlayers);                         // Calculate options for player


                                DoublesRolled = false;  // Turn off doubles rolled flag because player is not supposed to take another turn after getting out of jail

                                turnPhase = 1;          // Set the next phase
                                break;
                            }
                            else
                            {
                                Game1.debugMessageQueue.addMessageToQueue("You failed to roll doubles and stay in jail.");

                                currentTurnsPlayers.turnsInJail++;
                                turnPhase = 2;
                                break;
                            }
                        }

                        // If player chooses to pay to get out of jail
                        if (kbInput.IsKeyDown(Keys.T) && previousKeyboardInput.IsKeyUp(Keys.T))
                        {
                            Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + currentTurnsPlayers.getName + "\"" + " pays $50 to escape from Babysitting Kyungsan");

                            currentTurnsPlayers.PlayerPaysBank(50);     // Pay bank fine
                            currentTurnsPlayers.turnsInJail = 0;        // Set turns in jail back to zero
                            currentTurnsPlayers.inJail = false;         // Set player to be out of Jail
                            Game1.debugMessageQueue.addMessageToQueue("Player is no longer in jail!");

                            SoshiLandGameFunctions.RollDice(currentTurnsPlayers);              // Rolls Dice and Move Piece to Tile
                            turnPhase = 1;                              // Set next phase
                            PlayerOptions(currentTurnsPlayers);         // Calculate options for player

                            break;
                        }

                    }
                    else
                    {
                        // Roll Dice
                        if (kbInput.IsKeyDown(Keys.R) && previousKeyboardInput.IsKeyUp(Keys.R))
                        {
                            SoshiLandGameFunctions.RollDice(currentTurnsPlayers);              // Rolls Dice and Move Piece to Tile
                            turnPhase = 1;                              // Set next phase
                            PlayerOptions(currentTurnsPlayers);         // Calculate options for player
                            
                        }
                    }
                    break;

                    // Roll Phase
                case 1:
                    if (optionsCalculated)
                    {
                        // Player chooses to purchase property
                        if (kbInput.IsKeyDown(Keys.P) && previousKeyboardInput.IsKeyUp(Keys.P))
                        {
                            bool successfulPurchase = false;
                            // Purchase Property
                            if (optionPurchaseOrAuctionProperty)
                                successfulPurchase = currentTurnsPlayers.PurchaseProperty((PropertyTile)Tiles[currentTurnsPlayers.CurrentBoardPosition]);
                            // Purchase Utility
                            else if (optionPurchaseOrAuctionUtility)
                                successfulPurchase = currentTurnsPlayers.PurchaseUtility((UtilityTile)Tiles[currentTurnsPlayers.CurrentBoardPosition]);
                            // Player cannot purchase right now
                            else
                                Game1.debugMessageQueue.addMessageToQueue(
                                    "Player " + "\"" + currentTurnsPlayers.getName + "\"" + " cannot purchase \"" + Tiles[currentTurnsPlayers.CurrentBoardPosition].getName + "\"");
                            
                            // Turn off option to purchase if successful purchase has been made
                            if (successfulPurchase)
                            {
                                // Set flags for purchase/auction off
                                optionPurchaseOrAuctionUtility = false;
                                optionPurchaseOrAuctionProperty = false;
                                // Set the next phase
                                turnPhase = 2;
                            }
                        }

                        // Player chooses to Auction

                        if (optionPromptLuxuryTax)
                        {
                            bool successfulTaxPayment = false;
                            // Player chooses to pay 10% (Luxury Tax)
                            if (kbInput.IsKeyDown(Keys.K) && previousKeyboardInput.IsKeyUp(Keys.K) && !taxesMustPayTwoHundred)
                            {
                                successfulTaxPayment = SoshiLandGameFunctions.PayTenPercentWorthToBank(currentTurnsPlayers);       // Pay 10% to bank
                                if (successfulTaxPayment)
                                {
                                    turnPhase = 2;
                                    optionPromptLuxuryTax = false;                                          // Turn off the tax flag
                                }
                                else
                                {
                                    taxesMustPayTenPercent = true;              // Turn flag for paying 10%
                                    optionPromptMortgageOrTrade = true;         // Player is forced to mortgage
                                }
                            }
                            // Player chooses to pay $200 (Luxury Tax)
                            else if (kbInput.IsKeyDown(Keys.L) && previousKeyboardInput.IsKeyUp(Keys.L) && !taxesMustPayTenPercent)
                            {
                                if (currentTurnsPlayers.getMoney >= 200)            // Check if player has enough money
                                {
                                    currentTurnsPlayers.PlayerPaysBank(200);        // Pay $200 to bank
                                    optionPromptLuxuryTax = false;                  // Turn off the tax flag
                                    turnPhase = 2;                                  // Go to next phase
                                }
                                else
                                {
                                    taxesMustPayTwoHundred = true;                  // Turn flag on for paying $200
                                    optionPromptMortgageOrTrade = true;             // Player is forced to mortgage
                                }
                            }
                        }

                        // Player chooses to mortgage

                        // Player chooses to trade
                    }
                    break;
                    // Post Roll Phase

                case 2:
                    // Player chooses to end turn
                    if (kbInput.IsKeyDown(Keys.E) && previousKeyboardInput.IsKeyUp(Keys.E))
                    {
                        // Check if doubles has been rolled
                        if (DoublesRolled && !currentTurnsPlayers.inJail)
                        {
                            // Go back to phase 0 for current player
                            turnPhase = 0;
                            Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + currentTurnsPlayers.getName + "\"" + " gets to roll again!");
                        }
                        else
                        {
                            // Start next player's turn
                            SoshiLandGameFunctions.startNextPlayerTurn(ListOfPlayers);
                            // Set phase back to 0 for next player
                            turnPhase = 0;
                            optionsCalculated = false;
                            taxesMustPayTenPercent = false;
                            taxesMustPayTwoHundred = false;
                            displayJailMessageOnce = true;
                            // set number of doubles back to zero
                            numberOfDoubles = 0;
                        }
                    }
                    break;
            }

            previousKeyboardInput = kbInput;
        }

        
    }
}
