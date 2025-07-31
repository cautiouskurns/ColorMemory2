#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for configuring collision feedback system in CollisionManager.
/// Provides automated audio-visual feedback component setup and configuration.
/// </summary>
public static class Task1134CreateCollisionFeedbackSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1134 Configure Collision Feedback";
    
    /// <summary>
    /// Configures collision feedback system on existing CollisionManager.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void ConfigureCollisionFeedback()
    {
        Debug.Log("📋 [Task 1.1.3.4] Starting Collision Feedback configuration...");
        
        try
        {
            // Step 1: Find and validate CollisionManager
            CollisionManager collisionManager = FindCollisionManager();
            
            // Step 2: Configure AudioSource component
            ConfigureAudioSource(collisionManager);
            
            // Step 3: Configure ParticleSystem component
            ConfigureParticleSystem(collisionManager);
            
            // Step 4: Configure Camera reference
            ConfigureCameraReference(collisionManager);
            
            // Step 5: Configure feedback parameters
            ConfigureFeedbackParameters(collisionManager);
            
            // Step 6: Validate feedback system setup
            ValidateFeedbackSetup(collisionManager);
            
            // Step 7: Save and finalize
            EditorUtility.SetDirty(collisionManager.gameObject);
            AssetDatabase.SaveAssets();
            
            LogSuccessfulSetup(collisionManager);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.3.4] Collision Feedback configuration failed: {e.Message}");
            Debug.LogError("📋 Please ensure CollisionManager exists and is properly configured.");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if CollisionManager exists without feedback components.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateConfigureCollisionFeedback()
    {
        CollisionManager cm = GameObject.FindFirstObjectByType<CollisionManager>();
        return cm != null && (cm.GetComponent<AudioSource>() == null || cm.GetComponent<ParticleSystem>() == null);
    }
    
    /// <summary>
    /// Finds and validates the CollisionManager in the scene.
    /// </summary>
    /// <returns>CollisionManager component</returns>
    private static CollisionManager FindCollisionManager()
    {
        Debug.Log("🔍 [Step 1/6] Finding CollisionManager...");
        
        CollisionManager collisionManager = GameObject.FindFirstObjectByType<CollisionManager>();
        
        if (collisionManager == null)
        {
            Debug.LogError("❌ CollisionManager not found in scene!");
            Debug.LogError("📋 Please run 'Breakout/Setup/Task1132 Create Collision Manager' first.");
            throw new System.NullReferenceException("CollisionManager component is required");
        }
        
        Debug.Log($"✅ [Step 1/6] CollisionManager found: {collisionManager.gameObject.name}");
        return collisionManager;
    }
    
    /// <summary>
    /// Configures AudioSource component for collision audio feedback.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to configure</param>
    private static void ConfigureAudioSource(CollisionManager collisionManager)
    {
        Debug.Log("🔊 [Step 2/6] Configuring AudioSource component...");
        
        AudioSource audioSource = collisionManager.GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            // Add AudioSource component
            audioSource = collisionManager.gameObject.AddComponent<AudioSource>();
            Debug.Log("   • AudioSource component added to CollisionManager");
        }
        else
        {
            Debug.Log("   • AudioSource component already present");
        }
        
        // Configure AudioSource settings for collision feedback
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 0.7f;
        audioSource.pitch = 1.0f;
        audioSource.spatialBlend = 0.0f; // 2D sound
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        
        // Assign AudioSource to CollisionManager via SerializedObject
        SerializedObject serializedManager = new SerializedObject(collisionManager);
        SerializedProperty audioSourceProperty = serializedManager.FindProperty("audioSource");
        
        if (audioSourceProperty != null)
        {
            audioSourceProperty.objectReferenceValue = audioSource;
            serializedManager.ApplyModifiedProperties();
            Debug.Log("   • AudioSource reference assigned to CollisionManager");
        }
        
        Debug.Log("✅ [Step 2/6] AudioSource configuration complete");
    }
    
    /// <summary>
    /// Configures ParticleSystem component for collision visual effects.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to configure</param>
    private static void ConfigureParticleSystem(CollisionManager collisionManager)
    {
        Debug.Log("✨ [Step 3/6] Configuring ParticleSystem component...");
        
        ParticleSystem particleSystem = collisionManager.GetComponent<ParticleSystem>();
        
        if (particleSystem == null)
        {
            // Add ParticleSystem component
            particleSystem = collisionManager.gameObject.AddComponent<ParticleSystem>();
            Debug.Log("   • ParticleSystem component added to CollisionManager");
        }
        else
        {
            Debug.Log("   • ParticleSystem component already present");
        }
        
        // Configure particle system for collision burst effects
        var main = particleSystem.main;
        main.startLifetime = 0.3f;
        main.startSpeed = 2.0f;
        main.startSize = 0.1f;
        main.startColor = Color.white;
        main.maxParticles = 50;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        // Configure emission (manual burst mode)
        var emission = particleSystem.emission;
        emission.enabled = false; // We'll emit manually
        
        // Configure shape for collision burst
        var shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.2f;
        
        // Configure velocity over lifetime for burst spread
        var velocityOverLifetime = particleSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
        velocityOverLifetime.radial = new ParticleSystem.MinMaxCurve(3.0f);
        
        // Configure size over lifetime for fade effect
        var sizeOverLifetime = particleSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        
        // Assign ParticleSystem to CollisionManager via SerializedObject
        SerializedObject serializedManager = new SerializedObject(collisionManager);
        SerializedProperty particleSystemProperty = serializedManager.FindProperty("collisionParticles");
        
        if (particleSystemProperty != null)
        {
            particleSystemProperty.objectReferenceValue = particleSystem;
            serializedManager.ApplyModifiedProperties();
            Debug.Log("   • ParticleSystem reference assigned to CollisionManager");
        }
        
        Debug.Log("✅ [Step 3/6] ParticleSystem configuration complete");
    }
    
    /// <summary>
    /// Configures Camera reference for screen shake effects.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to configure</param>
    private static void ConfigureCameraReference(CollisionManager collisionManager)
    {
        Debug.Log("📷 [Step 4/6] Configuring Camera reference...");
        
        // Find main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            // Try to find any camera in the scene
            mainCamera = GameObject.FindFirstObjectByType<Camera>();
        }
        
        if (mainCamera != null)
        {
            // Assign Camera to CollisionManager via SerializedObject
            SerializedObject serializedManager = new SerializedObject(collisionManager);
            SerializedProperty cameraProperty = serializedManager.FindProperty("gameCamera");
            
            if (cameraProperty != null)
            {
                cameraProperty.objectReferenceValue = mainCamera;
                serializedManager.ApplyModifiedProperties();
                Debug.Log($"   • Camera reference assigned: {mainCamera.gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning("   ⚠️ No camera found in scene. Screen shake effects will be disabled.");
            Debug.LogWarning("   📋 Create a Camera and assign it manually to CollisionManager.gameCamera field.");
        }
        
        Debug.Log("✅ [Step 4/6] Camera reference configuration complete");
    }
    
    /// <summary>
    /// Configures feedback parameters for optimal arcade-style response.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to configure</param>
    private static void ConfigureFeedbackParameters(CollisionManager collisionManager)
    {
        Debug.Log("⚙️ [Step 5/6] Configuring feedback parameters...");
        
        SerializedObject serializedManager = new SerializedObject(collisionManager);
        
        // Configure screen shake parameters
        SerializedProperty screenShakeIntensity = serializedManager.FindProperty("screenShakeIntensity");
        SerializedProperty screenShakeDuration = serializedManager.FindProperty("screenShakeDuration");
        SerializedProperty particleBurstCount = serializedManager.FindProperty("particleBurstCount");
        
        // Configure feedback enable flags
        SerializedProperty enableAudioFeedback = serializedManager.FindProperty("enableAudioFeedback");
        SerializedProperty enableParticleFeedback = serializedManager.FindProperty("enableParticleFeedback");
        SerializedProperty enableScreenShake = serializedManager.FindProperty("enableScreenShake");
        
        if (screenShakeIntensity != null && screenShakeDuration != null && particleBurstCount != null)
        {
            // Set optimal feedback parameters for arcade gameplay
            screenShakeIntensity.floatValue = 0.1f;  // Subtle screen shake
            screenShakeDuration.floatValue = 0.15f;  // Quick shake duration
            particleBurstCount.intValue = 5;         // Moderate particle count
            
            Debug.Log($"   • Screen Shake Intensity: {screenShakeIntensity.floatValue:F2}");
            Debug.Log($"   • Screen Shake Duration: {screenShakeDuration.floatValue:F2}s");
            Debug.Log($"   • Particle Burst Count: {particleBurstCount.intValue}");
        }
        
        if (enableAudioFeedback != null && enableParticleFeedback != null && enableScreenShake != null)
        {
            // Enable all feedback types by default
            enableAudioFeedback.boolValue = true;
            enableParticleFeedback.boolValue = true;
            enableScreenShake.boolValue = true;
            
            Debug.Log("   • Audio Feedback: Enabled");
            Debug.Log("   • Particle Feedback: Enabled");
            Debug.Log("   • Screen Shake Feedback: Enabled");
        }
        
        // Apply changes
        serializedManager.ApplyModifiedProperties();
        
        Debug.Log("✅ [Step 5/6] Feedback parameters configured");
    }
    
    /// <summary>
    /// Validates collision feedback system setup.
    /// </summary>
    /// <param name="collisionManager">CollisionManager to validate</param>
    private static void ValidateFeedbackSetup(CollisionManager collisionManager)
    {
        Debug.Log("🧪 [Step 6/6] Validating feedback system setup...");
        
        int componentCount = 0;
        
        // Validate AudioSource component
        AudioSource audioSource = collisionManager.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            componentCount++;
            Debug.Log("   • AudioSource: Present and configured");
        }
        else
        {
            Debug.LogError("   ❌ AudioSource component missing!");
        }
        
        // Validate ParticleSystem component
        ParticleSystem particleSystem = collisionManager.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            componentCount++;
            Debug.Log("   • ParticleSystem: Present and configured");
        }
        else
        {
            Debug.LogError("   ❌ ParticleSystem component missing!");
        }
        
        // Validate Camera reference (using SerializedObject to access private field)
        SerializedObject serializedManager = new SerializedObject(collisionManager);
        SerializedProperty cameraProperty = serializedManager.FindProperty("gameCamera");
        
        if (cameraProperty != null && cameraProperty.objectReferenceValue != null)
        {
            componentCount++;
            Camera assignedCamera = (Camera)cameraProperty.objectReferenceValue;
            Debug.Log($"   • Camera Reference: {assignedCamera.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("   ⚠️ Camera reference not assigned. Screen shake will be disabled.");
        }
        
        Debug.Log($"   • Feedback Components: {componentCount}/3 configured");
        
        if (componentCount >= 2) // AudioSource and ParticleSystem minimum
        {
            Debug.Log("   ✅ Feedback system ready for collision audio-visual effects");
        }
        else
        {
            Debug.LogWarning("   ⚠️ Feedback system incomplete - some effects may not work");
        }
        
        Debug.Log("✅ [Step 6/6] Feedback system validation complete");
    }
    
    /// <summary>
    /// Logs successful collision feedback setup summary.
    /// </summary>
    /// <param name="collisionManager">Configured CollisionManager</param>
    private static void LogSuccessfulSetup(CollisionManager collisionManager)
    {
        Debug.Log("✅ [Task 1.1.3.4] Collision Feedback configured successfully!");
        Debug.Log("📋 Collision Feedback Summary:");
        Debug.Log($"   • Component: Enhanced CollisionManager on {collisionManager.gameObject.name}");
        Debug.Log($"   • Location: {GetGameObjectPath(collisionManager.gameObject)}");
        
        // Log feedback system capabilities
        Debug.Log("🎯 Collision Feedback Features:");
        Debug.Log("   → Audio Feedback: Collision-specific sound effects for paddle, wall, brick, and power-up collisions");
        Debug.Log("   → Particle Effects: Visual impact bursts with collision-type color coding");
        Debug.Log("   → Screen Shake: Camera shake effects with intensity scaling based on collision force");
        Debug.Log("   → Intensity Scaling: All feedback effects scale with collision velocity for dynamic response");
        Debug.Log("   → Immediate Response: Sub-frame latency feedback synchronized with physics events");
        
        // Log collision type feedback mapping
        Debug.Log("🎮 Collision Type Feedback Mapping:");
        Debug.Log("   • Paddle Collisions: Cyan particles + paddle bounce audio + subtle shake");
        Debug.Log("   • Wall/Boundary Collisions: White particles + wall bounce audio + medium shake");
        Debug.Log("   • Brick Collisions: Yellow particles + brick hit audio + strong shake");
        Debug.Log("   • Power-Up Collisions: Magenta particles + power-up audio + light shake");
        
        // Log technical implementation
        Debug.Log("🔧 Technical Implementation:");
        Debug.Log("   • AudioSource.PlayOneShot() for collision sound effects without audio cutting");
        Debug.Log("   • ParticleSystem.Emit() for burst particle effects at collision contact points");
        Debug.Log("   • Coroutine-based screen shake with smooth dampening and position restoration");
        Debug.Log("   • Collision intensity calculated from relativeVelocity.magnitude for realistic feedback");
        Debug.Log("   • Feedback triggers integrated into existing collision handler framework");
        
        // Log configuration parameters
        Debug.Log("⚙️ Current Feedback Configuration:");
        Debug.Log("   • Screen Shake: 0.1 intensity, 0.15s duration with smooth dampening");
        Debug.Log("   • Particle Burst: 5 particles per collision with type-specific colors");
        Debug.Log("   • Audio Volume: 0.3-1.0 range scaled by collision intensity");
        Debug.Log("   • Feedback Types: Audio + Particle + Screen Shake all enabled");
        
        // Log feedback system API
        Debug.Log("📊 Feedback System API:");
        Debug.Log("   • TriggerCollisionFeedback(type, position, intensity) - Manual feedback trigger");
        Debug.Log("   • CalculateCollisionIntensity(collision) - Intensity calculation from physics data");
        Debug.Log("   • TriggerAudioFeedback(type, intensity) - Audio-only feedback");
        Debug.Log("   • TriggerParticleFeedback(type, position, intensity) - Particle-only feedback");
        Debug.Log("   • TriggerScreenShake(intensity) - Screen shake-only feedback");
        
        // Log audio clip assignment instructions
        Debug.Log("🎵 Audio Clip Assignment:");
        Debug.Log("   → Assign audio clips in CollisionManager Inspector for full audio feedback:");
        Debug.Log("     • Paddle Bounce Clip: Short, satisfying bounce sound");
        Debug.Log("     • Wall Bounce Clip: Sharp, reflective bounce sound");
        Debug.Log("     • Brick Hit Clip: Impact sound with slight metallic ring");
        Debug.Log("     • Power-Up Clip: Positive, collectible sound effect");
        
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Collision feedback automatically triggers during all collision events");
        Debug.Log("   2. Assign audio clips in Inspector for full audio feedback experience");
        Debug.Log("   3. Adjust feedback intensities in Inspector for different game feel");
        Debug.Log("   4. Enable/disable individual feedback types via Inspector checkboxes");
        Debug.Log("   5. Screen shake requires Camera reference assignment for proper function");
        
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Assign audio clips to paddleBounceClip, wallBounceClip, brickHitClip, powerUpClip fields");
        Debug.Log("   → Test collision feedback in Play mode with Ball-object collisions");
        Debug.Log("   → Fine-tune feedback parameters for optimal arcade game feel");
        Debug.Log("   → Verify particle colors and audio cues are distinct for each collision type");
        
        Debug.Log("⚠️ Integration Requirements:");
        Debug.Log("   → Audio clips must be assigned for audio feedback (AudioClip assets in project)");
        Debug.Log("   → Camera reference required for screen shake effects");
        Debug.Log("   → Ball GameObject must forward collision events to CollisionManager for feedback triggers");
        Debug.Log("   → Feedback system requires CollisionManager collision detection framework from Task 1.1.3.2");
    }
    
    /// <summary>
    /// Gets the full hierarchy path of a GameObject.
    /// </summary>
    /// <param name="gameObject">GameObject to get path for</param>
    /// <returns>Hierarchy path string</returns>
    private static string GetGameObjectPath(GameObject gameObject)
    {
        string path = gameObject.name;
        Transform current = gameObject.transform.parent;
        
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        
        return path;
    }
}
#endif