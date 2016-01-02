using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LevelEditor;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame2.Core.EnemyTypes
{
    class Bee:EnemyData
    {
        Vector2 velocity = new Vector2();
        Vector2 maxVelocity = new Vector2(1000, 300);

        private float movement = 1;
        private float previousBottom;
        

        private float enemyCenter;


        private bool isOnGround;
                           
        
        private const float moveSpeed = 600f;

        
        public Bee(Texture2D beeTexture,Vector2 position)
       {
           this.textureImage = beeTexture;
           this.position = position;
           this.frameSize = new Point(192, 158);
           this.millisecondsPerFrame = 25;
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
                physicsAndCollision(gameTime, playableSectors);

            base.Update(gameTime);
        }

        private void physicsAndCollision(GameTime gameTime, PlayableSector[] playableSectors)
       {
           float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

           enemyCenter = position.X + (frameSize.X / 2);
           
           currentScreen = enemyCenter / 1280;
           screenIndex = (int)currentScreen;

           
           //stop the player from moving too fast
      
           velocity.Y = MathHelper.Clamp(velocity.Y + movement * moveSpeed * elapsed, -maxVelocity.Y, maxVelocity.Y);

          

          
           if (velocity.X > 0)
               isFacingLeft = false;
           if (velocity.X < 0)
               isFacingLeft = true;

           

           //update the players position

           position += velocity * elapsed;
           position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

           isOnGround = false;

           if (position.Y < 70 && movement < 0)
           {
               movement *= -1;
           }
           else if (position.Y > 500 && movement > 0)
           {
               movement *= -1;
           }
            
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
                           velocity.Y = 0;
                           
                       }
                       if (playableSectors[screenIndex].collisionBoxes[i].passable == false || isOnGround)
                       {
                           position = new Vector2(position.X, position.Y + depth.Y);
                           movement *= -1;
                       }

                   }
                   

               }




           }

           previousBottom = collisionRect.Bottom;
       }
       
    }
    }

