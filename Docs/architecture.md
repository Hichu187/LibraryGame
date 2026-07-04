# Kiến trúc Scripts

## Layer Data — ScriptableObject
| Class | Fields |
|-------|--------|
| `Book` | title, author, series, volumeNumber, description, cover, content |
| `BookSeries` | seriesName, description, cover |

## Layer World — Scene Objects
- **`BookShelf`** — chứa `List<BookSlot>`, mỗi slot giữ một `Book` SO + `Transform` anchor. Properties: `IsComplete`, `EmptySlotCount`. Query helpers: `GetSlotByVolume`, `GetSlotByBook`, `GetClosestEmptySlot`.
- **`BookSlotAnchor`** — component trên collider vật lý của slot, bridge tới `BookShelf.BookSlot` qua `_slotIndex`.
- **`BookObject : InteractableObject`** — đại diện vật lý của sách. State: `IsHeld`, `IsOnShelf`. Methods: `Pickup()`, `Drop()`, `Place(shelf, slot)`.

## Layer Interaction
- **`IInteractable`** — interface: `InteractionHint`, `OnHoverEnter/Exit`, `OnInteract`
- **`InteractableObject`** — abstract base, quản lý `Outline` component (QuickOutline)

## Layer Player — Controller chain
| Script | Trách nhiệm |
|--------|-------------|
| `PlayerController` | Di chuyển WASD + sprint + nhảy qua `CharacterController` |
| `CameraController` | FPS look (pitch/yaw). `static IsUIActive` để UI block input |
| `InteractionController` | Raycast từ camera, detect `IInteractable`, fire hover/interact events |
| `BookPickupController` | Stack tối đa 5 quyển, scroll wheel xoay vòng |
| `BookShelfPlacementController` | Raycast từ camera tới `BookSlotAnchor`, ghost preview, đặt sách |

## GameManager
Hiện rỗng — chưa subscribe `BookObject.OnInteracted` hay bất kỳ event nào.

## Input mặc định
| Action | Key |
|--------|-----|
| Move | WASD |
| Sprint | Left Shift |
| Jump | Space |
| Look | Mouse Delta |
| Interact | E |
| Pickup | F |
| Drop | G |
| Scroll stack | Mouse Scroll Y |
| Place book | R |
