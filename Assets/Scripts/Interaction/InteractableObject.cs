using UnityEngine;

namespace LibraryGame
{
    [RequireComponent(typeof(Outline))]
    public abstract class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] Color _outlineColor = Color.yellow;
        [SerializeField] float _outlineWidth = 5f;
        [SerializeField] Outline.Mode _outlineMode = Outline.Mode.OutlineAll;

        Outline _outline;

        public abstract string InteractionHint { get; }

        protected virtual void Awake()
        {
            _outline = GetComponent<Outline>();
            _outline.OutlineColor = _outlineColor;
            _outline.OutlineWidth = _outlineWidth;
            _outline.OutlineMode = _outlineMode;
            _outline.enabled = false;
        }

        public void OnHoverEnter() => _outline.enabled = true;
        public void OnHoverExit()  => _outline.enabled = false;

        public abstract void OnInteract();
    }
}
