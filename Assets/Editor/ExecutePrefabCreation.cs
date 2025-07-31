#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Simple executor to run the prefab creation script and report results
/// </summary>
public static class ExecutePrefabCreation
{
    [MenuItem("Breakout/Debug/Execute Prefab Creation Now")]
    public static void ExecuteNow()
    {
        Debug.Log("🚀 Executing Brick Prefab Creation...");
        
        try
        {
            // Call the prefab creation method directly
            Task1217CreateBrickPrefabSetup.CreateBrickPrefab();
            Debug.Log("✅ Prefab creation completed successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Prefab creation failed: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
    }
    
    [MenuItem("Breakout/Debug/Check Prefab Exists")]
    public static void CheckPrefabExists()
    {
        string prefabPath = "Assets/Prefabs/Gameplay/Brick.prefab";
        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefabAsset != null)
        {
            Debug.Log($"✅ Brick prefab found at: {prefabPath}");
            
            // Check components
            Brick brickScript = prefabAsset.GetComponent<Brick>();
            SpriteRenderer spriteRenderer = prefabAsset.GetComponent<SpriteRenderer>();
            BoxCollider2D boxCollider = prefabAsset.GetComponent<BoxCollider2D>();
            AudioSource audioSource = prefabAsset.GetComponent<AudioSource>();
            ParticleSystem particleSystem = prefabAsset.GetComponentInChildren<ParticleSystem>();
            
            Debug.Log($"   • Brick script: {(brickScript != null ? "✅" : "❌")}");
            Debug.Log($"   • SpriteRenderer: {(spriteRenderer != null ? "✅" : "❌")}");
            Debug.Log($"   • BoxCollider2D: {(boxCollider != null ? "✅" : "❌")}");
            Debug.Log($"   • AudioSource: {(audioSource != null ? "✅" : "❌")}");
            Debug.Log($"   • ParticleSystem: {(particleSystem != null ? "✅" : "❌")}");
            
            if (brickScript != null && spriteRenderer != null && boxCollider != null && 
                audioSource != null && particleSystem != null)
            {
                Debug.Log("🎉 All required components present in prefab!");
            }
        }
        else
        {
            Debug.LogWarning($"❌ Brick prefab not found at: {prefabPath}");
            Debug.Log("📋 Run 'Execute Prefab Creation Now' to create it");
        }
    }
}
#endif