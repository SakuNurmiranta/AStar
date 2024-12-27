using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab; // Assign the Tile prefab here
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float tileSpacing = 1.0f; // Spacing between tiles

    private TileState[,] _grid; // Grid data structure
    private TileState _startTile; // Home tile
    private TileState _targetTile; // Target tile

    public TileState[,] GetGrid()
    {
        return _grid;
    }

    public System.Action GetGenerateGrid()
    {
        return GenerateGrid;
    }

    public System.Action GetRandomlySetTheBoard()
    {
        return RandomlySetTheBoard;
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
            homeY = Random.Range(0, gridHeight); // Home placed along the left edge
            targetX = gridWidth - 1;
            targetY = Random.Range(0, gridHeight); // Target placed along the right edge
        }
        else
        {
            homeX = Random.Range(0, gridWidth); // Home placed along the bottom edge
            homeY = 0;
            targetX = Random.Range(0, gridWidth); // Target placed along the top edge
            targetY = gridHeight - 1;
        }

        _startTile = _grid[homeX, homeY];
        _targetTile = _grid[targetX, targetY];

        _startTile.SetTileType(TileState.TileType.Home);
        _targetTile.SetTileType(TileState.TileType.Target);

        // Step 3: Place NoEntry tiles to block the path between Home and Target
        if (isHorizontal)
        {
            // Block horizontally by adding NoEntry tiles in the middle columns between Home and Target
            int centerX = gridWidth / 2;

            for (int x = centerX - 1; x <= centerX + 1; x++) // Block 3 columns at the center
            {
                for (int y = Mathf.Min(homeY, targetY); y <= Mathf.Max(homeY, targetY); y++)
                {
                    // Skip if it's the Home or Target tile
                    if ((x == homeX && y == homeY) || (x == targetX && y == targetY))
                        continue;

                    if (_grid[x, y] != null)
                    {
                        _grid[x, y].SetTileType(TileState.TileType.NoEntry);
                    }
                }
            }
        }
        else
        {
            // Block vertically by adding NoEntry tiles in the middle rows between Home and Target
            int centerY = gridHeight / 2;

            for (int y = centerY - 1; y <= centerY + 1; y++) // Block 3 rows at the center
            {
                for (int x = Mathf.Min(homeX, targetX); x <= Mathf.Max(homeX, targetX); x++)
                {
                    // Skip if it's the Home or Target tile
                    if ((x == homeX && y == homeY) || (x == targetX && y == targetY))
                        continue;

                    if (_grid[x, y] != null)
                    {
                        _grid[x, y].SetTileType(TileState.TileType.NoEntry);
                    }
                }
            }
        }
    }
}