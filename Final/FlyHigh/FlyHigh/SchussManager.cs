using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyHigh
{
    public class SchussManager
    {
        public Model missile, laser;
        public List<Bullet> schussListe = new List<Bullet>();
        public List<Bullet> schussRemoveListe = new List<Bullet>();

        KeyboardState lkb;
        public SchussManager()
        {
            if (Game1.instance.model == 1)
                missile = Game1.instance.Content.Load<Model>("Missile");
            else
                missile = Game1.instance.Content.Load<Model>("Lazzor");
            lkb = Keyboard.GetState();
        }

        public void update() {


            if (Keyboard.GetState().IsKeyDown(Keys.Space) && lkb.IsKeyUp(Keys.Space))
            {
                if (Game1.instance.model == 1)
                    Game1.instance.sound.playFliegerSchussSound();
                else
                    Game1.instance.sound.playSpaceSchussSound();

                schussListe.Add(new Bullet(Game1.instance.player.playerPosition,
                                new Vector3(0.05f, 0.05f, 0.05f),
                                Vector3.Right,
                                Game1.instance.player.qPlayerRotation,
                                missile,
                                0.5f,
                                Game1.instance.angle.X));
            }

            foreach (Bullet m in schussListe)
            {
                m.Update();
                if (m.isDead)
                    schussRemoveListe.Add(m);
            }


            foreach (Bullet m in schussRemoveListe)
            {
                schussListe.Remove(m);
            }
            schussRemoveListe.Clear();

            lkb = Keyboard.GetState();
        }

        public void draw()
        {
            foreach (Bullet m in schussListe)
            {
                m.Draw();
            }
        }
       
    }
}
