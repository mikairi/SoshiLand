using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoshiLandSilverlight
{
    public class Player
    {
        private string Name;                        // Player's Screen Name
        private int Money;                          // Player's Total Cash
        private bool Jail = false;                  // boolean for when player is in Jail or not
        private int numberOfTurnsInJail = 0;        // Keep track of how many turns Player has been in jail
        private int currentPositionOnBoard;         // Player's position on the board in the Tiles[] array (index 0)
        private byte numberOfFreeJailCards = 0;     // Number of Get Out of Jail Free cards player has

        private int actualAmountRemoved;           // If the player must pay another player an amount greater than what they own
        private int netWorth;                      // Player's net worth (Money + Buildings + printed prices of Mortgaged and Unmortgaged properties

        public byte FreeJailCards
        {
            set { numberOfFreeJailCards = value; }
            get { return numberOfFreeJailCards; }
        }

        public int getNetWorth
        {
            get { return netWorth; }
        }

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

        public int getMoney
        {
            get { return Money; }
        }

        public bool PurchaseProperty(PropertyTile property)
        {
            if (Money >= property.getPropertyPrice)
            {
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" purchased \"" + property.getName + "\" for $" + property.getPropertyPrice);
                
                removeMoney(property.getPropertyPrice);
                property.Owner = this;
                netWorth += property.getPropertyPrice;
                return true;
            }
            else
            {
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" does not have enough to purchase \"" + property.getName + "\"");
                return false;
            }
        }

        public bool PurchaseUtility(UtilityTile utility)
        {
            if (Money >= utility.getPropertyPrice)
            {
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" purchased \"" + utility.getName + "\" for $" + utility.getPropertyPrice);
                
                removeMoney(utility.getPropertyPrice);
                utility.Owner = this;
                netWorth += utility.getPropertyPrice;
                
                return true;
            }
            else
            {
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" does not have enough to purchase \"" + utility.getName + "\"");
                return false;
            }
        }

        public bool MortgageProperty(PropertyTile property)
        {
            // Check if property has zero houses
            if (property.getNumberOfHouses == 0)
            {
                property.MortgageStatus = true;                 // Set the property to mortgaged
                BankPaysPlayer(property.getMortgageValue);      // Give player money equal to mortgage value
                netWorth -= property.getMortgageValue;          // Set net worth back before it was mortgaged (net worth shouldn't change)

                return true;
            }
            else
            {
                // Player cannot mortgage
                Game1.debugMessageQueue.addMessageToQueue("Cannot mortgage " + property.getName + " when there are still houses.");
                return false;
            }
        }

        public bool UnmortgageProperty(PropertyTile property)
        {
            // Calculate unmortgage value (110% of mortgage price
            int newPrice = (int)Math.Round(property.getMortgageValue * 1.1);

            if (Money >= newPrice)                  // Check if player has enough money to unmortgage
            {
                PlayerPaysBank(newPrice);           // Pay bank
                property.MortgageStatus = false;    // Unmortgage house
                return true;
            }
            else
            {
                // Player cannot unmortgage
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + getName + "\" does not have enough money to unmortgage " + property.getName);
                return false;
            }
        }

        public void PlayerPurchasesHouse(int amountPaid)
        {
            netWorth += amountPaid;
            PlayerPaysBank(amountPaid);
        }

        public void PlayerPaysBank(int amountPaid)
        {
            Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" pays $" + amountPaid + " to the bank");
            removeMoney(amountPaid);
        }

        public void CurrentPlayerPaysPlayer(Player paidPlayer, int amountPaid)
        {
            // This function assumes the Player has sufficient funds to pay.
            // There is a separate function that will deal with the case where
            // The player does not have enough funds to pay

            Game1.debugMessageQueue.addMessageToQueue("Player \"" + paidPlayer.getName + "\" receives $" + amountPaid + " from Player \"" + this.getName + "\"");
            
            paidPlayer.addMoney(amountPaid);
            removeMoney(amountPaid);
        }

        public void BankPaysPlayer(int amountPaid)
        {
            Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" receives $" + amountPaid + " from the bank");
            addMoney(amountPaid);
        }
        
        private void addMoney(int money)
        {
            Money += money;
            netWorth += money;
        }

        private void removeMoney(int m)
        {
            Money -= m;
            netWorth -= m;

            Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" has $" + Money + " remaining");
        }
    }
}
