using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    private TileState[,] _grid; // Reference to the grid
    private List<TileState> _openSet;
    private List<TileState> _closedSet;
    private TileState _endTile;
    private TileState _currentTile;

    public PathFinding(TileState[,] grid)
    {
        _grid = grid;
    }

    public void InitializeDijkstra(TileState startTile, TileState endTile)
    {
        if (startTile == null || endTile == null)
        {
            Debug.LogError("Start or End tile is null, cannot run Dijkstra!");
            return;
        }

        // Initialize variables for the algorithm
        _openSet = new List<TileState>();
        _closedSet = new List<TileState>();
        _endTile = endTile;

        _openSet.Add(startTile);

        startTile.SetParent(null);
        startTile.SetGCost(0);
    }

    public bool StepDijkstra()
    {
        // If there are no more tiles to process, the algorithm is done
        if (_openSet.Count == 0) return true;

        // Get the tile with the lowest G cost
        _currentTile = GetLowestGCost(_openSet);

        // Mark the current tile as visited visually
        _currentTile.SetTileType(TileState.TileType.Visited);

        // If we reached the end tile, mark it and stop
        if (_currentTile == _endTile)
        {
            _endTile.SetTileType(TileState.TileType.Target);
            Debug.Log("Pathfinding complete!");
            return true;
        }

        _openSet.Remove(_currentTile);
        _closedSet.Add(_currentTile);

        // Process neighbors
        List<TileState> neighbors = GetNeighbors(_currentTile);
        foreach (TileState neighbor in neighbors)
        {
            if (_closedSet.Contains(neighbor)) continue;

            float tentativeGCost = _currentTile.GetGCost() + GetDistance(_currentTile, neighbor);
            if (tentativeGCost < neighbor.GetGCost() || !_openSet.Contains(neighbor))
            {
                neighbor.SetGCost(tentativeGCost);
                neighbor.SetParent(_currentTile);

                if (!_openSet.Contains(neighbor))
                {
                    _openSet.Add(neighbor);
                    neighbor.SetTileType(TileState.TileType.Visited); // Update neighbor state visually
                }
            }
        }

        return false;
    }

    public List<TileState> GetFinalPath()
    {
        if (_endTile.GetParent() == null) return null; // No path exists
        return ReconstructPath(_endTile);
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

    private List<TileState> _aStarOpenSet;
    private HashSet<TileState> _aStarClosedSet;

    public void InitializeAStar(TileState startTile, TileState endTile)
    {
        if (startTile == null || endTile == null)
        {
            Debug.LogError("Start or End tile is null, cannot run A*!");
            return;
        }

        _aStarOpenSet = new List<TileState>();
        _aStarClosedSet = new HashSet<TileState>();
        _endTile = endTile;

        startTile.SetGCost(0);
        startTile.SetHCost(GetHeuristicDistance(startTile, endTile));
        startTile.SetParent(null);

        _aStarOpenSet.Add(startTile);
    }

    public bool StepAStar()
    {
        if (_aStarOpenSet == null || _aStarOpenSet.Count == 0)
        {
            Debug.Log("Open set is empty. A* failed to find a path.");
            return true; // Algorithm has completed but no path
        }

        // Get the tile with the lowest F cost
        TileState currentTile = _aStarOpenSet[0];
        foreach (TileState tile in _aStarOpenSet)
        {
            if (tile.GetFCost() < currentTile.GetFCost() ||
                (tile.GetFCost() == currentTile.GetFCost() && tile.GetHCost() < currentTile.GetHCost()))
            {
                currentTile = tile;
            }
        }

        // If we reached the target, algorithm is complete
        if (currentTile == _endTile)
        {
            Debug.Log("Path found!");
            return true; // A* is done
        }

        // Move current tile from open to closed set
        _aStarOpenSet.Remove(currentTile);
        _aStarClosedSet.Add(currentTile);
        currentTile.SetTileType(TileState.TileType.Visited); // Optional visual feedback

        // Process neighbors
        foreach (TileState neighbor in GetNeighbors(currentTile))
        {
            if (!neighbor.IsWalkable || _aStarClosedSet.Contains(neighbor))
                continue;

            float tentativeGCost = currentTile.GetGCost() + GetDistance(currentTile, neighbor);

            if (tentativeGCost < neighbor.GetGCost() || !_aStarOpenSet.Contains(neighbor))
            {
                neighbor.SetGCost(tentativeGCost);
                neighbor.SetHCost(GetHeuristicDistance(neighbor, _endTile));
                neighbor.SetParent(currentTile);

                if (!_aStarOpenSet.Contains(neighbor))
                {
                    _aStarOpenSet.Add(neighbor);
                    neighbor.SetTileType(TileState.TileType.Visited); // Optional for visualization
                }
            }
        }

        return false; // A* needs to continue
    }

// Heuristic function (Manhattan distance)
    private float GetHeuristicDistance(TileState a, TileState b)
    {
        return Mathf.Abs(a.GridPosition.x - b.GridPosition.x) + Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
    }
}