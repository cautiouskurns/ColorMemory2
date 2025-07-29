#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor setup script for creating and configuring the Ball GameObject with physics components.
/// Creates Ball with Rigidbody2D, CircleCollider2D, Physics Material 2D, and visual representation.
/// </summary>
public static class CreateBallGameObjectSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Ball GameObject";
    private const string BALL_NAME = "Ball";
    private const string GAME_AREA_NAME = "GameArea";
    private const string BALL_LAYER_NAME = "Ball";
    private const float BALL_RADIUS = 0.25f;
    private const float BALL_MASS = 1f;
    private const float PHYSICS_MATERIAL_BOUNCINESS = 1f;
    private const float PHYSICS_MATERIAL_FRICTION = 0f;
    
    /// <summary>
    /// Creates and configures the Ball GameObject with all required physics components.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateBallGameObject()
    {
        Debug.Log("⚪ [Task 1.1.1.2] Starting Ball GameObject configuration...");
        
        try
        {
            // Step 1: Ensure GameArea parent exists
            GameObject gameArea = EnsureGameAreaExists();
            
            // Step 2: Create Ball GameObject with hierarchy placement
            GameObject ball = CreateBallInHierarchy(gameArea);
            
            // Step 3: Configure physics components
            Rigidbody2D rb = ConfigureRigidbody2D(ball);
            CircleCollider2D collider = ConfigureCircleCollider2D(ball);
            
            // Step 4: Create and assign Physics Material 2D
            PhysicsMaterial2D physicsMaterial = CreatePhysicsMaterial();
            AssignPhysicsMaterial(collider, physicsMaterial);
            
            // Step 5: Configure collision layers
            ConfigureCollisionLayers(ball);
            
            // Step 6: Set up visual representation
            SpriteRenderer renderer = ConfigureSpriteRenderer(ball);
            Sprite ballSprite = CreateBallSprite();
            renderer.sprite = ballSprite;
            
            // Step 7: Create prefab
            CreateBallPrefab(ball);
            
            // Step 8: Validate BallData integration capability
            ValidateBallDataIntegration();
            
            // Step 9: Final setup and selection
            Selection.activeGameObject = ball;
            EditorUtility.SetDirty(ball);
            
            LogSuccessfulSetup(ball, rb, collider, physicsMaterial);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.1.2] Ball GameObject creation failed: {e.Message}");
            Debug.LogError("📋 Please check Unity Physics2D system is properly configured.");
        }
    }
    
    /// <summary>
    /// Menu validation - prevents duplicate Ball creation.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateBallGameObject()
    {
        GameObject existingBall = GameObject.Find(BALL_NAME);
        if (existingBall != null)
        {
            Debug.LogWarning($"⚠️ Ball GameObject already exists at: {GetGameObjectPath(existingBall)}");
        }
        return existingBall == null;
    }
    
    /// <summary>
    /// Ensures GameArea parent container exists, creating if necessary.
    /// </summary>
    private static GameObject EnsureGameAreaExists()
    {
        GameObject gameArea = GameObject.Find(GAME_AREA_NAME);
        if (gameArea == null)
        {
            gameArea = new GameObject(GAME_AREA_NAME);
            gameArea.transform.position = Vector3.zero;
            Debug.Log($"📦 Created missing GameArea parent container");
        }
        else
        {
            Debug.Log($"✅ Found existing GameArea parent");
        }
        return gameArea;
    }
    
    /// <summary>
    /// Creates Ball GameObject within GameArea hierarchy.
    /// </summary>
    private static GameObject CreateBallInHierarchy(GameObject gameArea)
    {
        GameObject ball = new GameObject(BALL_NAME);
        ball.transform.SetParent(gameArea.transform, false);
        ball.transform.localPosition = Vector3.zero;
        
        Debug.Log($"📦 [Step 1/7] Ball GameObject created in hierarchy: {GAME_AREA_NAME}/{BALL_NAME}");
        return ball;
    }
    
    /// <summary>
    /// Configures Rigidbody2D component for arcade physics.
    /// </summary>
    private static Rigidbody2D ConfigureRigidbody2D(GameObject ball)
    {
        Rigidbody2D rb = ball.AddComponent<Rigidbody2D>();
        
        // Configure for arcade physics
        rb.mass = BALL_MASS;
        rb.gravityScale = 0f; // No gravity for Breakout
        rb.linearDamping = 0f; // No linear drag
        rb.angularDamping = 0f; // No angular drag
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Prevent tunneling
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep; // Always active
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Smooth movement
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // No rotation
        
        Debug.Log("⚙️ [Step 2/7] Rigidbody2D configured:");
        Debug.Log($"   • Mass: {rb.mass}");
        Debug.Log($"   • Collision Detection: Continuous (prevents tunneling)");
        Debug.Log($"   • Gravity: Disabled");
        Debug.Log($"   • Rotation: Frozen");
        
        return rb;
    }
    
    /// <summary>
    /// Configures CircleCollider2D component for collision detection.
    /// </summary>
    private static CircleCollider2D ConfigureCircleCollider2D(GameObject ball)
    {
        CircleCollider2D collider = ball.AddComponent<CircleCollider2D>();
        
        collider.radius = BALL_RADIUS;
        collider.isTrigger = false; // Physical collisions
        
        Debug.Log($"🔵 [Step 3/7] CircleCollider2D configured:");
        Debug.Log($"   • Radius: {collider.radius}");
        Debug.Log($"   • Is Trigger: {collider.isTrigger}");
        
        return collider;
    }
    
    /// <summary>
    /// Creates Physics Material 2D asset for arcade bouncing.
    /// </summary>
    private static PhysicsMaterial2D CreatePhysicsMaterial()
    {
        // Ensure Materials directory exists
        string materialsPath = "Assets/Materials";
        if (!Directory.Exists(materialsPath))
        {
            Directory.CreateDirectory(materialsPath);
            AssetDatabase.Refresh();
        }
        
        // Create physics material asset
        PhysicsMaterial2D material = new PhysicsMaterial2D();
        material.name = "BallPhysics";
        material.bounciness = PHYSICS_MATERIAL_BOUNCINESS;
        material.friction = PHYSICS_MATERIAL_FRICTION;
        
        // Save as asset
        string assetPath = $"{materialsPath}/BallPhysics.physicsMaterial2D";
        
        // Check if material already exists
        PhysicsMaterial2D existingMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(assetPath);
        if (existingMaterial != null)
        {
            Debug.Log($"📄 Using existing physics material: {assetPath}");
            return existingMaterial;
        }
        
        AssetDatabase.CreateAsset(material, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"📄 [Step 4/7] Physics Material 2D created:");
        Debug.Log($"   • Path: {assetPath}");
        Debug.Log($"   • Bounciness: {material.bounciness} (perfect bounce)");
        Debug.Log($"   • Friction: {material.friction} (frictionless)");
        
        return material;
    }
    
    /// <summary>
    /// Assigns physics material to collider.
    /// </summary>
    private static void AssignPhysicsMaterial(CircleCollider2D collider, PhysicsMaterial2D material)
    {
        collider.sharedMaterial = material;
        Debug.Log($"✅ Physics material assigned to collider");
    }
    
    /// <summary>
    /// Configures collision layers for Ball GameObject.
    /// </summary>
    private static void ConfigureCollisionLayers(GameObject ball)
    {
        // Check if Ball layer exists
        int ballLayer = LayerMask.NameToLayer(BALL_LAYER_NAME);
        
        if (ballLayer == -1)
        {
            // Create Ball layer if it doesn't exist
            Debug.LogWarning($"⚠️ '{BALL_LAYER_NAME}' layer not found. Please create it in Project Settings > Tags and Layers");
            Debug.Log("   Using Default layer as fallback");
            ball.layer = 0; // Default layer
        }
        else
        {
            ball.layer = ballLayer;
            Debug.Log($"🏷️ [Step 5/7] Collision layer configured: '{BALL_LAYER_NAME}' (Layer {ballLayer})");
        }
    }
    
    /// <summary>
    /// Configures SpriteRenderer component for visual representation.
    /// </summary>
    private static SpriteRenderer ConfigureSpriteRenderer(GameObject ball)
    {
        SpriteRenderer renderer = ball.AddComponent<SpriteRenderer>();
        
        renderer.color = Color.white;
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = 10; // Above game elements
        
        Debug.Log($"🎨 [Step 6/7] SpriteRenderer configured:");
        Debug.Log($"   • Color: White");
        Debug.Log($"   • Sorting Order: {renderer.sortingOrder}");
        
        return renderer;
    }
    
    /// <summary>
    /// Creates a white circle sprite for the ball.
    /// </summary>
    private static Sprite CreateBallSprite()
    {
        // Create a white circle texture
        int textureSize = 64;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        Color[] pixels = new Color[textureSize * textureSize];
        
        Vector2 center = new Vector2(textureSize / 2f, textureSize / 2f);
        float radius = textureSize / 2f - 2; // Slight margin
        
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    // Anti-aliasing on edges
                    float alpha = Mathf.Clamp01((radius - distance) * 2f);
                    pixels[y * textureSize + x] = new Color(1f, 1f, 1f, alpha);
                }
                else
                {
                    pixels[y * textureSize + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Create sprite from texture
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, textureSize, textureSize),
            new Vector2(0.5f, 0.5f),
            textureSize * 2f // Pixels per unit
        );
        
        Debug.Log($"   • Sprite created: {textureSize}x{textureSize} white circle");
        
        return sprite;
    }
    
    /// <summary>
    /// Creates a prefab from the configured Ball GameObject.
    /// </summary>
    private static void CreateBallPrefab(GameObject ball)
    {
        // Ensure Prefabs directory exists
        string prefabsPath = "Assets/Prefabs";
        if (!Directory.Exists(prefabsPath))
        {
            Directory.CreateDirectory(prefabsPath);
            AssetDatabase.Refresh();
        }
        
        string prefabPath = $"{prefabsPath}/Ball.prefab";
        
        // Check if prefab already exists
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            // Update existing prefab
            PrefabUtility.SaveAsPrefabAssetAndConnect(ball, prefabPath, InteractionMode.UserAction);
            Debug.Log($"🎯 [Step 7/7] Ball prefab updated: {prefabPath}");
        }
        else
        {
            // Create new prefab
            PrefabUtility.SaveAsPrefabAssetAndConnect(ball, prefabPath, InteractionMode.UserAction);
            Debug.Log($"🎯 [Step 7/7] Ball prefab created: {prefabPath}");
        }
    }
    
    /// <summary>
    /// Validates that BallData integration is possible.
    /// </summary>
    private static void ValidateBallDataIntegration()
    {
        // Check if BallData class exists by attempting to use its type
        System.Type ballDataType = System.Type.GetType("BallData");
        
        if (ballDataType != null)
        {
            Debug.Log($"✅ BallData integration ready - class found");
        }
        else
        {
            Debug.LogWarning($"⚠️ BallData class not found - ensure Task 1.1.1.1 is complete");
        }
    }
    
    /// <summary>
    /// Logs successful configuration summary.
    /// </summary>
    private static void LogSuccessfulSetup(GameObject ball, Rigidbody2D rb, CircleCollider2D collider, PhysicsMaterial2D material)
    {
        Debug.Log("✅ [Task 1.1.1.2] Ball GameObject created successfully!");
        Debug.Log("📋 Ball Configuration Summary:");
        Debug.Log($"   • GameObject: '{ball.name}' in {GetGameObjectPath(ball)}");
        Debug.Log($"   • Rigidbody2D: Mass={rb.mass}, Continuous Collision, No Gravity");
        Debug.Log($"   • CircleCollider2D: Radius={collider.radius}");
        Debug.Log($"   • Physics Material: Bounciness={material.bounciness}, Friction={material.friction}");
        Debug.Log($"   • Layer: {LayerMask.LayerToName(ball.layer)} (Layer {ball.layer})");
        Debug.Log($"   • Visual: White circle sprite");
        Debug.Log($"   • Prefab: Created at Assets/Prefabs/Ball.prefab");
        Debug.Log("🚀 Ready for next steps:");
        Debug.Log("   → BallController component attachment");
        Debug.Log("   → Physics behavior implementation");
        Debug.Log("   → Launch mechanics integration");
        Debug.Log("   → Collision event handling");
    }
    
    /// <summary>
    /// Gets full hierarchy path of a GameObject.
    /// </summary>
    private static string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }
}
#endif