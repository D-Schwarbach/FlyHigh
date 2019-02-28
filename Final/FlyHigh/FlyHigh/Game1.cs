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

using OculusRift.Oculus;

namespace FlyHigh
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public int Highscore;

        public static Game1 instance;
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public GameTimer timer;
        public SpriteFont font;

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

        public Quaternion qCamRotation = Quaternion.Identity;
        public Vector3 rollLeftRight;
        public Vector3 CamPosition;


        /// <summary>
        /// Define active camera (view matrix).
        /// - FPV: First person view
        /// - TPV: Third person view
        /// - SV : Static view 
        /// Note: Chance style with F1-Key
        /// </summary>
        public enum CameraStyle { FPV, TPV, SV };
        public CameraStyle cameraStyle = CameraStyle.FPV;

        public enum GameState { startMenue, ingame, pause, gameSettings, gameover, win };
        public GameState gameState = GameState.startMenue;

        public Sounds sound;

        public SchussManager schussManager;
        public ScheibenManager scheibenManager;
        public IntersectionManager intersectionManager;

        //Modelauswahl
        public int model;

        //First Person View
        Texture2D kreuz;
        Rectangle kreuzRec;

        // Cockpit
        public Texture2D cockpit1, cockpit2;
        public Rectangle cockRec1, cockRec2;

        // Menues
        Menue startMenue;
        Settings settingMenue;


        #region Oculus Vars
        OculusClient oculusClient;
        Effect oculusRiftDistortionShader;
        RenderTarget2D renderTargetLeft;
        RenderTarget2D renderTargetRight;
        Texture2D renderTextureLeft;
        Texture2D renderTextureRight;
        float scaleImageFactor;
        float fov_x;
        float fov_d;
        //int IPD = 0;
        public static float aspectRatio;
        float yfov;
        float viewCenter;
        float eyeProjectionShift;
        float projectionCenterOffset;
        Matrix projCenter;
        Matrix projLeft;
        Matrix projRight;
        Matrix viewLeft;
        Matrix viewRight;
        float halfIPD;

        // View, Projection, Resolution
        public Matrix projection,
                      view;
        double resolutionX = 1280,
               resolutionY = 800;

        // Update ResolutionAndRenderTargets-Function
        private int viewportWidth;
        private int viewportHeight;
        private Rectangle sideBySideLeftSpriteSize;
        private Rectangle sideBySideRightSpriteSize;
        #endregion

        bool oculusActive = false;
        public bool debug = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;

            // Global game instance
            instance = this;

            // Game settings
            IsMouseVisible = true;
            if (!oculusActive)
            {
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
            }

            if (oculusActive) { 
                // Create the OculusClient
                oculusClient = new OculusClient();
                scaleImageFactor = 0.71f;

                // PresentationSettings
                graphics.PreferredBackBufferWidth = (int)Math.Ceiling(resolutionX / scaleImageFactor);
                graphics.PreferredBackBufferHeight = (int)Math.Ceiling(resolutionY / scaleImageFactor);

                graphics.IsFullScreen = true;
            }

            // Control settings
            lastMouseState = Mouse.GetState();
            Mouse.SetPosition(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
        }

        protected override void Initialize()
        {
            // Screen center
            centerX = GraphicsDevice.Viewport.Width / 2;
            centerY = GraphicsDevice.Viewport.Height / 2;
            Mouse.SetPosition(centerX, centerY);

            startMenue = new Menue();
            settingMenue = new Settings();
            sound = new Sounds();

        //    room = new Raum(this);
            player = new Flugzeug(this);

            // Init projection 
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), graphics.GraphicsDevice.Viewport.AspectRatio, .1f, 10000f);

            camPos = Vector3.Zero;

            if(oculusActive)
                InitOculus();
            // Dont forget to call InitOculus()
            base.Initialize();
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            //timer.loadContent(Content);
            player.loadContent(Content);
            //First Person View
            kreuz = Content.Load<Texture2D>("Img/kreuz");
            kreuzRec = new Rectangle(1280/2-35, 720/2-35, 70,70);
            
            
            
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

            // Switch gamestates
            switch (gameState)
            {
                case GameState.startMenue:
                    // Update Menue
                    sound.playStartmenueTrack();

                    startMenue.updateStartMenue(gameTime);
                    IsMouseVisible = true;
                    break;

                case GameState.ingame:                

                    player.update();
                    IsMouseVisible = false;

                    if (model == 1) 
                    sound.playInGameTrackFlieger();

                    if (model == 2)
                        sound.playInGameTrackSpace();

                    if (Keyboard.GetState().IsKeyDown(Keys.F1) && lastKb.IsKeyUp(Keys.F1) && cameraStyle == CameraStyle.TPV)
                        cameraStyle = CameraStyle.FPV;
                    else if (Keyboard.GetState().IsKeyDown(Keys.F1) && lastKb.IsKeyUp(Keys.F1) && cameraStyle == CameraStyle.FPV)
                        cameraStyle = CameraStyle.SV;
                    else if (Keyboard.GetState().IsKeyDown(Keys.F1) && lastKb.IsKeyUp(Keys.F1) && cameraStyle == CameraStyle.SV)
                        cameraStyle = CameraStyle.TPV;

                    if (cameraStyle == CameraStyle.TPV)
                        UpdateCameraThirdPerson();
                    if (cameraStyle == CameraStyle.FPV)
                        UpdateCameraFirstPerson();

                    schussManager.update();
                    scheibenManager.update(gameTime);
                    intersectionManager.update();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                         Game1.instance.gameState = Game1.GameState.pause;
                    }

                    // Win Bedingung

                    if (scheibenManager.scheibenListe.Count == 0)
                    {
                        sound.stopTrack();
                        Game1.instance.gameState = Game1.GameState.win;
                    }

                    timer.Update(gameTime);
                   
                    break;

                case GameState.pause:
                    // pausemenu updaten
                    startMenue.updatePauseMenue();
                    IsMouseVisible = true;

                    if (Keyboard.GetState().IsKeyDown(Keys.P))
                    {
                        Game1.instance.gameState = Game1.GameState.ingame;
                    }
                    break;
                case GameState.gameover:

                    sound.playGameover();
                    startMenue.updateGameover();
                    IsMouseVisible = true;
                    break;

                case GameState.win:
                    sound.playVictory();
                    startMenue.updateWin();
                    IsMouseVisible = true;
                    break;
            }

            lastKb = Keyboard.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.PowderBlue);

            if (!oculusActive)
            {
                drawGame(gameTime);
            }

            if (oculusActive)
            {
                // Oculus Setup
                SetProjectionOffset();

                DrawLeftEye();
                drawGame(gameTime);

                DrawRightEye();
                drawGame(gameTime);

                DrawOculusRenderTargets();
            }
            

            base.Draw(gameTime);
        }

        private void drawGame(GameTime gameTime)
        {
            
            switch (gameState)
            {
                case GameState.startMenue:
                    // Draw Menue
                    startMenue.drawStartMenue(spriteBatch);
                    break;

                case GameState.gameSettings:
                    sound.playStartmenueTrack();
                    settingMenue.update();
                    settingMenue.draw(spriteBatch);
                    break;

                case GameState.ingame:
                    // Draw ingame
                    room.Draw(gameTime);
                    schussManager.draw();
                    scheibenManager.draw(gameTime);
                    player.draw();
                    timer.Started = true;
                    timer.Draw(spriteBatch);
                    break;

                case GameState.pause:
                    // pausemenu draw
                    startMenue.drawPauseMenue(spriteBatch);
                    timer.Draw(spriteBatch);
                    break;

                case GameState.gameover:
                    startMenue.drawGameover(spriteBatch);
                    break;

                case GameState.win:
                    startMenue.drawWin(spriteBatch);
                    break;
            }

            // Zeichnen von Fadenkreuz in der FPV
            if (cameraStyle == CameraStyle.FPV && gameState == GameState.ingame)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(kreuz, kreuzRec, Color.White);
                if (model == 1)
                    spriteBatch.Draw(cockpit1, cockRec1, Color.White);
                else
                    spriteBatch.Draw(cockpit2, cockRec2, Color.White);

                spriteBatch.End();
            }

        }


        #region Controls
        private void UpdateCameraThirdPerson()
        {
            // Set camera offset and transform it with player rotation
            CamPosition = new Vector3(0, 0.2f, -1);
            CamPosition = Vector3.Transform(CamPosition, Matrix.CreateFromQuaternion(player.qPlayerRotation));

            // Add player position 
            CamPosition += player.playerPosition;

            // Look at player position
            Vector3 lookAt = player.playerPosition;

            // Define up vector and transform it with player rotation
            Vector3 up = new Vector3(0, 1, 0);
            up = Vector3.Transform(up, Matrix.CreateFromQuaternion(player.qPlayerRotation));

            // Define oculus rotation matrix
            Matrix oculusRot = Matrix.CreateFromQuaternion(OculusRift.Oculus.OculusClient.GetPredictedOrientation());

            // Set look at
            viewMatrix = Matrix.CreateLookAt(CamPosition, lookAt, up) * oculusRot;

        }

        private void UpdateCameraFirstPerson()
        {
            // Define oculus rotation matrix
            Matrix oculusRot = Matrix.CreateFromQuaternion(OculusRift.Oculus.OculusClient.GetPredictedOrientation());
            CamPosition = new Vector3(0, 0, .15f);
            CamPosition = Vector3.Transform(CamPosition, Matrix.CreateFromQuaternion(player.qPlayerRotation));
            CamPosition += player.playerPosition;
            Vector3 lookAtOffset = new Vector3(0, 0, 1);
            lookAtOffset = Vector3.Transform(lookAtOffset, Matrix.CreateFromQuaternion(player.qPlayerRotation));
            Vector3 lookAt = player.playerPosition + lookAtOffset;
            Vector3 up = new Vector3(0, 1, 0);
            up = Vector3.Transform(up, Matrix.CreateFromQuaternion(player.qPlayerRotation));
            viewMatrix = Matrix.CreateLookAt(CamPosition, lookAt, up) * oculusRot;// * Matrix.CreateTranslation(new Vector3(0, 0, 0));
        }

        public void UpdateControls()
        {
            // Turnspeed for mouse
            float turnSpeed = 0.2f;

            // Keyboard speed
            float speed = 0.1f;

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

            // if (mouse.LeftButton == ButtonState.Pressed)
            if (keyboard.IsKeyDown(Keys.P))

            // Alter keyboardstate muss aktualisiert werden -> für einmaliges Keyevent 
            lastKb = Keyboard.GetState();
        }
        #endregion

        #region Oculus Functions
        private void UpdateResolutionAndRenderTargets()
        {

            if (viewportWidth != GraphicsDevice.Viewport.Width || viewportHeight != GraphicsDevice.Viewport.Height)
            {
                viewportWidth = GraphicsDevice.Viewport.Width;
                viewportHeight = GraphicsDevice.Viewport.Height;
                sideBySideLeftSpriteSize = new Microsoft.Xna.Framework.Rectangle(0, 0, viewportWidth / 2, viewportHeight);
                sideBySideRightSpriteSize = new Microsoft.Xna.Framework.Rectangle(viewportWidth / 2, 0, viewportWidth / 2, viewportHeight);
            }
        }

        private void InitOculus()
        {
            // Load the Oculus Rift Distortion Shader
            // https://mega.co.nz/#!E4YkjJ6K!MuIDuB78NwgHsGgeONikDAT_OLJQ0ZeLXbfGF1OAhzw
            oculusRiftDistortionShader = Content.Load<Effect>(@"OculusRift");

            aspectRatio = (float)(OculusClient.GetScreenResolution().X * 0.5f / (float)(OculusClient.GetScreenResolution().Y));
            fov_d = OculusClient.GetEyeToScreenDistance();
            fov_x = OculusClient.GetScreenSize().Y * scaleImageFactor;
            yfov = 2.0f * (float)Math.Atan(fov_x / fov_d);

            // Set ProjectionMatrix
            projection = Matrix.CreatePerspectiveFieldOfView(yfov, aspectRatio, 1.0f, 100000.0f);

            // Init left and right RenderTarget
            renderTargetLeft = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight);
            renderTargetRight = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight);

            OculusClient.SetSensorPredictionTime(0, 0.03f);
            UpdateResolutionAndRenderTargets();
        }

        private void SetProjectionOffset()
        {
            viewCenter = OculusClient.GetScreenSize().X * 0.212f; // 0.25f
            eyeProjectionShift = viewCenter - OculusClient.GetLensSeparationDistance() * 0.5f;
            projectionCenterOffset = 4.0f * eyeProjectionShift / OculusClient.GetScreenSize().X;

            projCenter = projection;
            projLeft = Matrix.CreateTranslation(projectionCenterOffset, 0, 0) * projCenter;
            projRight = Matrix.CreateTranslation(-projectionCenterOffset, 0, 0) * projCenter;

            halfIPD = OculusClient.GetInterpupillaryDistance() * 0.5f;
            viewLeft = Matrix.CreateTranslation(halfIPD, 0, 0) * view;
            viewRight = Matrix.CreateTranslation(-halfIPD, 0, 0) * view;
        }

        private void DrawLeftEye()
        {
            GraphicsDevice.SetRenderTarget(renderTargetLeft);
            GraphicsDevice.Clear(Color.Black);
            view = viewLeft;
            projection = projLeft;
        }

        private void DrawRightEye()
        {
            GraphicsDevice.SetRenderTarget(renderTargetRight);
            GraphicsDevice.Clear(Color.Black);
            view = viewRight;
            projection = projRight;
        }

        private void DrawOculusRenderTargets()
        {
            // Set RenderTargets
            GraphicsDevice.SetRenderTarget(null);
            renderTextureLeft = (Texture2D)renderTargetLeft;
            renderTextureRight = (Texture2D)renderTargetRight;
            GraphicsDevice.Clear(Color.Black);

            //Set the four Distortion params of the oculus
            oculusRiftDistortionShader.Parameters["distK0"].SetValue(oculusClient.DistK0);
            oculusRiftDistortionShader.Parameters["distK1"].SetValue(oculusClient.DistK1);
            oculusRiftDistortionShader.Parameters["distK2"].SetValue(oculusClient.DistK2);
            oculusRiftDistortionShader.Parameters["distK3"].SetValue(oculusClient.DistK3);
            oculusRiftDistortionShader.Parameters["imageScaleFactor"].SetValue(scaleImageFactor);

            // Pass for left lens
            oculusRiftDistortionShader.Parameters["drawLeftLens"].SetValue(true);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, oculusRiftDistortionShader);
            spriteBatch.Draw(renderTextureLeft, sideBySideLeftSpriteSize, Microsoft.Xna.Framework.Color.White);
            spriteBatch.End();

            // Pass for right lens
            oculusRiftDistortionShader.Parameters["drawLeftLens"].SetValue(false);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, oculusRiftDistortionShader);
            spriteBatch.Draw(renderTextureRight, sideBySideRightSpriteSize, Microsoft.Xna.Framework.Color.White);
            spriteBatch.End();
        }

        private void ResetRiftOrientation()
        {
            bool resetOk;
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                resetOk = OculusClient.ResetSensorOrientation(0);
            }
        }
        #endregion
    }
}
