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
    public class Bullet
    {
        Model missile;
        public Vector3 spawnPos, spawnScale, addPos, offset, finOffset, finX;
        public Quaternion rotation;
        float finalSpeed, speed, xRot;
        public bool isDead = false;

        public BoundingSphere sphere;
        Matrix sphereTranslation;
        float bulletRange = 20.0f;

        public Bullet(Vector3 spawnPos, Vector3 spawnScale, Vector3 offset, Quaternion rotation, Model missile, float speed, float xRot)
        {
            this.spawnPos = spawnPos;
            this.spawnScale = spawnScale;
            this.offset = offset;
            this.missile = missile;
            this.speed = speed;
            this.rotation = rotation;
            this.xRot = xRot;
        }

        public void Update()
        {
            finalSpeed += speed;
            addPos = Vector3.Transform(new Vector3(0, 0, -finalSpeed), Matrix.CreateFromQuaternion(rotation));

            // Check distance to  bulletspawnpos
            if (-finalSpeed <= -bulletRange)
                isDead = true;
        }

        public void Draw()
        {
            DrawBullet();
        }

        public void DrawBullet()
        {
            Matrix bullets = Matrix.Identity
                    * Matrix.CreateScale(spawnScale)
                    * Matrix.CreateRotationY(MathHelper.ToRadians(180.0f))
                    * Matrix.CreateFromQuaternion(rotation)
                    * Matrix.CreateTranslation(spawnPos)
                    * Matrix.CreateTranslation(-addPos);

            sphereTranslation = Matrix.Identity
                    * Matrix.CreateFromQuaternion(rotation)
                    * Matrix.CreateTranslation(spawnPos)
                    * Matrix.CreateTranslation(-addPos);

            foreach (ModelMesh mesh in this.missile.Meshes)
            {
                sphere = BoundingSphere.CreateMerged(sphere, mesh.BoundingSphere);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.World = bullets;
                    effect.View = Game1.instance.viewMatrix;
                    effect.Projection = Game1.instance.projectionMatrix;

                    sphere.Center = sphereTranslation.Translation;
                    sphere.Radius = 0.2f;
                }
                mesh.Draw();
            }
            if(Game1.instance.debug)
                BoundingSphereRenderer.Render(sphere, Game1.instance.GraphicsDevice, Game1.instance.viewMatrix, Game1.instance.projectionMatrix, Color.Red);
        }
    }
}
