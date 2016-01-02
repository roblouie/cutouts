using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LevelEditor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame2.Core.EnemyTypes
{
    class Bat:EnemyData
    {
        Vector2 velocity = new Vector2();
        Vector2 maxVelocity = new Vector2(6000, 4000);

        private Vector2 movement = new Vector2();
        private float previousBottom;

        private const float AirDrag = 0.8f;

        private float enemyCenter;


        private bool isOnGround;
                           
        
        private const float moveSpeed = 4000f;

        
        public Bat(Texture2D beeTexture,Vector2 position)
       {
           this.textureImage = beeTexture;
           this.position = position;
           this.frameSize = new Point(210, 101);
           this.millisecondsPerFrame = 90;
           this.sheetSize = new Point(6, 0);
           this.ownedScreen = (int)(position.X / 1280);
           
           this.collisionOffsetLeft = 10;
           this.collisionOffsetRight = 17;
           this.collisionOffsetTop = 30;
           this.collisionOffsetBottom = 30;

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
            if (!GameEngine.loadingMap)
                physicsAndCollision(gameTime, playableSectors, playerHitBox);

            base.Update(gameTime);
        }

        private void physicsAndCollision(GameTime gameTime, PlayableSector[] playableSectors, Rectangle playerHitBox)
       {
           float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

           enemyCenter = position.X + (frameSize.X / 2);
           
           currentScreen = enemyCenter / 1280;
           screenIndex = (int)currentScreen;

           if (playerHitBox.Center.X < enemyCenter)
           {
               movement.X = -1;
              
           }
           else if (playerHitBox.Center.X > enemyCenter)
           {
               movement.X = 1;
             
           }
           else if(playerHitBox.Center.X == enemyCenter)
           {
               movement.X = 0;
               
           }
          
           if (playerHitBox.Top < position.Y)
           {
               movement.Y = -1;
              
           }
           else if (playerHitBox.Top > position.Y)
           {
               movement.Y = 1;
               
           }
           else if(playerHitBox.Top == position.Y)
           {
               movement.Y = 0;
               
           }

           if (position.Y < 0)
               position.Y = 0;

           //stop the player from moving too fast
           velocity.X = MathHelper.Clamp(velocity.X + movement.X * moveSpeed * elapsed, -maxVelocity.X, maxVelocity.X);
           velocity.Y = MathHelper.Clamp(velocity.Y + movement.Y * moveSpeed * elapsed, -maxVelocity.Y, maxVelocity.Y);

           velocity.X *= AirDrag;
           velocity.Y *= AirDrag;

          
           if (velocity.X < 0)
               isFacingLeft = false;
           if (velocity.X > 0)
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


                           }
                           if (playableSectors[screenIndex].collisionBoxes[i].passable == false || isOnGround)
                           {
                               position = new Vector2(position.X, position.Y + depth.Y);

                           }

                       }
                       else if (playableSectors[screenIndex].collisionBoxes[i].passable == false)
                       {
                           position = new Vector2(position.X + depth.X, position.Y);

                       }

                   }

               }
           }
           previousBottom = collisionRect.Bottom;
       }
       
    }
 }

