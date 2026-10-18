# Tap-and-Fly

A **Unity / C# 3D block-removal puzzle prototype**. Select a block to send it along its assigned direction; blocked moves count against the player, while successful moves clear the board.

The repository name is Tap-and-Fly. Its previous README used the working title `DriftPrototype3D`.

## Systems to explore

| System | Implementation |
| --- | --- |
| Procedural board generation | [`MazeGenerator.cs`](Assets/Scripts/MazeGenerator.cs) builds a 3D grid and uses reverse peeling to assign escape directions. |
| Move validation | [`ObjectMovement.cs`](Assets/Scripts/ObjectMovement.cs) checks the path with a raycast, then moves the block or plays blocked-move feedback. |
| Input | [`RaycastManager.cs`](Assets/Scripts/RaycastManager.cs) maps screen clicks to a block's movement action. |
| Progression | [`GameManager.cs`](Assets/Scripts/GameManager.cs) derives grid dimensions and time allowance from the current level and saves settings with PlayerPrefs. |
| Presentation | [`OrbitCamera.cs`](Assets/Scripts/OrbitCamera.cs), [`Counter.cs`](Assets/Scripts/Counter.cs), and [`LevelButtonManager.cs`](Assets/Scripts/LevelButtonManager.cs) handle the camera, round state, and level UI. |

The generator is intended to construct removable layouts. Its fail-safe and generated boards still need systematic validation; this README does not claim a formal solvability proof.

## Open the project

1. Clone the repository and add the folder in Unity Hub.
2. Use **Unity 6000.3.10f1**, as recorded in [`ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt).
3. Allow package and asset import to complete.
4. Open `Assets/Scenes/Scenes/main menu.unity` and enter Play mode. `Assets/Scenes/Scenes/1.unity` is the first listed gameplay scene.

The project includes DOTween calls and third-party art/UI assets. Preserve their dependencies and respect the original asset licenses. Public repository visibility does not grant unrestricted reuse of those assets.

## Project layout

```text
Assets/Scripts/          Gameplay, generation, camera, progression, and UI
Assets/Scenes/Scenes/    Main menu and numbered gameplay scenes
Packages/               Unity package configuration
ProjectSettings/        Unity version and build settings
apk/                    Existing Android artifacts
```

## Scope

This is a game prototype and code sample. The documentation was checked against repository files; this profile update did not run a fresh Unity build or validate the existing APKs.

For a release, check board generation, blocked moves, timeout and completion behavior, level unlocking, persistence, and input on a physical Android device.
