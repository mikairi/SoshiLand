using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
// Required to read XML file
using System.Xml;


namespace SoshiLand
{
    class SoshilandGame
    {
        private List<Player> ListOfPlayers;             // Contains the list of players in the game. This will be in the order from first to last player
        private Player currentTurnsPlayers;             // Holds the Player of the current turn         
        private Tile[] Tiles = new Tile[48];            // Array of Tiles

        private static Random die = new Random();       // Need to create a static random die generator so it doesn't reuse the same seed over and over

        private bool DoublesRolled;                     // Flag to indicate doubles were rolled
        private int currentDiceRoll;                    // Global dice roll variable for special instances when we need to know (ie. determining player order)
        private int numberOfDoubles;                    // Keep track of the number of doubles rolled

        public static int Houses = 32;                  // Static Global variable for number of houses remaining
        public static int Hotels = 12;                  // Static Global variable for number of hotels remaining

        private bool gameInitialized = false;           // Flag for when the game is officially started
        private bool optionsCalculated = false;         // Flag for when player options are ready to prompt

        private bool displayJailMessageOnce = true;    // Flag to display message only once

        // Player Options during turn
        private bool optionPurchaseOrAuctionProperty = false;
        private bool optionPurchaseOrAuctionUtility = false;
        private bool optionDevelopProperty = false;

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

        private byte turnPhase = 0;

        private KeyboardState previousKeyboardInput;

        // TEMPORARY
        Player[] playerArray;

        public SoshilandGame()
        {
            InitializeTiles();                          // Initialize Tiles on the board
            InitializeGame();                           // Initialize Game
        }

        private void InitializeGame()
        {
            // Temporary list of players
            Player player1 = new Player("Mark");
            Player player2 = new Player("Wooski");
            Player player3 = new Player("Yook");
            Player player4 = new Player("Addy");
            Player player5 = new Player("Colby");
            Player player6 = new Player("Skylar");

            playerArray = new Player[6];
            playerArray[0] = player1;
            playerArray[1] = player2;
            playerArray[2] = player3;
            playerArray[3] = player4;
            playerArray[4] = player5;
            playerArray[5] = player6;
            // Determine order of players
            DeterminePlayerOrder(playerArray);
            // Players choose pieces (this can be implemented later)

            // Players are given starting money
            DistributeStartingMoney();
            // Place all Pieces on Go
            PlaceAllPiecesOnGo();
            startNextPlayerTurn();
        }

        public void startNextPlayerTurn()
        {
            if (Game1.DEBUG)
            {
                if (currentTurnsPlayers != null)
                {
                    Console.WriteLine("Player " + "\"" + currentTurnsPlayers.getName + "\"'s " + " turn ends");
                }
            }

            int previousPlayersTurn = ListOfPlayers.IndexOf(currentTurnsPlayers);
            int nextPlayersTurn;
            // Checks if the player is at the end of the list
            if (previousPlayersTurn == ListOfPlayers.Count - 1)
                nextPlayersTurn = 0;
            else
                nextPlayersTurn = previousPlayersTurn + 1;

            PlayerTurn(ListOfPlayers.ElementAt(nextPlayersTurn));
        }

        private void PlayerTurn(Player player)
        {
            currentTurnsPlayers = player;

            if (Game1.DEBUG)
            {
                Console.WriteLine("Player " + "\"" + currentTurnsPlayers.getName + "\"'s " + " turn begins");
            }

            // Set phase to Pre Roll Phase
            turnPhase = 0;

            // Check if player is currently in Jail

            // Determine what Tile was landed on and give options


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
                    // If the property is not owned yet
                    if (currentProperty.Owner == null)
                        optionPurchaseOrAuctionProperty = true;
                    // If the property is owned by another player
                    else if (currentProperty.Owner != player)
                    {
                        // Check if the player has enough money to pay Rent
                        if (player.getMoney >= currentProperty.getRent)
                            // Pay rent
                            player.CurrentPlayerPaysPlayer(currentProperty.Owner, currentProperty.getRent);
                        else
                            // Player must decide to mortgage or trade to get money
                            // Put this in later
                            ;
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

                    // If the property is not owned yet
                    if (currentUtility.Owner == null)
                        optionPurchaseOrAuctionUtility = true;
                    // If the property is owned by another player
                    else if (currentUtility.Owner != player)
                    {
                        // Calculate the amount to pay for Utility Rent
                        uint utilityRent;
                        // Check if player owns both utilities
                        if (currentUtility.Owner == otherUtility.Owner)
                            utilityRent = (uint)currentDiceRoll * 10;
                        else
                            utilityRent = (uint)currentDiceRoll * 4;

                        // Check if the player has enough money to pay Rent
                        if (player.getMoney >= utilityRent)
                            // Pay rent
                            player.CurrentPlayerPaysPlayer(currentUtility.Owner, utilityRent);
                        else
                            // Player must decide to mortgage or trade to get money
                            // Put this in later
                            ;
                    }
                    break;

                case TileType.Chance:
                    break;
                case TileType.CommunityChest:
                    break;
                case TileType.FanMeeting:
                    break;
                case TileType.Jail:
                    break;
                case TileType.ShoppingSpree:
                    break;
                case TileType.SpecialLuxuryTax:
                    break;
                case TileType.GoToJail:
                    MovePlayerToJail(player);
                    break;
                case TileType.Go:
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

                Console.WriteLine(optionsMessage);
            }
        }

        private void DistributeStartingMoney()
        {
            if (Game1.DEBUG)
            {
                Console.WriteLine("Distributing Starting Money");
            }

            foreach (Player p in ListOfPlayers)
            {
                // Starting money is $1500
                p.BankPaysPlayer(1500);
            }
        }

        private void PlaceAllPiecesOnGo()
        {
            if (Game1.DEBUG)
            {
                Console.WriteLine("Placing all players on Go");
            }
            foreach (Player p in ListOfPlayers)
            {
                // Move player to Go
                MovePlayer(p, 0);
            }
            gameInitialized = true;
        }

        private void DeterminePlayerOrder(Player[] arrayOfPlayers)
        {
            // Note!
            // arrayOfPlayers is the order the players are sitting in around the board.
            // So the order is determined by starting at the player with the highest roll 
            // and moving clockwise around the board

            if (Game1.DEBUG)
            {
                Console.WriteLine("Players rolling to determine Order");
            }

            int[] playerRolls = new int[arrayOfPlayers.Length];     // An array the size of the number of players to hold their dice rolls
            List<Player> tiedPlayers = new List<Player>();          // List of players that are tied for highest roll

            int currentHighestPlayer = 0;                           // Current player index in arrayOfPlayers with the highest roll

            // Have each player roll a pair of dice and store the result in the playerRolls array
            for (int i = 0; i < arrayOfPlayers.Length; i++)
            {
                RollDice(arrayOfPlayers[i]);
                playerRolls[i] = currentDiceRoll;

                // If the current highest player's roll is less than the new player's roll
                // Replace that player with the new player with the highest roll
                if (playerRolls[currentHighestPlayer] < playerRolls[i] && i != currentHighestPlayer)
                {
                    // Set the new Highest Player roll
                    currentHighestPlayer = i;
                    // Clear the list of tied players
                    tiedPlayers.Clear();
                }
                else if (playerRolls[currentHighestPlayer] == playerRolls[i] && i != currentHighestPlayer)
                {
                    // Only add the current highest player if the list is empty
                    // That player would've already been added to the list
                    if (tiedPlayers.Count == 0)
                        tiedPlayers.Add(arrayOfPlayers[currentHighestPlayer]);
                    // Add the new player to the list of tied players
                    tiedPlayers.Add(arrayOfPlayers[i]);
                }

                if (Game1.DEBUG)
                {
                    Console.WriteLine("Player " + "\"" + arrayOfPlayers[currentHighestPlayer].getName + "\"" + " is the current highest roller with: " + playerRolls[currentHighestPlayer]);
                }
            }

            // Initialize the list of players
            ListOfPlayers = new List<Player>();

            // Check if there is a tie with highest rolls
            if (tiedPlayers.Count > 0)
            {
                if (Game1.DEBUG)
                {
                    Console.WriteLine("There's a tie!");
                }
                // New list to store second round of tied players
                List<Player> secondRoundOfTied = new List<Player>();
                // Keep rolling until no more tied players
                while (secondRoundOfTied.Count != 1)
                {
                    int currentHighestRoll = 0;

                    // Roll the dice for each player
                    foreach (Player p in tiedPlayers)
                    {

                        RollDice(p);                                                    // Roll the dice for the player
                        // If the new roll is higher than the current highest roll
                        if (currentDiceRoll > currentHighestRoll)
                        {
                            // Clear the list since everyone who may have been in the list is lower 
                            secondRoundOfTied.Clear();

                            // Set the new highest roll
                            currentHighestRoll = currentDiceRoll;
                            secondRoundOfTied.Add(p);
                        }
                        // If there's another tie, just add it to the new array without clearing it
                        else if (currentDiceRoll == currentHighestRoll)
                        {
                            secondRoundOfTied.Add(p);
                        }
                        // Otherwise, the player rolled less and is removed
                    }

                    // If there are still tied players, transfer them into the old List and clear the new List
                    if (secondRoundOfTied.Count > 1)
                    {
                        // Clear the players that did not roll high enough
                        tiedPlayers.Clear();
                        foreach (Player p in secondRoundOfTied)
                        {
                            tiedPlayers.Add(p);
                        }
                        secondRoundOfTied.Clear();
                    }
                }

                // Should be one clear winner now
                ListOfPlayers.Add(secondRoundOfTied[0]);
            }

            if (ListOfPlayers.Count == 0)
                ListOfPlayers.Add(arrayOfPlayers[currentHighestPlayer]);

            int firstPlayer = 0;
            // Search for the first player in the player array
            while (arrayOfPlayers[firstPlayer] != ListOfPlayers[0])
                firstPlayer++;

            // Populate the players in clockwise order
            for (int a = firstPlayer + 1; a < arrayOfPlayers.Length; a++)
                ListOfPlayers.Add(arrayOfPlayers[a]);
            if (firstPlayer != 0)
            {
                for (int b = 0; b < firstPlayer; b++)
                    ListOfPlayers.Add(arrayOfPlayers[b]);
            }


            if (Game1.DEBUG)
            {
                Console.WriteLine("Player Order Determined! ");
                for (int i = 1; i < ListOfPlayers.Count + 1; i++)
                {
                    Console.WriteLine(i + ": " + ListOfPlayers[i - 1].getName);
                }

            }
        }

        private Color getColorFromNumber(int c)
        {
            // These colors can be specified by RGBA values later.
            // For now, I put in the standard Colors from the Color class.
            switch (c)
            {
                case 1:
                    return Color.Orange;
                case 2:
                    return Color.Blue;
                case 3:
                    return Color.DarkBlue;
                case 4:
                    return Color.Purple;
                case 5:
                    return Color.Green;
                case 6:
                    return Color.Red;
                case 7:
                    return Color.LightBlue;
                case 8:
                    return Color.Yellow;
                case 9:
                    return Color.Black;
            }

            // Invalid color type
            Console.WriteLine("Warning! Could not find color that matches code: " + c);
            return Color.White;
        }

        private void RollDice(Player p)
        {
            DoublesRolled = false;
            int dice1Int = die.Next(1, 6);
            int dice2Int = die.Next(1, 6);

            int total = dice1Int + dice2Int;

            currentDiceRoll = total;                // Set the global dice roll variable

            if (dice1Int == dice2Int && gameInitialized)
            {
                DoublesRolled = true;
                // Check if it's the third consecutive double roll
                if (numberOfDoubles == 3)
                    // Move player to jail
                    MovePlayerToJail(p);
                else
                    // Increment number of doubles
                    numberOfDoubles++;
            }

            if (Game1.DEBUG)
            {
                Console.WriteLine("Player " + "\"" + p.getName + "\"" + " rolls dice: " + dice1Int + " and " + dice2Int + ". Total: " + total);
                if (DoublesRolled)
                {
                    Console.WriteLine("Player " + "\"" + p.getName + "\"" + " rolled doubles!");
                }
            }

            // Only move if the player is not in jail, or if doubles were rolled (getting the player out of jail)
            if ((!p.inJail || DoublesRolled) && gameInitialized)
                MovePlayerDiceRoll(p, total);
        }

        private void MovePlayerDiceRoll(Player p, int roll)
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
            MovePlayer(p, newPosition);
        }

        private void MovePlayer(Player p, int position)
        {
            // Update the player's current position to the new position
            p.CurrentBoardPosition = position;

            if (Game1.DEBUG)
            {
                Console.WriteLine("Player " + "\"" + p.getName + "\"" + " moves to Tile \"" + Tiles[position].getName + "\"");
            }
        }

        private void MovePlayerToJail(Player p)
        {
            if (Game1.DEBUG)
            {
                Console.WriteLine("Player " + "\"" + p.getName + "\"" + " goes to jail!");
            }
            // Set jail flag for player
            p.inJail = true;
            MovePlayer(p, 12);

            // Set phase to Post Roll Phase
            turnPhase = 2;
        }

        private void InitializeTiles()
        {
            // This probably isn't the most efficient way of creating the Tiles,
            // But it'll only be run once at the start of a game.

            // XML Reading Variables
            XmlReader xmlReader;
            xmlReader = XmlReader.Create("PropertyCards.xml");      // Set the XML file to read

            // First, reserve spots in array for non-property Tiles
            Tiles[0] = new Tile("Go", TileType.Go);
            Tiles[5] = new Tile("Special Luxury", TileType.SpecialLuxuryTax);
            Tiles[8] = new Tile("Chance", TileType.Chance);
            Tiles[12] = new Tile("Hello Baby", TileType.Jail);
            Tiles[15] = new UtilityTile("Soshi Bond");
            Tiles[20] = new Tile("Community Chest", TileType.CommunityChest);
            Tiles[24] = new Tile("Fan Meeting", TileType.FanMeeting);
            Tiles[27] = new Tile("Chance", TileType.Chance);
            Tiles[33] = new UtilityTile("Forever 9");
            Tiles[36] = new Tile("Babysit Kyung San", TileType.GoToJail);
            Tiles[40] = new Tile("Community Chest", TileType.CommunityChest);
            Tiles[45] = new Tile("Shopping Spree", TileType.ShoppingSpree);


            // Fill in the gaps with Colored Property Tiles

            int counter = 0;                       // Keep track of current location in array
            Color currentColor = Color.White;      // Keep track of current Color in XML
            string currentTileName = "";           // Keep track of current Tile Name

            string currentMember = "";             // Name of SNSD member who owns the Property
            uint currentBaseRent = 0;              // Rent
            uint currentHouse1Rent = 0;            // Rent with 1 House
            uint currentHouse2Rent = 0;            // Rent with 2 Houses
            uint currentHouse3Rent = 0;            // Rent with 3 Houses
            uint currentHouse4Rent = 0;            // Rent with 4 Houses
            uint currentHotelRent = 0;             // Rent with Hotel
            uint currentMortgageValue = 0;         // Mortgage Value
            uint currentHouseCost = 0;             // Cost for each house
            uint currentHotelCost = 0;             // Cost for Hotel (+ 4 houses)
            uint currentPropertyPrice = 0;         // Cost to initially purchase property

            int propertyCardInfoCounter = 0;        // This is a counter to ensure that the current property card has read all the required data

            // Read in XML data for Properties
            while (xmlReader.Read())
            {
                XmlNodeType nodeType = xmlReader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        // If the current element is a Color
                        case "Color":
                            // Checks if there is only one attribute
                            // THIS MUST BE TRUE! Otherwise the XML structure is wrong
                            if (xmlReader.AttributeCount == 1)
                            {
                                try
                                {
                                    currentColor = getColorFromNumber(Convert.ToInt16(xmlReader.GetAttribute(0)));
                                }
                                catch (Exception e)
                                {
                                    Console.Error.WriteLine("Warning: Invalid color value in XML file. " + " ERROR: " + e.Message);
                                }
                            }
                            else
                                Console.Error.WriteLine("Warning: Color in XML file is missing attribute value");
                            break;
                        // If the current element is a Tile
                        case "Tile":
                            if (xmlReader.AttributeCount == 1)
                            {
                                try
                                {
                                    currentTileName = xmlReader.GetAttribute(0);
                                    propertyCardInfoCounter++;
                                }
                                catch (Exception e)
                                {
                                    Console.Error.WriteLine("Warning: Invalid string value in XML file. " + " ERROR: " + e.Message);
                                }
                            }
                            else
                                Console.Error.WriteLine("Warning: Tile in XML file is missing attribute value");
                            break;
                        case "Member":
                            currentMember = ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter);
                            break;
                        case "Rent":
                            currentBaseRent = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter));
                            break;
                        case "House1Rent":
                            currentHouse1Rent = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter));
                            break;
                        case "House2Rent":
                            currentHouse2Rent = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter));
                            break;
                        case "House3Rent":
                            currentHouse3Rent = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter));
                            break;
                        case "House4Rent":
                            currentHouse4Rent = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter));
                            break;
                        case "Hotel":
                            currentHotelRent = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter));
                            break;
                        case "Mortgage Value":
                            currentMortgageValue = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter));
                            break;
                        case "HouseCost":
                            currentHouseCost = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter));
                            break;
                        case "HotelCost":
                            currentHotelCost = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter));
                            break;
                        case "PropertyPrice":
                            currentPropertyPrice = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref propertyCardInfoCounter));

                            // Check if enough data has been pulled
                            if (propertyCardInfoCounter == 11)
                            {
                                // Skip over pre-made tiles
                                while (Tiles[counter] != null)
                                    counter++;
                                // Create the Tile
                                Tiles[counter] = new PropertyTile(
                                    TileType.Property,
                                    currentTileName,
                                    currentColor,
                                    currentBaseRent,
                                    currentHouse1Rent,
                                    currentHouse2Rent,
                                    currentHouse3Rent,
                                    currentHouse4Rent,
                                    currentHotelRent,
                                    currentMortgageValue,
                                    currentHouseCost,
                                    currentHotelCost,
                                    currentPropertyPrice);

                                // Reset the Card Counter
                                propertyCardInfoCounter = 0;
                            }
                            else
                                Console.Error.WriteLine("ERROR! Tile is missing data");
                            break;
                    }

                }
            }
        }

        private string ReadStringFromCurrentNode(XmlReader x, ref int counter)
        {
            string tempString = null;
            try
            {
                tempString = x.ReadInnerXml();
                counter++;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Warning: Invalid string value in XML file. " + " ERROR: " + e.Message);
            }

            return tempString;
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
                            Console.WriteLine("Player " + "\"" + currentTurnsPlayers.getName + "\"" + " is currently in jail");
                            Console.WriteLine("Press T to pay $50 to get out of jail, or R to try and roll doubles");
                            displayJailMessageOnce = false;
                        }

                        // Player decides to roll for doubles
                        if (kbInput.IsKeyDown(Keys.R) && previousKeyboardInput.IsKeyUp(Keys.R))
                        {
                            // Roll Dice
                            RollDice(currentTurnsPlayers);

                            // Only move if doubles were rolled or if player has been in jail for the third turn
                            if (DoublesRolled || currentTurnsPlayers.turnsInJail == 2)
                            {
                                if (currentTurnsPlayers.turnsInJail == 2)
                                {
                                    Console.WriteLine("Player " + "\"" + currentTurnsPlayers.getName + "\"" + " must pay $50 to get out of jail on third turn.");

                                    // Pay bank fine
                                    currentTurnsPlayers.PlayerPaysBank(50);
                                    // Set player out of jail
                                    currentTurnsPlayers.inJail = false;
                                    // Set turns in jail back to zero
                                    currentTurnsPlayers.turnsInJail = 0;
                                }

                                MovePlayerDiceRoll(currentTurnsPlayers, currentDiceRoll);
                                // Calculate options for player
                                PlayerOptions(currentTurnsPlayers);

                                // Turn off doubles rolled flag because player is not supposed to take another turn after getting out of jail
                                DoublesRolled = false;

                                turnPhase = 1;
                            }
                            else
                            {
                                if (Game1.DEBUG)
                                {
                                    Console.WriteLine("You failed to roll doubles and stay in jail.");
                                }

                                currentTurnsPlayers.turnsInJail++;
                                turnPhase = 2;
                            }
                        }

                        // If player chooses to pay to get out of jail
                        if (kbInput.IsKeyDown(Keys.T) && previousKeyboardInput.IsKeyUp(Keys.T))
                        {
                            Console.WriteLine("Player " + "\"" + currentTurnsPlayers.getName + "\"" + " pays $50 to escape from Babysitting Kyungsan");

                            // Pay bank fine
                            currentTurnsPlayers.PlayerPaysBank(50);
                            // Set turns in jail back to zero
                            currentTurnsPlayers.turnsInJail = 0;
                            currentTurnsPlayers.inJail = false;
                        }

                    }
                    else
                    {
                        // Roll Dice
                        if (kbInput.IsKeyDown(Keys.R) && previousKeyboardInput.IsKeyUp(Keys.R))
                        {
                            // Rolls Dice and Move Piece to Tile
                            RollDice(currentTurnsPlayers);
                            // Calculate options for player
                            PlayerOptions(currentTurnsPlayers);

                            // Set next phase
                            turnPhase = 1;
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
                            {
                                if (Game1.DEBUG)
                                {
                                    Console.WriteLine("Player " + "\"" + currentTurnsPlayers.getName + "\"" + " cannot purchase \"" + Tiles[currentTurnsPlayers.CurrentBoardPosition].getName + "\"");
                                }

                                // Go to next phase
                                turnPhase = 2;
                            }
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

                            if (Game1.DEBUG)
                            {
                                Console.WriteLine("Player " + "\"" + currentTurnsPlayers.getName + "\"" + " gets to roll again!");
                            }
                        }
                        else
                        {
                            // Start next player's turn
                            startNextPlayerTurn();
                            // Set phase back to 0 for next player
                            turnPhase = 0;
                            optionsCalculated = false;
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
