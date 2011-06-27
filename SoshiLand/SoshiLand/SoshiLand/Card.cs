using System;
using System.Net;


namespace SoshiLand
{

    // There doesn't seem to be a difference between Chance and Community Chest cards,
    // So I'm making just one class for both Chance and Community Chest cards
    public class Card
    {
        private string description = "";        // Description of the card (literal text of the card)
        private int moveModifier = 0;           // Value that the card will move the player from his or her current position (ie. go back 3 spaces)
        private int movePosition = 0;           // Specific tile that the card will move the player to (ie. advance to Go)
        private int moneyModifier = 0;          // Amount of money to add/subtract

        private bool perPlayer = false;         // Per player means all players contribute to the moneyAdded / moneySubtracted value
        // Ex.
        // $10 added with perPlayer == true means the player who draws this card will receive $10 from each player
        // $10 subtracted with perPlayer == false means that the player who draws this card will give each player $10

        // Special cards (namely Jail related cards, but if we decide to add other special cards later, this'll be easier to implement with this enum
        private SpecialCardType specialCardType = SpecialCardType.None;

        public int getMoveModifier
        {
            get { return moveModifier; }
        }

        public int getMovePosition
        {
            get { return movePosition; }
        }

        public int getMoneyModifier
        {
            get { return moneyModifier; } 
        }

        public Card(
            string cardDescription,
            int cardMoveModifier,
            int cardMovePosition,
            uint cardMoneyAdded,
            uint cardMoneySubtracted,
            bool cardPerPlayer,
            string cardSpecial)
        {
            // Set the variables
            description = cardDescription;
            moveModifier = cardMoveModifier;
            movePosition = cardMovePosition;
            perPlayer = cardPerPlayer;

            // Check if the card is subtracting money
            if (cardMoneyAdded == 0)
                moneyModifier -= (int)cardMoneySubtracted;      // Make the moneyModifier negative by subtracting the amountSubtracted from 0
            else
                moneyModifier += (int)cardMoneyAdded;           // Make the moneyModifier positive by adding the amountAdded to 0

            // Check if the card has a special attribute
            if (cardSpecial != "")
            {
                if (cardSpecial == "freeJail")
                    specialCardType = SpecialCardType.GetOutOfJailFreeCard;
                if (cardSpecial == "goToJail")
                    specialCardType = SpecialCardType.GoToJailCard;
            }
        }

    }
}
