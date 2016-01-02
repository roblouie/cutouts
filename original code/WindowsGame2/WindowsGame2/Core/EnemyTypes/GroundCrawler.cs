using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LevelEditor;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame2.Core.EnemyTypes
{
    class GroundCrawler:EnemyData
    {

        private bool isOnGround;

        private const float GroundDrag = 0.68f;
        private const float AirDrag = 0.65f;

             
        private const float gravity = 500f;
        private const float moveSpeed = 1000f;

        
        Vector2 velocity = new Vector2();
        Vector2 maxVelocity = new Vector2(1000, 500);

        private float movement = -1;
        private float previousBottom;
       

        private float enemyCenter;

        
        

       public GroundCrawler(Texture2D crawlerTexture,Vector2 position)
       {
           this.textureImage = crawlerTexture;
           this.position = position;
           this.originalPosition = position;
           this.frameSize = new Point(125, 90);
           this.millisecondsPerFrame = 250;
           this.sheetSize = new Point(3, 0);
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
          if(!GameEngine.loadingMap)
           physicsAndCollision(gameTime, playableSectors);

          base.Update(gameTime);
       }
       private void physicsAndCollision(GameTime gameTime, PlayableSector[] playableSectors)
       {
           float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

           enemyCenter = position.X + (frameSize.X / 2);
           
           currentScreen = enemyCenter / 1280;
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
                       }

                   }


               }

           }

           previousBottom = collisionRect.Bottom;
       }
       
    }
}
