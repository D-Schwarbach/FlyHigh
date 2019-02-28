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
    public class Raumobjekte : Microsoft.Xna.Framework.DrawableGameComponent
    {

        Model objekt;
        Vector3 position;
        float rotation, scale;

        public Raumobjekte(Game game, Model model, Vector3 pos, float rot, float sca)
            : base(game)
          {
              objekt = model;
              position = pos;
              rotation = rot;
              scale = sca;
          }

        public override void Draw(GameTime gameTime)
        {
            draw();
        }

        private void draw()
        {
            Matrix planeWorld = Matrix.Identity;

            planeWorld = Matrix.Identity
                                * Matrix.CreateScale(scale)
                                * Matrix.CreateRotationY(rotation)
                                * Matrix.CreateTranslation(position);

            foreach (ModelMesh mesh in objekt.Meshes)
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
