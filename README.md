# Thiết kế game 2D endless runner ngang


## 1. Định hướng tổng thể

Với game endless runner góc nhìn ngang, nên chia dự án thành 5 lớp logic:

- Input
- Player State
- World Interaction
- Encounter
- Persistence

Cách tiếp cận này giúp tách điều khiển nhân vật khỏi logic trap, boss và item.
Điểm quan trọng là game có yếu tố tiến-lùi thay vì chỉ chạy một chiều. Vì vậy đây nên được xem là một side-scrolling runner có nhịp tiến trình, không phải runner truyền thống một hướng. Điều này ảnh hưởng trực tiếp đến camera, spawn, lưu vị trí và điều kiện xuất hiện boss.

## 2. Kiến trúc đề xuất

| Module | Nhiệm vụ | Đối tượng chính | Ghi chú thiết kế |
|---|---|---|---|
| Player | Di chuyển, nhảy, chịu sát thương, stamina, trạng thái dịch bệnh | PlayerController, Health, Stamina, StatusEffect | Nên tách motor vật lý với dữ liệu sinh lực |
| Item | Hồi thể lực, hồi máu, xóa hiệu ứng bệnh | ItemPickup, ItemEffect, ItemDatabase | Dùng data-driven để dễ thêm item mới |
| Trap | Bẫy tĩnh gây sát thương | TrapBase, HazardVolume | Trap không tự di chuyển, chỉ kích hoạt khi va chạm/trigger |
| Boss | Xuất hiện theo điều kiện, đánh tự động, biến mất sau thời gian | BossEncounter, BossAI, BossAttackPattern | Tách điều kiện spawn khỏi hành vi chiến đấu |
| Save | Tự lưu theo chu kỳ và lưu trạng thái hiện tại | SaveService, SaveData, SaveSlot | Không lưu trực tiếp object runtime; chỉ lưu snapshot |

## 3. Thiết kế luồng gameplay

### Luồng cơ bản

1. Người chơi điều khiển di chuyển tiến/lui bằng A/D và nhảy bằng W hoặc Space.
2. Khi chạy và/hoặc giữ trạng thái hoạt động mạnh, stamina giảm theo thời gian.
3. Khi stamina cạn, áp dụng hậu quả lên máu hoặc trạng thái suy kiệt theo thời gian
4. Bẫy tĩnh gây sát thương ngay khi chạm vùng kích hoạt.
5. Item được nhặt để phục hồi hoặc xóa trạng thái xấu.
6. Boss chỉ xuất hiện khi thỏa điều kiện đặc biệt và tấn công tự động theo pattern ngẫu nhiên có kiểm soát.
7. Game lưu định kỳ và khi có mốc an toàn như nhặt item quan trọng hoặc vượt checkpoint.

### Luồng state player

Nên dùng một state machine nhẹ với các trạng thái:

- Idle
- Move
- Jump
- Exhausted
- Infected
- Dead

Các trạng thái này giúp kiểm soát animation, tốc độ di chuyển và giới hạn hành vi.

### Luồng boss encounter

Không nên để boss tự kiểm tra mọi thứ trong scene. Hãy tạo một `BossEncounterTrigger` riêng, chịu trách nhiệm:

- Phát hiện điều kiện kích hoạt
- Khóa hoặc điều chỉnh camera nếu cần
- Spawn boss
- Bắt đầu đồng hồ đếm thời gian hiện diện

Khi hết thời gian, encounter tự kết thúc, boss biến mất và hệ thống trở về trạng thái thường.

## 4. Thiết kế từng hệ thống

### Player

Dùng 2 thành phần chính:

- `PlayerMotor` cho physics
- `PlayerVitals` cho máu, thể lực và trạng thái bệnh

Máu giảm khi nhận sát thương hoặc bị hiệu ứng dịch bệnh. Stamina giảm theo thời gian khi chạy, và hồi phục khi nghỉ hoặc dùng item. Hồi phục đứng yên chỉ là cơ chế phụ, tốc độ rất chậm.

Khuyến nghị: để stamina là nguồn nhịp độ, còn health là nguồn sinh tồn.

### Item

Mỗi item nên có data gồm loại effect, giá trị hồi, thời lượng và điều kiện kích hoạt. 3 item cần sớm là:

- Hồi stamina
- Hồi máu
- Xóa bệnh

Các item này nên spawn ngẫu nhiên dọc đường chạy để tạo động lực khám phá.

Nếu game có nhiều item sau này, hãy dùng ScriptableObject để cấu hình. Runtime chỉ cần đọc dữ liệu và áp effect.

### Trap

Trap là object tĩnh, có collider hoặc trigger, không dùng AI di chuyển. Nó chỉ phát sát thương khi player đi vào vùng kích hoạt hoặc chạm vật lý.

Chướng ngại vật cũng nên được xem là một nhánh của trap level design vì người chơi phải nhảy qua.

Nên chuẩn hóa base class của trap để sau này thêm spike, poison cloud, flame floor mà không phải viết lại luồng damage.

### Boss

Boss không nên gắn logic vào chính prefab chiến đấu. Nên tách thành:

- Điều kiện xuất hiện
- Controller
- Pattern tấn công
- Vòng đời `spawn -> active -> despawn`

Các đòn đánh không cố định nên là random có trọng số, để boss vẫn ngẫu nhiên nhưng không thành hỗn loạn.

## 5. Phương án thiết kế stamina và máu

| Cơ chế | Phương án A | 
|---|---|
| Stamina giảm | Giảm theo thời gian khi chạy |
| Stamina cạn | Trừ máu trực tiếp | 
| Bệnh dịch | Debuff gây DOT lên máu 
| Hồi phục | Hồi theo thời gian, bằng item/checkpoint | 

## 6. Lưu trữ và auto-save

Save system nên lưu snapshot theo các nhóm dữ liệu:

- Vị trí player
- HP
- Stamina
- Hiệu ứng đang dính
- Inventory/item đã nhặt
- Trạng thái boss encounter
- Các trap/item đã bị tiêu thụ hoặc vô hiệu hóa

Auto-save nên kích hoạt khi nhặt item hiếm và khi vượt màn. Nếu cần thêm một lớp an toàn, có thể lưu định kỳ theo chu kỳ thời gian trong cài đặt, nhưng hai mốc ưu tiên vẫn là item hiếm và clear stage.

Tránh lưu trực tiếp reference của object Unity. Thay vào đó, map dữ liệu sang DTO thuần C# để serialize dễ và ổn định hơn.

## 7. Dữ liệu cần lưu

| Nhóm dữ liệu | Trường cần lưu | Lý do |
|---|---|---|
| Player | position, health, stamina, activeEffects, facing, velocity optional | Khôi phục đúng trạng thái chơi |
| Progress | currentStage, checkpointId, runTime, clearedStageCount | Đồng bộ tiến trình endless run |
| Inventory/Items | pickedItemIds, consumedItemIds, rareItemPickups | Tránh spawn lại vật phẩm đã lấy và phục vụ auto-save |
| World | disabledTraps, openedPaths, spawnedBossState, generatedStageSeed | Khôi phục môi trường đã thay đổi |

## 8. Phương án triển khai thực tế

| Giai đoạn | Mục tiêu | Ưu tiên |
|---|---|---|
| Phase 1 | Player movement + health/stamina + save cơ bản | Rất cao |
| Phase 2 | Trap + item pickup + status effect | Rất cao |
| Phase 3 | Boss encounter + attack pattern | Cao |
| Phase 4 | Tối ưu UI, checkpoint, balancing, content mở rộng | Trung bình |

## Gợi ý kỹ thuật thêm

Nếu làm trên Unity, nên ưu tiên:

- ScriptableObject cho dữ liệu
- Event-driven cho giao tiếp giữa hệ thống
- State machine cho player và boss
- JSON hoặc binary thuần C# cho save, nhưng giữ schema ổn định để tránh lỗi phiên bản

Về tổ chức thư mục, có thể chia thành:

- `Scripts/Core`
- `Scripts/Player`
- `Scripts/Items`
- `Scripts/Traps`
- `Scripts/Boss`
- `Scripts/Save`
- `Scripts/UI`
- `ScriptableObjects/Data`

