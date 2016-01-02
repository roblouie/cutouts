using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LevelEditor;

namespace WindowsGame2.Core
{
    public class EffectsEngine
    {
        private Texture2D explosionTexture;
        private List<smallExplosion> explosions;

        private Texture2D puffTexture;
        public List<PuffBall> puffBalls;

        private Texture2D hatTexture;
        public List<Hat> hats;

        private Texture2D debugTexture;
        
       
        public void Initialize(Game game)
        {
            explosions = new List<smallExplosion>();
            explosionTexture = game.Content.Load<Texture2D>(@"Effects\smallExplosion");

            puffBalls = new List<PuffBall>();
            puffTexture = game.Content.Load<Texture2D>(@"Effects\puffBall");

            hats = new List<Hat>();
            hatTexture = game.Content.Load<Texture2D>(@"Player1\hat");

            debugTexture = game.Content.Load<Texture2D>(@"pixel");
        }

        public void AddHat(Vector2 position, bool isLost)
        {
            hats.Add(new Hat(hatTexture,position, isLost));
        }
        
        public void AddPuffBall(Vector2 position)
        {
            puffBalls.Add(new PuffBall(puffTexture,position,debugTexture));
        }

                
        public void AddExplosion(Vector2 position)
        {
            explosions.Add(new smallExplosion(explosionTexture,position));
        }
        public void Update(GameTime gameTime, PlayableSector[] playableSectors)
        {
            for (int i = 0; i < explosions.Count(); i++)
            {
                explosions[i].Update(gameTime);

                if (explosions[i].isDead)
                    explosions.RemoveAt(i);
            }
            for (int i = 0; i < puffBalls.Count(); i++)
            {
                puffBalls[i].Update(gameTime,playableSectors);
                
                if (puffBalls[i].isDead)
                    puffBalls.RemoveAt(i);
            }

            for (int i = 0; i < hats.Count(); i++)
            {
                hats[i].Update(gameTime);

                if (hats[i].isDead)
                    hats.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < explosions.Count(); i++)
            {
                explosions[i].Draw(spriteBatch);
            }

            for (int i = 0; i < puffBalls.Count(); i++)
            {
                puffBalls[i].Draw(spriteBatch);
            }

            for (int i = 0; i < hats.Count(); i++)
            {
                hats[i].Draw(spriteBatch);
            }
        }


    }

    public class PuffBall
    {
        private Texture2D puffTexture;
        private Rectangle collisionRect;
        public Rectangle collisionBox;
        public Vector2 position;
        public Vector2 origin;
                
        public int lifeTime;
        
        private bool isOnGround;

        private const float GroundDrag = 0.8f;
        private const float AirDrag = 0.8f;

        
        private const float gravity = 500f;
        private const float moveSpeed = 9000f;

        public Vector2 velocity = new Vector2();
        Vector2 maxVelocity = new Vector2(9000, 500);

        private float movement = 0;
        private float previousBottom;
        private float puffCenter;

        private float currentScreen;
        private int screenIndex;

        private int width = 50;
        private int height = 46;

        public bool isDead;
        public bool isHeld;

        Texture2D debugTexture;
       
        
        public PuffBall(Texture2D texture, Vector2 position, Texture2D debugTexture)
        {
            puffTexture = texture;
            lifeTime = 4;
            this.position = position;
            collisionRect = new Rectangle((int)position.X, (int)position.Y, width, height);
            
            origin = new Vector2(width / 2, height / 2);
            this.debugTexture = debugTexture;
        }


        public Rectangle collisionRec
        {
            get
            {
                collisionRect.X = (int)position.X;
                collisionRect.Y = (int)position.Y;
                collisionRect.Width = width;
                collisionRect.Height = height;
                return collisionRect;
            }
        }

        public void ThrowLeft()
        {
            isHeld = false;
            movement = -1;
            velocity.X = -1200;
        }

        public void ThrowRight()
        {
            isHeld = false;
            movement = 1;
            velocity.X = 1200;
        }
        public void Update(GameTime gameTime, PlayableSector[] playableSectors)
        {
            if (lifeTime <= 0)
                isDead = true;

            
            physicsAndCollision(gameTime, playableSectors);
             
        }

         private void physicsAndCollision(GameTime gameTime, PlayableSector[] playableSectors)
       {
           float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

           puffCenter = collisionRec.X + (collisionRec.Width / 2);
           
           currentScreen = puffCenter / 1280;
           screenIndex = (int)currentScreen;

           //STOPS ERROR REMOVE LATER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
           if (screenIndex < 0)
               screenIndex = 0;

           //stop the player from moving too fast
           velocity.X = MathHelper.Clamp(velocity.X + movement * moveSpeed * elapsed, -maxVelocity.X, maxVelocity.X);
           velocity.Y = MathHelper.Clamp(velocity.Y + gravity * elapsed, -maxVelocity.Y, maxVelocity.Y);

           //Jump(gameTime);

           if (isOnGround)
               velocity.X *= GroundDrag;
           else
               velocity.X *= AirDrag;

          

           

           //update the players position

           position += velocity * elapsed;
           position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

           isOnGround = false;

           if (screenIndex >= playableSectors.Count() || screenIndex < 0)
               isDead = true;
             
           if (!isDead)
           {
               for (int i = 0; i < playableSectors[screenIndex].collisionBoxes.Count; i++)
               {
                   Vector2 depth = GameEngine.GetIntersectionDepth(collisionRec, playableSectors[screenIndex].collisionBoxes[i].collisionBox);

                   if (depth != Vector2.Zero)
                   {

                       if (playableSectors[screenIndex].collisionBoxes[i].killsYou)
                           isDead = true;
                       
                       float absDepthX = Math.Abs(depth.X);
                       float absDepthY = Math.Abs(depth.Y);

                       if (absDepthY < absDepthX || playableSectors[screenIndex].collisionBoxes[i].passable == true)
                       {
                           if (previousBottom <= playableSectors[screenIndex].collisionBoxes[i].collisionBox.Bottom)
                           {
                               isOnGround = true;
                               velocity.Y = 0;

                           }
                           if (playableSectors[screenIndex].collisionBoxes[i].passable == false || isOnGround)
                           {
                               position = new Vector2(position.X, position.Y + depth.Y);
                               velocity.Y = 0;
                           }

                       }
                       else if (playableSectors[screenIndex].collisionBoxes[i].passable == false)
                       {
                           position = new Vector2(position.X + depth.X, position.Y);
                           velocity.X*=-1;
                           movement *= -1;
                       }

                   }




               }
           }
           previousBottom = collisionRec.Bottom;
       }
       
    
        
        public void Hit()
        {
            lifeTime--;

            width -= 7;
            height -= 7;

            origin.X = width / 2;
            origin.Y = height / 2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(puffTexture, collisionRec, null, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            
            //spriteBatch.Draw(debugTexture, collisionRec, Color.White);
        }
    }


    public class Hat
    {
        private Texture2D hatTexture;
        public Vector2 position;
        public Vector2 centerOffset;
        private float hatRotation;
        public bool isDead;
        public bool isLost;

        public Hat(Texture2D texture, Vector2 position, bool isLost)
        {
            hatTexture = texture;
            this.position = position;
            this.isLost = isLost;
            centerOffset = new Vector2(51, 11);
        }

        public void Update(GameTime gameTime)
        {
            if (isLost)
            {
                position.Y += 4;
                hatRotation += 0.1f;
                if (position.Y > 750)
                    isDead = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(hatTexture, position, null, Color.White, hatRotation, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
    
    public class smallExplosion
    {
        private Texture2D explosionTexture;
        private Vector2 position;
        private Vector2 origin;
        private float lifeTime;
        private float age = 0;
        private float rotation;
        private float scale;

        public bool isDead;

        public smallExplosion(Texture2D texture,Vector2 position)
        {
            explosionTexture = texture;
            lifeTime = 200;
            origin = new Vector2(51, 51);
            this.position = position + origin;
        }

        public void Update(GameTime gameTime)
        {
            age += gameTime.ElapsedGameTime.Milliseconds;

            if (scale < 1.0f)
                scale += 0.1f;

            rotation += 0.1f;

            if(age > lifeTime)
                isDead = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(explosionTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.None, 1.0f);
        }
    }
}
