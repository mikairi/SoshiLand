using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoshiLand
{
    class Player
    {
        private string Name;                        // Player's Screen Name
        private uint Money;                         // Player's Total Cash
        private bool Jail = false;                  // boolean for when player is in Jail or not
        private int currentPositionOnBoard;          // Player's position on the board in the Tiles[] array (index 0)

        public bool inJail
        {
            set { Jail = true; }
            get { return Jail; }
        }

        public int CurrentBoardPosition
        {
            set { currentPositionOnBoard = value; }
            get { return currentPositionOnBoard; }
        }

        public string getName
        {
            get { return Name; }
        }

        public Player(string n)
        {
            Name = n;
        }

        public void BankPaysPlayer(uint amountPaid)
        {

            if (Game1.DEBUG)
            {
                Console.WriteLine("Player \"" + this.getName + "\" receives $" + amountPaid + " from the bank");
            }
            addMoney(amountPaid);
        }

        private void addMoney(uint money)
        {
            Money += money;
        }

        private void removeMoney(uint m)
        {
            // Check if the amount to remove is greater than the Player's total money
            // Since money is a uint, must be positive
            if (!(m > Money))
                Money -= m;
            // Otherwise, the player is required to sell / trade / mortgage 
            // Need to put an else here later.
        }
    }
}
