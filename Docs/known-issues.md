# Known Issues & Backlog

## Đã fix
- **Raycast từ player root** → chuyển sang bắn từ `Camera` trong cả `InteractionController` và `BookShelfPlacementController`. Camera inject qua SerializeField, fallback `Camera.main`.
- **CameraController dead code** → xóa cursor lock no-op, xóa `_cursorToggleAction`. Thêm `static bool IsUIActive` để UI block camera input.

## Chưa fix — cần theo dõi

| Vấn đề | Mô tả | Ưu tiên |
|--------|-------|---------|
| Cursor không lock | Tạm bỏ qua. Sẽ thêm lại khi implement UI hoàn chỉnh | Trung bình |
| Không validate series khi đặt sách | Player có thể đặt sách vào kệ sai bộ. `HandlePlace` cần kiểm tra `book.BookData.Series == _targetShelf.Series` | Trung bình |
| `BookSlotAnchor` misconfiguration silent fail | `_shelf` null hoặc `_slotIndex` out of range → `IsEmpty` trả `true` sai, ghost hiện nhưng đặt không được. Cần `OnValidate` warning | Trung bình |
| `BookShelf.OnValidate` warning quá sớm | Kệ mới tạo (0 slot) bị warn ngay. Nên bỏ qua khi `_slots.Count == 0` | Thấp |
| `GameManager` rỗng | Chưa xử lý `BookObject.OnInteracted` (mở UI đọc sách) hay win condition (`BookShelf.IsComplete`) | Cao |
