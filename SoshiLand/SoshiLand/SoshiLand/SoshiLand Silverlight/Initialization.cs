using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using System.Xml;

namespace SoshiLandSilverlight
{
    public class Initialization
    {
        public void DistributeStartingMoney(List<Player> listOfPlayers)
        {
            Game1.debugMessageQueue.addMessageToQueue("Distributing Starting Money");

            foreach (Player p in listOfPlayers)
            {
                // Starting money is $1500
                p.BankPaysPlayer(1500);
            }
        }

        public void InitializeTiles(Tile[] tiles)
        {
            // This probably isn't the most efficient way of creating the Tiles,
            // But it'll only be run once at the start of a game.

            // XML Reading Variables
            XmlReader xmlReader;
            xmlReader = XmlReader.Create("PropertyCards.xml");      // Set the XML file to read

            // First, reserve spots in array for non-property Tiles
            tiles[0] = new Tile("Go", TileType.Go);
            tiles[5] = new Tile("Special Luxury", TileType.SpecialLuxuryTax);
            tiles[8] = new Tile("Chance", TileType.Chance);
            tiles[12] = new Tile("Hello Baby", TileType.Jail);
            tiles[15] = new UtilityTile("Soshi Bond");
            tiles[20] = new Tile("Community Chest", TileType.CommunityChest);
            tiles[24] = new Tile("Fan Meeting", TileType.FanMeeting);
            tiles[27] = new Tile("Chance", TileType.Chance);
            tiles[33] = new UtilityTile("Forever 9");
            tiles[36] = new Tile("Babysit Kyung San", TileType.GoToJail);
            tiles[40] = new Tile("Community Chest", TileType.CommunityChest);
            tiles[45] = new Tile("Shopping Spree", TileType.ShoppingSpree);


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
                                while (tiles[counter] != null)
                                    counter++;
                                // Create the Tile
                                tiles[counter] = new PropertyTile(
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

        public void InitializeCards(List<Card> chanceCards, List<Card> communityChestCards)
        {
            // XML Reading Variables
            XmlReader xmlReader;
            xmlReader = XmlReader.Create("ChanceCards.xml");      // Set the XML file to read Chance Cards

            string currentDescription = "";
            uint currentMoneyAdded = 0;
            uint currentMoneySubtracted = 0;
            int currentPositionModifier = 0;
            int currentPositionMover = 0;
            bool currentPerPlayer = false;
            string currentSpecial = "";

            int chanceCardInfoCounter = 0;

            // Read in XML data for Chance Cards
            while (xmlReader.Read())
            {
                XmlNodeType nodeType = xmlReader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        case "Description":
                            currentDescription = ReadStringFromCurrentNode(xmlReader, ref chanceCardInfoCounter);
                            break;
                        case "MoneyAdded":
                            currentMoneyAdded = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref chanceCardInfoCounter));
                            break;
                        case "MoneySubtracted":
                            currentMoneySubtracted = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref chanceCardInfoCounter));
                            break;
                        case "PositionModifier":
                            currentPositionModifier = Convert.ToInt16(ReadStringFromCurrentNode(xmlReader, ref chanceCardInfoCounter));
                            break;
                        case "PositionMover":
                            currentPositionMover = Convert.ToInt16(ReadStringFromCurrentNode(xmlReader, ref chanceCardInfoCounter));
                            break;
                        case "PerPlayer":

                            string boolCheck = ReadStringFromCurrentNode(xmlReader, ref chanceCardInfoCounter);

                            if (boolCheck == "true")
                                currentPerPlayer = true;
                            else if (boolCheck == "false")
                                currentPerPlayer = false;
                            else
                                Console.WriteLine("Warning! Invalid value in Chance Card XML file for PerPlayer");
                            break;
                        case "Special":
                            currentSpecial = ReadStringFromCurrentNode(xmlReader, ref chanceCardInfoCounter);

                            if (chanceCardInfoCounter == 7)
                            {
                                // Create the Card and add it to the list of Chance Cards
                                chanceCards.Add(
                                    new Card(
                                        currentDescription,
                                        currentPositionModifier,
                                        currentPositionMover,
                                        currentMoneyAdded,
                                        currentMoneySubtracted,
                                        currentPerPlayer,
                                        currentSpecial));

                                // Reset the Card Counter
                                chanceCardInfoCounter = 0;
                            }
                        else
                            Console.Error.WriteLine("ERROR! Chance Card is missing data!");
                        break;
                    }
                    
                }
            }

            int communityChestCardInfoCounter = 0;

            xmlReader.Close();
            xmlReader = XmlReader.Create("CommunityChestCards.xml");        // Set the XML reader to read Community Chest Cards XML

            // Read in XML data for Community Chest Cards
            while (xmlReader.Read())
            {
                XmlNodeType nodeType = xmlReader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.Name)
                    {
                        case "Description":
                            currentDescription = ReadStringFromCurrentNode(xmlReader, ref communityChestCardInfoCounter);
                            break;
                        case "MoneyAdded":
                            currentMoneyAdded = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref communityChestCardInfoCounter));
                            break;
                        case "MoneySubtracted":
                            currentMoneySubtracted = Convert.ToUInt16(ReadStringFromCurrentNode(xmlReader, ref communityChestCardInfoCounter));
                            break;
                        case "PositionModifier":
                            currentPositionModifier = Convert.ToInt16(ReadStringFromCurrentNode(xmlReader, ref communityChestCardInfoCounter));
                            break;
                        case "PositionMover":
                            currentPositionMover = Convert.ToInt16(ReadStringFromCurrentNode(xmlReader, ref communityChestCardInfoCounter));
                            break;
                        case "PerPlayer":

                            string boolCheck = ReadStringFromCurrentNode(xmlReader, ref communityChestCardInfoCounter);

                            if (boolCheck == "true")
                                currentPerPlayer = true;
                            else if (boolCheck == "false")
                                currentPerPlayer = false;
                            else
                                Console.WriteLine("Warning! Invalid value in Chance Card XML file for PerPlayer");
                            break;
                        case "Special":
                            currentSpecial = ReadStringFromCurrentNode(xmlReader, ref communityChestCardInfoCounter);

                            if (communityChestCardInfoCounter == 7)
                            {
                                // Create the Card and add it to the list of Chance Cards
                                communityChestCards.Add(
                                    new Card(
                                        currentDescription,
                                        currentPositionModifier,
                                        currentPositionMover,
                                        currentMoneyAdded,
                                        currentMoneySubtracted,
                                        currentPerPlayer,
                                        currentSpecial));

                                // Reset the Card Counter
                                communityChestCardInfoCounter = 0;
                            }
                            else
                                Console.Error.WriteLine("ERROR! Community Chest Card is missing data!");
                            break;
                    }
                    
                }
            }
        }

        public void DeterminePlayerOrder(Player[] arrayOfPlayers, ref List<Player> ListOfPlayers)
        {
            // Note!
            // arrayOfPlayers is the order the players are sitting in around the board.
            // So the order is determined by starting at the player with the highest roll 
            // and moving clockwise around the board

            Game1.debugMessageQueue.addMessageToQueue("Players rolling to determine Order");

            int[] playerRolls = new int[arrayOfPlayers.Length];     // An array the size of the number of players to hold their dice rolls
            List<Player> tiedPlayers = new List<Player>();          // List of players that are tied for highest roll

            int currentHighestPlayer = 0;                           // Current player index in arrayOfPlayers with the highest roll

            // Have each player roll a pair of dice and store the result in the playerRolls array
            for (int i = 0; i < arrayOfPlayers.Length; i++)
            {
                SoshiLandGameFunctions.RollDice(arrayOfPlayers[i]);
                playerRolls[i] = SoshilandGame.currentDiceRoll;

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

                Game1.debugMessageQueue.addMessageToQueue("Player " + "\"" + arrayOfPlayers[currentHighestPlayer].getName + "\"" + " is the current highest roller with: " + playerRolls[currentHighestPlayer]);
            }

            // Initialize the list of players
            ListOfPlayers = new List<Player>();

            // Check if there is a tie with highest rolls
            if (tiedPlayers.Count > 0)
            {
                Game1.debugMessageQueue.addMessageToQueue("There's a tie!");
                // New list to store second round of tied players
                List<Player> secondRoundOfTied = new List<Player>();
                // Keep rolling until no more tied players
                while (secondRoundOfTied.Count != 1)
                {
                    int currentHighestRoll = 0;

                    // Roll the dice for each player
                    foreach (Player p in tiedPlayers)
                    {

                        SoshiLandGameFunctions.RollDice(p);                                                    // Roll the dice for the player
                        // If the new roll is higher than the current highest roll
                        if (SoshilandGame.currentDiceRoll > currentHighestRoll)
                        {
                            // Clear the list since everyone who may have been in the list is lower 
                            secondRoundOfTied.Clear();

                            // Set the new highest roll
                            currentHighestRoll = SoshilandGame.currentDiceRoll;
                            secondRoundOfTied.Add(p);
                        }
                        // If there's another tie, just add it to the new array without clearing it
                        else if (SoshilandGame.currentDiceRoll == currentHighestRoll)
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
                Game1.debugMessageQueue.addMessageToQueue("Player Order Determined! ");
                for (int i = 1; i < ListOfPlayers.Count + 1; i++)
                    Game1.debugMessageQueue.addMessageToQueue(i + ": " + ListOfPlayers[i - 1].getName);


            }
        }


        public void PlaceAllPiecesOnGo(List<Player> listOfPlayers)
        {
            Game1.debugMessageQueue.addMessageToQueue("Placing all players on Go");

            foreach (Player p in listOfPlayers)
            {
                // Move player to Go
                SoshiLandGameFunctions.MovePlayer(p, 0);
            }
            SoshilandGame.gameInitialized = true;
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
