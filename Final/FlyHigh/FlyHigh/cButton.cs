using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyHigh
{
    class cButton
    {
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;

        Color colour = new Color(255, 255, 255, 255);

        public bool isClicked;

        public cButton()
        {



        }

        public void Load(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;
        }


        public void Update(MouseState mouse, int zustand)
        {
            mouse = Mouse.GetState();

            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height);

            Rectangle mouseRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

            if (mouseRectangle.Intersects(rectangle))
            {

                if (mouse.LeftButton == ButtonState.Pressed && zustand == 0)
                {
                    isClicked = true;
                    colour.A = 255;
                    Game1.instance.gameState = Game1.GameState.ingame;
                }
                else if (mouse.LeftButton == ButtonState.Pressed && zustand == 1)
                {
                    Game1.instance.Exit();
                }
            }
            else if (colour.A < 255)
            {
                colour.A += 3;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, colour);
        }
    }
}
