# Project: 2D Endless Side-Scrolling Runner

## Tổng quan
Game runner 2D góc nhìn ngang, **có yếu tố tiến-lùi** (không phải one-direction runner truyền thống). Engine: Unity, ngôn ngữ: C#. Đây là dự án học tập của sinh viên IT năm nhất — ưu tiên code rõ ràng, dễ đọc, có comment giải thích.

## Kiến trúc — 5 lớp logic
```
Input → Player State → World Interaction → Encounter → Persistence
```

## Cấu trúc thư mục
```
Assets/
├── Scripts/
│   ├── Core/          ← GameManager, EventBus, interfaces chung
│   ├── Player/        ← PlayerController, PlayerMotor, PlayerVitals, StatusEffect
│   ├── Items/         ← ItemPickup, ItemEffect, ItemDatabase
│   ├── Traps/         ← TrapBase, SpikeTrap, PoisonCloud...
│   ├── Boss/          ← BossEncounterTrigger, BossController, BossAttackPattern
│   ├── Save/          ← SaveService, SaveData (DTO), ISaveable
│   └── UI/            ← HUD, HealthBar, StaminaBar
└── ScriptableObjects/
    └── Data/          ← ItemData, BossAttack, StatusEffectData
```

## Quy ước lập trình
- Mỗi class một file, tên file = tên class
- Dùng `[SerializeField] private` thay vì `public` cho Unity Inspector
- Dùng C# event (`Action`, `Func`) để giao tiếp giữa hệ thống
- ScriptableObject cho mọi dữ liệu cấu hình (item, boss attack, status effect)
- Không dùng singleton tràn lan — chỉ GameManager và SaveService được phép singleton
- Comment bằng tiếng Việt hoặc tiếng Anh nhất quán trong từng file

## Những việc KHÔNG làm
- ❌ Serialize MonoBehaviour hoặc GameObject vào save file
- ❌ Gọi `FindObjectOfType` trong Update() — cache trong Awake/Start
- ❌ Để Boss tự kiểm tra điều kiện spawn của mình
- ❌ Gộp logic physics và logic sinh lực vào cùng 1 class
- ❌ Hardcode logic cho từng loại item/trap/effect — dùng data-driven

## Giai đoạn phát triển hiện tại
| Phase | Trạng thái | Nội dung |
|---|---|---|
| Phase 1 | 🔧 Hiện tại | Player movement + health/stamina + save cơ bản |
| Phase 2 | Chờ | Trap + item pickup + status effect |
| Phase 3 | Chờ | Boss encounter + attack pattern |
| Phase 4 | Chờ | UI, balancing, content mở rộng |

## Liên kết tài liệu
- Thiết kế đầy đủ: `README.md` ở root project