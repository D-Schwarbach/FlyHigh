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
        // BoundingBox
        public BoundingBox boundingBox;
        private BasicEffect lineEffect;
        public BoundingBoxRenderer bbRenderer = new BoundingBoxRenderer();
        public Color bbColor = Color.Blue;

        Model objekt;
        Vector3 position;
        float rotation, scale;
        Vector3 boxScale;

        public Raumobjekte(Game game, Model model, Vector3 pos, float rot, float sca, Vector3 boxScale)
            : base(game)
          {
              objekt = model;
              position = pos;
              rotation = rot;
              scale = sca;
              this.boxScale = boxScale;

              lineEffect = new BasicEffect(Game1.instance.GraphicsDevice);
              lineEffect.LightingEnabled = false;
              lineEffect.TextureEnabled = false;
              lineEffect.VertexColorEnabled = true;

              setBoundingBox();
          }
    
        public override void Draw(GameTime gameTime)
        {
            draw();
            if (Game1.instance.debug)
            {
                DrawBoundingBox(bbRenderer.CreateBoundingBoxBuffers(boundingBox, Game1.instance.GraphicsDevice, bbColor),
                    lineEffect, Game1.instance.GraphicsDevice, Game1.instance.viewMatrix, Game1.instance.projectionMatrix);
            }
        }

        public void draw()
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
        #region BoundingBox
        public BoundingBox getBoundingBox()
        {
            return boundingBox;
        }

        protected void setBoundingBox()
        {
            Matrix translation = Matrix.CreateScale(boxScale) 
                               * Matrix.CreateFromQuaternion(Quaternion.Identity) 
                               * Matrix.CreateTranslation(position);

            boundingBox = bbRenderer.CreateBoundingBox(objekt, translation);
            // 6.1f, 2.1f, 10.1f
        }

        protected void DrawBoundingBox(BoundingBoxRenderer bbRenderer, BasicEffect effect, GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            graphicsDevice.SetVertexBuffer(bbRenderer.Vertices);
            graphicsDevice.Indices = bbRenderer.Indices;

            effect.World = Matrix.Identity; // Matrix.CreateFromQuaternion(playerRotation) * Matrix.CreateTranslation(playerPosition);
            effect.View = view;
            effect.Projection = projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0,
                    bbRenderer.VertexCount, 0, bbRenderer.PrimitiveCount);
            }
        }
        #endregion
    }
}
