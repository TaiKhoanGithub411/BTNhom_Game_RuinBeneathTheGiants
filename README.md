# Dự án Game: Ruins Beneath the Giants

## Tổng quan dự án

### Công nghệ ngôn ngữ sử dụng

- Engine: Unity
- Ngôn ngữ: C#
- Quản lý mã nguồn: Git

### Bối cảnh và cốt truyện

Trò chơi lấy bối cảnh trong một thế giới hậu tận thế nơi nhân loại đã sụp đổ và phải sinh tồn dưới chân những thực thể khổng lồ đầy bí ẩn được gọi là Giants. Người chơi hóa thân thành một kẻ sống sót phải băng qua các khu phố đổ nát, khu công nghiệp hoang tàn và những cánh rừng rậm nguy hiểm để tìm kiếm tài nguyên và con đường đến vùng an toàn.

---

## Thiết kế hệ thống chức năng

### 1. Hệ thống cốt lõi (Core Systems)

Hệ thống điều phối vòng lặp trò chơi, UI, lưu trạng thái và quá trình sản sinh môi trường.

#### Sơ đồ hệ thống: Service Locator & Interface (Decoupled Services)

```mermaid
classDiagram
    %% Chuẩn hóa tất cả các hệ thống lớn trong game
    class IGameService {
        <<interface>>
        +Initialize() void
        +Shutdown() void
    }

    %% Lưu trữ và cung cấp các dịch vụ toàn cục trong game
    class ServiceLocator {
        -Dictionary~Type, IGameService~ _services
        +RegisterService(Type, IGameService) void
        +GetService~T~() T
    }

    %% Quản lý luồng và trạng thái của game
    class GameManager {
        -ServiceLocator _locator
        +ChangeState(GameState) void
    }

    %% Xử lý các tính năng đặc thù của thể loại Roguelite
    class RogueliteService {
        +ApplyMetaProgression() void
    }

    %% Điều khiển hiển thị của các màn hình chức năng
    class UIService {
        +OpenScreen(ScreenID) void
    }

    %% Trung tâm giao tiếp sự kiện toàn cục giữa các hệ thống
    class EventBus {
        +Publish(GameEvent) void
        +Subscribe(GameEvent, Action) void
        +Unsubscribe(GameEvent, Action) void
    }

    %% Quản lý lưu và tải trạng thái meta-progression
    class SaveService {
        +SaveProgress() void
        +LoadProgress() void
    }

    %% Relationships
    IGameService <|.. RogueliteService : Implementation
    IGameService <|.. UIService : Implementation
    IGameService <|.. EventBus : Implementation
    IGameService <|.. SaveService : Implementation

    ServiceLocator "1" o-- "n" IGameService : Container
    GameManager ..> ServiceLocator : Dependency
```

#### Đặc điểm hệ thống

- Tuân theo nguyên tắc Dependency Inversion (SOLID)
- Các hệ thống giao tiếp với nhau thông qua interface
- `EventBus` là dịch vụ toàn cục, đăng ký vào `ServiceLocator` — tránh tạo instance cục bộ
- `SaveService` quản lý meta-progression cho thể loại Roguelite
- Dễ mở rộng, dễ tái sử dụng

---

### 2. Hệ thống người chơi (Player Systems)

Quản lý các tài nguyên, trạng thái của người chơi: di chuyển, trạng thái bất lợi, chiến đấu và kho đồ.

#### Sơ đồ hệ thống: Phân tách bằng Event Bus & Strategy (Event-driven Architecture)

```mermaid
classDiagram
    %% Trung tâm giao tiếp sự kiện toàn cục (đăng ký qua ServiceLocator)
    class EventBus {
        +Publish(GameEvent) void
        +Subscribe(GameEvent, Action) void
        +Unsubscribe(GameEvent, Action) void
    }

    %% Cổng giao tiếp điều khiển (không quan tâm nguồn input)
    class IInputProvider {
        <<interface>>
        +GetMoveInput() Vector2
        +IsAttackPressed() bool
    }

    %% Chiến lược tấn công có thể hoán đổi
    class ICombatBehavior {
        <<interface>>
        +Attack() void
    }

    %% Quản lý vòng đời và điều phối giữa các thành phần
    class PlayerEntity {
        +Initialize() void
        +OnDestroy() void
    }

    %% Lưu trữ và xử lý dữ liệu sinh tồn
    class PlayerStats {
        +float HP
        +float MaxHP
        +TakeDamage(float) void
    }

    %% Xử lý logic tấn công
    class PlayerCombat {
        -ICombatBehavior _attackStrategy
        +SetStrategy(ICombatBehavior) void
        +ExecuteAttack() void
    }

    %% Quản lý giao diện - lắng nghe từ EventBus để cập nhật UI
    class UIManager {
        +HandleHealthChangeEvent() void
        +HandleCombatEvent() void
    }

    %% Relationships
    PlayerEntity *-- PlayerStats : Thành phần thiết yếu
    PlayerEntity *-- PlayerCombat : Thành phần thiết yếu
    PlayerEntity o--> IInputProvider : Nhận lệnh từ nguồn ngoài
    PlayerCombat o--> ICombatBehavior : Chiến lược tấn công

    PlayerStats ..> EventBus : Publish PlayerDamagedEvent
    PlayerCombat ..> EventBus : Publish PlayerAttackEvent
    UIManager ..> EventBus : Subscribe để cập nhật giao diện
```

#### Lưu ý quan trọng

- `EventBus` được lấy qua `ServiceLocator.GetService<EventBus>()`, **không** khởi tạo cục bộ trong `PlayerEntity`
- Bắt buộc gọi `Unsubscribe` trong `OnDestroy()` để tránh memory leak khi entity bị hủy

---

### 3. Hệ thống Bẫy và Trùm (Trap & Boss Systems)

#### 3.1 Hệ thống kẻ thù thông thường

Mô hình cây kế thừa cho các loại kẻ thù.

```mermaid
classDiagram
    class EnemyBase {
        <<abstract>>
        +float HP
        +Move() void
        +TakeDamage(float) void
        +Die()* void
    }

    class MeleeEnemy {
        +Die() void
    }

    class RangedEnemy {
        +float AttackRange
        +Attack() void
        +Die() void
    }

    class GiantBoss {
        -BossStateMachine _stateMachine
        +TakeDamage(float) void
        +Die() void
    }

    EnemyBase <|-- MeleeEnemy : Kế thừa toàn bộ hành vi
    EnemyBase <|-- RangedEnemy : Kế thừa toàn bộ hành vi
    EnemyBase <|-- GiantBoss : Kế thừa, mở rộng bằng StateMachine
```

---

#### 3.2 Hệ thống bẫy (Trap System)

Bẫy không kế thừa từ `EnemyBase` — không có HP, không di chuyển, không có AI. Thiết kế tách biệt theo nguyên tắc Interface Segregation (SOLID).

```mermaid
classDiagram
    class ITrap {
        <<interface>>
        +Activate() void
        +Reset() void
    }

    %% Lớp trừu tượng dùng chung cho mọi loại bẫy
    class TrapBase {
        <<abstract>>
        #bool _isActive
        #CheckCondition() bool
        +Activate()* void
        +Reset() void
    }

    %% Kích hoạt khi người chơi đi vào vùng trigger
    class ProximityTrap {
        +OnTriggerEnter(Collider) void
        +Activate() void
    }

    %% Kích hoạt khi đạt điều kiện tùy chỉnh (thời gian, sự kiện, ...)
    class ConditionalTrap {
        +TrapCondition Condition
        +Activate() void
    }

    ITrap <|.. TrapBase
    TrapBase <|-- ProximityTrap : Kích hoạt theo vị trí
    TrapBase <|-- ConditionalTrap : Kích hoạt theo điều kiện

    ProximityTrap ..> EventBus : Publish TrapTriggeredEvent
    ConditionalTrap ..> EventBus : Publish TrapTriggeredEvent
```

---

#### 3.3 Boss State Machine (GiantBoss)

Điều phối AI của Boss qua các trạng thái rõ ràng.

```mermaid
classDiagram
    class IBossState {
        <<interface>>
        +Enter() void
        +Execute() void
        +Exit() void
    }

    %% Quản lý chuyển đổi trạng thái và timer sinh tồn
    class BossStateMachine {
        -IBossState _currentState
        -float _survivalTimer
        +ChangeState(IBossState) void
        +Update() void
    }

    %% Boss xuất hiện, chạy animation vào màn
    class SpawnState {
        +Enter() void
        +Execute() void
        +Exit() void
    }

    %% Báo hiệu hướng tấn công cho người chơi trước khi đánh
    class TelegraphState {
        -float _warningDuration
        +Enter() void
        +Execute() void
        +Exit() void
    }

    %% Thực hiện tấn công
    class AttackState {
        +Enter() void
        +Execute() void
        +Exit() void
    }

    %% Boss chết hoặc người chơi sống sót hết thời gian định sẵn
    class StageClearState {
        +ClearReason Reason
        +Enter() void
        +Execute() void
    }

    BossStateMachine o-- IBossState : Quản lý trạng thái hiện tại
    IBossState <|.. SpawnState
    IBossState <|.. TelegraphState
    IBossState <|.. AttackState
    IBossState <|.. StageClearState

    TelegraphState ..> EventBus : Publish BossWarningEvent
    StageClearState ..> EventBus : Publish StageClearedEvent
```

#### Luồng chuyển trạng thái

```
Spawn ──► Telegraph ──► Attack ──┬──► Telegraph  (lặp lại cho đến khi kết thúc)
                                  │
                                  └──► StageClear
                                           ├── Reason.BossDied   (boss bị tiêu diệt)
                                           └── Reason.TimerEnded (người chơi sống sót đủ thời gian)
```

#### Lưu ý quan trọng

- `SurvivalTimer` đặt trong `BossStateMachine`, chạy xuyên suốt các lần lặp Telegraph → Attack, **không** đặt bên trong `AttackState`
- `TelegraphState` publish `BossWarningEvent` lên `EventBus` để UI hiển thị chỉ báo hướng tấn công
- `StageClearState` phân biệt 2 lý do clear qua enum `ClearReason` — phục vụ logic drop loot và điểm số khác nhau

---

### 4. Hệ thống Vật phẩm và Kho đồ (Item & Inventory Systems)

Quản lý dữ liệu thông tin vật phẩm và tương tác nhặt đồ.

#### Sơ đồ hệ thống: ScriptableObject Kế thừa (Inheritance Data Assets)

```mermaid
classDiagram
    class ItemData {
        <<ScriptableObject>>
        +string ItemName
        +Sprite Icon
        +ItemType Type
    }

    class ConsumableData {
        +float RestoresHP
    }

    class WeaponData {
        +float Damage
        +float MaxDurability
    }

    %% Entity vật lý trong scene, tham chiếu đến dữ liệu ItemData
    class InteractableItem {
        -ItemData _itemRef
        +Pickup() void
    }

    ItemData <|-- ConsumableData : Mở rộng dữ liệu
    ItemData <|-- WeaponData : Mở rộng dữ liệu
    InteractableItem --> ItemData : Tham chiếu dữ liệu
```

#### Đặc điểm hệ thống

- Dữ liệu vật phẩm lưu dưới dạng `ScriptableObject` — chỉnh sửa trực tiếp trong Unity Editor mà không cần sửa code
- `InteractableItem` là MonoBehaviour đặt trong scene, tách biệt hoàn toàn khỏi dữ liệu
- Dễ mở rộng thêm loại vật phẩm mới bằng cách tạo subclass của `ItemData`
