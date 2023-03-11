using Core.Runtime.Interactables;
using GameplayIngredients;
using UnityEngine;

namespace Wonderland
{
    public class TeleportToPoint : Action
    {
        [SerializeField] private EntryPoint exitPoint;

        public override async void Execute(GameObject instigator)
        {
            var playerMovement = instigator.GetComponent<PlayerController>().PlayerMovement;
            if (!playerMovement) return;

            if (!exitPoint.GetPointOnNavMesh(out var navMeshPoint))
            {
                Debug.LogError("The teleport destination point is not correctly set");
                return;
            }

            await Manager.Get<FadeScreenManager>().Show();
            TeleportPlayer(playerMovement, navMeshPoint);
            await Manager.Get<FadeScreenManager>().Hide();
            OnExecutionFinished?.Invoke();
        }

        private void TeleportPlayer(PlayerMovement playerMovement, Vector3 navMeshPoint)
        {
            playerMovement.TeleportTo(navMeshPoint, true);
            playerMovement.LookTorwards(exitPoint.GetForwardPoint());
        }

        public override void CancelExecution(GameObject instigator) { }

        private void OnValidate()
        {
            gameObject.name = "Teleport To " + exitPoint.name;
        }
    }
}