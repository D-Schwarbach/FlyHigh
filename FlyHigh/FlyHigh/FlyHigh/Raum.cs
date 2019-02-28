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
    public class Raum : Microsoft.Xna.Framework.DrawableGameComponent
    {

        Model room;
        Model bed, couch, lowboy, plant1, plant2, rack, desk, chair, door;
        Raumobjekte bett, sofa, kommode, pflanze1, pflanze2, schrank, schreibtisch, stuhl, tuer;

        public Raum(Game game)
            : base(game)
        {
            loadContent(Game.Content);
            bett = new Raumobjekte(game, bed,new Vector3(0f, 1.4f, -14f), MathHelper.ToRadians(90) , 2f);
            sofa = new Raumobjekte(game, couch, new Vector3(-16f, 0f, -13f), MathHelper.ToRadians(90), 0.04f);
            kommode = new Raumobjekte(game, lowboy, new Vector3(16.8f, 0f, -14f), MathHelper.ToRadians(0), 1f);
            pflanze1 = new Raumobjekte(game, plant1, new Vector3(-15.8f, 0f, -7f), MathHelper.ToRadians(90), 0.004f);
            pflanze2 = new Raumobjekte(game, plant2, new Vector3(14.8f, 0f, 1.5f), MathHelper.ToRadians(90), 1f);
            schrank = new Raumobjekte(game, rack, new Vector3(16.8f, 0f, -10f), MathHelper.ToRadians(0), 1.68f);
            schreibtisch = new Raumobjekte(game, desk, new Vector3(16.8f, 0f, 9f), MathHelper.ToRadians(180), 1.5f);
            stuhl = new Raumobjekte(game, chair, new Vector3(13.0f, 0f, 9f), MathHelper.ToRadians(90), 1.1f);
            tuer = new Raumobjekte(game, door, new Vector3(-18f, 0f, 12f), MathHelper.ToRadians(90), 0.04f);

          //  tisch = new Raumobjekte(game, table, new Vector3(0f, 1.4f, -14f), MathHelper.ToRadians(0), 1f);
            
        }

        public void loadContent(ContentManager c)
        {
            room = c.Load<Model>("Raum");
            bed = c.Load<Model>("Bett");
            couch = c.Load<Model>("couch2");
            lowboy = c.Load<Model>("Kommode");
            plant1 = c.Load<Model>("Pflanze 1");
            plant2 = c.Load<Model>("Pflanze 2");
            rack = c.Load<Model>("Schrank");
            desk = c.Load<Model>("Schreibtisch");
            chair = c.Load<Model>("Stuhl");
            door = c.Load<Model>("tuer");
          //  table = c.Load<Model>("Tisch");

        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            draw();
            bett.Draw(gameTime);
            sofa.Draw(gameTime);
            kommode.Draw(gameTime);
            pflanze1.Draw(gameTime);
            pflanze2.Draw(gameTime);
            schrank.Draw(gameTime);
            schreibtisch.Draw(gameTime);
            stuhl.Draw(gameTime);
            tuer.Draw(gameTime);
           // tisch.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void draw()
        {
            Matrix planeWorld = Matrix.Identity;

            planeWorld = Matrix.Identity
                                * Matrix.CreateScale(2f);



            foreach (ModelMesh mesh in room.Meshes)
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
