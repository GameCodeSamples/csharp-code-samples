using Doozy.Runtime.UIManager.Containers;
using UnityEngine;

namespace Core.Runtime.Interactables
{
    public class NoItemPopUpAction : Action
    {
        //TODO: uncomment once the HasItemCondition is implemented
        // [SerializeField] private HasItemCondition condition;
        public override void Execute(GameObject instigator)
        {
            //TODO: uncomment once the HasItemCondition is implemented
            // NotifyNoItem(condition.itemId);
        }

        private void NotifyNoItem(string itemId)
        {
            //TODO: backend call to fetch this item info;
            const string title = "Buy an item";
            var text = "You need to get this item: " + itemId;
            var popUp = UIPopup.Get("GenericErrorPopUp");
            popUp
                .SetTexts(title, text, "Confirm")
                .Show();
        }

        public override void CancelExecution(GameObject instigator)
        {
        }
    }
}