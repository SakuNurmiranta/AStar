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

        // Run Dijkstra and log the result
        var path = _pathFinding.Dijkstra(_homeTile, _targetTile);
        if (path != null && path.Count > 0)
        {
            string pathOutput = string.Join(" -> ", path.ConvertAll(tile => tile.GridPosition.ToString()));
            Debug.Log($"Shortest path found: {pathOutput}");
        }
        else
        {
            Debug.Log("No path found between the home tile and target tile.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Optionally use this to implement any logic that needs to run every frame
        Debug.Log($"Program Update tick..., home tile: {_homeTile.GridPosition}, target tile: {_targetTile.GridPosition}");
    }
}