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
        private Cell[,] cells; // declare a two dimensional array for the cells
        private int width, height;
        private Random rand = new Random();

        public Assignment()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        // generate maze according to Prim's Algorithm
        private void GenerateMaze()
        {
            List<Vector2> frontier = new List<Vector2>();
            // Start from the top-left corner
            int startX = 0;
            int startY = 0;
            frontier.Add(new Vector2(startX, startY));
            cells[startX, startY].Visited = true;

            while (frontier.Count > 0)
            {
                // Choose a random frontier cell
                int randIndex = rand.Next(frontier.Count);
                Vector2 currentCell = frontier[randIndex];
                frontier.RemoveAt(randIndex);
                List<Vector2> neighbors = GetNeighbors((int)currentCell.X, (int)currentCell.Y);

                if (neighbors.Count > 0)
                {
                    // Choose a random neighbor
                    Vector2 neighbor = neighbors[rand.Next(neighbors.Count)];
                    // Remove the wall between the current cell and the chosen neighbor
                    RemoveWalls((int)currentCell.X, (int)currentCell.Y, (int)neighbor.X, (int)neighbor.Y);

                    // Mark the neighbor as visited and add its frontiers to the list
                    cells[(int)neighbor.X, (int)neighbor.Y].Visited = true;
                    AddFrontiers((int)neighbor.X, (int)neighbor.Y, frontier);
                }
            }
        }

        private List<Vector2> GetNeighbors(int x, int y)
        {
            List<Vector2> neighbors = new List<Vector2>();
            // Implementation of getting valid neighbors (omitted for brevity)
            return neighbors;
        }

        private void AddFrontiers(int x, int y, List<Vector2> frontier)
        {
            // Implementation of adding frontiers (omitted for brevity)
        }

        private void RemoveWalls(int x1, int y1, int x2, int y2)
        {
            if (x2 - x1 == 1) // Neighbor is below the current cell
            {
                cells[x1, y1].Walls[2] = false; // Remove current cell's bottom wall
                cells[x2, y2].Walls[0] = false; // Remove neighbor's top wall
            }
            else if (x2 - x1 == -1) // Neighbor is above the current cell
            {
                cells[x1, y1].Walls[0] = false; // Remove current cell's top wall
                cells[x2, y2].Walls[2] = false; // Remove neighbor's bottom wall
            }

            if (y2 - y1 == 1) // Neighbor is to the right of the current cell
            {
                cells[x1, y1].Walls[1] = false; // Remove current cell's right wall
                cells[x2, y2].Walls[3] = false; // Remove neighbor's left wall
            }
            else if (y2 - y1 == -1) // Neighbor is to the left of the current cell
            {
                cells[x1, y1].Walls[3] = false; // Remove current cell's left wall
                cells[x2, y2].Walls[1] = false; // Remove neighbor's right wall
            }
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

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

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }

    public class Cell
    {
        public bool Visited = false;
        public bool[] Walls = { true, true, true, true };
    }
}
