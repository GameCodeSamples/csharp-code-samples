using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Interactables
{
    public abstract class Action : MonoBehaviour
    {
        [field: SerializeField] public bool blocksExecution { get; protected set; } = true;

        [Tooltip("Enable if we want this action to be executed only on the instigator client. It is useful for actions such as animations" +
                 "that are replicated by the animation system and there is no need to replicate it manually")]
        [field: SerializeField]
        public bool executeOnClientOnly { get; protected set; } = true;

        [field: HideIf(nameof(blocksExecution)), SerializeField]
        public bool cancelOnExitInteractable { get; protected set; } = true;

        public System.Action OnExecutionFinished;

        public abstract void Execute(GameObject instigator);
        public abstract void CancelExecution(GameObject instigator);
    }
}