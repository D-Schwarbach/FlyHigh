using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlyHigh
{
    public class IntersectionManager
    {
        public IntersectionManager()
        {

        }

        public void update()
        {
            CheckDiscCollideWithAny();
        
            CheckPlaneCollideWithObject();

            checkPlaneCollideWithRoom();

            CheckBulletCollideWithDisc();

        }

        // Kollision zwischen den Scheiben und allen Objekten + Flugzeug
        private void CheckDiscCollideWithAny()
        {
                foreach(Scheibe s in Game1.instance.scheibenManager.scheibenListe)
                {

                    // Mindestens eine Durchführung
                    do
                    {
                        // Kollisions-Check mit Flugzeug
                        if (s.sphere.Intersects(Game1.instance.player.sphere))
                        {
                            s.posiblePos = false;
                            Game1.instance.Highscore -= 50;
                        }

                        // Kollisions-Check mit Objekten
                        foreach(Raumobjekte o in Game1.instance.room.rObj)
                        {
                            if(s.sphere.Intersects(o.boundingBox))
                            {
                                s.posiblePos = false;
                            }
                        }
                        // Wenn Kollision erfolgt, wird neue Position bestimmt
                        if(s.posiblePos == false)
                              s.newPos();

                    } while (s.posiblePos == false); // Nochmalige durchführung, wenn kollidiert ist
                }
         }


        private void CheckBulletCollideWithDisc()
        {
            foreach(Bullet b in Game1.instance.schussManager.schussListe)
            {
                foreach (Scheibe s in Game1.instance.scheibenManager.scheibenListe)
                {
                    if (b.sphere.Intersects(s.sphere))
                    {
                        b.isDead = true;
                        s.isDead = true;
                        Game1.instance.Highscore += 100;
                    }
                }
            }
        }
        
        private void checkPlaneCollideWithRoom()
        {
            if(!Game1.instance.room.boundingBox.Intersects(Game1.instance.player.sphere))
            {
                Console.WriteLine("Plane collide with Room!");
                Game1.instance.sound.stopTrack();
                Game1.instance.gameState = Game1.GameState.gameover;
            }
        }
        private void CheckPlaneCollideWithObject()
        {
            for (int j = 0; j < Game1.instance.room.rObj.Count; j++)
            {
                if (Game1.instance.room.rObj[j].boundingBox.Intersects(Game1.instance.player.sphere))
                {
                    Console.WriteLine("box collide with object");
                    Game1.instance.sound.stopTrack();
                    Game1.instance.gameState = Game1.GameState.gameover;
                }
            }

        }
    }
}
