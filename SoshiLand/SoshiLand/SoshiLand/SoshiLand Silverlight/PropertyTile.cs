using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
namespace SoshiLandSilverlight
{
    class PropertyTile : Tile
    {
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


        public PropertyTile(
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
            uint pP) : base(n)
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

    }
}
