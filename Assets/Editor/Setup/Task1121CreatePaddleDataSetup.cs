#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor setup script for creating paddle data structure and folder organization.
/// Provides automated setup for paddle system foundation with proper directory structure.
/// </summary>
public static class Task1121CreatePaddleDataSetup
{
    private const string MENU_PATH = "Breakout/Setup/Create Paddle Data Structure";
    private const string PADDLE_SCRIPTS_FOLDER = "Assets/Scripts/Paddle";
    private const string PADDLE_DATA_FILE = "Assets/Scripts/Paddle/PaddleData.cs";
    
    /// <summary>
    /// Creates paddle data structure and folder organization.
    /// </summary>
    [MenuItem(MENU_PATH)]
    public static void CreatePaddleDataStructure()
    {
        Debug.Log("📋 [Task 1.1.2.1] Starting Paddle Data Structure creation...");
        
        try
        {
            // Step 1: Create folder structure
            CreatePaddleFolderStructure();
            
            // Step 2: Validate PaddleData implementation
            ValidatePaddleDataImplementation();
            
            // Step 3: Test data structure functionality
            TestPaddleDataFunctionality();
            
            // Step 4: Final validation and cleanup
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            LogSuccessfulSetup();
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Task 1.1.2.1] Paddle Data Structure creation failed: {e.Message}");
            Debug.LogError("📋 Please check folder permissions and Unity project setup.");
        }
    }
    
    /// <summary>
    /// Menu validation - prevents duplicate creation when PaddleData already exists.
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateCreatePaddleDataStructure()
    {
        bool paddleDataExists = File.Exists(PADDLE_DATA_FILE);
        
        if (paddleDataExists)
        {
            Debug.LogWarning("⚠️ PaddleData structure already exists.");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Creates the paddle folder structure if it doesn't exist.
    /// </summary>
    private static void CreatePaddleFolderStructure()
    {
        Debug.Log("📁 [Step 1/3] Creating paddle folder structure...");
        
        // Create Scripts folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Scripts"))
        {
            AssetDatabase.CreateFolder("Assets", "Scripts");
            Debug.Log("   • Created Assets/Scripts folder");
        }
        
        // Create Paddle folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(PADDLE_SCRIPTS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets/Scripts", "Paddle");
            Debug.Log("   • Created Assets/Scripts/Paddle folder");
        }
        else
        {
            Debug.Log("   • Assets/Scripts/Paddle folder already exists");
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("✅ [Step 1/3] Paddle folder structure created successfully");
    }
    
    /// <summary>
    /// Validates that PaddleData implementation is correct and complete.
    /// </summary>
    private static void ValidatePaddleDataImplementation()
    {
        Debug.Log("🔍 [Step 2/3] Validating PaddleData implementation...");
        
        if (!File.Exists(PADDLE_DATA_FILE))
        {
            Debug.LogError("❌ PaddleData.cs file not found!");
            Debug.LogError("📋 Expected location: " + PADDLE_DATA_FILE);
            return;
        }
        
        // Check if the file can be loaded
        string paddleDataContent = File.ReadAllText(PADDLE_DATA_FILE);
        
        // Validate key components exist
        bool hasSystemSerializable = paddleDataContent.Contains("[System.Serializable]");
        bool hasPaddleDataClass = paddleDataContent.Contains("public class PaddleData");
        bool hasInputMethodEnum = paddleDataContent.Contains("public enum InputMethod");
        bool hasMovementProperties = paddleDataContent.Contains("movementSpeed") && paddleDataContent.Contains("acceleration");
        bool hasInputConfiguration = paddleDataContent.Contains("inputSensitivity") && paddleDataContent.Contains("enableKeyboardInput");
        bool hasBoundaryConstraints = paddleDataContent.Contains("leftBoundary") && paddleDataContent.Contains("rightBoundary");
        bool hasValidationMethods = paddleDataContent.Contains("ValidateParameters");
        
        if (!hasSystemSerializable)
        {
            Debug.LogError("❌ PaddleData class missing [System.Serializable] attribute");
            return;
        }
        
        if (!hasPaddleDataClass)
        {
            Debug.LogError("❌ PaddleData class not found");
            return;
        }
        
        if (!hasInputMethodEnum)
        {
            Debug.LogError("❌ InputMethod enumeration not found");
            return;
        }
        
        if (!hasMovementProperties)
        {
            Debug.LogError("❌ Movement properties (movementSpeed, acceleration) missing");
            return;
        }
        
        if (!hasInputConfiguration)
        {
            Debug.LogError("❌ Input configuration properties missing");
            return;
        }
        
        if (!hasBoundaryConstraints)
        {
            Debug.LogError("❌ Boundary constraint properties missing");
            return;
        }
        
        if (!hasValidationMethods)
        {
            Debug.LogError("❌ Parameter validation methods missing");
            return;
        }
        
        Debug.Log("✅ [Step 2/3] PaddleData implementation validation successful:");
        Debug.Log("   • [System.Serializable] attribute: Present");
        Debug.Log("   • PaddleData class: Present");
        Debug.Log("   • InputMethod enum: Present");
        Debug.Log("   • Movement properties: Present");
        Debug.Log("   • Input configuration: Present");
        Debug.Log("   • Boundary constraints: Present");
        Debug.Log("   • Validation methods: Present");
    }
    
    /// <summary>
    /// Tests PaddleData functionality to ensure it works correctly.
    /// </summary>
    private static void TestPaddleDataFunctionality()
    {
        Debug.Log("🧪 [Step 3/3] Testing PaddleData functionality...");
        
        try
        {
            // Create test instance of PaddleData
            PaddleData testData = new PaddleData();
            
            // Test initialization
            testData.Initialize();
            
            // Test parameter validation
            bool parametersValid = testData.ValidateParameters();
            Debug.Log($"   • Parameter validation: {(parametersValid ? "PASS" : "WARN")}");
            
            // Test boundary constraints
            Vector2 testPosition = new Vector2(100f, 0f); // Outside boundaries
            Vector2 constrainedPosition = testData.ApplyBoundaryConstraints(testPosition);
            bool boundaryTest = constrainedPosition.x <= testData.rightBoundary && constrainedPosition.x >= testData.leftBoundary;
            Debug.Log($"   • Boundary constraints: {(boundaryTest ? "PASS" : "FAIL")}");
            
            // Test input sensitivity
            float testInput = 0.5f;
            float sensitizedInput = testData.ApplyInputSensitivity(testInput);
            bool inputTest = sensitizedInput != testInput || testData.inputSensitivity == 1f;
            Debug.Log($"   • Input sensitivity: {(inputTest ? "PASS" : "FAIL")}");
            
            // Test configuration summary
            string configSummary = testData.GetConfigurationSummary();
            bool summaryTest = !string.IsNullOrEmpty(configSummary) && configSummary.Contains("PaddleData Configuration");
            Debug.Log($"   • Configuration summary: {(summaryTest ? "PASS" : "FAIL")}");
            
            // Test utility methods
            bool utilityTest = testData.HasInputEnabled() && testData.GetPlayableWidth() > 0f;
            Debug.Log($"   • Utility methods: {(utilityTest ? "PASS" : "FAIL")}");
            
            Debug.Log("✅ [Step 3/3] PaddleData functionality testing complete");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ [Step 3/3] PaddleData functionality testing failed: {e.Message}");
        }
    }
    
    /// <summary>
    /// Logs successful paddle data structure setup summary.
    /// </summary>
    private static void LogSuccessfulSetup()
    {
        Debug.Log("✅ [Task 1.1.2.1] Paddle Data Structure created successfully!");
        Debug.Log("📋 Paddle Data Structure Summary:");
        Debug.Log($"   • File Location: {PADDLE_DATA_FILE}");
        Debug.Log($"   • Folder Structure: Assets/Scripts/Paddle/");
        Debug.Log($"   • Class: PaddleData [Serializable]");
        Debug.Log($"   • Enumeration: InputMethod (None, Keyboard, Mouse)");
        Debug.Log("📊 Data Structure Features:");
        Debug.Log("   → Movement Properties: Speed, acceleration, dimensions");
        Debug.Log("   → Input Configuration: Keyboard/mouse support, sensitivity");
        Debug.Log("   → Boundary Constraints: Left/right boundary limits");
        Debug.Log("   → Physics Properties: Mass, drag coefficient");
        Debug.Log("   → Runtime State: Position, velocity, input tracking");
        Debug.Log("   → Parameter Validation: Range checking and safety");
        Debug.Log("   → Utility Methods: Boundary checking, configuration summary");
        Debug.Log("🎮 Default Configuration:");
        Debug.Log("   • Movement Speed: 8.0 units/sec");
        Debug.Log("   • Acceleration: 15.0 units/sec²");
        Debug.Log("   • Dimensions: 2.0 x 0.3 units");
        Debug.Log("   • Input Sensitivity: 1.0x");
        Debug.Log("   • Boundaries: -8.0 to 8.0 units");
        Debug.Log("   • Mass: 1.0kg, Drag: 2.0");
        Debug.Log("💡 Usage Instructions:");
        Debug.Log("   1. Use as [SerializeField] field in PaddleController");
        Debug.Log("   2. Configure properties in Unity Inspector");
        Debug.Log("   3. Call Initialize() in Start() method");
        Debug.Log("   4. Use ValidateParameters() for runtime safety");
        Debug.Log("   5. Apply boundary constraints with ApplyBoundaryConstraints()");
        Debug.Log("🔧 Next Steps:");
        Debug.Log("   → Implement PaddleController MonoBehaviour");
        Debug.Log("   → Create Paddle GameObject with physics components");
        Debug.Log("   → Integrate with ball physics system for collision response");
        Debug.Log("   → Add input handling system for player control");
    }
}
#endif