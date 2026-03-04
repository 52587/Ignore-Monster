# Ignore Monster

A 2D top-down horror survival game built in Unity where the key to survival is simple: **don't look at the monster**.

## Concept

The monster only chases you when you look at it. Stare at it long enough and it will hunt you down — but look away and it returns to patrolling. Navigate the environment, collect a key, and escape through the exit door before your fear consumes you.

## Gameplay

- **Objective:** Find the key hidden in the level and reach the exit door to escape.
- **The Monster:** Patrols a set path. If you look directly at it for long enough, it starts chasing you. Look away long enough and it goes back to patrolling.
- **Fear Meter:** Your fear increases the closer the monster is. If your fear meter fills completely and stays maxed out, you die. Keep your distance to let fear decay.
- **Flashlight:** Illuminates the area around you and is required to see in dark sections. It runs on battery — manage it carefully. Toggle it off to conserve power, but be aware that darkness hides the monster until it's dangerously close.

## Controls

| Input | Action |
|-------|--------|
| `W A S D` | Move |
| `Mouse` | Aim / look direction |
| `F` | Toggle flashlight |
| `Escape` | Pause / Resume |
| `R` | Restart game |

## Game Over Conditions

- The monster catches you (gets too close).
- Your fear meter stays at maximum for too long.

## Features

- **Monster AI** — Three states: Patrol, Idle, and Chase. Transitions are driven by how long the player looks at the monster.
- **Vision Cone** — A cone-based detection system checks whether the monster is within your field of view and line of sight.
- **Fear System** — Fear scales dynamically with proximity to the monster, with visual and audio feedback at each stage (Calm → Uneasy → Nervous → Terrified → PANIC!).
- **Flashlight with Battery** — Drains over time when on; flickers at low battery and shuts off when depleted.
- **Dynamic Audio** — Heartbeat volume and pitch increase with fear; dedicated chase music plays when the monster is actively hunting you.
- **Visual Effects** — Post-processing effects (vignette, chromatic aberration, desaturation, camera shake) intensify as fear rises.
- **Key & Exit** — Collect a key item to unlock the exit door and trigger the win condition.
- **Difficulty Settings** — Easy, Normal, and Hard presets that tune fear rates, battery drain, monster speed, and chase timing.

## Requirements

- **Unity** 6000.2.x (Unity 6)
- Universal Render Pipeline (URP)
- TextMeshPro

## Project Structure

```
Assets/
├── Audio/          # Music, ambience, and sound effects
├── Scenes/         # Game scenes
├── Scripts/
│   ├── Monster/    # MonsterAI
│   ├── Player/     # PlayerController, FearMeter, FlashlightSystem, PlayerVision, FootstepSounds
│   └── Systems/    # GameManager, UIManager, AudioManager, CameraFollow,
│                   # ExitDoor, KeyItem, GameSettings, FearVisualEffects, DebugHelper
└── Settings/       # URP and render pipeline settings
```

## Opening the Project

1. Install **Unity 6000.2.x** via Unity Hub.
2. Clone or download this repository.
3. Open the project folder in Unity Hub.
4. Open `Assets/Scenes/SampleScene.unity`.
5. Press **Play** to start the game.
