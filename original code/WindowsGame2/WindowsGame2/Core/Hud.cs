using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame2.Core
{
    class Hud
    {
        SpriteFont gameFont;
        
        private Color hudColor;

        private Vector2 livesOffset;
       
        private Vector2 coinsOffset;

        private Vector2 livesImageOffset;
        private Vector2 coinsImageOffset;

        private Texture2D livesImage;
        private Texture2D coinImage;

        private Vector2 origin = new Vector2(0, 0);

        /// <summary>
        /// Load the font and color,load images for the lives and collectibles counters
        /// and set the offset from center screen for HUD items
        /// </summary>
        public Hud(Game game, string livesImage, string coinsImage, string fontFile,Color fontColor,
            Vector2 livesOffset, Vector2 coinsOffset)
        {
            gameFont = game.Content.Load<SpriteFont>(fontFile);
            this.livesImage = game.Content.Load<Texture2D>(livesImage);
            this.coinImage = game.Content.Load<Texture2D>(coinsImage);
            hudColor = fontColor;

            this.livesImageOffset = livesOffset;
            this.livesOffset = livesOffset + new Vector2(this.livesImage.Width, this.livesImage.Height/3);
            this.coinsImageOffset = coinsOffset;
            this.coinsOffset = coinsOffset + new Vector2(this.coinImage.Width, this.coinImage.Height / 3);
        }

        public void DrawHud(SpriteBatch sBatch, Vector2 cameraPosition, string lives, string coins)
        {
            sBatch.Draw(livesImage, cameraPosition + livesImageOffset, Color.White);
            sBatch.DrawString(gameFont, lives, cameraPosition + livesOffset, hudColor, 0.0f, origin, 1.5f, SpriteEffects.None, 0.0f);

            sBatch.Draw(coinImage, cameraPosition + coinsImageOffset, Color.White);
            sBatch.DrawString(gameFont, coins, cameraPosition + coinsOffset, hudColor, 0.0f, origin, 1.3f, SpriteEffects.None, 0.0f);
            
        }
    }
}
