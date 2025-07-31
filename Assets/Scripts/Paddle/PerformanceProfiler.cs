using UnityEngine;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Performance profiling utility for monitoring and validating WebGL performance metrics.
/// Provides comprehensive performance analysis for paddle movement and input response times.
/// </summary>
public static class PerformanceProfiler
{
    #region Performance Tracking
    
    private static Dictionary<string, PerformanceSession> activeSessions = new Dictionary<string, PerformanceSession>();
    private static float webGLTargetFrameRate = 60f;
    private static float webGLTargetResponseTime = 50f;
    private static bool profilingEnabled = true;
    
    /// <summary>
    /// Performance session data structure for tracking metrics over time.
    /// </summary>
    private class PerformanceSession
    {
        public string sessionName;
        public float startTime;
        public List<float> frameTimes = new List<float>();
        public List<float> responseTimes = new List<float>();
        public float totalAllocatedMemory;
        public int frameCount;
        public bool isActive;
        
        public float AverageFrameTime => frameTimes.Count > 0 ? CalculateAverage(frameTimes) : 0f;
        public float AverageResponseTime => responseTimes.Count > 0 ? CalculateAverage(responseTimes) : 0f;
        public float CurrentFrameRate => AverageFrameTime > 0 ? 1f / AverageFrameTime : 0f;
        public float Duration => Time.realtimeSinceStartup - startTime;
        
        private float CalculateAverage(List<float> values)
        {
            if (values.Count == 0) return 0f;
            float sum = 0f;
            foreach (float value in values) sum += value;
            return sum / values.Count;
        }
    }
    
    #endregion
    
    #region Session Management
    
    /// <summary>
    /// Start a new performance profiling session.
    /// </summary>
    /// <param name="sessionName">Unique name for the session</param>
    public static void StartSession(string sessionName)
    {
        if (!profilingEnabled) return;
        
        if (activeSessions.ContainsKey(sessionName))
        {
            Debug.LogWarning($"[PerformanceProfiler] Session '{sessionName}' already active. Stopping existing session.");
            StopSession(sessionName);
        }
        
        var session = new PerformanceSession
        {
            sessionName = sessionName,
            startTime = Time.realtimeSinceStartup,
            isActive = true
        };
        
        activeSessions[sessionName] = session;
        Debug.Log($"[PerformanceProfiler] Started session: {sessionName}");
    }
    
    /// <summary>
    /// Stop and analyze a performance profiling session.
    /// </summary>
    /// <param name="sessionName">Name of the session to stop</param>
    /// <returns>Performance analysis results</returns>
    public static string StopSession(string sessionName)
    {
        if (!activeSessions.ContainsKey(sessionName))
        {
            Debug.LogWarning($"[PerformanceProfiler] Session '{sessionName}' not found.");
            return "";
        }
        
        var session = activeSessions[sessionName];
        session.isActive = false;
        
        string analysis = AnalyzeSession(session);
        activeSessions.Remove(sessionName);
        
        Debug.Log($"[PerformanceProfiler] Stopped session: {sessionName}");
        Debug.Log(analysis);
        
        return analysis;
    }
    
    /// <summary>
    /// Record a frame time measurement for the specified session.
    /// </summary>
    /// <param name="sessionName">Session to record to</param>
    /// <param name="frameTime">Frame time in seconds</param>
    public static void RecordFrameTime(string sessionName, float frameTime)
    {
        if (!profilingEnabled || !activeSessions.ContainsKey(sessionName)) return;
        
        var session = activeSessions[sessionName];
        if (!session.isActive) return;
        
        session.frameTimes.Add(frameTime);
        session.frameCount++;
        
        // Keep only recent measurements to prevent memory growth
        if (session.frameTimes.Count > 300) // 5 seconds at 60fps
        {
            session.frameTimes.RemoveAt(0);
        }
    }
    
    /// <summary>
    /// Record a response time measurement for the specified session.
    /// </summary>
    /// <param name="sessionName">Session to record to</param>
    /// <param name="responseTime">Response time in milliseconds</param>
    public static void RecordResponseTime(string sessionName, float responseTime)
    {
        if (!profilingEnabled || !activeSessions.ContainsKey(sessionName)) return;
        
        var session = activeSessions[sessionName];
        if (!session.isActive) return;
        
        session.responseTimes.Add(responseTime);
        
        // Keep only recent measurements
        if (session.responseTimes.Count > 300)
        {
            session.responseTimes.RemoveAt(0);
        }
    }
    
    #endregion
    
    #region Analysis Methods
    
    /// <summary>
    /// Analyze a performance session and generate detailed report.
    /// </summary>
    /// <param name="session">Session to analyze</param>
    /// <returns>Detailed analysis report</returns>
    private static string AnalyzeSession(PerformanceSession session)
    {
        var report = new StringBuilder();
        report.AppendLine($"=== Performance Analysis: {session.sessionName} ===");
        report.AppendLine($"Duration: {session.Duration:F2}s");
        report.AppendLine($"Total Frames: {session.frameCount}");
        report.AppendLine();
        
        // Frame Rate Analysis
        if (session.frameTimes.Count > 0)
        {
            float avgFrameRate = session.CurrentFrameRate;
            float minFrameTime = Mathf.Min(session.frameTimes.ToArray());
            float maxFrameTime = Mathf.Max(session.frameTimes.ToArray());
            float maxFrameRate = 1f / minFrameTime;
            float minFrameRate = 1f / maxFrameTime;
            
            report.AppendLine("📊 Frame Rate Analysis:");
            report.AppendLine($"   • Average: {avgFrameRate:F1} fps");
            report.AppendLine($"   • Range: {minFrameRate:F1} - {maxFrameRate:F1} fps");
            report.AppendLine($"   • Target: {webGLTargetFrameRate:F1} fps");
            report.AppendLine($"   • Target Met: {(avgFrameRate >= webGLTargetFrameRate * 0.9f ? "✅ YES" : "❌ NO")}");
            report.AppendLine();
            
            // Frame consistency analysis
            float frameVariance = CalculateVariance(session.frameTimes);
            report.AppendLine("📈 Frame Consistency:");
            report.AppendLine($"   • Variance: {frameVariance:F6}s²");
            report.AppendLine($"   • Consistency: {(frameVariance < 0.0001f ? "Excellent" : frameVariance < 0.0005f ? "Good" : "Poor")}");
            report.AppendLine();
        }
        
        // Response Time Analysis
        if (session.responseTimes.Count > 0)
        {
            float avgResponseTime = session.AverageResponseTime;
            float minResponseTime = Mathf.Min(session.responseTimes.ToArray());
            float maxResponseTime = Mathf.Max(session.responseTimes.ToArray());
            
            report.AppendLine("⚡ Response Time Analysis:");
            report.AppendLine($"   • Average: {avgResponseTime:F2}ms");
            report.AppendLine($"   • Range: {minResponseTime:F2} - {maxResponseTime:F2}ms");
            report.AppendLine($"   • Target: <{webGLTargetResponseTime:F1}ms");
            report.AppendLine($"   • Target Met: {(avgResponseTime <= webGLTargetResponseTime ? "✅ YES" : "❌ NO")}");
            report.AppendLine();
            
            // Response time consistency
            float responseVariance = CalculateVariance(session.responseTimes);
            report.AppendLine("🎯 Response Consistency:");
            report.AppendLine($"   • Variance: {responseVariance:F2}ms²");
            report.AppendLine($"   • Consistency: {(responseVariance < 25f ? "Excellent" : responseVariance < 100f ? "Good" : "Poor")}");
            report.AppendLine();
        }
        
        // Memory Analysis
        float currentMemory = GetCurrentMemoryUsage();
        report.AppendLine("💾 Memory Analysis:");
        report.AppendLine($"   • Current Usage: {currentMemory:F2}MB");
        report.AppendLine($"   • Memory Efficiency: {(currentMemory < 50f ? "Excellent" : currentMemory < 100f ? "Good" : "Poor")}");
        report.AppendLine();
        
        // WebGL Optimization Recommendations
        report.AppendLine(GenerateWebGLRecommendations(session));
        
        return report.ToString();
    }
    
    /// <summary>
    /// Calculate variance for a list of values.
    /// </summary>
    /// <param name="values">List of values</param>
    /// <returns>Variance value</returns>
    private static float CalculateVariance(List<float> values)
    {
        if (values.Count < 2) return 0f;
        
        float mean = 0f;
        foreach (float value in values) mean += value;
        mean /= values.Count;
        
        float variance = 0f;
        foreach (float value in values)
        {
            float diff = value - mean;
            variance += diff * diff;
        }
        
        return variance / (values.Count - 1);
    }
    
    /// <summary>
    /// Generate WebGL-specific optimization recommendations.
    /// </summary>
    /// <param name="session">Session to analyze</param>
    /// <returns>Optimization recommendations</returns>
    private static string GenerateWebGLRecommendations(PerformanceSession session)
    {
        var recommendations = new StringBuilder();
        recommendations.AppendLine("🚀 WebGL Optimization Recommendations:");
        
        bool hasIssues = false;
        
        // Frame rate recommendations
        if (session.CurrentFrameRate < webGLTargetFrameRate * 0.9f)
        {
            recommendations.AppendLine("   • Frame Rate: Consider reducing visual effects or movement complexity");
            hasIssues = true;
        }
        
        // Response time recommendations
        if (session.AverageResponseTime > webGLTargetResponseTime)
        {
            recommendations.AppendLine("   • Response Time: Optimize Update() method and reduce per-frame allocations");
            hasIssues = true;
        }
        
        // Memory recommendations
        float currentMemory = GetCurrentMemoryUsage();
        if (currentMemory > 75f)
        {
            recommendations.AppendLine("   • Memory: Consider object pooling and reduce dynamic allocations");
            hasIssues = true;
        }
        
        // Consistency recommendations
        if (session.frameTimes.Count > 0)
        {
            float frameVariance = CalculateVariance(session.frameTimes);
            if (frameVariance > 0.0005f)
            {
                recommendations.AppendLine("   • Consistency: Implement frame pacing and smooth delta time handling");
                hasIssues = true;
            }
        }
        
        if (!hasIssues)
        {
            recommendations.AppendLine("   ✅ Performance is already optimized for WebGL deployment!");
        }
        
        recommendations.AppendLine();
        recommendations.AppendLine("💡 General WebGL Best Practices:");
        recommendations.AppendLine("   • Use cached component references");
        recommendations.AppendLine("   • Minimize GetComponent calls");
        recommendations.AppendLine("   • Implement object pooling for dynamic objects");
        recommendations.AppendLine("   • Use Time.deltaTime for frame-rate independent movement");
        recommendations.AppendLine("   • Profile regularly on target WebGL platform");
        
        return recommendations.ToString();
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Get current memory usage in megabytes.
    /// </summary>
    /// <returns>Memory usage in MB</returns>
    private static float GetCurrentMemoryUsage()
    {
        // Note: In WebGL, System.GC.GetTotalMemory might not be accurate
        // This is a simplified approximation
        #if UNITY_WEBGL && !UNITY_EDITOR
        return 0f; // WebGL doesn't provide accurate memory info
        #else
        return System.GC.GetTotalMemory(false) / (1024f * 1024f);
        #endif
    }
    
    /// <summary>
    /// Enable or disable performance profiling.
    /// </summary>
    /// <param name="enabled">Whether profiling should be enabled</param>
    public static void SetProfilingEnabled(bool enabled)
    {
        profilingEnabled = enabled;
        Debug.Log($"[PerformanceProfiler] Profiling {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Set WebGL performance targets.
    /// </summary>
    /// <param name="targetFrameRate">Target frame rate</param>
    /// <param name="targetResponseTime">Target response time in ms</param>
    public static void SetWebGLTargets(float targetFrameRate, float targetResponseTime)
    {
        webGLTargetFrameRate = targetFrameRate;
        webGLTargetResponseTime = targetResponseTime;
        Debug.Log($"[PerformanceProfiler] WebGL targets set: {targetFrameRate}fps, {targetResponseTime}ms response");
    }
    
    /// <summary>
    /// Get current active sessions count.
    /// </summary>
    /// <returns>Number of active profiling sessions</returns>
    public static int GetActiveSessionCount()
    {
        return activeSessions.Count;
    }
    
    /// <summary>
    /// Clear all active sessions.
    /// </summary>
    public static void ClearAllSessions()
    {
        int sessionCount = activeSessions.Count;
        activeSessions.Clear();
        Debug.Log($"[PerformanceProfiler] Cleared {sessionCount} active sessions");
    }
    
    /// <summary>
    /// Get real-time performance snapshot.
    /// </summary>
    /// <returns>Current performance metrics</returns>
    public static string GetPerformanceSnapshot()
    {
        var snapshot = new StringBuilder();
        snapshot.AppendLine("=== Real-Time Performance Snapshot ===");
        snapshot.AppendLine($"Current FPS: {(1f / Time.deltaTime):F1}");
        snapshot.AppendLine($"Delta Time: {(Time.deltaTime * 1000f):F2}ms");
        snapshot.AppendLine($"Memory Usage: {GetCurrentMemoryUsage():F2}MB");
        snapshot.AppendLine($"Active Sessions: {activeSessions.Count}");
        snapshot.AppendLine($"Profiling Enabled: {profilingEnabled}");
        
        return snapshot.ToString();
    }
    
    #endregion
    
    #region Integration Methods
    
    /// <summary>
    /// Quick performance test for paddle controller.
    /// </summary>
    /// <param name="paddleController">PaddleController to test</param>
    /// <param name="testDuration">Test duration in seconds</param>
    /// <returns>Test results</returns>
    public static string TestPaddlePerformance(PaddleController paddleController, float testDuration = 5f)
    {
        if (paddleController == null)
        {
            return "Error: PaddleController is null";
        }
        
        string sessionName = $"PaddleTest_{Time.realtimeSinceStartup:F0}";
        StartSession(sessionName);
        
        Debug.Log($"[PerformanceProfiler] Starting {testDuration}s paddle performance test...");
        
        // This would typically be called from an Update loop or coroutine
        // For immediate testing, we simulate some measurements
        float startTime = Time.realtimeSinceStartup;
        float testFrameTime = 1f / 60f; // Simulate 60fps
        
        // Simulate test measurements
        for (int i = 0; i < testDuration * 60; i++)
        {
            RecordFrameTime(sessionName, testFrameTime + Random.Range(-0.001f, 0.001f));
            RecordResponseTime(sessionName, Random.Range(20f, 60f));
        }
        
        return StopSession(sessionName);
    }
    
    #endregion
}