#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Test script for manually triggering particle effects on bricks
/// </summary>
public static class TestParticleEffects
{
    [MenuItem("Breakout/Debug/Test Particle Effects")]
    public static void TestBrickParticleEffects()
    {
        Debug.Log("🧪 Testing particle effects on all bricks...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        if (bricks.Length == 0)
        {
            Debug.LogWarning("No bricks found in scene! Please create some test bricks first.");
            return;
        }
        
        foreach (Brick brick in bricks)
        {
            Debug.Log($"🎯 Testing particle effects on {brick.gameObject.name}");
            
            // Reset the effectsTriggered flag using reflection
            System.Reflection.FieldInfo effectsTriggeredField = typeof(Brick).GetField("effectsTriggered", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (effectsTriggeredField != null)
            {
                effectsTriggeredField.SetValue(brick, false);
                Debug.Log($"🔄 Reset effectsTriggered flag for {brick.gameObject.name}");
            }
            
            // Use reflection to call private method for testing
            System.Reflection.MethodInfo triggerMethod = typeof(Brick).GetMethod("TriggerDestructionEffects", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (triggerMethod != null)
            {
                try
                {
                    triggerMethod.Invoke(brick, null);
                    Debug.Log($"✅ Triggered particle effects on {brick.gameObject.name}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Failed to trigger effects on {brick.gameObject.name}: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("TriggerDestructionEffects method not found!");
            }
        }
        
        Debug.Log("🧪 Particle effects test completed");
    }
    
    [MenuItem("Breakout/Debug/Force Create Particle Systems")]
    public static void ForceCreateParticleSystems()
    {
        Debug.Log("🔧 Force creating particle systems on all bricks...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        int created = 0;
        
        foreach (Brick brick in bricks)
        {
            ParticleSystem existing = brick.GetComponentInChildren<ParticleSystem>();
            if (existing == null)
            {
                // Create particle system
                GameObject particleChild = new GameObject("TestDestructionParticles");
                particleChild.transform.SetParent(brick.transform);
                particleChild.transform.localPosition = Vector3.zero;
                
                ParticleSystem ps = particleChild.AddComponent<ParticleSystem>();
                
                // Configure for visibility
                var main = ps.main;
                main.startLifetime = 2.0f;
                main.startSpeed = 5.0f;
                main.startSize = 0.2f;
                main.startColor = Color.red;
                main.maxParticles = 30;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                main.loop = false;
                
                var emission = ps.emission;
                emission.enabled = false;
                
                var shape = ps.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = 1.0f;
                
                // Link to brick
                SerializedObject serializedBrick = new SerializedObject(brick);
                SerializedProperty destructionParticles = serializedBrick.FindProperty("destructionParticles");
                if (destructionParticles != null)
                {
                    destructionParticles.objectReferenceValue = ps;
                    serializedBrick.ApplyModifiedProperties();
                }
                
                created++;
                Debug.Log($"✅ Created ParticleSystem for {brick.gameObject.name}");
            }
            else
            {
                Debug.Log($"ℹ️ {brick.gameObject.name} already has ParticleSystem");
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"🔧 Created {created} new particle systems");
    }
    
    [MenuItem("Breakout/Debug/Manual Particle Burst")]
    public static void ManualParticleBurst()
    {
        Debug.Log("💥 Manual particle burst test...");
        
        Brick[] bricks = GameObject.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        
        foreach (Brick brick in bricks)
        {
            ParticleSystem ps = brick.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                Debug.Log($"💥 Emitting BIG visible particles on {brick.gameObject.name} at {brick.transform.position}");
                
                // Configure for maximum visibility
                var main = ps.main;
                main.startColor = Color.red; // Bright red
                main.startLifetime = 10.0f; // Very long lifetime
                main.startSpeed = 0.5f; // Very slow speed
                main.startSize = 2.0f; // Very large size
                main.maxParticles = 100;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                
                var emission = ps.emission;
                emission.enabled = false; // Ensure manual emission
                
                var shape = ps.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = 2.0f; // Large radius
                
                // Stop any existing playback
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                
                // Emit many particles
                ps.Emit(50);
                ps.Play();
                
                Debug.Log($"✅ Emitted 50 BIG RED particles on {brick.gameObject.name}");
                Debug.Log($"   Position: {ps.transform.position}");
                Debug.Log($"   Particle Count: {ps.particleCount}");
                Debug.Log($"   Is Playing: {ps.isPlaying}");
                Debug.Log($"   Main Module - Color: {main.startColor.color}, Size: {main.startSize.constant}, Speed: {main.startSpeed.constant}");
            }
            else
            {
                Debug.LogWarning($"⚠️ No ParticleSystem found on {brick.gameObject.name}");
            }
        }
        
        Debug.Log("💥 Manual particle burst completed - check Scene view for BIG RED particles!");
    }
    
    [MenuItem("Breakout/Debug/Test Camera Particles")]
    public static void TestCameraParticles()
    {
        Debug.Log("📹 Creating particle system at camera position for visibility test...");
        
        // Get main camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindFirstObjectByType<Camera>();
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("No camera found in scene!");
            return;
        }
        
        // Create temporary GameObject in front of camera
        Vector3 testPosition = mainCamera.transform.position + mainCamera.transform.forward * 5f;
        GameObject testObject = new GameObject("CameraParticleTest");
        testObject.transform.position = testPosition;
        
        // Add ParticleSystem
        ParticleSystem ps = testObject.AddComponent<ParticleSystem>();
        
        // Configure for maximum visibility
        var main = ps.main;
        main.startColor = Color.magenta; // Bright magenta
        main.startLifetime = 10.0f; // Very long lifetime
        main.startSpeed = 1.0f; // Very slow
        main.startSize = 1.0f; // Very large
        main.maxParticles = 100;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = true; // Keep emitting
        
        var emission = ps.emission;
        emission.enabled = true; // Continuous emission
        emission.rateOverTime = 10; // 10 particles per second
        
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 2.0f;
        
        ps.Play();
        
        Debug.Log($"📹 Created test particle system at {testPosition}");
        Debug.Log($"📹 Camera position: {mainCamera.transform.position}");
        Debug.Log($"📹 Look for BRIGHT MAGENTA particles in front of camera!");
        Debug.Log($"📹 Delete 'CameraParticleTest' GameObject when done testing");
        
        // Select the object so user can see it in hierarchy
        UnityEditor.Selection.activeGameObject = testObject;
    }
}
#endif