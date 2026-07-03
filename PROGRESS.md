# LibraryGame — Tiến độ dự án

> Cập nhật: 2026-07-01 (lần 3)

---

## Tổng quan

Unity first-person game, dùng **URP** + **New Input System** + **XR support** (Oculus, Mock HMD).  
Base class: `MonoCached` (package `BaXoai`) | Namespace: `LibraryGame`

---

## Trạng thái các hệ thống

### ✅ Đã hoàn thiện (code)

| Hệ thống | File | Mô tả |
|----------|------|-------|
| **Camera (FPS)** | `Assets/Scripts/Player/CameraController.cs` | First-person look, pitch clamp (−80°/+80°), cursor lock/unlock (Escape) |
| **Player Movement** | `Assets/Scripts/Player/PlayerController.cs` | WASD di chuyển, Shift sprint, Space nhảy, fall multiplier, ground check sphere |
| **Interaction (interface)** | `Assets/Scripts/Interaction/IInteractable.cs` | Interface: `InteractionHint`, `OnHoverEnter/Exit`, `OnInteract` |
| **Interaction (base)** | `Assets/Scripts/Interaction/InteractableObject.cs` | Abstract base — quản lý Outline tự động, `[RequireComponent]` |
| **Interaction (controller)** | `Assets/Scripts/Player/InteractionController.cs` | Raycast từ camera, detect target, events `OnHoverEnter/Exit`, nhấn E |
| **Book (data)** | `Assets/Scripts/Data/Book.cs` | ScriptableObject: title, author, series, volumeNumber, description, cover, content |
| **BookSeries (data)** | `Assets/Scripts/Data/BookSeries.cs` | ScriptableObject: seriesName, description, cover |
| **BookShelf** | `Assets/Scripts/World/BookShelf.cs` | MonoBehaviour: quản lý 10–15 BookSlot, validate series, tra cứu theo volume |
| **BookObject** | `Assets/Scripts/World/BookObject.cs` | `InteractableObject`: giữ Book SO + Shelf ref, bắn static event `OnInteracted` |
| **Quality Settings** | `Assets/Settings/` | Profile PC/Mobile Low+High, URP Renderer tương ứng |
| **Volume Profiles** | `Assets/Settings/VolumeProfiles/` | 8 profile: Cinematic, Default, High/Low, Market, ZenGarden, Outline, MediaOverrides |
| **XR Config** | `Assets/Settings/XR/` | Oculus Loader + Settings, Mock HMD Loader + Settings |
| **Terrain** | `Assets/New Terrain.asset` | Terrain cơ bản (557 KB) |

---

### 🔄 Đang làm / Cần setup trong scene

| Hệ thống | Việc cần làm |
|----------|-------------|
| **Game Manager** | `GameManager.cs` stub rỗng — cần implement game state (Playing / Paused / Reading), subscribe `BookObject.OnInteracted` |
| **Scene setup** | Gắn `InteractionController` lên Camera, tạo BookShelf + BookObject đầu tiên trong `Base.unity` |

---

### ❌ Chưa làm

| Hệ thống | Ghi chú |
|----------|--------|
| **UI đọc sách** | Panel hiện nội dung Book; subscribe `BookObject.OnInteracted` và `InteractionController.OnHoverEnter/Exit` để hiện hint |
| **Prefabs** | BookObject prefab, BookShelf prefab |
| **Audio Manager** | Nhạc nền, SFX bước chân, lật sách |
| **Save / Load** | Lưu tiến trình đọc sách (quyển nào đã đọc) |
| **NPC / AI** | Thủ thư hoặc nhân vật khác (nếu có) |
| **Scene Management** | Load scene, transition |
| **Mobile Input** | Touchscreen binding cho PlayerController + CameraController |

---

## Cấu trúc thư mục hiện tại

```
Assets/
├─ Scenes/
│  └─ Base.unity                     ← scene duy nhất
├─ Scripts/
│  ├─ Data/
│  │  ├─ Book.cs                     ✅
│  │  └─ BookSeries.cs               ✅
│  ├─ Interaction/
│  │  ├─ IInteractable.cs            ✅
│  │  └─ InteractableObject.cs       ✅
│  ├─ Player/
│  │  ├─ CameraController.cs         ✅
│  │  ├─ InteractionController.cs    ✅
│  │  └─ PlayerController.cs         ✅
│  ├─ System/
│  │  └─ GameManager.cs              🔄 stub
│  └─ World/
│     ├─ BookObject.cs               ✅
│     └─ BookShelf.cs                ✅
├─ QuickOutline/                     ✅ (thư viện bên ngoài)
├─ Settings/
│  ├─ Mobile/                        ✅ (Low/High)
│  ├─ PC/                            ✅ (Low/High)
│  ├─ VolumeProfiles/                ✅ (8 profile)
│  └─ XR/                            ✅ (Oculus + Mock HMD)
└─ New Terrain.asset                 ✅
```

---

## Luồng dữ liệu hiện tại

```
BookSeries (SO)
    └── Book (SO) ──────────────────────── voluemNumber, title, content...
         └── BookObject (MonoBehaviour)   gắn lên mesh sách trong scene
              ├── InteractableObject      quản lý Outline highlight
              └── BookShelf               giá sách chứa 10–15 BookSlot
                   └── BookSlot           anchor Transform + Book SO
```

```
InteractionController (Camera)
    ├── Raycast → IInteractable
    ├── OnHoverEnter/Exit → [UI hint]       (chưa làm)
    └── Nhấn E → OnInteract()
             └── BookObject.OnInteracted   (static event)
                      └── GameManager      (chưa làm)
                               └── [UI đọc sách]  (chưa làm)
```

---

## Bước tiếp theo đề xuất

1. **UI đọc sách** — panel hiện `Book.Content` khi `BookObject.OnInteracted` bắn
2. **GameManager** — quản lý state, subscribe event, lock/unlock cursor khi đọc
3. **Prefab BookObject + BookShelf** — setup scene `Base.unity`
4. **Save/Load** — lưu danh sách quyển đã đọc (PlayerPrefs hoặc JSON)

---

*File này được tạo tự động bởi Claude Code dựa trên duyệt toàn bộ project.*
