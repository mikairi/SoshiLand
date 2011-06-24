using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
// Required to read XML file
using System.Xml;


namespace SoshiLandSilverlight
{
    class SoshilandGame
    {
        List<Player> ListOfPlayers;                     // Contains the list of players in the game. This will be in the order from first to last player
        Tile[] Tiles = new Tile[48];                    // Array of Tiles

        private static Random die = new Random();       // Need to create a static random die generator so it doesn't reuse the same seed over and over

        private bool DoublesRolled;                     // Flag to indicate doubles were rolled
        private int currentDiceRoll;                    // Global dice roll variable for special instances when we need to know (ie. determining player order)
        private int numberOfDoubles;                    // Keep track of the number of doubles rolled

        private bool gameInitialized = false;           // Flag for when the game is officially started



        // TEMPORARY
        Player[] playerArray;

        public SoshilandGame()
        {
            InitializeTiles();                          // Initialize Tiles on the board

            InitializeGame();
        }

        private void InitializeGame()
        {
            // Temporary list of players
            Player player1 = new Player("Player 1");
            Player player2 = new Player("Player 2");
            Player player3 = new Player("Player 3");
            Player player4 = new Player("Player 4");

            playerArray = new Player[4];
            playerArray[0] = player1;
            playerArray[1] = player2;
            playerArray[2] = player3;
            playerArray[3] = player4;

            DeterminePlayerOrder(playerArray);
        }

        public void TESTPLAYERORDER()
        {
            DeterminePlayerOrder(playerArray);
        }

        private void DeterminePlayerOrder(Player[] arrayOfPlayers)
        {
            // Note!
            // arrayOfPlayers is the order the players are sitting in around the board.
            // So the order is determined by starting at the player with the highest roll 
            // and moving clockwise around the board



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
                    Console.WriteLine("Player " + "\"" + arrayOfPlayers[currentHighestPlayer].getName + "\"" + " is the current highest roller with: " + playerRolls[currentHighestPlayer]);
            }

            // Initialize the list of players
            ListOfPlayers = new List<Player>();

            // Check if there is a tie with highest rolls
            if (tiedPlayers.Count > 0)
            {
                if (Game1.DEBUG)
                    Console.WriteLine("There's a tie!");

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
                    Console.WriteLine(i + ": " + ListOfPlayers[i - 1].getName);

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

        public void RollDice(Player p)
        {
            DoublesRolled = false;
            int dice1Int = die.Next(1, 6);
            int dice2Int = die.Next(1, 6);
            int total = dice1Int + dice2Int;

            currentDiceRoll = total;                // Set the global dice roll variable

            if (dice1Int == dice2Int)
                DoublesRolled = true;

            if (Game1.DEBUG)
            {
                Console.WriteLine("Player " + "\"" + p.getName + "\"" + " rolls dice: " + dice1Int + " and " + dice2Int + ". Total: " + total);
                if (DoublesRolled)
                    Console.WriteLine("Player " + "\"" + p.getName + "\"" + " rolled doubles!");
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
                newPosition = Math.Abs(newPosition - 48);           // Get absolute value of the difference and move player to that new Tile

            // Move player to the new position
            MovePlayer(p, newPosition);
        }

        private void MovePlayer(Player p, int position)
        {
            // Update the player's current position to the new position
            p.CurrentBoardPosition = position;

            if (Game1.DEBUG)
                Console.WriteLine("Player " + "\"" + p.getName + "\"" + " moves to Tile \"" + Tiles[position].getName + "\"");
        }

        private void InitializeTiles()
        {
            // This probably isn't the most efficient way of creating the Tiles,
            // But it'll only be run once at the start of a game.

            // XML Reading Variables
            XmlReader xmlReader;
            xmlReader = XmlReader.Create("PropertyCards.xml");      // Set the XML file to read

            // First, reserve spots in array for non-property Tiles
            Tiles[0] = new Tile("Go");
            Tiles[5] = new Tile("Special Luxury");
            Tiles[8] = new Tile("Chance");
            Tiles[12] = new Tile("Hello Baby");
            Tiles[15] = new Tile("Soshi Bond");
            Tiles[20] = new Tile("Community Chest");
            Tiles[24] = new Tile("Fan Meeting");
            Tiles[27] = new Tile("Chance");
            Tiles[33] = new Tile("Forever 9");
            Tiles[36] = new Tile("Babysit Kyung San");
            Tiles[40] = new Tile("Community Chest");
            Tiles[45] = new Tile("Shopping Spree");

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
    }
}
