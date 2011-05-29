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
        Texture2D chance;
        Texture2D forever9;

        // An integer that determines which property card to show. 0 means no card is selected.
        int drawId = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            Window.Title = "SoshiLand";
            Window.AllowUserResizing = false;

            // Preferred window size is 640x640
            graphics.PreferredBackBufferHeight = 640;
            graphics.PreferredBackBufferWidth = 640;

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
            Console.Write(character);
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
            background = Content.Load<Texture2D>( "assets\\main_board" );
            mainFrame = new Rectangle( 0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height );

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
            chance = Content.Load<Texture2D>( "assets\\chance" );
            forever9 = Content.Load<Texture2D>( "assets\\forever9" );
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

            prevKeyboardState = kbInput;

            MouseState ms = Mouse.GetState();

            // Set drawId based on the mouse position when left-clicked
            if ( ms.LeftButton == ButtonState.Pressed )
            {
                if ( ms.Y <= 75 )
                {
                    if ( ms.X >= 74 )
                    {
                        if ( ms.X <= 120 )
                            drawId = 1;
                        else if ( ms.X <= 164 )
                            drawId = 2;
                        else if ( ms.X <= 208 )
                            drawId = 3;
                        else if ( ms.X <= 252 )
                            drawId = 4;
                        else if ( ms.X <= 298 )
                            drawId = 5;
                        else if ( ms.X <= 343 )
                            drawId = 6;
                        else if ( ms.X <= 388 )
                            drawId = 7;
                        else if ( ms.X <= 433 )
                            drawId = 8;
                        else if ( ms.X <= 478 )
                            drawId = 9;
                        else if ( ms.X <= 522 )
                            drawId = 10;
                        else if ( ms.X <= 566 )
                            drawId = 11;
                    }
                }
            }

            // drawId is set to 0 when right clicked
            if ( ms.RightButton == ButtonState.Pressed )
                drawId = 0;

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

            // Draw a property card based on the current drawId
            switch ( drawId )
            {
                case 1:
                    spriteBatch.Draw( propLaScala, makeTexturePos( propLaScala ), Color.White );
                    break;
                case 2:
                    spriteBatch.Draw( propBali, makeTexturePos( propBali ), Color.White );
                    break;
                case 3:
                    spriteBatch.Draw( chance, makeTexturePos( chance ), Color.White );
                    break;
                case 4:
                    spriteBatch.Draw( propTempMount, makeTexturePos( propTempMount ), Color.White );
                    break;
                case 5:
                    spriteBatch.Draw( propDamnoenMart, makeTexturePos( propDamnoenMart ), Color.White );
                    break;
                case 6:
                    spriteBatch.Draw( propGreatWall, makeTexturePos( propGreatWall ), Color.White );
                    break;
                case 7:
                    spriteBatch.Draw( propTajMahal, makeTexturePos( propTajMahal ), Color.White );
                    break;
                case 8:
                    spriteBatch.Draw( propStatLiberty, makeTexturePos( propStatLiberty ), Color.White );
                    break;
                case 9:
                    spriteBatch.Draw( forever9, makeTexturePos( forever9 ), Color.White );
                    break;
                case 10:
                    spriteBatch.Draw( propEiffel, makeTexturePos( propEiffel ), Color.White );
                    break;
                case 11:
                    spriteBatch.Draw( propParthenon, makeTexturePos( propParthenon ), Color.White );
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
