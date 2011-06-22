using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SoshiLand
{
    class Tile
    {
        private string Name;
        private Texture2D texture;

        public string getName
        {
            get { return Name; }
        }

        public Tile(string n)
        {
            Name = n;
        }
    }
}
