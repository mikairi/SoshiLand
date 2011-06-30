using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoshiLandSilverlight
{
    public class UtilityTile : Tile
    {
        private int propertyPrice = 150;       // Cost to initially purchase property. Default is 150
        private int mortgageValue = 75;        // Mortgage Value

        private Player owner;                   // Owner of property

        private bool mortgaged = false;         // Flag for whether this Utility is mortgaged

        public bool MortgageStatus
        {
            set { mortgaged = value; }
            get { return mortgaged; }
        }

        public Player Owner
        {
            set { owner = value; }
            get { return owner; }
        }

        public int getPropertyPrice
        {
            get { return propertyPrice; }
        }

        public UtilityTile(string name) : base(name, TileType.Utility)
        {

        }

    }
}
