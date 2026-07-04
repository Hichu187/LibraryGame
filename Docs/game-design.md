# Game Design Document — Library Game

## Concept
Game dọn dẹp thư viện góc nhìn thứ nhất (FPS). Sách bị để lộn xộn khắp nơi trong thư viện — người chơi phải nhặt và xếp lại đúng giá, đúng bộ.

## Core Loop
```
Đi quanh thư viện → Phát hiện sách → Nhặt (stack trên tay) → Tìm đúng giá → Đặt vào slot
```

## Quy tắc gameplay

### Sách & Giá sách
- Mỗi `BookShelf` gắn với một `BookSeries` cố định
- Mỗi slot trên kệ chứa đúng một quyển
- Chỉ được đặt sách đúng bộ vào kệ (validate series khi place)
- Kệ hoàn chỉnh khi tất cả slot có sách (`BookShelf.IsComplete`)

### Stack trên tay
- Mặc định: tối đa 5 quyển
- Scroll wheel xoay vòng để chọn quyển trên cùng (quyển sẽ được đặt)
- Thả tự do (G): quyển rơi vật lý xuống đất

### Win / Level end
- Khi toàn bộ kệ trong scene hoàn chỉnh → trigger kết thúc level

---

## Skill System (Kế hoạch)

### Passive Skills
| Skill | Mô tả | Ghi chú |
|-------|-------|---------|
| Stack Capacity + | Tăng số sách tối đa trên tay | Mặc định 5, nâng lên 7/10/... |
| Move Speed + | Tăng tốc độ di chuyển | |
| Interaction Range + | Tăng tầm raycast pickup/place | |

### Active Skills
| Skill | Key (gợi ý) | Mô tả |
|-------|-------------|-------|
| Series Highlight | Q | Highlight tất cả sách cùng bộ với quyển trên cùng tay |
| Auto Collect | T | Sách cùng bộ tự động bay về stack trên tay |
| *(phát triển thêm)* | | |

### Cơ chế nâng cấp
*(Chưa xác định — XP, currency, hay milestone-based)*

---

## Trạng thái implement

### Đã có
- [x] Player movement (WASD + sprint + nhảy)
- [x] FPS camera với UI input block (`CameraController.IsUIActive`)
- [x] Interaction system — raycast từ camera, hover outline (QuickOutline)
- [x] Book pickup / stack / drop / scroll cycle
- [x] Bookshelf placement với ghost preview
- [x] Data layer: `Book` và `BookSeries` ScriptableObject

### Chưa implement
- [ ] Validate series khi đặt sách (chặn đặt sai bộ)
- [ ] UI: Interaction hint HUD
- [ ] UI: Book info panel (đọc sách khi nhấn E)
- [ ] Cursor lock — gắn với trạng thái UI
- [ ] GameManager: xử lý events, kiểm tra win condition
- [ ] Win condition: khi toàn bộ kệ hoàn chỉnh
- [ ] Skill system (passive + active)
- [ ] Level design: sách rải rác trong scene

---

## Thứ tự ưu tiên (gợi ý)

```
1. Series validation khi place     ← gameplay correctness
2. UI Layer (hint HUD + book panel) ← feedback cho player
3. GameManager + Win condition      ← loop hoàn chỉnh
4. Skill system                     ← progression
5. Level design                     ← content
```
