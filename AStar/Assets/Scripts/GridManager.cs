using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab; // Assign the Tile prefab here
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float tileSpacing = 1.1f; // Spacing between tiles

    private TileState[,] _grid; // Grid data structure
    private TileState _startTile; // Home tile
    private TileState _targetTile; // Target tile
    

    void Start()
    {
        GenerateGrid(); // Generate the grid
        RandomlySetTheBoard(); // Set the board logic
    }

    // Generate the grid dynamically
    private void GenerateGrid()
    {
        _grid = new TileState[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Instantiate the tile prefab at the correct position
                Vector3 position = new Vector3(x * tileSpacing, y * tileSpacing, 0);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);

                // Set the parent in the hierarchy for better organization
                tile.transform.parent = transform;

                // Assign tile to the grid array and set its initial state
                TileState tileState = tile.GetComponent<TileState>();
                if (tileState != null)
                {
                    tileState.GridPosition = new Vector2Int(x, y); // Assign grid position
                    tileState.SetTileType(TileState.TileType.Unvisited); // Default state
                }

                // Add the tile to the grid array
                _grid[x, y] = tileState;
            }
        }
    }
    

    // Randomly initialize the board setup
    private void RandomlySetTheBoard()
    {
        // Step 1: Reset all tiles to Unvisited
        foreach (var tile in _grid)
        {
            if (tile != null)
            {
                tile.SetTileType(TileState.TileType.Unvisited);
            }
        }

        // Step 2: Define Home and Target tiles
        bool isHorizontal = Random.Range(0, 2) == 0;

        int homeX, homeY, targetX, targetY;

        if (isHorizontal)
        {
            homeX = 0;
            homeY = Random.Range(0, gridHeight);
            targetX = gridWidth - 1;
            targetY = Random.Range(0, gridHeight);
        }
        else
        {
            homeX = Random.Range(0, gridWidth);
            homeY = 0;
            targetX = Random.Range(0, gridWidth);
            targetY = gridHeight - 1;
        }

        _startTile = _grid[homeX, homeY];
        _targetTile = _grid[targetX, targetY];

        _startTile.SetTileType(TileState.TileType.Home);
        _targetTile.SetTileType(TileState.TileType.Target);

        // Step 3: Add NoEntry tiles near the center
        int centerX = gridWidth / 2;
        int centerY = gridHeight / 2;

        if (isHorizontal)
        {
            for (int y = Mathf.Max(0, centerY - 2); y <= Mathf.Min(gridHeight - 1, centerY + 2); y++)
            {
                if (_grid[centerX, y] != null)
                {
                    _grid[centerX, y].SetTileType(TileState.TileType.NoEntry);
                }
            }
        }
        else
        {
            for (int x = Mathf.Max(0, centerX - 2); x <= Mathf.Min(gridWidth - 1, centerX + 2); x++)
            {
                if (_grid[x, centerY] != null)
                {
                    _grid[x, centerY].SetTileType(TileState.TileType.NoEntry);
                }
            }
        }
    }
}