using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{
    private Queue<TileState> processingQueue; // Tiles to process
    private bool isRunning; // Whether the algorithm is running

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting Pathfinding Algorithm...");
        InitializeAlgorithm();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning && processingQueue.Count > 0)
        {
            // Process the next tile in the queue
            ProcessNextTile();
        }
        else if (isRunning && processingQueue.Count == 0)
        {
            Debug.Log("Pathfinding Complete");
            isRunning = false;
        }
    }

    // Initialize the algorithm
    private void InitializeAlgorithm()
    {
        processingQueue = new Queue<TileState>();
        isRunning = true;

        // Locate the starting tile, add it to the queue
        TileState startTile = FindStartTile();
        if (startTile != null)
        {
            startTile.SetTileType(TileState.TileType.Processing);
            processingQueue.Enqueue(startTile);
        }
        else
        {
            Debug.LogError("No start tile found!");
            isRunning = false;
        }
    }

    // Find the starting tile (assumes one "Home" tile in the scene)
    private TileState FindStartTile()
    {
        TileState[] allTiles = FindObjectsOfType<TileState>();
        foreach (var tile in allTiles)
        {
            if (tile.CurrentTileType == TileState.TileType.Home)
            {
                return tile;
            }
        }
        return null;
    }

    // Process the next tile in the queue
    private void ProcessNextTile()
    {
        TileState currentTile = processingQueue.Dequeue();

        // Ensure the tile's neighbors are processed (handle Dijkstra logic)
        foreach (var neighbor in currentTile.Neighbors)
        {
            if (neighbor == null || !neighbor.IsWalkable || neighbor.CurrentTileType == TileState.TileType.Visited)
                continue; // Skip invalid, non-walkable, or visited tiles

            // Calculate tentative cost to move to the neighbor
            float tentativeGCost = currentTile.GCost + GetDistance(currentTile, neighbor);

            if (tentativeGCost < neighbor.GCost)
            {
                neighbor.GCost = tentativeGCost;
                neighbor.Parent = currentTile;

                // Enqueue the neighbor for further processing
                if (neighbor.CurrentTileType != TileState.TileType.Processing)
                {
                    neighbor.SetTileType(TileState.TileType.Processing);
                    processingQueue.Enqueue(neighbor);
                }
            }
        }

        // Mark the current tile as "Visited" after processing
        currentTile.SetTileType(TileState.TileType.Visited);
    }

    // Calculate the distance between two tiles
    private float GetDistance(TileState from, TileState to)
    {
        Vector2Int fromPos = from.GetGridPosition();
        Vector2Int toPos = to.GetGridPosition();

        int dx = Mathf.Abs(fromPos.x - toPos.x);
        int dy = Mathf.Abs(fromPos.y - toPos.y);

        if (dx + dy == 1) // Orthogonal neighbor
        {
            return 1.0f;
        }
        else if (dx == 1 && dy == 1) // Diagonal neighbor
        {
            return 1.414f; // Approximation for sqrt(2)
        }

        return float.MaxValue; // Invalid distance
    }
}