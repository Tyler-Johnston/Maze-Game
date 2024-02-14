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
            maze = new Maze(10, 10);
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
            List<Vector2> neighbors = GetNeighborsInMaze(frontierCellX,frontierCellY);
            Vector2 randomCellInMaze = neighbors[rand.Next(0, neighbors.Count)];
            int cellInMazeX = (int)randomCellInMaze.X;
            int cellInMazeY = (int)randomCellInMaze.Y;

            if (neighbors.Count == 0)
            {
                return;
            }

            if (frontierCellX - cellInMazeX == 1) // Neighbor is below the current cell
            {
                cells[cellInMazeX, cellInMazeY].Walls[2] = false; // Remove current cell's bottom wall
                cells[frontierCellX, frontierCellY].Walls[0] = false; // Remove neighbor's top wall
            }
            if (frontierCellX - cellInMazeX == -1) // Neighbor is above the current cell
            {
                cells[cellInMazeX, cellInMazeY].Walls[0] = false; // Remove current cell's top wall
                cells[frontierCellX, frontierCellY].Walls[2] = false; // Remove neighbor's bottom wall
            }

            if (frontierCellY - cellInMazeY == 1) // Neighbor is to the right of the current cell
            {
                cells[cellInMazeX, cellInMazeY].Walls[1] = false; // Remove current cell's right wall
                cells[frontierCellX, frontierCellY].Walls[3] = false; // Remove neighbor's left wall
            }
            if (frontierCellY - cellInMazeY == -1) // Neighbor is to the left of the current cell
            {
                cells[cellInMazeX, cellInMazeY].Walls[3] = false; // Remove current cell's left wall
                cells[frontierCellX, frontierCellY].Walls[1] = false; // Remove neighbor's right wall
            }
        }

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

                    // Draw top wall
                    if (cells[x, y].Walls[0])
                    {
                        spriteBatch.Draw(wallTexture, new Rectangle((int)position.X, (int)position.Y, cellSize, 1), wallColor);
                    }

                    // Draw left wall
                    if (cells[x, y].Walls[3])
                    {
                        spriteBatch.Draw(wallTexture, new Rectangle((int)position.X, (int)position.Y, 1, cellSize), wallColor);
                    }

                    // Draw right wall only if it's the rightmost cell
                    if (x == width - 1 && cells[x, y].Walls[1])
                    {
                        spriteBatch.Draw(wallTexture, new Rectangle((int)(position.X + cellSize - 1), (int)position.Y, 1, cellSize), wallColor);
                    }

                    // Draw bottom wall only if it's the bottommost cell
                    if (y == height - 1 && cells[x, y].Walls[2])
                    {
                        spriteBatch.Draw(wallTexture, new Rectangle((int)position.X, (int)(position.Y + cellSize - 1), cellSize, 1), wallColor);
                    }
                }
            }
        }

        public bool VerifyMaze(out List<Vector2> unreachableCells)
        {
            unreachableCells = new List<Vector2>(); // List to store unreachable cells
            bool[,] visited = new bool[width, height];
            Stack<Vector2> stack = new Stack<Vector2>();
            stack.Push(new Vector2(0, 0)); // Assuming the entrance is at the top-left cell

            while (stack.Count > 0)
            {
                Vector2 current = stack.Pop();
                int x = (int)current.X;
                int y = (int)current.Y;

                // If we have already visited this cell, skip it
                if (visited[x, y])
                    continue;

                // Mark this cell as visited
                visited[x, y] = true;

                // Check all adjacent cells that are connected (i.e., no wall between them)
                if (!cells[x, y].Walls[0] && y > 0) stack.Push(new Vector2(x, y - 1)); // Top
                if (!cells[x, y].Walls[1] && x < width - 1) stack.Push(new Vector2(x + 1, y)); // Right
                if (!cells[x, y].Walls[2] && y < height - 1) stack.Push(new Vector2(x, y + 1)); // Bottom
                if (!cells[x, y].Walls[3] && x > 0) stack.Push(new Vector2(x - 1, y)); // Left
            }

            // Check if all cells were visited
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!visited[i, j])
                    {
                        unreachableCells.Add(new Vector2(i, j)); // Add the cell to the list of unreachable cells
                    }
                }
            }

            return unreachableCells.Count == 0; // Maze is perfect if there are no unreachable cells
        }

    }
}
