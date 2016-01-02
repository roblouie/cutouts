using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LevelEditor;


namespace WindowsGame2.Core
{
    public class Player: AnimatedSprite
    {

        private bool wantsToJump;
        private bool isJumping;
        private bool isOnGround;

        public bool isGrabbing;
        public Vector2 heldPosition;

        public bool enemyBounce;

        private const float GroundDrag = 0.68f;
        private const float AirDrag = 0.68f;

        private const float JumpLaunchVelocity = -1100f;
        private const float MaxJumpTime = 0.4f;//0.45f;



        private const float JumpControlPower = 2.0f;
        private const float gravity = 4500f;
        private float moveSpeed = 14000f;

        
        Vector2 velocity = new Vector2();
        Vector2 maxVelocity = new Vector2(2000, 1700);

        private Vector2 movement;
        private float previousBottom;
        private float jumpTime;

        public int score = 0;
        public int coins = 0;
        public int lives = 3;

        public float currentScreen;
        public int screenIndex;
        public int nextSection;
        public float playerCenter;

        public bool isFalling;

        private float invincibilityTimeLimit = 1000;
        private float invincibilityTime;
        public bool isInvincible;

        //DEBUG
        private Texture2D collisionBoxFiller;


        public Player(Vector2 position)
            : base(position)
        {
            collisionOffsetX = 40;
            collisionOffsetTop = 15;
            collisionOffsetBottom = 20;
        }

        public void LoseHat()
        {
            hasHat = false;
            isInvincible = true;
            invincibilityTime = 0.0f;
            playerColor.A = 128;
        }

        public void Reset()
        {
            hasHat = true;
            lives = 3;
            score = 0;
            coins = 0;
        }
        
        public void Load(Game game, Vector2 position)
        {
            this.position = position;
            currentScreen = 0;
            nextSection = 0;
            screenIndex = 0;
            isHolding = false;
            textureImage = game.Content.Load<Texture2D>("Player1\\MainCharacter3");
            hatImage = game.Content.Load<Texture2D>("Player1\\hat");
        }
        
        public void GetInput()
        {


            if (isDying == false)
            {

                //add the value of the thumbstick to the players velocity
                if (GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.X > 0)
                    movement.X = GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.X;

                if (GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.X < -0.0)
                    movement.X = GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.X;

                if (GamePad.GetState(ControllerManager.controllingPlayer).ThumbSticks.Left.Y < -0.2)
                {

                    isDucking = true;
                }
                else
                {


                    isDucking = false;
                }


                if (GamePad.GetState(ControllerManager.controllingPlayer).IsButtonDown(Buttons.X))
                {
                    isGrabbing = true;

                    if (moveSpeed < 18000f)
                        moveSpeed += 300;

                }
                else
                {
                    isGrabbing = false;
                    isHolding = false;
                    moveSpeed = 11000f;
                }

                wantsToJump = GamePad.GetState(ControllerManager.controllingPlayer).IsButtonDown(Buttons.A) || Keyboard.GetState().IsKeyDown(Keys.Space);

                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    movement.X += 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    movement.X -= 1;


            }
               

        }

        private void Jump(GameTime gameTime)
        {
            if(wantsToJump && !isJumping && isOnGround)
                GameAudio.soundBank.PlayCue("hop");
            
            if (wantsToJump || enemyBounce)
            {
                if ((!isJumping && isOnGround) || jumpTime > 0.0f || enemyBounce)
                {
                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    isJumpingAnimation = wantsToJump || enemyBounce;
                    isStanding = !isJumpingAnimation;
                    
                    
                }

                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    velocity.Y = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));

                }
                else
                {
                    jumpTime = 0.0f;
                   
                }

                 enemyBounce = false;
            }
            else
            {
                
                jumpTime = 0.0f;
            }
            isJumping = wantsToJump;
           
        

        }

       

        public void PhysicsAndCollision(GameTime gameTime,PlayableSurface playableSurface)
        {
          
            
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            playerCenter = position.X + (frameSize.X / 2);

            currentScreen = playerCenter / 1280;
            
            screenIndex = (int)currentScreen;
            nextSection = (int)Math.Round(currentScreen);
            if (nextSection >= playableSurface.playableSectors.Count())
                nextSection--;

            //stop the player from moving too fast
            velocity.X = MathHelper.Clamp(velocity.X + movement.X * moveSpeed * elapsed, -maxVelocity.X, maxVelocity.X);
            velocity.Y = MathHelper.Clamp(velocity.Y + gravity * elapsed, -maxVelocity.Y, maxVelocity.Y);



            if (velocity.X > 800 || velocity.X < -800)
                millisecondsPerFrame = 30;
            else if (velocity.X > 400 || velocity.X < -400)
                millisecondsPerFrame = 40;
            else
                millisecondsPerFrame = 80;
            
            Jump(gameTime);

            if (isOnGround)
                velocity.X *= GroundDrag;
            else
                velocity.X *= AirDrag;

            if (velocity.X > 0)
            {
                isFacingRight = true;

                
                
            }
            if (velocity.X < 0)
            {
                isFacingRight = false;

               
                
            }
            if (velocity.Y > 0)
                isFalling = true;
            else
                isFalling = false;

            if (movement.X < 0.1 && movement.X > -0.1)
                isStanding = true;
            else
            {
                isStanding = false;
                isDucking = false;
            }

            if (isDucking)
            {
                collisionOffsetTop = 100;
                collisionOffsetBottom = 105;
            }
            else
            {
                collisionOffsetTop = 15;
                collisionOffsetBottom = 20;
            }
            
            //update the players position

            position += velocity * elapsed;
            position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

            if (isFacingRight)
            {
                heldPosition.X = collisionRect.Right;
                heldPosition.Y = collisionRect.Y + 50;
            }
            else
            {
                heldPosition.X = collisionRect.Left;
                heldPosition.Y = collisionRect.Y + 50;
            }
            
            isOnGround = false;

            Collision(playableSurface.playableSectors[screenIndex].collisionBoxes);
            Collision(playableSurface.playableSectors[nextSection].collisionBoxes);

            previousBottom = collisionRect.Bottom;

            

           }

        private void Collision(List<CollisionRectangle> mapRecs)
        {
            for (int i = 0; i < mapRecs.Count; i++)
            {
                Vector2 depth = GameEngine.GetIntersectionDepth(collisionRect, mapRecs[i].collisionBox);

                if (depth != Vector2.Zero && isDying == false)
                {

                    if (mapRecs[i].killsYou)
                    {
                        isDying = true;
                        enemyBounce = true;
                    }

                   
                    float absDepthX = Math.Abs(depth.X);
                    float absDepthY = Math.Abs(depth.Y);

                    if (absDepthY < absDepthX || mapRecs[i].passable == true)
                    {
                        if (previousBottom <= mapRecs[i].collisionBox.Bottom)
                        {
                            isOnGround = true;
                            isJumpingAnimation = false;
                            
                           
                        }
                        if (mapRecs[i].passable == false || isOnGround)
                        {
                            position = new Vector2(position.X, position.Y + depth.Y);
                            velocity.Y = 0;
                        }

                    }
                    else if (mapRecs[i].passable == false)
                    {
                        position = new Vector2(position.X + depth.X, position.Y);

                        if (absDepthY > absDepthX)
                            isStanding = true;
                        else
                            velocity.Y = 0;

                    }

                }




            }
        }

                
        public void  Update(GameTime gameTime,PlayableSurface playableSurface)
        {
            if(!GameEngine.loadingMap)
                GetInput();
            
            
            
            PhysicsAndCollision(gameTime, playableSurface);

            if (isInvincible)
            {
                invincibilityTime += gameTime.ElapsedGameTime.Milliseconds;
                                              
                if (invincibilityTime >= invincibilityTimeLimit)
                {
                    isInvincible = false;
                    playerColor.A = 255;
                }
            }
            
            
            movement.X = 0;
            movement.Y = 0;
            
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(textureImage, position, Color.White);

            //Debug
            
            //spriteBatch.Draw(collisionBoxFiller, collisionRect, Color.White);

            
            base.Draw(gameTime, spriteBatch);
        }

        public void DrawDebug(Texture2D collisionTest, SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(collisionTest, collisionRect, color);
        }
    }
}
