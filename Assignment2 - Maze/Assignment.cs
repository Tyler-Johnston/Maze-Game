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
            maze = new Maze(10, 10); // Example dimensions
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
            // maze.Draw(m_spriteBatch, wallTexture);
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
            cells[1,1].Visited = true;
            // add the starting cells neighbors to the fronteir
            AddFrontiers(1,1, frontier);
            // radnomly choose a cell in the frontier
            Vector2 randomFrontierCell = frontier[rand.Next(0, frontier.Count)];
            // randomly select a wall from that cell that is also connected with any wall that is part of the maze
            // then remove that wall

            foreach (var frontee in frontier)
            {
                Console.WriteLine(frontee);
            }
            RemoveWalls((int)randomFrontierCell.X, (int)randomFrontierCell.Y);
            




            // do
            // {


            // } while (frontier.Count > 0);







            // int startX = 0;
            // int startY = 0;
            // frontier.Add(new Vector2(startX, startY));
            // cells[startX, startY].Visited = true;
            // while (frontier.Count > 0)
            // {
            //     // Choose a random frontier cell
            //     int randIndex = rand.Next(frontier.Count);
            //     Vector2 currentCell = frontier[randIndex];
            //     frontier.RemoveAt(randIndex);
            //     List<Vector2> neighbors = GetNeighbors((int)currentCell.X, (int)currentCell.Y);

            //     if (neighbors.Count > 0)
            //     {
            //         // Choose a random neighbor
            //         Vector2 neighbor = neighbors[rand.Next(neighbors.Count)];
            //         // Remove the wall between the current cell and the chosen neighbor
            //         RemoveWalls((int)currentCell.X, (int)currentCell.Y, (int)neighbor.X, (int)neighbor.Y);

            //         // Mark the neighbor as visited and add its frontiers to the list
            //         cells[(int)neighbor.X, (int)neighbor.Y].Visited = true;
            //         AddFrontiers((int)neighbor.X, (int)neighbor.Y, frontier);
            //     }
            // }
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

            Console.WriteLine("fronteir cell x/y: " + frontierCellX + " " + frontierCellY);
            Console.WriteLine("cell in maze x/y: " + cellInMazeX + " " + cellInMazeY);

            if (frontierCellX - cellInMazeX == 1) // Neighbor is below the current cell
            {
                Console.WriteLine("Neighbor is below the current cell");
                // cells[cellInMazeX, cellInMazeY].Walls[2] = false; // Remove current cell's bottom wall
                // cells[frontierCellX, frontierCellY].Walls[0] = false; // Remove neighbor's top wall
            }
            else if (frontierCellX - cellInMazeX == -1) // Neighbor is above the current cell
            {
                Console.WriteLine("Neighbor is above the current cell");
                // cells[cellInMazeX, cellInMazeY].Walls[0] = false; // Remove current cell's top wall
                // cells[frontierCellX, frontierCellY].Walls[2] = false; // Remove neighbor's bottom wall
            }

            if (frontierCellY - cellInMazeY == 1) // Neighbor is to the right of the current cell
            {
                Console.WriteLine("Neighbor is to the right of the current cell");
                // cells[cellInMazeX, cellInMazeY].Walls[1] = false; // Remove current cell's right wall
                // cells[frontierCellX, frontierCellY].Walls[3] = false; // Remove neighbor's left wall
            }
            else if (frontierCellY - cellInMazeY == -1) // Neighbor is to the left of the current cell
            {
                Console.WriteLine("Neighbor is to the left of the current cell");
                // cells[cellInMazeX, cellInMazeY].Walls[3] = false; // Remove current cell's left wall
                // cells[frontierCellX, frontierCellY].Walls[1] = false; // Remove neighbor's right wall
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

    }
}
