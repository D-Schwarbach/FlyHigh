using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FlyHigh
{
    class Schuss
    {
        Model missile;
        Vector3 pos;
        



        public Schuss(Model m, Vector3 position)
        {
            missile = m;
            pos = position;
        }

        public void Update(GameTime gameTime)
        {
            pos.Z -= 0.1f;
            
        }

        public void Draw(GameTime gameTime)
        {
            Matrix planeWorld = Matrix.Identity;

            planeWorld = Matrix.Identity
                                * Matrix.CreateScale(0.2f)
                                //* Matrix.CreateRotationX(.5f)
                                * Matrix.CreateTranslation(pos);

            foreach (ModelMesh mesh in missile.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = planeWorld;
                    effect.View = Game1.instance.viewMatrix;
                    effect.Projection = Game1.instance.projectionMatrix;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
