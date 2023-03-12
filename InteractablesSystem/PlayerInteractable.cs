namespace Wonderland
{
    public class PlayerInteractable : Interactable
    {
        public override string GetName()
        {
            return GetController().data.userName;
        }

        public PlayerController GetController() => GetComponentInParent<PlayerController>();
    }
}