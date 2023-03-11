using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.UIManager.Containers;
using Wonderland;
using Mirror;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Interactables
{
    public class InteractablesContainer : NetworkBehaviour
    {
        [ReorderableList] [SerializeField] private CommonInteractable[] interactables;
        private readonly SyncList<int> OccupedInteractables = new();
        private Dictionary<CommonInteractable, PlayerController> FullOccupedInteractablesInfo { [Server] get; [Server] set; } = new();

        [Client]
        public override void OnStartClient()
        {
            if (!isServer)
                OccupedInteractables.Callback += OnOccupedInteractablesChanged;

            CmdSyncInteractablesInitialState();
        }

        [Client]
        public void ClientUseOccupable(CommonInteractable interactable, int interactionIdx, PlayerController playerController)
        {
            if (!interactable.IsOccuped || interactable.IsLocalOccuped)
            {
                int targetInteractableIndex = Array.IndexOf(interactables, interactable);
                CmdUseOccupable(targetInteractableIndex, interactionIdx, playerController);
            }
            else
            {
                UIPopup popUp = UIPopup.Get(Constants.Popups.ALERT);
                popUp.SetTexts("Interactable Occuped");
                popUp.Show();
            }
        }

        [Client]
        public void ClientLeaveOccupable(CommonInteractable interactable, PlayerController playerController)
        {
            int targetInteractable = Array.IndexOf(interactables, interactable);
            CmdLeaveOccupable(targetInteractable, playerController);
        }

        [Client]
        public void ClientStopActiveInteraction(Interaction interaction)
        {
            if (!interaction) return;
            CmdStopActiveInteraction(Array.IndexOf(interactables, interaction.ParentInteractable));
        }

        [Client]
        public CommonInteractable GetFirstFreeInteractable()
        {
            foreach (var current in interactables)
            {
                if (current.IsOccuped) continue;
                return current;
            }

            return interactables[0];
        }

        [Command(requiresAuthority = false, channel = Channels.Reliable)]
        private void CmdStopActiveInteraction(int interactableIdx)
        {
            RpcStopActiveInteraction(interactableIdx);
        }

        [Command(requiresAuthority = false, channel = Channels.Reliable)]
        private void CmdSyncInteractablesInitialState(NetworkConnectionToClient sender = null)
        {
            for (var i = 0; i < FullOccupedInteractablesInfo.Count; i++)
                TargetSyncInteractableInitialState(sender);
        }

        [Command(requiresAuthority = false, channel = Channels.Reliable)]
        private void CmdUseOccupable(int targetInteractableIndex, int targetInteractionIdx, PlayerController controller, NetworkConnectionToClient sender = null)
        {
            var targetInteractable = interactables[targetInteractableIndex];
            FullOccupedInteractablesInfo.TryGetValue(targetInteractable, out var currentOccupingPlayer);
            if (targetInteractable.IsOccuped && !currentOccupingPlayer == controller) return;

            if (!OccupedInteractables.Contains(targetInteractableIndex)) //In case the interactable is already occuped by the player
            {
                OccupedInteractables.Add(targetInteractableIndex);
                FullOccupedInteractablesInfo.Add(targetInteractable, controller);
                controller.OccupedInteractable = targetInteractable;
            }

            RpcUseOccupableMessage(targetInteractableIndex, targetInteractionIdx, controller);
        }

        [Command(requiresAuthority = false, channel = Channels.Reliable)]
        private void CmdLeaveOccupable(int targetInteractableIndex, PlayerController playerController, NetworkConnectionToClient sender = null)
        {
            var targetInteractable = interactables[targetInteractableIndex];
            ServerLeaveOccupable(targetInteractable, playerController);
        }

        [Server]
        public void ServerLeaveOccupable(CommonInteractable interactable, PlayerController playerController)
        {
            if (!FullOccupedInteractablesInfo.TryGetValue(interactable, out var occuping) || playerController != occuping) return;

            FullOccupedInteractablesInfo.Remove(interactable);
            var interactableIndex = Array.IndexOf(interactables, interactable);
            OccupedInteractables.Remove(interactableIndex);

            if (playerController)
            {
                playerController.OccupedInteractable = null;
                playerController.PlayerMovement.TargetClientSetAgentEnabled(true);
            }
            RpcLeaveOccupableMessage(interactableIndex);
        }

        [Server]
        public void OnPlayerDisconected(PlayerController controller)
        {
            if (!FullOccupedInteractablesInfo.ContainsValue(controller)) return;

            var interactable = FullOccupedInteractablesInfo.FirstOrDefault(interactable => interactable.Value == controller).Key;
            if (interactable)
                interactable.ServerLeaveOccupable(controller);
        }

        [ClientRpc(channel = Channels.Reliable)]
        private void RpcLeaveOccupableMessage(int targetInteractableIndex)
        {
            var targetInteractable = interactables[targetInteractableIndex];
            if (PlayerController.localPlayer.OccupedInteractable == targetInteractable) PlayerController.localPlayer.OccupedInteractable = null;

            if (isServer) //this is an special case for when testing using start host and client
            {
                targetInteractable.RaiseOnInteractableSetState();
                targetInteractable.UpdateInteractablePoint();
            }

            ClientStopActiveInteraction(targetInteractable.ActiveInteraction);
        }

        [ClientRpc(channel = Channels.Reliable)]
        private void RpcStopActiveInteraction(int targetInteractableIndex)
        {
            var interaction = interactables[targetInteractableIndex].ActiveInteraction;
            if (!interaction) return;
            interaction.Stop();
            interaction.ParentInteractable.ActiveInteraction = null;
        }

        [ClientRpc(channel = Channels.Reliable)]
        private void RpcUseOccupableMessage(int targetInteractableIdx, int targetInteractionIdx, PlayerController playerController)
        {
            var targetInteractable = interactables[targetInteractableIdx];
            var targetInteraction = targetInteractable.Interactions[targetInteractionIdx];
            targetInteractable.ActiveInteraction = targetInteraction;

            targetInteractable.UpdateInteractablePoint();
            if (playerController == PlayerController.localPlayer)
                PlayerController.localPlayer.OccupedInteractable = targetInteractable;

            targetInteraction.Play(playerController.gameObject);

            if (!isClientOnly)
            {
                targetInteractable.RaiseOnInteractableSetState();
                targetInteractable.UpdateInteractablePoint();
            }
        }

        [TargetRpc(channel = Channels.Reliable)]
        private void TargetSyncInteractableInitialState(NetworkConnection target)
        {
            foreach (var interactable in OccupedInteractables)
            {
                var current = interactables[interactable];
                current.UpdateInteractablePoint();
            }
        }

        [Client]
        private void OnOccupedInteractablesChanged(SyncList<int>.Operation op, int itemindex, int olditem, int newitem)
        {
            CommonInteractable interactable = default;
            if (op == SyncList<int>.Operation.OP_ADD)
            {
                interactable = interactables[newitem];
            }
            else if (op == SyncList<int>.Operation.OP_REMOVEAT)
            {
                interactable = interactables[olditem];
            }

            if (interactable == default) return;
            interactable.RaiseOnInteractableSetState();
            interactable.UpdateInteractablePoint();
        }

        public bool IsOccuped(CommonInteractable interactable)
        {
            int targetInteractableIndex = Array.IndexOf(interactables, interactable);
            return OccupedInteractables.Contains(targetInteractableIndex);
        }

        [Server]
        public override void OnStopServer()
        {
            foreach (var interactable in interactables)
            {
                if (!interactable) continue;

                if (FullOccupedInteractablesInfo.ContainsKey(interactable))
                    FullOccupedInteractablesInfo.Remove(interactable);
            }

            base.OnStopServer();
        }
    }
}