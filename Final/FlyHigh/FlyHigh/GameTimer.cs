using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace FlyHigh
{
    public class GameTimer : GameComponent
    {
        private String text;
        public float time;

        private bool started;
        private bool paused;
        private bool finished;

        public GameTimer(Game game, float startTime)
            : base(game)
        {
            time = startTime * 60;
            started = false;
            paused = false;
            finished = false;
            Text = "";
        }

        #region Properties

        public String Text
        {
            get { return text; }
            set { text = value; }
        }

        public bool Started
        {
            get { return started; }
            set { started = value; }

        }

        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }

        public bool Finished
        {
            get { return finished; }
            set { finished = value; }
        }

        #endregion

        public override void Update(GameTime gameTime)
        {

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (started)
            {
                if (!paused)
                {
                    if (time > 0)
                        time -= deltaTime;
                    else
                        Game1.instance.gameState = Game1.GameState.gameover;
                }
            }

            Text = time.ToString("0");

            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(Game1.instance.font, "Restliche Zeit: " + text, new Vector2(50, 60), Color.White);
            spriteBatch.End();
        }

        public void updateTime(float t)
        {
            time = t * 60;
        }
    }
}
