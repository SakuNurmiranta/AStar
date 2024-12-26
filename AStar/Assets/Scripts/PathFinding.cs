using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    private TileState[,] _grid; // Reference to the grid

    public PathFinding(TileState[,] grid)
    {
        _grid = grid;
    }

    // Find the first tile with the specified state in the grid
    public TileState FindTile(TileState.TileType tileType)
    {
        // Loop through the grid to find the tile with the matching state
        foreach (var tile in _grid)
        {
            if (tile != null && tile.GetTileType() == tileType)
            {
                return tile;
            }
        }

        // If no such tile is found, return null
        return null;
    }
    
    public List<TileState> GetNeighbors(TileState queryTile)
    {
        if (queryTile == null) return null;
        
        //list to store
        List<TileState> neighbors = new List<TileState>();
        
        //gridPos of queryTile
        Vector2Int gridPos = queryTile.GridPosition;
        
        //Grid boundaries for loops
        int gWidth = _grid.GetLength(0);
        int gHeight = _grid.GetLength(1);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //skip queryTile itself
                if (x == 0 && y == 0) continue;
                
                //calculate gridPos of potential neighbour
                int neighbourX = gridPos.x + x;
                int neighbourY = gridPos.y + y;
                
                //check to see if there is land where you are looking at
                if (neighbourX >= 0 && neighbourX < gWidth && neighbourY >= 0 && neighbourY < gHeight)
                {
                    //check if a valid neighbour is at home (valid != NoEntry)
                    TileState neighbourTile = _grid[neighbourX, neighbourY];
                    if (neighbourTile.GetTileType() == TileState.TileType.NoEntry)
                    {
                        Debug.Log("a NoEntry skipped from neighbour search");
                        continue;
                    }
                    
                    //add a valid neighbour to the list
                    neighbors.Add(neighbourTile);
                }
            }
        }
        
        return neighbors;
    }

    public List<TileState> Dijkstra(TileState startTile, TileState endTile)
    {
        // If either the start or end tile is null, there is no valid path
        if (startTile == null || endTile == null) return null;

        // Initialize the path list to store the final shortest path
        List<TileState> openSet = new List<TileState>();
        
        // Initialize the closed set to store the tiles that have already been evaluated
        List<TileState> closedSet = new List<TileState>();

        // Add the starting tile to the path to initialize
        openSet.Add(startTile);

        // Set the starting tile's parent to null (as it has no predecessor)
        startTile.SetParent(null);

        // Set the starting tile's gCost to 0, explicitly marking the starting point
        startTile.SetGCost(0);

        // Initialize the current tile as the startTile
        // Comment: The algorithm will work on this "current" tile at each iteration
        while (openSet.Count > 0)
        {
            TileState current = GetLowestGCost(openSet);

            // Loop until we reach the target tile
            if (current == endTile)
            {
                return ReconstructPath(endTile);
            }
            
            openSet.Remove(current);
            closedSet.Add(current);
            // Pseudocode/Actual Steps:
            // 1. Get neighbors of the `current` tile
            List<TileState> neighborhood = GetNeighbors(current);
            // 2. Loop through neighbors to calculate and assign tentative gCosts
            foreach (TileState neighbor in neighborhood)
            {
                // Skip this neighbor if it is already in the closed set
                if (closedSet.Contains(neighbor)) continue;
                
                // Calculate tentative gCost
                float tentativeGCost = current.GetGCost() + GetDistance(current, neighbor);
                
                // If the tentative gCost is lower than the neighbor's current gCost, or it's not in the open set
                if (tentativeGCost < neighbor.GetGCost() || !openSet.Contains(neighbor))
                {
                    // Update the neighbor's gCost with the tentative value
                    neighbor.SetGCost(tentativeGCost);

                    // Set the neighbor's parent to the current tile for path reconstruction
                    neighbor.SetParent(current);

                    // If the neighbor isn't already in the open set, add it
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    private TileState GetLowestGCost(List<TileState> openSet)
    {
        TileState lowest = openSet[0];
        foreach (TileState tile in openSet)
        {
            if (tile.GetGCost() < lowest.GetGCost())
            {
                lowest = tile;
            }
        }
        return lowest;
    }
    private List<TileState> ReconstructPath(TileState endTile)
    {
        List<TileState> path = new List<TileState>();
        TileState current = endTile;

        // Traverse back through the parent nodes to build the path
        while (current != null)
        {
            path.Add(current);
            current = current.GetParent();
        }

        // Reverse the path to get it from start to end
        path.Reverse();
        return path;
    }
    
    private float GetDistance(TileState a, TileState b)
    {
        // Example: Use Manhattan distance for a grid
        return Mathf.Abs(a.GridPosition.x - b.GridPosition.x) + Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
    }
}

