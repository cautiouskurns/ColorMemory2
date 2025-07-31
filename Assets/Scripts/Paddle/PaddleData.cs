using UnityEngine;

/// <summary>
/// Enumeration defining input methods available for paddle control.
/// Used to track which input method is currently active and configure input preferences.
/// </summary>
public enum InputMethod
{
    /// <summary>
    /// No input method active - paddle not controlled by player input.
    /// </summary>
    None,
    
    /// <summary>
    /// Keyboard input control using arrow keys or A/D keys.
    /// </summary>
    Keyboard,
    
    /// <summary>
    /// Mouse input control using mouse movement for paddle positioning.
    /// </summary>
    Mouse
}

/// <summary>
/// Serializable data structure defining paddle physics properties, input configuration, and movement constraints.
/// Provides foundational configuration management for Inspector-driven paddle behavior customization.
/// </summary>
[System.Serializable]
public class PaddleData
{
    [Header("Movement Properties")]
    [Tooltip("Maximum movement speed of the paddle in units per second")]
    [Range(1f, 20f)]
    public float movementSpeed = 10.0f;
    
    [Tooltip("Acceleration rate for paddle movement responsiveness")]
    [Range(5f, 30f)]
    public float acceleration = 10.0f;
    
    [Tooltip("Physical dimensions of the paddle (width, height)")]
    public Vector2 paddleDimensions = new Vector2(3.0f, 0.3f);
    
    [Header("Input Configuration")]
    [Tooltip("Sensitivity multiplier for input response (0.5 = half sensitivity, 2.0 = double sensitivity)")]
    [Range(0.5f, 3.0f)]
    public float inputSensitivity = 1.0f;
    
    [Tooltip("Enable keyboard input control (Arrow keys, A/D keys)")]
    public bool enableKeyboardInput = true;
    
    [Tooltip("Enable mouse input control (Mouse movement)")]
    public bool enableMouseInput = true;
    
    [Header("Movement Smoothing")]
    [Tooltip("Acceleration/deceleration curve for smooth movement (0-1 input, output multiplier)")]
    public AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0f, 0.8f, 1f, 1f);
    
    [Tooltip("Smooth time for movement interpolation using SmoothDamp")]
    [Range(0.05f, 0.5f)]
    public float smoothTime = 0.05f;
    
    [Tooltip("Enable smooth input processing for direct input control")]
    public bool enableSmoothInput = false;
    
    [Tooltip("Smooth time for input-based movement")]
    [Range(0.02f, 0.2f)]
    public float inputSmoothTime = 0.02f;
    
    [Header("Performance Monitoring")]
    [Tooltip("Target response time in milliseconds for WebGL performance")]
    [Range(10f, 100f)]
    public float targetResponseTime = 50f;
    
    [Header("Boundary Constraints")]
    [Tooltip("Left boundary limit for paddle movement")]
    public float leftBoundary = -8.0f;
    
    [Tooltip("Right boundary limit for paddle movement")]
    public float rightBoundary = 8.0f;
    
    [Header("Physics Properties")]
    [Tooltip("Mass of the paddle for physics calculations")]
    [Range(0.1f, 10f)]
    public float paddleMass = 1.0f;
    
    [Tooltip("Drag coefficient for paddle movement smoothing")]
    [Range(0f, 10f)]
    public float movementDrag = 2f;
    
    [Header("Runtime State")]
    [Tooltip("Current position of the paddle in world space")]
    [SerializeField] private Vector2 currentPosition;
    
    [Tooltip("Current velocity of the paddle")]
    [SerializeField] private float currentVelocity;
    
    [Tooltip("Currently active input method")]
    [SerializeField] private InputMethod activeInputMethod = InputMethod.None;
    
    [Tooltip("Input value from active input method (-1 to 1)")]
    [SerializeField] private float inputValue;
    
    // Validation flags
    private bool parametersValidated = false;
    
    #region Property Accessors
    
    /// <summary>
    /// Gets or sets the current position of the paddle.
    /// Automatically applies boundary constraints when setting.
    /// </summary>
    public Vector2 CurrentPosition
    {
        get { return currentPosition; }
        set { currentPosition = ApplyBoundaryConstraints(value); }
    }
    
    /// <summary>
    /// Gets or sets the current velocity of the paddle.
    /// </summary>
    public float CurrentVelocity
    {
        get { return currentVelocity; }
        set { currentVelocity = value; }
    }
    
    /// <summary>
    /// Gets or sets the currently active input method.
    /// </summary>
    public InputMethod ActiveInputMethod
    {
        get { return activeInputMethod; }
        set { activeInputMethod = value; }
    }
    
    /// <summary>
    /// Gets or sets the current input value from the active input method.
    /// Value is clamped to -1 to 1 range.
    /// </summary>
    public float InputValue
    {
        get { return inputValue; }
        set { inputValue = Mathf.Clamp(value, -1f, 1f); }
    }
    
    #endregion
    
    #region Validation Methods
    
    /// <summary>
    /// Validates all paddle parameters and ensures they are within acceptable ranges.
    /// </summary>
    /// <returns>True if all parameters are valid</returns>
    public bool ValidateParameters()
    {
        bool isValid = true;
        
        // Validate movement properties (only fix truly invalid values)
        if (movementSpeed <= 0f)
        {
            Debug.LogWarning("[PaddleData] Movement speed must be greater than 0. Setting to minimum value.");
            movementSpeed = 1.0f;
            isValid = false;
        }
        
        if (acceleration <= 0f)
        {
            Debug.LogWarning("[PaddleData] Acceleration must be greater than 0. Setting to minimum value.");
            acceleration = 5.0f;
            isValid = false;
        }
        
        // Validate paddle dimensions
        if (paddleDimensions.x <= 0f || paddleDimensions.y <= 0f)
        {
            Debug.LogWarning("[PaddleData] Paddle dimensions must be greater than 0. Setting to minimum values.");
            if (paddleDimensions.x <= 0f) paddleDimensions.x = 0.5f;
            if (paddleDimensions.y <= 0f) paddleDimensions.y = 0.1f;
            isValid = false;
        }
        
        // Validate input sensitivity
        if (inputSensitivity <= 0f)
        {
            Debug.LogWarning("[PaddleData] Input sensitivity must be greater than 0. Setting to minimum value.");
            inputSensitivity = 0.1f;
            isValid = false;
        }
        
        // Validate boundaries
        if (leftBoundary >= rightBoundary)
        {
            Debug.LogWarning("[PaddleData] Left boundary must be less than right boundary. Setting to default values.");
            leftBoundary = -8.0f;
            rightBoundary = 8.0f;
            isValid = false;
        }
        
        // Validate physics properties
        if (paddleMass <= 0f)
        {
            Debug.LogWarning("[PaddleData] Paddle mass must be greater than 0. Setting to default value.");
            paddleMass = 1.0f;
            isValid = false;
        }
        
        if (movementDrag < 0f)
        {
            Debug.LogWarning("[PaddleData] Movement drag cannot be negative. Setting to default value.");
            movementDrag = 2f;
            isValid = false;
        }
        
        // Validate smoothing parameters
        if (smoothTime <= 0f)
        {
            Debug.LogWarning("[PaddleData] Smooth time must be greater than 0. Setting to default value.");
            smoothTime = 0.1f;
            isValid = false;
        }
        
        if (inputSmoothTime <= 0f)
        {
            Debug.LogWarning("[PaddleData] Input smooth time must be greater than 0. Setting to default value.");
            inputSmoothTime = 0.05f;
            isValid = false;
        }
        
        // Validate performance parameters
        if (targetResponseTime <= 0f)
        {
            Debug.LogWarning("[PaddleData] Target response time must be greater than 0. Setting to default value.");
            targetResponseTime = 50f;
            isValid = false;
        }
        
        // Validate acceleration curve
        if (accelerationCurve == null || accelerationCurve.length == 0)
        {
            Debug.LogWarning("[PaddleData] Acceleration curve is invalid. Setting to default curve.");
            accelerationCurve = AnimationCurve.EaseInOut(0f, 0.2f, 1f, 1f);
            isValid = false;
        }
        
        parametersValidated = true;
        return isValid;
    }
    
    /// <summary>
    /// Applies boundary constraints to a position vector.
    /// </summary>
    /// <param name="position">Position to constrain</param>
    /// <returns>Position clamped within boundary limits</returns>
    public Vector2 ApplyBoundaryConstraints(Vector2 position)
    {
        float clampedX = Mathf.Clamp(position.x, leftBoundary, rightBoundary);
        return new Vector2(clampedX, position.y);
    }
    
    /// <summary>
    /// Checks if the given position is within the paddle's movement boundaries.
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <returns>True if position is within boundaries</returns>
    public bool IsWithinBoundaries(Vector2 position)
    {
        return position.x >= leftBoundary && position.x <= rightBoundary;
    }
    
    /// <summary>
    /// Gets the total width of the playable area.
    /// </summary>
    /// <returns>Width of the area between left and right boundaries</returns>
    public float GetPlayableWidth()
    {
        return rightBoundary - leftBoundary;
    }
    
    /// <summary>
    /// Gets the center position between the boundaries.
    /// </summary>
    /// <returns>Center position of the playable area</returns>
    public float GetCenterPosition()
    {
        return (leftBoundary + rightBoundary) * 0.5f;
    }
    
    #endregion
    
    #region State Management
    
    /// <summary>
    /// Resets all runtime state to default values.
    /// </summary>
    public void ResetState()
    {
        currentPosition = new Vector2(GetCenterPosition(), currentPosition.y);
        currentVelocity = 0f;
        activeInputMethod = InputMethod.None;
        inputValue = 0f;
        
        Debug.Log("[PaddleData] State reset to defaults");
    }
    
    /// <summary>
    /// Configures optimal settings for WebGL deployment.
    /// </summary>
    public void ConfigureForWebGL()
    {
        targetResponseTime = 45f;  // Slightly below 50ms target
        smoothTime = 0.03f;        // Very responsive but smooth
        inputSmoothTime = 0.01f;   // Immediate input response
        enableSmoothInput = false; // Disable for maximum responsiveness
        movementSpeed = Mathf.Clamp(movementSpeed, 10f, 15f); // Higher speed range
        
        // Ensure acceleration curve starts high for immediate response
        accelerationCurve = AnimationCurve.EaseInOut(0f, 0.9f, 1f, 1f);
        
        ValidateParameters();
        Debug.Log("[PaddleData] Configured for optimal WebGL performance with responsive movement");
    }
    
    /// <summary>
    /// Initializes paddle data with validation and optional WebGL optimization.
    /// This method resets runtime state and should only be called for new instances.
    /// </summary>
    /// <param name="optimizeForWebGL">Whether to apply WebGL-specific optimizations</param>
    public void Initialize(bool optimizeForWebGL = false)
    {
        if (optimizeForWebGL)
        {
            ConfigureForWebGL();
        }
        else
        {
            ValidateParameters();
        }
        
        ResetState();
        
        Debug.Log($"[PaddleData] Paddle data initialized successfully (WebGL optimized: {optimizeForWebGL})");
    }
    
    /// <summary>
    /// Validates existing paddle data configuration without resetting state.
    /// Use this for Inspector-configured PaddleData to preserve user settings.
    /// </summary>
    /// <returns>True if all parameters are valid</returns>
    public bool ValidateExistingConfiguration()
    {
        bool isValid = ValidateParameters();
        
        Debug.Log($"[PaddleData] Existing configuration validated. Speed: {movementSpeed}, Acceleration: {acceleration}, Valid: {isValid}");
        return isValid;
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Gets a summary of the current paddle configuration.
    /// </summary>
    /// <returns>Configuration summary string</returns>
    public string GetConfigurationSummary()
    {
        ValidateParameters();
        
        return $"PaddleData Configuration:\n" +
               $"• Movement Speed: {movementSpeed:F1} units/sec\n" +
               $"• Acceleration: {acceleration:F1} units/sec²\n" +
               $"• Dimensions: {paddleDimensions.x:F1} x {paddleDimensions.y:F1}\n" +
               $"• Input Sensitivity: {inputSensitivity:F1}x\n" +
               $"• Keyboard Input: {enableKeyboardInput}\n" +
               $"• Mouse Input: {enableMouseInput}\n" +
               $"• Boundaries: {leftBoundary:F1} to {rightBoundary:F1}\n" +
               $"• Mass: {paddleMass:F1}kg\n" +
               $"• Drag: {movementDrag:F1}\n" +
               $"• Smooth Time: {smoothTime:F3}s\n" +
               $"• Input Smooth Time: {inputSmoothTime:F3}s\n" +
               $"• Smooth Input Enabled: {enableSmoothInput}\n" +
               $"• Target Response Time: {targetResponseTime:F1}ms\n" +
               $"• Acceleration Curve Keys: {accelerationCurve.length}\n" +
               $"• Current Position: {currentPosition}\n" +
               $"• Current Velocity: {currentVelocity:F2}\n" +
               $"• Active Input: {activeInputMethod}\n" +
               $"• Parameters Valid: {parametersValidated}";
    }
    
    /// <summary>
    /// Checks if any input method is enabled.
    /// </summary>
    /// <returns>True if keyboard or mouse input is enabled</returns>
    public bool HasInputEnabled()
    {
        return enableKeyboardInput || enableMouseInput;
    }
    
    /// <summary>
    /// Gets the maximum distance the paddle can travel.
    /// </summary>
    /// <returns>Maximum travel distance in units</returns>
    public float GetMaxTravelDistance()
    {
        return GetPlayableWidth() - paddleDimensions.x;
    }
    
    /// <summary>
    /// Calculates the effective input value with sensitivity applied.
    /// </summary>
    /// <param name="rawInput">Raw input value (-1 to 1)</param>
    /// <returns>Input value modified by sensitivity setting</returns>
    public float ApplyInputSensitivity(float rawInput)
    {
        return Mathf.Clamp(rawInput * inputSensitivity, -1f, 1f);
    }
    
    /// <summary>
    /// Evaluates the acceleration curve at the given progress value.
    /// </summary>
    /// <param name="progress">Movement progress (0-1)</param>
    /// <returns>Acceleration multiplier from curve</returns>
    public float EvaluateAccelerationCurve(float progress)
    {
        if (accelerationCurve == null || accelerationCurve.length == 0)
        {
            return 1f; // Default multiplier if curve is invalid
        }
        
        return Mathf.Max(0.1f, accelerationCurve.Evaluate(Mathf.Clamp01(progress)));
    }
    
    /// <summary>
    /// Gets the optimal smooth time based on movement distance and target performance.
    /// </summary>
    /// <param name="distance">Movement distance</param>
    /// <returns>Optimized smooth time value</returns>
    public float GetOptimalSmoothTime(float distance)
    {
        // Adjust smooth time based on distance for consistent feel
        float baseTime = enableSmoothInput ? inputSmoothTime : smoothTime;
        float distanceMultiplier = Mathf.Clamp(distance / GetPlayableWidth(), 0.5f, 2f);
        return baseTime * distanceMultiplier;
    }
    
    /// <summary>
    /// Checks if performance parameters are optimized for WebGL deployment.
    /// </summary>
    /// <returns>True if configuration is optimized for WebGL performance</returns>
    public bool IsOptimizedForWebGL()
    {
        return targetResponseTime <= 50f && 
               smoothTime <= 0.2f && 
               inputSmoothTime <= 0.1f &&
               movementSpeed >= 5f &&
               movementSpeed <= 15f;
    }
    
    /// <summary>
    /// Gets recommended settings for optimal WebGL performance.
    /// </summary>
    /// <returns>Performance optimization recommendations</returns>
    public string GetWebGLOptimizationRecommendations()
    {
        var recommendations = new System.Text.StringBuilder();
        recommendations.AppendLine("WebGL Performance Recommendations:");
        
        if (targetResponseTime > 50f)
            recommendations.AppendLine("• Reduce target response time to ≤50ms for better responsiveness");
            
        if (smoothTime > 0.2f)
            recommendations.AppendLine("• Reduce smooth time to ≤0.2s for better performance");
            
        if (inputSmoothTime > 0.1f)
            recommendations.AppendLine("• Reduce input smooth time to ≤0.1s for immediate response");
            
        if (movementSpeed < 5f)
            recommendations.AppendLine("• Increase movement speed to ≥5 units/sec for responsive gameplay");
            
        if (movementSpeed > 15f)
            recommendations.AppendLine("• Consider reducing movement speed to ≤15 units/sec for better control");
            
        if (recommendations.Length == "WebGL Performance Recommendations:\n".Length)
            recommendations.AppendLine("• Configuration is already optimized for WebGL!");
            
        return recommendations.ToString();
    }
    
    #endregion
}