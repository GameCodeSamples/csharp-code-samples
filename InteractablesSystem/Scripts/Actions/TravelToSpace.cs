using System.Collections.Generic;
using Core.Runtime.Interactables;
using Cysharp.Threading.Tasks;
using Doozy.Runtime.UIManager.Containers;
using Wonderland.Loacalization;
using Wonderland.Services;
using UnityEngine;
using UnityEngine.Localization;

namespace Wonderland
{
    public class TravelToSpace : Action
    {
        [SerializeField] private string spaceId;
        [SerializeField] private string spaceName;

        [SerializeField] private bool showConfirmationPopUp = true;
        private TravelSystem TravelSystem => Context.AppContext.GetService<TravelSystem>();

        public override async void Execute(GameObject instigator)
        {
            if (showConfirmationPopUp)
            {
                var localizedString = new LocalizedString(
                    Tables.UITEXT, Entries.TRAVEL_CONFIRMATION_TITLE);

                var travelTitle = await localizedString.GetLocalizedStringAsync();
                
                
                localizedString = new LocalizedString(Tables.UITEXT, Entries.TRAVEL_CONFIRMATION_MSG);
                localizedString.Arguments = new object[]
                {
                    new Dictionary<string, string>() {{Params.SPACE_NAME, spaceName}}
                };
                
                var travelMessage = await localizedString.GetLocalizedStringAsync();

                var popUp = UIPopup.Get(Constants.Popups.CONFIRMATION);
                popUp.SetEvents(OnTravelConfirmed)
                    .SetTexts(travelTitle, travelMessage)
                    .Show();
            }
            else
            {
                OnTravelConfirmed();
            }
        }

        private void OnTravelConfirmed()
        {
            TravelSystem.TravelTo(spaceId);
            OnExecutionFinished?.Invoke();
        }

        private void OnValidate()
        {
            gameObject.name = "Travel To " + spaceId;
        }

        public override void CancelExecution(GameObject instigator)
        {
        }
    }
}