using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CS5410
{
    public class Assignment : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Maze maze;
        private Texture2D wallTexture;

        public Assignment()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            maze = new Maze(10, 10);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            wallTexture = new Texture2D(GraphicsDevice, 1, 1);
            wallTexture.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();
            maze.Draw(spriteBatch, wallTexture);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }

    public class Cell
    {
        public bool Visited = false;
    }

    public class Maze
    {
        private Cell[,] cells;
        private int width, height;
        private Random rand = new Random();

        public Maze(int width, int height)
        {
            this.width = width * 2 + 1;
            this.height = height * 2 + 1;
            cells = new Cell[this.width, this.height];
            InitializeCells();
            GenerateMaze();
        }

        private void InitializeCells()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new Cell();
                }
            }
        }

        private void GenerateMaze()
        {
            HashSet<Vector2> frontier = new HashSet<Vector2>();
            int startX = 0;
            int startY = 0;
            cells[startX, startY].Visited = true;

            AddFrontiers(startX, startY, frontier);
            while (frontier.Any())
            {
                Vector2 randomFrontierCell = frontier.ElementAt(rand.Next(frontier.Count));
                List<Vector2> neighbors = GetNeighborsInMaze((int)randomFrontierCell.X, (int)randomFrontierCell.Y);

                if (neighbors.Count > 0)
                {
                    Vector2 randomCellInMaze = neighbors[rand.Next(neighbors.Count)];
                    RemoveWalls((int)randomFrontierCell.X, (int)randomFrontierCell.Y, (int)randomCellInMaze.X, (int)randomCellInMaze.Y);
                }

                frontier.Remove(randomFrontierCell);
                AddFrontiers((int)randomFrontierCell.X, (int)randomFrontierCell.Y, frontier);
            }
        }

        private List<Vector2> GetNeighborsInMaze(int x, int y)
        {
            List<Vector2> neighbors = new List<Vector2>();
            // Check left
            if (x > 1 && cells[x-2, y].Visited) neighbors.Add(new Vector2(x-2, y));
            // Check right
            if (x < width - 2 && cells[x+2, y].Visited) neighbors.Add(new Vector2(x+2, y));
            // Check up
            if (y > 1 && cells[x, y-2].Visited) neighbors.Add(new Vector2(x, y-2));
            // Check down
            if (y < height - 2 && cells[x, y+2].Visited) neighbors.Add(new Vector2(x, y+2));

            return neighbors;
        }

        private void AddFrontiers(int x, int y, HashSet<Vector2> frontier)
        {
            if (x > 1 && !cells[x-2, y].Visited) frontier.Add(new Vector2(x-2, y));
            if (x < width - 2 && !cells[x+2, y].Visited) frontier.Add(new Vector2(x+2, y));
            if (y > 1 && !cells[x, y-2].Visited) frontier.Add(new Vector2(x, y-2));
            if (y < height - 2 && !cells[x, y+2].Visited) frontier.Add(new Vector2(x, y+2));
        }

        private void RemoveWalls(int frontierCellX, int frontierCellY, int cellInMazeX, int cellInMazeY)
        {
            int inBetweenX = (frontierCellX + cellInMazeX) / 2;
            int inBetweenY = (frontierCellY + cellInMazeY) / 2;

            cells[frontierCellX, frontierCellY].Visited = true;
            cells[inBetweenX, inBetweenY].Visited = true;
            cells[cellInMazeX, cellInMazeY].Visited = true;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D wallTexture)
        {
            int cellSize = 20;
            Color wallColor = Color.Black;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 position = new Vector2(x * cellSize, y * cellSize);

                    // Draw cell based on its state
                    if (!cells[x, y].Visited)
                    {
                        spriteBatch.Draw(wallTexture, new Rectangle((int)position.X, (int)position.Y, cellSize, cellSize), wallColor);
                    }
                }
            }
        }

    }
}
