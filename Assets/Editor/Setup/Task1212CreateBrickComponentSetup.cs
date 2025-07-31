#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for creating and configuring Brick components for testing and validation.
/// Provides automated test brick GameObject creation with proper component configuration.
/// </summary>
public static class Task1212CreateBrickComponentSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1212 Create Brick Component";
    
    /// <summary>
    /// Creates test brick GameObject with Brick component for validation and testing.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreateBrickComponent()
    {
        Debug.Log("🧱 [Task 1.2.1.2] Starting Brick Component creation...");
        
        try
        {
            // Step 1: Validate dependencies
            ValidateDependencies();
            
            // Step 2: Create test brick GameObjects
            CreateTestBrickGameObjects();
            
            // Step 3: Validate component setup
            ValidateComponentSetup();
            
            // Step 4: Save assets and log success
            AssetDatabase.SaveAssets();
            LogSuccessfulSetup();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.2.1.2] Brick Component creation failed: {e.Message}");
            Debug.LogError("📋 Please ensure BrickData structures are available from Task 1.2.1.1");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if we have necessary dependencies
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreateBrickComponent()
    {
        // Always show menu - validation will happen during execution
        return true;
    }
    
    /// <summary>
    /// Validates that all dependencies are available
    /// </summary>
    private static void ValidateDependencies()
    {
        Debug.Log("🔍 [Step 1/4] Validating dependencies...");
        
        // Basic validation - try to create instances
        try
        {
            // Test BrickData creation
            BrickData testData = new BrickData(BrickType.Normal);
            Debug.Log("   • BrickData class: Available and functional ✅");
            
            // Test all BrickType enum values
            foreach (BrickType enumValue in System.Enum.GetValues(typeof(BrickType)))
            {
                BrickData typeTest = new BrickData(enumValue);
                Debug.Log($"   • BrickType.{enumValue}: Functional ✅");
            }
            
            // Check if Brick script is available
            System.Type brickComponentType = typeof(Brick);
            if (brickComponentType != null)
            {
                Debug.Log("   • Brick component type: Available ✅");
            }
            else
            {
                throw new System.Exception("Brick component type not found");
            }
        }
        catch (System.Exception e)
        {
            throw new System.Exception($"Dependency validation failed: {e.Message}. Please ensure both Task 1.2.1.1 and Task 1.2.1.2 scripts are compiled without errors.");
        }
        
        Debug.Log("✅ [Step 1/4] All dependencies validated successfully");
    }
    
    /// <summary>
    /// Creates test brick GameObjects with different configurations
    /// </summary>
    private static void CreateTestBrickGameObjects()
    {
        Debug.Log("🎮 [Step 2/4] Creating test brick GameObjects...");
        
        // Create parent object for organization
        GameObject brickParent = CreateBrickParent();
        
        // Create test bricks for each type
        CreateTestBrick(brickParent, BrickType.Normal, new Vector3(-2f, 0f, 0f));
        CreateTestBrick(brickParent, BrickType.Reinforced, new Vector3(-0.5f, 0f, 0f));
        CreateTestBrick(brickParent, BrickType.Indestructible, new Vector3(1f, 0f, 0f));
        CreateTestBrick(brickParent, BrickType.PowerUp, new Vector3(2.5f, 0f, 0f));
        
        // Select parent for easy inspection
        Selection.activeGameObject = brickParent;
        
        Debug.Log("✅ [Step 2/4] Test brick GameObjects created successfully");
    }
    
    /// <summary>
    /// Creates parent GameObject for organizing test bricks
    /// </summary>
    /// <returns>Parent GameObject</returns>
    private static GameObject CreateBrickParent()
    {
        GameObject parent = GameObject.Find("Test_BrickComponents");
        if (parent == null)
        {
            parent = new GameObject("Test_BrickComponents");
            parent.transform.position = Vector3.zero;
            Debug.Log("   • Created parent GameObject: Test_BrickComponents");
        }
        else
        {
            Debug.Log("   • Using existing parent GameObject: Test_BrickComponents");
        }
        
        return parent;
    }
    
    /// <summary>
    /// Creates individual test brick GameObject with specified configuration
    /// </summary>
    /// <param name="parent">Parent GameObject</param>
    /// <param name="brickType">Type of brick to create</param>
    /// <param name="position">World position for the brick</param>
    private static void CreateTestBrick(GameObject parent, BrickType brickType, Vector3 position)
    {
        // Create GameObject
        string brickName = $"TestBrick_{brickType}";
        GameObject brickObject = new GameObject(brickName);
        brickObject.transform.SetParent(parent.transform);
        brickObject.transform.position = position;
        
        // Add Brick component with error handling
        Brick brickComponent = null;
        try
        {
            // First check if we can get the component type
            System.Type brickComponentType = typeof(Brick);
            if (brickComponentType == null)
            {
                Debug.LogError($"Brick type not found when creating {brickName}");
                return;
            }
            
            // Try to add the component
            brickComponent = brickObject.AddComponent(brickComponentType) as Brick;
            if (brickComponent == null)
            {
                Debug.LogError($"Failed to add Brick component to {brickName} - AddComponent returned null. Check for compilation errors in Brick.cs");
                return;
            }
            Debug.Log($"   • Successfully added Brick component to {brickName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exception adding Brick component to {brickName}: {e.Message}");
            Debug.LogError($"This usually indicates compilation errors in the Brick.cs script. Check the Console for red compilation errors.");
            return;
        }
        
        // Add required Collider2D (BoxCollider2D for testing)
        BoxCollider2D collider = brickObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1f, 0.5f);
        collider.isTrigger = false; // Set to false for physics collisions
        
        // Add SpriteRenderer for visual representation
        SpriteRenderer spriteRenderer = brickObject.AddComponent<SpriteRenderer>();
        
        // Create simple square sprite for testing
        Texture2D texture = CreateTestTexture();
        Sprite testSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        testSprite.name = $"TestSprite_{brickType}";
        spriteRenderer.sprite = testSprite;
        
        // Initialize brick with appropriate data
        BrickData brickData = CreateBrickDataForType(brickType);
        if (brickData != null && brickComponent != null)
        {
            brickComponent.Initialize(brickData);
        }
        else
        {
            Debug.LogError($"Failed to initialize {brickName}: BrickData={brickData != null}, Component={brickComponent != null}");
        }
        
        // Enable debug logging for test bricks
        SerializedObject serializedBrick = new SerializedObject(brickComponent);
        SerializedProperty debugLogging = serializedBrick.FindProperty("enableDebugLogging");
        if (debugLogging != null)
        {
            debugLogging.boolValue = true;
            serializedBrick.ApplyModifiedProperties();
        }
        
        // Mark objects as dirty for saving
        EditorUtility.SetDirty(brickObject);
        
        Debug.Log($"   • Created test brick: {brickName} at {position}");
    }
    
    /// <summary>
    /// Creates test texture for sprite visualization
    /// </summary>
    /// <returns>Simple white texture</returns>
    private static Texture2D CreateTestTexture()
    {
        Texture2D texture = new Texture2D(64, 32, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[texture.width * texture.height];
        
        // Fill with white color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.name = "TestBrickTexture";
        
        return texture;
    }
    
    /// <summary>
    /// Creates BrickData instance for specified brick type
    /// </summary>
    /// <param name="brickType">Type of brick data to create</param>
    /// <returns>Configured BrickData instance</returns>
    private static BrickData CreateBrickDataForType(BrickType brickType)
    {
        try
        {
            // Use constructor instead of static factory methods to avoid potential issues
            BrickData brickData = new BrickData(brickType);
            if (brickData != null)
            {
                return brickData;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to create BrickData for type {brickType}: {e.Message}");
        }
        
        // Fallback: create with default constructor
        Debug.LogWarning($"Using fallback Normal brick configuration for type: {brickType}");
        return new BrickData(BrickType.Normal);
    }
    
    /// <summary>
    /// Validates that component setup is working correctly
    /// </summary>
    private static void ValidateComponentSetup()
    {
        Debug.Log("🧪 [Step 3/4] Validating component setup...");
        
        // Find all test bricks
        Brick[] testBricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        int validationScore = 0;
        int totalBricks = 0;
        
        foreach (Brick brick in testBricks)
        {
            if (brick.gameObject.name.StartsWith("TestBrick_"))
            {
                totalBricks++;
                
                // Validate initialization
                if (brick.IsInitialized)
                {
                    validationScore++;
                    Debug.Log($"   • {brick.gameObject.name}: Initialized ✅");
                }
                else
                {
                    Debug.LogWarning($"   • {brick.gameObject.name}: Not initialized ⚠️");
                }
                
                // Validate state
                if (brick.ValidateState())
                {
                    Debug.Log($"   • {brick.gameObject.name}: State valid ✅");
                }
                else
                {
                    Debug.LogWarning($"   • {brick.gameObject.name}: State invalid ⚠️");
                }
                
                // Log debug info
                Debug.Log($"   • {brick.gameObject.name}: {brick.GetDebugInfo()}");
            }
        }
        
        Debug.Log($"   • Validation Score: {validationScore}/{totalBricks} bricks properly initialized");
        
        if (validationScore == totalBricks && totalBricks > 0)
        {
            Debug.Log("   ✅ All test bricks validated successfully");
        }
        else if (totalBricks == 0)
        {
            Debug.LogWarning("   ⚠️ No test bricks found for validation");
        }
        else
        {
            Debug.LogWarning($"   ⚠️ Only {validationScore}/{totalBricks} bricks validated successfully");
        }
        
        Debug.Log("✅ [Step 3/4] Component validation complete");
    }
    
    /// <summary>
    /// Logs successful setup summary
    /// </summary>
    private static void LogSuccessfulSetup()
    {
        Debug.Log("✅ [Task 1.2.1.2] Brick Component creation completed successfully!");
        Debug.Log("📋 Brick Component Summary:");
        Debug.Log("   • Core MonoBehaviour: Implemented with Unity lifecycle and state management");
        Debug.Log("   • BrickData Integration: Full configuration-driven initialization system");
        Debug.Log("   • Inspector Organization: Clear sections with Headers and Tooltips");
        Debug.Log("   • Framework Methods: Prepared stubs for collision and destruction logic");
        
        Debug.Log("🧱 Test Brick GameObjects Created:");
        Debug.Log("   → TestBrick_Normal: Standard 1HP red brick for basic gameplay testing");
        Debug.Log("   → TestBrick_Reinforced: Durable 2HP blue brick for challenge testing");
        Debug.Log("   → TestBrick_Indestructible: Permanent gray brick for obstacle testing");
        Debug.Log("   → TestBrick_PowerUp: Special yellow brick for power-up testing");
        
        Debug.Log("🔧 Component Features:");
        Debug.Log("   • State Management: Hit points, destruction status, initialization tracking");
        Debug.Log("   • Visual Configuration: Automatic color application from BrickData");
        Debug.Log("   • Debug Support: Comprehensive logging and validation methods");
        Debug.Log("   • Editor Integration: Context menus and Inspector validation");
        Debug.Log("   • Performance Optimization: Component reference caching and efficient updates");
        
        Debug.Log("🎯 Framework Methods Prepared:");
        Debug.Log("   • OnCollisionDetected(): Ready for collision detection implementation");
        Debug.Log("   • DestroyBrick(): Ready for destruction effects and scoring");
        Debug.Log("   • ResetBrick(): Ready for level restart and brick recycling");
        Debug.Log("   • UpdateConfiguration(): Ready for runtime brick modification");
        
        Debug.Log("📊 Integration Points:");
        Debug.Log("   • CollisionManager: Framework ready for collision event handling");
        Debug.Log("   • Scoring System: Score values and awarding flags available");
        Debug.Log("   • Visual Effects: Destruction effect flags and color configuration");
        Debug.Log("   • Level Generation: Initialization and configuration systems ready");
        
        Debug.Log("💡 Testing Instructions:");
        Debug.Log("   1. Select 'Test_BrickComponents' in Hierarchy to view all test bricks");
        Debug.Log("   2. Use Context Menu commands on Brick components for testing");
        Debug.Log("   3. Modify BrickData in Inspector to see real-time configuration changes");
        Debug.Log("   4. Enable 'Debug Logging' on individual bricks for detailed console output");
        Debug.Log("   5. Use ValidateState() context menu to check brick integrity");
        
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Implement collision detection logic in OnCollisionDetected() method");
        Debug.Log("   → Add destruction effects and scoring in DestroyBrick() method");
        Debug.Log("   → Create brick prefab variants using different BrickData configurations");
        Debug.Log("   → Integrate with level generation system for dynamic brick placement");
        
        Debug.Log("⚠️ Architecture Notes:");
        Debug.Log("   → Brick component requires Collider2D for collision detection");
        Debug.Log("   → Visual components (SpriteRenderer/Renderer) recommended for visibility");
        Debug.Log("   → BrickData configuration should be set before or during Initialize() call");
        Debug.Log("   → Framework methods are prepared stubs - expand in future collision tasks");
    }
    
    /// <summary>
    /// Cleans up test bricks for fresh testing
    /// </summary>
    [MenuItem("Breakout/Setup/Clean Test Bricks", false, 1001)]
    public static void CleanupTestBricks()
    {
        GameObject testParent = GameObject.Find("Test_BrickComponents");
        if (testParent != null)
        {
            GameObject.DestroyImmediate(testParent);
            Debug.Log("🧹 [Cleanup] Test brick GameObjects removed");
        }
        else
        {
            Debug.Log("🧹 [Cleanup] No test brick GameObjects found to remove");
        }
    }
}
#endif