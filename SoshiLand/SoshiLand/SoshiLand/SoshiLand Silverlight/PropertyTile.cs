using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


namespace SoshiLandSilverlight
{
    class PropertyTile : Tile
    {
        private Player owner = null;               // Owner of the property

        private string member;              // Name of SNSD member who owns the Property
        private Color color;                // Property Color
        private uint baseRent = 0;          // Rent
        private uint house1Rent = 0;        // Rent with 1 House
        private uint house2Rent = 0;        // Rent with 2 Houses
        private uint house3Rent = 0;        // Rent with 3 Houses
        private uint house4Rent = 0;        // Rent with 4 Houses
        private uint hotelRent = 0;         // Rent with Hotel
        private uint mortgageValue = 0;     // Mortgage Value
        private uint houseCost = 0;         // Cost for each house
        private uint hotelCost = 0;         // Cost for Hotel (+ 4 houses)
        private uint propertyPrice = 0;     // Cost to initially purchase property

        private uint currentRentCost = 0;   // Current Rent Cost

        public uint Rent
        {
            get { return Rent; }
        }

        public PropertyTile(
            TileType t,
            string n,
            Color c,
            uint bR,
            uint h1R,
            uint h2R,
            uint h3R,
            uint h4R,
            uint hR,
            uint mV,
            uint hC,
            uint hotelC,
            uint pP) : base(n, t)
        {
            // Set all the property values
            // These are meant to be static, so they shouldn't change

            color = c;
            baseRent = bR;
            house1Rent = h1R;
            house2Rent = h2R;
            house3Rent = h3R;
            house4Rent = h4R;
            hotelRent = hR;
            mortgageValue = mV;
            houseCost = hC;
            hotelCost = hotelC;
            propertyPrice = pP;

        }

        public Player Owner
        {
            set { owner = value; }
            get { return owner; }
        }

        private bool addHouse()
        {
            // This assumes that the player already has monopoly over this color
            // There is another function that will allow house building based on a bool value for monopoly
            // There will also be another function that will check if the houses are equal or less across the monopoly

            // Checks if the player has enough money to buy a house
            if (houseCost > owner.getMoney)
            {
                if (Game1.DEBUG)
                {
                    Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" purchases a house for " + getName);
                    Console.WriteLine("Player \"" + this.getName + "\" purchases a house for " + getName);
                }

                owner.PlayerPaysBank(houseCost);
                // Player has enough funds and return success
                return true;
            }
            else
                // Player does not have enough funds
                return false;

        }

    }
}
