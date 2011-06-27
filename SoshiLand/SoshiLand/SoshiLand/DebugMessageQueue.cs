using System;
using System.Net;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SoshiLand
{
    public class DebugMessageQueue
    {
        private List<string> MessageQueue = new List<string>();

        private double messageInterval = 50;
        private double lastMessageInterval = 0;
        private bool resetGameTime = false;

        SpriteFont spriteFont;

        public DebugMessageQueue()
        {

        }

        

        public void addMessageToQueue(string message)
        {
            if (Game1.DEBUG)
            {
                //MessageQueue.Add(message);
                Console.WriteLine(message);
            }
        }

        public void PrintMessages(GameTime gameTime, SpriteBatch spriteBatch)
        {

            if (MessageQueue.Count > 20)
            {
                if (resetGameTime)
                {
                    lastMessageInterval = gameTime.TotalGameTime.TotalMilliseconds;
                    resetGameTime = false;
                }

                if ((gameTime.TotalGameTime.TotalMilliseconds - lastMessageInterval) > messageInterval)
                {
                    // Remove the first string
                    MessageQueue.RemoveAt(0);
                    resetGameTime = true;
                }
            }
            /*
            spriteBatch.Begin();

            for (int i = 0; i < 20; i++)
            {
                if (MessageQueue.Count > i)
                    spriteBatch.DrawString(spriteFont, MessageQueue[i], new Vector2(0, 30 * i), Color.HotPink);

            }

            spriteBatch.End();
            */ 
        }
    }
}
