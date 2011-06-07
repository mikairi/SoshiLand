using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

// Network Library
using Lidgren.Network;
// Library for Keyboard Input
// This will be primarily used for Chat and Text input
// Since XNA has a very poor interface for doing text input because the rate of input is dependent on the frame rate.
using Nuclex.Input;

namespace SoshiLand
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardState prevKeyboardState = Keyboard.GetState();

        Rectangle mainFrame;

        // Temporary Text Variables
        SpriteFont spriteFont;
        Rectangle boxIP;

        // Network Variables
        bool printToConsole = false;
        Network network;
        bool networkChosen = false;
        bool enterIP = false;
        string IP;
        bool ipEntered = false;
        System.Net.IPEndPoint networkIP;

        string networkMessage = "C (client) or H (Host) for server selection";

        // Input Manager for text input
        // Remember that this is here specifically for text input!
        InputManager input;
        private string enteredText;

        // The background which is also the board.
        Texture2D background;

        // Sprites of property cards.
        Texture2D propLaScala;
        Texture2D propBali;
        Texture2D propTempMount;
        Texture2D propDamnoenMart;
        Texture2D propGreatWall;
        Texture2D propTajMahal;
        Texture2D propStatLiberty;
        Texture2D propEiffel;
        Texture2D propParthenon;
        Texture2D chance1;
        Texture2D forever9;

        // The position to display a magnified property card.
        Vector2 zoomPos;

        // An integer that determines which property card to show. 0 means no card is selected.
        Props drawId = Props.None;

        public Game1()
        {
            graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            Window.Title = "SoshiLand";
            Window.AllowUserResizing = false;

            // Preferred window size is 640x640
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;

            input = new InputManager(Services, Window.Handle);
            Components.Add(input);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            input.GetKeyboard().CharacterEntered += keyboardCharacterEntered;
        }

        private void keyboardCharacterEntered(char character)
        {
            enteredText += character;
            if (printToConsole)
                Console.Write(character);
            if (enterIP)
            {
                // If the character is a backspace
                string testString = char.ConvertFromUtf32(8);
                char testChar = testString[0];
                if (character == testChar)
                    IP = IP.Substring(0, IP.Length - 1);
                else
                    IP += character;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch( GraphicsDevice );

            // Load the background which is also the board.
            background = Content.Load<Texture2D>( "assets\\main_screen_wide" );
            mainFrame = new Rectangle( 0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height );
            zoomPos = new Vector2( (float) mainFrame.Width * (float) 0.84375, (float) mainFrame.Height * (float) 0.0875 );

            // Load property cards.
            propLaScala = Content.Load<Texture2D>( "assets\\prop_la_scala" );
            propBali = Content.Load<Texture2D>( "assets\\prop_bali" );
            propTempMount = Content.Load<Texture2D>( "assets\\prop_temp_mount" );
            propDamnoenMart = Content.Load<Texture2D>( "assets\\prop_damnoen_mart" );
            propGreatWall = Content.Load<Texture2D>( "assets\\prop_great_wall" );
            propTajMahal = Content.Load<Texture2D>( "assets\\prop_taj_mahal" );
            propStatLiberty = Content.Load<Texture2D>( "assets\\prop_stat_liberty" );
            propEiffel = Content.Load<Texture2D>( "assets\\prop_eiffel" );
            propParthenon = Content.Load<Texture2D>( "assets\\prop_parthenon" );
            chance1 = Content.Load<Texture2D>( "assets\\chance1" );
            forever9 = Content.Load<Texture2D>( "assets\\forever9" );

            // Load Sprite Font
            spriteFont = Content.Load<SpriteFont>( "SpriteFont1" );
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( GameTime gameTime )
        {
            // Allows the game to exit
            if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed )
                this.Exit();

            KeyboardState kbInput = Keyboard.GetState();

            // Full screen function. Currently disabled.
            //if ( kbInput.IsKeyDown( Keys.F11 ) && prevKeyboardState.IsKeyUp( Keys.F11 ) )
            //{
            //    graphics.ToggleFullScreen();
            //    mainFrame.Height = GraphicsDevice.Viewport.Height;
            //    mainFrame.Width = mainFrame.Height;
            //    mainFrame.X = (GraphicsDevice.Viewport.Width - mainFrame.Width) / 2;
            //}

            MouseState ms = Mouse.GetState();

            // Set drawId based on the mouse position when left-clicked. Commented out to develop new UI.
            if ( ms.Y <= 84 )
            {
                if ( ms.X >= 324 )
                {
                    if ( ms.X <= 375 )
                        drawId = Props.LaScala;
                    else if ( ms.X <= 425 )
                        drawId = Props.Bali;
                    else if ( ms.X <= 474 )
                        drawId = Props.Chance1;
                    else if ( ms.X <= 525 )
                        drawId = Props.TempleMount;
                    else if ( ms.X <= 575 )
                        drawId = Props.DamnoenMarket;
                    else if ( ms.X <= 626 )
                        drawId = Props.GreatWall;
                    else if ( ms.X <= 677 )
                        drawId = Props.TajMahal;
                    else if ( ms.X <= 727 )
                        drawId = Props.StatueLiberty;
                    else if ( ms.X <= 778 )
                        drawId = Props.Forever9;
                    else if ( ms.X <= 827 )
                        drawId = Props.EiffelTower;
                    else if ( ms.X <= 876 )
                        drawId = Props.Parthenon;
                    else drawId = Props.None;
                }
                else drawId = Props.None;
            }
            else drawId = Props.None;

            //////////////////
            // Network Code //
            //////////////////

            // Choose between Host or Client
            if (!networkChosen)
            {
                if (kbInput.IsKeyDown(Keys.H))
                {
                    networkChosen = true;
                    network = new Network(14242);
                    network.startNetwork();
                    Console.WriteLine("HOST SERVER STARTED");

                    networkMessage = "Your IP is: " + network.getThisIP();
                    networkMessage = "Wait for Clients to connect.";
                }
                else if (kbInput.IsKeyDown(Keys.C))
                {
                    networkChosen = true;
                    network = new Network();
                    network.startNetwork();
                    Console.WriteLine("CLIENT SERVER STARTED");

                    // TEMPORARY - FOR TESTING
                    networkIP = new System.Net.IPEndPoint(0x8201a8c0, 14242);

                    networkMessage = "Press Enter to enter a Host to connect to.";
                }
            }

            // Enter Host IP

            if (enterIP && !ipEntered)
            {
                printToConsole = true;
                if (kbInput.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter))
                {
                    enterIP = false;
                    //printToConsole = false;

                    Console.WriteLine();
                    Console.WriteLine("ENTERED IP: " + IP);
                    networkMessage = "Connecting to " + IP + " ...";

                    network.clientDiscoverHost(network.convertIP(IP));

                    ipEntered = true;
                }

            }


            if (!enterIP && !ipEntered)
            {
                if (kbInput.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter))
                {
                    enterIP = true;
                    Console.WriteLine("ENTER IP");
                    networkMessage = "Enter an IP and press Enter.";
                }
            }

            

            if (kbInput.IsKeyDown(Keys.P) && prevKeyboardState.IsKeyUp(Keys.P))
            {
                network.clientDiscoverHost(networkIP);
            }

            if (kbInput.IsKeyDown(Keys.O) && prevKeyboardState.IsKeyUp(Keys.O))
            {
                network.clientDiscoverLAN();
            }

            // Network Update Code
            if (network != null) 
                network.Update(gameTime);

            prevKeyboardState = kbInput;

            base.Update( gameTime );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            GraphicsDevice.Clear( Color.White );

            spriteBatch.Begin();

            spriteBatch.Draw( background, mainFrame, Color.White );
            spriteBatch.DrawString(spriteFont, networkMessage, new Vector2(10, 10), Color.Black);
            if (network != null)
                spriteBatch.DrawString(spriteFont, network.NetworkMessage, new Vector2(10, 100), Color.Green);
            spriteBatch.DrawString(spriteFont, "Host IP:", new Vector2(10, 50), Color.Red);
            if (IP != null)
                spriteBatch.DrawString(spriteFont, IP, new Vector2(10, 70), Color.Red);

            // Draw a property card based on the current drawId
            switch ( drawId )
            {
                case Props.LaScala:
                    spriteBatch.Draw( propLaScala, zoomPos, Color.White );
                    break;
                case Props.Bali:
                    spriteBatch.Draw( propBali, zoomPos, Color.White );
                    break;
                case Props.Chance1:
                    spriteBatch.Draw( chance1, zoomPos, Color.White );
                    break;
                case Props.TempleMount:
                    spriteBatch.Draw( propTempMount, zoomPos, Color.White );
                    break;
                case Props.DamnoenMarket:
                    spriteBatch.Draw( propDamnoenMart, zoomPos, Color.White );
                    break;
                case Props.GreatWall:
                    spriteBatch.Draw( propGreatWall, zoomPos, Color.White );
                    break;
                case Props.TajMahal:
                    spriteBatch.Draw( propTajMahal, zoomPos, Color.White );
                    break;
                case Props.StatueLiberty:
                    spriteBatch.Draw( propStatLiberty, zoomPos, Color.White );
                    break;
                case Props.Forever9:
                    spriteBatch.Draw( forever9, zoomPos, Color.White );
                    break;
                case Props.EiffelTower:
                    spriteBatch.Draw( propEiffel, zoomPos, Color.White );
                    break;
                case Props.Parthenon:
                    spriteBatch.Draw( propParthenon, zoomPos, Color.White );
                    break;
                default:
                    break;
            }

            spriteBatch.End();

            base.Draw( gameTime );
        }

        // Return a Vector2 indicating the position to draw a property card (or a sprite in general).
        private Vector2 makeTexturePos( Texture2D tex )
        {
            Vector2 v = new Vector2( mainFrame.Width / 2 - tex.Width / 2, mainFrame.Height / 2 - tex.Height / 2 );
            return v;
        }
    }
}
