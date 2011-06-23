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
        List<Player> ListOfPlayers;                     // Contains the list of players in the game
        Tile[] Tiles = new Tile[48];                    // Array of Tiles


        public SoshilandGame()
        {
            InitializeTiles();
        }

        private void InitializeGame()
        {

        }

        private Color getColorFromNumber(int c)
        {
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

        private void InitializeTiles()
        {
            // This probably isn't the most efficient way of creating the Tiles,
            // But it'll only be run once at the start of a game.

            // XML Reading Variables
            XmlReader xmlReader;

            xmlReader = XmlReader.Create("PropertyCards.xml");
            
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

            string currentMember;                  // Name of SNSD member who owns the Property
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

            while (xmlReader.Read())
            {
                XmlNodeType nodeType = xmlReader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == "Color" && !Game1.DEBUG)
                        //Game1.DEBUG = false;;
                        ;
                    
                }
            }


            
            /*
            // Iterate through Colors
            foreach (XmlNode nodeColor in xmlNode)
            {   
                if (nodeColor.Attributes.Count > 0)
                {
                    foreach (XmlAttribute xmlAttribute in nodeColor.Attributes)
                        if (xmlAttribute.Name == "color")
                            currentColor = getColorFromNumber(Convert.ToInt16(xmlAttribute.Value));         // Get the color based on the number retrieved from XML
                }
                // Iterate through Tiles
                foreach (XmlNode nodeTile in nodeColor)
                {
                    if (nodeTile.Attributes.Count > 0)
                    {
                        foreach (XmlAttribute xmlAttribute in nodeTile.Attributes)
                            if (xmlAttribute.Name == "name")
                                currentTileName = xmlAttribute.Value;                                       // Get the name of the Tile as a string
                        
                    }
                    while (Tiles[counter] != null)
                        counter++;

                    // Iterate through Tile Members
                    foreach (XmlNode nodeTileMember in nodeTile)
                    {
                        // Assign Property Values
                        if (nodeTileMember.Name == "Member")
                            currentMember = nodeTileMember.InnerText;
                        if (nodeTileMember.Name == "Rent")
                            currentBaseRent = Convert.ToUInt16(nodeTileMember.InnerText);
                        if (nodeTileMember.Name == "House1Rent")
                            currentHouse1Rent = Convert.ToUInt16(nodeTileMember.InnerText);
                        if (nodeTileMember.Name == "House2Rent")
                            currentHouse2Rent = Convert.ToUInt16(nodeTileMember.InnerText);
                        if (nodeTileMember.Name == "House3Rent")
                            currentHouse3Rent = Convert.ToUInt16(nodeTileMember.InnerText);
                        if (nodeTileMember.Name == "House4Rent")
                            currentHouse4Rent = Convert.ToUInt16(nodeTileMember.InnerText);
                        if (nodeTileMember.Name == "Hotel")
                            currentHotelRent = Convert.ToUInt16(nodeTileMember.InnerText);
                        if (nodeTileMember.Name == "MortgageValue")
                            currentMortgageValue = Convert.ToUInt16(nodeTileMember.InnerText);
                        if (nodeTileMember.Name == "HouseCost")
                            currentHouseCost = Convert.ToUInt16(nodeTileMember.InnerText);
                        if (nodeTileMember.Name == "HotelCost")
                            currentHotelCost = Convert.ToUInt16(nodeTileMember.InnerText);
                        if (nodeTileMember.Name == "PropertyPrice")
                            currentPropertyPrice = Convert.ToUInt16(nodeTileMember.InnerText);
                    }

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
                }
            }
            */
        }
    }
}
