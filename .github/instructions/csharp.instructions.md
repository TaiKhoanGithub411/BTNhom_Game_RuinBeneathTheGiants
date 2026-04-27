---
applyTo: "**/*.cs"
---

## C# Conventions for this Unity Project

### Naming
- Classes, methods, properties, public fields: PascalCase
- Private fields: _camelCase (underscore prefix)
- Interfaces: prefix `I` — e.g. `IGameService`, `ITrap`
- Abstract base classes: suffix `Base` — e.g. `EnemyBase`, `TrapBase`
- Event payload classes: suffix `Event` — e.g. `PlayerDamagedEvent`
- Constants: SCREAMING_SNAKE_CASE

### Unity-specific
- Serialize fields with `[SerializeField] private`, never `public`
- Use `Awake` for self-initialization, `Start` for cross-component initialization
- Never use `FindObjectOfType` — retrieve dependencies via `ServiceLocator.GetService<T>()`
- Always implement `OnDestroy` on any class that subscribes to EventBus:

```csharp
private void OnDestroy()
{
    _eventBus.Unsubscribe<PlayerDamagedEvent>(HandleDamage);
}
```

### Patterns to follow

**Retrieving a service:**
```csharp
private EventBus _eventBus;

private void Awake()
{
    _eventBus = ServiceLocator.GetService<EventBus>();
}
```

**Publishing an event:**
```csharp
_eventBus.Publish(new PlayerDamagedEvent(damageAmount));
```

**Implementing a new ICombatBehavior:**
```csharp
public class MeleeBehavior : ICombatBehavior
{
    public void Attack() { /* logic */ }
}
```

**Implementing a new IBossState:**
```csharp
public class TelegraphState : IBossState
{
    public void Enter() { }
    public void Execute() { }
    public void Exit() { }
}
```

**Implementing a new ITrap:**
```csharp
public class SpikeTrap : TrapBase
{
    public override void Activate()
    {
        // trigger logic
        _eventBus.Publish(new TrapTriggeredEvent(gameObject));
    }
}
```

### Code Quality
- Prefer `readonly` for fields that are set only in Awake/constructor
- Use `null` checks before publishing events
- Keep MonoBehaviour methods (Update, Awake, etc.) thin — delegate logic to plain C# classes
- Avoid magic numbers; use named constants or ScriptableObject config values
