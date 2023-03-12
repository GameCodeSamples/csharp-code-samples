using Doozy.Runtime.Signals;
using Wonderland;
using Wonderland.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Runtime.Interactables
{
    public class JoinInteractableChat : Action
    {
        [SerializeField] private string interactableChatID;
        [SerializeField] private string interactableChatName;
        private string chatId;

        private void Start()
        {
            chatId = $"interactable_{Context.AppContext.GetService<SpaceSelectorService>().CurrentSpaceInstanceID}_{interactableChatID}";
        }

        public override void Execute(GameObject instigator)
        {
            // Signal.Send<string>(StreamId.Chat.JoinInteractableChat, $"{chatId};{interactableChatName}");
        }

        public override void CancelExecution(GameObject instigator)
        {
            // Signal.Send<string>(StreamId.Chat.LeaveInteractableChat, chatId);
        }

        private void OnValidate() => blocksExecution = false;
    }
}
