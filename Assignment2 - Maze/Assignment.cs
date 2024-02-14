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
        private Texture2D m_mrsaturn;
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
            maze = new Maze(15, 15);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            wallTexture = new Texture2D(GraphicsDevice, 1, 1);
            wallTexture.SetData(new[] { Color.White });

            // TODO: use this.Content to load your game content here
            m_ness = this.Content.Load<Texture2D>("Images/ness");
            m_mrsaturn = this.Content.Load<Texture2D>("Images/mrsaturn");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            var currentKeyboardState = Keyboard.GetState();
            Vector2 newPosition = maze.PlayerPosition;
            Vector2 previousPosition = maze.PlayerPosition;
            if (maze.shortestPath.Count > 0)
            {
                maze.hint = maze.shortestPath.Peek();
            }

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
                if (maze.CanMoveTo(newPosition))
                {
                    if (!maze.shortestPath.Contains(newPosition) && maze.CanMoveTo(newPosition))
                    {
                        maze.shortestPath.Push(previousPosition);
                    }
                    maze.PlayerPosition = newPosition;
                    maze.breadcrumbs.Add(newPosition);
                }
            }

            if (maze.shortestPath.Contains(maze.PlayerPosition))
            {
                maze.shortestPath.Pop();
            }

            // Toggle display of the shortest path on 'P' key press
            if (currentKeyboardState.IsKeyDown(Keys.P) && previousKeyboardState.IsKeyUp(Keys.P))
            {
                maze.displayShortestPath = !maze.displayShortestPath;
            }

            // Toggle hint display
            if (currentKeyboardState.IsKeyDown(Keys.H) && previousKeyboardState.IsKeyUp(Keys.H))
            {
                maze.displayHint = !maze.displayHint;
            }

            // Toggle breadcrumb  display
            if (currentKeyboardState.IsKeyDown(Keys.B) && previousKeyboardState.IsKeyUp(Keys.B))
            {
                maze.displayBreadcrumbs = !maze.displayBreadcrumbs;
            }

            previousKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_spriteBatch.Begin();
            maze.Draw(m_spriteBatch, wallTexture, GraphicsDevice, m_ness, m_mrsaturn);
            m_spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}