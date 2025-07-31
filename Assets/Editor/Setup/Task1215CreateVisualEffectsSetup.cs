#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for configuring visual effects on Brick components.
/// Provides automated configuration of ParticleSystem components and destruction effects.
/// </summary>
public static class Task1215CreateVisualEffectsSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1215 Configure Brick Visual Effects";
    
    /// <summary>
    /// Configures visual effects on existing Brick components in the scene.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void ConfigureVisualEffects()
    {
        Debug.Log("✨ [Task 1.2.1.5] Starting Visual Effects configuration...");
        
        try
        {
            // Step 1: Validate dependencies
            ValidateDependencies();
            
            // Step 2: Find and configure existing brick components
            ConfigureBrickVisualEffects();
            
            // Step 3: Create particle system prefabs if needed
            CreateParticleSystemPrefabs();
            
            // Step 4: Test visual effects setup
            TestVisualEffectsSetup();
            
            // Step 5: Save assets and log success
            AssetDatabase.SaveAssets();
            LogSuccessfulSetup();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.2.1.5] Visual Effects configuration failed: {e.Message}");
            Debug.LogError("📋 Please ensure Brick MonoBehaviour and ParticleSystem are available");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if we can find Brick components
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateConfigureVisualEffects()
    {
        return true; // Always show - validation happens during execution
    }
    
    /// <summary>
    /// Validates that all dependencies are available for visual effects setup
    /// </summary>
    private static void ValidateDependencies()
    {
        Debug.Log("🔍 [Step 1/5] Validating dependencies...");
        
        // Check if Brick components exist in scene
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        if (bricks.Length == 0)
        {
            throw new System.Exception("No Brick components found in scene. Please create brick GameObjects first.");
        }
        
        Debug.Log($"   • Found {bricks.Length} Brick components in scene");
        
        // Check if ParticleSystem is available
        try
        {
            GameObject testObject = new GameObject("ParticleSystemTest");
            ParticleSystem testPS = testObject.AddComponent<ParticleSystem>();
            if (testPS != null)
            {
                Debug.Log("   • ParticleSystem component available");
                GameObject.DestroyImmediate(testObject);
            }
        }
        catch (System.Exception e)
        {
            throw new System.Exception($"ParticleSystem component not available: {e.Message}");
        }
        
        Debug.Log("✅ [Step 1/5] Dependencies validated successfully");
    }
    
    /// <summary>
    /// Configures visual effects on all Brick components in the scene
    /// </summary>
    private static void ConfigureBrickVisualEffects()
    {
        Debug.Log("🎨 [Step 2/5] Configuring brick visual effects...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int configuredCount = 0;
        int particleSystemsAdded = 0;
        
        foreach (Brick brick in bricks)
        {
            try
            {
                ConfigureIndividualBrickEffects(brick, ref particleSystemsAdded);
                configuredCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to configure visual effects on {brick.gameObject.name}: {e.Message}");
            }
        }
        
        Debug.Log($"   • Configured visual effects on {configuredCount}/{bricks.Length} bricks");
        Debug.Log($"   • Added {particleSystemsAdded} ParticleSystem components");
        Debug.Log("✅ [Step 2/5] Brick visual effects configuration complete");
    }
    
    /// <summary>
    /// Configures visual effects on an individual brick component
    /// </summary>
    /// <param name="brick">Brick component to configure</param>
    /// <param name="particleSystemsAdded">Counter for added particle systems</param>
    private static void ConfigureIndividualBrickEffects(Brick brick, ref int particleSystemsAdded)
    {
        SerializedObject serializedBrick = new SerializedObject(brick);
        
        // Enable visual effects
        SerializedProperty enableVisualEffects = serializedBrick.FindProperty("enableVisualEffects");
        if (enableVisualEffects != null)
        {
            enableVisualEffects.boolValue = true;
            Debug.Log($"   • {brick.gameObject.name}: Enabled visual effects");
        }
        
        // Configure particle count
        SerializedProperty particleCount = serializedBrick.FindProperty("particleCount");
        if (particleCount != null)
        {
            particleCount.intValue = 15; // Default count for good visual impact
            Debug.Log($"   • {brick.gameObject.name}: Set particle count to {particleCount.intValue}");
        }
        
        // Configure particle lifetime
        SerializedProperty particleLifetime = serializedBrick.FindProperty("particleLifetime");
        if (particleLifetime != null)
        {
            particleLifetime.floatValue = 1.0f; // 1 second lifetime
            Debug.Log($"   • {brick.gameObject.name}: Set particle lifetime to {particleLifetime.floatValue}s");
        }
        
        // Configure particle speed
        SerializedProperty particleSpeed = serializedBrick.FindProperty("particleSpeed");
        if (particleSpeed != null)
        {
            particleSpeed.floatValue = 8.0f; // Good explosion speed
            Debug.Log($"   • {brick.gameObject.name}: Set particle speed to {particleSpeed.floatValue}");
        }
        
        // Configure particle size
        SerializedProperty particleSize = serializedBrick.FindProperty("particleSize");
        if (particleSize != null)
        {
            particleSize.floatValue = 0.1f; // Good visible size
            Debug.Log($"   • {brick.gameObject.name}: Set particle size to {particleSize.floatValue}");
        }
        
        // Enable effects logging for testing
        SerializedProperty enableEffectsLogging = serializedBrick.FindProperty("enableEffectsLogging");
        if (enableEffectsLogging != null)
        {
            enableEffectsLogging.boolValue = true;
            Debug.Log($"   • {brick.gameObject.name}: Enabled effects debug logging");
        }
        
        // Ensure ParticleSystem component exists
        ParticleSystem particleSystem = brick.GetComponentInChildren<ParticleSystem>();
        if (particleSystem == null)
        {
            // Create child GameObject for ParticleSystem
            GameObject particleChild = new GameObject("DestructionParticles");
            particleChild.transform.SetParent(brick.transform);
            particleChild.transform.localPosition = Vector3.zero;
            
            // Add and configure ParticleSystem
            particleSystem = particleChild.AddComponent<ParticleSystem>();
            ConfigureParticleSystemComponent(particleSystem, brick);
            
            particleSystemsAdded++;
            Debug.Log($"   • {brick.gameObject.name}: Added ParticleSystem child component");
        }
        else
        {
            // Configure existing particle system
            ConfigureParticleSystemComponent(particleSystem, brick);
            Debug.Log($"   • {brick.gameObject.name}: Configured existing ParticleSystem");
        }
        
        // Update serialized property reference to the ParticleSystem
        SerializedProperty destructionParticles = serializedBrick.FindProperty("destructionParticles");
        if (destructionParticles != null)
        {
            destructionParticles.objectReferenceValue = particleSystem;
            Debug.Log($"   • {brick.gameObject.name}: Linked ParticleSystem reference");
        }
        
        // Apply changes
        serializedBrick.ApplyModifiedProperties();
        EditorUtility.SetDirty(brick);
    }
    
    /// <summary>
    /// Configures ParticleSystem component with optimal settings for brick destruction
    /// </summary>
    /// <param name="particleSystem">ParticleSystem to configure</param>
    /// <param name="brick">Parent brick component for color matching</param>
    private static void ConfigureParticleSystemComponent(ParticleSystem particleSystem, Brick brick)
    {
        var main = particleSystem.main;
        var emission = particleSystem.emission;
        var shape = particleSystem.shape;
        var velocityOverLifetime = particleSystem.velocityOverLifetime;
        var sizeOverLifetime = particleSystem.sizeOverLifetime;
        var colorOverLifetime = particleSystem.colorOverLifetime;
        
        // Main module - basic particle properties
        main.startLifetime = 1.0f;
        main.startSpeed = 8.0f;
        main.startSize = 0.1f;
        main.startColor = Color.white; // Will be overridden by brick color
        main.maxParticles = 50;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.prewarm = false;
        main.loop = false;
        
        // Emission - disable automatic emission (manual burst only)
        emission.enabled = false;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0.0f, 15)
        });
        
        // Shape - box shape matching brick dimensions
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(1f, 0.5f, 0.1f);
        
        // Velocity over lifetime - radial explosion effect
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
        velocityOverLifetime.radial = new ParticleSystem.MinMaxCurve(2f, 5f);
        
        // Size over lifetime - particles shrink as they fade
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, 0f);
        
        // Color over lifetime - fade to transparent
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;
        
        Debug.Log($"   • ParticleSystem configured with explosion effect settings");
    }
    
    /// <summary>
    /// Creates reusable particle system prefabs for different brick types
    /// </summary>
    private static void CreateParticleSystemPrefabs()
    {
        Debug.Log("📦 [Step 3/5] Creating particle system prefabs...");
        
        // Create folder for prefabs if it doesn't exist
        string prefabFolder = "Assets/Prefabs/VFX";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "VFX");
            Debug.Log($"   • Created prefab folder: {prefabFolder}");
        }
        
        // Create different particle effect prefabs
        CreateBrickDestructionPrefab(prefabFolder, "NormalBrickDestruction", Color.red);
        CreateBrickDestructionPrefab(prefabFolder, "ReinforcedBrickDestruction", Color.blue);
        CreateBrickDestructionPrefab(prefabFolder, "PowerUpBrickDestruction", Color.yellow);
        
        Debug.Log("✅ [Step 3/5] Particle system prefabs created");
    }
    
    /// <summary>
    /// Creates a particle system prefab for brick destruction effects
    /// </summary>
    /// <param name="folder">Folder to save prefab</param>
    /// <param name="prefabName">Name of the prefab</param>
    /// <param name="color">Primary color for particles</param>
    private static void CreateBrickDestructionPrefab(string folder, string prefabName, Color color)
    {
        string prefabPath = $"{folder}/{prefabName}.prefab";
        
        // Skip if prefab already exists
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            Debug.Log($"   • Prefab already exists: {prefabName}");
            return;
        }
        
        // Create GameObject with ParticleSystem
        GameObject prefabObject = new GameObject(prefabName);
        ParticleSystem particleSystem = prefabObject.AddComponent<ParticleSystem>();
        
        // Configure particle system
        var main = particleSystem.main;
        main.startColor = color;
        main.startLifetime = 1.2f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 10f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        main.maxParticles = 20;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = false;
        
        var emission = particleSystem.emission;
        emission.enabled = false;
        
        var shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;
        
        var velocityOverLifetime = particleSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.radial = new ParticleSystem.MinMaxCurve(3f, 7f);
        
        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(prefabObject, prefabPath);
        GameObject.DestroyImmediate(prefabObject);
        
        Debug.Log($"   • Created prefab: {prefabName} with {color} color");
    }
    
    /// <summary>
    /// Tests visual effects setup with validation checks
    /// </summary>
    private static void TestVisualEffectsSetup()
    {
        Debug.Log("🧪 [Step 4/5] Testing visual effects setup...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int validEffects = 0;
        int readyForEffects = 0;
        
        foreach (Brick brick in bricks)
        {
            if (ValidateBrickEffectsSetup(brick))
            {
                validEffects++;
                
                if (brick.IsInitialized && !brick.IsDestroyed && brick.IsDestructible)
                {
                    readyForEffects++;
                }
            }
        }
        
        Debug.Log($"   • Visual effects validation: {validEffects}/{bricks.Length} bricks properly configured");
        Debug.Log($"   • Ready for effects: {readyForEffects}/{bricks.Length} bricks ready for destruction effects");
        
        // Test particle system functionality
        TestParticleSystemConfiguration();
        
        Debug.Log("✅ [Step 4/5] Visual effects testing complete");
    }
    
    /// <summary>
    /// Validates visual effects setup for individual brick
    /// </summary>
    /// <param name="brick">Brick to validate</param>
    /// <returns>True if effects setup is valid</returns>
    private static bool ValidateBrickEffectsSetup(Brick brick)
    {
        bool isValid = true;
        
        // Check ParticleSystem presence
        ParticleSystem particleSystem = brick.GetComponentInChildren<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError($"   ❌ {brick.gameObject.name}: Missing ParticleSystem component");
            isValid = false;
        }
        
        // Check visual effects initialization
        if (!brick.VisualEffectsInitialized)
        {
            Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: Visual effects system not yet initialized");
        }
        
        // Check if effects are enabled
        SerializedObject serializedBrick = new SerializedObject(brick);
        SerializedProperty enableVisualEffects = serializedBrick.FindProperty("enableVisualEffects");
        if (enableVisualEffects != null && !enableVisualEffects.boolValue)
        {
            Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: Visual effects disabled");
        }
        
        return isValid;
    }
    
    /// <summary>
    /// Tests particle system configuration and provides recommendations
    /// </summary>
    private static void TestParticleSystemConfiguration()
    {
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        foreach (Brick brick in bricks)
        {
            ParticleSystem particleSystem = brick.GetComponentInChildren<ParticleSystem>();
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                
                // Check particle count
                if (main.maxParticles < 10)
                {
                    Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: Low particle count ({main.maxParticles}) - may not be visually impactful");
                }
                
                // Check lifetime
                if (main.startLifetime.constant < 0.5f)
                {
                    Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: Short particle lifetime ({main.startLifetime.constant}s) - effects may be too brief");
                }
                
                Debug.Log($"   • {brick.gameObject.name}: ParticleSystem configured - MaxParticles: {main.maxParticles}, Lifetime: {main.startLifetime.constant}s");
            }
        }
    }
    
    /// <summary>
    /// Logs successful visual effects setup summary
    /// </summary>
    private static void LogSuccessfulSetup()
    {
        Debug.Log("✅ [Task 1.2.1.5] Visual Effects configuration completed successfully!");
        Debug.Log("📋 Visual Effects Summary:");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        Debug.Log($"   • Configured Bricks: {bricks.Length} brick components with visual effects");
        Debug.Log("   • ParticleSystem Integration: Destruction particle effects with dynamic color matching");
        Debug.Log("   • Effect Types: Explosion effects with radial velocity and size decay");
        Debug.Log("   • Color Matching: Particles automatically match brick data colors");
        
        Debug.Log("✨ Visual Effects Features:");
        Debug.Log("   → Destruction Particles: Burst emission on brick destruction");
        Debug.Log("   → Dynamic Colors: Particle colors match brick type automatically");
        Debug.Log("   → Explosion Effects: Radial velocity with size and alpha decay");
        Debug.Log("   → Performance Optimized: Manual emission, world space simulation");
        Debug.Log("   → Memory Management: Proper cleanup and reference management");
        
        Debug.Log("⚙️ Configuration Details:");
        Debug.Log("   • Particle Count: 15 particles per destruction (configurable)");
        Debug.Log("   • Particle Lifetime: 1.0 second with fade-out");
        Debug.Log("   • Explosion Speed: 8.0 units/second radial velocity");
        Debug.Log("   • Particle Size: 0.1 units with size decay over lifetime");
        Debug.Log("   • Emission: Manual burst emission (no continuous emission)");
        
        Debug.Log("🔧 Integration Points:");
        Debug.Log("   • Brick Destruction: TriggerDestructionEffects() called on ProcessDestruction()");
        Debug.Log("   • Color Synchronization: Particle colors updated from BrickData.brickColor");
        Debug.Log("   • Performance: ParticleSystem.Emit() for efficient burst effects");
        Debug.Log("   • Memory: CleanupParticleEffects() prevents memory leaks");
        Debug.Log("   • Configuration: Inspector-configurable particle parameters");
        
        Debug.Log("💡 Testing Instructions:");
        Debug.Log("   1. Select brick GameObjects to verify ParticleSystem child components");
        Debug.Log("   2. Enable 'Enable Effects Logging' on bricks for detailed debug output");
        Debug.Log("   3. Use brick destruction to test particle effects in play mode");
        Debug.Log("   4. Monitor Console for visual effects debug logs");
        Debug.Log("   5. Adjust particle parameters in Inspector for visual tuning");
        
        Debug.Log("🎮 Visual Effects Workflow:");
        Debug.Log("   1. Brick destroyed → ProcessDestruction() called");
        Debug.Log("   2. TriggerDestructionEffects() → Visual effects validation");
        Debug.Log("   3. ConfigureParticleSystem() → Dynamic color matching");
        Debug.Log("   4. EmitDestructionParticles() → Burst particle emission");
        Debug.Log("   5. CleanupParticleEffects() → Memory cleanup on GameObject destruction");
        
        Debug.Log("⚠️ Setup Requirements:");
        Debug.Log("   → ParticleSystem Component: Child component for each brick GameObject");
        Debug.Log("   → Visual Effects Enabled: 'enableVisualEffects' property set to true");
        Debug.Log("   → Particle Parameters: Configured count, lifetime, speed, and size");
        Debug.Log("   → BrickData Colors: Proper color configuration for dynamic matching");
        Debug.Log("   → Memory Management: Proper cleanup in OnDestroy() lifecycle");
        
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Test destruction effects with ball-brick collisions in play mode");
        Debug.Log("   → Fine-tune particle parameters for optimal visual impact");
        Debug.Log("   → Create custom particle effect prefabs for different brick types");
        Debug.Log("   → Integrate with audio effects for complete destruction feedback");
    }
    
    /// <summary>
    /// Utility method to clean up visual effects configuration for fresh testing
    /// </summary>
    [MenuItem("Breakout/Setup/Reset Brick Visual Effects", false, 1001)]
    public static void ResetBrickVisualEffects()
    {
        Debug.Log("🧹 [Reset] Resetting brick visual effects...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int resetCount = 0;
        int particleSystemsRemoved = 0;
        
        foreach (Brick brick in bricks)
        {
            SerializedObject serializedBrick = new SerializedObject(brick);
            
            // Reset visual effects settings
            SerializedProperty enableVisualEffects = serializedBrick.FindProperty("enableVisualEffects");
            if (enableVisualEffects != null)
            {
                enableVisualEffects.boolValue = false;
            }
            
            SerializedProperty enableEffectsLogging = serializedBrick.FindProperty("enableEffectsLogging");
            if (enableEffectsLogging != null)
            {
                enableEffectsLogging.boolValue = false;
            }
            
            // Clear particle system reference
            SerializedProperty destructionParticles = serializedBrick.FindProperty("destructionParticles");
            if (destructionParticles != null)
            {
                destructionParticles.objectReferenceValue = null;
            }
            
            // Remove ParticleSystem child components
            ParticleSystem[] particleSystems = brick.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in particleSystems)
            {
                if (ps.gameObject != brick.gameObject) // Don't destroy the brick itself
                {
                    GameObject.DestroyImmediate(ps.gameObject);
                    particleSystemsRemoved++;
                }
            }
            
            serializedBrick.ApplyModifiedProperties();
            EditorUtility.SetDirty(brick);
            resetCount++;
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"🧹 [Reset] Reset visual effects on {resetCount} brick components");
        Debug.Log($"🧹 [Reset] Removed {particleSystemsRemoved} ParticleSystem components");
    }
}
#endif