using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LevelEditor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace WindowsGame2.Core.EnemyTypes
{
    class Rabbit:EnemyData
    {
        private bool isOnGround;
        private bool wantsToJump = true;
        private bool startJump;
        
        private const float GroundDrag = 0.9f;
        private const float AirDrag = 0.9f;

        private const float JumpLaunchVelocity = -2000f;
        private const float MaxJumpTime = 0.45f;



        private const float JumpControlPower = 0.14f;
        private const float gravity = 500f;
        private const float moveSpeed = 1800f;

        
        Vector2 velocity = new Vector2();
        Vector2 maxVelocity = new Vector2(1300, 500);

        private float movement = -1;
        private float previousBottom;
        private float jumpTime;

        private float enemyCenter;
        private Point jumpingFrame = new Point(4, 0);
        
        

       public Rabbit(Texture2D rabbitTexture,Vector2 position)
       {
           this.textureImage = rabbitTexture;
           this.position = position;
           this.frameSize = new Point(130, 120);
           this.millisecondsPerFrame = 40;
           this.sheetSize = new Point(5, 0);
           this.ownedScreen = (int)(position.X / 1280);
           
           this.collisionOffsetLeft = 10;
           this.collisionOffsetRight = 17;
           this.collisionOffsetTop = 30;
           this.collisionOffsetBottom = 50;

           this.killOffsetLeft = 40;
           this.killOffsetRight = 70;
           this.killOffsetTop = 50;
           this.killOffsetBottom = 50;
       }

       public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, SpriteBatch spriteBatch)
       {

          
           base.Draw(gameTime, spriteBatch);
       }

       public override void Update(GameTime gameTime, PlayableSector[] playableSectors, Rectangle playerHitBox)
       {
          if(!GameEngine.loadingMap)
           physicsAndCollision(gameTime, playableSectors);

          if (startJump)
          {
              timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
              if (timeSinceLastFrame > millisecondsPerFrame)
              {
                  timeSinceLastFrame = 0;

                  currentFrame.X++;
                  if (currentFrame.X >= sheetSize.X)
                  {
                      startJump = false;
                      if (isOnGround)
                          currentFrame.X = 0;
                      else
                          currentFrame.X = 4;
                  }
              }
          }
          
       }

       private void Jump(GameTime gameTime)
       {
           //if (wantsToJump && !isJumping && isOnGround)
           //    GameAudio.soundBank.PlayCue("jump");

           if (wantsToJump)
           {
               if (isOnGround || jumpTime > 0.0f)
               {
                   jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                   //isJumpingAnimation = wantsToJump;
                   


               }

               if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
               {
                   velocity.Y = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                   
               }
               else
               {
                   jumpTime = 0.0f;

               }

               
           }
           else
           {

               jumpTime = 0.0f;
           }
           



       }
        
        private void physicsAndCollision(GameTime gameTime, PlayableSector[] playableSectors)
       {
           float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

           enemyCenter = position.X + (frameSize.X / 2);
           
           currentScreen = enemyCenter / 1280;
           screenIndex = (int)currentScreen;

           //STOPS ERROR REMOVE LATER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
           //if (screenIndex < 0)
           //    screenIndex = 0;

           //stop the player from moving too fast
           velocity.X = MathHelper.Clamp(velocity.X + movement * moveSpeed * elapsed, -maxVelocity.X, maxVelocity.X);
           velocity.Y = MathHelper.Clamp(velocity.Y + gravity * elapsed, -maxVelocity.Y, maxVelocity.Y);

           Jump(gameTime);

           if (isOnGround)
               velocity.X *= GroundDrag;
           else
               velocity.X *= AirDrag;

           if (velocity.X > 0)
               isFacingLeft = false;
           if (velocity.X < 0)
               isFacingLeft = true;

           

           //update the players position

           position += velocity * elapsed;
           position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

           isOnGround = false;
           if (screenIndex <= playableSectors.Count())
           {
               for (int i = 0; i < playableSectors[screenIndex].collisionBoxes.Count; i++)
               {
                   Vector2 depth = GameEngine.GetIntersectionDepth(collisionRect, playableSectors[screenIndex].collisionBoxes[i].collisionBox);

                   if (depth != Vector2.Zero)
                   {
                       float absDepthX = Math.Abs(depth.X);
                       float absDepthY = Math.Abs(depth.Y);

                       if (absDepthY < absDepthX || playableSectors[screenIndex].collisionBoxes[i].passable == true)
                       {
                           if (previousBottom <= playableSectors[screenIndex].collisionBoxes[i].collisionBox.Bottom)
                           {
                               isOnGround = true;
                               startJump = true;
                               currentFrame.X = 0;
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

                           movement *= -1;
                           velocity.X = 0;

                       }

                   }



               }
           }

           previousBottom = collisionRect.Bottom;
       }
       
    }
}

