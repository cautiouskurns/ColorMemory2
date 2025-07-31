using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enumeration defining collision types for centralized collision handling.
/// Used to categorize and route collision events to appropriate handlers.
/// </summary>
public enum CollisionType
{
    /// <summary>
    /// Collision with player paddle.
    /// </summary>
    Paddle,
    
    /// <summary>
    /// Collision with destructible brick.
    /// </summary>
    Brick,
    
    /// <summary>
    /// Collision with game boundary (walls).
    /// </summary>
    Boundary,
    
    /// <summary>
    /// Collision with power-up collectible.
    /// </summary>
    PowerUp,
    
    /// <summary>
    /// Unknown or unhandled collision type.
    /// </summary>
    Unknown
}

/// <summary>
/// Data structure for collision validation and debugging information.
/// </summary>
[System.Serializable]
public struct CollisionValidationData
{
    public CollisionType type;
    public Vector2 position;
    public Vector2 velocity;
    public float timestamp;
    public float intensity;
    public bool wasValidated;
    public string validationResult;
    
    public CollisionValidationData(CollisionType collisionType, Vector2 contactPosition, Vector2 ballVelocity, float time, float collisionIntensity)
    {
        type = collisionType;
        position = contactPosition;
        velocity = ballVelocity;
        timestamp = time;
        intensity = collisionIntensity;
        wasValidated = false;
        validationResult = "";
    }
}

/// <summary>
/// Central collision manager using singleton pattern for coordinating all collision responses in Breakout game.
/// Provides framework for collision detection, categorization, and routing to specific handlers.
/// </summary>
public class CollisionManager : MonoBehaviour
{
    #region Singleton Pattern
    
    /// <summary>
    /// Singleton instance of CollisionManager.
    /// </summary>
    public static CollisionManager Instance { get; private set; }
    
    #endregion
    
    #region Inspector Configuration
    
    [Header("Collision Detection")]
    [Tooltip("Enable detailed collision logging for debugging")]
    [SerializeField] private bool enableCollisionLogging = true;
    
    [Header("GameObject References")]
    [Tooltip("Reference to Ball GameObject for collision event handling")]
    [SerializeField] private GameObject ballGameObject;
    
    [Header("Bounce Calculation")]
    [Tooltip("Minimum bounce angle in degrees (prevents horizontal bouncing)")]
    [Range(5f, 45f)]
    [SerializeField] private float minBounceAngle = 15f;
    
    [Tooltip("Maximum bounce angle in degrees (prevents horizontal bouncing)")]
    [Range(135f, 175f)]
    [SerializeField] private float maxBounceAngle = 165f;
    
    [Tooltip("Paddle width for hit position calculation (auto-detected if 0)")]
    [SerializeField] private float paddleWidth = 2.0f;
    
    [Tooltip("Enable bounce angle visualization in Scene view")]
    [SerializeField] private bool enableBounceVisualization = true;
    
    [Header("Collision Feedback")]
    [Tooltip("AudioSource component for collision sound effects")]
    [SerializeField] private AudioSource audioSource;
    
    [Tooltip("ParticleSystem component for collision visual effects")]
    [SerializeField] private ParticleSystem collisionParticles;
    
    [Tooltip("Main camera reference for screen shake effects")]
    [SerializeField] private Camera gameCamera;
    
    [Header("Audio Clips")]
    [Tooltip("Audio clip for paddle bounce collisions")]
    [SerializeField] private AudioClip paddleBounceClip;
    
    [Tooltip("Audio clip for wall/boundary bounce collisions")]
    [SerializeField] private AudioClip wallBounceClip;
    
    [Tooltip("Audio clip for brick hit collisions")]
    [SerializeField] private AudioClip brickHitClip;
    
    [Tooltip("Audio clip for power-up collection")]
    [SerializeField] private AudioClip powerUpClip;
    
    [Header("Feedback Settings")]
    [Tooltip("Screen shake intensity multiplier")]
    [Range(0f, 1f)]
    [SerializeField] private float screenShakeIntensity = 0.1f;
    
    [Tooltip("Screen shake duration in seconds")]
    [Range(0.05f, 0.5f)]
    [SerializeField] private float screenShakeDuration = 0.15f;
    
    [Tooltip("Number of particles to emit on collision")]
    [Range(1, 20)]
    [SerializeField] private int particleBurstCount = 5;
    
    [Tooltip("Enable audio feedback for collisions")]
    [SerializeField] private bool enableAudioFeedback = true;
    
    [Tooltip("Enable particle feedback for collisions")]
    [SerializeField] private bool enableParticleFeedback = true;
    
    [Tooltip("Enable screen shake feedback for collisions")]
    [SerializeField] private bool enableScreenShake = true;
    
    [Header("Edge Case Handling")]
    [Tooltip("Minimum allowed ball speed (prevents stuck balls)")]
    [Range(1f, 10f)]
    [SerializeField] private float minBallSpeed = 3.0f;
    
    [Tooltip("Maximum allowed ball speed (prevents physics instability)")]
    [Range(10f, 30f)]
    [SerializeField] private float maxBallSpeed = 15.0f;
    
    [Tooltip("Time in seconds before stuck ball detection triggers")]
    [Range(1f, 5f)]
    [SerializeField] private float stuckDetectionTime = 2.0f;
    
    [Tooltip("Velocity threshold below which ball is considered stuck")]
    [Range(0.05f, 1f)]
    [SerializeField] private float stuckVelocityThreshold = 0.1f;
    
    [Tooltip("Force magnitude for stuck ball correction")]
    [Range(1f, 10f)]
    [SerializeField] private float stuckCorrectionForce = 5.0f;
    
    [Tooltip("Enable debug visualization for edge case handling")]
    [SerializeField] private bool enableValidationDebug = true;
    
    [Tooltip("Maximum simultaneous collisions to process per frame")]
    [Range(1, 10)]
    [SerializeField] private int maxSimultaneousCollisions = 3;
    
    #endregion
    
    #region Private Fields
    
    // Layer indices for collision type detection
    private int ballLayerIndex = -1;
    private int paddleLayerIndex = -1;
    private int bricksLayerIndex = -1;
    private int powerUpsLayerIndex = -1;
    private int boundariesLayerIndex = -1;
    
    // Collision statistics
    private int totalCollisions = 0;
    private float lastCollisionTime;
    
    // Bounce calculation cache
    private Rigidbody2D ballRigidbody;
    private GameObject paddleGameObject;
    private BoxCollider2D paddleCollider;
    
    // Bounce calculation debug data
    private Vector2 lastHitPosition;
    private float lastBounceAngle;
    private Vector2 lastBounceVelocity;
    
    // Collision feedback system
    private Vector3 originalCameraPosition;
    private Coroutine currentShakeCoroutine;
    private ParticleSystem.EmitParams particleEmitParams;
    private bool feedbackSystemInitialized = false;
    
    // Edge case handling system
    private float ballStuckTimer = 0f;
    private Vector2 lastBallPosition;
    private float lastSpeedValidationTime;
    private Queue<Collision2D> pendingCollisions = new Queue<Collision2D>();
    private List<CollisionValidationData> recentCollisions = new List<CollisionValidationData>();
    private bool validationSystemInitialized = false;
    
    #endregion
    
    #region Unity Lifecycle
    
    /// <summary>
    /// Initialize singleton instance and cache layer indices.
    /// </summary>
    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCollisionManager();
        }
        else
        {
            Debug.LogWarning("[CollisionManager] Multiple CollisionManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Connect to Ball GameObject and validate setup.
    /// </summary>
    private void Start()
    {
        ConnectToBallGameObject();
        ConnectToPaddleGameObject();
        InitializeFeedbackSystem();
        InitializeValidationSystem();
        ValidateSetup();
    }
    
    /// <summary>
    /// Physics update for continuous collision validation.
    /// </summary>
    private void FixedUpdate()
    {
        if (validationSystemInitialized && ballRigidbody != null)
        {
            ValidateCollisionIntegrity();
        }
    }
    
    /// <summary>
    /// Cleanup singleton instance on destroy.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// Initialize collision manager with layer detection and configuration.
    /// </summary>
    private void InitializeCollisionManager()
    {
        Debug.Log("[CollisionManager] Initializing collision coordination system...");
        
        // Cache layer indices for efficient collision type detection
        CacheLayerIndices();
        
        // Reset collision statistics
        totalCollisions = 0;
        lastCollisionTime = 0f;
        
        Debug.Log("[CollisionManager] Collision manager initialized successfully");
    }
    
    /// <summary>
    /// Cache physics layer indices for efficient collision type detection.
    /// </summary>
    private void CacheLayerIndices()
    {
        ballLayerIndex = LayerMask.NameToLayer("Ball");
        paddleLayerIndex = LayerMask.NameToLayer("Paddle");
        bricksLayerIndex = LayerMask.NameToLayer("Bricks");
        powerUpsLayerIndex = LayerMask.NameToLayer("PowerUps");
        boundariesLayerIndex = LayerMask.NameToLayer("Boundaries");
        
        // Log layer detection results
        Debug.Log($"[CollisionManager] Layer indices cached:");
        Debug.Log($"   • Ball: {ballLayerIndex}");
        Debug.Log($"   • Paddle: {paddleLayerIndex}");
        Debug.Log($"   • Bricks: {bricksLayerIndex}");
        Debug.Log($"   • PowerUps: {powerUpsLayerIndex}");
        Debug.Log($"   • Boundaries: {boundariesLayerIndex}");
        
        // Warn about missing layers
        if (ballLayerIndex == -1) Debug.LogWarning("[CollisionManager] 'Ball' layer not found. Run Physics Layer setup first.");
        if (paddleLayerIndex == -1) Debug.LogWarning("[CollisionManager] 'Paddle' layer not found. Run Physics Layer setup first.");
        if (bricksLayerIndex == -1) Debug.LogWarning("[CollisionManager] 'Bricks' layer not found. Run Physics Layer setup first.");
        if (powerUpsLayerIndex == -1) Debug.LogWarning("[CollisionManager] 'PowerUps' layer not found. Run Physics Layer setup first.");
        if (boundariesLayerIndex == -1) Debug.LogWarning("[CollisionManager] 'Boundaries' layer not found. Run Physics Layer setup first.");
    }
    
    /// <summary>
    /// Connect to Ball GameObject for collision event handling.
    /// </summary>
    private void ConnectToBallGameObject()
    {
        // Try to find Ball GameObject if not assigned
        if (ballGameObject == null)
        {
            ballGameObject = GameObject.Find("Ball");
        }
        
        if (ballGameObject != null)
        {
            Debug.Log($"[CollisionManager] Connected to Ball GameObject: {ballGameObject.name}");
            
            // Cache Ball Rigidbody2D for bounce calculations
            ballRigidbody = ballGameObject.GetComponent<Rigidbody2D>();
            Collider2D ballCollider = ballGameObject.GetComponent<Collider2D>();
            
            if (ballRigidbody == null)
            {
                Debug.LogWarning("[CollisionManager] Ball GameObject missing Rigidbody2D component!");
            }
            
            if (ballCollider == null)
            {
                Debug.LogWarning("[CollisionManager] Ball GameObject missing Collider2D component!");
            }
            
            // Check if Ball is on correct layer
            if (ballGameObject.layer == ballLayerIndex)
            {
                Debug.Log("[CollisionManager] Ball GameObject is on correct layer");
            }
            else
            {
                Debug.LogWarning($"[CollisionManager] Ball GameObject is on layer {ballGameObject.layer}, expected layer {ballLayerIndex} ('Ball')");
            }
        }
        else
        {
            Debug.LogWarning("[CollisionManager] Ball GameObject not found. Collision events will not be captured until Ball is created and connected.");
        }
    }
    
    /// <summary>
    /// Connect to Paddle GameObject for bounce calculation.
    /// </summary>
    private void ConnectToPaddleGameObject()
    {
        // Try to find Paddle GameObject if not assigned
        paddleGameObject = GameObject.Find("Paddle");
        
        if (paddleGameObject != null)
        {
            Debug.Log($"[CollisionManager] Connected to Paddle GameObject: {paddleGameObject.name}");
            
            // Cache Paddle collider for width calculation
            paddleCollider = paddleGameObject.GetComponent<BoxCollider2D>();
            
            if (paddleCollider != null)
            {
                // Auto-detect paddle width if not manually set
                if (paddleWidth <= 0f)
                {
                    paddleWidth = paddleCollider.size.x;
                    Debug.Log($"[CollisionManager] Auto-detected paddle width: {paddleWidth:F2}");
                }
                else
                {
                    Debug.Log($"[CollisionManager] Using configured paddle width: {paddleWidth:F2}");
                }
            }
            else
            {
                Debug.LogWarning("[CollisionManager] Paddle GameObject missing BoxCollider2D component! Bounce calculations may be inaccurate.");
                if (paddleWidth <= 0f)
                {
                    paddleWidth = 2.0f; // Safe default
                    Debug.Log($"[CollisionManager] Using default paddle width: {paddleWidth:F2}");
                }
            }
            
            // Check if Paddle is on correct layer
            if (paddleGameObject.layer == paddleLayerIndex)
            {
                Debug.Log("[CollisionManager] Paddle GameObject is on correct layer");
            }
            else
            {
                Debug.LogWarning($"[CollisionManager] Paddle GameObject is on layer {paddleGameObject.layer}, expected layer {paddleLayerIndex} ('Paddle')");
            }
        }
        else
        {
            Debug.LogWarning("[CollisionManager] Paddle GameObject not found. Bounce angle calculations will use fallback methods.");
        }
    }
    
    /// <summary>
    /// Initialize collision feedback system components.
    /// </summary>
    private void InitializeFeedbackSystem()
    {
        Debug.Log("[CollisionManager] Initializing collision feedback system...");
        
        // Initialize feedback components
        ValidateFeedbackComponents();
        
        // Initialize camera for screen shake
        if (gameCamera != null)
        {
            originalCameraPosition = gameCamera.transform.position;
            Debug.Log("[CollisionManager] Camera reference cached for screen shake effects");
        }
        else
        {
            // Try to find main camera automatically
            gameCamera = Camera.main;
            if (gameCamera != null)
            {
                originalCameraPosition = gameCamera.transform.position;
                Debug.Log("[CollisionManager] Main camera automatically assigned for screen shake effects");
            }
            else
            {
                Debug.LogWarning("[CollisionManager] No camera found for screen shake effects. Assign manually in Inspector.");
            }
        }
        
        // Initialize particle system emit parameters
        if (collisionParticles != null)
        {
            particleEmitParams = new ParticleSystem.EmitParams();
            Debug.Log("[CollisionManager] Particle system initialized for collision effects");
        }
        
        feedbackSystemInitialized = true;
        Debug.Log("[CollisionManager] Collision feedback system initialized successfully");
    }
    
    /// <summary>
    /// Validate feedback system components and log configuration status.
    /// </summary>
    private void ValidateFeedbackComponents()
    {
        Debug.Log("[CollisionManager] Validating feedback components...");
        
        // Validate AudioSource
        if (audioSource != null)
        {
            Debug.Log("   • AudioSource: Present and ready for collision audio feedback");
        }
        else
        {
            Debug.LogWarning("   • AudioSource: Missing! Audio feedback will be disabled. Add AudioSource component to CollisionManager.");
        }
        
        // Validate ParticleSystem
        if (collisionParticles != null)
        {
            Debug.Log("   • ParticleSystem: Present and ready for collision visual effects");
        }
        else
        {
            Debug.LogWarning("   • ParticleSystem: Missing! Particle effects will be disabled. Add ParticleSystem component to CollisionManager.");
        }
        
        // Validate audio clips
        int clipCount = 0;
        if (paddleBounceClip != null) clipCount++;
        if (wallBounceClip != null) clipCount++;
        if (brickHitClip != null) clipCount++;
        if (powerUpClip != null) clipCount++;
        
        Debug.Log($"   • Audio Clips: {clipCount}/4 assigned (assign remaining clips in Inspector for full audio feedback)");
        
        // Log feedback settings
        Debug.Log($"   • Screen Shake: {(enableScreenShake ? "Enabled" : "Disabled")} (Intensity: {screenShakeIntensity:F2}, Duration: {screenShakeDuration:F2}s)");
        Debug.Log($"   • Audio Feedback: {(enableAudioFeedback ? "Enabled" : "Disabled")}");
        Debug.Log($"   • Particle Feedback: {(enableParticleFeedback ? "Enabled" : "Disabled")} (Burst Count: {particleBurstCount})");
    }
    
    /// <summary>
    /// Initialize validation system for edge case handling.
    /// </summary>
    private void InitializeValidationSystem()
    {
        Debug.Log("[CollisionManager] Initializing collision validation system...");
        
        // Initialize validation data structures
        pendingCollisions.Clear();
        recentCollisions.Clear();
        
        // Initialize ball tracking for stuck detection
        if (ballRigidbody != null)
        {
            lastBallPosition = ballRigidbody.transform.position;
            ballStuckTimer = 0f;
            lastSpeedValidationTime = Time.fixedTime;
            Debug.Log("[CollisionManager] Ball tracking initialized for stuck detection");
        }
        else
        {
            Debug.LogWarning("[CollisionManager] Ball Rigidbody2D not found. Validation system will run with limited functionality.");
        }
        
        // Log validation parameters
        Debug.Log($"[CollisionManager] Validation Parameters:");
        Debug.Log($"   • Speed Constraints: {minBallSpeed:F1} - {maxBallSpeed:F1} units/sec");
        Debug.Log($"   • Stuck Detection: {stuckDetectionTime:F1}s threshold, {stuckVelocityThreshold:F2} velocity limit");
        Debug.Log($"   • Stuck Correction: {stuckCorrectionForce:F1} force magnitude");
        Debug.Log($"   • Max Simultaneous Collisions: {maxSimultaneousCollisions}");
        Debug.Log($"   • Debug Visualization: {(enableValidationDebug ? "Enabled" : "Disabled")}");
        
        validationSystemInitialized = true;
        Debug.Log("[CollisionManager] Collision validation system initialized successfully");
    }
    
    /// <summary>
    /// Validate collision manager setup and configuration.
    /// </summary>
    private void ValidateSetup()
    {
        Debug.Log("[CollisionManager] Validating collision manager setup...");
        
        bool setupValid = true;
        
        // Validate singleton instance
        if (Instance != this)
        {
            Debug.LogError("[CollisionManager] Singleton instance validation failed!");
            setupValid = false;
        }
        
        // Validate Ball connection
        if (ballGameObject == null)
        {
            Debug.LogWarning("[CollisionManager] Ball GameObject not connected. Some collision events may not be captured.");
        }
        
        // Validate layer configuration
        int validLayers = 0;
        if (ballLayerIndex != -1) validLayers++;
        if (paddleLayerIndex != -1) validLayers++;
        if (bricksLayerIndex != -1) validLayers++;
        if (powerUpsLayerIndex != -1) validLayers++;
        if (boundariesLayerIndex != -1) validLayers++;
        
        Debug.Log($"[CollisionManager] Physics layers configured: {validLayers}/5");
        
        if (validLayers < 5)
        {
            Debug.LogWarning("[CollisionManager] Not all physics layers are configured. Run 'Breakout/Setup/Task1131 Create Physics Layers' first.");
        }
        
        if (setupValid && validLayers >= 3) // Ball, Paddle, Boundaries minimum
        {
            Debug.Log("[CollisionManager] Setup validation passed - ready for collision handling");
        }
        else
        {
            Debug.LogWarning("[CollisionManager] Setup validation incomplete - some functionality may not work correctly");
        }
    }
    
    #endregion
    
    #region Collision Event Handling
    
    /// <summary>
    /// Handle collision enter events and route to appropriate handlers.
    /// This method should be called by objects that detect collisions (typically Ball).
    /// </summary>
    /// <param name="collision">Collision2D data from Unity physics system</param>
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;
        
        // Update collision statistics
        totalCollisions++;
        lastCollisionTime = Time.time;
        
        // Determine collision type
        CollisionType collisionType = DetermineCollisionType(collision);
        
        // Log collision event if enabled
        if (enableCollisionLogging)
        {
            LogCollisionEvent("ENTER", collision, collisionType);
        }
        
        // Route collision to appropriate handler
        RouteCollision(collision, collisionType, true);
    }
    
    /// <summary>
    /// Handle collision exit events and route to appropriate handlers.
    /// This method should be called by objects that detect collisions (typically Ball).
    /// </summary>
    /// <param name="collision">Collision2D data from Unity physics system</param>
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision == null) return;
        
        // Determine collision type
        CollisionType collisionType = DetermineCollisionType(collision);
        
        // Log collision event if enabled
        if (enableCollisionLogging)
        {
            LogCollisionEvent("EXIT", collision, collisionType);
        }
        
        // Route collision to appropriate handler
        RouteCollision(collision, collisionType, false);
    }
    
    #endregion
    
    #region Collision Detection and Routing
    
    /// <summary>
    /// Determine collision type based on GameObject layer.
    /// </summary>
    /// <param name="collision">Collision2D data</param>
    /// <returns>CollisionType enum value</returns>
    private CollisionType DetermineCollisionType(Collision2D collision)
    {
        if (collision?.gameObject == null) return CollisionType.Unknown;
        
        int objectLayer = collision.gameObject.layer;
        
        // Match layer index to collision type
        if (objectLayer == paddleLayerIndex) return CollisionType.Paddle;
        if (objectLayer == bricksLayerIndex) return CollisionType.Brick;
        if (objectLayer == boundariesLayerIndex) return CollisionType.Boundary;
        if (objectLayer == powerUpsLayerIndex) return CollisionType.PowerUp;
        
        // Unknown collision type
        return CollisionType.Unknown;
    }
    
    /// <summary>
    /// Route collision to appropriate handler method.
    /// </summary>
    /// <param name="collision">Collision2D data</param>
    /// <param name="collisionType">Determined collision type</param>
    /// <param name="isEnter">True for collision enter, false for collision exit</param>
    private void RouteCollision(Collision2D collision, CollisionType collisionType, bool isEnter)
    {
        switch (collisionType)
        {
            case CollisionType.Paddle:
                HandlePaddleCollision(collision, isEnter);
                break;
                
            case CollisionType.Brick:
                HandleBrickCollision(collision, isEnter);
                break;
                
            case CollisionType.Boundary:
                HandleBoundaryCollision(collision, isEnter);
                break;
                
            case CollisionType.PowerUp:
                HandlePowerUpCollision(collision, isEnter);
                break;
                
            case CollisionType.Unknown:
                HandleUnknownCollision(collision, isEnter);
                break;
        }
    }
    
    #endregion
    
    #region Collision Feedback System
    
    /// <summary>
    /// Trigger collision feedback effects based on collision type and intensity.
    /// </summary>
    /// <param name="collisionType">Type of collision that occurred</param>
    /// <param name="position">World position of collision contact point</param>
    /// <param name="intensity">Collision intensity (0.0 to 1.0)</param>
    private void TriggerCollisionFeedback(CollisionType collisionType, Vector2 position, float intensity)
    {
        if (!feedbackSystemInitialized)
        {
            return;
        }
        
        // Clamp intensity to valid range
        intensity = Mathf.Clamp01(intensity);
        
        if (enableCollisionLogging)
        {
            Debug.Log($"[CollisionManager] Triggering feedback: Type={collisionType}, Position={position}, Intensity={intensity:F2}");
        }
        
        // Trigger audio feedback
        if (enableAudioFeedback)
        {
            TriggerAudioFeedback(collisionType, intensity);
        }
        
        // Trigger particle feedback
        if (enableParticleFeedback)
        {
            TriggerParticleFeedback(collisionType, position, intensity);
        }
        
        // Trigger screen shake feedback
        if (enableScreenShake)
        {
            TriggerScreenShake(intensity);
        }
    }
    
    /// <summary>
    /// Play audio feedback for collision based on type and intensity.
    /// </summary>
    /// <param name="collisionType">Type of collision</param>
    /// <param name="intensity">Audio volume multiplier</param>
    private void TriggerAudioFeedback(CollisionType collisionType, float intensity)
    {
        if (audioSource == null) return;
        
        AudioClip clipToPlay = null;
        
        // Select appropriate audio clip based on collision type
        switch (collisionType)
        {
            case CollisionType.Paddle:
                clipToPlay = paddleBounceClip;
                break;
            case CollisionType.Boundary:
                clipToPlay = wallBounceClip;
                break;
            case CollisionType.Brick:
                clipToPlay = brickHitClip;
                break;
            case CollisionType.PowerUp:
                clipToPlay = powerUpClip;
                break;
        }
        
        if (clipToPlay != null)
        {
            // Play with intensity-based volume (minimum 0.3 to ensure audibility)
            float volume = Mathf.Lerp(0.3f, 1f, intensity);
            audioSource.PlayOneShot(clipToPlay, volume);
            
            if (enableCollisionLogging)
            {
                Debug.Log($"[CollisionManager] Audio feedback: {collisionType} clip at volume {volume:F2}");
            }
        }
        else if (enableCollisionLogging)
        {
            Debug.LogWarning($"[CollisionManager] No audio clip assigned for {collisionType} collision");
        }
    }
    
    /// <summary>
    /// Emit particle effects for collision impact.
    /// </summary>
    /// <param name="collisionType">Type of collision</param>
    /// <param name="position">World position for particle emission</param>
    /// <param name="intensity">Particle count multiplier</param>
    private void TriggerParticleFeedback(CollisionType collisionType, Vector2 position, float intensity)
    {
        if (collisionParticles == null) return;
        
        // Set particle emission position
        particleEmitParams.position = position;
        
        // Set particle color based on collision type
        switch (collisionType)
        {
            case CollisionType.Paddle:
                particleEmitParams.startColor = Color.cyan; // Blue for paddle
                break;
            case CollisionType.Boundary:
                particleEmitParams.startColor = Color.white; // White for walls
                break;
            case CollisionType.Brick:
                particleEmitParams.startColor = Color.yellow; // Yellow for bricks
                break;
            case CollisionType.PowerUp:
                particleEmitParams.startColor = Color.magenta; // Magenta for power-ups
                break;
            default:
                particleEmitParams.startColor = Color.gray; // Gray for unknown
                break;
        }
        
        // Calculate particle count based on intensity
        int particleCount = Mathf.RoundToInt(particleBurstCount * Mathf.Lerp(0.5f, 1.5f, intensity));
        particleCount = Mathf.Clamp(particleCount, 1, particleBurstCount * 2);
        
        // Emit particles
        collisionParticles.Emit(particleEmitParams, particleCount);
        
        if (enableCollisionLogging)
        {
            Debug.Log($"[CollisionManager] Particle feedback: {particleCount} {collisionType} particles at {position}");
        }
    }
    
    /// <summary>
    /// Trigger screen shake effect based on collision intensity.
    /// </summary>
    /// <param name="intensity">Shake intensity multiplier</param>
    private void TriggerScreenShake(float intensity)
    {
        if (gameCamera == null) return;
        
        // Stop any existing shake coroutine
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
        }
        
        // Calculate shake parameters
        float shakeIntensity = screenShakeIntensity * intensity;
        float shakeDuration = screenShakeDuration;
        
        // Start new shake coroutine
        currentShakeCoroutine = StartCoroutine(ScreenShakeCoroutine(shakeIntensity, shakeDuration));
        
        if (enableCollisionLogging)
        {
            Debug.Log($"[CollisionManager] Screen shake: Intensity={shakeIntensity:F3}, Duration={shakeDuration:F2}s");
        }
    }
    
    /// <summary>
    /// Coroutine for smooth screen shake effect.
    /// </summary>
    /// <param name="intensity">Shake intensity</param>
    /// <param name="duration">Shake duration in seconds</param>
    private IEnumerator ScreenShakeCoroutine(float intensity, float duration)
    {
        Vector3 originalPosition = originalCameraPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            // Calculate shake offset with dampening
            float dampen = 1f - (elapsed / duration);
            float shakeX = Random.Range(-1f, 1f) * intensity * dampen;
            float shakeY = Random.Range(-1f, 1f) * intensity * dampen;
            
            // Apply shake offset
            gameCamera.transform.position = originalPosition + new Vector3(shakeX, shakeY, 0f);
            
            yield return null;
        }
        
        // Reset camera position
        gameCamera.transform.position = originalPosition;
        currentShakeCoroutine = null;
    }
    
    /// <summary>
    /// Calculate collision intensity from collision data.
    /// </summary>
    /// <param name="collision">Collision2D data</param>
    /// <returns>Normalized intensity value (0.0 to 1.0)</returns>
    private float CalculateCollisionIntensity(Collision2D collision)
    {
        if (collision == null || collision.relativeVelocity == Vector2.zero)
        {
            return 0.5f; // Default medium intensity
        }
        
        // Calculate intensity based on relative velocity magnitude
        float velocity = collision.relativeVelocity.magnitude;
        float maxVelocity = 20f; // Expected maximum collision velocity
        float intensity = Mathf.Clamp01(velocity / maxVelocity);
        
        // Ensure minimum intensity for feedback visibility
        intensity = Mathf.Max(intensity, 0.2f);
        
        return intensity;
    }
    
    #endregion
    
    #region Edge Case Handling and Validation System
    
    /// <summary>
    /// Main validation method called every FixedUpdate to check for physics anomalies.
    /// </summary>
    private void ValidateCollisionIntegrity()
    {
        if (ballRigidbody == null) return;
        
        // Check for stuck ball scenarios
        DetectStuckBall();
        
        // Validate ball speed constraints
        ValidateSpeedConstraints();
        
        // Process any pending simultaneous collisions
        ProcessPendingCollisions();
        
        // Clean up old collision validation data
        CleanupValidationData();
    }
    
    /// <summary>
    /// Detect if ball is stuck and apply correction if needed.
    /// </summary>
    private void DetectStuckBall()
    {
        Vector2 currentPosition = ballRigidbody.transform.position;
        float currentSpeed = ballRigidbody.linearVelocity.magnitude;
        
        // Check if ball is moving too slowly
        if (currentSpeed < stuckVelocityThreshold)
        {
            ballStuckTimer += Time.fixedDeltaTime;
            
            if (ballStuckTimer >= stuckDetectionTime)
            {
                HandleStuckBall();
                ballStuckTimer = 0f; // Reset timer after correction
            }
        }
        else
        {
            // Ball is moving normally, reset timer
            ballStuckTimer = 0f;
        }
        
        // Update last position for next frame comparison
        lastBallPosition = currentPosition;
        
        if (enableValidationDebug && ballStuckTimer > 0f)
        {
            Debug.Log($"[CollisionManager] Stuck ball detection: Timer={ballStuckTimer:F2}s, Speed={currentSpeed:F2}");
        }
    }
    
    /// <summary>
    /// Apply correction to stuck ball by adding random impulse force.
    /// </summary>
    private void HandleStuckBall()
    {
        if (ballRigidbody == null) return;
        
        // Generate random direction for correction force
        Vector2 randomDirection = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f) // Bias upward to prevent downward corrections
        ).normalized;
        
        // Apply correction impulse
        Vector2 correctionForce = randomDirection * stuckCorrectionForce;
        ballRigidbody.AddForce(correctionForce, ForceMode2D.Impulse);
        
        // Log correction event
        Debug.LogWarning($"[CollisionManager] Stuck ball corrected: Applied force {correctionForce} at position {ballRigidbody.transform.position}");
        
        // Record validation event
        RecordValidationEvent("Stuck Ball Correction", $"Applied force: {correctionForce}, Position: {ballRigidbody.transform.position}");
    }
    
    /// <summary>
    /// Validate and enforce ball speed constraints.
    /// </summary>
    private void ValidateSpeedConstraints()
    {
        if (ballRigidbody == null) return;
        
        Vector2 velocity = ballRigidbody.linearVelocity;
        float currentSpeed = velocity.magnitude;
        bool speedCorrected = false;
        
        // Check minimum speed constraint
        if (currentSpeed < minBallSpeed && currentSpeed > 0.01f) // Avoid correcting nearly zero velocity
        {
            velocity = velocity.normalized * minBallSpeed;
            speedCorrected = true;
            
            if (enableValidationDebug)
            {
                Debug.Log($"[CollisionManager] Speed constraint: Increased speed from {currentSpeed:F2} to {minBallSpeed:F2}");
            }
        }
        // Check maximum speed constraint
        else if (currentSpeed > maxBallSpeed)
        {
            velocity = velocity.normalized * maxBallSpeed;
            speedCorrected = true;
            
            if (enableValidationDebug)
            {
                Debug.Log($"[CollisionManager] Speed constraint: Reduced speed from {currentSpeed:F2} to {maxBallSpeed:F2}");
            }
        }
        
        // Apply corrected velocity if needed
        if (speedCorrected)
        {
            ballRigidbody.linearVelocity = velocity;
            RecordValidationEvent("Speed Constraint", $"Corrected from {currentSpeed:F2} to {velocity.magnitude:F2}");
        }
    }
    
    /// <summary>
    /// Prevent ball tunneling through colliders by validating collision contacts.
    /// </summary>
    /// <param name="collision">Collision data to validate</param>
    private void PreventTunneling(Collision2D collision)
    {
        if (collision == null || collision.contacts.Length == 0) return;
        
        // Check if collision has valid contact points
        ContactPoint2D contact = collision.contacts[0];
        Vector2 contactPoint = contact.point;
        Vector2 contactNormal = contact.normal;
        
        // Validate contact point is reasonable (not too far from objects)
        float distanceToBall = Vector2.Distance(contactPoint, ballRigidbody.transform.position);
        float ballRadius = ballRigidbody.GetComponent<Collider2D>().bounds.size.x * 0.5f;
        
        if (distanceToBall > ballRadius * 2f) // Contact point too far from ball
        {
            // Correct ball position to be just touching the surface
            Vector2 correctedPosition = contactPoint - contactNormal * ballRadius;
            ballRigidbody.transform.position = correctedPosition;
            
            if (enableValidationDebug)
            {
                Debug.LogWarning($"[CollisionManager] Tunneling prevented: Corrected ball position from {ballRigidbody.transform.position} to {correctedPosition}");
            }
            
            RecordValidationEvent("Tunneling Prevention", $"Position corrected: {correctedPosition}");
        }
    }
    
    /// <summary>
    /// Handle multiple simultaneous collisions by priority.
    /// </summary>
    private void ProcessPendingCollisions()
    {
        if (pendingCollisions.Count == 0) return;
        
        // Process up to maximum allowed simultaneous collisions
        int processedCount = 0;
        List<Collision2D> collisionsToProcess = new List<Collision2D>();
        
        // Dequeue collisions up to limit
        while (pendingCollisions.Count > 0 && processedCount < maxSimultaneousCollisions)
        {
            collisionsToProcess.Add(pendingCollisions.Dequeue());
            processedCount++;
        }
        
        // Sort by collision distance (closer collisions processed first)
        collisionsToProcess.Sort((a, b) => {
            float distA = Vector2.Distance(a.contacts[0].point, ballRigidbody.transform.position);
            float distB = Vector2.Distance(b.contacts[0].point, ballRigidbody.transform.position);
            return distA.CompareTo(distB);
        });
        
        // Process collisions in priority order
        foreach (Collision2D collision in collisionsToProcess)
        {
            PreventTunneling(collision);
        }
        
        if (enableValidationDebug && collisionsToProcess.Count > 1)
        {
            Debug.Log($"[CollisionManager] Processed {collisionsToProcess.Count} simultaneous collisions");
        }
        
        // Clear any remaining pending collisions if queue is full
        if (pendingCollisions.Count > maxSimultaneousCollisions * 2)
        {
            Debug.LogWarning("[CollisionManager] Clearing excess pending collisions to prevent memory buildup");
            pendingCollisions.Clear();
        }
    }
    
    /// <summary>
    /// Add collision to pending queue for simultaneous collision handling.
    /// </summary>
    /// <param name="collision">Collision to queue for processing</param>
    private void QueueCollisionForValidation(Collision2D collision)
    {
        if (collision != null && collision.contacts.Length > 0)
        {
            pendingCollisions.Enqueue(collision);
        }
    }
    
    /// <summary>
    /// Record validation event for debugging and analysis.
    /// </summary>
    /// <param name="eventType">Type of validation event</param>
    /// <param name="details">Detailed information about the event</param>
    private void RecordValidationEvent(string eventType, string details)
    {
        if (!enableValidationDebug) return;
        
        // Create validation data entry
        CollisionValidationData validationData = new CollisionValidationData(
            CollisionType.Unknown,
            ballRigidbody != null ? (Vector2)ballRigidbody.transform.position : Vector2.zero,
            ballRigidbody != null ? ballRigidbody.linearVelocity : Vector2.zero,
            Time.time,
            0f
        );
        
        validationData.wasValidated = true;
        validationData.validationResult = $"{eventType}: {details}";
        
        // Add to recent validations list
        recentCollisions.Add(validationData);
        
        // Log validation event
        Debug.Log($"[CollisionManager] Validation Event - {eventType}: {details} at {Time.time:F2}s");
    }
    
    /// <summary>
    /// Clean up old validation data to prevent memory buildup.
    /// </summary>
    private void CleanupValidationData()
    {
        float currentTime = Time.time;
        float maxAge = 10f; // Keep validation data for 10 seconds
        
        // Remove old validation entries
        recentCollisions.RemoveAll(data => currentTime - data.timestamp > maxAge);
        
        // Limit total validation entries
        int maxEntries = 100;
        if (recentCollisions.Count > maxEntries)
        {
            int entriesToRemove = recentCollisions.Count - maxEntries;
            recentCollisions.RemoveRange(0, entriesToRemove);
        }
    }
    
    /// <summary>
    /// Get validation system status for debugging.
    /// </summary>
    /// <returns>Formatted validation status string</returns>
    public string GetValidationStatus()
    {
        if (!validationSystemInitialized)
        {
            return "Validation System: NOT INITIALIZED";
        }
        
        string status = "Collision Validation Status:\n";
        status += $"• Ball Speed: {(ballRigidbody != null ? ballRigidbody.linearVelocity.magnitude.ToString("F2") : "N/A")} units/sec\n";
        status += $"• Speed Constraints: {minBallSpeed:F1} - {maxBallSpeed:F1} units/sec\n";
        status += $"• Stuck Timer: {ballStuckTimer:F2}s / {stuckDetectionTime:F1}s\n";
        status += $"• Pending Collisions: {pendingCollisions.Count}\n";
        status += $"• Recent Validations: {recentCollisions.Count}\n";
        status += $"• Debug Enabled: {enableValidationDebug}";
        
        return status;
    }
    
    #endregion
    
    #region Collision Handler Framework (Stub Methods)
    
    /// <summary>
    /// Handle paddle collision events with bounce angle calculation.
    /// Calculates bounce angle based on paddle hit position for player control.
    /// </summary>
    /// <param name="collision">Collision2D data</param>
    /// <param name="isEnter">True for collision enter, false for collision exit</param>
    private void HandlePaddleCollision(Collision2D collision, bool isEnter)
    {
        if (enableCollisionLogging)
        {
            Debug.Log($"[CollisionManager] HandlePaddleCollision called: {(isEnter ? "ENTER" : "EXIT")} with {collision.gameObject.name}");
        }
        
        // Only calculate bounce on collision enter (not exit)
        if (isEnter)
        {
            // Queue collision for validation processing
            QueueCollisionForValidation(collision);
            
            CalculateAndApplyBounceAngle(collision);
            
            // Trigger collision feedback for paddle bounce
            Vector2 contactPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : (Vector2)collision.transform.position;
            float intensity = CalculateCollisionIntensity(collision);
            TriggerCollisionFeedback(CollisionType.Paddle, contactPoint, intensity);
        }
        
        // Future implementation will add:
        // - Paddle movement influence on ball velocity
        // - Power-up activation triggers
        // - Score/combo multiplier updates
    }
    
    /// <summary>
    /// Handle brick collision events.
    /// Framework method for future brick collision response implementation.
    /// </summary>
    /// <param name="collision">Collision2D data</param>
    /// <param name="isEnter">True for collision enter, false for collision exit</param>
    private void HandleBrickCollision(Collision2D collision, bool isEnter)
    {
        if (enableCollisionLogging)
        {
            Debug.Log($"[CollisionManager] HandleBrickCollision called: {(isEnter ? "ENTER" : "EXIT")} with {collision.gameObject.name}");
        }
        
        // Trigger collision feedback for brick hit
        if (isEnter)
        {
            // Queue collision for validation processing
            QueueCollisionForValidation(collision);
            
            Vector2 contactPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : (Vector2)collision.transform.position;
            float intensity = CalculateCollisionIntensity(collision);
            TriggerCollisionFeedback(CollisionType.Brick, contactPoint, intensity);
        }
        
        // Framework stub - future implementation will add:
        // - Brick destruction logic
        // - Score calculation and awarding
        // - Power-up spawn probability
        // - Level completion detection
    }
    
    /// <summary>
    /// Handle boundary wall collision events.
    /// Framework method for future boundary collision response implementation.
    /// </summary>
    /// <param name="collision">Collision2D data</param>
    /// <param name="isEnter">True for collision enter, false for collision exit</param>
    private void HandleBoundaryCollision(Collision2D collision, bool isEnter)
    {
        if (enableCollisionLogging)
        {
            Debug.Log($"[CollisionManager] HandleBoundaryCollision called: {(isEnter ? "ENTER" : "EXIT")} with {collision.gameObject.name}");
        }
        
        // Trigger collision feedback for wall bounce
        if (isEnter)
        {
            // Queue collision for validation processing
            QueueCollisionForValidation(collision);
            
            Vector2 contactPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : (Vector2)collision.transform.position;
            float intensity = CalculateCollisionIntensity(collision);
            TriggerCollisionFeedback(CollisionType.Boundary, contactPoint, intensity);
        }
        
        // Framework stub - future implementation will add:
        // - Ball out-of-bounds detection (bottom boundary)
        // - Life/ball count decrementation
        // - Ball respawn/reset logic
        // - Game over condition checking
    }
    
    /// <summary>
    /// Handle power-up collision events.
    /// Framework method for future power-up collision response implementation.
    /// </summary>
    /// <param name="collision">Collision2D data</param>
    /// <param name="isEnter">True for collision enter, false for collision exit</param>
    private void HandlePowerUpCollision(Collision2D collision, bool isEnter)
    {
        if (enableCollisionLogging)
        {
            Debug.Log($"[CollisionManager] HandlePowerUpCollision called: {(isEnter ? "ENTER" : "EXIT")} with {collision.gameObject.name}");
        }
        
        // Trigger collision feedback for power-up collection
        if (isEnter)
        {
            // Queue collision for validation processing
            QueueCollisionForValidation(collision);
            
            Vector2 contactPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : (Vector2)collision.transform.position;
            float intensity = CalculateCollisionIntensity(collision);
            TriggerCollisionFeedback(CollisionType.PowerUp, contactPoint, intensity);
        }
        
        // Framework stub - future implementation will add:
        // - Power-up collection and activation
        // - Player ability modifications
        // - Power-up inventory management
        // - Duration-based effect application
    }
    
    /// <summary>
    /// Handle unknown collision types.
    /// Framework method for debugging unexpected collisions.
    /// </summary>
    /// <param name="collision">Collision2D data</param>
    /// <param name="isEnter">True for collision enter, false for collision exit</param>
    private void HandleUnknownCollision(Collision2D collision, bool isEnter)
    {
        if (enableCollisionLogging)
        {
            Debug.LogWarning($"[CollisionManager] HandleUnknownCollision: {(isEnter ? "ENTER" : "EXIT")} with {collision.gameObject.name} on layer {collision.gameObject.layer} ({LayerMask.LayerToName(collision.gameObject.layer)})");
        }
        
        // Framework stub - helps identify missing collision types or misconfigured layers
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Log detailed collision event information.
    /// </summary>
    /// <param name="eventType">ENTER or EXIT</param>
    /// <param name="collision">Collision2D data</param>
    /// <param name="collisionType">Determined collision type</param>
    private void LogCollisionEvent(string eventType, Collision2D collision, CollisionType collisionType)
    {
        string timestamp = Time.time.ToString("F2");
        string objectName = collision.gameObject.name;
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);
        
        Debug.Log($"[CollisionManager] [{timestamp}s] {eventType} | Type: {collisionType} | Object: {objectName} | Layer: {layerName} | Total: {totalCollisions}");
    }
    
    /// <summary>
    /// Get collision statistics for debugging and monitoring.
    /// </summary>
    /// <returns>Formatted collision statistics string</returns>
    public string GetCollisionStatistics()
    {
        return $"CollisionManager Statistics:\n" +
               $"• Total Collisions: {totalCollisions}\n" +
               $"• Last Collision: {(Time.time - lastCollisionTime):F2}s ago\n" +
               $"• Ball Connected: {ballGameObject != null}\n" +
               $"• Logging Enabled: {enableCollisionLogging}\n" +
               $"• Configured Layers: {(ballLayerIndex != -1 ? 1 : 0) + (paddleLayerIndex != -1 ? 1 : 0) + (bricksLayerIndex != -1 ? 1 : 0) + (powerUpsLayerIndex != -1 ? 1 : 0) + (boundariesLayerIndex != -1 ? 1 : 0)}/5";
    }
    
    /// <summary>
    /// Enable or disable collision event logging.
    /// </summary>
    /// <param name="enabled">Whether to enable collision logging</param>
    public void SetCollisionLogging(bool enabled)
    {
        enableCollisionLogging = enabled;
        Debug.Log($"[CollisionManager] Collision logging {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Reset collision statistics.
    /// </summary>
    public void ResetStatistics()
    {
        totalCollisions = 0;
        lastCollisionTime = 0f;
        Debug.Log("[CollisionManager] Collision statistics reset");
    }
    
    #endregion
    
    #region Bounce Angle Calculation
    
    /// <summary>
    /// Calculate and apply bounce angle based on paddle hit position.
    /// </summary>
    /// <param name="collision">Collision2D data containing contact information</param>
    private void CalculateAndApplyBounceAngle(Collision2D collision)
    {
        if (ballRigidbody == null)
        {
            Debug.LogWarning("[CollisionManager] Ball Rigidbody2D not found. Cannot apply bounce angle.");
            return;
        }
        
        try
        {
            // Calculate relative hit position on paddle
            float hitPosition = CalculateHitPosition(collision);
            
            // Map hit position to bounce angle
            float bounceAngle = CalculateBounceAngle(hitPosition);
            
            // Preserve ball speed while changing direction
            Vector2 newVelocity = CalculateBounceVelocity(bounceAngle, ballRigidbody.linearVelocity.magnitude);
            
            // Apply new velocity to ball
            ballRigidbody.linearVelocity = newVelocity;
            
            // Store debug data
            lastHitPosition = collision.contacts[0].point;
            lastBounceAngle = bounceAngle;
            lastBounceVelocity = newVelocity;
            
            if (enableCollisionLogging)
            {
                Debug.Log($"[CollisionManager] Bounce calculated: HitPos={hitPosition:F2}, Angle={bounceAngle:F1}°, Speed={newVelocity.magnitude:F2}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[CollisionManager] Bounce calculation failed: {e.Message}");
            // Continue without bounce modification - ball will use Unity's default physics
        }
    }
    
    /// <summary>
    /// Calculate relative hit position on paddle (-1.0 to 1.0).
    /// </summary>
    /// <param name="collision">Collision2D data</param>
    /// <returns>Hit position from -1.0 (left edge) to 1.0 (right edge)</returns>
    private float CalculateHitPosition(Collision2D collision)
    {
        if (collision.contacts.Length == 0)
        {
            Debug.LogWarning("[CollisionManager] No collision contacts found. Using center hit position.");
            return 0f;
        }
        
        // Get collision contact point
        Vector2 contactPoint = collision.contacts[0].point;
        
        // Get paddle center position
        Vector2 paddleCenter;
        if (paddleGameObject != null)
        {
            paddleCenter = paddleGameObject.transform.position;
        }
        else
        {
            // Fallback: try to get paddle position from collision
            paddleCenter = collision.gameObject.transform.position;
            Debug.LogWarning("[CollisionManager] Using collision GameObject position as paddle center.");
        }
        
        // Calculate relative position
        float relativeX = contactPoint.x - paddleCenter.x;
        float halfWidth = paddleWidth * 0.5f;
        
        // Normalize to -1.0 to 1.0 range
        float hitPosition = Mathf.Clamp(relativeX / halfWidth, -1f, 1f);
        
        if (enableCollisionLogging)
        {
            Debug.Log($"[CollisionManager] Hit position calculation: ContactX={contactPoint.x:F2}, PaddleCenterX={paddleCenter.x:F2}, RelativeX={relativeX:F2}, HitPos={hitPosition:F2}");
        }
        
        return hitPosition;
    }
    
    /// <summary>
    /// Map hit position to bounce angle in degrees.
    /// </summary>
    /// <param name="hitPosition">Hit position from -1.0 to 1.0</param>
    /// <returns>Bounce angle in degrees (15-165 range)</returns>
    private float CalculateBounceAngle(float hitPosition)
    {
        // Map hit position (-1.0 to 1.0) to angle range
        // Hit position -1.0 (left edge) -> maxBounceAngle (165°)
        // Hit position 0.0 (center) -> 90° (straight up)
        // Hit position 1.0 (right edge) -> minBounceAngle (15°)
        
        // Normalize hit position to 0.0-1.0 range
        float normalizedPosition = (hitPosition + 1f) * 0.5f;
        
        // Map to angle range (inverted because left edge should give larger angle)
        float bounceAngle = Mathf.Lerp(maxBounceAngle, minBounceAngle, normalizedPosition);
        
        // Ensure angle stays within valid range
        bounceAngle = Mathf.Clamp(bounceAngle, minBounceAngle, maxBounceAngle);
        
        if (enableCollisionLogging)
        {
            Debug.Log($"[CollisionManager] Angle calculation: HitPos={hitPosition:F2}, NormPos={normalizedPosition:F2}, Angle={bounceAngle:F1}°");
        }
        
        return bounceAngle;
    }
    
    /// <summary>
    /// Calculate new velocity vector from bounce angle and speed.
    /// </summary>
    /// <param name="bounceAngle">Bounce angle in degrees</param>
    /// <param name="ballSpeed">Current ball speed to preserve</param>
    /// <returns>New velocity vector</returns>
    private Vector2 CalculateBounceVelocity(float bounceAngle, float ballSpeed)
    {
        // Convert angle to radians
        float angleRadians = bounceAngle * Mathf.Deg2Rad;
        
        // Calculate velocity components
        float velocityX = Mathf.Cos(angleRadians) * ballSpeed;
        float velocityY = Mathf.Sin(angleRadians) * ballSpeed;
        
        Vector2 newVelocity = new Vector2(velocityX, velocityY);
        
        if (enableCollisionLogging)
        {
            Debug.Log($"[CollisionManager] Velocity calculation: Angle={bounceAngle:F1}°, Speed={ballSpeed:F2}, VelX={velocityX:F2}, VelY={velocityY:F2}");
        }
        
        return newVelocity;
    }
    
    /// <summary>
    /// Get bounce calculation debug information.
    /// </summary>
    /// <returns>Formatted bounce calculation debug string</returns>
    public string GetBounceCalculationDebug()
    {
        return $"Bounce Calculation Debug:\n" +
               $"• Last Hit Position: {lastHitPosition}\n" +
               $"• Last Bounce Angle: {lastBounceAngle:F1}°\n" +
               $"• Last Bounce Velocity: {lastBounceVelocity}\n" +
               $"• Paddle Width: {paddleWidth:F2}\n" +
               $"• Min Bounce Angle: {minBounceAngle:F1}°\n" +
               $"• Max Bounce Angle: {maxBounceAngle:F1}°\n" +
               $"• Ball Rigidbody: {(ballRigidbody != null ? "Connected" : "Missing")}\n" +
               $"• Paddle GameObject: {(paddleGameObject != null ? paddleGameObject.name : "Missing")}";
    }
    
    /// <summary>
    /// Configure bounce calculation parameters.
    /// </summary>
    /// <param name="minAngle">Minimum bounce angle in degrees</param>
    /// <param name="maxAngle">Maximum bounce angle in degrees</param>
    /// <param name="width">Paddle width for hit position calculation</param>
    public void ConfigureBounceCalculation(float minAngle, float maxAngle, float width)
    {
        minBounceAngle = Mathf.Clamp(minAngle, 5f, 45f);
        maxBounceAngle = Mathf.Clamp(maxAngle, 135f, 175f);
        paddleWidth = Mathf.Max(width, 0.5f);
        
        Debug.Log($"[CollisionManager] Bounce calculation configured: MinAngle={minBounceAngle:F1}°, MaxAngle={maxBounceAngle:F1}°, Width={paddleWidth:F2}");
    }
    
    /// <summary>
    /// Test bounce calculation with sample hit position.
    /// </summary>
    /// <param name="hitPosition">Test hit position (-1.0 to 1.0)</param>
    /// <returns>Calculated bounce angle</returns>
    public float TestBounceCalculation(float hitPosition)
    {
        hitPosition = Mathf.Clamp(hitPosition, -1f, 1f);
        float bounceAngle = CalculateBounceAngle(hitPosition);
        
        Debug.Log($"[CollisionManager] Bounce test: HitPos={hitPosition:F2} -> Angle={bounceAngle:F1}°");
        
        return bounceAngle;
    }
    
    #endregion
    
    #region Scene Gizmos
    
    /// <summary>
    /// Draw bounce angle visualization in Scene view.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!enableBounceVisualization || paddleGameObject == null) return;
        
        // Draw paddle width visualization
        Vector3 paddlePos = paddleGameObject.transform.position;
        float halfWidth = paddleWidth * 0.5f;
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(paddlePos + Vector3.left * halfWidth, paddlePos + Vector3.right * halfWidth);
        
        // Draw bounce angle ranges
        Vector3 leftEdge = paddlePos + Vector3.left * halfWidth;
        Vector3 rightEdge = paddlePos + Vector3.right * halfWidth;
        
        // Left edge angle (max angle)
        float leftAngleRad = maxBounceAngle * Mathf.Deg2Rad;
        Vector3 leftDirection = new Vector3(Mathf.Cos(leftAngleRad), Mathf.Sin(leftAngleRad), 0f);
        
        // Right edge angle (min angle)
        float rightAngleRad = minBounceAngle * Mathf.Deg2Rad;
        Vector3 rightDirection = new Vector3(Mathf.Cos(rightAngleRad), Mathf.Sin(rightAngleRad), 0f);
        
        // Center angle (90 degrees)
        Vector3 centerDirection = Vector3.up;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(leftEdge, leftDirection * 2f);
        Gizmos.DrawRay(rightEdge, rightDirection * 2f);
        Gizmos.DrawRay(paddlePos, centerDirection * 2f);
        
        // Draw last hit position if available
        if (lastHitPosition != Vector2.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastHitPosition, 0.1f);
            
            // Draw last bounce direction
            if (lastBounceVelocity != Vector2.zero)
            {
                Vector3 bounceDir = lastBounceVelocity.normalized;
                Gizmos.DrawRay(lastHitPosition, bounceDir * 1.5f);
            }
        }
    }
    
    #endregion
    
    #region Public Configuration
    
    /// <summary>
    /// Set Ball GameObject reference for collision event handling.
    /// </summary>
    /// <param name="ball">Ball GameObject</param>
    public void SetBallGameObject(GameObject ball)
    {
        ballGameObject = ball;
        Debug.Log($"[CollisionManager] Ball GameObject reference updated: {(ball != null ? ball.name : "null")}");
    }
    
    /// <summary>
    /// Check if CollisionManager is properly configured and ready.
    /// </summary>
    /// <returns>True if ready for collision handling</returns>
    public bool IsReady()
    {
        return Instance == this && 
               ballLayerIndex != -1 && 
               paddleLayerIndex != -1 && 
               boundariesLayerIndex != -1;
    }
    
    #endregion
}