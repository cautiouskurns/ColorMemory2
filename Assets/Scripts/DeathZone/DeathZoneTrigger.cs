using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

/// <summary>
/// Death zone trigger detection system for Breakout gameplay.
/// Handles collision detection with ball objects and provides event notifications
/// for life management and feedback systems through loose coupling architecture.
/// </summary>
[System.Serializable]
public class DeathZoneTrigger : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Death zone configuration asset")]
    public DeathZoneConfig config;
    
    [Tooltip("Death zone positioning system reference")]
    public DeathZonePositioning positioning;
    
    [Header("Trigger Setup")]
    [Tooltip("BoxCollider2D component for trigger detection")]
    public Collider2D triggerCollider;
    
    [Tooltip("Size of trigger detection area")]
    public Vector2 triggerSize = new Vector2(20f, 2f);
    
    [Tooltip("Enable automatic trigger size updates from positioning system")]
    public bool autoUpdateTriggerSize = true;
    
    [Header("Ball Detection")]
    [Tooltip("Layer mask for ball objects")]
    public LayerMask ballLayer = 1;
    
    [Tooltip("Tag identifier for ball objects")]
    public string ballTag = "Ball";
    
    [Tooltip("Enable component-based ball detection as fallback")]
    public bool useComponentDetection = true;
    
    [Tooltip("Minimum velocity threshold for ball detection")]
    [Range(0f, 5f)]
    public float minimumVelocityThreshold = 0.1f;
    
    [Header("Events")]
    [Tooltip("Unity Event fired when ball enters death zone")]
    public UnityEvent OnBallEnterDeathZone;
    
    [Tooltip("Unity Event with GameObject parameter for ball reference")]
    public UnityEvent<GameObject> OnBallEnterDeathZoneWithBall;
    
    [Header("Detection Settings")]
    [Tooltip("Enable cooldown period to prevent multiple triggers")]
    public bool enableTriggerCooldown = true;
    
    [Tooltip("Cooldown duration between trigger detections")]
    [Range(0.1f, 2f)]
    public float triggerCooldownDuration = 0.5f;
    
    [Tooltip("Enable validation to prevent false positives")]
    public bool enableValidation = true;
    
    [Header("Debug")]
    [Tooltip("Show debug gizmos in Scene view")]
    public bool showDebugGizmos = true;
    
    [Tooltip("Enable debug logging")]
    public bool enableDebugLogging = false;
    
    [Tooltip("Show trigger area in Scene view")]
    [SerializeField] private bool showTriggerArea = true;
    
    [Tooltip("Debug gizmo color for trigger area")]
    [SerializeField] private Color debugGizmoColor = new Color(1f, 0f, 0f, 0.3f);
    
    // C# Events for loose coupling
    /// <summary>
    /// Event fired when ball enters death zone - provides GameObject reference
    /// </summary>
    public static event System.Action<GameObject> BallLostEvent;
    
    /// <summary>
    /// Event fired when death zone is triggered - simple notification
    /// </summary>
    public static event System.Action DeathZoneTriggeredEvent;
    
    // Internal state
    private bool isInitialized = false;
    private bool isTriggerOnCooldown = false;
    private Coroutine cooldownCoroutine;
    private int triggeredCount = 0;
    
    // Cached components
    private BoxCollider2D boxCollider;
    private Camera mainCamera;
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize trigger system and validate setup
    /// </summary>
    private void Awake()
    {
        InitializeTriggerSystem();
    }
    
    /// <summary>
    /// Complete trigger setup and validate configuration
    /// </summary>
    private void Start()
    {
        CompleteTriggerSetup();
        ValidateTriggerSetup();
    }
    
    /// <summary>
    /// Clean up any running coroutines
    /// </summary>
    private void OnDestroy()
    {
        CleanupTriggerSystem();
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// Initialize core trigger system components
    /// </summary>
    private void InitializeTriggerSystem()
    {
        // Find or create trigger collider
        InitializeTriggerCollider();
        
        // Load configuration if not assigned
        if (config == null)
        {
            config = Resources.Load<DeathZoneConfig>("DeathZoneConfig");
        }
        
        // Find positioning system if not assigned
        if (positioning == null)
        {
            positioning = GetComponentInParent<DeathZonePositioning>();
            if (positioning == null)
            {
                positioning = FindFirstObjectByType<DeathZonePositioning>();
            }
        }
        
        // Initialize events if null
        if (OnBallEnterDeathZone == null)
            OnBallEnterDeathZone = new UnityEvent();
        
        if (OnBallEnterDeathZoneWithBall == null)
            OnBallEnterDeathZoneWithBall = new UnityEvent<GameObject>();
        
        // Cache main camera reference
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
        
        isInitialized = true;
        LogDebug("[DeathZoneTrigger] Initialized trigger system components");
    }
    
    /// <summary>
    /// Initialize or find BoxCollider2D component for trigger detection
    /// </summary>
    private void InitializeTriggerCollider()
    {
        // Get existing collider or create new one
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            LogDebug("[DeathZoneTrigger] Created new BoxCollider2D component");
        }
        
        // Configure as trigger
        boxCollider.isTrigger = true;
        
        // Set trigger collider reference
        triggerCollider = boxCollider;
        
        // Update trigger size
        UpdateTriggerSize();
    }
    
    /// <summary>
    /// Complete trigger setup with configuration integration
    /// </summary>
    private void CompleteTriggerSetup()
    {
        // Apply configuration if available
        if (config != null)
        {
            ApplyConfigurationSettings();
        }
        
        // Update trigger position and size
        UpdateTriggerConfiguration();
        
        LogDebug("[DeathZoneTrigger] Completed trigger setup");
    }
    
    /// <summary>
    /// Apply settings from DeathZoneConfig
    /// </summary>
    private void ApplyConfigurationSettings()
    {
        // Update trigger size from configuration
        if (config.triggerSize.x > 0 && config.triggerSize.y > 0)
        {
            triggerSize = config.triggerSize;
        }
        
        // Apply detection sensitivity
        minimumVelocityThreshold = config.detectionSensitivity;
        
        // Update debug settings
        showDebugGizmos = config.showTriggerGizmos;
        
        LogDebug($"[DeathZoneTrigger] Applied configuration: Size={triggerSize}, Sensitivity={minimumVelocityThreshold}");
    }
    
    /// <summary>
    /// Validate trigger system setup
    /// </summary>
    private void ValidateTriggerSetup()
    {
        bool isValid = true;
        
        // Check trigger collider
        if (triggerCollider == null)
        {
            Debug.LogError("[DeathZoneTrigger] Trigger collider is missing");
            isValid = false;
        }
        else if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning("[DeathZoneTrigger] Collider is not configured as trigger");
            triggerCollider.isTrigger = true;
        }
        
        // Check positioning system
        if (positioning == null)
        {
            Debug.LogWarning("[DeathZoneTrigger] DeathZonePositioning reference not found - trigger position may not update");
        }
        
        // Check configuration
        if (config == null)
        {
            Debug.LogWarning("[DeathZoneTrigger] DeathZoneConfig not found - using default settings");
        }
        
        // Validate trigger size
        if (triggerSize.x <= 0 || triggerSize.y <= 0)
        {
            Debug.LogWarning("[DeathZoneTrigger] Invalid trigger size - setting to default");
            triggerSize = new Vector2(20f, 2f);
            UpdateTriggerSize();
            isValid = false;
        }
        
        if (isValid)
        {
            LogDebug("[DeathZoneTrigger] Trigger setup validation passed");
        }
        else
        {
            Debug.LogWarning("[DeathZoneTrigger] Trigger setup validation issues detected");
        }
    }
    
    #endregion
    
    #region Trigger Configuration
    
    /// <summary>
    /// Update trigger configuration based on positioning system
    /// </summary>
    public void UpdateTriggerConfiguration()
    {
        if (!isInitialized) return;
        
        // Update trigger size if auto-update enabled
        if (autoUpdateTriggerSize)
        {
            UpdateTriggerSizeFromConfiguration();
        }
        
        // Update trigger size on collider
        UpdateTriggerSize();
        
        LogDebug("[DeathZoneTrigger] Updated trigger configuration");
    }
    
    /// <summary>
    /// Update trigger size from configuration and positioning
    /// </summary>
    private void UpdateTriggerSizeFromConfiguration()
    {
        if (config != null)
        {
            // Get scaled trigger size from config
            Vector2 configSize = config.GetScaledTriggerSize();
            if (configSize.x > 0 && configSize.y > 0)
            {
                triggerSize = configSize;
            }
        }
        
        // Apply resolution scaling if positioning system available
        if (positioning != null && positioning.adaptToResolution)
        {
            float scaleFactor = positioning.GetCurrentScaleFactor();
            triggerSize *= scaleFactor;
        }
    }
    
    /// <summary>
    /// Update BoxCollider2D size to match trigger size
    /// </summary>
    private void UpdateTriggerSize()
    {
        if (boxCollider != null)
        {
            boxCollider.size = triggerSize;
            LogDebug($"[DeathZoneTrigger] Updated trigger collider size: {triggerSize}");
        }
    }
    
    #endregion
    
    #region Collision Detection
    
    /// <summary>
    /// Handle trigger enter events for collision detection
    /// </summary>
    /// <param name="other">Collider that entered trigger area</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Skip if on cooldown
        if (enableTriggerCooldown && isTriggerOnCooldown)
        {
            LogDebug($"[DeathZoneTrigger] Trigger on cooldown, ignoring: {other.name}");
            return;
        }
        
        // Validate ball detection
        if (IsBallObject(other.gameObject))
        {
            // Additional validation if enabled
            if (enableValidation && !ValidateBallTrigger(other.gameObject))
            {
                LogDebug($"[DeathZoneTrigger] Ball validation failed: {other.name}");
                return;
            }
            
            // Process ball trigger
            ProcessBallTrigger(other.gameObject);
        }
        else
        {
            LogDebug($"[DeathZoneTrigger] Non-ball object ignored: {other.name}");
        }
    }
    
    /// <summary>
    /// Process valid ball trigger detection
    /// </summary>
    /// <param name="ballObject">Ball GameObject that triggered death zone</param>
    private void ProcessBallTrigger(GameObject ballObject)
    {
        LogDebug($"[DeathZoneTrigger] Ball entered death zone: {ballObject.name}");
        
        // Increment trigger count
        triggeredCount++;
        
        // Start cooldown if enabled
        if (enableTriggerCooldown)
        {
            StartTriggerCooldown();
        }
        
        // Fire events
        FireTriggerEvents(ballObject);
        
        Debug.Log($"[DeathZoneTrigger] ⚡ Death zone triggered by {ballObject.name} (Count: {triggeredCount})");
    }
    
    /// <summary>
    /// Fire all trigger events for loose coupling
    /// </summary>
    /// <param name="ballObject">Ball GameObject that triggered death zone</param>
    private void FireTriggerEvents(GameObject ballObject)
    {
        try
        {
            // Fire Unity Events
            OnBallEnterDeathZone?.Invoke();
            OnBallEnterDeathZoneWithBall?.Invoke(ballObject);
            
            // Fire C# static events
            BallLostEvent?.Invoke(ballObject);
            DeathZoneTriggeredEvent?.Invoke();
            
            LogDebug("[DeathZoneTrigger] All trigger events fired successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DeathZoneTrigger] Error firing trigger events: {e.Message}");
        }
    }
    
    #endregion
    
    #region Ball Identification
    
    /// <summary>
    /// Determine if GameObject is a valid ball object
    /// </summary>
    /// <param name="obj">GameObject to test</param>
    /// <returns>True if object is identified as ball</returns>
    private bool IsBallObject(GameObject obj)
    {
        // Check tag-based identification
        if (!string.IsNullOrEmpty(ballTag) && obj.CompareTag(ballTag))
        {
            LogDebug($"[DeathZoneTrigger] Ball identified by tag: {obj.name}");
            return true;
        }
        
        // Check layer-based identification
        int objLayer = obj.layer;
        if ((ballLayer.value & (1 << objLayer)) != 0)
        {
            LogDebug($"[DeathZoneTrigger] Ball identified by layer: {obj.name} (Layer: {objLayer})");
            return true;
        }
        
        // Check component-based identification if enabled
        if (useComponentDetection)
        {
            if (HasBallComponent(obj))
            {
                LogDebug($"[DeathZoneTrigger] Ball identified by component: {obj.name}");
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if GameObject has ball-related components
    /// </summary>
    /// <param name="obj">GameObject to check</param>
    /// <returns>True if ball components found</returns>
    private bool HasBallComponent(GameObject obj)
    {
        // Check for common ball component types
        if (obj.GetComponent<Rigidbody2D>() != null && obj.GetComponent<CircleCollider2D>() != null)
        {
            return true;
        }
        
        // Check for potential ball controller components
        var ballComponents = obj.GetComponents<MonoBehaviour>();
        foreach (var component in ballComponents)
        {
            string componentName = component.GetType().Name.ToLower();
            if (componentName.Contains("ball"))
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Validate ball trigger with additional checks
    /// </summary>
    /// <param name="ballObject">Ball GameObject to validate</param>
    /// <returns>True if validation passes</returns>
    private bool ValidateBallTrigger(GameObject ballObject)
    {
        // Check velocity threshold if Rigidbody2D available
        var rigidBody = ballObject.GetComponent<Rigidbody2D>();
        if (rigidBody != null)
        {
            float velocity = rigidBody.linearVelocity.magnitude;
            if (velocity < minimumVelocityThreshold)
            {
                LogDebug($"[DeathZoneTrigger] Ball velocity too low: {velocity} < {minimumVelocityThreshold}");
                return false;
            }
        }
        
        // Check if ball is moving downward (below paddle)
        if (rigidBody != null && rigidBody.linearVelocity.y > 0)
        {
            LogDebug("[DeathZoneTrigger] Ball moving upward - potential false positive");
            return false;
        }
        
        return true;
    }
    
    #endregion
    
    #region Cooldown System
    
    /// <summary>
    /// Start trigger cooldown to prevent multiple rapid triggers
    /// </summary>
    private void StartTriggerCooldown()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }
        
        cooldownCoroutine = StartCoroutine(TriggerCooldownCoroutine());
    }
    
    /// <summary>
    /// Trigger cooldown coroutine
    /// </summary>
    /// <returns>Coroutine enumerator</returns>
    private IEnumerator TriggerCooldownCoroutine()
    {
        isTriggerOnCooldown = true;
        LogDebug($"[DeathZoneTrigger] Trigger cooldown started: {triggerCooldownDuration}s");
        
        yield return new WaitForSeconds(triggerCooldownDuration);
        
        isTriggerOnCooldown = false;
        LogDebug("[DeathZoneTrigger] Trigger cooldown ended");
        
        cooldownCoroutine = null;
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Force update trigger configuration
    /// </summary>
    public void ForceUpdateTriggerConfiguration()
    {
        UpdateTriggerConfiguration();
        LogDebug("[DeathZoneTrigger] Force updated trigger configuration");
    }
    
    /// <summary>
    /// Reset trigger cooldown immediately
    /// </summary>
    public void ResetTriggerCooldown()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
        
        isTriggerOnCooldown = false;
        LogDebug("[DeathZoneTrigger] Trigger cooldown reset");
    }
    
    /// <summary>
    /// Get current trigger state information
    /// </summary>
    /// <returns>Trigger state summary</returns>
    public string GetTriggerStatus()
    {
        return $"DeathZoneTrigger Status:\n" +
               $"• Initialized: {isInitialized}\n" +
               $"• Trigger Size: {triggerSize}\n" +
               $"• Ball Tag: {ballTag}\n" +
               $"• Ball Layer: {ballLayer}\n" +
               $"• On Cooldown: {isTriggerOnCooldown}\n" +
               $"• Triggered Count: {triggeredCount}\n" +
               $"• Validation Enabled: {enableValidation}\n" +
               $"• Component Detection: {useComponentDetection}\n" +
               $"• Config Loaded: {(config != null ? "Yes" : "No")}\n" +
               $"• Positioning Connected: {(positioning != null ? "Yes" : "No")}";
    }
    
    /// <summary>
    /// Test trigger detection with specific GameObject
    /// </summary>
    /// <param name="testObject">GameObject to test</param>
    /// <returns>True if object would trigger death zone</returns>
    public bool TestBallDetection(GameObject testObject)
    {
        if (testObject == null) return false;
        
        bool isBall = IsBallObject(testObject);
        bool passesValidation = !enableValidation || ValidateBallTrigger(testObject);
        
        LogDebug($"[DeathZoneTrigger] Test detection: {testObject.name} - Ball: {isBall}, Validation: {passesValidation}");
        return isBall && passesValidation;
    }
    
    /// <summary>
    /// Get number of times death zone has been triggered
    /// </summary>
    /// <returns>Trigger count</returns>
    public int GetTriggerCount()
    {
        return triggeredCount;
    }
    
    /// <summary>
    /// Reset trigger count to zero
    /// </summary>
    public void ResetTriggerCount()
    {
        triggeredCount = 0;
        LogDebug("[DeathZoneTrigger] Trigger count reset to zero");
    }
    
    #endregion
    
    #region Debug and Visualization
    
    /// <summary>
    /// Draw debug gizmos in Scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        DrawTriggerAreaGizmo();
        DrawDebugInformation();
    }
    
    /// <summary>
    /// Draw trigger area visualization
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;
        
        // Draw detailed gizmos when selected
        DrawTriggerAreaGizmo();
        DrawDebugLabels();
    }
    
    /// <summary>
    /// Draw trigger area gizmo
    /// </summary>
    private void DrawTriggerAreaGizmo()
    {
        if (!showTriggerArea) return;
        
        // Set gizmo properties
        Color gizmoColor = debugGizmoColor;
        if (isTriggerOnCooldown)
        {
            gizmoColor = new Color(1f, 1f, 0f, 0.3f); // Yellow when on cooldown
        }
        
        Gizmos.color = gizmoColor;
        
        // Draw trigger area
        Vector3 center = transform.position;
        Vector3 size = new Vector3(triggerSize.x, triggerSize.y, 0f);
        
        Gizmos.DrawCube(center, size);
        
        // Draw wireframe outline
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        Gizmos.DrawWireCube(center, size);
    }
    
    /// <summary>
    /// Draw debug information gizmos
    /// </summary>
    private void DrawDebugInformation()
    {
        if (!Application.isPlaying) return;
        
        // Draw status indicators
        Vector3 labelPos = transform.position + Vector3.up * (triggerSize.y * 0.5f + 1f);
        
        // Show trigger count
        if (triggeredCount > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(labelPos, 0.3f);
        }
        
        // Show cooldown status
        if (isTriggerOnCooldown)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(labelPos + Vector3.right * 0.8f, 0.2f);
        }
    }
    
    /// <summary>
    /// Draw debug labels when selected
    /// </summary>
    private void DrawDebugLabels()
    {
        #if UNITY_EDITOR
        Vector3 labelPosition = transform.position + Vector3.up * (triggerSize.y + 2f);
        
        string debugInfo = $"Death Zone Trigger\n" +
                          $"Size: {triggerSize}\n" +
                          $"Triggers: {triggeredCount}\n" +
                          $"Cooldown: {(isTriggerOnCooldown ? "Active" : "Ready")}";
        
        UnityEditor.Handles.Label(labelPosition, debugInfo);
        #endif
    }
    
    /// <summary>
    /// Log debug message if debug logging enabled
    /// </summary>
    /// <param name="message">Debug message</param>
    private void LogDebug(string message)
    {
        if (enableDebugLogging)
        {
            Debug.Log(message);
        }
    }
    
    #endregion
    
    #region Cleanup
    
    /// <summary>
    /// Clean up trigger system resources
    /// </summary>
    private void CleanupTriggerSystem()
    {
        // Stop any running coroutines
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
        
        LogDebug("[DeathZoneTrigger] Trigger system cleaned up");
    }
    
    #endregion
    
    #region Editor Support
    
    /// <summary>
    /// Called when values change in Inspector (Editor only)
    /// </summary>
    private void OnValidate()
    {
        // Ensure trigger size is positive
        triggerSize.x = Mathf.Max(0.1f, triggerSize.x);
        triggerSize.y = Mathf.Max(0.1f, triggerSize.y);
        
        // Clamp cooldown duration
        triggerCooldownDuration = Mathf.Max(0.1f, triggerCooldownDuration);
        
        // Clamp velocity threshold
        minimumVelocityThreshold = Mathf.Max(0f, minimumVelocityThreshold);
        
        // Update trigger configuration if playing
        if (Application.isPlaying && isInitialized)
        {
            UpdateTriggerConfiguration();
        }
    }
    
    #endregion
}