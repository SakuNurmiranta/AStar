using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Program : MonoBehaviour
{
    private GridManager _gridManager;
    private PathFinding _pathFinding;
    private TileState _homeTile;
    private TileState _targetTile;

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
        _gridManager.GetRandomlySetTheBoard()?.Invoke();
        
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

        // Start the Dijkstra coroutine
        StartCoroutine(VisualizePathFinding());
    }
    private IEnumerator VisualizePathFinding()
    {
        _pathFinding.InitializeDijkstra(_homeTile, _targetTile);

        bool isComplete = false;
        while (!isComplete)
        {
            // Execute one step of the algorithm
            isComplete = _pathFinding.StepDijkstra();

            // Yield for visual updates
            yield return new WaitForSeconds(0.01f); // Adjust timing for desired visual speed
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

