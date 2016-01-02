using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame2.Core
{



    public abstract class AnimatedSprite
    {
        protected Texture2D textureImage;
        protected Texture2D hatImage;
        public Vector2 hatPosition;
        private Vector2 hatOrigin = new Vector2(51,11);
        private float hatRotation;

        protected Color playerColor;
        
        public Vector2 position;
        private Vector2 bottomHalf;
        protected Point frameSize;
        
        protected int collisionOffsetX;
        protected int collisionOffsetTop;
        protected int collisionOffsetBottom;
        
        public Point currentFrame;
        public Point sheetSize;

        public bool hasHat = true;


        Point standingFrame = new Point(0,0);
        Point standingHoldingFrame = new Point(7, 1);
        Point jumpingFrame = new Point(5,1);
        Point jumpingHoldingFrame = new Point(8, 1);
        Point movingStart = new Point(1,0);
        Point movingEnd = new Point(3,1);
        Point duckingStart = new Point(4,1);
        Point dyingFrame = new Point(6, 1);
        
        int timeSinceLastFrame = 0;
        public int millisecondsPerFrame;
     
        
        public bool isFacingRight = true;
        public bool isStanding = false;
        public bool isDucking = false;
        public bool isJumpingAnimation = false;
        public bool isDying = false;

        public bool isHolding = false;

        public AnimatedSprite(Vector2 position)
        {
            playerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            this.position = position;
            this.frameSize = new Point(102,150);
            this.currentFrame = new Point(0, 0);
            this.sheetSize = new Point(9,2);
            
            this.millisecondsPerFrame = 50;
        }

        public virtual void Update(GameTime gameTime)
        {

            


            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;

                if (isDucking)
                {
                    currentFrame = duckingStart;
                    

                }
                else if (isJumpingAnimation)
                {
                    if (isHolding)
                        currentFrame = jumpingHoldingFrame;
                    else
                        currentFrame = jumpingFrame;
                }
                else if (isStanding)
                {
                    if (isHolding)
                        currentFrame = standingHoldingFrame;
                    else
                        currentFrame = standingFrame;
                }
                else
                {


                    ++currentFrame.X;
                    if (currentFrame.X >= sheetSize.X)
                    {


                        currentFrame.X = 0;
                        ++currentFrame.Y;
                        if (currentFrame.Y >= sheetSize.Y)
                            currentFrame.Y = 1;

                    }
                    if (currentFrame.X > movingEnd.X && currentFrame.Y >= movingEnd.Y)
                    {
                        currentFrame = movingStart;
                    }

                }

                
                if (isDying)
                    currentFrame = dyingFrame;

            }

            

            if (currentFrame != duckingStart)
            {
                hatPosition.Y = position.Y + hatOrigin.Y;
                hatPosition.X = position.X + hatOrigin.X;
                hatRotation = 0.0f;
            }
            if (currentFrame == duckingStart)
            {
                 hatPosition.Y = position.Y + 78 + hatOrigin.Y;

                 if (isFacingRight)
                 {
                     hatPosition.X = position.X + 35 + hatOrigin.X;
                     hatRotation = 0.4f;
                 }
                 else
                 {
                     hatPosition.X = position.X - 35 + hatOrigin.X;
                     hatRotation = -0.4f;
                 }
               
            }
        
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (isHolding && !isDucking && !isDying)
            { 
                bottomHalf.Y = position.Y + 100;
                bottomHalf.X = position.X;
                
                if (isFacingRight)
                {
                                 
                    
                    spriteBatch.Draw(textureImage, bottomHalf, new Rectangle(currentFrame.X * frameSize.X,
                                                                                (currentFrame.Y * frameSize.Y + 100),
                                                                                frameSize.X, frameSize.Y -100),
                                                                                playerColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 1.0f);
                    
                    spriteBatch.Draw(textureImage, position, new Rectangle(6 * frameSize.X,
                                                                               0 * frameSize.Y,
                                                                               frameSize.X, frameSize.Y - 50),
                                                                               playerColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 1.0f);

                    if (hasHat)
                        spriteBatch.Draw(hatImage, hatPosition, null, Color.White, hatRotation, hatOrigin, 1.0f, SpriteEffects.None, 1.0f); 
                }
                else
                {
                    
                    
                    spriteBatch.Draw(textureImage, bottomHalf, new Rectangle(currentFrame.X * frameSize.X,
                                                                                (currentFrame.Y * frameSize.Y + 100),
                                                                                frameSize.X, frameSize.Y - 100),
                                                                                playerColor, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1.0f);

                    spriteBatch.Draw(textureImage, position, new Rectangle(6 * frameSize.X,
                                                                               0 * frameSize.Y,
                                                                               frameSize.X, frameSize.Y - 50),
                                                                               playerColor, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1.0f);
                    if (hasHat)
                        spriteBatch.Draw(hatImage, hatPosition, null, Color.White, hatRotation, hatOrigin, 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
                }
            }
            else
            {
                if (isFacingRight)
                {
                    
                    
                    spriteBatch.Draw(textureImage, position, new Rectangle(currentFrame.X * frameSize.X,
                                                                            currentFrame.Y * frameSize.Y,
                                                                            frameSize.X, frameSize.Y),
                                                                            playerColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 1.0f);
                    if (hasHat)
                        spriteBatch.Draw(hatImage, hatPosition, null, Color.White, hatRotation, hatOrigin, 1.0f, SpriteEffects.None, 1.0f);
                }
                else
                {
                   
                    
                    spriteBatch.Draw(textureImage, position, new Rectangle(currentFrame.X * frameSize.X,
                                                                            currentFrame.Y * frameSize.Y,
                                                                            frameSize.X, frameSize.Y),
                                                                            playerColor, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1.0f);
                    if (hasHat)
                        spriteBatch.Draw(hatImage, hatPosition, null, Color.White, hatRotation, hatOrigin, 1.0f, SpriteEffects.FlipHorizontally, 1.0f);
                }
            }
        }

       

        public Rectangle collisionRect
        {
            get
            {
                return new Rectangle((int)position.X + collisionOffsetX,
                                     (int)position.Y + collisionOffsetTop,
                                      frameSize.X - (collisionOffsetX * 2),
                                      frameSize.Y - collisionOffsetBottom);
            }
        }

    }
}
