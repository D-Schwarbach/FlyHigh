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

    public class Flugzeug
    {
        Model plane1;
        Model plane2;
        Texture2D texture1;
        Texture2D texture2;

        public Vector3 playerPosition;
        public Matrix mPlayerRotation;
        public Quaternion qPlayerRotation;
        Quaternion calculatedRotation;

        public float playerSpeed;
        float playerLeftRightRot;
        float playerUpDownRot;
        float sensitivity = 0.003f;
        public float speedToAdd = 0.003f;
        public float maxSpeed = 0.2f;

        public BoundingSphere sphere;
        KeyboardState kbState;

        public Flugzeug(Game game)
        {

            qPlayerRotation = Quaternion.Identity;
            //cockpit
            
                Game1.instance.cockpit1 = Game1.instance.Content.Load<Texture2D>("Img/cockpit1");
                Game1.instance.cockRec1 = new Rectangle(0, 0, 1280, 720);
            
                Game1.instance.cockpit2 = Game1.instance.Content.Load<Texture2D>("Img/cockpit2");
                Game1.instance.cockRec2 = new Rectangle(0, 0, 1280, 720);
            
        }

        public void loadContent(ContentManager c)
        {
            plane1 = c.Load<Model>("plane1");
            plane2 = c.Load<Model>("plane2");
            texture1 = c.Load<Texture2D>("texture1");
            texture2 = c.Load<Texture2D>("texture2");

        }

        public void update()
        {
            KeyboardControls();
            MouseControls();
            MoveForward();
        }

        public void draw()
        {
            Matrix PlayerTransformation = Matrix.CreateScale(new Vector3(0.1f, 0.1f, 0.1f))

                                        * Matrix.CreateFromQuaternion(qPlayerRotation)
                                        * Matrix.CreateTranslation(playerPosition);


            Matrix sphereTrans1 = Matrix.Identity
                                * Matrix.CreateFromQuaternion(qPlayerRotation)
                                * Matrix.CreateTranslation(playerPosition);


            if (Game1.instance.model == 1)
            {
                foreach (ModelMesh mesh in plane1.Meshes)
                {
                    sphere = BoundingSphere.CreateMerged(sphere, mesh.BoundingSphere);

                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = PlayerTransformation;
                        effect.View = Game1.instance.viewMatrix;
                        effect.Projection = Game1.instance.projectionMatrix;
                        effect.EnableDefaultLighting();
                        effect.TextureEnabled = true;
                        effect.Texture = texture1;

                        sphere.Center = sphereTrans1.Translation;
                        sphere.Radius = 0.3f;
                    }
                    mesh.Draw();
                }
            }
            else
            {
                foreach (ModelMesh mesh in plane2.Meshes)
                {
                    sphere = BoundingSphere.CreateMerged(sphere, mesh.BoundingSphere);

                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = PlayerTransformation;
                        effect.View = Game1.instance.viewMatrix;
                        effect.Projection = Game1.instance.projectionMatrix;
                        effect.EnableDefaultLighting();
                        effect.TextureEnabled = true;
                        effect.Texture = texture2;

                        sphere.Center = sphereTrans1.Translation;
                        sphere.Radius = 0.1f;

                    }
                    mesh.Draw();
                }
            }
            if (Game1.instance.debug)
                BoundingSphereRenderer.Render(sphere, Game1.instance.GraphicsDevice, Game1.instance.viewMatrix, Game1.instance.projectionMatrix, Color.Red);
        }

        public void resetPlayer()
        {
            playerPosition = new Vector3(0, 2, 0);
            playerSpeed = 0f;
        }


        #region Controls
        private void KeyboardControls()
        {
            // Rotation of the Ship
            kbState = Keyboard.GetState();


            if (kbState.IsKeyDown(Keys.Down) || kbState.IsKeyDown(Keys.W))
                playerUpDownRot += sensitivity;

            if (kbState.IsKeyDown(Keys.Up) || kbState.IsKeyDown(Keys.S))
                playerUpDownRot -= sensitivity;

            if (kbState.IsKeyDown(Keys.Left) || kbState.IsKeyDown(Keys.A))
                playerLeftRightRot += sensitivity / 2;

            if (kbState.IsKeyDown(Keys.Right) || kbState.IsKeyDown(Keys.D))
                playerLeftRightRot -= sensitivity / 2;

            calculatedRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), playerLeftRightRot)
                               * Quaternion.CreateFromAxisAngle(new Vector3(-1, 0, 0), playerUpDownRot);
            qPlayerRotation = calculatedRotation;

            // Speed of the Ship 
            // Vorwärts, wenn negative maxSpeed nicht erreicht ist
            if (kbState.IsKeyDown(Keys.W) && (playerSpeed > -maxSpeed))
                playerSpeed += -speedToAdd;

            // Rückwärts, wenn die hälfte der MaxSpeed nicht erreicht ist
            if (kbState.IsKeyDown(Keys.S) && (playerSpeed < (maxSpeed / 3)))
                playerSpeed += speedToAdd;
        }

        private void MouseControls()
        {
            //mState = Mouse.GetState();
            Game1.instance.mouse = Mouse.GetState();

            playerLeftRightRot = 0.0f;
            playerUpDownRot = 0.0f;

            playerLeftRightRot -= (Game1.instance.mouse.X - (Game1.instance.GraphicsDevice.Viewport.Width / 2));
            playerUpDownRot -= (Game1.instance.mouse.Y - (Game1.instance.GraphicsDevice.Viewport.Height / 2));

            calculatedRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), playerLeftRightRot * 0.01f)
                               * Quaternion.CreateFromAxisAngle(new Vector3(-1, 0, 0), playerUpDownRot * 0.01f);
            qPlayerRotation = calculatedRotation;

        }

        private void MoveForward()
        {
            Vector3 calculatedVector = Vector3.Transform(new Vector3(0, 0, -playerSpeed), Matrix.CreateFromQuaternion(qPlayerRotation));
            playerPosition += calculatedVector;
        }
        #endregion

    }
}
