using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame2.Core
{
    public class Camera
    {
        Vector2 position = new Vector2(0,0);
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                updatematrix();
            }
        }
        public float X
        {
            get { return position.X; }
            set
            {
                position.X = value;
                updatematrix();
            }
        }
        public float Y
        {
            get { return position.Y; }
            set
            {
                position.Y = value;
                updatematrix();
            }
        }
        float rotation;
        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                updatematrix();
            }
        }

        float scale;
        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                updatematrix();
            }
        }

        public Matrix matrix;
        Vector2 viewport;

        public Camera(float width, float height)
        {
            position = new Vector2(0, 0);
            position.X += (width / 2);
            position.Y += (height / 2);
            rotation = 0;
            scale = 1.0f;
            viewport = new Vector2(width, height);
            updatematrix();
        }

        void updatematrix()
        {
            matrix = Matrix.CreateTranslation(-position.X, -position.Y, 0.0f) *
                     Matrix.CreateRotationZ(rotation) *
                     Matrix.CreateScale(scale) *
                     Matrix.CreateTranslation(viewport.X / 2, viewport.Y / 2, 0.0f);
        }

        public void updateviewport(float width, float height)
        {
            viewport.X = width;
            viewport.Y = height;
            updatematrix();
        }
    }


    
}
