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
    
}