using UnityEngine;

/// <summary>
/// Enumeration defining the launch states for ball physics and game flow control.
/// Used by BallController to manage launch mechanics and state transitions.
/// </summary>
public enum BallLaunchState
{
    /// <summary>
    /// Ball is positioned on paddle, ready for launch input.
    /// Ball follows paddle movement and waits for spacebar input.
    /// </summary>
    Ready,
    
    /// <summary>
    /// Ball is in the process of launching.
    /// Direction calculation and velocity application occurs in this state.
    /// </summary>
    Launching,
    
    /// <summary>
    /// Ball is in normal physics gameplay mode.
    /// Ball moves independently with velocity management and collision response.
    /// </summary>
    InPlay
}

/// <summary>
/// Static utility class for BallLaunchState management and validation.
/// Provides state transition logic and validation methods for launch mechanics.
/// </summary>
public static class BallLaunchStateExtensions
{
    /// <summary>
    /// Validates if a state transition is allowed based on launch mechanics rules.
    /// </summary>
    /// <param name="currentState">Current launch state</param>
    /// <param name="targetState">Desired target state</param>
    /// <returns>True if transition is valid, false otherwise</returns>
    public static bool CanTransitionTo(this BallLaunchState currentState, BallLaunchState targetState)
    {
        // Define valid state transitions for launch mechanics
        switch (currentState)
        {
            case BallLaunchState.Ready:
                // From Ready: can launch or stay ready
                return targetState == BallLaunchState.Launching || targetState == BallLaunchState.Ready;
                
            case BallLaunchState.Launching:
                // From Launching: must transition to InPlay (no going back)
                return targetState == BallLaunchState.InPlay;
                
            case BallLaunchState.InPlay:
                // From InPlay: can only return to Ready (ball reset/respawn)
                return targetState == BallLaunchState.Ready;
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Gets a description of the launch state for debugging and logging.
    /// </summary>
    /// <param name="state">Launch state to describe</param>
    /// <returns>Human-readable description of the state</returns>
    public static string GetDescription(this BallLaunchState state)
    {
        switch (state)
        {
            case BallLaunchState.Ready:
                return "Ball positioned on paddle, awaiting launch input";
                
            case BallLaunchState.Launching:
                return "Ball launching with calculated direction and velocity";
                
            case BallLaunchState.InPlay:
                return "Ball in active gameplay with physics control";
                
            default:
                return "Unknown launch state";
        }
    }
    
    /// <summary>
    /// Determines if the ball should be positioned relative to the paddle.
    /// </summary>
    /// <param name="state">Current launch state</param>
    /// <returns>True if ball should follow paddle positioning</returns>
    public static bool RequiresPaddlePositioning(this BallLaunchState state)
    {
        return state == BallLaunchState.Ready;
    }
    
    /// <summary>
    /// Determines if input polling should be active for launch triggering.
    /// </summary>
    /// <param name="state">Current launch state</param>
    /// <returns>True if input should be polled for launch trigger</returns>
    public static bool ShouldPollForInput(this BallLaunchState state)
    {
        return state == BallLaunchState.Ready;
    }
    
    /// <summary>
    /// Determines if velocity management should be active.
    /// </summary>
    /// <param name="state">Current launch state</param>
    /// <returns>True if velocity management should process the ball</returns>
    public static bool RequiresVelocityManagement(this BallLaunchState state)
    {
        return state == BallLaunchState.InPlay;
    }
}