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
    class Scheibe
    {
        Model target;
        Vector3 pos, rotation;
        



        public Scheibe(Model m, Vector3 position)
        {
            target = m;
            pos = position;
        }

        public void Update(GameTime gameTime)
        {

            rotation.Y += .05f;
        }

        public void Draw(GameTime gameTime)
        {
            Matrix planeWorld = Matrix.Identity;

            planeWorld = Matrix.Identity
                                * Matrix.CreateScale(0.5f)
                                * Matrix.CreateRotationX(.5f)
                                * Matrix.CreateRotationY(rotation.Y)
                                * Matrix.CreateTranslation(pos);

            foreach (ModelMesh mesh in target.Meshes)
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
