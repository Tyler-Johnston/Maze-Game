
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Maze
{
    public Cell[,] cells;
    public int width, height;
    private Random rand = new Random();
    private const int TOP = 0;
    private const int RIGHT = 1;
    private const int BOTTOM = 2;
    private const int LEFT = 3;
    private const int m_cellSize = 30;
    public Stack<Vector2> shortestPath;

    public HashSet<Vector2> breadcrumbs;
    public Vector2 hint;
    private Vector2 playerPosition;
    public bool displayShortestPath = false;
    public bool displayHint = false;
    public bool displayBreadcrumbs = false;

    public Maze(int width, int height)
    {
        this.width = width;
        this.height = height;
        InitializeCells();
        GenerateMaze();
        shortestPath = FindShortestPath();
        playerPosition = new Vector2(0, 0);
        breadcrumbs = new HashSet<Vector2>
        {
            new Vector2(0, 0)
        };
    }

    public Vector2 PlayerPosition
    {
        get { return playerPosition; }
        set { playerPosition = value; }
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
        cells[0,0].InMaze = true;
        // add the starting cells neighbors to the frontier
        AddFrontiers(0,0, frontier);

        while (frontier.Count > 0)
        {
            // randomly choose a cell in the frontier
            int randomIndex = rand.Next(0, frontier.Count);
            Vector2 randomFrontierCell = frontier[randomIndex];
            // randomly select a wall from that cell that is also connected with any wall that is part of the maze & remove it
            RemoveWalls((int)randomFrontierCell.X, (int)randomFrontierCell.Y);
            // that frontier cell is now InMaze / in the maze
            cells[(int)randomFrontierCell.X, (int)randomFrontierCell.Y].InMaze = true;
            // remove that cell from the frontier as it is now in the maze
            frontier.RemoveAt(randomIndex);
            // After marking the cell as InMaze and removing walls, add its unInMaze neighbors to the frontier
            AddFrontiers((int)randomFrontierCell.X, (int)randomFrontierCell.Y, frontier);
        }
    }

    private List<Vector2> GetNeighborsInMaze(int x, int y)
    {
        List<Vector2> neighbors = new List<Vector2>();

        if (x + 1 < width && cells[x+1,y].InMaze)
        {
            neighbors.Add(new Vector2(x + 1, y));
        }
        if (x - 1 >= 0 && cells[x-1,y].InMaze)
        {
            neighbors.Add(new Vector2(x - 1, y));
        }
        if (y + 1 < height && cells[x,y+1].InMaze)
        {
            neighbors.Add(new Vector2(x, y + 1));
        }
        if (y - 1 >= 0 && cells[x,y-1].InMaze)
        {
            neighbors.Add(new Vector2(x, y - 1));
        }
        return neighbors;
    }

    private List<Vector2> GetNeighborsNotInMaze(int x, int y)
    {
        List<Vector2> neighbors = new List<Vector2>();

        if (x + 1 < width && !cells[x+1,y].InMaze)
        {
            neighbors.Add(new Vector2(x + 1, y));
        }
        if (x - 1 >= 0 && !cells[x-1,y].InMaze)
        {
            neighbors.Add(new Vector2(x - 1, y));
        }
        if (y + 1 < height && !cells[x,y+1].InMaze)
        {
            neighbors.Add(new Vector2(x, y + 1));
        }
        if (y - 1 >= 0 && !cells[x,y-1].InMaze)
        {
            neighbors.Add(new Vector2(x, y - 1));
        }

        return neighbors;
    }

    private List<Vector2> GetAccessibleNeighbors(Vector2 cell)
    {
        int x = (int)cell.X;
        int y = (int)cell.Y;
        List<Vector2> neighbors = new List<Vector2>();

        if (y > 0 && !cells[x, y].Walls[TOP])
            neighbors.Add(new Vector2(x, y - 1));
        if (x < width - 1 && !cells[x, y].Walls[RIGHT])
            neighbors.Add(new Vector2(x + 1, y));
        if (y < height - 1 && !cells[x, y].Walls[BOTTOM])
            neighbors.Add(new Vector2(x, y + 1));
        if (x > 0 && !cells[x, y].Walls[LEFT])
            neighbors.Add(new Vector2(x - 1, y));

        return neighbors;
    }

    private void AddFrontiers(int x, int y, List<Vector2> frontier)
    {

        List<Vector2> neighbors = GetNeighborsNotInMaze(x,y);
        foreach (var neighbor in neighbors)
        {
            if ((!cells[(int)neighbor.X, (int)neighbor.Y].InMaze) && (!frontier.Contains(neighbor)))
            {
                frontier.Add(neighbor);
            }
        }
    }
    private void RemoveWalls(int frontierCellX, int frontierCellY)
    {
        List<Vector2> neighbors = GetNeighborsInMaze(frontierCellX, frontierCellY);

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

    public bool CanMoveTo(Vector2 newPosition)
    {
        int x = (int)playerPosition.X;
        int y = (int)playerPosition.Y;
        int newX = (int)newPosition.X;
        int newY = (int)newPosition.Y;

        // Ensure newPosition is adjacent to the current position
        if (Math.Abs(newX - x) + Math.Abs(newY - y) != 1) return false;

        // If there is a wall in the way, return false
        if (newX > x && cells[x, y].Walls[RIGHT]) return false;
        if (newX < x && cells[x, y].Walls[LEFT]) return false;
        if (newY > y && cells[x, y].Walls[BOTTOM]) return false;
        if (newY < y && cells[x, y].Walls[TOP]) return false;

        // If trying to move outside the maze, return false
        if (newX < 0 || newY < 0 || newX >= width || newY >= height) return false;

        return true;
    }

    public Stack<Vector2> FindShortestPath()
    {
        // Initialize data structures for BFS
        Queue<Vector2> queue = new Queue<Vector2>();
        Dictionary<Vector2, Vector2?> prev = new Dictionary<Vector2, Vector2?>();
        Stack<Vector2> shortestPath = new Stack<Vector2>();
        Vector2 start = new Vector2(0, 0);
        Vector2 end = new Vector2(width - 1, height - 1);

        // Start by enqueuing the start position
        queue.Enqueue(start);
        prev[start] = null;

        while (queue.Count > 0)
        {
            Vector2 current = queue.Dequeue();

            // If we've reached the end, break out of the loop
            if (current == end)
                break;

            // Get the accessible neighbors where there's no wall in between
            foreach (Vector2 neighbor in GetAccessibleNeighbors(current))
            {
                if (!prev.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    prev[neighbor] = current;
                }
            }
        }

        // If we reached the end, reconstruct the path from end to start
        if (prev.ContainsKey(end))
        {
            for (Vector2? at = end; at != null; at = prev[at.Value])
            {
                shortestPath.Push(at.Value);
            }
        }
        return shortestPath;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D wallTexture, GraphicsDevice graphicsDevice, Texture2D m_ness, Texture2D m_mrsaturn, Texture2D m_dog, Texture2D m_grass)
    {
        Color wallColor = Color.Black;

        // Calculate total size of the maze
        int mazeWidth = width * m_cellSize;
        int mazeHeight = height * m_cellSize;

        // Calculate the starting position to center the maze
        int offsetX = (graphicsDevice.Viewport.Width - mazeWidth) / 2;
        int offsetY = (graphicsDevice.Viewport.Height - mazeHeight) / 2;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x * m_cellSize + offsetX, y * m_cellSize + offsetY);
                spriteBatch.Draw(m_grass, new Rectangle((int)position.X, (int)position.Y, m_cellSize, m_cellSize), Color.White);
                if (displayShortestPath && shortestPath.Contains(new Vector2(x, y)))
                {
                    float scaleForDog = m_cellSize / (float)Math.Max(m_dog.Width, m_dog.Height);
                    spriteBatch.Draw(m_dog, position, null, Color.White, 0f, Vector2.Zero, scaleForDog, SpriteEffects.None, 0f);
                }
                if (displayBreadcrumbs && breadcrumbs.Contains(new Vector2(x,y)))
                {
                    float scaleForMrSaturn = m_cellSize / (float)Math.Max(m_mrsaturn.Width, m_mrsaturn.Height);
                    spriteBatch.Draw(m_mrsaturn, position, null, Color.White, 0f, Vector2.Zero, scaleForMrSaturn, SpriteEffects.None, 0f);
                }
                if (displayHint && hint == new Vector2(x, y))
                {
                    float scaleForMrSaturn = m_cellSize / (float)Math.Max(m_mrsaturn.Width, m_mrsaturn.Height);
                    spriteBatch.Draw(m_mrsaturn, position, null, Color.White, 0f, Vector2.Zero, scaleForMrSaturn, SpriteEffects.None, 0f);
                }
                if (cells[x, y].Walls[TOP])
                {
                    spriteBatch.Draw(wallTexture, new Rectangle((int)position.X, (int)position.Y, m_cellSize, 1), wallColor);
                }
                if (cells[x, y].Walls[RIGHT])
                {
                    spriteBatch.Draw(wallTexture, new Rectangle((int)(position.X + m_cellSize), (int)position.Y, 1, m_cellSize), wallColor);
                }
                if (cells[x, y].Walls[BOTTOM])
                {
                    spriteBatch.Draw(wallTexture, new Rectangle((int)position.X, (int)(position.Y + m_cellSize), m_cellSize, 1), wallColor);
                }
                if (cells[x, y].Walls[LEFT])
                {
                    spriteBatch.Draw(wallTexture, new Rectangle((int)position.X, (int)position.Y, 1, m_cellSize), wallColor);
                }
                float scale = (float)m_cellSize / Math.Max(m_ness.Width, m_ness.Height);
                Vector2 playerScreenPosition = new Vector2(playerPosition.X * m_cellSize + offsetX, playerPosition.Y * m_cellSize + offsetY);
                spriteBatch.Draw(m_ness, playerScreenPosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
