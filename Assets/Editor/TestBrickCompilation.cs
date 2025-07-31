#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class TestBrickCompilation
{
    [MenuItem("Debug/Test Brick Compilation")]
    public static void TestBrickCompilation_Menu()
    {
        Debug.Log("=== Testing Brick Compilation ===");
        
        // Test 1: Can we reference the type?
        System.Type brickType = typeof(Brick);
        Debug.Log($"Brick type found: {brickType != null}");
        if (brickType != null)
        {
            Debug.Log($"Brick type name: {brickType.FullName}");
            Debug.Log($"Is MonoBehaviour: {typeof(MonoBehaviour).IsAssignableFrom(brickType)}");
        }
        
        // Test 2: Can we reference BrickData?
        System.Type brickDataType = typeof(BrickData);
        Debug.Log($"BrickData type found: {brickDataType != null}");
        
        // Test 3: Can we create BrickData?
        try
        {
            BrickData testData = new BrickData(BrickType.Normal);
            Debug.Log($"BrickData creation successful: {testData != null}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"BrickData creation failed: {e.Message}");
        }
        
        // Test 4: Try creating GameObject and adding component
        GameObject testObj = new GameObject("TestBrick");
        Debug.Log("Created test GameObject");
        
        try
        {
            Component comp = testObj.AddComponent(brickType);
            Debug.Log($"AddComponent result: {comp != null}");
            if (comp != null)
            {
                Debug.Log($"Component type: {comp.GetType().Name}");
                Brick brick = comp as Brick;
                Debug.Log($"Cast to Brick successful: {brick != null}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AddComponent failed: {e.Message}");
        }
        finally
        {
            GameObject.DestroyImmediate(testObj);
        }
    }
}
#endif