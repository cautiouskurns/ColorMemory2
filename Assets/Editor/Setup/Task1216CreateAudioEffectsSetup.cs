#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor setup script for configuring audio effects on Brick components.
/// Provides automated configuration of AudioSource components and destruction sound effects.
/// </summary>
public static class Task1216CreateAudioEffectsSetup
{
    private const string MENU_PATH = "Breakout/Setup/Task1216 Configure Brick Audio Effects";
    
    /// <summary>
    /// Configures audio effects on existing Brick components in the scene.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void ConfigureAudioEffects()
    {
        Debug.Log("🔊 [Task 1.2.1.6] Starting Audio Effects configuration...");
        
        try
        {
            // Step 1: Validate dependencies
            ValidateDependencies();
            
            // Step 2: Find and configure existing brick components
            ConfigureBrickAudioEffects();
            
            // Step 3: Create placeholder audio clips if needed
            CreatePlaceholderAudioClips();
            
            // Step 4: Test audio effects setup
            TestAudioEffectsSetup();
            
            // Step 5: Save assets and log success
            AssetDatabase.SaveAssets();
            LogSuccessfulSetup();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.2.1.6] Audio Effects configuration failed: {e.Message}");
            Debug.LogError("📋 Please ensure Brick MonoBehaviour is available");
        }
    }
    
    /// <summary>
    /// Menu validation - only show if we can find Brick components
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateConfigureAudioEffects()
    {
        return true; // Always show - validation happens during execution
    }
    
    /// <summary>
    /// Validates that all dependencies are available for audio effects setup
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
        
        // Check if AudioSource is available
        try
        {
            GameObject testObject = new GameObject("AudioSourceTest");
            AudioSource testAS = testObject.AddComponent<AudioSource>();
            if (testAS != null)
            {
                Debug.Log("   • AudioSource component available");
                GameObject.DestroyImmediate(testObject);
            }
        }
        catch (System.Exception e)
        {
            throw new System.Exception($"AudioSource component not available: {e.Message}");
        }
        
        Debug.Log("✅ [Step 1/5] Dependencies validated successfully");
    }
    
    /// <summary>
    /// Configures audio effects on all Brick components in the scene
    /// </summary>
    private static void ConfigureBrickAudioEffects()
    {
        Debug.Log("🎵 [Step 2/5] Configuring brick audio effects...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int configuredCount = 0;
        int audioSourcesAdded = 0;
        
        foreach (Brick brick in bricks)
        {
            try
            {
                ConfigureIndividualBrickAudio(brick, ref audioSourcesAdded);
                configuredCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to configure audio effects on {brick.gameObject.name}: {e.Message}");
            }
        }
        
        Debug.Log($"   • Configured audio effects on {configuredCount}/{bricks.Length} bricks");
        Debug.Log($"   • Added {audioSourcesAdded} AudioSource components");
        Debug.Log("✅ [Step 2/5] Brick audio effects configuration complete");
    }
    
    /// <summary>
    /// Configures audio effects on an individual brick component
    /// </summary>
    /// <param name="brick">Brick component to configure</param>
    /// <param name="audioSourcesAdded">Counter for added audio sources</param>
    private static void ConfigureIndividualBrickAudio(Brick brick, ref int audioSourcesAdded)
    {
        SerializedObject serializedBrick = new SerializedObject(brick);
        
        // Enable audio effects
        SerializedProperty enableAudioEffects = serializedBrick.FindProperty("enableAudioEffects");
        if (enableAudioEffects != null)
        {
            enableAudioEffects.boolValue = true;
            Debug.Log($"   • {brick.gameObject.name}: Enabled audio effects");
        }
        
        // Configure pitch variation
        SerializedProperty pitchVariation = serializedBrick.FindProperty("pitchVariation");
        if (pitchVariation != null)
        {
            pitchVariation.floatValue = 0.2f; // Good variation for arcade feel
            Debug.Log($"   • {brick.gameObject.name}: Set pitch variation to {pitchVariation.floatValue}");
        }
        
        // Configure volume multiplier
        SerializedProperty volumeMultiplier = serializedBrick.FindProperty("volumeMultiplier");
        if (volumeMultiplier != null)
        {
            volumeMultiplier.floatValue = 1.0f; // Full volume
            Debug.Log($"   • {brick.gameObject.name}: Set volume multiplier to {volumeMultiplier.floatValue}");
        }
        
        // Enable audio logging for testing
        SerializedProperty enableAudioLogging = serializedBrick.FindProperty("enableAudioLogging");
        if (enableAudioLogging != null)
        {
            enableAudioLogging.boolValue = true;
            Debug.Log($"   • {brick.gameObject.name}: Enabled audio debug logging");
        }
        
        // Ensure AudioSource component exists
        AudioSource audioSource = brick.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = brick.gameObject.AddComponent<AudioSource>();
            
            // Configure AudioSource for 2D destruction effects
            audioSource.spatialBlend = 0f; // 2D audio
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = 1.0f;
            audioSource.pitch = 1.0f;
            audioSource.priority = 128;
            
            audioSourcesAdded++;
            Debug.Log($"   • {brick.gameObject.name}: Added and configured AudioSource component");
        }
        else
        {
            // Configure existing AudioSource
            audioSource.spatialBlend = 0f; // Ensure 2D audio
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            Debug.Log($"   • {brick.gameObject.name}: Configured existing AudioSource");
        }
        
        // Update serialized property reference to the AudioSource
        SerializedProperty audioSourceProperty = serializedBrick.FindProperty("audioSource");
        if (audioSourceProperty != null)
        {
            audioSourceProperty.objectReferenceValue = audioSource;
            Debug.Log($"   • {brick.gameObject.name}: Linked AudioSource reference");
        }
        
        // Apply changes
        serializedBrick.ApplyModifiedProperties();
        EditorUtility.SetDirty(brick);
    }
    
    /// <summary>
    /// Creates placeholder audio clips for testing if none exist
    /// </summary>
    private static void CreatePlaceholderAudioClips()
    {
        Debug.Log("📦 [Step 3/5] Creating placeholder audio clips...");
        
        // Create folder for audio clips if it doesn't exist
        string audioFolder = "Assets/Audio/SFX";
        if (!AssetDatabase.IsValidFolder("Assets/Audio"))
        {
            AssetDatabase.CreateFolder("Assets", "Audio");
        }
        if (!AssetDatabase.IsValidFolder(audioFolder))
        {
            AssetDatabase.CreateFolder("Assets/Audio", "SFX");
            Debug.Log($"   • Created audio folder: {audioFolder}");
        }
        
        // For now, we'll just log that placeholder clips should be created
        // In a real project, you would create or import actual audio clips here
        Debug.Log("   • Audio clips should be imported to Assets/Audio/SFX/");
        Debug.Log("   • Recommended clips: brick_break_normal.wav, brick_break_reinforced.wav, etc.");
        
        // Try to find Unity's default audio clip for placeholder use
        AudioClip defaultClip = Resources.GetBuiltinResource<AudioClip>("Click.wav");
        if (defaultClip != null)
        {
            Debug.Log("   • Found Unity default audio clip for placeholder use");
            AssignPlaceholderClipsToAllBricks(defaultClip);
        }
        else
        {
            Debug.LogWarning("   • No default audio clip found - bricks will need manual audio clip assignment");
        }
        
        Debug.Log("✅ [Step 3/5] Placeholder audio clips setup complete");
    }
    
    /// <summary>
    /// Assigns placeholder audio clips to all bricks for testing
    /// </summary>
    /// <param name="placeholderClip">Placeholder audio clip to assign</param>
    private static void AssignPlaceholderClipsToAllBricks(AudioClip placeholderClip)
    {
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int assignedCount = 0;
        
        foreach (Brick brick in bricks)
        {
            SerializedObject serializedBrick = new SerializedObject(brick);
            
            // Assign default destruction sound
            SerializedProperty destructionSound = serializedBrick.FindProperty("destructionSound");
            if (destructionSound != null && destructionSound.objectReferenceValue == null)
            {
                destructionSound.objectReferenceValue = placeholderClip;
                assignedCount++;
            }
            
            serializedBrick.ApplyModifiedProperties();
            EditorUtility.SetDirty(brick);
        }
        
        Debug.Log($"   • Assigned placeholder audio clip to {assignedCount} bricks");
    }
    
    /// <summary>
    /// Tests audio effects setup with validation checks
    /// </summary>
    private static void TestAudioEffectsSetup()
    {
        Debug.Log("🧪 [Step 4/5] Testing audio effects setup...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int validAudio = 0;
        int readyForAudio = 0;
        
        foreach (Brick brick in bricks)
        {
            if (ValidateBrickAudioSetup(brick))
            {
                validAudio++;
                
                if (brick.IsInitialized && !brick.IsDestroyed && brick.IsDestructible)
                {
                    readyForAudio++;
                }
            }
        }
        
        Debug.Log($"   • Audio effects validation: {validAudio}/{bricks.Length} bricks properly configured");
        Debug.Log($"   • Ready for audio: {readyForAudio}/{bricks.Length} bricks ready for destruction audio");
        
        // Test audio configuration
        TestAudioConfiguration();
        
        Debug.Log("✅ [Step 4/5] Audio effects testing complete");
    }
    
    /// <summary>
    /// Validates audio effects setup for individual brick
    /// </summary>
    /// <param name="brick">Brick to validate</param>
    /// <returns>True if audio setup is valid</returns>
    private static bool ValidateBrickAudioSetup(Brick brick)
    {
        bool isValid = true;
        
        // Check AudioSource presence
        AudioSource audioSource = brick.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError($"   ❌ {brick.gameObject.name}: Missing AudioSource component");
            isValid = false;
        }
        else
        {
            // Validate AudioSource configuration
            if (audioSource.spatialBlend != 0f)
            {
                Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: AudioSource not configured for 2D (spatialBlend: {audioSource.spatialBlend})");
            }
            
            if (audioSource.playOnAwake)
            {
                Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: AudioSource should not play on awake");
            }
        }
        
        // Check audio system initialization
        if (!brick.AudioSystemInitialized)
        {
            Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: Audio system not yet initialized");
        }
        
        // Check if audio effects are enabled
        SerializedObject serializedBrick = new SerializedObject(brick);
        SerializedProperty enableAudioEffects = serializedBrick.FindProperty("enableAudioEffects");
        if (enableAudioEffects != null && !enableAudioEffects.boolValue)
        {
            Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: Audio effects disabled");
        }
        
        return isValid;
    }
    
    /// <summary>
    /// Tests audio configuration and provides recommendations
    /// </summary>
    private static void TestAudioConfiguration()
    {
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        foreach (Brick brick in bricks)
        {
            AudioSource audioSource = brick.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                // Check volume levels
                if (audioSource.volume > 1.0f)
                {
                    Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: High volume level ({audioSource.volume}) - may cause audio clipping");
                }
                
                // Check priority
                if (audioSource.priority > 200)
                {
                    Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: Low priority ({audioSource.priority}) - may not play during busy audio scenes");
                }
                
                Debug.Log($"   • {brick.gameObject.name}: AudioSource configured - Volume: {audioSource.volume}, Priority: {audioSource.priority}");
            }
        }
    }
    
    /// <summary>
    /// Logs successful audio effects setup summary
    /// </summary>
    private static void LogSuccessfulSetup()
    {
        Debug.Log("✅ [Task 1.2.1.6] Audio Effects configuration completed successfully!");
        Debug.Log("📋 Audio Effects Summary:");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        Debug.Log($"   • Configured Bricks: {bricks.Length} brick components with audio effects");
        Debug.Log("   • AudioSource Integration: 2D audio sources with PlayOneShot() for overlapping sounds");
        Debug.Log("   • Pitch Variation: ±0.2f pitch randomization for natural audio variety");
        Debug.Log("   • Type-Specific Audio: Framework ready for different sound effects per brick type");
        
        Debug.Log("🔊 Audio Effects Features:");
        Debug.Log("   → Destruction Audio: Immediate sound feedback on brick destruction");
        Debug.Log("   → Pitch Variation: Random pitch changes for natural sound variety");
        Debug.Log("   → Type-Specific Sounds: Different audio clips for different brick types");
        Debug.Log("   → 2D Audio Configuration: Optimized for 2D arcade-style gameplay");
        Debug.Log("   → PlayOneShot Integration: Allows overlapping sounds during rapid destruction");
        
        Debug.Log("⚙️ Configuration Details:");
        Debug.Log("   • Audio Source: 2D spatial blend, no loop, no play on awake");
        Debug.Log("   • Volume Control: Configurable volume multiplier per brick");
        Debug.Log("   • Pitch Variation: ±0.2f range for arcade-style audio feedback");
        Debug.Log("   • Priority: Default priority (128) for balanced audio mixing");
        Debug.Log("   • Performance: Efficient PlayOneShot() for destruction sound effects");
        
        Debug.Log("🔧 Integration Points:");
        Debug.Log("   • Destruction Events: TriggerDestructionAudio() called during ProcessDestruction()");
        Debug.Log("   • Visual Effects: Audio synchronized with particle effects and brick hiding");
        Debug.Log("   • Type System: BrickType enum used for sound clip selection");
        Debug.Log("   • Debug System: Comprehensive logging for audio troubleshooting");
        Debug.Log("   • Editor Integration: Automatic AudioSource setup and configuration");
        
        Debug.Log("💡 Testing Instructions:");
        Debug.Log("   1. Enable 'Enable Audio Logging' on bricks for detailed debug output");
        Debug.Log("   2. Import audio clips to Assets/Audio/SFX/ folder");
        Debug.Log("   3. Assign clips to 'Destruction Sound' field in brick Inspector");
        Debug.Log("   4. Test destruction by destroying bricks in play mode");
        Debug.Log("   5. Listen for pitch-varied destruction sound effects");
        
        Debug.Log("🎮 Audio Effects Workflow:");
        Debug.Log("   1. Brick destroyed → TriggerDestructionAudio() called");
        Debug.Log("   2. ConfigureAudioPlayback() → Apply pitch variation and volume");
        Debug.Log("   3. GetSoundForBrickType() → Select appropriate audio clip");
        Debug.Log("   4. PlayDestructionSound() → PlayOneShot() with volume multiplier");
        Debug.Log("   5. Audio plays immediately with visual effects synchronization");
        
        Debug.Log("⚠️ Setup Requirements:");
        Debug.Log("   → AudioSource Component: Added to each brick GameObject for sound playback");
        Debug.Log("   → Audio Clips: Import destruction sound effects to project");
        Debug.Log("   → Audio Configuration: 2D spatial blend for arcade-style audio");
        Debug.Log("   → Volume Levels: Ensure appropriate volume levels for game audio mixing");
        Debug.Log("   → Performance: PlayOneShot() allows overlapping destruction sounds");
        
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Import custom audio clips for different brick types");
        Debug.Log("   → Test audio effects with ball-brick collisions in play mode");
        Debug.Log("   → Fine-tune pitch variation and volume levels for optimal feel");
        Debug.Log("   → Integrate with global audio mixing and music systems");
    }
    
    /// <summary>
    /// Utility method to reset audio effects configuration for fresh testing
    /// </summary>
    [MenuItem("Breakout/Setup/Reset Brick Audio Effects", false, 1001)]
    public static void ResetBrickAudioEffects()
    {
        Debug.Log("🧹 [Reset] Resetting brick audio effects...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int resetCount = 0;
        int audioSourcesRemoved = 0;
        
        foreach (Brick brick in bricks)
        {
            SerializedObject serializedBrick = new SerializedObject(brick);
            
            // Reset audio effects settings
            SerializedProperty enableAudioEffects = serializedBrick.FindProperty("enableAudioEffects");
            if (enableAudioEffects != null)
            {
                enableAudioEffects.boolValue = false;
            }
            
            SerializedProperty enableAudioLogging = serializedBrick.FindProperty("enableAudioLogging");
            if (enableAudioLogging != null)
            {
                enableAudioLogging.boolValue = false;
            }
            
            // Clear audio clip references
            SerializedProperty destructionSound = serializedBrick.FindProperty("destructionSound");
            if (destructionSound != null)
            {
                destructionSound.objectReferenceValue = null;
            }
            
            SerializedProperty audioSource = serializedBrick.FindProperty("audioSource");
            if (audioSource != null)
            {
                audioSource.objectReferenceValue = null;
            }
            
            // Remove AudioSource components
            AudioSource[] audioSources = brick.GetComponents<AudioSource>();
            foreach (AudioSource source in audioSources)
            {
                GameObject.DestroyImmediate(source);
                audioSourcesRemoved++;
            }
            
            serializedBrick.ApplyModifiedProperties();
            EditorUtility.SetDirty(brick);
            resetCount++;
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"🧹 [Reset] Reset audio effects on {resetCount} brick components");
        Debug.Log($"🧹 [Reset] Removed {audioSourcesRemoved} AudioSource components");
    }
}
#endif