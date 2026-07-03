namespace LibraryGame
{
    public interface IInteractable
    {
        string InteractionHint { get; }

        void OnHoverEnter();
        void OnHoverExit();
        void OnInteract();
    }
}
