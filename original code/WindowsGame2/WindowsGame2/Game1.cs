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
using WindowsGame2.Core;
        

        
namespace WindowsGame2
{       
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public static class GameAudio
    {
        public static AudioEngine audioEngine;
        public static WaveBank waveBank;
        public static SoundBank soundBank;

        public static Cue World1BGM;
        public static Cue World2BGM;
        public static Cue World3BGM;

        public static void Initialize()
        {
            World1BGM = soundBank.GetCue("world1BGM");
            World2BGM = soundBank.GetCue("world2BGM");
            World3BGM = soundBank.GetCue("world3BGM");
        }
    }

    public static class ControllerManager
    {
        public static PlayerIndex controllingPlayer = PlayerIndex.One;

        public static bool DetermineController()
        {
            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
            {
                if (GamePad.GetState(index).Buttons.Start == ButtonState.Pressed)
                {
                    controllingPlayer = index;
                    return true;
                    
                }
                
            }
            return false;
        }
    }
    
    public enum GameState { Intro, Menu, InGame, LevelTransition, Paused, GameOver };
    
        
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameEngine gameEngine;
        GameMenu gameMenu;
        GamerServicesComponent gamerServiceComponent;     
        Color menuColor = new Color(0,0,0,1);
       
        

        //Variables for the pause Menu
        public SpriteFont gameFont;
        private bool pauseToggle;
        private bool selectionToggle;
        private bool showQuitScreen;
        private bool quitGame;
        bool resume = true;

        public bool dimScreen;
        private bool oneTimeToggle = true;

        private Texture2D pixel;
        Color pauseColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);
        
        Color resumeColor = Color.White;
        float resumeScale = 1.2f;

        Color quitColor = Color.Gray;
        float quitScale = 1.0f;


        //Variables for level transition
        float alphaFade = 0.0f;
        int elapsedTime;
        public bool changeLevel = false;
        public static int levelNumber = 8;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.graphics.IsFullScreen = false;
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
           
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Guide.SimulateTrialMode = true;
            
            
            // TODO: Add your initialization logic here
            GameAudio.audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            
            
            GameAudio.waveBank = new WaveBank(GameAudio.audioEngine, @"Content\Audio\Wave Bank.xwb");
            GameAudio.soundBank = new SoundBank(GameAudio.audioEngine, @"Content\Audio\Sound Bank.xsb");

            GameAudio.Initialize();
             spriteBatch = new SpriteBatch(GraphicsDevice);

            Services.AddService(typeof(SpriteBatch), spriteBatch);

             // Create a new SpriteBatch, which can be used to draw textures.

            
            gameEngine = new GameEngine(this);
            gameEngine.LoadGameComponents(this,levelNumber);
            gameEngine.Enabled = false;
            gameEngine.Visible = false;
            Components.Add(gameEngine);
            gameMenu = new GameMenu(this);
            gameMenu.Initialize();
            gameMenu.Enabled = true;
            gameMenu.Visible = true;
            Components.Add(gameMenu);
            gamerServiceComponent = new GamerServicesComponent(this);
            Components.Add(gamerServiceComponent);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {                              

            pixel = Content.Load<Texture2D>("pixel");
            
            gameFont = Content.Load<SpriteFont>(@"Fonts\ChiMenu");

            //gameEngine.LoadGameComponents(this, levelNumber);

            base.LoadContent();
            
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
           
            
            switch (GameEngine.gameState)
            {
                case GameState.InGame:

                    if (gameEngine.player1.position.Y > 720 && !gameEngine.rollCredits)
                    {
                        gameEngine.player1.lives--;
                        gameEngine.player1.hasHat = false;
                        gameEngine.player1.coins = 0;
                        gameEngine.Visible = false;
                        gameEngine.Enabled = false;
                        GameEngine.gameState = GameState.LevelTransition;

                    }
                    
                    if (levelNumber == 8 && gameEngine.player1.position.Y >= 700 && gameEngine.player1.screenIndex == 22)
                    {
                        gameEngine.Visible = false;
                        gameEngine.Enabled = false;
                        changeLevel = true;
                        levelNumber++;
                        gameEngine.rollCredits = true;
                        GameEngine.gameState = GameState.LevelTransition;
                    }
                    if (gameEngine.player1.collisionRect.Right >= gameEngine.level.playableSurface.playableSectors[gameEngine.level.playableSurface.playableSectors.GetUpperBound(0)].SectorRec.Right)
                    {
                       
                            gameEngine.Visible = false;
                            gameEngine.Enabled = false;
                            changeLevel = true;
                            //gameEngine.Draw(gameTime);
                            levelNumber++;
                            GameEngine.gameState = GameState.LevelTransition;
                       
                    }
                    
                    
                    if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.Start == ButtonState.Pressed)
                        pauseToggle = true;
                    
                    if (pauseToggle)
                    {
                        dimScreen = true;
                        gameEngine.Enabled = false;
                        
                        if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.Start == ButtonState.Released)
                        {
                            pauseToggle = false;
                            GameEngine.gameState = GameState.Paused;
                        }
                     }                         
                                 
                break;
                

                case GameState.LevelTransition:
                    //TODO:add code to fade from level end to black screen with level number,
                    //load next level, then fade back into the next level.


                if (!(Guide.IsTrialMode && levelNumber > 1))
                {


                    elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

                    if (elapsedTime <= 1000 && alphaFade < 1.0f)
                    {

                        oneTimeToggle = true;
                        GameEngine.loadingMap = true;
                        alphaFade += 0.02f;
                    }
                    if (elapsedTime > 1000 && elapsedTime < 2000)
                    {
                        if (oneTimeToggle)
                        {
                            gameMenu.Visible = false;
                            Content.Unload();

                            LoadContent();

                            gameMenu.Initialize();

                            gameEngine.Enabled = true;

                            if (levelNumber >9 || gameEngine.player1.lives < 0)
                            {
                                GameEngine.gameState = GameState.GameOver;
                                gameEngine.player1.position.X = 0;
                                gameEngine.player1.Reset();
                                levelNumber = 0;
                            }
                            else
                            {
                                gameEngine.ManageLevelChanges(levelNumber);
                                
                            }

                            gameEngine.player1.isDying = false;

                            oneTimeToggle = false;

                        }

                    }
                    if (elapsedTime >= 2000 && elapsedTime < 3000)
                    {
                        if (GameEngine.loadingMap && elapsedTime > 2500)
                            GameEngine.loadingMap = false;


                        alphaFade -= 0.02f;
                    }
                    if (elapsedTime >= 3000)
                    {

                        elapsedTime = 0;
                        alphaFade = 0;


                        GameEngine.gameState = GameState.InGame;
                    }
                }
                break;
                

                case GameState.Intro:


                if (ControllerManager.DetermineController())
                {
                    gameMenu.showIntro = false;
                    GameEngine.gameState = GameState.Menu;
                }
                    
                break;
                

                case GameState.Menu:
                if (gameMenu.startGame)
                {
                    if (Guide.IsTrialMode)
                    {
                        levelNumber = 0;
                    }

                        gameMenu.startGame = false;
                        GameEngine.gameState = GameState.LevelTransition;
                    
                }
                break;
                

                case GameState.GameOver:

                gameEngine.rollCredits = false;
                    
                break;
                
                
                case GameState.Paused:
                    if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.Start == ButtonState.Pressed)
                         pauseToggle = true;
                       
                    if (pauseToggle)
                    {
                        dimScreen = false;
                        gameEngine.Enabled = true;
                        
                        if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.Start == ButtonState.Released)
                        {
                            pauseToggle = false;
                            showQuitScreen = false;
                            GameEngine.gameState = GameState.InGame;
                        }
                    }
                break;
            }

            GameAudio.audioEngine.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            
            switch (GameEngine.gameState)
            {
                case GameState.InGame:
                    if (!gameEngine.Visible)
                    {
                        gameEngine.Visible = true;
                        gameEngine.Enabled = true;
                        gameMenu.Visible = false;
                    }
                break;
                
                case GameState.LevelTransition:

                if (Guide.IsTrialMode && levelNumber > 1)
                {
                        Texture2D buyNow = Content.Load<Texture2D>(@"DemoMode/buyNow");
                        Texture2D levels = Content.Load<Texture2D>(@"DemoMode/levels");
                        Texture2D price = Content.Load<Texture2D>(@"DemoMode/price");

                        
                        elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                        
                        spriteBatch.Begin();

                        if (elapsedTime < 3000)
                        {
                            spriteBatch.Draw(buyNow, Vector2.Zero, Color.White);
                            
                        }

                        else if (elapsedTime >= 3000 && elapsedTime < 6000)
                        {
                            spriteBatch.Draw(levels, Vector2.Zero, Color.White);
                        }
                        else if (elapsedTime >= 6000 && elapsedTime < 9000)
                        {
                            spriteBatch.Draw(price, Vector2.Zero, Color.White);
                        }
                        else if (elapsedTime >= 9000)
                        {
                            Guide.ShowMarketplace(ControllerManager.controllingPlayer);
                            GameEngine.gameState = GameState.Menu;
                            elapsedTime = 0;
                        }
                        spriteBatch.End();
                    
                }
                else
                {
                    if (elapsedTime <= 1000)
                        gameEngine.Draw(gameTime);

                    if (elapsedTime >= 2000)
                        gameEngine.Draw(gameTime);

                    LevelTransition();
                }
                   
                break;
                
                case GameState.Intro:
                                                                                   
                
                

                break;


                case GameState.Menu:
                    if (!gameMenu.Visible)
                    {
                        gameMenu.Visible = true;
                        gameMenu.Enabled = true;
                    }
                    break;
                case GameState.GameOver:

                    GameOver();
                    elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

                    if (elapsedTime >= 4000)
                    {
                        elapsedTime = 0;
                        
                        
                        levelNumber = 0;
                        gameEngine.ManageLevelChanges(0);
                        GameEngine.gameState = GameState.Menu;
                        

                    }
                    break;
                case GameState.Paused:
                    gameEngine.Visible = false;
                    gameEngine.Draw(gameTime);
                    if (dimScreen)
                        PauseMenu();
                break;
            }
           
            
            
            base.Draw(gameTime);
           
        }

        private void PauseMenu()
        {
            spriteBatch.Begin();
            if (GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.Y >= 0.3f)
                resume = true;

            if (GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.Y <= -0.3f)
                resume = false;

            if (resume)
            {
                resumeColor = Color.White;
                resumeScale = 1.2f;

                quitColor = Color.Gray;
                quitScale = 1.0f;

                if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Pressed && !showQuitScreen)
                {
                    
                    selectionToggle = true;
                }

                if (selectionToggle)
                {
                    if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Released  && !showQuitScreen)
                    {
                        selectionToggle = false;
                        resume = true;
                        dimScreen = false;
                        GameEngine.gameState = GameState.InGame;
                    }
                }
            }
            else
            {
                resumeColor = Color.Gray;
                resumeScale = 1.0f;

                quitColor = Color.White;
                quitScale = 1.2f;

                if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Pressed  && !showQuitScreen)
                {

                    selectionToggle = true;
                }

                if (selectionToggle)
                {
                    if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Released && !showQuitScreen)
                    {
                        selectionToggle = false;
                        resume = true;
                        showQuitScreen = true;
                    }
                }
            }
            
            

            if (dimScreen)
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 1280, 720), pauseColor);
            
            spriteBatch.DrawString(gameFont, "Paused", new Vector2((Window.ClientBounds.Width / 2)
                                                                    - (gameFont.MeasureString("Paused").X / 2),
                                                                    200),
                                                                    Color.LightGray);

            
            spriteBatch.DrawString(gameFont, "Resume", new Vector2((Window.ClientBounds.Width / 2)
                                                                    - (gameFont.MeasureString("Resume").X / 2),
                                                                    300), resumeColor, 0.0f, new Vector2(0,0), resumeScale, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(gameFont, "Quit", new Vector2((Window.ClientBounds.Width / 2)
                                                                    - (gameFont.MeasureString("Quit").X / 2),
                                                                    400), quitColor, 0.0f, new Vector2(0, 0), quitScale, SpriteEffects.None, 0.0f);


            if (showQuitScreen)
               QuitMenu();

            spriteBatch.End();
           

        }

        private void QuitMenu()
        {
            spriteBatch.Draw(pixel, new Rectangle(0, 0, 1280, 720), Color.Black);

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

                if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Pressed)
                {

                    selectionToggle = true;
                }

                if (selectionToggle)
                {
                    if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Released)
                    {
                        quitGame = false;
                        showQuitScreen = false;
                        selectionToggle = false;
                        gameEngine.Enabled = false;
                        gameEngine.Visible = false;
                        gameMenu.Enabled = true;
                        gameMenu.Visible = true;
                        GameEngine.gameState = GameState.Menu;
                        
                    }
                }
            }
            else
            {
                resumeColor = Color.White;
                resumeScale = 1.2f;

                quitColor = Color.Gray;
                quitScale = 1.0f;

                if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Pressed)
                {

                    selectionToggle = true;
                }

                if (selectionToggle)
                {
                    if (GamePad.GetState(ControllerManager.controllingPlayer).Buttons.A == ButtonState.Released)
                    {
                        selectionToggle = false;
                        quitGame = false;
                        showQuitScreen = false;
                    }
                }
            }
            
            
            
            spriteBatch.DrawString(gameFont, "Are You Sure You Want To Quit?", new Vector2((Window.ClientBounds.Width / 2)
                                                                    - (gameFont.MeasureString("Are You Sure You Want To Quit?").X / 2),
                                                                    200),
                                                                    Color.LightGray);

            spriteBatch.DrawString(gameFont, "Yes", new Vector2((Window.ClientBounds.Width / 2)
                                                                        - (gameFont.MeasureString("Resume").X / 2),
                                                                        300), quitColor, 0.0f, new Vector2(0, 0), quitScale, SpriteEffects.None, 0.0f);

            spriteBatch.DrawString(gameFont, "No", new Vector2((Window.ClientBounds.Width / 2)
                                                                    - (gameFont.MeasureString("Quit").X / 2),
                                                                    400), resumeColor, 0.0f, new Vector2(0, 0), resumeScale, SpriteEffects.None, 0.0f);
        
        }

        private void LevelTransition()
        {
            
            spriteBatch.Begin();
            
            spriteBatch.Draw(pixel, new Rectangle(0, 0, 1280, 720), new Color(0.0f,0.0f,0.0f,alphaFade));

            if (levelNumber <= 8 && gameEngine.player1.lives >= 0)
            {
                spriteBatch.DrawString(gameFont, "Level " + (1 + levelNumber).ToString(), new Vector2((Window.ClientBounds.Width / 2)
                                                                        - (gameFont.MeasureString("Level " + (1 + levelNumber).ToString()).X / 2),
                                                                        200),
                                                                       new Color(1.0f, 1.0f, 1.0f, alphaFade));
            }
            spriteBatch.End();
        }

        private void GameOver()
        {

            spriteBatch.Begin();

            spriteBatch.Draw(pixel, new Rectangle(0, 0, 1280, 720), new Color(0.0f, 0.0f, 0.0f, alphaFade));

           
                spriteBatch.DrawString(gameFont, "Game Over", new Vector2((Window.ClientBounds.Width / 2)
                                                                        - (gameFont.MeasureString("Game Over").X / 2),
                                                                        300),
                                                                       new Color(1.0f, 1.0f, 1.0f, alphaFade));
            
            spriteBatch.End();
        }

        private void StartScreen()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(pixel, new Rectangle(0, 0, 1280, 720), new Color(0.0f, 0.0f, 0.0f, alphaFade));


            spriteBatch.DrawString(gameFont, "Press Start", new Vector2((Window.ClientBounds.Width / 2)
                                                                    - (gameFont.MeasureString("Press Start").X / 2),
                                                                    300),
                                                                   new Color(1.0f, 1.0f, 1.0f, alphaFade));

            spriteBatch.End();
        }
    }
}
