# Ruins Beneath the Giants — Copilot Instructions

## Project Overview

Unity 2D Roguelite survival game. The player navigates post-apocalyptic environments
while avoiding Giants. Core mechanics: resource scavenging, trap avoidance, boss survival.

## Tech Stack

- Engine: Unity 6 (LTS)
- Language: C# 9+
- Version Control: Git
- Asset Data: ScriptableObject-based data assets

## Architecture Patterns

This project enforces specific patterns. Always follow them when generating code.

**Service Locator (Core Systems)**
All major systems (EventBus, UIService, SaveService, RogueliteService) are registered
as IGameService via ServiceLocator. Never use singletons or FindObjectOfType.
Retrieve services with: `ServiceLocator.GetService<T>()`

**Global EventBus**
A single EventBus instance is registered in ServiceLocator. Never instantiate a local
EventBus inside a MonoBehaviour. Always call Unsubscribe in OnDestroy to prevent memory leaks.

**Strategy Pattern (Combat)**
PlayerCombat uses ICombatBehavior. When adding attack types, implement ICombatBehavior,
do not add branches inside PlayerCombat.

**State Machine (Boss AI)**
GiantBoss AI runs on BossStateMachine with IBossState. States: SpawnState, TelegraphState,
AttackState, StageClearState. The SurvivalTimer lives in BossStateMachine, not in any state.

**ScriptableObject Data Assets**
Item data (ItemData, ConsumableData, WeaponData) are ScriptableObjects. Do not hardcode
item stats in MonoBehaviours. InteractableItem holds a reference to ItemData, not the data itself.

**Trap System**
Traps implement ITrap and extend TrapBase. They do not inherit from EnemyBase.
ProximityTrap uses OnTriggerEnter. ConditionalTrap evaluates a TrapCondition.
Both publish TrapTriggeredEvent to EventBus on activation.

## Project Structure

- `Assets/Scripts/Core/`       — ServiceLocator, GameManager, EventBus, SaveService
- `Assets/Scripts/Player/`     — PlayerEntity, PlayerStats, PlayerCombat, IInputProvider
- `Assets/Scripts/Enemy/`      — EnemyBase, MeleeEnemy, RangedEnemy, GiantBoss, BossStateMachine
- `Assets/Scripts/Trap/`       — ITrap, TrapBase, ProximityTrap, ConditionalTrap
- `Assets/Scripts/Item/`       — ItemData, ConsumableData, WeaponData, InteractableItem
- `Assets/Scripts/UI/`         — UIManager, UIService
- `Assets/ScriptableObjects/`  — ItemData assets, level configs

## Coding Conventions

- PascalCase for classes, methods, properties and public fields
- _camelCase (underscore prefix) for private fields
- Interfaces prefixed with `I` (IGameService, ITrap, ICombatBehavior)
- Abstract classes suffixed with `Base` (EnemyBase, TrapBase)
- Event classes suffixed with `Event` (PlayerDamagedEvent, TrapTriggeredEvent)
- One class per file; filename matches class name
- Always use `[SerializeField] private` instead of `public` for Unity-serialized fields
- Prefer composition over inheritance where possible

## Important Notes

- Never use `FindObjectOfType` or static singletons — use ServiceLocator instead
- Always unsubscribe from EventBus in OnDestroy
- StageClearState must distinguish ClearReason.BossDied vs ClearReason.TimerEnded
- TelegraphState must publish BossWarningEvent so UI can display the attack indicator
- Roguelite meta-progression is saved via SaveService, not PlayerPrefs
