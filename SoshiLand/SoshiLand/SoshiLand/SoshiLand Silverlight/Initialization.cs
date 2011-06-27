using System;

using Microsoft.Xna.Framework;

using System.Xml;

namespace SoshiLandSilverlight
{
    public static class Initialization
    {


        public static void InitializeTiles(Tile[] tiles)
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

        private static Color getColorFromNumber(int c)
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

        private static string ReadStringFromCurrentNode(XmlReader x, ref int counter)
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
