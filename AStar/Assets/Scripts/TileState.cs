using UnityEngine;

public class TileState : MonoBehaviour
{
    public enum TileType
    {
        Unvisited,
        Home,
        Visited,
        Path,
        NoEntry,
        Target
    }

    // Reference to the TextMesh component for displaying numbers
    private TextMesh _textMesh;

    // Current state of the tile
    private TileType CurrentTileType { get; set; }
    public TileType GetTileType()
    {
        return CurrentTileType;
    }
    // Reference to the SpriteRenderer
    private SpriteRenderer _spriteRenderer;

    // Colors for different states
    public Color homeColor = Color.blue;
    public Color whiteColor = Color.white;
    public Color greenColor = Color.green;
    public Color blueColor = Color.cyan;
    public Color targetColor = Color.yellow;
    public Color blackColor = Color.black;

    // Pathfinding attributes
    
    private float GCost { get; set; } // Cost from start to this node

    public float GetGCost()
    {
        return GCost;
    }
    public void SetGCost(float gCost)
    {
        GCost = gCost;
    }
    private float HCost { get; set; } // Heuristic cost (for A*)

    public float GetHCost()
    {
        return HCost;
    }
    public void SetHCost(float hCost)
    {
        HCost = hCost;
    }
    public float FCost => GCost + HCost; // Total cost (for A*)

    public float GetFCost()
    {
        return FCost;
    }
    
    private TileState Parent { get; set; } // Parent tile (to trace the path)
    // Public getter for Parent
    public TileState GetParent()
    {
        return Parent;
    }
    // Public setter for Parent
    public void SetParent(TileState parent)
    {
        Parent = parent;
    }
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
        // Prevent the Home or Target tiles from being overwritten
        if (CurrentTileType == TileType.Home || CurrentTileType == TileType.Target || CurrentTileType == TileType.NoEntry)
        {
            // Optional: Log a warning if there’s an unintended overwrite attempt
            //Debug.LogWarning($"Attempted to overwrite a {CurrentTileType} tile at position {GridPosition}.");
            return;
        }

        CurrentTileType = type; // Update the tile's type
        UpdateTileStatus(); // Update visuals
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
                break;

            case TileType.Home:
                _spriteRenderer.color = homeColor;
                GCost = 0; // Starting point should have 0 cost
                HCost = 0;
                Parent = null;
                break;

            case TileType.NoEntry:
                _spriteRenderer.color = blackColor;
                GCost = -1.0f; // No entry tiles should have negative cost
                HCost = -1.0f; // remember this when doing checks at pathfinding time
                Parent = null;
                break;

            case TileType.Visited:
                _spriteRenderer.color = greenColor;
                break;

            case TileType.Path:
                _spriteRenderer.color = blueColor;
                break;

            case TileType.Target:
                _spriteRenderer.color = targetColor;
                GCost = 0; // Ensure the target starts with default values
                HCost = 0;
                break;
        }
        UpdateTileText();
    }

    // Method to update the text on the tile
    private void UpdateTileText()
    {
        if (_textMesh != null)
        {
            // Check the state of GCost and display accordingly
            if (GCost == Mathf.Infinity)
            {
                _textMesh.text = "inf"; // Show "infinity" for unvisited tiles
            }
            else if (GCost < 0.0f)
            {
                _textMesh.color = whiteColor;
                _textMesh.text = "N/A"; // Show "N/A" for no-entry tiles
            }
            else
            {
                _textMesh.text = GCost.ToString("0.0"); // Show the actual GCost value
            }
        }
    }
    public void UpdateCosts(float gCost, float hCost)
    {
        GCost = gCost;
        HCost = hCost;

        // Update the displayed text to reflect the new GCost
        UpdateTileText();
    }
    public void UpdateVisualFeedback(float gCost)
    {
        GCost = gCost; // Update GCost dynamically for visual purposes
        _spriteRenderer.color = Color.red; // Temporary visual feedback
        UpdateTileText(); // Automatically update the text based on the new GCost
    }

    public void ClearVisualFeedback()
    {
        // Restore to default color based on state
        _spriteRenderer.color = (_spriteRenderer.color == Color.red) ? whiteColor : _spriteRenderer.color;
        UpdateTileText(); // Ensure the text still reflects the numeric GCost
    }
}