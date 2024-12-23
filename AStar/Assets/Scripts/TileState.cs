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

    // Current state of the tile
    public TileType CurrentTileType { get; private set; }

    // Reference to the SpriteRenderer
    private SpriteRenderer spriteRenderer;

    // Colors for different states
    public Color homeColor = Color.blue;
    public Color whiteColor = Color.white;
    public Color redColor = Color.red;
    public Color greenColor = Color.green;
    public Color blueColor = Color.cyan;
    public Color targetColor = Color.yellow;
    public Color blackColor = Color.black;

    // Pathfinding attributes
    public float GCost { get; set; } // Cost from start to this node
    public float HCost { get; set; } // Heuristic cost (for A*)
    public float FCost => GCost + HCost; // Total cost (for A*)
    public TileState Parent { get; set; } // Parent tile (to trace the path)
    public bool IsWalkable => CurrentTileType != TileType.NoEntry; // Check if the tile is a valid node in pathfinding

    // Neighbors (filled by GridManager later)
    public TileState[] Neighbors { get; set; }

    private void Awake()
    {
        // Initialize SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        CurrentTileType = TileType.Home; // Default state
        UpdateTileStatus();
    }

    // Public method to set the current tile type
    public void SetTileType(TileType type)
    {
        CurrentTileType = type; // Update the tile's type
        UpdateTileStatus(); // Update the color based on the new type
    }

    // Private method to update the tile's color
    private void UpdateTileStatus()
    {
        Debug.Log($"Updating tile {name} to type {CurrentTileType}");

        switch (CurrentTileType)
        {
            case TileType.Unvisited:
                spriteRenderer.color = whiteColor;
                GCost = 0;
                HCost = 0;
                Parent = null;
                break;

            case TileType.Home:
                spriteRenderer.color = homeColor;
                GCost = 0; // Starting point should have 0 cost
                HCost = 0;
                Parent = null;
                break;

            case TileType.Processing:
                spriteRenderer.color = redColor;

                // Process neighbors for Dijkstra's algorithm
                ProcessNeighbors();
                break;

            case TileType.Visited:
                spriteRenderer.color = greenColor;
                break;

            case TileType.Path:
                spriteRenderer.color = blueColor;
                break;

            case TileType.NoEntry:
                spriteRenderer.color = blackColor;
                GCost = float.MaxValue; // NoEntry is impassable
                HCost = float.MaxValue;
                Parent = null;
                break;

            case TileType.Target:
                spriteRenderer.color = targetColor;
                HCost = 0; // Target tile does not require heuristic cost
                break;
        }
    
        Debug.Log($"Updated {name}: GCost = {GCost}, HCost = {HCost}, Parent = {Parent?.name}");
    }

    // Method to process neighbors in Dijkstra's logic
    private void ProcessNeighbors()
    {
        if (Neighbors == null || Neighbors.Length == 0)
        {
            Debug.Log($"Tile {name} has no neighbors to process.");
            return;
        }

        foreach (var neighbor in Neighbors)
        {
            // Ignore null, non-walkable, or already visited tiles
            if (neighbor == null || !neighbor.IsWalkable || neighbor.CurrentTileType == TileType.Visited)
            {
                continue;
            }

            // Tentative GCost: Current GCost + cost of moving to this neighbor
            float tentativeGCost = GCost + GetDistance(neighbor);

            if (tentativeGCost < neighbor.GCost)
            {
                // Update the neighbor's GCost and Parent if a better path is found
                neighbor.GCost = tentativeGCost;
                neighbor.Parent = this;

                Debug.Log($"Updated {neighbor.name}'s GCost to {neighbor.GCost} (Parent: {name})");

                // Mark the neighbor for further processing
                neighbor.SetTileType(TileType.Processing);
            }
        }

        // Mark this tile as "Visited" after processing neighbors
        SetTileType(TileType.Visited);
    }

    // Calculate the distance cost between tiles
    private float GetDistance(TileState neighbor)
    {
        // Grid-based simple movement costs
        Vector2Int thisPos = GetGridPosition();
        Vector2Int neighborPos = neighbor.GetGridPosition();

        int dx = Mathf.Abs(thisPos.x - neighborPos.x);
        int dy = Mathf.Abs(thisPos.y - neighborPos.y);

        if (dx + dy == 1) // Orthogonal neighbors
        {
            return 1.0f;
        }
        else if (dx == 1 && dy == 1) // Diagonal neighbors, if movement is allowed
        {
            return 1.414f; // Approximation for √2
        }

        return float.MaxValue; // Invalid neighbor, should not happen
    }

    // Assuming GridManager sets grid positions during initialization
    public Vector2Int GridPosition { get; set; }

    public Vector2Int GetGridPosition()
    {
        return GridPosition;
    }

    // Click-related logic retained
    public static TileState lastClickedTile;

    private void OnMouseDown()
    {
        // Reset the previously clicked tile
        if (lastClickedTile != null && lastClickedTile != this)
        {
            lastClickedTile.SetTileType(TileType.Unvisited);
        }

        // Set this tile to "Processing" (start processing neighbors)
        if (CurrentTileType == TileType.Unvisited)
        {
            SetTileType(TileType.Processing);
            Debug.Log("Tile is now being processed.");
        }

        // Update the static reference
        lastClickedTile = this;
    }
}