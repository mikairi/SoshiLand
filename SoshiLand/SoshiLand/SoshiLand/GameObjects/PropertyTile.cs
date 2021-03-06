﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


namespace SoshiLand
{
    public class PropertyTile : Tile
    {
        private Player owner = null;               // Owner of the property

        private string member;              // Name of SNSD member who owns the Property
        private Color color;                // Property Color
        private int baseRent = 0;          // Rent
        private int[] houseRent;           // Rent with 1-4 houses, index 0
        private int hotelRent = 0;         // Rent with Hotel
        private int mortgageValue = 0;     // Mortgage Value
        private int houseCost = 0;         // Cost for each house
        private int hotelCost = 0;         // Cost for Hotel (+ 4 houses)
        private int propertyPrice = 0;     // Cost to initially purchase property

        private int currentRentCost = 0;   // Current Rent Cost
        private int numberOfHouses = 0;    // Number of Houses current on property. For simplicity, this will be 5 for hotel.
        private bool monopoly = false;      // Flag for whether this property is part of a monopoly
        private bool mortgaged = false;     // Flag for whether this property is mortgaged

        public int getNumberOfHouses
        {
            get { return numberOfHouses; }
        }

        public int getMortgageValue
        {
            get { return mortgageValue; }
        }

        public bool MortgageStatus
        {
            set { mortgaged = value; }
            get { return mortgaged; }
        }

        public int getPropertyPrice
        {
            get { return propertyPrice; }
        }

        public int getRent
        {
            get { return currentRentCost; }
        }
        public Player Owner
        {
            set { owner = value; }
            get { return owner; }
        }
        public PropertyTile(
            TileType t,
            string n,
            Color c,
            int bR,
            int h1R,
            int h2R,
            int h3R,
            int h4R,
            int hR,
            int mV,
            int hC,
            int hotelC,
            int pP) : base(n, t)
        {
            // Set all the property values
            // These are meant to be static, so they shouldn't change

            houseRent = new int[4];

            color = c;
            baseRent = bR;
            houseRent[0] = h1R;
            houseRent[1] = h2R;
            houseRent[2] = h3R;
            houseRent[3] = h4R;
            hotelRent = hR;
            mortgageValue = mV;
            houseCost = hC;
            hotelCost = hotelC;
            propertyPrice = pP;

            currentRentCost = baseRent;

        }

        private bool addHouse()
        {
            // This assumes that the player already has monopoly over this color
            // There is another function that will allow house building based on a bool value for monopoly
            // There will also be another function that will check if the houses are equal or less across the monopoly

            // Checks if the player has enough money to buy a house
            if (houseCost > owner.getMoney && numberOfHouses < 4)
            {
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" purchases a house for " + houseCost);

                owner.PlayerPurchasesHouse(houseCost);
                numberOfHouses++;

                updateRentCost();

                // Player has enough funds and return success
                return true;
            }
            else
                // Player does not have enough funds or reached max number of houses
                return false;

        }

        private bool addHotel()
        {
            // Checks if the player has enough money to buy a house
            if (hotelCost > owner.getMoney && numberOfHouses == 4)
            {
                Game1.debugMessageQueue.addMessageToQueue("Player \"" + this.getName + "\" purchases a hotel for " + hotelCost);

                owner.PlayerPaysBank(hotelCost);
                numberOfHouses++;
                updateRentCost();

                // Player has enough funds and return success
                return true;
            }
            else
                // Either player doesn't have enough funds or there is already a hotel.
                return false;
        }

        private bool removeHouse()
        {
            // Check if there is at least one house
            if (numberOfHouses > 0)
            {

                numberOfHouses--;               // Remove one house
                updateRentCost();               // Update cost

                int refund = (int)(houseCost * 0.5);  // Calculate the refund. half the cost of a house
                Owner.BankPaysPlayer(refund);           // Bank pays player the refund

                return true;
            }
            else
                return false;
        }

        private void updateRentCost()
        {
            switch (numberOfHouses)
            {
                case 0:
                    if (monopoly)
                        currentRentCost = baseRent * 2;
                    else
                        currentRentCost = baseRent;
                    break;
                case 1:
                    currentRentCost = houseRent[0];
                    break;
                case 2:
                    currentRentCost = houseRent[1];
                    break;
                case 3:
                    currentRentCost = houseRent[2];
                    break;
                case 4:
                    currentRentCost = houseRent[3];
                    break;
                case 5:
                    currentRentCost = hotelRent;
                    break;
            }
            
            Game1.debugMessageQueue.addMessageToQueue("New Rent cost: " + currentRentCost);

        }

    }
}
