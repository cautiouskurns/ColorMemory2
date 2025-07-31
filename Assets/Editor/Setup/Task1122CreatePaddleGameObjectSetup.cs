#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor setup script for creating paddle GameObject with proper physics components and collision configuration.
/// Provides automated setup for paddle system with Unity physics integration and proper collision detection.
/// </summary>
public static class Task1122CreatePaddleGameObjectSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Paddle GameObject";
    private const string PADDLE_DATA_FILE = "Assets/Scripts/Paddle/PaddleData.cs";
    private const string MATERIALS_FOLDER = "Assets/Materials";
    private const string PREFABS_FOLDER = "Assets/Prefabs";
    private const string PHYSICS_MATERIAL_PATH = "Assets/Materials/PaddlePhysics.physicsMaterial2D";
    private const string PREFAB_PATH = "Assets/Prefabs/Paddle.prefab";
    
    /// <summary>
    /// Creates paddle GameObject with complete physics and visual configuration.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreatePaddleGameObject()
    {
        Debug.Log("📋 [Task 1.1.2.2] Starting Paddle GameObject creation...");
        
        try
        {
            // Step 1: Validate prerequisites
            ValidatePrerequisites();
            
            // Step 2: Create folder structure
            CreateFolderStructure();
            
            // Step 3: Create physics material
            PhysicsMaterial2D physicsMaterial = CreatePaddlePhysicsMaterial();
            
            // Step 4: Create paddle GameObject
            GameObject paddleGameObject = CreatePaddleGameObjectWithComponents(physicsMaterial);
            
            // Step 5: Configure collision layers
            ConfigureCollisionLayers(paddleGameObject);
            
            // Step 6: Create prefab
            CreatePaddlePrefab(paddleGameObject);
            
            // Step 7: Final validation and cleanup
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            LogSuccessfulSetup(paddleGameObject);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.2.2] Paddle GameObject creation failed: {e.Message}");
            Debug.LogError("📋 Please check Unity project setup and component availability.");
        }
    }
    
    /// <summary>
    /// Menu validation - prevents duplicate creation when Paddle GameObject already exists.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreatePaddleGameObject()
    {
        bool paddleExists = GameObject.Find("Paddle") != null;
        
        if (paddleExists)
        {
            Debug.LogWarning("⚠️ Paddle GameObject already exists in scene.");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Validates that prerequisite systems are in place before creating paddle GameObject.
    /// </summary>
    private static void ValidatePrerequisites()
    {
        Debug.Log("🔍 [Step 1/6] Validating prerequisites...");
        
        // Check if PaddleData exists
        if (!File.Exists(PADDLE_DATA_FILE))
        {
            Debug.LogWarning("⚠️ PaddleData structure missing. Creating prerequisite...");
            Task1121CreatePaddleDataSetup.CreatePaddleDataStructure();
        }
        
        Debug.Log("✅ [Step 1/6] Prerequisites validated successfully");
    }
    
    /// <summary>
    /// Creates necessary folder structure for materials and prefabs.
    /// </summary>
    private static void CreateFolderStructure()
    {
        Debug.Log("📁 [Step 2/6] Creating folder structure...");
        
        // Create Materials folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(MATERIALS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
            Debug.Log("   • Created Assets/Materials folder");
        }
        
        // Create Prefabs folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(PREFABS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
            Debug.Log("   • Created Assets/Prefabs folder");
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("✅ [Step 2/6] Folder structure created successfully");
    }
    
    /// <summary>
    /// Creates custom physics material for arcade-style paddle bouncing behavior.
    /// </summary>
    /// <returns>Created PhysicsMaterial2D asset</returns>
    private static PhysicsMaterial2D CreatePaddlePhysicsMaterial()
    {
        Debug.Log("⚙️ [Step 3/6] Creating paddle physics material...");
        
        // Check if physics material already exists
        PhysicsMaterial2D existingMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(PHYSICS_MATERIAL_PATH);
        if (existingMaterial != null)
        {
            Debug.Log("   • Using existing PaddlePhysics material");
            return existingMaterial;
        }
        
        // Create new physics material
        PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D("PaddlePhysics");
        
        // Configure arcade-style bounce properties
        physicsMaterial.bounciness = 0.8f;    // High bounce for arcade feel
        physicsMaterial.friction = 0.1f;      // Low friction for smooth bounces
        
        // Save physics material as asset
        AssetDatabase.CreateAsset(physicsMaterial, PHYSICS_MATERIAL_PATH);
        AssetDatabase.SaveAssets();
        
        Debug.Log("   • Created PaddlePhysics material (Bounciness: 0.8, Friction: 0.1)");
        Debug.Log("✅ [Step 3/6] Physics material created successfully");
        
        return physicsMaterial;
    }
    
    /// <summary>
    /// Creates paddle GameObject with all required components configured.
    /// </summary>
    /// <param name="physicsMaterial">Physics material for collision behavior</param>
    /// <returns>Configured paddle GameObject</returns>
    private static GameObject CreatePaddleGameObjectWithComponents(PhysicsMaterial2D physicsMaterial)
    {
        Debug.Log("🎮 [Step 4/6] Creating paddle GameObject with components...");
        
        // Create base GameObject
        GameObject paddleGameObject = new GameObject("Paddle");
        
        // Position at bottom of playfield
        paddleGameObject.transform.position = new Vector3(0f, -4.0f, 0f);
        
        // Add BoxCollider2D component
        BoxCollider2D boxCollider = paddleGameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(2.0f, 0.3f);  // GDD-specified dimensions
        boxCollider.sharedMaterial = physicsMaterial;
        
        Debug.Log("   • Added BoxCollider2D (2.0 x 0.3) with physics material");
        
        // Add SpriteRenderer component
        SpriteRenderer spriteRenderer = paddleGameObject.AddComponent<SpriteRenderer>();
        
        // Create simple sprite texture for paddle
        Texture2D paddleTexture = CreatePaddleTexture();
        Sprite paddleSprite = Sprite.Create(paddleTexture, new Rect(0, 0, paddleTexture.width, paddleTexture.height), new Vector2(0.5f, 0.5f), 100f);
        paddleSprite.name = "PaddleSprite";
        
        spriteRenderer.sprite = paddleSprite;
        spriteRenderer.color = new Color(0f, 0.5f, 1f, 1f); // Bright blue #0080FF
        spriteRenderer.sortingOrder = 1;
        
        Debug.Log("   • Added SpriteRenderer with bright blue color (#0080FF)");
        
        // Set up GameArea parent container
        SetupGameAreaContainer(paddleGameObject);
        
        Debug.Log("✅ [Step 4/6] Paddle GameObject created with components");
        
        return paddleGameObject;
    }
    
    /// <summary>
    /// Creates a simple texture for the paddle sprite.
    /// </summary>
    /// <returns>Paddle texture</returns>
    private static Texture2D CreatePaddleTexture()
    {
        int width = 200;  // 2.0 units * 100 pixels per unit
        int height = 30;  // 0.3 units * 100 pixels per unit
        
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        
        // Fill with white pixels (will be tinted by SpriteRenderer)
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.name = "PaddleTexture";
        
        return texture;
    }
    
    /// <summary>
    /// Sets up GameArea container and parents the paddle to it.
    /// </summary>
    /// <param name="paddleGameObject">Paddle GameObject to parent</param>
    private static void SetupGameAreaContainer(GameObject paddleGameObject)
    {
        // Find or create GameArea container
        GameObject gameArea = GameObject.Find("GameArea");
        if (gameArea == null)
        {
            gameArea = new GameObject("GameArea");
            gameArea.transform.position = Vector3.zero;
            Debug.Log("   • Created GameArea container");
        }
        
        // Parent paddle to GameArea
        paddleGameObject.transform.SetParent(gameArea.transform);
        Debug.Log("   • Parented paddle to GameArea container");
    }
    
    /// <summary>
    /// Configures collision layers for proper physics interactions.
    /// </summary>
    /// <param name="paddleGameObject">Paddle GameObject to configure</param>
    private static void ConfigureCollisionLayers(GameObject paddleGameObject)
    {
        Debug.Log("🔧 [Step 5/6] Configuring collision layers...");
        
        // Set paddle to layer 8 (Paddle layer)
        int paddleLayer = LayerMask.NameToLayer("Paddle");
        if (paddleLayer == -1)
        {
            // Create Paddle layer if it doesn't exist
            Debug.LogWarning("   • Paddle layer not found. Please create 'Paddle' layer in Physics2D settings.");
            Debug.LogWarning("   • Using Default layer for now.");
            paddleGameObject.layer = 0; // Default layer
        }
        else
        {
            paddleGameObject.layer = paddleLayer;
            Debug.Log("   • Assigned paddle to 'Paddle' collision layer");
        }
        
        Debug.Log("✅ [Step 5/6] Collision layers configured");
    }
    
    /// <summary>
    /// Creates prefab asset from configured paddle GameObject.
    /// </summary>
    /// <param name="paddleGameObject">Configured paddle GameObject</param>
    private static void CreatePaddlePrefab(GameObject paddleGameObject)
    {
        Debug.Log("💾 [Step 6/6] Creating paddle prefab...");
        
        // Check if prefab already exists
        if (File.Exists(PREFAB_PATH))
        {
            Debug.LogWarning("   • Overwriting existing Paddle prefab");
        }
        
        // Create prefab from GameObject
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(paddleGameObject, PREFAB_PATH);
        
        if (prefab != null)
        {
            Debug.Log($"   • Created prefab at: {PREFAB_PATH}");
            Debug.Log("✅ [Step 6/6] Paddle prefab created successfully");
        }
        else
        {
            Debug.LogError("❌ Failed to create paddle prefab");
        }
    }
    
    /// <summary>
    /// Logs successful paddle GameObject setup summary.
    /// </summary>
    /// <param name="paddleGameObject">Created paddle GameObject</param>
    private static void LogSuccessfulSetup(GameObject paddleGameObject)
    {
        Debug.Log("✅ [Task 1.1.2.2] Paddle GameObject created successfully!");
        Debug.Log("📋 Paddle GameObject Summary:");
        Debug.Log($"   • GameObject Name: {paddleGameObject.name}");
        Debug.Log($"   • Position: {paddleGameObject.transform.position}");
        Debug.Log($"   • Parent: {(paddleGameObject.transform.parent != null ? paddleGameObject.transform.parent.name : "None")}");
        Debug.Log("🔧 Component Configuration:");
        
        // Log BoxCollider2D info
        BoxCollider2D collider = paddleGameObject.GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            Debug.Log($"   → BoxCollider2D: Size {collider.size}, Material: {(collider.sharedMaterial != null ? collider.sharedMaterial.name : "None")}");
        }
        
        // Log SpriteRenderer info
        SpriteRenderer renderer = paddleGameObject.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Debug.Log($"   → SpriteRenderer: Color {renderer.color}, Sorting Order {renderer.sortingOrder}");
        }
        
        Debug.Log("📊 Physics Configuration:");
        Debug.Log("   • Physics Material: PaddlePhysics (Bounciness: 0.8, Friction: 0.1)");
        Debug.Log($"   • Collision Layer: {LayerMask.LayerToName(paddleGameObject.layer)}");
        Debug.Log("📁 Assets Created:");
        Debug.Log($"   • Physics Material: {PHYSICS_MATERIAL_PATH}");
        Debug.Log($"   • Prefab: {PREFAB_PATH}");
        Debug.Log("🎮 Usage Instructions:");
        Debug.Log("   1. Paddle GameObject is positioned at bottom of playfield (Y = -4.0)");
        Debug.Log("   2. BoxCollider2D configured for ball collision detection");
        Debug.Log("   3. Physics material provides arcade-style bounce behavior");
        Debug.Log("   4. Use prefab for instantiating paddles in other scenes");
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Implement PaddleController MonoBehaviour for movement");
        Debug.Log("   → Add input handling system for player control");
        Debug.Log("   → Integrate with ball physics for collision response");
        Debug.Log("   → Configure collision layers in Physics2D settings if needed");
    }
}
#endif