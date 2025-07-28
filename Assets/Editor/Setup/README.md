# Editor Setup Scripts

This folder contains Unity Editor scripts for rapid prototyping and scene setup for the Color Memory game.

## Usage

All setup scripts are accessible via Unity's menu system: **Color Memory/Setup/[Action Name]**

## Script Organization

### Naming Convention
- File: `[TaskID]Create[TaskName]Setup.cs`
- Class: `Create[TaskName]Setup`
- Menu: `Color Memory/Setup/Create [Task Name]`

### Current Scripts
- `111CreateCanvasHierarchySetup.cs` - Creates Canvas hierarchy with WebGL configuration

## Development Guidelines

- Include validation to prevent duplicate creation
- Provide comprehensive logging for setup process
- Follow Unity Editor script conventions
- Document component configuration decisions