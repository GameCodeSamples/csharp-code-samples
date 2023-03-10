using Cysharp.Threading.Tasks;
using Wonderland;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Runtime.Interactables
{
    public class PlayAnimation : Action
    {
        [SerializeField] private AssetReferenceT<PlayerAnimation> playerAnimation;
        private NetworkPlayerAnimationController networkPlayerAnimationController;

        //NOTE: If an action is blocking, it is responsible for sending the OnExecutionFinishedEvent in order to keep calling the rest of the actions
        private string stateToComplete = "";

        public override void Execute(GameObject instigator)
        {
            instigator.gameObject.TryGetComponent(out networkPlayerAnimationController);
            LoadAndPlayAnimation();
        }

        private async void LoadAndPlayAnimation()
        {
            networkPlayerAnimationController.TryPlayAnimation(playerAnimation, Priority.Interactable, false);
            networkPlayerAnimationController.OnAnimationCompletedEvent += OnAnimationPlayed;
        }

        public override void CancelExecution(GameObject instigator)
        {
            // networkPlayerAnimationController.RemoveInteractablePriority();
        }

        void OnAnimationPlayed(string clipName)
        {
            OnExecutionFinished?.Invoke();
            networkPlayerAnimationController.OnAnimationCompletedEvent -= OnAnimationPlayed;
        }
    }
}