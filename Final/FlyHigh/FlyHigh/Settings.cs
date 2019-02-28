using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyHigh
{
    public class Settings
    {
        public int time;
        // Buttons
        Texture2D fore;
        Rectangle fRec;

        Texture2D back;
        Rectangle bRec;

        Texture2D button2, button3, button5, highlightButton;
        Rectangle buRec1, buRec2, buRec3, hbRec;

        //Models
        Texture2D model1, model2, highlight;
        Rectangle m1Rec, m2Rec, hRec;

        // Backrounds
        Texture2D backg;
        Rectangle backgrec;


        // Mouse
        Texture2D mouseTex;
        Rectangle mouseRec;
        Vector2 mousePos;

       // bool debug = true;

        public Settings()
        {
            loadContent();
            hRec = m1Rec;
            hbRec = buRec3;
            Game1.instance.model = 1;
            time = 3;
            
            update();
        }

        private void loadContent()
        {

            // Mouse
            mouseTex = Game1.instance.Content.Load<Texture2D>("Img/MouseRec");
            highlight = Game1.instance.Content.Load<Texture2D>("Img/HighLight");


            // Buttons
            fore = Game1.instance.Content.Load<Texture2D>("Img/ButtonForward");
            fRec = new Rectangle(1080, 600, 100, 100);

            back = Game1.instance.Content.Load<Texture2D>("Img/ButtonBack");
            bRec = new Rectangle(50, 600, 100, 100);

            model1 = Game1.instance.Content.Load<Texture2D>("Img/Model1");
            m1Rec = new Rectangle(200, 100, 285, 285);

            model2 = Game1.instance.Content.Load<Texture2D>("Img/Model2");
            m2Rec = new Rectangle(800, 100, 285, 285);

            button2 = Game1.instance.Content.Load<Texture2D>("Img/1min");
            buRec1 = new Rectangle(250, 500, 200, 70);
            button3 = Game1.instance.Content.Load<Texture2D>("Img/2min");
            buRec2 = new Rectangle(550, 500, 200, 70);
            button5 = Game1.instance.Content.Load<Texture2D>("Img/3min");
            buRec3 = new Rectangle(850, 500, 200, 70);
            highlightButton = Game1.instance.Content.Load<Texture2D>("Img/HighlightButton");
           
            //Background
            backg = Game1.instance.Content.Load<Texture2D>("Img/HintergrundSetting");
            backgrec = new Rectangle(0, 0, 1280+20, 720);
        }

        public void update()
        {

            mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            mouseRec = new Rectangle((int)mousePos.X - 10, (int)mousePos.Y - 10, 20, 20);

            // Intersect ist collsionsüberprüfung 
            if (mouseRec.Intersects(fRec) && Mouse.GetState().LeftButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.P))
            {

                // Player einstellen
                Game1.instance.player.resetPlayer();
                Game1.instance.player.sphere.Center = Game1.instance.player.playerPosition;
                Game1.instance.cameraStyle = Game1.CameraStyle.FPV;
                Game1.instance.Highscore = 0;
                


                Game1.instance.room = new Raum(Game1.instance);
                Game1.instance.timer = new GameTimer(Game1.instance, this.time);
                Game1.instance.sound.stopTrack();

                // Kollisionen einstellen
                Game1.instance.intersectionManager = new IntersectionManager();
                Game1.instance.schussManager = new SchussManager();
                Game1.instance.scheibenManager = new ScheibenManager();

                // Starte Spiel
                Game1.instance.gameState = Game1.GameState.ingame;
                Mouse.SetPosition(646,371);            
            }

            if (mouseRec.Intersects(bRec) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Game1.instance.gameState = Game1.GameState.startMenue;
                Mouse.SetPosition(646, 371);
            }

            changePlane();
            changeTimer();
        }

        public void draw(SpriteBatch batch)
        {
            batch.Begin();
            //White für Standartfarbe bei Texturen
            batch.Draw(backg, backgrec, Color.White);

            batch.Draw(fore, fRec, Color.White);
            batch.Draw(back, bRec, Color.White);

            batch.Draw(model1, m1Rec, Color.White);
            batch.Draw(model2, m2Rec, Color.White);

            batch.Draw(button2, buRec1, Color.White);
            batch.Draw(button3, buRec2, Color.White);
            batch.Draw(button5, buRec3, Color.White);  

            batch.Draw(highlight, hRec, Color.White);
            batch.Draw(highlightButton, hbRec, Color.White);


            batch.End();
        }

        public void changePlane()
        {
            // Highlight des ausgewählten Flugzeugs

            if (mouseRec.Intersects(m1Rec) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                hRec = m1Rec;
                Game1.instance.model = 1;
            }

            if (mouseRec.Intersects(m2Rec) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                hRec = m2Rec;
                Game1.instance.model = 2;
            }
        }
        public void changeTimer()
        {

            //Highligth der Timer Buttons und Auswahl der Zeit
            if (mouseRec.Intersects(buRec1) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                hbRec = buRec1;
                time = 1;
            }

            if (mouseRec.Intersects(buRec2) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                hbRec = buRec2;
                time = 2;
            }

            if (mouseRec.Intersects(buRec3) && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                hbRec = buRec3;
                time = 3;
            }

        }
    }
}


