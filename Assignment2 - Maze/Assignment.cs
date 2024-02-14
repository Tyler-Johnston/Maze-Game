using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace CS5410
{
    public class Assignment : Game
    {
        private GraphicsDeviceManager m_graphics;
        private SpriteBatch m_spriteBatch;
        private Maze maze;
        private Texture2D wallTexture;

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
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            m_spriteBatch.Begin();
            maze.Draw(m_spriteBatch, wallTexture);
            m_spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    public class Cell
    {
        public bool Visited = false;
        public bool[] Walls = { true, true, true, true }; // top, right, bottom, left
    }

    public class Maze
    {
        private Cell[,] cells;
        private int width, height;
        private Random rand = new Random();
        private const int TOP = 0;
        private const int RIGHT = 1;
        private const int BOTTOM = 2;
        private const int LEFT = 3;
        public Maze(int width, int height)
        {
            this.width = width;
            this.height = height;
            InitializeCells();
            GenerateMaze();
        }
        private void InitializeCells()
        {
            cells = new Cell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new Cell();
                }
            }
        }

        // generate maze according to Prim's Algorithm
        private void GenerateMaze()
        {
            List<Vector2> frontier = new List<Vector2>();
            // choose 0,0 as the starting position
            cells[0,0].Visited = true;
            // add the starting cells neighbors to the frontier
            AddFrontiers(0,0, frontier);

            while (frontier.Count > 0)
            {
                // randomly choose a cell in the frontier
                int randomIndex = rand.Next(0, frontier.Count);
                Vector2 randomFrontierCell = frontier[randomIndex];
                // randomly select a wall from that cell that is also connected with any wall that is part of the maze & remove it
                RemoveWalls((int)randomFrontierCell.X, (int)randomFrontierCell.Y);
                // that frontier cell is now visited / in the maze
                cells[(int)randomFrontierCell.X, (int)randomFrontierCell.Y].Visited = true;
                // remove that cell from the frontier as it is now in the maze
                frontier.RemoveAt(randomIndex);
                // After marking the cell as visited and removing walls, add its unvisited neighbors to the frontier
                AddFrontiers((int)randomFrontierCell.X, (int)randomFrontierCell.Y, frontier);
            }
        }

        // maybe later try to not have two 'get neighbors in/out of maze' functions to clean up the code, as these are so similar
        private List<Vector2> GetNeighborsInMaze(int x, int y)
        {
            List<Vector2> neighbors = new List<Vector2>();

            if (x + 1 < width && cells[x+1,y].Visited)
            {
                neighbors.Add(new Vector2(x + 1, y));
            }
            if (x - 1 >= 0 && cells[x-1,y].Visited)
            {
                neighbors.Add(new Vector2(x - 1, y));
            }
            if (y + 1 < height && cells[x,y+1].Visited)
            {
                neighbors.Add(new Vector2(x, y + 1));
            }
            if (y - 1 >= 0 && cells[x,y-1].Visited)
            {
                neighbors.Add(new Vector2(x, y - 1));
            }
            return neighbors;
        }

        private List<Vector2> GetNeighborsNotInMaze(int x, int y)
        {
            List<Vector2> neighbors = new List<Vector2>();

            if (x + 1 < width && !cells[x+1,y].Visited)
            {
                neighbors.Add(new Vector2(x + 1, y));
            }
            if (x - 1 >= 0 && !cells[x-1,y].Visited)
            {
                neighbors.Add(new Vector2(x - 1, y));
            }
            if (y + 1 < height && !cells[x,y+1].Visited)
            {
                neighbors.Add(new Vector2(x, y + 1));
            }
            if (y - 1 >= 0 && !cells[x,y-1].Visited)
            {
                neighbors.Add(new Vector2(x, y - 1));
            }

            return neighbors;
        }

        private void AddFrontiers(int x, int y, List<Vector2> frontier)
        {

            List<Vector2> neighbors = GetNeighborsNotInMaze(x,y);
            foreach (var neighbor in neighbors)
            {
                if ((!cells[(int)neighbor.X, (int)neighbor.Y].Visited) && (!frontier.Contains(neighbor)))
                {
                    frontier.Add(neighbor);
                }
            }
        }


        private void RemoveWalls(int frontierCellX, int frontierCellY)
        {
            List<Vector2> neighbors = GetNeighborsInMaze(frontierCellX, frontierCellY);
            if (neighbors.Count == 0)
            {
                // This should not happen if the algorithm is correct
                throw new InvalidOperationException("No neighboring cells in the maze; this indicates a bug in the maze generation logic.");
            }

            // Randomly select a neighbor that is already part of the maze
            Vector2 randomCellInMaze = neighbors[rand.Next(neighbors.Count)];
            int cellInMazeX = (int)randomCellInMaze.X;
            int cellInMazeY = (int)randomCellInMaze.Y;

            // Determine which wall is shared and remove it
            if (frontierCellX == cellInMazeX)
            {
                // Cells are on the same X axis, so we remove the horizontal walls
                if (frontierCellY > cellInMazeY)
                {
                    // Frontier cell is below the current cell
                    cells[cellInMazeX, cellInMazeY].Walls[BOTTOM] = false;
                    cells[frontierCellX, frontierCellY].Walls[TOP] = false;
                }
                else if (frontierCellY < cellInMazeY)
                {
                    // Frontier cell is above the current cell
                    cells[cellInMazeX, cellInMazeY].Walls[TOP] = false;
                    cells[frontierCellX, frontierCellY].Walls[BOTTOM] = false;
                }
            }
            else if (frontierCellY == cellInMazeY)
            {
                // Cells are on the same Y axis, so we remove the vertical walls
                if (frontierCellX > cellInMazeX)
                {
                    // Frontier cell is to the right of the current cell
                    cells[cellInMazeX, cellInMazeY].Walls[RIGHT] = false;
                    cells[frontierCellX, frontierCellY].Walls[LEFT] = false;
                }
                else if (frontierCellX < cellInMazeX)
                {
                    // Frontier cell is to the left of the current cell
                    cells[cellInMazeX, cellInMazeY].Walls[LEFT] = false;
                    cells[frontierCellX, frontierCellY].Walls[RIGHT] = false;
                }
            }
        }


        // this draw method was not designed by me, i had chat-gpt generate it so i could see if my maze was implemented right
        public void Draw(SpriteBatch spriteBatch, Texture2D wallTexture)
        {
            int cellSize = 20; // Size of each cell, adjust as necessary
            Color wallColor = Color.Black; // Color of the walls

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Calculate the position of the cell
                    Vector2 position = new Vector2(x * cellSize, y * cellSize);

                    // Draw the top wall if the cell has one
                    if (cells[x, y].Walls[0])
                    {
                        spriteBatch.Draw(wallTexture, new Rectangle((int)position.X, (int)position.Y, cellSize, 1), wallColor);
                    }
                    // Draw the right wall if the cell has one
                    if (cells[x, y].Walls[1])
                    {
                        spriteBatch.Draw(wallTexture, new Rectangle((int)(position.X + cellSize), (int)position.Y, 1, cellSize), wallColor);
                    }
                    // Draw the bottom wall if the cell has one
                    if (cells[x, y].Walls[2])
                    {
                        spriteBatch.Draw(wallTexture, new Rectangle((int)position.X, (int)(position.Y + cellSize), cellSize, 1), wallColor);
                    }
                    // Draw the left wall if the cell has one
                    if (cells[x, y].Walls[3])
                    {
                        spriteBatch.Draw(wallTexture, new Rectangle((int)position.X, (int)position.Y, 1, cellSize), wallColor);
                    }
                }
            }
        }

    }
}