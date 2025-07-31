#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Runtime testing component for BrickGrid Manager in Play mode.
/// Provides UI buttons for testing framework methods when properly initialized.
/// </summary>
[CustomEditor(typeof(BrickGrid))]
public class BrickGridRuntimeTester : Editor
{
    private BrickGrid brickGrid;
    
    private void OnEnable()
    {
        brickGrid = (BrickGrid)target;
    }
    
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Testing", EditorStyles.boldLabel);
        
        // Show initialization status
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Initialized:", GUILayout.Width(80));
        EditorGUILayout.LabelField(brickGrid.IsInitialized ? "✅ Yes" : "❌ No");
        EditorGUILayout.EndHorizontal();
        
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to test framework methods", MessageType.Info);
            return;
        }
        
        if (!brickGrid.IsInitialized)
        {
            EditorGUILayout.HelpBox("BrickGrid is not initialized. Check console for errors.", MessageType.Warning);
            return;
        }
        
        EditorGUILayout.Space();
        
        // Framework method testing
        EditorGUILayout.LabelField("Framework Methods", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Grid"))
        {
            Debug.Log("🏗️ [Inspector] Testing GenerateGrid()...");
            brickGrid.GenerateGrid();
        }
        
        if (GUILayout.Button("Clear Grid"))
        {
            Debug.Log("🧹 [Inspector] Testing ClearGrid()...");
            brickGrid.ClearGrid();
        }
        
        if (GUILayout.Button("Validate Grid"))
        {
            Debug.Log("🧪 [Inspector] Testing ValidateGrid()...");
            bool result = brickGrid.ValidateGrid();
            Debug.Log($"   • ValidateGrid() returned: {result}");
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // State management testing
        EditorGUILayout.LabelField("State Management", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset State"))
        {
            Debug.Log("🔄 [Inspector] Resetting grid state...");
            brickGrid.ResetGridState();
        }
        
        if (GUILayout.Button("Set Generated (10)"))
        {
            Debug.Log("✅ [Inspector] Setting grid as generated with 10 bricks...");
            brickGrid.SetGridGenerated(10);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Update Count +1"))
        {
            int newCount = brickGrid.ActiveBrickCount + 1;
            Debug.Log($"🔢 [Inspector] Updating brick count to {newCount}...");
            brickGrid.UpdateBrickCount(newCount);
        }
        
        if (GUILayout.Button("Update Count -1"))
        {
            int newCount = Mathf.Max(0, brickGrid.ActiveBrickCount - 1);
            Debug.Log($"🔢 [Inspector] Updating brick count to {newCount}...");
            brickGrid.UpdateBrickCount(newCount);
        }
        
        if (GUILayout.Button("Set Count to 0"))
        {
            Debug.Log("🔢 [Inspector] Setting brick count to 0...");
            brickGrid.UpdateBrickCount(0);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Debug information
        EditorGUILayout.LabelField("Debug Information", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Show Debug Info"))
        {
            string debugInfo = brickGrid.GetGridDebugInfo();
            Debug.Log("📋 [Inspector] BrickGrid Debug Info:\n" + debugInfo);
        }
        
        // Current state display
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current State", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField($"Grid Generated: {(brickGrid.IsGridGenerated ? "✅" : "❌")}");
        EditorGUILayout.LabelField($"Active Bricks: {brickGrid.ActiveBrickCount}");
        EditorGUILayout.LabelField($"Is Complete: {(brickGrid.IsComplete ? "✅" : "❌")}");
        
        if (brickGrid.GridConfiguration != null)
        {
            EditorGUILayout.LabelField($"Configuration: {brickGrid.GridConfiguration.name}");
            EditorGUILayout.LabelField($"Pattern: {brickGrid.GridConfiguration.pattern}");
            EditorGUILayout.LabelField($"Grid Size: {brickGrid.GridDimensions.x:F1} × {brickGrid.GridDimensions.y:F1}");
        }
        else
        {
            EditorGUILayout.LabelField("Configuration: None (Assign GridData)");
        }
        EditorGUILayout.EndVertical();
        
        // Repaint in play mode for live updates
        if (Application.isPlaying && brickGrid.IsInitialized)
        {
            Repaint();
        }
    }
}
#endif