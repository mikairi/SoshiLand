using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoshiLand
{
    class UtilityTile : Tile
    {
        private uint propertyPrice = 150;       // Cost to initially purchase property. Default is 150
        private uint mortgageValue = 75;        // Mortgage Value

        private Player owner;                   // Owner of property

        public Player Owner
        {
            set { owner = value; }
            get { return owner; }
        }

        public UtilityTile(string name) : base(name, TileType.Utility)
        {

        }

    }
}
