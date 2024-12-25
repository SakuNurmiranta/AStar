using UnityEngine;

public class TileState : MonoBehaviour
{
    public enum TileType
    {
        Unvisited,
        Home,
        Processing,
        Visited,
        Path,
        NoEntry,
        Target
    }

    // Reference to the TextMesh component for displaying numbers
    private TextMesh _textMesh;

    // Current state of the tile
    private TileType CurrentTileType { get; set; }

    // Reference to the SpriteRenderer
    private SpriteRenderer _spriteRenderer;

    // Colors for different states
    public Color homeColor = Color.blue;
    public Color whiteColor = Color.white;
    public Color redColor = Color.red;
    public Color greenColor = Color.green;
    public Color blueColor = Color.cyan;
    public Color targetColor = Color.yellow;
    public Color blackColor = Color.black;

    // Pathfinding attributes
    private float GCost { get; set; } // Cost from start to this node
    private float HCost { get; set; } // Heuristic cost (for A*)
    public float FCost => GCost + HCost; // Total cost (for A*)
    private TileState Parent { get; set; } // Parent tile (to trace the path)
    public bool IsWalkable => CurrentTileType != TileType.NoEntry; // Check if the tile is a valid node in pathfinding

    // New: Grid position of this tile in the grid
    public Vector2Int GridPosition { get; set; } // Tile's coordinates in the grid

    private void Awake()
    {
        // Initialize components
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _textMesh = GetComponentInChildren<TextMesh>(); // Assumes TextMesh is a child GameObject

        if (_textMesh == null)
        {
            Debug.LogError("TextMesh not found! Please add a TextMesh as a child of the Tile.");
        }

        CurrentTileType = TileType.Unvisited; // Default state
        UpdateTileStatus();
    }

    // Public method to set the current tile type
    public void SetTileType(TileType type)
    {
        CurrentTileType = type; // Update the tile's type
        UpdateTileStatus(); // Update the color and text based on the new type
    }

    // Private method to update the tile's color and text
    private void UpdateTileStatus()
    {
        switch (CurrentTileType)
        {
            case TileType.Unvisited:
                _spriteRenderer.color = whiteColor;
                GCost = Mathf.Infinity;
                HCost = Mathf.Infinity;
                Parent = null;
                UpdateTileText("inf");
                break;

            case TileType.Home:
                _spriteRenderer.color = homeColor;
                GCost = 0; // Starting point should have 0 cost
                HCost = 0;
                Parent = null;
                UpdateTileText("0");
                break;

            case TileType.NoEntry:
                _spriteRenderer.color = blackColor;
                GCost = float.MaxValue; // NoEntry is impassable
                HCost = float.MaxValue;
                Parent = null;
                UpdateTileText("n/a");
                break;

            case TileType.Processing:
                _spriteRenderer.color = redColor;
                UpdateTileText(GCost.ToString("0.0")); // Show precise cost
                break;

            case TileType.Visited:
                _spriteRenderer.color = greenColor;
                UpdateTileText(GCost.ToString("0.0"));
                break;

            case TileType.Path:
                _spriteRenderer.color = blueColor;
                UpdateTileText(GCost.ToString("0.0"));
                break;

            case TileType.Target:
                _spriteRenderer.color = targetColor;
                GCost = 0; // Ensure the target starts with default values
                HCost = 0;
                UpdateTileText(GCost.ToString("0"));
                break;
        }
    }

    // Method to update the text on the tile
    private void UpdateTileText(string text)
    {
        if (_textMesh != null)
        {
            _textMesh.text = text; // Set the text displayed on the tile
        }
    }
}