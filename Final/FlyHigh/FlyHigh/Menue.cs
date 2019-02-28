using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyHigh
{
    public class Menue
    {
        // Buttons
        Texture2D sb;
        Rectangle sbrec;

        Texture2D end;
        Rectangle endrec;

        // Backrounds
        Texture2D backg;
        Rectangle backgrec;

        // Mouse
        Texture2D mouseTex;
        Rectangle mouseRec;
        Vector2 mousePos;

        // Pause Menue Stuff
        cButton btnPlay, btnQuit;
        bool pause = false;
        Texture2D pausedTexture;
        Rectangle pausedRectangle;

        // Gameover Buttons
        Texture2D go, beC, stC;
        Rectangle goRec, beCRec, stCRec;

        // Win Stuff
        Texture2D win;
        Rectangle winRec;


        public Menue()
        {
            loadContent();
        }

        private void loadContent()
        {
            // Mouse
            mouseTex = Game1.instance.Content.Load<Texture2D>("Img/MouseRec");

            // Buttons
            sb = Game1.instance.Content.Load<Texture2D>("Img/Spielstart");
            sbrec = new Rectangle(900, 450, 324, 104);

            end = Game1.instance.Content.Load<Texture2D>("Img/spielbeenden");
            endrec = new Rectangle(900, 570, 324, 104);

            //Background
            backg = Game1.instance.Content.Load<Texture2D>("Img/Hintergrund");
            backgrec = new Rectangle(0, 0, 1280 + 20, 720);


            // Pause Menue
            pausedTexture = Game1.instance.Content.Load<Texture2D>("Img/pause");
            pausedRectangle = new Rectangle(0, 0, pausedTexture.Width, pausedTexture.Height);
            btnPlay = new cButton();
            btnPlay.Load(Game1.instance.Content.Load<Texture2D>("Img/Weiter"), new Vector2(480, 350));
            btnQuit = new cButton();
            btnQuit.Load(Game1.instance.Content.Load<Texture2D>("Img/Spielbeenden"), new Vector2(480, 500));

            // Gameover Menue
            go = Game1.instance.Content.Load<Texture2D>("Img/gameover");
            goRec = new Rectangle(0, 0, 1280 + 20, 720);

            beC = Game1.instance.Content.Load<Texture2D>("Img/spielbeendenCrashed");
            beCRec = new Rectangle(900, 550, 324, 104);
            stC = Game1.instance.Content.Load<Texture2D>("Img/SpielstartCrashed");
            stCRec = new Rectangle(900, 400, 324, 104);

            win = Game1.instance.Content.Load<Texture2D>("Img/win");
            winRec = new Rectangle(0, 0, 1280 + 20, 720);
        }

        public void updateStartMenue(GameTime gt)
        {
            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            mouseRec = new Rectangle((int)mousePos.X - 10, (int)mousePos.Y - 10, 20, 20);

            // Intersect ist collsionsüberprüfung 
            if (mouseRec.Intersects(sbrec) && Mouse.GetState().LeftButton == ButtonState.Pressed  || Keyboard.GetState().IsKeyDown(Keys.G))
            {
                //Game1.instance.sound.stopStartmenueTrack();
                Game1.instance.gameState = Game1.GameState.gameSettings;
            }

            if (mouseRec.Intersects(endrec) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Game1.instance.Exit();
            }

        }

        public void drawStartMenue(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(backg, backgrec, Color.White);
            batch.Draw(sb, sbrec, Color.White);
            batch.Draw(end, endrec, Color.White);


            batch.End();
        }

        public void updatePauseMenue()
        {
            float lastTime = Game1.instance.timer.time;
            Game1.instance.timer.time = lastTime;

            MouseState mouse = Mouse.GetState();
            if (!pause)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    pause = true;
                    btnPlay.isClicked = false;
                    // Game1.instance.gameState = Game1.GameState.pause;
                }

            }
            else if (pause)
            {
                if (btnPlay.isClicked)
                {
                    pause = false;
                    mousePos = new Vector2(Game1.instance.mouse.X, Game1.instance.mouse.Y);
                }
                   
                if (btnQuit.isClicked)
                    Game1.instance.Exit();
                btnPlay.Update(mouse,0);
                btnQuit.Update(mouse,1);
            }

        }

        public void drawPauseMenue(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            // if (pause)
            // {
            spriteBatch.Draw(pausedTexture, pausedRectangle, Color.White);
            btnPlay.Draw(spriteBatch);
            btnQuit.Draw(spriteBatch);
            //}
            spriteBatch.End();
        }

        public void updateGameover()
        {
            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            mouseRec = new Rectangle((int)mousePos.X - 10, (int)mousePos.Y - 10, 20, 20);

            // Intersect ist collsionsüberprüfung 
            if (mouseRec.Intersects(stCRec) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                //Game1.instance.sound.stopStartmenueTrack();
                Game1.instance.sound.stopTrack();
                Game1.instance.gameState = Game1.GameState.gameSettings;
            }

            if (mouseRec.Intersects(beCRec) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Game1.instance.Exit();
            }
        }

        public void drawGameover(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(go, goRec, Color.White);
            batch.Draw(stC, stCRec, Color.White);
            batch.Draw(beC, beCRec, Color.White);
            batch.End();
        }


        public void updateWin()
        {
            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            mouseRec = new Rectangle((int)mousePos.X - 10, (int)mousePos.Y - 10, 20, 20);

            // Intersect ist collsionsüberprüfung 
            if (mouseRec.Intersects(stCRec) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                //Game1.instance.sound.stopStartmenueTrack();
                Game1.instance.sound.stopTrack();
                Game1.instance.gameState = Game1.GameState.gameSettings;
            }

            if (mouseRec.Intersects(beCRec) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Game1.instance.Exit();
            }
        }

        public void drawWin(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(win, winRec, Color.White);
            batch.Draw(sb, sbrec, Color.White);
            batch.Draw(end, endrec, Color.White);
            batch.End();
        }
    }
}
