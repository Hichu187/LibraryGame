using UnityEngine;
using UnityEditor;

namespace LibraryGame
{
    [CustomEditor(typeof(BookShelf))]
    public class BookShelfEditor : Editor
    {
        SerializedProperty _slotsProp;

        int     _genCount         = 10;
        float   _genSpacing       = 0.05f;
        Vector3 _genStart         = Vector3.zero;
        Vector3 _genDirection     = Vector3.right;
        Vector3 _slotLocalEuler   = new(0f, 0f, 90f);
        Vector3 _slotColliderSize = new(1f, 0.2f, 0.75f);

        void OnEnable()
        {
            _slotsProp = serializedObject.FindProperty("_slots");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Tạo Slots tự động", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            _genCount         = EditorGUILayout.IntSlider("Số slot", _genCount, 1, 30);
            _genSpacing       = EditorGUILayout.FloatField("Khoảng cách (m)", _genSpacing);
            _genStart         = EditorGUILayout.Vector3Field("Vị trí bắt đầu (local)", _genStart);
            _genDirection     = EditorGUILayout.Vector3Field("Hướng (local)", _genDirection);
            _slotLocalEuler   = EditorGUILayout.Vector3Field("Rotation sách (local Euler)", _slotLocalEuler);
            _slotColliderSize = EditorGUILayout.Vector3Field("Collider Size", _slotColliderSize);

            EditorGUILayout.Space(6);

            bool dirInvalid = _genDirection == Vector3.zero;
            using (new EditorGUI.DisabledScope(dirInvalid))
            {
                if (GUILayout.Button("Tạo Slots", GUILayout.Height(28)))
                    GenerateSlots();
            }

            if (dirInvalid)
                EditorGUILayout.HelpBox("Hướng không được là Vector3.zero.", MessageType.Warning);

            EditorGUILayout.Space(2);

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 0.45f, 0.45f);
            if (GUILayout.Button("Xóa tất cả Slots"))
            {
                if (EditorUtility.DisplayDialog("Xác nhận xóa",
                    "Xóa toàn bộ slots và các anchor GameObjects đã tạo tự động?",
                    "Xóa", "Hủy"))
                    ClearSlots();
            }
            GUI.backgroundColor = prevColor;
        }

        void GenerateSlots()
        {
            // Cảnh báo nếu slot cũ đang có sách
            bool hasBooksAssigned = false;
            for (int i = 0; i < _slotsProp.arraySize; i++)
            {
                var bookProp = _slotsProp.GetArrayElementAtIndex(i).FindPropertyRelative("_book");
                if (bookProp.objectReferenceValue != null) { hasBooksAssigned = true; break; }
            }

            if (hasBooksAssigned && !EditorUtility.DisplayDialog("Cảnh báo",
                "Một số slot đang được gán sách. Tiếp tục sẽ xóa toàn bộ và tạo lại.",
                "Tiếp tục", "Hủy"))
                return;

            Undo.SetCurrentGroupName("Generate BookShelf Slots");
            int undoGroup = Undo.GetCurrentGroup();

            var shelf = (BookShelf)target;
            ClearAutoAnchors(shelf);

            serializedObject.Update();
            _slotsProp.arraySize = _genCount;

            var dir = _genDirection.normalized;

            for (int i = 0; i < _genCount; i++)
            {
                var anchorGO = new GameObject($"Slot_{i:D2}");
                Undo.RegisterCreatedObjectUndo(anchorGO, "Create Slot Anchor");
                anchorGO.transform.SetParent(shelf.transform, false);
                anchorGO.transform.localPosition    = _genStart + dir * (_genSpacing * i);
                anchorGO.transform.localEulerAngles = _slotLocalEuler;

                // BookSlotAnchor — [RequireComponent] tự thêm BoxCollider
                var slotAnchor = Undo.AddComponent<BookSlotAnchor>(anchorGO);
                slotAnchor.Init(shelf, i);

                var col = anchorGO.GetComponent<BoxCollider>();
                col.size    = _slotColliderSize;
                col.center  = Vector3.zero;

                var slotProp = _slotsProp.GetArrayElementAtIndex(i);
                slotProp.FindPropertyRelative("_anchor").objectReferenceValue = anchorGO.transform;
                slotProp.FindPropertyRelative("_book").objectReferenceValue   = null;
            }

            serializedObject.ApplyModifiedProperties();
            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log($"[BookShelf] '{shelf.name}': tạo {_genCount} slots, spacing={_genSpacing}m.", shelf);
        }

        void ClearSlots()
        {
            Undo.SetCurrentGroupName("Clear BookShelf Slots");
            int undoGroup = Undo.GetCurrentGroup();
            ClearAutoAnchors((BookShelf)target);
            Undo.CollapseUndoOperations(undoGroup);
        }

        // Chỉ xóa các anchor được tạo tự động (tên bắt đầu bằng "Slot_")
        void ClearAutoAnchors(BookShelf shelf)
        {
            serializedObject.Update();

            for (int i = 0; i < _slotsProp.arraySize; i++)
            {
                var anchorProp = _slotsProp.GetArrayElementAtIndex(i).FindPropertyRelative("_anchor");
                if (anchorProp.objectReferenceValue is Transform t && t != null && t.name.StartsWith("Slot_"))
                    Undo.DestroyObjectImmediate(t.gameObject);
            }

            _slotsProp.arraySize = 0;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
