using Doozy.Runtime.UIManager.Containers;
using Wonderland.Services;
using UnityEngine;

namespace Wonderland
{
    public class RequestDrag : RequestInput
    {
        private TouchEventChannel TouchEventChannel => Context.AppContext.GetService<TouchEventChannel>();
        protected override void SetPopUpCallbacks(UIPopup popup)
        {
            TouchEventChannel.OnDragUIEvent += OnDrag;
            popup.OnHideCallback.Event.AddListener(() => { TouchEventChannel.OnDragUIEvent -= OnDrag; });
        }

        private void OnDrag(Vector2 drag)
        {
            //TODO: check things about the drag for example the drag amount
            RaiseOnInputCompleted();
        }
    }
}