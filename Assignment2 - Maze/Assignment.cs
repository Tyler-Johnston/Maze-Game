using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS5410
{
    public class Assignment : Game
    {
        private GraphicsDeviceManager m_graphics;

        public enum GameState
        {
            MainMenu,
            Playing
        }
        private GameState currentState = GameState.MainMenu;

        private SpriteBatch m_spriteBatch;
        private int m_cellSize = 30;
        private Maze maze;
        private Texture2D wallTexture;
        private Texture2D m_ness;
        private Texture2D m_mrsaturn;
        private Texture2D m_dog;
        private Texture2D m_background;

        private Texture2D m_grass;
        private SpriteFont m_font;
        private KeyboardState previousKeyboardState = Keyboard.GetState();

        public Assignment()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            AdjustWindowSize(30, 20);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            wallTexture = new Texture2D(GraphicsDevice, 1, 1);
            wallTexture.SetData(new[] { Color.White });

            m_ness = this.Content.Load<Texture2D>("Images/ness");
            m_mrsaturn = this.Content.Load<Texture2D>("Images/mrsaturn");
            m_dog = this.Content.Load<Texture2D>("Images/dog");
            m_background = this.Content.Load<Texture2D>("Images/background");
            m_grass = this.Content.Load<Texture2D>("Images/grass");
            m_font = this.Content.Load<SpriteFont>("Fonts/PixelifySans");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            var currentKeyboardState = Keyboard.GetState();
            // New Game 5x5
            if (currentKeyboardState.IsKeyDown(Keys.F1) && previousKeyboardState.IsKeyUp(Keys.F1))
            {
                maze = new Maze(5, 5);
                currentState = GameState.Playing;
            }

            // New Game 10x10
            if (currentKeyboardState.IsKeyDown(Keys.F2) && previousKeyboardState.IsKeyUp(Keys.F2))
            {
                maze = new Maze(10, 10);
                currentState = GameState.Playing;
            }

            // New Game 15x15
            if (currentKeyboardState.IsKeyDown(Keys.F3) && previousKeyboardState.IsKeyUp(Keys.F3))
            {
                maze = new Maze(15, 15);
                currentState = GameState.Playing;
            }

            // New Game 20x20
            if (currentKeyboardState.IsKeyDown(Keys.F4) && previousKeyboardState.IsKeyUp(Keys.F4))
            {
                maze = new Maze(20, 20);
                currentState = GameState.Playing;
            }

            // Display High Scores
            if (currentKeyboardState.IsKeyDown(Keys.F5) && previousKeyboardState.IsKeyUp(Keys.F5))
            {
                // Placeholder for displaying high scores
                // This could toggle a boolean flag that you check in Draw to display scores
                Console.WriteLine("Display High Scores - implement UI update accordingly");
            }

            // Display Credits
            if (currentKeyboardState.IsKeyDown(Keys.F6) && previousKeyboardState.IsKeyUp(Keys.F6))
            {
                // Placeholder for displaying credits
                // This could toggle a boolean flag that you check in Draw to display credits
                Console.WriteLine("Display Credits - implement UI update accordingly");
            }

            // Back to Main Menu
            if (currentKeyboardState.IsKeyDown(Keys.F7) && previousKeyboardState.IsKeyUp(Keys.F7))
            {
                maze = null;
                currentState = GameState.MainMenu;
            }

            if (currentState == GameState.MainMenu)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
                {
                    currentState = GameState.Playing;
                    maze = new Maze(5, 5);
                }
            }
            else if (currentState == GameState.Playing)
            {
                Vector2 newPosition = maze.PlayerPosition;
                Vector2 previousPosition = maze.PlayerPosition;

                if (maze.shortestPath.Count > 0)
                {
                    maze.hint = maze.shortestPath.Peek();
                }
                if ((currentKeyboardState.IsKeyDown(Keys.Right) && previousKeyboardState.IsKeyUp(Keys.Right)) || (currentKeyboardState.IsKeyDown(Keys.D) && previousKeyboardState.IsKeyUp(Keys.D)))
                {
                    newPosition += new Vector2(1, 0);
                }
                if ((currentKeyboardState.IsKeyDown(Keys.Left) && previousKeyboardState.IsKeyUp(Keys.Left)) || (currentKeyboardState.IsKeyDown(Keys.A) && previousKeyboardState.IsKeyUp(Keys.A)))
                {
                    newPosition += new Vector2(-1, 0);
                }
                if ((currentKeyboardState.IsKeyDown(Keys.Up) && previousKeyboardState.IsKeyUp(Keys.Up)) || (currentKeyboardState.IsKeyDown(Keys.W) && previousKeyboardState.IsKeyUp(Keys.W)))
                {
                    newPosition += new Vector2(0, -1);
                }
                if ((currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down)) || (currentKeyboardState.IsKeyDown(Keys.S) && previousKeyboardState.IsKeyUp(Keys.S)))
                {
                    newPosition += new Vector2(0, 1);
                }

                // Check if the new position is within the maze bounds
                if (newPosition.X >= 0 && newPosition.X < maze.width && newPosition.Y >= 0 && newPosition.Y < maze.height)
                {
                    if (maze.CanMoveTo(newPosition))
                    {
                        if (!maze.shortestPath.Contains(newPosition))
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
            }

            previousKeyboardState = currentKeyboardState;
            base.Update(gameTime);
        }

        private void AdjustWindowSize(int mazeWidth, int mazeHeight)
        {
            int width = mazeWidth * m_cellSize;
            int height = mazeHeight * m_cellSize;

            int padding = 100;
            width += padding;
            height += padding;

            m_graphics.PreferredBackBufferWidth = width;
            m_graphics.PreferredBackBufferHeight = height;
            m_graphics.ApplyChanges();
        }

        private void DrawText(string text, Vector2 position, Color textColor, Color backgroundColor)
        {
            Vector2 textSize = m_font.MeasureString(text);
            int padding = 5;
            Rectangle backgroundRectangle = new Rectangle((int)position.X - padding, (int)position.Y - padding, (int)textSize.X + 2 * padding, (int)textSize.Y + 2 * padding);
            // Draw the background
            m_spriteBatch.Draw(wallTexture, backgroundRectangle, backgroundColor);
            // Draw the text
            m_spriteBatch.DrawString(m_font, text, position, textColor);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            if (currentState == GameState.MainMenu)
            {
                string menuText = "Earthbound Maze Game\nF1 - 5x5\nF2 - 10x10\nF3 - 15x15\nF4 - 20x20\nF5 - Display High Score\nF6 - Display Credits";
                Vector2 textSize = m_font.MeasureString(menuText);
                Vector2 textPosition = new Vector2((GraphicsDevice.Viewport.Width - textSize.X) / 2, (GraphicsDevice.Viewport.Height - textSize.Y) / 2);
                DrawText(menuText, textPosition, Color.Black, Color.Cornsilk);
            }
            else if (currentState == GameState.Playing)
            {
                if (maze != null)
                {
                    maze.Draw(m_spriteBatch, wallTexture, GraphicsDevice, m_ness, m_mrsaturn, m_dog, m_grass);
                    // Top-centered text
                    string playingText = "Current Maze";
                    Vector2 playingTextSize = m_font.MeasureString(playingText);
                    Vector2 playingTextPosition = new Vector2((GraphicsDevice.Viewport.Width - playingTextSize.X) / 2, 5);
                    DrawText(playingText, playingTextPosition, Color.Black, Color.Cornsilk);

                    // Bottom-centered text
                    string bottomText = "F7 to navigate to main menu";
                    Vector2 bottomTextSize = m_font.MeasureString(bottomText);
                    Vector2 bottomTextPosition = new Vector2((GraphicsDevice.Viewport.Width - bottomTextSize.X) / 2, GraphicsDevice.Viewport.Height - bottomTextSize.Y - 5); // 20 pixels from the bottom
                    DrawText(bottomText, bottomTextPosition, Color.Black, Color.Cornsilk);
                }
            }
            m_spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}