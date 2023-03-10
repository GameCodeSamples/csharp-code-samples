using Doozy.Runtime.UIManager.Containers;
using Wonderland.Services;

namespace Wonderland
{
    public class RequestSwipe : RequestInput
    {
        private TouchEventChannel TouchEventChannel => Context.AppContext.GetService<TouchEventChannel>();
        protected override void SetPopUpCallbacks(UIPopup popup)
        {
            TouchEventChannel.OnSwipeUIEvent += OnSwipe;
            popup.OnHideCallback.Event.AddListener(() => { TouchEventChannel.OnSwipeUIEvent -= OnSwipe; });
        }

        private void OnSwipe(SwipeDirection swipe)
        {
            RaiseOnInputCompleted();
        }
    }
}