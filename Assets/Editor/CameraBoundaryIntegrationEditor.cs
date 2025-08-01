#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for CameraBoundaryIntegration component.
/// Adds manual update buttons and debug information display.
/// </summary>
[CustomEditor(typeof(CameraBoundaryIntegration))]
public class CameraBoundaryIntegrationEditor : Editor
{
    private CameraBoundaryIntegration integration;
    
    private void OnEnable()
    {
        integration = (CameraBoundaryIntegration)target;
    }
    
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();
        
        GUILayout.Space(10);
        
        // Manual update buttons
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Force Update Bounds"))
        {
            integration.CalculateCameraBounds();
            EditorUtility.SetDirty(integration);
            Debug.Log("[CameraBoundaryIntegration] Manual bounds calculation triggered");
        }
        
        if (GUILayout.Button("Update Positions"))
        {
            integration.UpdateBoundaryPositions();
            EditorUtility.SetDirty(integration);
            Debug.Log("[CameraBoundaryIntegration] Manual position update triggered");
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Force Complete Update"))
        {
            integration.ForceUpdate();
            EditorUtility.SetDirty(integration);
            Debug.Log("[CameraBoundaryIntegration] Complete force update triggered");
        }
        
        if (GUILayout.Button("Verify Alignment"))
        {
            bool aligned = integration.VerifyBoundaryAlignment();
            string message = aligned ? "✅ All boundaries aligned" : "❌ Some boundaries misaligned";
            Debug.Log($"[CameraBoundaryIntegration] {message}");
            
            // Show alignment status
            var status = integration.GetAlignmentStatus();
            foreach (var kvp in status)
            {
                string statusText = kvp.Value ? "✅" : "❌";
                Debug.Log($"   {kvp.Key}: {statusText}");
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        // Status information
        EditorGUILayout.LabelField("Status Information", EditorStyles.boldLabel);
        
        if (integration.gameCamera != null)
        {
            EditorGUILayout.LabelField("Camera", integration.gameCamera.name);
            EditorGUILayout.LabelField("Camera Type", integration.gameCamera.orthographic ? "Orthographic" : "Perspective");
            
            if (integration.gameCamera.orthographic)
            {
                EditorGUILayout.LabelField("Orthographic Size", integration.gameCamera.orthographicSize.ToString("F2"));
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No camera assigned", MessageType.Warning);
        }
        
        // Display current bounds if calculated
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("Camera World Size", integration.GetCameraWorldSize().ToString());
            EditorGUILayout.LabelField("Camera Aspect Ratio", integration.GetCameraAspectRatio().ToString("F2"));
            EditorGUILayout.LabelField("World Min", integration.cameraWorldMin.ToString());
            EditorGUILayout.LabelField("World Max", integration.cameraWorldMax.ToString());
        }
        
        GUILayout.Space(10);
        
        // Help box with instructions
        EditorGUILayout.HelpBox(
            "Use 'Force Complete Update' to manually recalculate camera bounds and reposition all boundary walls. " +
            "Enable debug visualization to see camera bounds (yellow) and alignment markers (cyan) in Scene view.",
            MessageType.Info);
        
        // Show integration summary button
        if (GUILayout.Button("Log Integration Summary"))
        {
            Debug.Log(integration.GetIntegrationSummary());
        }
    }
}
#endif