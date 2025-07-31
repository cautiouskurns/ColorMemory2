using UnityEngine;

/// <summary>
/// Enumeration defining all available brick types for Breakout gameplay.
/// Each type has distinct behavior characteristics and destruction patterns.
/// </summary>
public enum BrickType
{
    /// <summary>
    /// Standard brick - 1 hit to destroy, basic scoring
    /// </summary>
    Normal,
    
    /// <summary>
    /// Reinforced brick - 2 hits to destroy, higher scoring
    /// </summary>
    Reinforced,
    
    /// <summary>
    /// Indestructible brick - Cannot be destroyed, no scoring
    /// </summary>
    Indestructible,
    
    /// <summary>
    /// Power-up brick - 1 hit to destroy, spawns power-up when destroyed
    /// </summary>
    PowerUp
}

/// <summary>
/// Configuration data structure for brick behavior and visual properties.
/// Contains all properties needed to define brick characteristics and gameplay effects.
/// Serializable for Unity Inspector integration and data persistence.
/// </summary>
[System.Serializable]
public class BrickData
{
    #region Public Properties
    
    [Header("Brick Configuration")]
    [Tooltip("Type of brick determining behavior and destruction characteristics")]
    public BrickType brickType;
    
    [Tooltip("Number of hits required to destroy this brick (0 = indestructible)")]
    [Range(0, 5)]
    public int hitPoints;
    
    [Tooltip("Score awarded when this brick is destroyed")]
    [Range(0, 1000)]
    public int scoreValue;
    
    [Header("Visual Properties")]
    [Tooltip("Primary color for brick rendering")]
    public Color brickColor;
    
    [Tooltip("Whether this brick should display destruction particle effects")]
    public bool hasDestructionEffects;
    
    [Header("Advanced Properties")]
    [Tooltip("Whether this brick can spawn power-ups when destroyed")]
    public bool canSpawnPowerUp;
    
    [Tooltip("Damage multiplier applied to collision feedback intensity")]
    [Range(0.5f, 2.0f)]
    public float damageMultiplier;
    
    #endregion
    
    #region Constructors
    
    /// <summary>
    /// Default constructor - creates Normal brick configuration
    /// </summary>
    public BrickData() : this(BrickType.Normal)
    {
    }
    
    /// <summary>
    /// Creates BrickData with default values for specified brick type.
    /// Provides sensible configuration based on gameplay requirements.
    /// </summary>
    /// <param name="type">Brick type to configure</param>
    public BrickData(BrickType type)
    {
        brickType = type;
        ConfigureDefaultValues(type);
    }
    
    /// <summary>
    /// Full constructor for custom brick configuration
    /// </summary>
    /// <param name="type">Brick type</param>
    /// <param name="hitPoints">Hits required to destroy</param>
    /// <param name="scoreValue">Score awarded on destruction</param>
    /// <param name="color">Brick color</param>
    /// <param name="hasEffects">Enable destruction effects</param>
    public BrickData(BrickType type, int hitPoints, int scoreValue, Color color, bool hasEffects = true)
    {
        this.brickType = type;
        this.hitPoints = Mathf.Max(0, hitPoints);
        this.scoreValue = Mathf.Max(0, scoreValue);
        this.brickColor = color;
        this.hasDestructionEffects = hasEffects;
        this.canSpawnPowerUp = (type == BrickType.PowerUp);
        this.damageMultiplier = GetDefaultDamageMultiplier(type);
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Checks if this brick can be destroyed by collision
    /// </summary>
    /// <returns>True if brick is destructible</returns>
    public bool IsDestructible()
    {
        return brickType != BrickType.Indestructible && hitPoints > 0;
    }
    
    /// <summary>
    /// Checks if this brick awards score when destroyed
    /// </summary>
    /// <returns>True if brick provides scoring</returns>
    public bool AwardsScore()
    {
        return IsDestructible() && scoreValue > 0;
    }
    
    /// <summary>
    /// Gets the effective collision damage multiplier for feedback systems
    /// </summary>
    /// <returns>Damage multiplier for collision effects</returns>
    public float GetDamageMultiplier()
    {
        return IsDestructible() ? damageMultiplier : 0.1f; // Minimal effect for indestructible
    }
    
    /// <summary>
    /// Validates brick data configuration and fixes invalid values
    /// </summary>
    public void ValidateConfiguration()
    {
        // Ensure hit points are valid
        if (brickType == BrickType.Indestructible)
        {
            hitPoints = 0; // Indestructible bricks have 0 hit points
            scoreValue = 0; // No score for indestructible bricks
        }
        else if (hitPoints <= 0)
        {
            hitPoints = 1; // Destructible bricks need at least 1 hit point
        }
        
        // Ensure power-up configuration is consistent
        if (brickType == BrickType.PowerUp)
        {
            canSpawnPowerUp = true;
        }
        else if (brickType != BrickType.PowerUp && canSpawnPowerUp)
        {
            // Only PowerUp type should spawn power-ups
            canSpawnPowerUp = false;
        }
        
        // Clamp damage multiplier to valid range
        damageMultiplier = Mathf.Clamp(damageMultiplier, 0.5f, 2.0f);
        
        // Ensure score value is non-negative
        scoreValue = Mathf.Max(0, scoreValue);
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// Configures default values based on brick type for balanced gameplay
    /// </summary>
    /// <param name="type">Brick type to configure</param>
    private void ConfigureDefaultValues(BrickType type)
    {
        switch (type)
        {
            case BrickType.Normal:
                hitPoints = 1;
                scoreValue = 100;
                brickColor = Color.red;
                hasDestructionEffects = true;
                canSpawnPowerUp = false;
                damageMultiplier = 1.0f;
                break;
                
            case BrickType.Reinforced:
                hitPoints = 2;
                scoreValue = 200;
                brickColor = Color.blue;
                hasDestructionEffects = true;
                canSpawnPowerUp = false;
                damageMultiplier = 1.2f;
                break;
                
            case BrickType.Indestructible:
                hitPoints = 0; // Cannot be destroyed
                scoreValue = 0; // No score awarded
                brickColor = Color.gray;
                hasDestructionEffects = false;
                canSpawnPowerUp = false;
                damageMultiplier = 0.1f; // Minimal collision feedback
                break;
                
            case BrickType.PowerUp:
                hitPoints = 1;
                scoreValue = 150;
                brickColor = Color.yellow;
                hasDestructionEffects = true;
                canSpawnPowerUp = true;
                damageMultiplier = 1.1f;
                break;
                
            default:
                // Fallback to Normal configuration
                ConfigureDefaultValues(BrickType.Normal);
                Debug.LogWarning($"[BrickData] Unknown brick type: {type}. Using Normal configuration.");
                break;
        }
    }
    
    /// <summary>
    /// Gets default damage multiplier for specified brick type
    /// </summary>
    /// <param name="type">Brick type</param>
    /// <returns>Default damage multiplier</returns>
    private float GetDefaultDamageMultiplier(BrickType type)
    {
        return type switch
        {
            BrickType.Normal => 1.0f,
            BrickType.Reinforced => 1.2f,
            BrickType.Indestructible => 0.1f,
            BrickType.PowerUp => 1.1f,
            _ => 1.0f
        };
    }
    
    #endregion
    
    #region Static Factory Methods
    
    /// <summary>
    /// Creates a Normal brick with default configuration
    /// </summary>
    /// <returns>Configured Normal brick data</returns>
    public static BrickData CreateNormal()
    {
        return new BrickData(BrickType.Normal);
    }
    
    /// <summary>
    /// Creates a Reinforced brick with default configuration
    /// </summary>
    /// <returns>Configured Reinforced brick data</returns>
    public static BrickData CreateReinforced()
    {
        return new BrickData(BrickType.Reinforced);
    }
    
    /// <summary>
    /// Creates an Indestructible brick with default configuration
    /// </summary>
    /// <returns>Configured Indestructible brick data</returns>
    public static BrickData CreateIndestructible()
    {
        return new BrickData(BrickType.Indestructible);
    }
    
    /// <summary>
    /// Creates a PowerUp brick with default configuration
    /// </summary>
    /// <returns>Configured PowerUp brick data</returns>
    public static BrickData CreatePowerUp()
    {
        return new BrickData(BrickType.PowerUp);
    }
    
    /// <summary>
    /// Creates custom brick configuration with validation
    /// </summary>
    /// <param name="type">Brick type</param>
    /// <param name="hitPoints">Hits to destroy</param>
    /// <param name="scoreValue">Score awarded</param>
    /// <param name="color">Brick color</param>
    /// <returns>Validated custom brick data</returns>
    public static BrickData CreateCustom(BrickType type, int hitPoints, int scoreValue, Color color)
    {
        BrickData customBrick = new BrickData(type, hitPoints, scoreValue, color);
        customBrick.ValidateConfiguration();
        return customBrick;
    }
    
    /// <summary>
    /// Creates array of default brick configurations for level generation
    /// </summary>
    /// <returns>Array containing all default brick types</returns>
    public static BrickData[] CreateDefaultSet()
    {
        return new BrickData[]
        {
            CreateNormal(),
            CreateReinforced(),
            CreateIndestructible(),
            CreatePowerUp()
        };
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Gets display name for brick type (useful for UI and debugging)
    /// </summary>
    /// <returns>Human-readable brick type name</returns>
    public string GetDisplayName()
    {
        return brickType switch
        {
            BrickType.Normal => "Normal Brick",
            BrickType.Reinforced => "Reinforced Brick",
            BrickType.Indestructible => "Indestructible Brick",
            BrickType.PowerUp => "Power-Up Brick",
            _ => "Unknown Brick"
        };
    }
    
    /// <summary>
    /// Gets brick description for tooltips and documentation
    /// </summary>
    /// <returns>Brick behavior description</returns>
    public string GetDescription()
    {
        return brickType switch
        {
            BrickType.Normal => $"Standard brick requiring {hitPoints} hit to destroy. Awards {scoreValue} points.",
            BrickType.Reinforced => $"Reinforced brick requiring {hitPoints} hits to destroy. Awards {scoreValue} points.",
            BrickType.Indestructible => "Cannot be destroyed. Provides no score.",
            BrickType.PowerUp => $"Special brick that spawns power-up when destroyed. Awards {scoreValue} points.",
            _ => "Unknown brick type"
        };
    }
    
    /// <summary>
    /// Creates a deep copy of this BrickData instance
    /// </summary>
    /// <returns>Independent copy of brick data</returns>
    public BrickData Clone()
    {
        return new BrickData(brickType, hitPoints, scoreValue, brickColor, hasDestructionEffects)
        {
            canSpawnPowerUp = this.canSpawnPowerUp,
            damageMultiplier = this.damageMultiplier
        };
    }
    
    #endregion
    
    #region Debug and Validation
    
    /// <summary>
    /// Returns detailed debug information about this brick configuration
    /// </summary>
    /// <returns>Debug information string</returns>
    public override string ToString()
    {
        return $"BrickData [{GetDisplayName()}]: HP={hitPoints}, Score={scoreValue}, " +
               $"Color={brickColor}, Effects={hasDestructionEffects}, PowerUp={canSpawnPowerUp}, " +
               $"Multiplier={damageMultiplier:F1}";
    }
    
    /// <summary>
    /// Validates that this brick data configuration is gameplay-ready
    /// </summary>
    /// <returns>True if configuration is valid</returns>
    public bool IsValidConfiguration()
    {
        // Check basic constraints
        if (hitPoints < 0 || scoreValue < 0) return false;
        if (damageMultiplier < 0.5f || damageMultiplier > 2.0f) return false;
        
        // Check type-specific constraints
        switch (brickType)
        {
            case BrickType.Indestructible:
                return hitPoints == 0 && scoreValue == 0;
                
            case BrickType.PowerUp:
                return hitPoints > 0 && canSpawnPowerUp;
                
            case BrickType.Normal:
            case BrickType.Reinforced:
                return hitPoints > 0;
                
            default:
                return false;
        }
    }
    
    #endregion
}