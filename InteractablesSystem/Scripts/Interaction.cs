using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Runtime.UIManager.Containers;
using Wonderland;
using Wonderland.Services;
using Wonderland.Social;
using NaughtyAttributes;
using NetworkEntities.WonderlandGame;
using SuperMaxim.Core.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Runtime.Interactables
{
    public class Interaction : MonoBehaviour

    {
        [field: BoxGroup("General Config"), SerializeField]
        public Sprite Icon { get; private set; }

        [field: BoxGroup("General Config"), SerializeField]
        public string Name { get; private set; }

        [BoxGroup("General Config"), SerializeField]
        private EntryPoint entryPoint;
        [field: BoxGroup("General Config"), SerializeField]
        public bool AllowOffset { get; private set; }= false;

        [field: BoxGroup("General Config"), SerializeField]
        public bool facePointForward { get; private set; }

        [BoxGroup("General Config"), SerializeField]
        private CommonInteractable parentInteractable;

        [BoxGroup("Logic"), SerializeField] private List<Condition> conditions;
        [BoxGroup("Logic"), SerializeField] private List<Action> onSuccessActions;
        [BoxGroup("Logic"), SerializeField] private InteractionCost cost;

        public InteractionCost Cost => cost;
        private GameObject instigator;
        private LocalPlayerInfo LocalPlayerInfo => Context.AppContext.GetService<LocalPlayerInfo>();

        public CommonInteractable ParentInteractable => parentInteractable;
        public bool Occupied => parentInteractable.IsOccuped;
        public EntryPoint EntryPoint => entryPoint;
        public bool HasCost => cost != null;
        private ActionsSequence actionsSequence;

        public void Play(GameObject instigatorGo)
        {
            instigator = instigatorGo;
            actionsSequence = new ActionsSequence(onSuccessActions);
            actionsSequence.ExecuteActionsList(instigatorGo);
        }

        public void Stop() => actionsSequence.InterruptActions();

        public async UniTask<bool> ExecuteConditions()
        {
            if (HasCost && !await CanAffordInteraction())
            {
                NotifyNotEnoughMoney(cost);
                return false;
            }

            foreach (Condition condition in conditions)
            {
                bool conditionsSucceeded = await condition.Evaluate();
                if (conditionsSucceeded) continue;

                var onFailSequence = new ActionsSequence(condition.OnFailActions);
                onFailSequence.ExecuteActionsList(instigator);
                return false;
            }

            return true;
        }

        private async UniTask<bool> CanAffordInteraction() => await HasEnoughCurrency(cost);

        private UniTask<bool> HasEnoughCurrency(InteractionCost cost)
        {
            return UniTask.FromResult(LocalPlayerInfo.GetAmountCurrency(Currency.Soft_Currency) >= cost.AmountCost);
        }
        
        private void NotifyNotEnoughMoney(InteractionCost interactionCost)
        {
            //TODO: backend call to fetch this user level;
            const string title = "You are broke!";
            const string text = "You need more money to interact";

            UIPopup popUp = UIPopup.Get("GenericErrorPopUp");
            popUp
                .SetTexts(title, text, "Confirm")
                .Show();
        }

        public void ShowInteractionCostPopUp(bool showTimer, UnityAction confirmAction, UnityAction cancelAction)
        {
            UIPopup popUp;
            if (showTimer)
            {
                popUp = UIPopup.Get(Constants.Popups.COST_TIMER);
                popUp.SetEvents(confirmAction, cancelAction, cancelAction)
                    .OnVisibleCallback.Event.AddListener(cancelAction);
            }
            else
            {
                popUp = UIPopup.Get(Constants.Popups.COST);
                popUp.SetEvents(confirmAction, cancelAction);
            }

            popUp
                //TODO: use the correct currency type
                .SetTexts("Confirmation", Name, Cost.AmountCost.ToString(), "Confirm", "Cancel")
                .Show();
        }

        private void OnValidate()
        {
            if (!Name.IsNullOrEmpty()) gameObject.name = Name;
        }
    }
}