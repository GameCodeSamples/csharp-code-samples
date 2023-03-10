using Core.Runtime.Interactables;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;

namespace Wonderland
{
    public abstract class RequestInput : Action
    {
        [SerializeField] protected string inputPopUpName = "WonderlandTapActionPopUp";
        private UIPopup popUpInstance;

        public override void Execute(GameObject instigator)
        {
            popUpInstance = UIPopup.Get(inputPopUpName);
            popUpInstance.Show();
            SetPopUpCallbacks(popUpInstance);
        }

        protected abstract void SetPopUpCallbacks(UIPopup popup);

        public override void CancelExecution(GameObject instigator)
        {
            if (popUpInstance) popUpInstance.Hide();
        }

        protected void RaiseOnInputCompleted() => OnExecutionFinished?.Invoke();
    }
}