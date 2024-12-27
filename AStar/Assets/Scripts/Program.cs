using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main class for the A* Pathfinding algorithm

public class Program : MonoBehaviour
{
    private GridManager _gridManager; //GridManager handles setting up the grid
    private PathFinding _pathFinding; //PathFinding handles the A* algorithm
    private TileState _homeTile; //Home tile
    private TileState _targetTile; //Target tile

    void Start()
    {
        // Find the GridManager in the scene
        _gridManager = FindObjectOfType<GridManager>();

        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene!");
            return;
        }

        Debug.Log("Calling GenerateGrid...");
        _gridManager.GetGenerateGrid()?.Invoke();

        Debug.Log("Calling RandomlySetTheBoard...");
        _gridManager.GetRandomlySetTheBoard()?.Invoke(); //Creates a random scenario

        // Initialize PathFinding with the grid from GridManager
        TileState[,] grid = _gridManager.GetGrid();
        _pathFinding = new PathFinding(grid);

        // Find Home
        _homeTile = _pathFinding.FindTile(TileState.TileType.Home);
        _targetTile = _pathFinding.FindTile(TileState.TileType.Target);

        if (_homeTile == null || _targetTile == null)
        {
            Debug.LogError("Home or Target tile not found!");
            return;
        }

        // Start the A* Pathfinding Coroutine
        StartCoroutine(VisualizeAStarPathFinding());
    }
    
    private IEnumerator VisualizeAStarPathFinding()
    {
        // Initialize A* algorithm
        _pathFinding.InitializeAStar(_homeTile, _targetTile);

        bool isComplete = false;
        while (!isComplete)
        {
            // Execute one step of the A* algorithm
            isComplete = _pathFinding.StepAStar();

            // Delay for visualization
            yield return new WaitForSeconds(0.04f); // Adjust timing for desired speed
        }

        // Retrieve and display the completed path
        List<TileState> path = _pathFinding.GetFinalPath();
        if (path != null)
        {
            Debug.Log("Path found!");
            foreach (TileState tile in path)
            {
                tile.SetTileType(TileState.TileType.Path); // Highlight the final path
            }
        }
        else
        {
            Debug.Log("No path found.");
        }
    }
}

