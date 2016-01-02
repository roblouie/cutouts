using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using LevelEditor;
using System.Xml;
using Microsoft.Xna.Framework.Audio;
using WindowsGame2.Core.EnemyTypes;
using Microsoft.Xna.Framework.GamerServices;

//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace WindowsGame2.Core
{
    
    
    class GameEngine : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public bool rollCredits = false;
 
        public static GameState gameState = GameState.Intro;

        public static bool loadingMap = true;
        
        private SpriteBatch sBatch;
        
        public Player player1 = new Player(new Vector2(100,100));
        private Texture2D[] playerTextures = new Texture2D[2];

        private Camera camera = new Camera(1280, 720);
        

        private Hud gameHud;
        private EffectsEngine effectsEngine = new EffectsEngine();


        public Level level = new Level();

        private string[] levelDataFiles = new string[] { @"Levels\World1\Level1", @"Levels\World1\Level2", @"Levels\World1\Level3",
                                                         @"Levels\World2\Level4", @"Levels\World2\Level5",@"Levels\World2\Level6",
                                                         @"Levels\World3\Level7", @"Levels\World3\Level8", @"Levels\World3\Level9",
                                                         @"Levels\TheEnd\Level10" };
                                                         

        private List<EnemyData> enemies;

        public GameEngine(Game game)
            : base(game)
        {
            
            sBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
                        
        }

        public void LoadGameComponents(Game game, int levelToLoad)
        {
            
            
            gameHud = new Hud(game, @"Player1\livesMarker", @"Player1\collectible", @"Fonts\chiHUD", Color.White,
                new Vector2(-540, -300), new Vector2(440, -300));
                  

            player1.Load(game, new Vector2(25000,100));
                     
           
            effectsEngine.Initialize(game);

            camera = new Camera(1280, 720);

            level.backgroundLayers = new List<Layer>();
            
            //World 1 Backgrounds
            if (levelToLoad <= 2 || levelToLoad == 9)
            {
                if (!GameAudio.World1BGM.IsPlaying)
                {
                    GameAudio.World1BGM = GameAudio.soundBank.GetCue("world1BGM");    
                    GameAudio.World1BGM.Play();
                    GameAudio.World2BGM.Stop(AudioStopOptions.Immediate);
                    GameAudio.World3BGM.Stop(AudioStopOptions.Immediate);
                   
                }
                level.AddBackgroundLayer(game, @"Backgrounds\World1\skyBackground", new Vector2(0.0f, 0.0f), new Vector2(-camera.X, -camera.Y));
                level.AddBackgroundLayer(game, @"Backgrounds\World1\backgroundLayer4", new Vector2(0.3f, 0.0f), new Vector2((-camera.X * 0.7f), -camera.Y));
                level.AddBackgroundLayer(game, @"Backgrounds\World1\backgroundLayer3", new Vector2(0.5f, 0.0f), new Vector2((-camera.X * 0.5f), -camera.Y));
                level.AddBackgroundLayer(game, @"Backgrounds\World1\backgroundLayer2", new Vector2(0.7f, 0.0f), new Vector2((-camera.X * 0.3f), -camera.Y));
            }
            //World 2 Backgrounds
            else if (levelToLoad > 2 && levelToLoad <= 5)
            {
                if (!GameAudio.World2BGM.IsPlaying)
                {
                    GameAudio.World2BGM = GameAudio.soundBank.GetCue("world2BGM");   
                    GameAudio.World2BGM.Play();
                    GameAudio.World1BGM.Stop(AudioStopOptions.Immediate);
                    GameAudio.World3BGM.Stop(AudioStopOptions.Immediate);
                }
                level.AddBackgroundLayer(game, @"Backgrounds\World2\mountainsSky", new Vector2(0.0f, 0.0f), new Vector2(-camera.X,-camera.Y));
                level.AddBackgroundLayer(game, @"Backgrounds\World2\mountains3", new Vector2(0.3f, 0.0f), new Vector2((-camera.X * 0.7f), -camera.Y));
                level.AddBackgroundLayer(game, @"Backgrounds\World2\mountains2", new Vector2(0.5f, 0.0f), new Vector2((-camera.X * 0.5f), -camera.Y));
                level.AddBackgroundLayer(game, @"Backgrounds\World2\mountains1", new Vector2(0.7f, 0.0f), new Vector2((-camera.X * 0.3f), -camera.Y));
            }
            //World 3 Backgrounds
            else if (levelToLoad > 5 && levelToLoad <= 8)
            {
                if (!GameAudio.World3BGM.IsPlaying)
                {
                    GameAudio.World3BGM = GameAudio.soundBank.GetCue("world3BGM");   
                    GameAudio.World3BGM.Play();
                    GameAudio.World1BGM.Stop(AudioStopOptions.Immediate);
                    GameAudio.World2BGM.Stop(AudioStopOptions.Immediate);
                }
                level.AddBackgroundLayer(game, @"Backgrounds\World3\nightSky", new Vector2(0.0f, 0.0f), new Vector2(-camera.X, -camera.Y));
                level.AddBackgroundLayer(game, @"Backgrounds\World3\farStars", new Vector2(0.3f, 0.0f), new Vector2((-camera.X * 0.7f), -camera.Y));
                level.AddBackgroundLayer(game, @"Backgrounds\World3\middleStars", new Vector2(0.5f, 0.0f), new Vector2((-camera.X * 0.5f), -camera.Y));
                level.AddBackgroundLayer(game, @"Backgrounds\World3\closeStars", new Vector2(0.7f, 0.0f), new Vector2((-camera.X * 0.3f), -camera.Y));
            }

            level.playableSurface = new PlayableSurface();            
            level.playableSurface.LoadContent(Game.Content.Load <PlayableSurface>(levelDataFiles[levelToLoad]));
            level.LoadPlayableLayer(Game);
            LoadEnemies();

            for (int i = 0; i < level.playableSurface.hatPositions.Count(); i++)
            {
                effectsEngine.AddHat(level.playableSurface.hatPositions[i], false);
            }

                        
        }

        private void LoadEnemies()
        {
            for (int i = 0; i < level.playableSurface.playableSectors.Count(); i++)
            {
                for (int j = 0; j < level.playableSurface.playableSectors[i].enemyPositions.Count(); j++)
                {
                    switch (level.playableSurface.playableSectors[i].enemyPositions[j].enemyType)
                    {
                        case EnemyType.GroundCrawler:
                            level.playableSurface.playableSectors[i].enemyPositions[j] = new GroundCrawler(level.enemyTextures[level.playableSurface.playableSectors[i].enemyPositions[j].enemyTexture], level.playableSurface.playableSectors[i].enemyPositions[j].position);        
                            break;

                        case EnemyType.Bee:
                            level.playableSurface.playableSectors[i].enemyPositions[j] = new Bee(level.enemyTextures[level.playableSurface.playableSectors[i].enemyPositions[j].enemyTexture], level.playableSurface.playableSectors[i].enemyPositions[j].position);
                            break;

                        case EnemyType.Rabbit:
                            level.playableSurface.playableSectors[i].enemyPositions[j] = new Rabbit(level.enemyTextures[level.playableSurface.playableSectors[i].enemyPositions[j].enemyTexture], level.playableSurface.playableSectors[i].enemyPositions[j].position);
                            break;
                        
                        case EnemyType.Bat:
                            level.playableSurface.playableSectors[i].enemyPositions[j] = new Bat(level.enemyTextures[level.playableSurface.playableSectors[i].enemyPositions[j].enemyTexture], level.playableSurface.playableSectors[i].enemyPositions[j].position);
                            break;

                        case EnemyType.GroundJumper:
                            level.playableSurface.playableSectors[i].enemyPositions[j] = new GroundJumper(level.enemyTextures[level.playableSurface.playableSectors[i].enemyPositions[j].enemyTexture], level.playableSurface.playableSectors[i].enemyPositions[j].position);
                            break;
                        
                        case EnemyType.RockMan:
                            level.playableSurface.playableSectors[i].enemyPositions[j] = new RockMan(level.enemyTextures[level.playableSurface.playableSectors[i].enemyPositions[j].enemyTexture], level.playableSurface.playableSectors[i].enemyPositions[j].position);
                            break;
                    }
                    
                }
            }
            
          
        }

        private void DrawBackgrounds(SpriteBatch spriteBatch)
        {
            foreach (Layer backgroundLayer in level.backgroundLayers)
            {
                Vector2 oldCameraPosition = camera.Position;
                camera.Position *= backgroundLayer.zDistance;

                sBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None, camera.matrix);

                backgroundLayer.Draw(sBatch, camera.Position);

                camera.Position = oldCameraPosition;

                sBatch.End();
            }
        }

        public override void Draw(GameTime gameTime)
        {


            DrawBackgrounds(sBatch);
            
            
            sBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None, camera.matrix);

            level.DrawPlayableLayer(gameTime, sBatch, player1.currentScreen, player1.screenIndex);


            effectsEngine.Draw(sBatch);

            player1.Draw(gameTime, sBatch);
            
            //DEBUG
            //player1.DrawDebug(Game.Content.Load<Texture2D>("pixel"), sBatch, new Color(1.0f,1.0f,1.0f,0.5f));    
            
            //DEBUG HUD
            //gameHud.DrawHud(sBatch, camera.Position, player1.nextSection.ToString(), player1.screenIndex.ToString());


            gameHud.DrawHud(sBatch, camera.Position, player1.lives.ToString(), player1.coins.ToString());

           sBatch.End();
           

            base.Draw(gameTime);
        }
        public override void Update(GameTime gameTime)
        {
            // ManageLevelChanges();
            HandleCoins();
            HandleEnemies();
            ManageCamera();
           


            effectsEngine.Update(gameTime,level.playableSurface.playableSectors);              
            
            
            // TODO: Add your update code here
            if(player1.screenIndex<= level.playableSurface.playableSectors.GetUpperBound(0))
                player1.Update(gameTime,level.playableSurface);

            ManagePuffBalls();

            level.Update(gameTime, level.playableSurface.playableSectors, player1.currentScreen, player1.screenIndex, player1.collisionRect);

           

            base.Update(gameTime);
        }

        public void ManagePuffBalls()
        {

            //TODO: write code for player holding/throwing/being hit by puff balls

            for (int i = 0; i < effectsEngine.puffBalls.Count(); i++)
            {
               
                
                if (effectsEngine.puffBalls[i].isHeld)
                {
                    effectsEngine.puffBalls[i].velocity = Vector2.Zero;

                    

                    if (player1.isFacingRight)
                    {
                        effectsEngine.puffBalls[i].position.X = player1.collisionRect.Center.X + effectsEngine.puffBalls[i].collisionRec.Width - 6 - effectsEngine.puffBalls[i].origin.X;
                        effectsEngine.puffBalls[i].position.Y = player1.collisionRect.Center.Y - effectsEngine.puffBalls[i].origin.Y;
                    }
                    else
                    {
                        effectsEngine.puffBalls[i].position.X = player1.collisionRect.Center.X - effectsEngine.puffBalls[i].collisionRec.Width - (effectsEngine.puffBalls[i].collisionRec.Width * 0.1f) - effectsEngine.puffBalls[i].origin.X + 4;
                        effectsEngine.puffBalls[i].position.Y = player1.collisionRect.Center.Y - effectsEngine.puffBalls[i].origin.Y;
                    }


                    if (player1.isGrabbing == false)
                    {
                        player1.isHolding = false;

                        if (player1.isFacingRight)
                        {
                            effectsEngine.puffBalls[i].ThrowRight();
                          
                        }
                        else
                        {
                            effectsEngine.puffBalls[i].ThrowLeft();
                          
                        }
                    }

                   
                }
                
                if (player1.collisionRect.Intersects(effectsEngine.puffBalls[i].collisionRec) && player1.isGrabbing && !player1.isHolding)
                {
                    effectsEngine.puffBalls[i].isHeld = true;
                    effectsEngine.puffBalls[i].velocity = Vector2.Zero;

                    player1.isHolding = true;

                }
               
            }

        }

        private void EndingEvent()
        {
            if (GameAudio.World3BGM.IsPlaying)
            {
                GameAudio.World3BGM.Stop(AudioStopOptions.AsAuthored);
            }
            
            if (player1.position.X <= level.playableSurface.playableSectors[level.playableSurface.playableSectors.GetUpperBound(0)].SectorRec.X)
            {
                player1.position.X = level.playableSurface.playableSectors[level.playableSurface.playableSectors.GetUpperBound(0)].SectorRec.X;
            }

            if (camera.X < level.playableSurface.playableSectors[level.playableSurface.playableSectors.GetUpperBound(0)].SectorRec.Center.X)
            {
                camera.X += 1;
            }
            else
                camera.X = level.playableSurface.playableSectors[level.playableSurface.playableSectors.GetUpperBound(0)].SectorRec.Center.X;
        }

        public void ManageCamera()
        {
            

            if (Game1.levelNumber == 8 && player1.screenIndex == 22)
            {
                EndingEvent();
            }
            else
            {
                if (player1.playerCenter < 640)
                    camera.X = 640;
                else if (player1.playerCenter >= level.playableSurface.playableSectors[level.playableSurface.playableSectors.GetUpperBound(0)].SectorRec.Center.X)
                    camera.X = level.playableSurface.playableSectors[level.playableSurface.playableSectors.GetUpperBound(0)].SectorRec.Center.X;
                else
                    camera.X = player1.playerCenter;
            }
        }

        private void HandleCoins()
        {
            if (player1.coins >= 100)
            {
                GameAudio.soundBank.PlayCue("newLife");
                player1.lives++;
                player1.coins = 0;
            }

            for (int i = 0; i < effectsEngine.hats.Count(); i++)
            {
                Vector2 hatPosition = effectsEngine.hats[i].position + effectsEngine.hats[i].centerOffset;
                
                if ((hatPosition.X < (player1.collisionRect.Right + 20) && hatPosition.X > (player1.collisionRect.Left-20) &&
                        hatPosition.Y > (player1.collisionRect.Top - 20) && hatPosition.Y < (player1.collisionRect.Bottom + 20)) && !effectsEngine.hats[i].isLost)
                {
                    if (player1.hasHat == false)
                    {
                        player1.hasHat = true;
                        effectsEngine.hats[i].isDead = true;
                    }
                    else
                        effectsEngine.hats[i].isLost = true;
                }
            }

            if (player1.screenIndex <= level.playableSurface.playableSectors.GetUpperBound(0))
            {
                for (int i = 0; i < level.playableSurface.playableSectors[player1.screenIndex].collectiblePositions.Count; i++)
                {
                    Vector2 coin = level.playableSurface.playableSectors[player1.screenIndex].collectiblePositions[i];

                    coin += level.coinCenterOffset;

                    if (coin.X < (player1.collisionRect.Right + 20) && coin.X > (player1.collisionRect.Left - 20) &&
                        coin.Y > (player1.collisionRect.Top - 20) && coin.Y < (player1.collisionRect.Bottom + 20))
                    {
                        level.playableSurface.playableSectors[player1.screenIndex].collectiblePositions.RemoveAt(i);
                        GameAudio.soundBank.PlayCue("bloop");
                        player1.coins += 1;
                    }
                }
            }
        }

        private void HandleEnemies()
        {
            //TODO: add enemy<->player collision detection here!!
            if (player1.screenIndex < level.playableSurface.playableSectors.Count())
            {
                enemies = level.playableSurface.playableSectors[player1.screenIndex].enemyPositions;
                EnemyCollision(enemies);
            }

            if (level.nextSection < level.playableSurface.playableSectors.Count() && level.nextSection >=0)
            {
                enemies = level.playableSurface.playableSectors[level.nextSection].enemyPositions;
                EnemyCollision(enemies);
            }
            
            

        }

        private void EnemyCollision(List<EnemyData> enemies)
        {

            for (int i = 0; i < enemies.Count(); i++)
            {
                //enemy / projectile collision
                for (int j = 0; j < effectsEngine.puffBalls.Count(); j++)
                {
                    if (enemies.Count() > i && effectsEngine.puffBalls[j].collisionRec.Intersects(enemies[i].collisionRect) && (effectsEngine.puffBalls[j].isHeld || effectsEngine.puffBalls[j].velocity.X != 0))
                    {
                        effectsEngine.AddExplosion(enemies[i].position);
                        GameAudio.soundBank.PlayCue("enemyDeath");
                        if(!(enemies[i] is RockMan))
                            enemies.RemoveAt(i);

                        if (effectsEngine.puffBalls[j].isHeld && effectsEngine.puffBalls[j].lifeTime <= 1)
                            player1.isHolding = false;

                        effectsEngine.puffBalls[j].Hit();
                    }
                }
            }
            
            for (int i = 0; i < enemies.Count(); i++)
            {
               
                
                //enemy / player collision
                if (player1.collisionRect.Intersects(enemies[i].collisionRect) && player1.isDying == false)
                {
                    if (player1.isFalling && player1.collisionRect.Bottom < enemies[i].collisionRect.Center.Y)
                    {
                        effectsEngine.AddExplosion(enemies[i].position);
                        GameAudio.soundBank.PlayCue("enemyDeath");
                        
                        if (enemies[i] is Rabbit)
                            effectsEngine.AddPuffBall(enemies[i].position);


                        if (!(enemies[i] is RockMan))
                            enemies.RemoveAt(i);
                        

                        player1.enemyBounce = true;
                    }
                    else
                    {
                        if (player1.collisionRect.Intersects(enemies[i].killRect))
                        {
                            if (player1.hasHat)
                            {
                                GameAudio.soundBank.PlayCue("playerDeath");
                                player1.LoseHat();
                                effectsEngine.AddHat(player1.hatPosition,true);
                            }
                            else if (player1.isInvincible == false)
                            {
                                GameAudio.soundBank.PlayCue("playerDeath");
                                player1.enemyBounce = true;
                                player1.isDying = true;
                            }
                        }

                    }
                }

            }
        }

        

        public void ManageLevelChanges(int levelToChangeTo)
        {
            
            LoadGameComponents(Game,levelToChangeTo);
            
            //level.playableSurface = Game.Content.Load<PlayableSurface>(levelDataFiles[levelToChangeTo]);
            //level.LoadPlayableLayer(Game);
        }

        
        public static Vector2 GetIntersectionDepth(Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
    }
    
}
