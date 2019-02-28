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
    public class Scheibe
    {
        Random rand = new Random();

        Model target;
        public Vector3 pos, rotation;
        public BoundingSphere sphere;
        Matrix sphereTranslation;
        public bool isDead;
        public bool posiblePos;

        public Scheibe(Model m, Vector3 position)
        {
            posiblePos = true;
            isDead = false;
            target = m;
            pos = position;
        }

        public void Update(GameTime gameTime)
        {
            rotation.Y += .05f;
        }

        public void Draw(GameTime gametime)
        {
            if (!isDead)
            {
                drawScheibe(gametime);
            }
        }

        public void drawScheibe(GameTime gameTime)
        {
            Matrix planeWorld = Matrix.Identity;

            planeWorld = Matrix.Identity
                                * Matrix.CreateScale(0.5f)
                                //* Matrix.CreateRotationX(.5f)
                                * Matrix.CreateRotationY(rotation.Y)
                                * Matrix.CreateTranslation(pos);
                                

            sphereTranslation = Matrix.CreateTranslation(pos);

            foreach (ModelMesh mesh in target.Meshes)
            {
                sphere = BoundingSphere.CreateMerged(sphere, mesh.BoundingSphere);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = planeWorld;
                    effect.View = Game1.instance.viewMatrix;
                    effect.Projection = Game1.instance.projectionMatrix;
                    effect.EnableDefaultLighting();

                    sphere.Center = sphereTranslation.Translation;
                    sphere.Radius = .3f;
                }
                mesh.Draw();
            }
            if (Game1.instance.debug)
                BoundingSphereRenderer.Render(sphere, Game1.instance.GraphicsDevice, Game1.instance.viewMatrix, Game1.instance.projectionMatrix, Color.Red);
        }
        // Ändert Position der Scheiben und stellt posiblePos auf "true"
        public void newPos()
        {
            if (posiblePos == false)
            {
                pos = new Vector3(rand.Next(-11, 11), rand.Next(1, 8), rand.Next(-18, 18));
                posiblePos = true;
            }
        }
    }
}
