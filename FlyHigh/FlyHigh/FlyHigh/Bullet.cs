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
    class Bullet
    {
        Model missile;
        public Vector3 spawnPos, spawnScale, addPos, offset, finOffset, finX;
        public Matrix rotation;
        float finalSpeed, speed, xRot;

        public Bullet(Vector3 spawnPos, Vector3 spawnScale, Vector3 offset, Matrix rotation, Model missile, float speed, float xRot)
        {
            this.spawnPos = spawnPos;
            this.spawnScale = spawnScale;
            this.offset = offset;
            this.missile = missile;// Game1.instance.Content.Load<Model>(@"Assets/Weapons/LaserBeam");
            this.speed = speed;
            this.rotation = rotation;//Game1.instance.playerOne.PlayerRotation;
            this.xRot = xRot;

        }

        public void Update()
        {
            finalSpeed += speed;
            addPos = Vector3.Transform(new Vector3(0.0f, 0.0f, -finalSpeed), rotation);
            //finOffset = Vector3.Transform(offset, rotation);
            //finX = Vector3.Transform(new Vector3(0.0f, 0.0f, -finalSpeed), Matrix.CreateRotationX(-xRot));
            addPos = Vector3.Transform(addPos, Matrix.CreateRotationX(-xRot));

        }

        public void Draw()
        {
            //Switch Offset for Single/DoubleFire in WeaponManager
            DrawSingleFire();
            //DrawDoubleFire();
        }

        public void DrawSingleFire()
        {

            foreach (ModelMesh mesh in this.missile.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.World = Matrix.Identity 
                                 * Matrix.CreateScale(spawnScale)
                                 * Matrix.CreateRotationX(Game1.instance.angle.X)
                                 * rotation
                                 * Matrix.CreateTranslation(spawnPos)
                                 * Matrix.CreateTranslation(addPos);
                                 //* Matrix.CreateTranslation(finX);
                    effect.View = Game1.instance.viewMatrix;
                    effect.Projection = Game1.instance.projectionMatrix;
                }
                mesh.Draw();
            }
        }
    }

    //class Schuss
    //{
    //    Model missile;
    //    Vector3 pos;
        



    //    public Schuss(Model m, Vector3 position)
    //    {
    //        missile = m;
    //        pos = position;
    //    }

    //    public void Update(GameTime gameTime)
    //    {
    //        pos.Z -= 0.1f;
            
    //    }

    //    public void Draw(GameTime gameTime)
    //    {
    //        Matrix planeWorld = Matrix.Identity;

    //        planeWorld = Matrix.Identity
    //                            * Matrix.CreateScale(0.2f)
    //                            //* Matrix.CreateRotationX(.5f)
    //                            * Matrix.CreateTranslation(pos);

    //        foreach (ModelMesh mesh in missile.Meshes)
    //        {
    //            foreach (BasicEffect effect in mesh.Effects)
    //            {
    //                effect.World = planeWorld;
    //                effect.View = Game1.instance.viewMatrix;
    //                effect.Projection = Game1.instance.projectionMatrix;
    //                effect.EnableDefaultLighting();
    //            }
    //            mesh.Draw();
    //        }
    //    }
    //}
}
