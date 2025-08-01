using UnityEngine;

/// <summary>
/// Optional component to add visual representation to boundary walls for debugging.
/// Adds sprite renderers to make invisible walls visible during testing.
/// </summary>
[RequireComponent(typeof(BoundaryWall))]
public class BoundaryWallVisualizer : MonoBehaviour
{
    [Header("Visual Settings")]
    [Tooltip("Enable visual representation of boundary wall")]
    public bool showVisual = true;
    
    [Tooltip("Color for wall visualization")]
    public Color wallColor = new Color(1f, 1f, 1f, 0.3f);
    
    [Tooltip("Sprite to use for wall visualization")]
    public Sprite wallSprite;
    
    [Tooltip("Sorting order for wall rendering")]
    public int sortingOrder = -10;
    
    // Components
    private SpriteRenderer spriteRenderer;
    private BoundaryWall boundaryWall;
    private BoxCollider2D boxCollider;
    
    private void Awake()
    {
        boundaryWall = GetComponent<BoundaryWall>();
        boxCollider = GetComponent<BoxCollider2D>();
        SetupVisualizer();
    }
    
    private void Start()
    {
        UpdateVisualization();
    }
    
    private void SetupVisualizer()
    {
        // Get or create sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null && showVisual)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        
        if (spriteRenderer != null)
        {
            // Use default Unity sprite if none assigned
            if (wallSprite == null)
            {
                wallSprite = CreateDefaultSprite();
            }
            
            spriteRenderer.sprite = wallSprite;
            spriteRenderer.sortingOrder = sortingOrder;
            
            // Set color based on wall type
            SetWallColor();
        }
    }
    
    private void UpdateVisualization()
    {
        if (spriteRenderer == null) return;
        
        spriteRenderer.enabled = showVisual;
        
        if (showVisual && boxCollider != null)
        {
            // Scale sprite to match collider size
            Vector2 colliderSize = boxCollider.size;
            if (wallSprite != null)
            {
                float spriteWidth = wallSprite.bounds.size.x;
                float spriteHeight = wallSprite.bounds.size.y;
                
                transform.localScale = new Vector3(
                    colliderSize.x / spriteWidth,
                    colliderSize.y / spriteHeight,
                    1f
                );
            }
        }
    }
    
    private void SetWallColor()
    {
        if (boundaryWall == null || spriteRenderer == null) return;
        
        // Use configuration color if available
        if (boundaryWall.config != null)
        {
            BoundaryWallConfig wallConfig = boundaryWall.config.GetBoundaryConfig(boundaryWall.wallType);
            wallColor = wallConfig.visualColor;
            wallColor.a = 0.3f; // Semi-transparent
        }
        else
        {
            // Default colors based on wall type
            switch (boundaryWall.wallType)
            {
                case BoundaryType.Top:
                    wallColor = new Color(0.2f, 0.4f, 0.8f, 0.3f); // Blue
                    break;
                case BoundaryType.Left:
                case BoundaryType.Right:
                    wallColor = new Color(0.2f, 0.8f, 0.4f, 0.3f); // Green
                    break;
                case BoundaryType.Bottom:
                    wallColor = new Color(0.8f, 0.2f, 0.2f, 0.3f); // Red
                    break;
            }
        }
        
        spriteRenderer.color = wallColor;
    }
    
    private Sprite CreateDefaultSprite()
    {
        // Create a simple white square sprite
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        return Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );
    }
    
    public void ToggleVisibility()
    {
        showVisual = !showVisual;
        UpdateVisualization();
    }
    
    private void OnValidate()
    {
        if (Application.isPlaying && spriteRenderer != null)
        {
            spriteRenderer.enabled = showVisual;
            spriteRenderer.color = wallColor;
            spriteRenderer.sortingOrder = sortingOrder;
        }
    }
}