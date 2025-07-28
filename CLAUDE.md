# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ColorMemory2 is a Unity-based browser memory game where players memorize and repeat increasingly complex color sequences. The game is built for WebGL deployment with a clean, minimalist design.

## Unity Project Information

- **Unity Version**: 6000.0.28f1 (Unity 6)
- **Target Platform**: WebGL (Browser)
- **Rendering Pipeline**: Universal Render Pipeline (URP) 2D
- **Input System**: New Unity Input System (1.11.2)

## Development Commands

Unity projects are typically built and run through the Unity Editor GUI. Common development tasks:

### Building the Project
- Open project in Unity Editor
- Use File → Build Settings to configure WebGL build
- Click "Build" to generate WebGL output

### Running/Testing
- Press Play button in Unity Editor for immediate testing
- For WebGL testing, build and serve the output folder locally

### Package Management
- Packages are managed via Unity Package Manager (Window → Package Manager)
- Package dependencies are defined in `Packages/manifest.json`

## Architecture & Code Structure

### Scene Hierarchy (Planned)
Based on the design specification, the game follows a manager-pattern architecture:

```
ColorMemoryGame/
├── Managers/
│   ├── GameStateManager - Controls game flow (Menu → Playing → GameOver)
│   ├── ScoreManager - Handles scoring and high score persistence
│   ├── SequenceManager - Generates and validates color sequences
│   └── AudioManager - Manages sound effects
├── GameGrid/
│   ├── RedSquare, BlueSquare, GreenSquare, YellowSquare - Interactive color buttons
└── UI/
    ├── Canvas - Main UI container
    ├── ScoreText, LevelText - Game information display
    └── RestartButton - Game restart control
```

### Core Systems

**Game State Management**: Event-driven communication between managers to control game flow
**Data Persistence**: Uses Unity PlayerPrefs for high score storage (WebGL browser persistence)
**Performance Target**: 60fps with minimal GC allocations during gameplay

### Key Enums & Data Structures
```csharp
public enum ColorIndex { Red = 0, Blue = 1, Green = 2, Yellow = 3 }
public enum GameState { Menu, ShowingSequence, WaitingForInput, GameOver }
```

### Color Palette
- Red: #FF4444
- Blue: #4444FF  
- Green: #44FF44
- Yellow: #FFFF44

## Current Development Status

The project is in initial setup phase with:
- Basic Unity project structure established
- URP 2D pipeline configured
- Design specification and game design document created
- One placeholder script (`NewMonoBehaviourScript.cs`) ready for development

## File Organization

- `Assets/Scripts/` - All C# game scripts
- `Assets/Scenes/` - Unity scene files (currently contains SampleScene)
- `Assets/Docs/` - Design specification and game design document
- `ProjectSettings/` - Unity project configuration
- `Packages/` - Unity package dependencies

## Development Guidelines

- Follow Unity naming conventions (PascalCase for public members, camelCase for private)
- Use Unity's component-based architecture with MonoBehaviour scripts
- Implement event-driven communication between managers
- Target WebGL compatibility (avoid platform-specific APIs)
- Maintain 60fps performance target
- Use Unity's built-in UI system (uGUI) for interface elements