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
        private int numberOfTurnsInJail = 0;                // Keep track of how many turns Player has been in jail
        private int currentPositionOnBoard;         // Player's position on the board in the Tiles[] array (index 0)

        private uint actualAmountRemoved;           // If the player must pay another player an amount greater than what they own

        public bool inJail
        {
            set { Jail = true; }
            get { return Jail; }
        }

        public int turnsInJail
        {
            set { numberOfTurnsInJail = value; }
            get { return numberOfTurnsInJail; }
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

        public uint getMoney
        {
            get { return Money; }
        }

        public bool PurchaseProperty(PropertyTile property)
        {
            if (Money >= property.getPropertyPrice)
            {
                if (Game1.DEBUG)
                {
                    Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" purchased \"" + property.getName + "\" for $" + property.getPropertyPrice);
                    Console.WriteLine("Player \"" + this.getName + "\" purchased \"" + property.getName + "\" for $" + property.getPropertyPrice);
                }
                removeMoney(property.getPropertyPrice);
                property.Owner = this;
                
                return true;
            }
            else
            {
                if (Game1.DEBUG)
                {
                    Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" does not have enough to purchase \"" + property.getName + "\"");
                    Console.WriteLine("Player \"" + this.getName + "\" does not have enough to purchase \"" + property.getName + "\"");
                }
                return false;
            }
        }

        public bool PurchaseUtility(UtilityTile utility)
        {
            if (Money >= utility.getPropertyPrice)
            {
                if (Game1.DEBUG)
                {
                    Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" purchased \"" + utility.getName + "\" for $" + utility.getPropertyPrice);
                    Console.WriteLine("Player \"" + this.getName + "\" purchased \"" + utility.getName + "\" for $" + utility.getPropertyPrice);
                }
                removeMoney(utility.getPropertyPrice);
                utility.Owner = this;
                
                return true;
            }
            else
            {
                if (Game1.DEBUG)
                {
                    Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" does not have enough to purchase \"" + utility.getName + "\"");
                    Console.WriteLine("Player \"" + this.getName + "\" does not have enough to purchase \"" + utility.getName + "\"");
                }
                return false;
            }
        }

        public void PlayerPaysBank(uint amountPaid)
        {
            if (Game1.DEBUG)
            {
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" pays $" + amountPaid + " to the bank");
                Console.WriteLine("Player \"" + this.getName + "\" pays $" + amountPaid + " to the bank");
            }

            removeMoney(amountPaid);
        }

        public void CurrentPlayerPaysPlayer(Player paidPlayer, uint amountPaid)
        {
            // This function assumes the Player has sufficient funds to pay.
            // There is a separate function that will deal with the case where
            // The player does not have enough funds to pay

            if (Game1.DEBUG)
            {
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + paidPlayer.getName + "\" receives $" + amountPaid + " from Player \"" + this.getName + "\"");
                Console.WriteLine("Player \"" + paidPlayer.getName + "\" receives $" + amountPaid + " from Player \"" + this.getName + "\"");
            }
            
            paidPlayer.addMoney(amountPaid);
            removeMoney(amountPaid);
        }

        public void BankPaysPlayer(uint amountPaid)
        {

            if (Game1.DEBUG)
            {
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" receives $" + amountPaid + " from the bank");
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

            if (Game1.DEBUG)
            {
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" has $" + Money + " remaining");
                Console.WriteLine("Player \"" + this.getName + "\" has $" + Money + " remaining");
            }
        }
    }
}
