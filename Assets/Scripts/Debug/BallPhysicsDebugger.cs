using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Comprehensive physics debugging toolkit providing real-time monitoring and validation for ball physics system.
/// Displays physics data, monitors performance, and detects anomalies to ensure 60fps target achievement.
/// </summary>
public class BallPhysicsDebugger : MonoBehaviour
{
    [Header("Debug Display Settings")]
    [SerializeField] private bool enableDebugDisplay = true;
    [SerializeField] private bool enablePerformanceMonitoring = true;
    [SerializeField] private bool enableAnomalyDetection = true;
    [SerializeField] private bool enableVisualDebugAids = true;
    
    [Header("Component References")]
    [SerializeField] private BallController ballController;
    [SerializeField] private Canvas debugCanvas;
    
    [Header("Debug Data")]
    [SerializeField] private Vector2 currentVelocity;
    [SerializeField] private float currentSpeed;
    [SerializeField] private int collisionCount;
    [SerializeField] private float frameRate;
    [SerializeField] private BallLaunchState currentLaunchState;
    
    [Header("Performance Monitoring")]
    [SerializeField] private float averageFrameRate;
    [SerializeField] private float minFrameRate = 60f;
    [SerializeField] private float maxFrameRate;
    [SerializeField] private bool performanceWarning;
    
    [Header("Anomaly Detection")]
    [SerializeField] private bool ballStuckDetected;
    [SerializeField] private bool extremeSpeedDetected;
    [SerializeField] private float lastUpdateTime;
    
    // Performance tracking
    private Queue<float> frameTimeHistory = new Queue<float>();
    private const int FRAME_HISTORY_SIZE = 60; // 1 second at 60fps
    private float performanceUpdateTimer;
    private const float PERFORMANCE_UPDATE_INTERVAL = 0.5f;
    
    // Anomaly detection
    private Vector3 lastPosition;
    private float stuckDetectionTimer;
    private const float STUCK_THRESHOLD = 0.1f;
    private const float STUCK_TIME_LIMIT = 2f;
    private const float EXTREME_SPEED_THRESHOLD = 20f;
    
    // Visual debugging
    private Vector3[] velocityHistory = new Vector3[10];
    private int velocityHistoryIndex = 0;
    
    // Component validation
    private bool componentsValid = false;
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize debugging system and validate component references.
    /// </summary>
    private void Start()
    {
        ValidateComponents();
        InitializeDebugging();
        
        if (enableDebugDisplay)
        {
            Debug.Log("[PhysicsDebugger] Physics debugging system initialized");
        }
    }
    
    /// <summary>
    /// Real-time data collection and display update.
    /// </summary>
    private void Update()
    {
        if (!componentsValid) return;
        
        // Update physics data
        UpdatePhysicsData();
        
        // Monitor performance
        if (enablePerformanceMonitoring)
        {
            MonitorPerformance();
        }
        
        // Detect anomalies
        if (enableAnomalyDetection)
        {
            DetectAnomalies();
        }
        
        // Update visual debug aids
        if (enableVisualDebugAids)
        {
            UpdateVisualDebugData();
        }
        
        lastUpdateTime = Time.time;
    }
    
    /// <summary>
    /// Debug information display overlay using immediate mode GUI.
    /// </summary>
    private void OnGUI()
    {
        if (!enableDebugDisplay || !componentsValid) return;
        
        // Null check for GUI skin
        if (GUI.skin == null) return;
        
        // Set up GUI style with null checks
        GUIStyle debugStyle = new GUIStyle(GUI.skin.box);
        if (debugStyle == null) return;
        
        debugStyle.alignment = TextAnchor.UpperLeft;
        debugStyle.fontSize = 12;
        debugStyle.normal.textColor = Color.white;
        
        // Calculate display area
        float displayWidth = 300f;
        float displayHeight = 400f;
        Rect displayRect = new Rect(10f, 10f, displayWidth, displayHeight);
        
        // Draw debug information panel
        GUI.Box(displayRect, "", debugStyle);
        
        // Create content string
        string debugContent = BuildDebugContent();
        
        // Display debug information
        Rect contentRect = new Rect(displayRect.x + 10f, displayRect.y + 10f, 
                                   displayRect.width - 20f, displayRect.height - 20f);
        GUI.Label(contentRect, debugContent, debugStyle);
        
        // Draw performance warning if needed
        if (performanceWarning)
        {
            DrawPerformanceWarning();
        }
    }
    
    #endregion
    
    #region Component Management
    
    /// <summary>
    /// Validates required component references for debugging functionality.
    /// </summary>
    private void ValidateComponents()
    {
        // Find BallController if not assigned
        if (ballController == null)
        {
            ballController = FindObjectOfType<BallController>();
        }
        
        // Validate BallController exists
        if (ballController == null)
        {
            Debug.LogError("[PhysicsDebugger] BallController component not found! Physics debugging disabled.");
            componentsValid = false;
            return;
        }
        
        // Find Canvas if not assigned
        if (debugCanvas == null)
        {
            debugCanvas = FindObjectOfType<Canvas>();
        }
        
        componentsValid = true;
        Debug.Log("[PhysicsDebugger] Component validation successful");
    }
    
    /// <summary>
    /// Initializes debugging system with default settings.
    /// </summary>
    private void InitializeDebugging()
    {
        // Initialize performance tracking
        frameTimeHistory.Clear();
        performanceUpdateTimer = 0f;
        
        // Initialize anomaly detection
        if (ballController != null)
        {
            lastPosition = ballController.transform.position;
        }
        stuckDetectionTimer = 0f;
        
        // Initialize visual debugging
        for (int i = 0; i < velocityHistory.Length; i++)
        {
            velocityHistory[i] = Vector3.zero;
        }
        velocityHistoryIndex = 0;
        
        Debug.Log("[PhysicsDebugger] Debugging initialization complete");
    }
    
    #endregion
    
    #region Physics Data Monitoring
    
    /// <summary>
    /// Updates real-time physics data from BallController.
    /// </summary>
    private void UpdatePhysicsData()
    {
        if (ballController == null) return;
        
        // Update basic physics data
        currentVelocity = ballController.GetCurrentVelocity();
        currentSpeed = ballController.GetCurrentSpeed();
        currentLaunchState = ballController.GetLaunchState();
        
        // Update collision count if BallData is available
        BallData ballData = ballController.GetBallData();
        if (ballData != null)
        {
            collisionCount = ballData.collisionCount;
        }
        
        // Update frame rate
        frameRate = 1f / Time.unscaledDeltaTime;
    }
    
    /// <summary>
    /// Monitors performance metrics and maintains frame rate history.
    /// </summary>
    private void MonitorPerformance()
    {
        // Update frame time history
        frameTimeHistory.Enqueue(Time.unscaledDeltaTime);
        if (frameTimeHistory.Count > FRAME_HISTORY_SIZE)
        {
            frameTimeHistory.Dequeue();
        }
        
        // Update performance metrics periodically
        performanceUpdateTimer += Time.unscaledDeltaTime;
        if (performanceUpdateTimer >= PERFORMANCE_UPDATE_INTERVAL)
        {
            CalculatePerformanceMetrics();
            performanceUpdateTimer = 0f;
        }
    }
    
    /// <summary>
    /// Calculates performance metrics from frame time history.
    /// </summary>
    private void CalculatePerformanceMetrics()
    {
        if (frameTimeHistory.Count == 0) return;
        
        float totalFrameTime = 0f;
        float minFrameTime = float.MaxValue;
        float maxFrameTime = 0f;
        
        foreach (float frameTime in frameTimeHistory)
        {
            totalFrameTime += frameTime;
            if (frameTime < minFrameTime) minFrameTime = frameTime;
            if (frameTime > maxFrameTime) maxFrameTime = frameTime;
        }
        
        // Calculate metrics
        averageFrameRate = frameTimeHistory.Count / totalFrameTime;
        minFrameRate = 1f / maxFrameTime;
        maxFrameRate = 1f / minFrameTime;
        
        // Check for performance warnings
        performanceWarning = averageFrameRate < 55f; // 5fps buffer below 60fps target
        
        if (performanceWarning)
        {
            Debug.LogWarning($"[PhysicsDebugger] Performance warning: Average FPS {averageFrameRate:F1} below 60fps target");
        }
    }
    
    #endregion
    
    #region Anomaly Detection
    
    /// <summary>
    /// Detects physics anomalies including stuck ball and extreme speeds.
    /// </summary>
    private void DetectAnomalies()
    {
        if (ballController == null) return;
        
        Vector3 currentPosition = ballController.transform.position;
        
        // Detect stuck ball
        DetectStuckBall(currentPosition);
        
        // Detect extreme speeds
        DetectExtremeSpeed();
        
        // Update last position for next frame
        lastPosition = currentPosition;
    }
    
    /// <summary>
    /// Detects if ball is stuck by monitoring position changes.
    /// </summary>
    /// <param name="currentPosition">Current ball position</param>
    private void DetectStuckBall(Vector3 currentPosition)
    {
        float distanceMoved = Vector3.Distance(currentPosition, lastPosition);
        
        if (distanceMoved < STUCK_THRESHOLD && ballController.IsMoving())
        {
            stuckDetectionTimer += Time.unscaledDeltaTime;
            
            if (stuckDetectionTimer >= STUCK_TIME_LIMIT && !ballStuckDetected)
            {
                ballStuckDetected = true;
                LogPhysicsEvent("ANOMALY", $"Stuck ball detected: Position={currentPosition}, Speed={currentSpeed:F2}");
            }
        }
        else
        {
            if (ballStuckDetected && distanceMoved >= STUCK_THRESHOLD)
            {
                ballStuckDetected = false;
                LogPhysicsEvent("RECOVERY", "Ball unstuck - normal movement resumed");
            }
            stuckDetectionTimer = 0f;
        }
    }
    
    /// <summary>
    /// Detects extreme ball speeds that may indicate physics issues.
    /// </summary>
    private void DetectExtremeSpeed()
    {
        bool currentExtremeSpeed = currentSpeed > EXTREME_SPEED_THRESHOLD;
        
        if (currentExtremeSpeed && !extremeSpeedDetected)
        {
            extremeSpeedDetected = true;
            LogPhysicsEvent("ANOMALY", $"Extreme speed detected: {currentSpeed:F2} units/sec (threshold: {EXTREME_SPEED_THRESHOLD})");
        }
        else if (!currentExtremeSpeed && extremeSpeedDetected)
        {
            extremeSpeedDetected = false;
            LogPhysicsEvent("RECOVERY", "Speed normalized - extreme speed resolved");
        }
    }
    
    #endregion
    
    #region Visual Debug Aids
    
    /// <summary>
    /// Updates visual debugging data for Gizmos display.
    /// </summary>
    private void UpdateVisualDebugData()
    {
        if (ballController == null) return;
        
        // Update velocity history for trajectory visualization
        velocityHistory[velocityHistoryIndex] = currentVelocity;
        velocityHistoryIndex = (velocityHistoryIndex + 1) % velocityHistory.Length;
    }
    
    /// <summary>
    /// Draws visual debugging aids in Scene view using Gizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!enableVisualDebugAids || ballController == null) return;
        
        Vector3 ballPosition = ballController.transform.position;
        
        // Draw velocity vector
        DrawVelocityVector(ballPosition);
        
        // Draw collision bounds
        DrawCollisionBounds(ballPosition);
        
        // Draw trajectory prediction
        DrawTrajectoryPrediction(ballPosition);
        
        // Draw anomaly indicators
        DrawAnomalyIndicators(ballPosition);
    }
    
    /// <summary>
    /// Draws current velocity vector as a colored line.
    /// </summary>
    /// <param name="position">Ball position</param>
    private void DrawVelocityVector(Vector3 position)
    {
        if (currentVelocity.magnitude < 0.1f) return;
        
        // Color based on speed
        Color velocityColor = Color.Lerp(Color.green, Color.red, currentSpeed / 15f);
        Gizmos.color = velocityColor;
        
        Vector3 velocityEnd = position + (Vector3)currentVelocity * 0.5f;
        Gizmos.DrawLine(position, velocityEnd);
        Gizmos.DrawWireSphere(velocityEnd, 0.1f);
    }
    
    /// <summary>
    /// Draws collision bounds and detection radius.
    /// </summary>
    /// <param name="position">Ball position</param>
    private void DrawCollisionBounds(Vector3 position)
    {
        Gizmos.color = Color.yellow;
        
        // Get ball radius from collider
        CircleCollider2D collider = ballController.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            Gizmos.DrawWireSphere(position, collider.radius);
        }
    }
    
    /// <summary>
    /// Draws predicted trajectory based on current velocity.
    /// </summary>
    /// <param name="position">Ball position</param>
    private void DrawTrajectoryPrediction(Vector3 position)
    {
        if (currentVelocity.magnitude < 0.1f) return;
        
        Gizmos.color = Color.cyan;
        
        // Draw predicted path
        Vector3 predictedPosition = position;
        Vector3 velocity = currentVelocity;
        
        for (int i = 0; i < 20; i++)
        {
            Vector3 nextPosition = predictedPosition + (Vector3)velocity * 0.1f;
            Gizmos.DrawLine(predictedPosition, nextPosition);
            predictedPosition = nextPosition;
        }
    }
    
    /// <summary>
    /// Draws visual indicators for detected anomalies.
    /// </summary>
    /// <param name="position">Ball position</param>
    private void DrawAnomalyIndicators(Vector3 position)
    {
        // Stuck ball indicator
        if (ballStuckDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(position, Vector3.one * 0.8f);
        }
        
        // Extreme speed indicator
        if (extremeSpeedDetected)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(position, 1f);
        }
    }
    
    #endregion
    
    #region Debug Information Display
    
    /// <summary>
    /// Builds comprehensive debug content string for GUI display.
    /// </summary>
    /// <returns>Formatted debug information string</returns>
    private string BuildDebugContent()
    {
        System.Text.StringBuilder content = new System.Text.StringBuilder();
        
        content.AppendLine("=== BALL PHYSICS DEBUG ===");
        content.AppendLine();
        
        // Physics state
        content.AppendLine("[PHYSICS STATE]");
        content.AppendLine($"Position: {ballController.transform.position}");
        content.AppendLine($"Velocity: {currentVelocity}");
        content.AppendLine($"Speed: {currentSpeed:F2} units/sec");
        content.AppendLine($"Launch State: {currentLaunchState}");
        content.AppendLine($"Collisions: {collisionCount}");
        content.AppendLine($"Moving: {ballController.IsMoving()}");
        content.AppendLine();
        
        // Performance metrics
        if (enablePerformanceMonitoring)
        {
            content.AppendLine("[PERFORMANCE]");
            content.AppendLine($"Current FPS: {frameRate:F1}");
            content.AppendLine($"Average FPS: {averageFrameRate:F1}");
            content.AppendLine($"Min FPS: {minFrameRate:F1}");
            content.AppendLine($"Max FPS: {maxFrameRate:F1}");
            content.AppendLine($"Target: 60 FPS");
            content.AppendLine();
        }
        
        // Anomaly detection
        if (enableAnomalyDetection)
        {
            content.AppendLine("[ANOMALY DETECTION]");
            content.AppendLine($"Ball Stuck: {(ballStuckDetected ? "YES" : "No")}");
            content.AppendLine($"Extreme Speed: {(extremeSpeedDetected ? "YES" : "No")}");
            content.AppendLine($"Stuck Timer: {stuckDetectionTimer:F1}s");
            content.AppendLine();
        }
        
        // System status
        content.AppendLine("[SYSTEM STATUS]");
        content.AppendLine($"Components Valid: {componentsValid}");
        content.AppendLine($"Debug Display: {enableDebugDisplay}");
        content.AppendLine($"Performance Mon: {enablePerformanceMonitoring}");
        content.AppendLine($"Anomaly Detection: {enableAnomalyDetection}");
        
        return content.ToString();
    }
    
    /// <summary>
    /// Draws performance warning overlay when FPS drops below target.
    /// </summary>
    private void DrawPerformanceWarning()
    {
        GUIStyle warningStyle = new GUIStyle(GUI.skin.box);
        warningStyle.normal.background = Texture2D.whiteTexture;
        warningStyle.normal.textColor = Color.red;
        warningStyle.fontSize = 14;
        warningStyle.fontStyle = FontStyle.Bold;
        warningStyle.alignment = TextAnchor.MiddleCenter;
        
        Rect warningRect = new Rect(Screen.width - 320f, 10f, 300f, 60f);
        
        GUI.color = new Color(1f, 0f, 0f, 0.8f);
        GUI.Box(warningRect, $"PERFORMANCE WARNING\nFPS: {averageFrameRate:F1} (Target: 60)", warningStyle);
        GUI.color = Color.white;
    }
    
    #endregion
    
    #region Logging System
    
    /// <summary>
    /// Logs categorized physics events with detailed information.
    /// </summary>
    /// <param name="eventType">Type of physics event</param>
    /// <param name="details">Detailed event information</param>
    private void LogPhysicsEvent(string eventType, string details)
    {
        string timestamp = Time.time.ToString("F2");
        string logMessage = $"[PhysicsDebugger][{eventType}][{timestamp}s] {details}";
        
        switch (eventType)
        {
            case "ANOMALY":
                Debug.LogWarning(logMessage);
                break;
            case "RECOVERY":
                Debug.Log(logMessage);
                break;
            case "PERFORMANCE":
                Debug.LogWarning(logMessage);
                break;
            default:
                Debug.Log(logMessage);
                break;
        }
    }
    
    #endregion
    
    #region Public Interface
    
    /// <summary>
    /// Sets the BallController reference for debugging monitoring.
    /// </summary>
    /// <param name="controller">BallController to monitor</param>
    public void SetBallController(BallController controller)
    {
        ballController = controller;
        ValidateComponents();
        
        if (componentsValid)
        {
            Debug.Log("[PhysicsDebugger] BallController reference updated successfully");
        }
    }
    
    /// <summary>
    /// Toggles debug display visibility.
    /// </summary>
    /// <param name="enabled">Whether debug display should be visible</param>
    public void SetDebugDisplayEnabled(bool enabled)
    {
        enableDebugDisplay = enabled;
        Debug.Log($"[PhysicsDebugger] Debug display {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Toggles performance monitoring.
    /// </summary>
    /// <param name="enabled">Whether performance monitoring should be active</param>
    public void SetPerformanceMonitoringEnabled(bool enabled)
    {
        enablePerformanceMonitoring = enabled;
        Debug.Log($"[PhysicsDebugger] Performance monitoring {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Toggles anomaly detection.
    /// </summary>
    /// <param name="enabled">Whether anomaly detection should be active</param>
    public void SetAnomalyDetectionEnabled(bool enabled)
    {
        enableAnomalyDetection = enabled;
        Debug.Log($"[PhysicsDebugger] Anomaly detection {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Gets comprehensive debug information string.
    /// </summary>
    /// <returns>Current debug information</returns>
    public string GetDebugInfo()
    {
        return BuildDebugContent();
    }
    
    #endregion
}