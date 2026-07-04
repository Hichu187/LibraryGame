# Game Design Document — Library Game

## Concept
Game dọn dẹp thư viện góc nhìn thứ nhất (FPS). Sách bị để lộn xộn khắp nơi trong thư viện — người chơi phải nhặt và xếp lại đúng giá, đúng bộ.

## Core Loop
```
Đi quanh thư viện → Phát hiện sách → Nhặt (stack trên tay) → Tìm giá → Đặt vào slot
```

## Quy tắc gameplay

### Sách & Giá sách
- Mỗi `BookShelf` gắn với một `BookSeries` cố định
- Mỗi slot trên kệ chứa đúng một quyển
- **Sách có thể đặt vào bất kỳ slot trống nào** — không bị chặn
- **"Đúng"** = sách thuộc đúng `BookSeries` của kệ đó
- Kệ **hoàn chỉnh đúng** khi tất cả slot có sách VÀ tất cả sách đúng bộ

### Feedback khi đặt sách
| Tình huống | Phản hồi |
|-----------|---------|
| Đặt sách đúng bộ vào kệ | Hiệu ứng "correct placement" (particle / âm thanh) |
| Đặt sách sai bộ vào kệ | Không có hiệu ứng đặc biệt (cho phép nhưng không khuyến khích) |
| Kệ full đúng hoàn toàn | Hiệu ứng "shelf complete" + cập nhật tiến trình |

### Tiến trình (Progress)
- HUD hiển thị: số kệ hoàn chỉnh đúng / tổng số kệ (ví dụ: `3 / 12`)
- Chỉ đếm kệ **full VÀ đúng bộ** — kệ full nhưng có sách sai không được tính
- Win condition: tất cả kệ hoàn chỉnh đúng

### Stack trên tay
- Mặc định: tối đa 5 quyển
- Scroll wheel xoay vòng để chọn quyển trên cùng (quyển sẽ được đặt)
- Thả tự do (G): quyển rơi vật lý xuống đất

---

## Skill System (Kế hoạch)

### Passive Skills
| Skill | Mô tả |
|-------|-------|
| Stack Capacity + | Tăng số sách tối đa trên tay (mặc định 5) |
| Move Speed + | Tăng tốc độ di chuyển |
| Interaction Range + | Tăng tầm raycast pickup/place |

### Active Skills
| Skill | Key (gợi ý) | Mô tả |
|-------|-------------|-------|
| Series Highlight | Q | Highlight tất cả sách trong scene cùng bộ với quyển trên cùng tay |
| Auto Collect | T | Sách cùng bộ với quyển trên cùng tự động bay về stack |
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
- [x] `BookShelf.IsComplete` — kệ full (bất kể đúng/sai bộ)

### Chưa implement
- [ ] `BookShelf.IsCorrectlyComplete` — kệ full VÀ đúng bộ hoàn toàn
- [ ] Event: correct placement (đặt đúng bộ)
- [ ] Event: shelf correctly complete
- [ ] Hiệu ứng correct placement (particle / âm thanh)
- [ ] Hiệu ứng shelf complete
- [ ] GameManager: track tiến trình, đếm kệ hoàn chỉnh đúng
- [ ] HUD: hiển thị tiến trình `n / total`
- [ ] Win condition: tất cả kệ hoàn chỉnh đúng
- [ ] UI: Interaction hint HUD
- [ ] UI: Book info panel (đọc sách khi nhấn E)
- [ ] Cursor lock — gắn với trạng thái UI
- [ ] Skill system (passive + active)
- [ ] Level design: sách rải rác trong scene

---

## Thứ tự ưu tiên (gợi ý)

```
1. BookShelf.IsCorrectlyComplete + events   ← logic cốt lõi
2. Effects (correct place + shelf complete) ← feedback trực tiếp
3. GameManager + progress HUD               ← vòng lặp hoàn chỉnh
4. UI Layer (hint + book panel)             ← polish
5. Skill system                             ← progression
6. Level design                             ← content
```
