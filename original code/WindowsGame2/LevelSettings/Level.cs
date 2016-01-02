using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace LevelEditor
{
    public enum EnemyType { GroundCrawler,Bee,Rabbit,Bat,GroundJumper,RockMan };
    
    public class Level
    {
        public PlayableSurface playableSurface;
        public List<Layer> backgroundLayers = new List<Layer>();
        Texture2D[] playableArt;
        public List<Texture2D> enemyTextures; 
        
        Texture2D coin;
        public Vector2 coinCenterOffset = new Vector2(28, 28);
        
        int playableSections=0;
        public int nextSection = 0;
        Layer foregroundLayer;

        private enum collisionState { killPlayer, killEnemy, noCollision };

        //DEBUG
        Texture2D collisionTest;
        Color debugColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
               
        public void AddBackgroundLayer(Game game, string TextureFile, Vector2 zDistance)
        {
            backgroundLayers.Add(new Layer(game.Content.Load<Texture2D>(TextureFile),
                                           zDistance));

        }

        public void AddBackgroundLayer(Game game, string TextureFile, Vector2 zDistance, Vector2 location)
        {
            backgroundLayers.Add(new Layer(game.Content.Load<Texture2D>(TextureFile),zDistance,location));
        }
        

       // public void LoadEnemyTextures(Game game)
       // {
       //     foreach (string s in playableSurface.enemyFiles)
       //     {
       //         enemyTextures.Add(game.Content.Load<Texture2D>(s));
       //     }
       // }
        private void LoadEnemyArt(Game game)
        {
            enemyTextures = new List<Texture2D>();

            foreach (string s in playableSurface.enemyFiles)
            {
                enemyTextures.Add(game.Content.Load<Texture2D>(s));
            }

          

        }

        public void LoadPlayableLayer(Game game)
        {
            playableSections = 0;
            LoadEnemyArt(game);

            foreach (string s in playableSurface.artFiles)
            {
                playableSections++;
            }

            playableArt = new Texture2D[playableSections];


            for (int i = 0; i <= playableArt.GetUpperBound(0); i++)
            {
                playableArt[i] = game.Content.Load<Texture2D>(playableSurface.artFiles[i]);
            }

            for (int i = 0; i <= playableSurface.playableSectors.GetUpperBound(0); i++)
            {
                for (int j = 0; j < playableSurface.playableSectors[i].artPieces.Count(); j++)
                {
                    playableSurface.playableSectors[i].artPieces[j].SetTexture(ref playableArt[playableSurface.playableSectors[i].artPieces[j].type]);
                }
            }

            coin = game.Content.Load<Texture2D>("coin");
            
            //DEBUG
            collisionTest = game.Content.Load<Texture2D>("pixel");

           // playableLayer = new Layer(game.Content.Load<Texture2D>(TextureFile),new Point(1,1));

        }

        public void AddForegroundLayer(Game game, string TextureFile)
        {
            foregroundLayer = new Layer(game.Content.Load<Texture2D>(TextureFile), new Vector2(2,1));
        }

       
        
        private void UpdateEnemies(GameTime gameTime,PlayableSector[] playableSector, int screenIndex, int nextSection, Rectangle playerHitBox)
        {
            
            
            
            if (screenIndex <= playableSurface.playableSectors.GetUpperBound(0))
            {
                                              
                if (nextSection >= 0 && nextSection < playableSurface.playableSectors.Count())
                {
                    for (int i = 0; i < playableSurface.playableSectors[nextSection].enemyPositions.Count(); i++)
                    {
                                               
                        playableSurface.playableSectors[nextSection].enemyPositions[i].Update(gameTime, playableSector, playerHitBox);


                        //change so that this resets enemy to original position
                        if (playableSurface.playableSectors[nextSection].enemyPositions[i].GetRightSideIndex() > playableSurface.playableSectors[nextSection].enemyPositions[i].GetOwnedScreen())
                        {

                            //playableSurface.playableSectors[nextSection].enemyPositions[i].ResetEnemy();
                            playableSurface.TravelEnemyForward(playableSurface.playableSectors[nextSection].enemyPositions[i], i);

                        }

                       

                    }
                    for (int i = 0; i < playableSurface.playableSectors[nextSection].enemyPositions.Count(); i++)
                    {
                        if (playableSurface.playableSectors[nextSection].enemyPositions[i].GetLeftSideIndex() < playableSurface.playableSectors[nextSection].enemyPositions[i].GetOwnedScreen())
                        {
                            playableSurface.TravelEnemyBackward(playableSurface.playableSectors[nextSection].enemyPositions[i], i);
                        }
                    }
                }


                for (int i = 0; i < playableSurface.playableSectors[screenIndex].enemyPositions.Count(); i++)
                {
                    playableSurface.playableSectors[screenIndex].enemyPositions[i].Update(gameTime, playableSector, playerHitBox);
                                                           
                    if (playableSurface.playableSectors[screenIndex].enemyPositions[i].GetLeftSideIndex() > playableSurface.playableSectors[screenIndex].enemyPositions[i].GetOwnedScreen())
                    {
                       playableSurface.TravelEnemyForward(playableSurface.playableSectors[screenIndex].enemyPositions[i], i);
                    }

                }
                for (int i = 0; i < playableSurface.playableSectors[screenIndex].enemyPositions.Count(); i++)
                {
                    if (playableSurface.playableSectors[screenIndex].enemyPositions[i].GetRightSideIndex() < playableSurface.playableSectors[screenIndex].enemyPositions[i].GetOwnedScreen())
                    {
                        playableSurface.TravelEnemyBackward(playableSurface.playableSectors[screenIndex].enemyPositions[i], i);
                    }
                }
                
            }

        }

        public void Update(GameTime gameTime, PlayableSector[] playableSector, float currentScreen, int screenIndex, Rectangle playerHitBox)
        {
            //TODO rewrite so Update takes a player as a parameter, then extracts the screens and X,Y position
            //for collisoin
            nextSection = (int)Math.Round(currentScreen);
                                    
            if (nextSection == screenIndex)
                nextSection--;

            //if (nextSection == -1)
            //    nextSection = 0;

            UpdateEnemies(gameTime,playableSector, screenIndex, nextSection, playerHitBox);
        }

        
        /// <summary>
        /// Draws the surface the player interacts on, should be called after
        /// DrawBackgroundLayers and before DrawForeground.
        /// </summary>
        public void DrawPlayableLayer(GameTime gameTime, SpriteBatch sBatch, float currentScreen, int screenIndex)
        {
            nextSection = (int)Math.Round(currentScreen);

            if (nextSection == screenIndex)
                nextSection--;

            //sBatch.Draw(playableArt[screenIndex], playableSurface.playableSectors[screenIndex].SectorRec,
            //                                       Color.White);

            if (screenIndex <= playableSurface.playableSectors.GetUpperBound(0))
            {
                for (int i = 0; i < playableSurface.playableSectors[screenIndex].artPieces.Count(); i++)
                {
                    playableSurface.playableSectors[screenIndex].artPieces[i].Draw(sBatch);
                }

                //sBatch.Draw(playableArt[screenIndex], playableSurface.playableSectors[screenIndex].SectorRec, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);

                if (nextSection >= 0 && nextSection < playableSurface.playableSectors.Count())
                {

                    for (int i = 0; i < playableSurface.playableSectors[nextSection].artPieces.Count(); i++)
                    {
                        playableSurface.playableSectors[nextSection].artPieces[i].Draw(sBatch);
                    }
                    
                    //sBatch.Draw(playableArt[nextSection], playableSurface.playableSectors[nextSection].SectorRec, Color.White);

                    for (int i = 0; i < playableSurface.playableSectors[nextSection].collectiblePositions.Count(); i++)
                    {
                        sBatch.Draw(coin, playableSurface.playableSectors[nextSection].collectiblePositions[i], Color.White);
                    
                    }
                    for (int i = 0; i < playableSurface.playableSectors[nextSection].enemyPositions.Count(); i++)
                    {
                       playableSurface.playableSectors[nextSection].enemyPositions[i].Draw(gameTime,sBatch);
                    }
                }

                //DEBUG:  Used for drawing map collision boxes
                //for (int i = 0; i < playableSurface.playableSectors[screenIndex].collisionBoxes.Count(); i++)
                //{
                //    if(playableSurface.playableSectors[screenIndex].collisionBoxes[i].passable == true)
                //       sBatch.Draw(collisionTest, playableSurface.playableSectors[screenIndex].collisionBoxes[i].collisionBox, Color.White);
                //   else
                //       sBatch.Draw(collisionTest, playableSurface.playableSectors[screenIndex].collisionBoxes[i].collisionBox, Color.Red);
               // }

                
                for (int i = 0; i < playableSurface.playableSectors[screenIndex].collectiblePositions.Count(); i++)
                {
                    sBatch.Draw(coin, playableSurface.playableSectors[screenIndex].collectiblePositions[i], Color.White);
                }

                for (int i = 0; i < playableSurface.playableSectors[screenIndex].enemyPositions.Count(); i++)
                {
                    playableSurface.playableSectors[screenIndex].enemyPositions[i].Draw(gameTime, sBatch);

                    //DEBUG
                    //playableSurface.playableSectors[screenIndex].enemyPositions[i].DrawDebug(collisionTest, sBatch, debugColor);
                    
                }

                //DEBUG
                //sBatch.Draw(collisionTest, new Rectangle(1280, 0, 10, 720), Color.Red);
            }
        }

       
    }
            
    public class PlayableSurface
    {
        public PlayableSector[] playableSectors;
        
        public String[] artFiles;

        public List<string> enemyFiles = new List<string>();

        public List<Vector2> hatPositions = new List<Vector2>();

        private int sourceScreen;
        private int destinationScreen;

        public void LoadContent(PlayableSurface surfaceToLoad)
        {
            int numberOfSectors;
            

            numberOfSectors = surfaceToLoad.playableSectors.Count();
            

            playableSectors = new PlayableSector[numberOfSectors];
            

            artFiles = surfaceToLoad.artFiles;
            enemyFiles = surfaceToLoad.enemyFiles;
            hatPositions = surfaceToLoad.hatPositions;


            for (int i = 0; i < numberOfSectors; i++)
            {
                playableSectors[i] = new PlayableSector();
                playableSectors[i].LoadContent(surfaceToLoad.playableSectors[i]);
            }

            
        }

        public void TravelEnemyForward(EnemyData enemy, int enemyIndex)
        {
            sourceScreen = enemy.GetOwnedScreen();
            destinationScreen = sourceScreen + 1;
            
            
                enemy.SetOwnedScreen(destinationScreen);
                if (destinationScreen < playableSectors.Count())
                {
                    playableSectors[destinationScreen].AddEnemy(enemy);
                }
                playableSectors[sourceScreen].RemoveEnemyAt(enemyIndex);
                        
        }
        public void TravelEnemyBackward(EnemyData enemy, int enemyIndex)
        {
            sourceScreen = enemy.GetOwnedScreen();
            destinationScreen = sourceScreen - 1;

            

                enemy.SetOwnedScreen(destinationScreen);

                playableSectors[destinationScreen].AddEnemy(enemy);
                playableSectors[sourceScreen].RemoveEnemyAt(enemyIndex);
            
        }
        
    }

    public class ArtPiece
    {
        public Vector2 position;
        public int type;
        private Texture2D texture;

        public void SetTexture(ref Texture2D texture)
        {
            this.texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }


    /// <summary>
    /// Layer holds the layers Texture, a point that represents a scrolling speed ratio,
    /// and a float between 0(front) and 1(back) for zOrder when drawing.
    /// </summary>
    public class Layer
    {
        public Texture2D layerTexture;
        public Vector2 zDistance;
        public float zOrder;
        private Vector2 location;

        public Layer(Texture2D layerTexture, Vector2 zDistance)
        {
            this.layerTexture = layerTexture;
            this.zDistance = zDistance;
            this.location = Vector2.Zero;
        }
        public Layer(Texture2D layerTexture, Vector2 zDistance, Vector2 location)
        {
            this.layerTexture = layerTexture;
            this.zDistance = zDistance;
            this.location = location;    
        }
        public Layer()
        {
        }
        
        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            
            spriteBatch.Draw(layerTexture, location, Color.White);
            
            if(location.X < cameraPosition.X - 640)
                spriteBatch.Draw(layerTexture, new Vector2(location.X + 1280, location.Y), Color.White);
            else if(location.X > cameraPosition.X + 640)
                spriteBatch.Draw(layerTexture, new Vector2(location.X - 1280, location.Y), Color.White);

            //To determine if the right edge of the background is off the camera you add its width(1280) to the
            //to the position of the left edge of the camera.  The camera's X position is set to the center of
            //the screen (640).  So you add 1280 to 640 and you get 1920.  Add this to the location of the background
            //and compare it to the camera position and you will determine if its right edge has gone off screen.

            //*if the right edge of the image goes of the left side of the screen
            if (location.X + 1920 < cameraPosition.X)
            {
               location.X += 1280;
            }
            else if(location.X  > cameraPosition.X - 640)
            {
                location.X -= 1280;
            }
           
        }

    }

    
    
    public class PlayableSector
    {
        public Rectangle SectorRec;

        public List<ArtPiece> artPieces = new List<ArtPiece>();
        
        public List<CollisionRectangle> collisionBoxes = new List<CollisionRectangle>();

        public List<EnemyData> enemyPositions = new List<EnemyData>();

        public List<Vector2> collectiblePositions = new List<Vector2>();

               
        public void AddEnemy(EnemyData enemy)
        {
            enemyPositions.Add(enemy);
        }

        public void RemoveEnemyAt(int enemyIndex)
        {
            enemyPositions.RemoveAt(enemyIndex);
        }


        internal void LoadContent(PlayableSector sectorToLoad)
        {
            int numberOfEnemies = sectorToLoad.enemyPositions.Count();
            int numberOfCollectibles = sectorToLoad.collectiblePositions.Count();
            int numberOfCollisionBoxes = sectorToLoad.collisionBoxes.Count();

            collisionBoxes = sectorToLoad.collisionBoxes;
            enemyPositions = new List<EnemyData>();
            collectiblePositions = new List<Vector2>();
            artPieces = sectorToLoad.artPieces;

            for (int i = 0; i < numberOfEnemies; i++)
            {
                EnemyData tempEnemy = new EnemyData();

                tempEnemy.position = new Vector2(sectorToLoad.enemyPositions[i].position.X,sectorToLoad.enemyPositions[i].position.Y);
                tempEnemy.enemyTexture = sectorToLoad.enemyPositions[i].enemyTexture;
                tempEnemy.enemyType = sectorToLoad.enemyPositions[i].enemyType;

                enemyPositions.Add(tempEnemy);
            }

            for (int i = 0; i < numberOfCollectibles; i++)
            {
                Vector2 tempCollectible = new Vector2(sectorToLoad.collectiblePositions[i].X,
                                                      sectorToLoad.collectiblePositions[i].Y);

                collectiblePositions.Add(tempCollectible);
            }

            SectorRec = new Rectangle(sectorToLoad.SectorRec.X, sectorToLoad.SectorRec.Y,
                sectorToLoad.SectorRec.Width, sectorToLoad.SectorRec.Height);


        }
    }

    public class CollisionRectangle
    {
        public Rectangle collisionBox;
        public bool passable;
        public bool killsYou;
    }

    public class EnemyData
    {
        
        public int enemyTexture;
        public EnemyType enemyType;
        protected Texture2D textureImage;
        public Vector2 position;
        protected Vector2 originalPosition;
        protected Point frameSize;
        protected Point currentFrame;
        protected Point sheetSize;

        protected Rectangle bigHitBox;
        protected Rectangle smallHitBox;
        
        protected float currentScreen;
        protected int screenIndex;
        
        protected int collisionOffsetLeft;
        protected int collisionOffsetRight;
        protected int collisionOffsetTop;
        protected int collisionOffsetBottom;

        protected int killOffsetLeft;
        protected int killOffsetRight;
        protected int killOffsetTop;
        protected int killOffsetBottom;

        protected int timeSinceLastFrame = 0;
        protected int millisecondsPerFrame;
        private Vector2 speed;

        protected int ownedScreen; //stores the index of which playable sector it belongs to.

        protected bool isFacingLeft = true;
       

        public EnemyData()
        {
        }

        public EnemyData(Vector2 position,int enemyType)
        {
            this.position = position;
            this.originalPosition = position;
            this.enemyTexture = enemyType;
            this.ownedScreen = (int)(position.X / 1280);
            this.bigHitBox = new Rectangle((int)position.X + collisionOffsetLeft,
                                           (int)position.Y + collisionOffsetTop,
                                                frameSize.X - collisionOffsetRight,
                                                frameSize.Y - collisionOffsetBottom);

            this.smallHitBox = new Rectangle((int)position.X + killOffsetLeft,
                                             (int)position.Y + killOffsetTop,
                                                  frameSize.X - killOffsetRight,
                                                  frameSize.Y - killOffsetBottom);

        }
        public virtual void Update(GameTime gameTime, PlayableSector[] playableSector, Rectangle playerHitBox)
        {

        }

        public void ResetEnemy()
        {
            position = originalPosition;
        }
        
        public int GetOwnedScreen()
        {
            return ownedScreen;
        }

        public void SetOwnedScreen(int screenNumber)
        {
            ownedScreen = screenNumber;
        }
        public virtual void Update(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;

                currentFrame.X++;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    currentFrame.Y++;
                    if (currentFrame.Y >= sheetSize.Y)
                        currentFrame.Y = 0;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (isFacingLeft)
                spriteBatch.Draw(textureImage, position, new Rectangle(currentFrame.X * frameSize.X,
                                                                       currentFrame.Y * frameSize.Y,
                                                                       frameSize.X, frameSize.Y),
                                                                       Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            else
                spriteBatch.Draw(textureImage, position, new Rectangle(currentFrame.X * frameSize.X,
                                                                       currentFrame.Y * frameSize.Y,
                                                                       frameSize.X, frameSize.Y),
                                                                       Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0.0f);

           
            
        }

        public void DrawDebug(Texture2D collisionTest, SpriteBatch spriteBatch,Color color)
        {
            spriteBatch.Draw(collisionTest, collisionRect, color);
            spriteBatch.Draw(collisionTest, killRect, Color.Red);
        }
        public int GetRightSideIndex()
        {
            return (collisionRect.Right/1280);
        }

        public int GetLeftSideIndex()
        {
            return screenIndex;
        }
        public Rectangle collisionRect
        {
            get
            {
                bigHitBox.X = (int)position.X + collisionOffsetLeft;
                bigHitBox.Y = (int)position.Y + collisionOffsetTop;
                bigHitBox.Width = frameSize.X - collisionOffsetRight;
                bigHitBox.Height = frameSize.Y - collisionOffsetBottom;
                return bigHitBox;
            }
        }

        public Rectangle killRect
        {
            get
            {
                smallHitBox.X = (int)position.X + killOffsetLeft;
                smallHitBox.Y = (int)position.Y + killOffsetTop;
                smallHitBox.Width = frameSize.X - killOffsetRight;
                smallHitBox.Height = frameSize.Y - killOffsetBottom;
                return smallHitBox;
            }

        }
    }
}
