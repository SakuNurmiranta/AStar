using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab; // Assign the Tile prefab here
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float tileSpacing = 1.1f; // Spacing between tiles

    private TileState[,] grid;

    void Start()
    {
        GenerateGrid();
        RandomlySetTheBoard();
    }

    private void GenerateGrid()
    {
        grid = new TileState[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Instantiate the tile prefab at the correct position
                Vector3 position = new Vector3(x * tileSpacing, y * tileSpacing, 0);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);

                // Set the parent in the hierarchy for better organization
                tile.transform.parent = transform;

                // Assign tile to the grid array
                grid[x, y] = tile.GetComponent<TileState>();
            }
        }
    }

    private void RandomlySetTileStates()
    {
        foreach (var tile in grid)
        {
            if (tile != null) // Ensure the tile is not null
            {
                // Randomly assign a TileType
                tile.SetTileType((TileState.TileType)Random.Range(0, System.Enum.GetValues(typeof(TileState.TileType)).Length));
            }
        }
    }

    private void RandomlySetTheBoard()
{
    // Reset all tiles to the "Neutral" type
    foreach (var tile in grid)
    {
        if (tile != null) // Ensure the tile is properly initialized
        {
            tile.SetTileType(TileState.TileType.Unvisited);
        }
    }

    // Define the sides for "Home" and "Target"
    bool isHorizontal = Random.Range(0, 2) == 0; // Randomly decide if alignment is horizontal or vertical

    int homeX, homeY, targetX, targetY;
    
    if (isHorizontal)
    {
        // Horizontal placement: Home on the left, Target on the right
        homeX = 0; // Left-most column
        homeY = Random.Range(0, grid.GetLength(1)); // Random row
        targetX = grid.GetLength(0) - 1; // Right-most column
        targetY = Random.Range(0, grid.GetLength(1)); // Random row
    }
    else
    {
        // Vertical placement: Home on the top, Target on the bottom
        homeX = Random.Range(0, grid.GetLength(0)); // Random column
        homeY = 0; // Top-most row
        targetX = Random.Range(0, grid.GetLength(0)); // Random column
        targetY = grid.GetLength(1) - 1; // Bottom-most row
    }

    // Set the Home and Target tiles
    grid[homeX, homeY].SetTileType(TileState.TileType.Home);
    grid[targetX, targetY].SetTileType(TileState.TileType.Target);

    // Define the center of the grid for NoEntry tiles
    int centerX = grid.GetLength(0) / 2;
    int centerY = grid.GetLength(1) / 2;

    // Align NoEntry tiles based on the Home-Target relationship
    if (!isHorizontal)
    {
        // Center-aligned "NoEntry" tiles horizontally
        grid[centerX - 2, centerY].SetTileType(TileState.TileType.NoEntry);
        grid[centerX - 1, centerY].SetTileType(TileState.TileType.NoEntry);
        grid[centerX, centerY].SetTileType(TileState.TileType.NoEntry);
        grid[centerX + 1, centerY].SetTileType(TileState.TileType.NoEntry);
        grid[centerX + 2, centerY].SetTileType(TileState.TileType.NoEntry);
    }
    else
    {
        // Center-aligned "NoEntry" tiles vertically
        grid[centerX, centerY - 2].SetTileType(TileState.TileType.NoEntry);
        grid[centerX, centerY - 1].SetTileType(TileState.TileType.NoEntry);
        grid[centerX, centerY].SetTileType(TileState.TileType.NoEntry);
        grid[centerX, centerY + 1].SetTileType(TileState.TileType.NoEntry);
        grid[centerX, centerY + 2].SetTileType(TileState.TileType.NoEntry);
    }
}
}