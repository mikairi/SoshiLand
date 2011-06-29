using System;
using System.Linq;
using System.Collections.Generic;
namespace SoshiLandSilverlight
{
    public class DeckOfCards
    {
        List<Card> Deck;            // The Deck

        public DeckOfCards(Card[] cardArray)
        {
            Deck = new List<Card>();
            foreach (Card c in cardArray)
                Deck.Add(c);
        }

        public DeckOfCards()
        {
            Deck = new List<Card>();
        }

        public void AddCard(Card c)
        {
            Deck.Add(c);
        }

        public void RemoveCard(Card c)
        {
            if (Deck.Contains(c))
                Deck.Remove(c);
            else
                Game1.debugMessageQueue.addMessageToQueue("Warning: Card " + c + "does not exist! Cannot remove from deck!");
        }

        public Card drawCard()
        {
            // Draws a card and puts it on the bottom of the deck

            Card cardToDraw = Deck[0];          // Take the card on the top of the deck

            Deck.RemoveAt(0);                   // Remove it from the deck
            if (cardToDraw.getSpecialCardType != SpecialCardType.GetOutOfJailFreeCard)          // Check if the card drawn is not a get out of jail free card
                Deck.Add(cardToDraw);               // Add it back to the deck, but now on the bottom of the deck

            // Note: if the card is a get out of jail free card, then the player keeps the card and the card does NOT get placed on the bottom of the deck

            return cardToDraw;                  // Return the card on the top of the deck
        }

        public void ShuffleDeck()
        {
            Game1.debugMessageQueue.addMessageToQueue("Shuffled Deck");

            var shuffled = Deck.OrderBy(a => Guid.NewGuid());       // Shuffles the deck by a random GUID assigned to each card
            Deck.Clear();                                           // Clear the Deck
            foreach (Card c in shuffled)
                Deck.Add(c);                                        // Construct the new shuffled deck
        }
    }
}
