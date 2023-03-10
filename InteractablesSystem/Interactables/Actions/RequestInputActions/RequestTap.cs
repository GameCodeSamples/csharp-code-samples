using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;

namespace Wonderland
{
    public class RequestTap : RequestInput
    {
        protected override void SetPopUpCallbacks(UIPopup popup)
        {
            popup.Overlay.GetComponent<UIButton>().onClickEvent.AddListener(RaiseOnInputCompleted);
            popup.OnHideCallback.Event.AddListener(() => { popup.Overlay.GetComponent<UIButton>().onClickEvent.RemoveListener(RaiseOnInputCompleted); });
        }
    }
}