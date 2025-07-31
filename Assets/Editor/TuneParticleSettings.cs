#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class TuneParticleSettings
{
    [MenuItem("Breakout/Debug/Make Particles More Visible")]
    public static void MakeParticlesVisible()
    {
        Debug.Log("👁️ Making particles more visible for gameplay...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int tuned = 0;
        
        foreach (Brick brick in bricks)
        {
            SerializedObject serializedBrick = new SerializedObject(brick);
            
            // Increase particle size for visibility
            SerializedProperty particleSize = serializedBrick.FindProperty("particleSize");
            if (particleSize != null)
            {
                particleSize.floatValue = 0.3f; // Much larger
            }
            
            // Increase particle count for better effect
            SerializedProperty particleCount = serializedBrick.FindProperty("particleCount");
            if (particleCount != null)
            {
                particleCount.intValue = 25; // More particles
            }
            
            // Increase lifetime for longer visibility
            SerializedProperty particleLifetime = serializedBrick.FindProperty("particleLifetime");
            if (particleLifetime != null)
            {
                particleLifetime.floatValue = 2.0f; // Longer lifetime
            }
            
            // Reduce speed so particles don't fly away too fast
            SerializedProperty particleSpeed = serializedBrick.FindProperty("particleSpeed");
            if (particleSpeed != null)
            {
                particleSpeed.floatValue = 4.0f; // Slower speed
            }
            
            // Ensure visual effects are enabled
            SerializedProperty enableVisualEffects = serializedBrick.FindProperty("enableVisualEffects");
            if (enableVisualEffects != null)
            {
                enableVisualEffects.boolValue = true;
            }
            
            serializedBrick.ApplyModifiedProperties();
            EditorUtility.SetDirty(brick);
            tuned++;
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"👁️ Tuned particle settings on {tuned} bricks for better visibility");
        Debug.Log("👁️ New settings: Size=0.3, Count=25, Lifetime=2.0s, Speed=4.0");
    }
    
    [MenuItem("Breakout/Debug/Test Real Brick Destruction")]
    public static void TestRealBrickDestruction()
    {
        Debug.Log("💥 Testing real brick destruction with particles...");
        
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
        
        Debug.Log($"💥 Destroying {testBrick.gameObject.name} to test particles...");
        
        // Use reflection to call ProcessDestruction method
        System.Reflection.MethodInfo processDestructionMethod = typeof(Brick).GetMethod("ProcessDestruction", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (processDestructionMethod != null)
        {
            try
            {
                processDestructionMethod.Invoke(testBrick, null);
                Debug.Log($"✅ Triggered real destruction on {testBrick.gameObject.name} - watch for particles!");
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
}
#endif