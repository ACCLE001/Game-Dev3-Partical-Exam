namespace ThanethHamburut
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        void Interact(PlayerInteractionDetector interactor);
    }
}
