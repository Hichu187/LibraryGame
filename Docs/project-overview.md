# Library Game — Tổng quan dự án

Game thư viện góc nhìn thứ nhất (FPS) xây dựng bằng Unity. Người chơi nhặt sách, xếp lại lên đúng kệ theo bộ (series).

**Core loop:** di chuyển → nhặt sách → đặt đúng kệ

## Stack kỹ thuật
- Unity (C#)
- Unity Input System (new)
- ScriptableObject cho data layer
- QuickOutline cho hiệu ứng hover

## Quy ước code
- **Namespace:** `LibraryGame`
- **Base class:** `MonoCached` (thư viện `BaXoai`) — cung cấp `transformCached` thay cho `transform`

## Định hướng
Prototype / học tập — chưa có build target cụ thể. Ưu tiên prototype nhanh, tránh over-engineer.
