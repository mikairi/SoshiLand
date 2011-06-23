using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoshiLandSilverlight
{
    class Player
    {
        private string Name;                        // Player's Screen Name
        private uint Money;                         // Player's Total Cash
        private bool Jail = false;                  // boolean for when player is in Jail or not


        public Player(string n)
        {
            Name = n;
        }

        private void addMoney(uint money)
        {
            Money += money;
        }

        private void removeMoney(uint money)
        {
            // Check if the amount to remove is greater than the Player's total money
            // Since money is a uint, must be positive
            if (!(money > Money))
                Money -= money;
        }
    }
}
