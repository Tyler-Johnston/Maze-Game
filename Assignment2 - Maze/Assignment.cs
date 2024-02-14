using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS5410
{
    public class Assignment : Game
    {
        private GraphicsDeviceManager m_graphics;
        private SpriteBatch m_spriteBatch;
        private Maze maze;
        private Texture2D wallTexture;
        private Texture2D m_ness;
        private KeyboardState previousKeyboardState = Keyboard.GetState();

        public Assignment()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            maze = new Maze(5, 5);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            wallTexture = new Texture2D(GraphicsDevice, 1, 1);
            wallTexture.SetData(new[] { Color.White });

            // TODO: use this.Content to load your game content here
            m_ness = this.Content.Load<Texture2D>("Images/ness");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            var currentKeyboardState = Keyboard.GetState();
            Vector2 newPosition = maze.PlayerPosition;

            if (currentKeyboardState.IsKeyDown(Keys.Right) && previousKeyboardState.IsKeyUp(Keys.Right))
            {
                newPosition += new Vector2(1, 0);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Left) && previousKeyboardState.IsKeyUp(Keys.Left))
            {
                newPosition += new Vector2(-1, 0);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up))
            {
                newPosition += new Vector2(0, -1);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down))
            {
                newPosition += new Vector2(0, 1);
            }

            // Check if the new position is within the maze bounds
            if (newPosition.X >= 0 && newPosition.X < maze.width && newPosition.Y >= 0 && newPosition.Y < maze.height)
            {
                // Check for walls in the maze at the new position
                if (maze.CanMoveTo(newPosition))
                {
                    maze.PlayerPosition = newPosition;
                }
            }

            // Toggle display of the shortest path on 'P' key press
            if (currentKeyboardState.IsKeyDown(Keys.P) && previousKeyboardState.IsKeyUp(Keys.P))
            {
                maze.displayShortestPath = !maze.displayShortestPath;
            }


            previousKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_spriteBatch.Begin();
            maze.Draw(m_spriteBatch, wallTexture, GraphicsDevice, m_ness);
            m_spriteBatch.End();
            base.Draw(gameTime);
        }
        
    }

}