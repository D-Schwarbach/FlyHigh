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


    public class Game1 : Microsoft.Xna.Framework.Game
    {

        public static Game1 instance;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Controls
        public MouseState lastMouseState, mouse;
        public KeyboardState lastKb;

        // Screen center
        int centerX, centerY;

        // Player object
        public Flugzeug player;

        // Raum Objekte
        public Raum room;

        // Camera ------------------------
        public Vector3 camPos;
        // Var for cam angle by mouse input
        public Vector2 angle = Vector2.Zero;
        public Vector3 moveNearFar;
        public Vector3 moveLeftRight;
        public Matrix viewMatrix, projectionMatrix, cameraRotationMatrix;

        /// <summary>
        /// Define active camera (view matrix).
        /// - FPV: First person view
        /// - TPV: Third person view
        /// - SV : Static view 
        /// Note: Chance style with F1-Key
        /// </summary>
        public enum CameraStyle { FPV, TPV, SV };
        public CameraStyle cameraStyle = CameraStyle.TPV;


        Model target;
        List<Scheibe> scheibenListe = new List<Scheibe>();
        Random rand = new Random();
        int scheibenAnzahl;

        Model missile;
        List<Schuss> schussListe = new List<Schuss>();

        int schussAnz;

        Boolean schiessen = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Global game instance
            instance = this;

            // Game settings
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1289;
            graphics.PreferredBackBufferHeight = 720;

            // Control settings
            lastMouseState = Mouse.GetState();
            Mouse.SetPosition(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            scheibenListe = new List<Scheibe>();
            schussListe = new List<Schuss>();
            
            scheibenAnzahl = 10;
            schussAnz = 1;
            
        }


        protected override void Initialize()
        {
            // Screen center
            centerX = GraphicsDevice.Viewport.Width / 2;
            centerY = GraphicsDevice.Viewport.Height / 2;
            Mouse.SetPosition(centerX, centerY);

            // Initialisierung von Gameobjekten geschieht hier
            // Hier noch ein Trick: 
            // Wenn man das player-Objekt der Componenten-Datenstruktur hinzufügt, wird die
            // Update und Draw-Methode der Playerklasse automatisch sequentiell aufgerufen !
            // Allerdings muss die Playerklasse von DrawableComponent erben und auch der 
            // Konstruktor muss etwas modifiziert werden -> Siehe player class
            player = new Flugzeug(this);
            room = new Raum(this);

            Components.Add(player);
            Components.Add(room);

            // Init projection 
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), graphics.GraphicsDevice.Viewport.AspectRatio, .1f, 10000f);

            camPos = Vector3.Zero;

            base.Initialize();
        }


        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);



            Model target = Content.Load<Model>("Scheibe");

            for (int i = 0; i <= scheibenAnzahl; i++)
            {
                Vector3 targetPos = new Vector3(rand.Next(-11, 11), rand.Next(1, 8), rand.Next(-18, 18));
                scheibenListe.Add(new Scheibe(target, targetPos));
            }

            Model missile = Content.Load<Model>("Missile");

            for (int i = 0; i <= schussAnz; i++)
            {
                Vector3 targetPos = new Vector3(0,1,0);
                schussListe.Add(new Schuss(missile, targetPos));
            }

            player.loadContent(Content);

        }


        protected override void UnloadContent()
        {
            Content.Unload();

        }


        protected override void Update(GameTime gameTime)
        {
            

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

           

            // Wenn Klasse nicht in der Componenten Datenstruktur enthalten ist, muss Draw manuell aufgerufen werden
            //player.Update(gameTime);

            UpdateControls();
            foreach (Scheibe target in scheibenListe)
            {
                target.Update(gameTime);
            }

            foreach (Schuss missile in schussListe)
            {
                missile.Update(gameTime);                
            }
            



            base.Update(gameTime);
        }


        public void UpdateControls()
        {
            // Turnspeed for mouse
            float turnSpeed = 0.4f;

            // Keyboard speed
            float speed = 0.2f;

            // Get mouse and keyboard
            KeyboardState keyboard = Keyboard.GetState();

            // Diese oder vorherige Mauspos holen
            mouse = Mouse.GetState();

            // Vorherige Mausposition speichern
            // Vector2 oldMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            // maus auf bildschirmmitte zentrieren
            Mouse.SetPosition(centerX, centerY);

            // Mouse pitch (neigen)
            angle.X += MathHelper.ToRadians((mouse.Y - centerY) * turnSpeed);

            // Mouse yaw (gieren)
            angle.Y += MathHelper.ToRadians((mouse.X - centerX) * turnSpeed);

            //Console.WriteLine("Deg X: " + (oldMousePos.Y - centerY) * turnSpeed + " | Deg Y: " + (oldMousePos.X - centerX) * turnSpeed);
            //Console.WriteLine("Rad X: " + angle.X + " | Rad Y: " + angle.Y);

            // Move to direction we are looking at
            moveNearFar = Vector3.Normalize(new Vector3((float)Math.Sin(-angle.Y) * (float)Math.Cos(angle.X), (float)Math.Sin(angle.X), (float)Math.Cos(-angle.Y) * (float)Math.Cos(angle.X)));
            moveLeftRight = Vector3.Normalize(new Vector3((float)Math.Cos(angle.Y), 0f, (float)Math.Sin(angle.Y)));

            if (keyboard.IsKeyDown(Keys.W))
                camPos -= moveNearFar * speed;

            if (keyboard.IsKeyDown(Keys.S))
                camPos += moveNearFar * speed;

            if (keyboard.IsKeyDown(Keys.D))
                camPos += moveLeftRight * speed;

            if (keyboard.IsKeyDown(Keys.A))
                camPos -= moveLeftRight * speed;

            if (keyboard.IsKeyDown(Keys.E))
                camPos += Vector3.Up * speed;

            if (keyboard.IsKeyDown(Keys.Q))
                camPos += Vector3.Down * speed;

            // Keyevent wird nur einmalig ausgeführt
            if (keyboard.IsKeyDown(Keys.F1) && lastKb.IsKeyUp(Keys.F1) && cameraStyle == CameraStyle.TPV)
                cameraStyle = CameraStyle.FPV;
            else if (keyboard.IsKeyDown(Keys.F1) && lastKb.IsKeyUp(Keys.F1) && cameraStyle == CameraStyle.FPV)
                cameraStyle = CameraStyle.SV;
            else if (keyboard.IsKeyDown(Keys.F1) && lastKb.IsKeyUp(Keys.F1) && cameraStyle == CameraStyle.SV)
                cameraStyle = CameraStyle.TPV;

            // Set view matrix third person view
            if (cameraStyle == CameraStyle.TPV)
            {
                // Set rotation and view matrix
                cameraRotationMatrix = Matrix.Identity * Matrix.CreateRotationY(angle.Y);
                viewMatrix = Matrix.Identity * Matrix.CreateTranslation(-camPos) * cameraRotationMatrix; // * Matrix.Invert(Matrix.CreateTranslation(Vector3.Transform(offset, cameraRotationMatrix)));
                IsMouseVisible = true;
            }

            // Set view matrix for debug first person view
            if (cameraStyle == CameraStyle.FPV)
            {
                // Set rotation and view matrix
                cameraRotationMatrix = Matrix.Identity * Matrix.CreateRotationY(angle.Y) * Matrix.CreateRotationX(angle.X);
                viewMatrix = Matrix.Identity * Matrix.CreateTranslation(-camPos) * cameraRotationMatrix;
                IsMouseVisible = true;
            }

            // Set view matrix for static view 
            if (cameraStyle == CameraStyle.SV)
            {
                Vector3 staticViewPosition = new Vector3(0.0f, 0.0f, -250.0f);
                cameraRotationMatrix = Matrix.Identity * Matrix.CreateRotationY(angle.Y);
                viewMatrix = Matrix.CreateLookAt(staticViewPosition, camPos, Vector3.Up);
                IsMouseVisible = false;
            }

            if (mouse.LeftButton == ButtonState.Pressed)
                schiessen = true;

            // Alter keyboardstate muss aktualisiert werden -> für einmaliges Keyevent 
            lastKb = Keyboard.GetState();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Indigo);
            

            // Wenn Klasse nicht in der Componenten Datenstruktur enthalten ist, muss Draw manuell aufgerufen werden
            //player.Draw(gameTime);

            foreach (Scheibe target in scheibenListe)
            {
                target.Draw(gameTime);
            }


            foreach (Schuss missile in schussListe)
            {
                if (schiessen == true)
                {
                    missile.Draw(gameTime);
                }
                
            }
                    
            base.Draw(gameTime);
        }
    }
}
