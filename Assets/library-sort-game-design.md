# Library Sort — Game Design Document

> Game sắp xếp sách thư viện: nhặt sách lộn xộn trong phòng, dùng stack mang trên tay,
> đặt đúng giá - đúng bộ - đúng thứ tự (1→10).

---

## 1. Tổng quan

**Thể loại:** Puzzle / Organization, góc nhìn FPS hoặc Third-person
**Engine giả định:** Unity (C#) — có thể chuyển sang Godot/Unreal, logic core không đổi

### Core Loop

```
Nhặt sách từ sàn ──► Push vào Stack (đỉnh)
                          │
                          ▼
        Nhìn (raycast) vào 1 slot trống trên giá
                          │
                          ▼
        Hiện Ghost Preview (mờ) = sách đang ở đỉnh Stack
                          │
                          ▼
                  Bấm nút "Place"
                    │           │
               Đúng vị trí    Sai vị trí
                    │           │
              Pop khỏi stack   Reject (không pop)
              Gắn vào slot     Feedback lỗi (shake/sound)
              Chơi VFX
                    │
                    ▼
        Tất cả slot trên tất cả giá đầy & đúng ──► WIN
```

### Quyết định thiết kế đã chốt

| Vấn đề | Quyết định | Lý do |
|---|---|---|
| Kích thước sách | Đồng nhất (1 size duy nhất) | Bỏ physics overlap, đơn giản hoá pickup/place |
| Cơ chế mang sách | Stack LIFO trên tay, capacity mặc định = 5 | Tạo chiều sâu chiến thuật (thứ tự nhặt quan trọng) |
| Tương tác | Raycast từ camera (aim), không drag-drop | Phù hợp FPS-style, dễ làm UX rõ ràng |
| Slot trên giá | Cố định theo index (slot #1 → vị trí cố định, … slot #10) | Dễ hiểu, dễ làm ghost preview & hint UI |
| Stack bị "kẹt" thứ tự | Cho phép thả lại (drop-back) quyển đỉnh xuống sàn | Tránh deadlock / frustration |
| Hiển thị stack | UI list dọc, hiện rõ thứ tự đỉnh→đáy, màu bộ + số | Cho phép người chơi lập kế hoạch trước khi nhặt |
| Ghost preview khi nhìn slot không hợp lệ | Hiện màu đỏ + lý do ngắn (sai bộ / sai thứ tự) | Feedback tức thì, tránh đoán mò |
| Capacity stack | Upgrade được (5 → cao hơn) | Progression / reward loop |

---

## 2. Data Structures

```csharp
// ============ BOOK ============
[System.Serializable]
public class BookData
{
    public string setID;       // VD: "SetA", "SetB", "Mystery01"
    public int orderIndex;     // 1-10
    public Material spineMaterial; // màu/texture đại diện cho bộ
}

// ============ PLAYER STACK (LIFO) ============
public class PlayerStack
{
    private List<BookData> books = new List<BookData>();
    public int capacity = 5; // upgrade được

    public BookData PeekTop() => books.Count > 0 ? books[^1] : null;

    public bool TryPush(BookData book)
    {
        if (books.Count >= capacity) return false; // đầy
        books.Add(book);
        return true;
    }

    public BookData PopTop()
    {
        if (books.Count == 0) return null;
        var top = books[^1];
        books.RemoveAt(books.Count - 1);
        return top;
    }

    public bool IsFull()  => books.Count >= capacity;
    public bool IsEmpty() => books.Count == 0;
    public IReadOnlyList<BookData> GetAllForUI() => books; // dùng cho UI hiển thị toàn stack
}

// ============ SHELF & SLOT ============
public class ShelfSlot
{
    public int expectedIndex;        // 1-10, vị trí cố định trên giá
    public BookData placedBook = null;
    public bool IsEmpty() => placedBook == null;
}

public class BookShelf
{
    public string assignedSetID = null; // null = chưa "claim" bộ nào
    public ShelfSlot[] slots = new ShelfSlot[10];

    public bool IsComplete()
    {
        foreach (var s in slots)
            if (s.IsEmpty()) return false;
        return true;
    }
}
```

---

## 3. Logic chính: Validate & Place

```csharp
public enum PlaceResult
{
    Success,
    SlotOccupied,
    WrongSet,
    WrongOrder
}

public PlaceResult TryPlaceBook(BookShelf shelf, ShelfSlot slot, BookData book)
{
    if (!slot.IsEmpty())
        return PlaceResult.SlotOccupied;

    // Giá chưa có bộ nào -> claim theo bộ của sách này
    if (shelf.assignedSetID == null)
        shelf.assignedSetID = book.setID;
    else if (shelf.assignedSetID != book.setID)
        return PlaceResult.WrongSet;

    if (slot.expectedIndex != book.orderIndex)
        return PlaceResult.WrongOrder;

    slot.placedBook = book;
    return PlaceResult.Success;
}
```

**Flow gọi khi người chơi bấm "Place":**

```csharp
void OnPlaceButtonPressed()
{
    var targetSlot = raycastTargeting.GetCurrentSlot(); // null nếu không nhìn vào slot nào
    var targetShelf = raycastTargeting.GetCurrentShelf();
    var topBook = playerStack.PeekTop();

    if (targetSlot == null || topBook == null) return;

    var result = TryPlaceBook(targetShelf, targetSlot, topBook);

    switch (result)
    {
        case PlaceResult.Success:
            playerStack.PopTop();
            PlayPlaceVFX(targetSlot.transform.position);
            PlaySound("place_correct");
            CheckWinCondition();
            break;

        case PlaceResult.WrongSet:
            PlaySound("reject_wrong_set");
            ShowFeedbackText("Sai bộ sách!");
            break;

        case PlaceResult.WrongOrder:
            PlaySound("reject_wrong_order");
            ShowFeedbackText("Sai thứ tự!");
            break;

        case PlaceResult.SlotOccupied:
            // không nên xảy ra nếu ghost preview chỉ hiện ở slot trống
            break;
    }
}
```

---

## 4. Raycast Targeting (aim vào slot)

```csharp
public class RaycastTargeting : MonoBehaviour
{
    public Camera playerCamera;
    public float interactRange = 3f;
    public LayerMask slotLayerMask;

    private ShelfSlot currentSlot;
    private BookShelf currentShelf;

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, slotLayerMask))
        {
            var slotComponent = hit.collider.GetComponent<ShelfSlotView>();
            if (slotComponent != null && slotComponent.Data.IsEmpty())
            {
                currentSlot = slotComponent.Data;
                currentShelf = slotComponent.ParentShelf;
                UpdateGhostPreview(slotComponent);
                return;
            }
        }

        // Không nhìn vào slot hợp lệ -> tắt ghost
        currentSlot = null;
        currentShelf = null;
        HideGhostPreview();
    }

    void UpdateGhostPreview(ShelfSlotView slotView)
    {
        var topBook = PlayerStack.Instance.PeekTop();
        if (topBook == null) { HideGhostPreview(); return; }

        bool valid = IsValidPreview(slotView, topBook);
        GhostPreview.Instance.Show(
            position: slotView.transform.position,
            material: topBook.spineMaterial,
            state: valid ? GhostState.Valid : GhostState.Invalid
        );
    }

    bool IsValidPreview(ShelfSlotView slotView, BookData book)
    {
        var shelf = slotView.ParentShelf;
        if (shelf.assignedSetID != null && shelf.assignedSetID != book.setID) return false;
        if (slotView.Data.expectedIndex != book.orderIndex) return false;
        return true;
    }

    public ShelfSlot GetCurrentSlot() => currentSlot;
    public BookShelf GetCurrentShelf() => currentShelf;
}
```

---

## 5. Pickup & Drop-back (chống deadlock)

```csharp
public class BookPickup : MonoBehaviour
{
    public void TryPickupBook(BookFloorObject floorBook)
    {
        if (PlayerStack.Instance.IsFull())
        {
            ShowFeedbackText("Stack đã đầy!");
            return;
        }

        PlayerStack.Instance.TryPush(floorBook.Data);
        Destroy(floorBook.gameObject); // hoặc pool lại
        RefreshStackUI();
    }

    // Bắt buộc có: thả lại quyển trên cùng xuống sàn gần player
    // để gỡ kẹt khi nhặt sai thứ tự
    public void DropTopToFloor()
    {
        var book = PlayerStack.Instance.PopTop();
        if (book == null) return;

        SpawnBookOnFloor(book, transform.position + transform.forward * 0.5f);
        RefreshStackUI();
    }
}
```

---

## 6. UI hiển thị Stack

Yêu cầu: người chơi phải thấy **toàn bộ** stack hiện tại, không chỉ đỉnh — để lập kế hoạch
nhặt/đặt mà không bị "mù" về thứ tự bên dưới.

```
┌─────────────┐
│  STACK (3/5) │
├─────────────┤
│ ▣ SetA #4   │ ← đỉnh (sẽ được place / hiện ghost)
│ ▣ SetA #2   │
│ ▢ SetB #7   │ ← đáy
└─────────────┘
```

Gợi ý implement: 1 `VerticalLayoutGroup` trong Canvas, mỗi item là prefab nhỏ
(màu nền = setID, text = orderIndex), re-render mỗi khi stack thay đổi (push/pop).

```csharp
void RefreshStackUI()
{
    ClearStackUIItems();
    var books = PlayerStack.Instance.GetAllForUI();
    for (int i = books.Count - 1; i >= 0; i--) // đỉnh hiện trên cùng UI
    {
        var item = Instantiate(stackItemPrefab, stackUIContainer);
        item.GetComponent<StackItemView>().Setup(books[i], isTop: i == books.Count - 1);
    }
    capacityText.text = $"{books.Count}/{PlayerStack.Instance.capacity}";
}
```

---

## 7. Win Condition

```csharp
public class GameManager : MonoBehaviour
{
    public List<BookShelf> allShelves;

    public void CheckWinCondition()
    {
        foreach (var shelf in allShelves)
            if (!shelf.IsComplete()) return;

        OnLevelComplete();
    }

    void OnLevelComplete()
    {
        // VFX toàn màn hình, sound, chuyển level/UI thắng
    }
}
```

Check theo **event-driven** (chỉ gọi sau mỗi lần `Place` thành công), không cần check mỗi frame.

---

## 8. Random Layout (đảm bảo solvable)

Nguyên tắc bắt buộc khi generate level:

1. Số giá luôn = số bộ sách cần trong level (1 giá : 1 bộ, không thừa giá / thiếu giá)
2. Mỗi bộ luôn có đủ đúng 10 quyển (index 1-10), không thiếu không trùng
3. Vị trí sách trên sàn: random points không chồng nhau (vì size đồng nhất, dùng
   grid + jitter hoặc Poisson disk sampling đơn giản)
4. Độ khó tăng dần qua biến số:
   - Số bộ sách cùng lúc (2 → 3 → 4...)
   - Mức độ trộn lẫn vị trí các bộ trên sàn (trộn nhiều = phải đi lại nhiều hơn)
   - Khoảng cách trung bình từ vùng sách tới giá

```csharp
public LevelConfig GenerateLevel(int numSets, float mixFactor)
{
    var config = new LevelConfig();
    for (int s = 0; s < numSets; s++)
    {
        string setID = $"Set_{s}";
        for (int i = 1; i <= 10; i++)
            config.books.Add(new BookData { setID = setID, orderIndex = i });
    }
    ShuffleAndScatter(config.books, mixFactor); // random vị trí sàn
    return config;
}
```

---

## 9. MVP Checklist

- [ ] Player controller + camera (FPS hoặc third-person)
- [ ] Raycast targeting + crosshair UI
- [ ] `PlayerStack` (push/pop/peek) + capacity 5
- [ ] Pickup sách trên sàn → push stack
- [ ] Ghost preview tại slot khi nhìn (valid/invalid state)
- [ ] `TryPlaceBook` validate + VFX khi đúng
- [ ] Feedback rõ ràng khi đặt sai (sound + text)
- [ ] UI hiển thị toàn bộ stack (không chỉ đỉnh)
- [ ] Drop-back: thả quyển đỉnh xuống sàn
- [ ] Win condition check
- [ ] Random level generator (solvable-guaranteed)

### Sau MVP (nice-to-have)

- [ ] Upgrade capacity stack (currency/level reward)
- [ ] Nhiều level / độ khó tăng dần
- [ ] Animation đặt sách (bay từ tay → slot) thay vì snap tức thì
- [ ] Hint system (highlight giá còn thiếu, hoặc số sách còn lại theo bộ)

---

## 10. Câu hỏi mở cần quyết định khi build

| Câu hỏi | Ảnh hưởng |
|---|---|
| Có giới hạn thời gian / điểm số không, hay chỉ thuần puzzle (không time pressure)? | Ảnh hưởng tới cảm giác "casual" vs "competitive" |
| Camera: first-person hay third-person nhìn chéo xuống? | Ảnh hưởng độ khó nhìn rõ số trên gáy sách |
| Cho phép xem trước toàn bộ vị trí sách trên sàn (map/minimap) hay phải tự khám phá? | Ảnh hưởng độ khó tìm kiếm |
| VFX khi đặt đúng: chỉ tại slot, hay cộng thêm hiệu ứng khi hoàn thành cả giá? | UX polish, tạo cảm giác "thưởng" rõ ràng hơn |

---

## 11. Ghi chú quy ước

- Tài liệu này được trao đổi và cập nhật giữa người thiết kế và Claude (Anthropic) trong quá trình lên ý tưởng.

