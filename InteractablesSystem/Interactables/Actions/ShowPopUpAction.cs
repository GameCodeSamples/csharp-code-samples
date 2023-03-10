using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using Action = Core.Runtime.Interactables.Action;

namespace Wonderland
{
    public class ShowPopUpAction : Action
    {
        [SerializeField] string popUpTitle;
        [SerializeField] string popUpMessage;

        [SerializeField] private float autoHideTime = 0f;

        private UIPopup popUp;

        public override void Execute(GameObject instigator)
        {
            popUp = UIPopup.Get("WonderlandPopUP");
            popUp.OnHiddenCallback.Event.AddListener(OnPopUpHidden);

            if (!string.IsNullOrEmpty(popUpTitle))
                popUp.SetTexts(popUpTitle, popUpMessage);
            else
                popUp.SetTexts(popUpMessage);

            if (autoHideTime > 0f)
            {
                popUp.AutoHideAfterShow = true;
                popUp.AutoHideAfterShowDelay = autoHideTime;
            }

            popUp.Show();
        }

        private void OnPopUpHidden() => OnExecutionFinished?.Invoke();

        public override void CancelExecution(GameObject instigator)
        {
            if (popUp)
                popUp?.Hide();
        }
    }
}