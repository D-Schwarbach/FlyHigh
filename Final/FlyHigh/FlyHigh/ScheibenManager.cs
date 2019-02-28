using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyHigh
{
    public class ScheibenManager
    {
        public List<Scheibe> scheibenListe = new List<Scheibe>();
        public List<Scheibe> scheibenRemoveListe = new List<Scheibe>();
        Random rand = new Random();
        public int scheibenAnzahl;


        public ScheibenManager()
        {
            scheibenAnzahl = 19;
            Model target = Game1.instance.Content.Load<Model>("Scheibe");
            
            for (int i = 0; i <= scheibenAnzahl; i++)
            {
                Vector3 targetPos = new Vector3(rand.Next(-11, 11), rand.Next(1, 8), rand.Next(-18, 18));
                scheibenListe.Add(new Scheibe(target, targetPos));


            }
        }

        public void update(GameTime gameTime)
        {
            foreach (Scheibe s in scheibenListe)
            {
                s.Update(gameTime);
                if (s.isDead)
                    scheibenRemoveListe.Add(s);
            }

            foreach (Scheibe s in scheibenRemoveListe)
            {
                scheibenListe.Remove(s);
            }
            scheibenRemoveListe.Clear();
        }

        public void draw(GameTime gameTime)
        {

            // Restliche Scheiben werden angezeigt
            Game1.instance.spriteBatch.Begin();
            Game1.instance.spriteBatch.DrawString(Game1.instance.font,"Restliche Scheiben: " + scheibenListe.Count.ToString(""), new Vector2(50, 80), Color.White);
            Game1.instance.spriteBatch.DrawString(Game1.instance.font, "Highscore: " + Game1.instance.Highscore.ToString(""), new Vector2(50, 100), Color.White);
            Game1.instance.spriteBatch.End();

            foreach (Scheibe target in scheibenListe)
                target.Draw(gameTime);
        }
    }
}
