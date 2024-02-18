using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CS5410
{
    public class Assignment : Game
    {
        private GraphicsDeviceManager m_graphics;
        private enum GameState
        {
            MainMenu,
            Playing,
            HighScores,
            Credits
        }
        private GameState currentState = GameState.MainMenu;
        private SpriteBatch m_spriteBatch;
        private int m_cellSize = 30;
        private Maze maze;
        private Texture2D wallTexture;
        private Texture2D m_ness;
        private Texture2D m_poo;
        private Texture2D m_mrsaturn;
        private Texture2D m_dog;
        private Texture2D m_background;

        private Texture2D m_grass;
        private SpriteFont m_font;
        private bool acceptInput = true;
        private KeyboardState previousKeyboardState = Keyboard.GetState();
        private DateTime startTime;
        private TimeSpan elapsedTime;
        private List<Dictionary<string, object>> highScores = new List<Dictionary<string, object>>();
        private bool highScoreRecorded = false;

        public Assignment()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            elapsedTime = TimeSpan.Zero;
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
            m_poo = this.Content.Load<Texture2D>("Images/poo");
            m_dog = this.Content.Load<Texture2D>("Images/dog");
            m_background = this.Content.Load<Texture2D>("Images/background");
            m_grass = this.Content.Load<Texture2D>("Images/grass");
            m_font = this.Content.Load<SpriteFont>("Fonts/PixelifySans");
        }

        private void processInGameInput(KeyboardState currentKeyboardState)
        {

            if (!(currentState == GameState.Playing && acceptInput))
            {
                return;
            }
  
            Vector2 newPosition = maze.PlayerPosition;
            Vector2 previousPosition = maze.PlayerPosition;
            elapsedTime = DateTime.Now - startTime;
            
            if (maze.ShortestPath.Count > 0)
            {
                maze.hint = maze.ShortestPath.Peek();
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
            if (newPosition.X >= 0 && newPosition.X < maze.Width && newPosition.Y >= 0 && newPosition.Y < maze.Height)
            {
                if (maze.CanMoveTo(newPosition))
                {
                    if (!maze.breadcrumbs.Contains(newPosition))
                    {
                        if (maze.ShortestPath.Contains(newPosition))
                        {
                            maze.Score += 5;
                        }
                        else
                        {
                            maze.Score -= 2;
                        }
                    }
                    if (!maze.ShortestPath.Contains(newPosition))
                    {
                        maze.ShortestPath.Push(previousPosition);
                    }
                    maze.PlayerPosition = newPosition;
                    maze.breadcrumbs.Add(newPosition);
                }
            }
            if (maze.ShortestPath.Contains(maze.PlayerPosition))
            {
                maze.ShortestPath.Pop();
            }

            // Toggle display of the shortest path on 'P' key press
            if (currentKeyboardState.IsKeyDown(Keys.P) && previousKeyboardState.IsKeyUp(Keys.P))
            {
                maze.DisplayShortestPath = !maze.DisplayShortestPath;
            }

            // Toggle hint display
            if (currentKeyboardState.IsKeyDown(Keys.H) && previousKeyboardState.IsKeyUp(Keys.H))
            {
                maze.DisplayHint = !maze.DisplayHint;
            }

            // Toggle breadcrumb  display
            if (currentKeyboardState.IsKeyDown(Keys.B) && previousKeyboardState.IsKeyUp(Keys.B))
            {
                maze.DisplayBreadcrumbs = !maze.DisplayBreadcrumbs;
            }
            // check win condition and stop the user from being able to move
            if (maze.PlayerPosition == new Vector2(maze.Width - 1, maze.Height - 1) && !maze.GameWon)
            {
                maze.GameWon = true;
                acceptInput = false;

                if (!highScoreRecorded)
                {
                    var scoreEntry = new Dictionary<string, object>
                    {
                        { "score", maze.Score },
                        { "time", elapsedTime },
                        { "size", $"{maze.Width}x{maze.Height}" }
                    };
                    highScores.Add(scoreEntry);

                    highScores = highScores.OrderByDescending(entry => entry["score"]).ToList();

                    highScoreRecorded = true;
                }
            }
        }

        private void ProcessMainMenuInput(KeyboardState currentKeyboardState)
        {
            // Exit the maze game if the player presses "Esc"
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // New Game 5x5
            if (currentKeyboardState.IsKeyDown(Keys.F1) && previousKeyboardState.IsKeyUp(Keys.F1))
            {
                maze = new Maze(5, 5);
                currentState = GameState.Playing;
                acceptInput = true;
                startTime = DateTime.Now;
                highScoreRecorded = false;
            }

            // New Game 10x10
            if (currentKeyboardState.IsKeyDown(Keys.F2) && previousKeyboardState.IsKeyUp(Keys.F2))
            {
                maze = new Maze(10, 10);
                currentState = GameState.Playing;
                acceptInput = true;
                startTime = DateTime.Now;
                highScoreRecorded = false;
            }

            // New Game 15x15
            if (currentKeyboardState.IsKeyDown(Keys.F3) && previousKeyboardState.IsKeyUp(Keys.F3))
            {
                maze = new Maze(15, 15);
                currentState = GameState.Playing;
                acceptInput = true;
                startTime = DateTime.Now;
                highScoreRecorded = false;
            }

            // New Game 20x20
            if (currentKeyboardState.IsKeyDown(Keys.F4) && previousKeyboardState.IsKeyUp(Keys.F4))
            {
                maze = new Maze(20, 20);
                currentState = GameState.Playing;
                acceptInput = true;
                startTime = DateTime.Now;
                highScoreRecorded = false;
            }

            // Display High Scores
            if (currentKeyboardState.IsKeyDown(Keys.F5) && previousKeyboardState.IsKeyUp(Keys.F5))
            {
                currentState = GameState.HighScores;
            }

            // Display Credits
            if (currentKeyboardState.IsKeyDown(Keys.F6) && previousKeyboardState.IsKeyUp(Keys.F6))
            {
                currentState = GameState.Credits;
            }

            // Back to Main Menu
            if (currentKeyboardState.IsKeyDown(Keys.F7) && previousKeyboardState.IsKeyUp(Keys.F7))
            {
                maze = null;
                currentState = GameState.MainMenu;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var currentKeyboardState = Keyboard.GetState();
            ProcessMainMenuInput(currentKeyboardState);
            processInGameInput(currentKeyboardState);
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

        private void DrawMainMenu()
        {
            string menuText = "Earthbound Maze Game\nF1 - 5x5\nF2 - 10x10\nF3 - 15x15\nF4 - 20x20\nF5 - Display High Score\nF6 - Display Credits\nEsc - Exit";
            Vector2 textSize = m_font.MeasureString(menuText);
            Vector2 textPosition = new Vector2((GraphicsDevice.Viewport.Width - textSize.X) / 2, (GraphicsDevice.Viewport.Height - textSize.Y) / 2);
            DrawText(menuText, textPosition, Color.Black, Color.Cornsilk);
        }


        private void DrawPlaying()
        {
            maze.Draw(m_spriteBatch, wallTexture, GraphicsDevice, m_ness, m_mrsaturn, m_dog, m_grass, m_poo);
            if (maze.GameWon)
            {
                string winMessage = "You Won!";
                Vector2 messageSize = m_font.MeasureString(winMessage);
                Vector2 messagePosition = new Vector2(GraphicsDevice.Viewport.Width / 2 - messageSize.X / 2, GraphicsDevice.Viewport.Height / 2 - messageSize.Y / 2);
                m_spriteBatch.DrawString(m_font, winMessage, messagePosition, Color.Yellow);
            }
            // Top-centered text
            string timeText = $"Time: {elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2} / Score: {maze.Score}";
            Vector2 playingTextSize = m_font.MeasureString(timeText);
            Vector2 playingTextPosition = new Vector2((GraphicsDevice.Viewport.Width - playingTextSize.X) / 2, 5);
            DrawText(timeText, playingTextPosition, Color.Black, Color.Cornsilk);

            // Bottom-centered text
            string bottomText = "F7 to navigate to main menu";
            Vector2 bottomTextSize = m_font.MeasureString(bottomText);
            Vector2 bottomTextPosition = new Vector2((GraphicsDevice.Viewport.Width - bottomTextSize.X) / 2, GraphicsDevice.Viewport.Height - bottomTextSize.Y - 5); // 20 pixels from the bottom
            DrawText(bottomText, bottomTextPosition, Color.Black, Color.Cornsilk);
        }

        private void DrawHighScores()
        {
            string highScoresText = "High Scores:\n";
            foreach (var entry in highScores)
            {
                TimeSpan time = (TimeSpan)entry["time"];
                string size = (string)entry["size"]; // Retrieve the size
                highScoresText += $"Size: {size}, Score: {entry["score"]}, Time: {time.Minutes:D2}:{time.Seconds:D2}\n";
            }

            Vector2 highScoresSize = m_font.MeasureString(highScoresText);
            Vector2 highScoresPosition = new Vector2((GraphicsDevice.Viewport.Width - highScoresSize.X) / 2, (GraphicsDevice.Viewport.Height - highScoresSize.Y) / 2);
            DrawText(highScoresText, highScoresPosition, Color.Black, Color.Cornsilk);
        }

        private void DrawCredits()
        {
            string creditsText = "Credits\nProgramming: Tyler Johnston\nArtwork: Nintendo";
            Vector2 creditsSize = m_font.MeasureString(creditsText);
            Vector2 creditsPosition = new Vector2((GraphicsDevice.Viewport.Width - creditsSize.X) / 2, (GraphicsDevice.Viewport.Height - creditsSize.Y) / 2);
            DrawText(creditsText, creditsPosition, Color.Black, Color.Cornsilk);
        }


       protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            switch (currentState)
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameState.Playing:
                    DrawPlaying();
                    break;
                case GameState.HighScores:
                    DrawHighScores();
                    break;
                case GameState.Credits:
                    DrawCredits();
                    break;
            }

            m_spriteBatch.End();
            base.Draw(gameTime);
        }

    }
}