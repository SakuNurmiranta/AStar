using UnityEngine;

public class TileState : MonoBehaviour
{
    public enum TileType
    {
        Neutral,
        Home,
        Red,
        Green,
        Blue,
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
    
    private void Awake()
    {
        // Initialize SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        CurrentTileType = TileType.Home; // Default state
        UpdateTileColor();
    }

    // Public method to set the current tile type
    public void SetTileType(TileType type)
    {
        CurrentTileType = type; // Update the tile's type
        UpdateTileColor(); // Update the color based on the new type
    }

    // Private method to update the tile's color
    private void UpdateTileColor()
    {
        switch (CurrentTileType)
        {
            case TileType.Neutral:
                spriteRenderer.color = whiteColor;
                break;
            case TileType.Home:
                spriteRenderer.color = homeColor;
                break;
            case TileType.Red:
                spriteRenderer.color = redColor;
                break;
            case TileType.Green:
                spriteRenderer.color = greenColor;
                break;
            case TileType.Blue:
                spriteRenderer.color = blueColor;
                break;
            case TileType.NoEntry:
                spriteRenderer.color = blackColor;
                break;
            case TileType.Target:
                spriteRenderer.color = targetColor;
                break;
        }
    }

    // Method to cycle through states when clicked
    private void OnMouseDown()
    {
        // Increment the tile type and loop back to Home if necessary
        CurrentTileType = (TileType)(((int)CurrentTileType + 1) % System.Enum.GetValues(typeof(TileType)).Length);

        // Update the color based on the new state
        UpdateTileColor();
    }
}