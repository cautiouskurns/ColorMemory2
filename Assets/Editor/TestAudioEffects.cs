#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Test script for manually triggering and validating audio effects on bricks
/// </summary>
public static class TestAudioEffects
{
    [MenuItem("Breakout/Debug/Test Audio Effects")]
    public static void TestBrickAudioEffects()
    {
        Debug.Log("🔊 Testing audio effects on all bricks...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        if (bricks.Length == 0)
        {
            Debug.LogWarning("No bricks found in scene! Please create some test bricks first.");
            return;
        }
        
        foreach (Brick brick in bricks)
        {
            Debug.Log($"🎵 Testing audio effects on {brick.gameObject.name}");
            
            // Reset the audioTriggered flag using reflection
            System.Reflection.FieldInfo audioTriggeredField = typeof(Brick).GetField("audioTriggered", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (audioTriggeredField != null)
            {
                audioTriggeredField.SetValue(brick, false);
                Debug.Log($"🔄 Reset audioTriggered flag for {brick.gameObject.name}");
            }
            
            // Use reflection to call private audio method for testing
            System.Reflection.MethodInfo triggerMethod = typeof(Brick).GetMethod("TriggerDestructionAudio", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (triggerMethod != null)
            {
                try
                {
                    triggerMethod.Invoke(brick, null);
                    Debug.Log($"✅ Triggered audio effects on {brick.gameObject.name}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Failed to trigger audio on {brick.gameObject.name}: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("TriggerDestructionAudio method not found!");
            }
        }
        
        Debug.Log("🔊 Audio effects test completed");
    }
    
    [MenuItem("Breakout/Debug/Test Audio Configuration")]
    public static void TestAudioConfiguration()
    {
        Debug.Log("🎛️ Testing audio configuration on all bricks...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int validConfigs = 0;
        
        foreach (Brick brick in bricks)
        {
            Debug.Log($"🔍 Checking audio config for {brick.gameObject.name}");
            
            // Check AudioSource component
            AudioSource audioSource = brick.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                Debug.Log($"   • AudioSource: Volume={audioSource.volume}, Pitch={audioSource.pitch}, Spatial={audioSource.spatialBlend}");
                
                if (audioSource.spatialBlend == 0f && !audioSource.loop && !audioSource.playOnAwake)
                {
                    validConfigs++;
                    Debug.Log($"   ✅ {brick.gameObject.name}: Audio configuration valid");
                }
                else
                {
                    Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: Audio configuration needs adjustment");
                }
            }
            else
            {
                Debug.LogError($"   ❌ {brick.gameObject.name}: No AudioSource component found");
            }
            
            // Check brick audio system status
            Debug.Log($"   • Audio System Initialized: {brick.AudioSystemInitialized}");
            Debug.Log($"   • Audio Triggered: {brick.AudioTriggered}");
            
            // Check audio clip assignment
            SerializedObject serializedBrick = new SerializedObject(brick);
            SerializedProperty destructionSound = serializedBrick.FindProperty("destructionSound");
            if (destructionSound != null && destructionSound.objectReferenceValue != null)
            {
                AudioClip clip = destructionSound.objectReferenceValue as AudioClip;
                Debug.Log($"   • Destruction Sound: {clip.name} ({clip.length:F1}s)");
            }
            else
            {
                Debug.LogWarning($"   ⚠️ {brick.gameObject.name}: No destruction sound clip assigned");
            }
        }
        
        Debug.Log($"🎛️ Audio configuration test completed - {validConfigs}/{bricks.Length} bricks properly configured");
    }
    
    [MenuItem("Breakout/Debug/Play Sample Audio")]
    public static void PlaySampleAudio()
    {
        Debug.Log("🎵 Playing sample audio on first available brick...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        if (bricks.Length == 0)
        {
            Debug.LogWarning("No bricks found!");
            return;
        }
        
        Brick testBrick = bricks[0];
        AudioSource audioSource = testBrick.GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on test brick!");
            return;
        }
        
        // Check if we have an audio clip
        SerializedObject serializedBrick = new SerializedObject(testBrick);
        SerializedProperty destructionSound = serializedBrick.FindProperty("destructionSound");
        
        AudioClip testClip = null;
        if (destructionSound != null && destructionSound.objectReferenceValue != null)
        {
            testClip = destructionSound.objectReferenceValue as AudioClip;
        }
        else
        {
            // Try to find Unity's default click sound
            testClip = Resources.GetBuiltinResource<AudioClip>("Click.wav");
        }
        
        if (testClip == null)
        {
            Debug.LogError("No audio clip available for testing!");
            return;
        }
        
        Debug.Log($"🎵 Playing test audio clip: {testClip.name} on {testBrick.gameObject.name}");
        
        // Configure for testing
        audioSource.pitch = Random.Range(0.8f, 1.2f); // Add some pitch variation
        audioSource.volume = 1.0f;
        
        // Play the sound
        audioSource.PlayOneShot(testClip);
        
        Debug.Log($"✅ Audio played with pitch: {audioSource.pitch:F2}");
    }
    
    [MenuItem("Breakout/Debug/Test Audio with Real Destruction")]
    public static void TestAudioWithRealDestruction()
    {
        Debug.Log("💥 Testing audio with real brick destruction...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        if (bricks.Length == 0)
        {
            Debug.LogWarning("No bricks found!");
            return;
        }
        
        // Find a destructible brick
        Brick testBrick = null;
        foreach (Brick brick in bricks)
        {
            if (brick.IsDestructible && !brick.IsDestroyed)
            {
                testBrick = brick;
                break;
            }
        }
        
        if (testBrick == null)
        {
            Debug.LogWarning("No destructible bricks found!");
            return;
        }
        
        Debug.Log($"💥 Destroying {testBrick.gameObject.name} to test audio + visual effects...");
        
        // Enable audio logging temporarily
        SerializedObject serializedBrick = new SerializedObject(testBrick);
        SerializedProperty enableAudioLogging = serializedBrick.FindProperty("enableAudioLogging");
        if (enableAudioLogging != null)
        {
            enableAudioLogging.boolValue = true;
            serializedBrick.ApplyModifiedProperties();
        }
        
        // Use reflection to call ProcessDestruction method
        System.Reflection.MethodInfo processDestructionMethod = typeof(Brick).GetMethod("ProcessDestruction", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (processDestructionMethod != null)
        {
            try
            {
                processDestructionMethod.Invoke(testBrick, null);
                Debug.Log($"✅ Triggered real destruction with audio on {testBrick.gameObject.name}");
                Debug.Log("🎵 Listen for destruction sound effect!");
                Debug.Log("💥 Watch for particle effects!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to destroy brick: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("ProcessDestruction method not found!");
        }
    }
    
    [MenuItem("Breakout/Debug/Audio System Status")]
    public static void ShowAudioSystemStatus()
    {
        Debug.Log("📊 Audio System Status Report");
        Debug.Log("============================");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        if (bricks.Length == 0)
        {
            Debug.Log("❌ No bricks found in scene");
            return;
        }
        
        int audioSourceCount = 0;
        int audioClipCount = 0;
        int initializedCount = 0;
        int enabledCount = 0;
        
        foreach (Brick brick in bricks)
        {
            // Count AudioSources
            if (brick.GetComponent<AudioSource>() != null)
            {
                audioSourceCount++;
            }
            
            // Count audio clips assigned
            SerializedObject serializedBrick = new SerializedObject(brick);
            SerializedProperty destructionSound = serializedBrick.FindProperty("destructionSound");
            if (destructionSound != null && destructionSound.objectReferenceValue != null)
            {
                audioClipCount++;
            }
            
            // Count initialized systems
            if (brick.AudioSystemInitialized)
            {
                initializedCount++;
            }
            
            // Count enabled audio effects
            SerializedProperty enableAudioEffects = serializedBrick.FindProperty("enableAudioEffects");
            if (enableAudioEffects != null && enableAudioEffects.boolValue)
            {
                enabledCount++;
            }
        }
        
        Debug.Log($"📊 Bricks Found: {bricks.Length}");
        Debug.Log($"🔊 AudioSources: {audioSourceCount}/{bricks.Length} ({(audioSourceCount * 100f / bricks.Length):F0}%)");
        Debug.Log($"🎵 Audio Clips: {audioClipCount}/{bricks.Length} ({(audioClipCount * 100f / bricks.Length):F0}%)");
        Debug.Log($"⚙️ Initialized: {initializedCount}/{bricks.Length} ({(initializedCount * 100f / bricks.Length):F0}%)");
        Debug.Log($"✅ Enabled: {enabledCount}/{bricks.Length} ({(enabledCount * 100f / bricks.Length):F0}%)");
        
        if (audioSourceCount == bricks.Length && audioClipCount == bricks.Length)
        {
            Debug.Log("🎉 Audio system fully configured!");
        }
        else
        {
            Debug.Log("⚠️ Audio system needs configuration - run 'Configure Brick Audio Effects'");
        }
    }
}
#endif