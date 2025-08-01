using UnityEngine;

/// <summary>
/// Enumeration defining different types of death zone triggers for flexible detection systems.
/// </summary>
[System.Serializable]
public enum DeathZoneTriggerType
{
    /// <summary>
    /// Simple below-paddle detection - triggers when ball falls below paddle Y position
    /// </summary>
    BelowPaddle,
    
    /// <summary>
    /// Bottom boundary trigger - uses screen bottom boundary with offset
    /// </summary>
    BottomBoundary,
    
    /// <summary>
    /// Custom positioned trigger zone with manual positioning
    /// </summary>
    CustomZone,
    
    /// <summary>
    /// Dynamic zone that adjusts based on paddle movement
    /// </summary>
    DynamicPaddleRelative
}

/// <summary>
/// Enumeration for different feedback types when death zone is triggered.
/// </summary>
[System.Serializable]
public enum DeathZoneFeedbackType
{
    /// <summary>
    /// No feedback - silent death zone trigger
    /// </summary>
    None,
    
    /// <summary>
    /// Audio only feedback
    /// </summary>
    Audio,
    
    /// <summary>
    /// Visual effects only
    /// </summary>
    Visual,
    
    /// <summary>
    /// Combined audio and visual feedback
    /// </summary>
    AudioVisual,
    
    /// <summary>
    /// Screen shake and haptic feedback
    /// </summary>
    Haptic
}

/// <summary>
/// Main configuration ScriptableObject for death zone system setup and management.
/// Contains all parameters needed for death zone trigger detection, life management,
/// positioning, and feedback configuration in Breakout gameplay.
/// </summary>
[CreateAssetMenu(fileName = "DeathZoneConfig", menuName = "Breakout/Death Zone Configuration", order = 1)]
public class DeathZoneConfig : ScriptableObject
{
    [Header("Trigger Configuration")]
    [Tooltip("Type of death zone trigger to use")]
    public DeathZoneTriggerType triggerType = DeathZoneTriggerType.BelowPaddle;
    
    [Tooltip("Size of the death zone trigger area in world units")]
    public Vector2 triggerSize = new Vector2(30f, 2f);
    
    [Tooltip("Detection sensitivity - smaller values are more sensitive")]
    [Range(0.01f, 1f)]
    public float detectionSensitivity = 0.1f;
    
    [Tooltip("Enable trigger visualization in Scene view for debugging")]
    public bool showTriggerGizmos = true;
    
    [Header("Positioning Parameters")]
    [Tooltip("Offset below paddle position for death zone placement")]
    [Range(0f, 10f)]
    public float paddleOffset = 2f;
    
    [Tooltip("Additional position offsets for fine-tuning (X: horizontal, Y: vertical)")]
    public Vector2 positioningOffsets = Vector2.zero;
    
    [Tooltip("Automatically adjust position based on screen resolution")]
    public bool enableResolutionScaling = true;
    
    [Tooltip("Minimum distance from screen bottom")]
    [Range(0f, 5f)]
    public float minimumBottomDistance = 1f;
    
    [Header("Life Management")]
    [Tooltip("Number of lives player starts with")]
    [Range(1, 10)]
    public int startingLives = 3;
    
    [Tooltip("Number of lives lost when death zone is triggered")]
    [Range(1, 5)]
    public int livesReduction = 1;
    
    [Tooltip("Enable automatic game over detection when lives reach zero")]
    public bool enableGameOverDetection = true;
    
    [Tooltip("Delay before respawning ball after life loss")]
    [Range(0f, 5f)]
    public float respawnDelay = 1.5f;
    
    [Header("Feedback Configuration")]
    [Tooltip("Type of feedback to provide when death zone is triggered")]
    public DeathZoneFeedbackType feedbackType = DeathZoneFeedbackType.AudioVisual;
    
    [Tooltip("Audio volume for death zone trigger sound")]
    [Range(0f, 1f)]
    public float audioVolume = 0.7f;
    
    [Tooltip("Duration of feedback effects")]
    [Range(0.1f, 3f)]
    public float feedbackDuration = 1f;
    
    [Tooltip("Intensity of visual and haptic effects")]
    [Range(0f, 1f)]
    public float effectIntensity = 0.8f;
    
    [Header("Visual Feedback")]
    [Tooltip("Color for death zone visualization")]
    public Color deathZoneColor = new Color(1f, 0.2f, 0.2f, 0.5f); // Red with transparency
    
    [Tooltip("Flash screen color on death zone trigger")]
    public Color screenFlashColor = new Color(1f, 0f, 0f, 0.3f); // Red flash
    
    [Tooltip("Enable particle effects on death zone trigger")]
    public bool enableParticleEffects = true;
    
    [Header("Performance Settings")]
    [Tooltip("Enable death zone trigger detection")]
    public bool enableDeathZone = true;
    
    [Tooltip("Collision detection mode for death zone triggers")]
    public CollisionDetectionMode2D collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    
    [Tooltip("Physics layer for death zone collision detection")]
    public int deathZoneLayer = 0;
    
    /// <summary>
    /// Validates the death zone configuration for completeness and correctness.
    /// </summary>
    /// <returns>True if configuration is valid</returns>
    public bool ValidateConfiguration()
    {
        bool isValid = true;
        
        // Validate trigger dimensions
        if (triggerSize.x <= 0f || triggerSize.y <= 0f)
        {
            Debug.LogError("[DeathZoneConfig] Invalid trigger size - width and height must be positive");
            isValid = false;
        }
        
        // Validate life management
        if (startingLives <= 0)
        {
            Debug.LogError("[DeathZoneConfig] Starting lives must be greater than zero");
            isValid = false;
        }
        
        if (livesReduction <= 0)
        {
            Debug.LogError("[DeathZoneConfig] Lives reduction must be greater than zero");
            isValid = false;
        }
        
        // Validate detection sensitivity
        if (detectionSensitivity <= 0f)
        {
            Debug.LogError("[DeathZoneConfig] Detection sensitivity must be greater than zero");
            isValid = false;
        }
        
        // Validate feedback settings
        if (feedbackDuration <= 0f)
        {
            Debug.LogError("[DeathZoneConfig] Feedback duration must be greater than zero");
            isValid = false;
        }
        
        // Validate collision layer
        if (deathZoneLayer < 0 || deathZoneLayer > 31)
        {
            Debug.LogError("[DeathZoneConfig] Death zone layer must be between 0 and 31");
            isValid = false;
        }
        
        return isValid;
    }
    
    /// <summary>
    /// Resets all configuration values to default Breakout gameplay settings.
    /// </summary>
    public void ResetToDefaults()
    {
        // Trigger configuration
        triggerType = DeathZoneTriggerType.BelowPaddle;
        triggerSize = new Vector2(30f, 2f);
        detectionSensitivity = 0.1f;
        showTriggerGizmos = true;
        
        // Positioning parameters
        paddleOffset = 2f;
        positioningOffsets = Vector2.zero;
        enableResolutionScaling = true;
        minimumBottomDistance = 1f;
        
        // Life management
        startingLives = 3;
        livesReduction = 1;
        enableGameOverDetection = true;
        respawnDelay = 1.5f;
        
        // Feedback configuration
        feedbackType = DeathZoneFeedbackType.AudioVisual;
        audioVolume = 0.7f;
        feedbackDuration = 1f;
        effectIntensity = 0.8f;
        
        // Visual feedback
        deathZoneColor = new Color(1f, 0.2f, 0.2f, 0.5f);
        screenFlashColor = new Color(1f, 0f, 0f, 0.3f);
        enableParticleEffects = true;
        
        // Performance settings
        enableDeathZone = true;
        collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        deathZoneLayer = 0;
    }
    
    /// <summary>
    /// Calculates the world position for death zone placement based on configuration.
    /// </summary>
    /// <param name="paddlePosition">Current paddle world position</param>
    /// <param name="screenBounds">Screen bounds for boundary calculations</param>
    /// <returns>Calculated death zone world position</returns>
    public Vector3 CalculateDeathZonePosition(Vector3 paddlePosition, Bounds screenBounds)
    {
        Vector3 position = paddlePosition;
        
        switch (triggerType)
        {
            case DeathZoneTriggerType.BelowPaddle:
                position.y = paddlePosition.y - paddleOffset;
                break;
                
            case DeathZoneTriggerType.BottomBoundary:
                position.y = screenBounds.min.y + minimumBottomDistance;
                position.x = screenBounds.center.x;
                break;
                
            case DeathZoneTriggerType.CustomZone:
                // Use positioning offsets as absolute position
                position = new Vector3(positioningOffsets.x, positioningOffsets.y, 0f);
                break;
                
            case DeathZoneTriggerType.DynamicPaddleRelative:
                // Dynamic positioning that follows paddle with offset
                position.x = paddlePosition.x;
                position.y = paddlePosition.y - paddleOffset;
                break;
        }
        
        // Apply additional positioning offsets
        position.x += positioningOffsets.x;
        position.y += positioningOffsets.y;
        
        // Ensure minimum distance from bottom
        float minimumY = screenBounds.min.y + minimumBottomDistance;
        if (position.y < minimumY)
        {
            position.y = minimumY;
        }
        
        // Ensure z position is 0 for 2D gameplay
        position.z = 0f;
        
        return position;
    }
    
    /// <summary>
    /// Gets the effective trigger size with resolution scaling applied.
    /// </summary>
    /// <param name="scaleFactor">Resolution scale factor</param>
    /// <returns>Scaled trigger size</returns>
    public Vector2 GetScaledTriggerSize(float scaleFactor = 1f)
    {
        if (!enableResolutionScaling)
        {
            return triggerSize;
        }
        
        return triggerSize * scaleFactor;
    }
    
    /// <summary>
    /// Checks if the configuration should provide feedback for death zone triggers.
    /// </summary>
    /// <returns>True if feedback is enabled</returns>
    public bool ShouldProvideFeedback()
    {
        return feedbackType != DeathZoneFeedbackType.None && effectIntensity > 0f;
    }
    
    /// <summary>
    /// Checks if audio feedback is enabled.
    /// </summary>
    /// <returns>True if audio feedback should play</returns>
    public bool ShouldPlayAudio()
    {
        return (feedbackType == DeathZoneFeedbackType.Audio || 
                feedbackType == DeathZoneFeedbackType.AudioVisual) && 
               audioVolume > 0f;
    }
    
    /// <summary>
    /// Checks if visual feedback is enabled.
    /// </summary>
    /// <returns>True if visual feedback should display</returns>
    public bool ShouldShowVisualFeedback()
    {
        return feedbackType == DeathZoneFeedbackType.Visual || 
               feedbackType == DeathZoneFeedbackType.AudioVisual ||
               feedbackType == DeathZoneFeedbackType.Haptic;
    }
    
    /// <summary>
    /// Gets a comprehensive summary of the death zone configuration.
    /// </summary>
    /// <returns>Configuration summary string</returns>
    public string GetConfigurationSummary()
    {
        return $"DeathZoneConfig Summary:\n" +
               $"• Trigger Type: {triggerType}\n" +
               $"• Trigger Size: {triggerSize}\n" +
               $"• Detection Sensitivity: {detectionSensitivity:F2}\n" +
               $"• Paddle Offset: {paddleOffset:F1}\n" +
               $"• Starting Lives: {startingLives}\n" +
               $"• Lives Reduction: {livesReduction}\n" +
               $"• Game Over Detection: {enableGameOverDetection}\n" +
               $"• Respawn Delay: {respawnDelay:F1}s\n" +
               $"• Feedback Type: {feedbackType}\n" +
               $"• Audio Volume: {audioVolume:F1}\n" +
               $"• Effect Intensity: {effectIntensity:F1}\n" +
               $"• Resolution Scaling: {enableResolutionScaling}\n" +
               $"• Death Zone Enabled: {enableDeathZone}\n" +
               $"• Configuration Valid: {ValidateConfiguration()}";
    }
    
    /// <summary>
    /// Called when values change in Inspector (Editor only).
    /// </summary>
    private void OnValidate()
    {
        // Clamp values to valid ranges
        startingLives = Mathf.Max(1, startingLives);
        livesReduction = Mathf.Max(1, livesReduction);
        detectionSensitivity = Mathf.Max(0.01f, detectionSensitivity);
        paddleOffset = Mathf.Max(0f, paddleOffset);
        minimumBottomDistance = Mathf.Max(0f, minimumBottomDistance);
        respawnDelay = Mathf.Max(0f, respawnDelay);
        feedbackDuration = Mathf.Max(0.1f, feedbackDuration);
        audioVolume = Mathf.Clamp01(audioVolume);
        effectIntensity = Mathf.Clamp01(effectIntensity);
        
        // Ensure trigger size is positive
        triggerSize.x = Mathf.Max(0.1f, triggerSize.x);
        triggerSize.y = Mathf.Max(0.1f, triggerSize.y);
        
        // Clamp collision layer
        deathZoneLayer = Mathf.Clamp(deathZoneLayer, 0, 31);
    }
}