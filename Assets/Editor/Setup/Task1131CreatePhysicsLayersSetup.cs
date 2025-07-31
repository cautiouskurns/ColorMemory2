#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for configuring physics layers and collision matrix for Breakout arcade game.
/// Creates 5 named physics layers and sets up proper collision interactions between game objects.
/// </summary>
public static class Task1131CreatePhysicsLayersSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1131 Create Physics Layers";
    
    // Layer names for the Breakout game
    private const string BALL_LAYER = "Ball";
    private const string PADDLE_LAYER = "Paddle";
    private const string BRICKS_LAYER = "Bricks";
    private const string POWERUPS_LAYER = "PowerUps";
    private const string BOUNDARIES_LAYER = "Boundaries";
    
    // Layer indices (will be determined at runtime)
    private static int ballLayerIndex = -1;
    private static int paddleLayerIndex = -1;
    private static int bricksLayerIndex = -1;
    private static int powerUpsLayerIndex = -1;
    private static int boundariesLayerIndex = -1;
    
    /// <summary>
    /// Creates physics layers and configures collision matrix for Breakout game.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreatePhysicsLayers()
    {
        Debug.Log("📋 [Task 1.1.3.1] Starting Physics Layer Configuration...");
        
        try
        {
            // Step 1: Create named physics layers
            CreateNamedLayers();
            
            // Step 2: Configure collision matrix
            ConfigureCollisionMatrix();
            
            // Step 3: Apply layers to existing GameObjects
            ApplyLayersToGameObjects();
            
            // Step 4: Validate configuration
            ValidateConfiguration();
            
            // Step 5: Save project settings
            AssetDatabase.SaveAssets();
            
            LogSuccessfulSetup();
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.3.1] Physics Layer Configuration failed: {e.Message}");
            Debug.LogError("📋 Please check Unity project settings and existing GameObjects.");
        }
    }
    
    /// <summary>
    /// Menu validation - allows reconfiguration.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreatePhysicsLayers()
    {
        return true; // Allow reconfiguration
    }
    
    /// <summary>
    /// Creates the 5 named physics layers using TagManager.asset manipulation.
    /// </summary>
    private static void CreateNamedLayers()
    {
        Debug.Log("🔍 [Step 1/4] Creating named physics layers...");
        
        // Get the TagManager asset
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        
        if (layersProp == null)
        {
            Debug.LogError("❌ Could not find layers property in TagManager");
            throw new System.NullReferenceException("TagManager layers property not found");
        }
        
        // Create layer mapping
        string[] layerNames = { BALL_LAYER, PADDLE_LAYER, BRICKS_LAYER, POWERUPS_LAYER, BOUNDARIES_LAYER };
        
        foreach (string layerName in layerNames)
        {
            int layerIndex = CreateOrFindLayer(layersProp, layerName);
            
            // Store layer indices for later use
            switch (layerName)
            {
                case BALL_LAYER:
                    ballLayerIndex = layerIndex;
                    break;
                case PADDLE_LAYER:
                    paddleLayerIndex = layerIndex;
                    break;
                case BRICKS_LAYER:
                    bricksLayerIndex = layerIndex;
                    break;
                case POWERUPS_LAYER:
                    powerUpsLayerIndex = layerIndex;
                    break;
                case BOUNDARIES_LAYER:
                    boundariesLayerIndex = layerIndex;
                    break;
            }
            
            Debug.Log($"   • {layerName} layer: Index {layerIndex}");
        }
        
        // Apply changes to TagManager
        tagManager.ApplyModifiedProperties();
        
        Debug.Log("✅ [Step 1/4] Named physics layers created successfully");
    }
    
    /// <summary>
    /// Creates or finds a physics layer by name.
    /// </summary>
    /// <param name="layersProp">SerializedProperty for layers array</param>
    /// <param name="layerName">Name of the layer to create</param>
    /// <returns>Index of the created or existing layer</returns>
    private static int CreateOrFindLayer(SerializedProperty layersProp, string layerName)
    {
        // First check if layer already exists
        for (int i = 8; i < 32; i++) // User layers start at index 8
        {
            SerializedProperty layerElement = layersProp.GetArrayElementAtIndex(i);
            if (layerElement.stringValue == layerName)
            {
                Debug.Log($"   • Found existing layer '{layerName}' at index {i}");
                return i;
            }
        }
        
        // Find first empty slot
        for (int i = 8; i < 32; i++) // User layers start at index 8
        {
            SerializedProperty layerElement = layersProp.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(layerElement.stringValue))
            {
                layerElement.stringValue = layerName;
                Debug.Log($"   • Created new layer '{layerName}' at index {i}");
                return i;
            }
        }
        
        Debug.LogError($"❌ No available layer slots for '{layerName}'");
        throw new System.InvalidOperationException($"No available layer slots for '{layerName}'");
    }
    
    /// <summary>
    /// Configures the collision matrix to allow only intended interactions.
    /// </summary>
    private static void ConfigureCollisionMatrix()
    {
        Debug.Log("⚙️ [Step 2/4] Configuring collision matrix...");
        
        // Validate layer indices
        if (ballLayerIndex == -1 || paddleLayerIndex == -1 || bricksLayerIndex == -1 || 
            powerUpsLayerIndex == -1 || boundariesLayerIndex == -1)
        {
            Debug.LogError("❌ Layer indices not properly set. Cannot configure collision matrix.");
            throw new System.InvalidOperationException("Layer indices not initialized");
        }
        
        // First, disable all interactions between our custom layers
        DisableAllCustomLayerInteractions();
        
        // Configure specific interactions according to specification:
        
        // Ball interacts with: Paddle, Bricks, Boundaries
        Physics2D.IgnoreLayerCollision(ballLayerIndex, paddleLayerIndex, false);
        Physics2D.IgnoreLayerCollision(ballLayerIndex, bricksLayerIndex, false);
        Physics2D.IgnoreLayerCollision(ballLayerIndex, boundariesLayerIndex, false);
        Debug.Log("   • Ball interactions: Paddle ✓, Bricks ✓, Boundaries ✓");
        
        // Paddle interacts with: Ball, PowerUps, Boundaries
        Physics2D.IgnoreLayerCollision(paddleLayerIndex, ballLayerIndex, false); // Already set above
        Physics2D.IgnoreLayerCollision(paddleLayerIndex, powerUpsLayerIndex, false);
        Physics2D.IgnoreLayerCollision(paddleLayerIndex, boundariesLayerIndex, false);
        Debug.Log("   • Paddle interactions: Ball ✓, PowerUps ✓, Boundaries ✓");
        
        // Bricks interact with: Ball only (already set above)
        Debug.Log("   • Bricks interactions: Ball ✓");
        
        // PowerUps interact with: Paddle, Boundaries (already set above)
        Physics2D.IgnoreLayerCollision(powerUpsLayerIndex, boundariesLayerIndex, false);
        Debug.Log("   • PowerUps interactions: Paddle ✓, Boundaries ✓");
        
        // Boundaries interact with: Ball, Paddle, PowerUps (already set above)
        Debug.Log("   • Boundaries interactions: Ball ✓, Paddle ✓, PowerUps ✓");
        
        Debug.Log("✅ [Step 2/4] Collision matrix configured successfully");
    }
    
    /// <summary>
    /// Disables all interactions between custom layers initially.
    /// </summary>
    private static void DisableAllCustomLayerInteractions()
    {
        int[] customLayers = { ballLayerIndex, paddleLayerIndex, bricksLayerIndex, powerUpsLayerIndex, boundariesLayerIndex };
        
        for (int i = 0; i < customLayers.Length; i++)
        {
            for (int j = i; j < customLayers.Length; j++)
            {
                if (i != j) // Don't ignore self-collision
                {
                    Physics2D.IgnoreLayerCollision(customLayers[i], customLayers[j], true);
                }
            }
        }
    }
    
    /// <summary>
    /// Applies appropriate layers to existing GameObjects in the scene.
    /// </summary>
    private static void ApplyLayersToGameObjects()
    {
        Debug.Log("🎯 [Step 3/4] Applying layers to GameObjects...");
        
        // Find and assign Ball GameObject
        GameObject ball = GameObject.Find("Ball");
        if (ball != null)
        {
            ball.layer = ballLayerIndex;
            Debug.Log($"   • Ball GameObject assigned to '{BALL_LAYER}' layer (index {ballLayerIndex})");
        }
        else
        {
            Debug.LogWarning("   ⚠️ Ball GameObject not found in scene. Please create Ball GameObject and manually assign to 'Ball' layer.");
        }
        
        // Find and assign Paddle GameObject
        GameObject paddle = GameObject.Find("Paddle");
        if (paddle != null)
        {
            paddle.layer = paddleLayerIndex;
            Debug.Log($"   • Paddle GameObject assigned to '{PADDLE_LAYER}' layer (index {paddleLayerIndex})");
        }
        else
        {
            Debug.LogWarning("   ⚠️ Paddle GameObject not found in scene. Please create Paddle GameObject and manually assign to 'Paddle' layer.");
        }
        
        // Look for GameArea/Boundaries
        GameObject gameArea = GameObject.Find("GameArea");
        if (gameArea != null)
        {
            // Check for boundary child objects
            Transform[] children = gameArea.GetComponentsInChildren<Transform>();
            int boundaryCount = 0;
            
            foreach (Transform child in children)
            {
                if (child != gameArea.transform && 
                    (child.name.ToLower().Contains("wall") || child.name.ToLower().Contains("boundary")))
                {
                    child.gameObject.layer = boundariesLayerIndex;
                    boundaryCount++;
                    Debug.Log($"   • {child.name} assigned to '{BOUNDARIES_LAYER}' layer");
                }
            }
            
            if (boundaryCount == 0)
            {
                Debug.LogWarning("   ⚠️ No boundary objects found in GameArea. Create wall/boundary GameObjects and manually assign to 'Boundaries' layer.");
            }
        }
        else
        {
            Debug.LogWarning("   ⚠️ GameArea container not found. Create boundary GameObjects and manually assign to 'Boundaries' layer.");
        }
        
        Debug.Log("✅ [Step 3/4] GameObject layer assignment complete");
    }
    
    /// <summary>
    /// Validates the physics layer configuration.
    /// </summary>
    private static void ValidateConfiguration()
    {
        Debug.Log("🧪 [Step 4/4] Validating physics layer configuration...");
        
        try
        {
            // Validate layer names
            if (LayerMask.LayerToName(ballLayerIndex) != BALL_LAYER)
            {
                Debug.LogError($"❌ Ball layer validation failed: Expected '{BALL_LAYER}', got '{LayerMask.LayerToName(ballLayerIndex)}'");
                return;
            }
            
            if (LayerMask.LayerToName(paddleLayerIndex) != PADDLE_LAYER)
            {
                Debug.LogError($"❌ Paddle layer validation failed: Expected '{PADDLE_LAYER}', got '{LayerMask.LayerToName(paddleLayerIndex)}'");
                return;
            }
            
            // Validate collision matrix settings
            bool ballPaddleCollision = !Physics2D.GetIgnoreLayerCollision(ballLayerIndex, paddleLayerIndex);
            bool ballBricksCollision = !Physics2D.GetIgnoreLayerCollision(ballLayerIndex, bricksLayerIndex);
            bool ballPowerUpsCollision = Physics2D.GetIgnoreLayerCollision(ballLayerIndex, powerUpsLayerIndex);
            
            Debug.Log("📊 Collision Matrix Validation:");
            Debug.Log($"   • Ball-Paddle collision: {(ballPaddleCollision ? "ENABLED ✓" : "DISABLED ❌")}");
            Debug.Log($"   • Ball-Bricks collision: {(ballBricksCollision ? "ENABLED ✓" : "DISABLED ❌")}");
            Debug.Log($"   • Ball-PowerUps collision: {(ballPowerUpsCollision ? "DISABLED ✓" : "ENABLED ❌")} (should be disabled)");
            
            if (ballPaddleCollision && ballBricksCollision && ballPowerUpsCollision)
            {
                Debug.Log("✅ [Step 4/4] Physics layer configuration validation passed");
            }
            else
            {
                Debug.LogWarning("⚠️ [Step 4/4] Some collision matrix settings may not be correct");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Validation failed: {e.Message}");
        }
    }
    
    /// <summary>
    /// Logs successful physics layer setup summary.
    /// </summary>
    private static void LogSuccessfulSetup()
    {
        Debug.Log("✅ [Task 1.1.3.1] Physics Layer Configuration completed successfully!");
        Debug.Log("📋 Physics Layer Configuration Summary:");
        
        // Log layer assignments
        Debug.Log("🏷️ Created Layers:");
        Debug.Log($"   • {BALL_LAYER}: Index {ballLayerIndex}");
        Debug.Log($"   • {PADDLE_LAYER}: Index {paddleLayerIndex}");
        Debug.Log($"   • {BRICKS_LAYER}: Index {bricksLayerIndex}");
        Debug.Log($"   • {POWERUPS_LAYER}: Index {powerUpsLayerIndex}");
        Debug.Log($"   • {BOUNDARIES_LAYER}: Index {boundariesLayerIndex}");
        
        // Log collision matrix
        Debug.Log("⚙️ Collision Matrix Configuration:");
        Debug.Log("   • Ball interacts with: Paddle, Bricks, Boundaries");
        Debug.Log("   • Paddle interacts with: Ball, PowerUps, Boundaries");
        Debug.Log("   • Bricks interact with: Ball only");
        Debug.Log("   • PowerUps interact with: Paddle, Boundaries only");
        Debug.Log("   • Boundaries interact with: Ball, Paddle, PowerUps");
        
        // Log GameObject assignments
        Debug.Log("🎯 GameObject Layer Assignments:");
        GameObject ball = GameObject.Find("Ball");
        GameObject paddle = GameObject.Find("Paddle");
        
        if (ball != null)
            Debug.Log($"   • Ball GameObject: Layer {ball.layer} ({LayerMask.LayerToName(ball.layer)})");
        if (paddle != null)
            Debug.Log($"   • Paddle GameObject: Layer {paddle.layer} ({LayerMask.LayerToName(paddle.layer)})");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Physics layers are now configured for proper collision isolation");
        Debug.Log("   2. Assign Bricks GameObjects to 'Bricks' layer when created");
        Debug.Log("   3. Assign PowerUp GameObjects to 'PowerUps' layer when created");
        Debug.Log("   4. Assign boundary walls to 'Boundaries' layer");
        Debug.Log("   5. Use these layer indices in collision detection code:");
        Debug.Log($"      - Ball Layer: {ballLayerIndex}");
        Debug.Log($"      - Paddle Layer: {paddleLayerIndex}");
        Debug.Log($"      - Bricks Layer: {bricksLayerIndex}");
        Debug.Log($"      - PowerUps Layer: {powerUpsLayerIndex}");
        Debug.Log($"      - Boundaries Layer: {boundariesLayerIndex}");
        
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Implement CollisionManager for event-based collision handling");
        Debug.Log("   → Create Bricks GameObjects and assign to 'Bricks' layer");
        Debug.Log("   → Create PowerUp GameObjects and assign to 'PowerUps' layer");
        Debug.Log("   → Verify collision interactions in Play mode");
    }
}
#endif