# Agent Instructions — Ruins Beneath the Giants

## Bootstrap

This is a Unity project. Do not run `npm install` or `pip install`.
Open the project with Unity Hub using Unity 6 LTS.

## Before Writing Any Code

1. Read `.github/copilot-instructions.md` for architecture overview and patterns
2. Read `.github/instructions/csharp.instructions.md` for C# conventions
3. Identify which system the task belongs to (Core / Player / Enemy / Trap / Item / UI)
   and place new files in the corresponding folder under `Assets/Scripts/`

## Creating New Systems or Classes

- New services must implement `IGameService` and be registered in `ServiceLocator`
- New enemy types must extend `EnemyBase`
- New traps must extend `TrapBase` (not `EnemyBase`)
- New boss states must implement `IBossState`
- New item types must extend `ItemData` as a ScriptableObject
- New combat behaviors must implement `ICombatBehavior`

## Modifying Existing Classes

- Do not add new fields to `PlayerEntity` without a clear reason — prefer adding components
- Do not move `SurvivalTimer` out of `BossStateMachine` into any state class
- Do not change `EventBus` to a static singleton — it must stay as a registered `IGameService`

## Event Naming Convention

When creating new GameEvent types, follow existing pattern:
- `[Subject][Action]Event` — e.g. `PlayerDamagedEvent`, `TrapTriggeredEvent`, `BossWarningEvent`
- Place event classes in `Assets/Scripts/Core/Events/`

## What to Avoid

- Do not use `FindObjectOfType` anywhere
- Do not store item stats directly in MonoBehaviours — use ScriptableObject references
- Do not use `PlayerPrefs` for progression — use `SaveService`
- Do not create new singletons — register via `ServiceLocator` instead
