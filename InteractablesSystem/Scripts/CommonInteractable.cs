using System.Collections.Generic;
using System.Linq;
using Core.Runtime.Interactables;
using Doozy.Runtime.Common.Extensions;
using NaughtyAttributes;
using UnityEngine;
using Action = System.Action;

namespace Wonderland
{
    public class CommonInteractable : Interactable
    {
        [SerializeField] private InteractablesContainer interactablesContainer;
        [SerializeField] private string interactableName;
        [field: SerializeField] public bool IsOccupiable { get; private set; } = true;
        [field: SerializeField] public List<Interaction> Interactions { get; private set; }
        [field: SerializeField] public string ShopLink { get; private set; }

        [Space(10)]

        [BoxGroup("Visuals"), SerializeField] 
        public MeshRenderer meshRenderer;
        [BoxGroup("Visuals"), SerializeField]
        public GameObject vfxObject;

        [BoxGroup("Visuals"), SerializeField] private Collider col;

        public event Action OnInteractableSetState;

        public Interaction ActiveInteraction { get; set; }

        public bool IsOccuped => IsOccupiable && interactablesContainer.IsOccuped(this);

        public bool IsLocalOccuped => PlayerController.localPlayer && this == PlayerController.localPlayer.OccupedInteractable;

        public Interaction GetFirstInteraction() => Interactions[0];

        public void ClientUseOccupable(PlayerController controller, Interaction interaction)
        {
            var interactionIdx = Interactions.IndexOf(interaction);
            interactablesContainer.ClientUseOccupable(this, interactionIdx, controller);
        }

        public void ClientLeaveOccupable(PlayerController controller) => interactablesContainer.ClientLeaveOccupable(this, controller);

        public void ServerLeaveOccupable(PlayerController controller) => interactablesContainer.ServerLeaveOccupable(this, controller);

        public void UpdateInteractablePoint()
        {
            if (!IsOccupiable) return;
            var visible = !IsOccuped;
            vfxObject.SetActive(visible);

            col.enabled = !IsOccuped || IsLocalOccuped;
        }

        public override string GetName() => interactableName;
        public void RaiseOnInteractableSetState() => OnInteractableSetState?.Invoke();
        public void ClientStopActiveInteraction() => interactablesContainer.ClientStopActiveInteraction(ActiveInteraction);

        public void OnPlayerDisconnected(PlayerController playerController) => interactablesContainer.OnPlayerDisconected(playerController);

        private void OnValidate()
        {
            var childInteractions = GetComponentsInChildren<Interaction>();
            if (!childInteractions.IsNullOrEmpty()) Interactions = childInteractions.ToList();

            var container = GetComponentInParent<InteractablesContainer>();
            if (container) interactablesContainer = container;

            if (TryGetComponent(out Collider foundCollider)) col = foundCollider;
        }
    }
}