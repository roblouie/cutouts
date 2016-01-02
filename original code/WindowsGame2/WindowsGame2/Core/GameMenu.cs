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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace WindowsGame2.Core
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GameMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private GamePadState previousGamePadState;
        private Texture2D backgroundImage;
        private Texture2D onePlayer;
        private Texture2D twoPlayer;
        private Texture2D exit;
        private Texture2D helpScreen;
        private Texture2D buy;
        private Texture2D buyHighlighted;
        private Texture2D buySelected;

        private SpriteFont menuFont;
        private Color selectedColor = Color.White;
        private Color normalColor = Color.Gray;
       
        private string[] menuItems = { "Buy", "Start", "Help","Exit Game" };
        private Vector2[] menuItemLocations = { new Vector2(300, 200), new Vector2(300, 300) };
        private int selectedMenuItem = 0;

        public bool startGame = false;
        private bool exitGame = false;

        private bool selectionToggle = false;
        private bool quitGame = false;
        private bool showHelpScreen = false;
        public bool showIntro = true;
        private float elapsedTime = 0.0f;

        Color pauseColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);

        Color resumeColor = Color.White;
        float resumeScale = 1.2f;

        Color quitColor = Color.Gray;
        float quitScale = 1.0f;

        public GameMenu(Game game)
            : base(game)
        {
            // TODO: Construct any child components here 
           
        }

        protected override void LoadContent()
        {
            

            
            
            base.LoadContent();
        }
          
        
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            backgroundImage = Game.Content.Load<Texture2D>(@"MainMenu\OpeningScreen");
            onePlayer = Game.Content.Load<Texture2D>(@"MainMenu\1Player");
            twoPlayer = Game.Content.Load<Texture2D>(@"MainMenu\2Player");
            exit = Game.Content.Load<Texture2D>(@"MainMenu\Exit");

            helpScreen = Game.Content.Load<Texture2D>(@"MainMenu\helpScreen");

            menuFont = Game.Content.Load<SpriteFont>(@"Fonts\chiMenu");

            if (Guide.IsTrialMode)
            {
                                
                buy = Game.Content.Load<Texture2D>(@"MainMenu\buy");
                buyHighlighted = Game.Content.Load<Texture2D>(@"MainMenu\buyHighlighted");
                buySelected = Game.Content.Load<Texture2D>(@"MainMenu\buySelected");
            }

            

            // TODO: Add your initialization code here

            base.Initialize();
        }
        
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            
            if (!showHelpScreen && !showIntro)
            {
                
                
                if (GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.Y >= 0.2f && previousGamePadState.ThumbSticks.Left.Y <= 0.2f && !exitGame)
                    selectedMenuItem--;

                if (GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.Y <= -0.2f && previousGamePadState.ThumbSticks.Left.Y >= -0.2f && !exitGame)
                    selectedMenuItem++;

                if (!Guide.IsTrialMode)
                {
                    if (selectedMenuItem == 0)
                        selectedMenuItem = 1;

                }
                
                if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Pressed && previousGamePadState.Buttons.A == ButtonState.Released && !exitGame)
                {
                    if (selectedMenuItem == 0)
                    {
                        if (Guide.IsTrialMode)
                        {
                            Guide.ShowMarketplace(ControllerManager.controllingPlayer);
                        }
                    }
                    
                    if (selectedMenuItem == 1)
                    {
                        GameAudio.soundBank.PlayCue("newLife");
                        startGame = true;
                        this.Enabled = false;
                        //this.Visible = false;
                    }

                    if (selectedMenuItem == 2)
                    {
                        showHelpScreen = true;
                    }

                    if (selectedMenuItem == 3)
                    {
                        exitGame = true;
                    }
                }
            }
            else if (showHelpScreen)
            {
                if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.B == ButtonState.Pressed)
                    showHelpScreen = false;
            }
           
            if (selectedMenuItem < 0)
                selectedMenuItem = menuItems.GetUpperBound(0);

            if (selectedMenuItem > menuItems.GetUpperBound(0))
                selectedMenuItem = 0;

            previousGamePadState = GamePad.GetState(ControllerManager.controllingPlayer);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            
            spriteBatch.Begin();

            spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, 1280, 720), Color.White);

            if (Guide.IsTrialMode)
            {
                spriteBatch.Draw(buy, new Vector2(0, 80), Color.White);
            }

            if (selectedMenuItem == 0 && Guide.IsTrialMode)
            {
                spriteBatch.Draw(buyHighlighted, new Vector2(0, 80), Color.White);
                spriteBatch.Draw(buySelected, Vector2.Zero, Color.White);
            }
            if (selectedMenuItem == 1)
                spriteBatch.Draw(onePlayer, Vector2.Zero, Color.White);

            if (selectedMenuItem == 2)
                spriteBatch.Draw(twoPlayer, Vector2.Zero, Color.White);

            if (selectedMenuItem == 3)
                spriteBatch.Draw(exit, Vector2.Zero, Color.White);

            if (showHelpScreen)
                spriteBatch.Draw(helpScreen, Vector2.Zero, Color.White);

            if (showIntro)
                StartScreen();
            
            if (exitGame)
                QuitMenu();

            
            
            spriteBatch.End();
                
            base.Draw(gameTime);
        }

        public void StartScreen()
        {
           

            spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, 1280, 720), new Color(0.0f, 0.0f, 0.0f, 0.8f));


            spriteBatch.DrawString(menuFont, "Press Start", new Vector2((Game.Window.ClientBounds.Width / 2)
                                                                    - (menuFont.MeasureString("Press Start").X / 2),
                                                                    300),
                                                                   new Color(1.0f, 1.0f, 1.0f, 1.0f));

           
        }

        private void QuitMenu()
        {
            spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, 1280, 720), new Color(0.0f,0.0f,0.0f,0.8f));

            if (GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.Y >= 0.3f)
                quitGame = true;

            if (GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.Y <= -0.3f)
                quitGame = false;

            if (quitGame)
            {
                resumeColor = Color.Gray;
                resumeScale = 1.0f;

                quitColor = Color.White;
                quitScale = 1.2f;

                if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Released)
                {

                    selectionToggle = true;
                }

                if (selectionToggle)
                {
                    if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Pressed)
                    {
                                             
                        Game.Exit();

                    }
                }
            }
            else
            {
                resumeColor = Color.White;
                resumeScale = 1.2f;

                quitColor = Color.Gray;
                quitScale = 1.0f;

                if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Released)
                {

                    selectionToggle = true;
                }

                if (selectionToggle)
                {
                    if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Pressed)
                    {
                        selectionToggle = false;

                        exitGame = false;
                    }
                }
            }

            

            spriteBatch.DrawString(menuFont, "Are You Sure You Want To Exit?", new Vector2((Game.Window.ClientBounds.Width / 2)
                                                                    - (menuFont.MeasureString("Are You Sure You Want To Exit?").X / 2),
                                                                    200),
                                                                    Color.White);

            spriteBatch.DrawString(menuFont, "Yes", new Vector2((Game.Window.ClientBounds.Width / 2)
                                                                        - (menuFont.MeasureString("Yes").X / 2),
                                                                        300), quitColor, 0.0f, new Vector2(0, 0), quitScale, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(menuFont, "No", new Vector2((Game.Window.ClientBounds.Width / 2)
                                                                    - (menuFont.MeasureString("No").X / 2),
                                                                    400), resumeColor, 0.0f, new Vector2(0, 0), resumeScale, SpriteEffects.None, 0.0f);

        }
    }
}