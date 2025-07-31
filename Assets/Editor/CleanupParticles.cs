#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CleanupParticles
{
    [MenuItem("Breakout/Debug/EMERGENCY: Stop All Particles")]
    public static void StopAllParticles()
    {
        Debug.Log("🛑 EMERGENCY: Stopping all particle systems...");
        
        // Find and stop all particle systems in scene
        ParticleSystem[] allParticles = GameObject.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        
        foreach (ParticleSystem ps in allParticles)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            Debug.Log($"🛑 Stopped particle system: {ps.gameObject.name}");
        }
        
        // Find and delete the camera test object
        GameObject cameraTest = GameObject.Find("CameraParticleTest");
        if (cameraTest != null)
        {
            GameObject.DestroyImmediate(cameraTest);
            Debug.Log("🛑 Deleted CameraParticleTest GameObject");
        }
        
        Debug.Log($"🛑 Stopped {allParticles.Length} particle systems");
    }
    
    [MenuItem("Breakout/Debug/Reset All Particle Systems")]
    public static void ResetAllParticles()
    {
        Debug.Log("🔄 Resetting all particle systems to safe defaults...");
        
        ParticleSystem[] allParticles = GameObject.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        
        foreach (ParticleSystem ps in allParticles)
        {
            // Stop and clear
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            // Reset to safe defaults
            var main = ps.main;
            main.loop = false;
            main.startLifetime = 1.0f;
            main.startSpeed = 5.0f;
            main.startSize = 0.1f;
            main.maxParticles = 20;
            
            var emission = ps.emission;
            emission.enabled = false;
            emission.rateOverTime = 0;
            
            Debug.Log($"🔄 Reset particle system: {ps.gameObject.name}");
        }
        
        Debug.Log($"🔄 Reset {allParticles.Length} particle systems");
    }
}
#endif