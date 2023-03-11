using System.Collections.Generic;
using System.Linq;
using Core.Runtime.Interactables;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Wonderland
{
    public class ActionsSequence
    {
        private List<Action> actions = new();
        public System.Action OnExecutionFinished;

        private List<Action> runningInBackground = new();
        private int currentActionIndex;
        private Action inProgressAction;
        private GameObject instigator;

        private bool IsLastAction => currentActionIndex >= actions.Count - 1;

        public ActionsSequence(List<Action> actions)
        {
            this.actions = actions;
        }

        public void ExecuteActionsList(GameObject instigator)
        {
            this.instigator = instigator;
            currentActionIndex = 0;

            var pc = instigator.GetComponent<PlayerController>();
            var actionsToPlay = actions.Where(x => !x.executeOnClientOnly || (x.executeOnClientOnly && pc.isLocalPlayer)).ToList();
            ExecuteActionsRecursive(actionsToPlay);
        }

        public void InterruptActions()
        {
            var actionsToCancel = runningInBackground.Where(x => x.cancelOnExitInteractable).ToList();
            if (inProgressAction != null) actionsToCancel.Add(inProgressAction);

            CancelActions(actionsToCancel);

            runningInBackground = new List<Action>();
            actions = new List<Action>();
            currentActionIndex = 0;
        }

        private void ExecuteActionsRecursive(List<Action> actionsList)
        {
            if (actionsList.IsNullOrEmpty() || currentActionIndex >= actionsList.Count) return;

            Action currentAction = actionsList[currentActionIndex];
            if (currentAction.blocksExecution)
            {
                if (!IsLastAction)
                {
                    currentAction.OnExecutionFinished = () => ExecuteActionsRecursive(actionsList);
                }
                else
                {
                    currentAction.OnExecutionFinished = () =>
                    {
                        // currentAction.CancelExecution(null);
                        this.OnExecutionFinished?.Invoke();
                    };
                }
            }

            inProgressAction = currentAction;
            currentAction.Execute(instigator);

            currentActionIndex++;

            if (!currentAction.blocksExecution)
            {
                runningInBackground.Add(currentAction);
                //If is non blocking, the recursion will continue right after we play the current, not with the on finish callback
                ExecuteActionsRecursive(actionsList);
            }
        }

        private void CancelActions(List<Action> actions)
        {
            if (actions.IsNullOrEmpty()) return; 
            foreach (Action action in actions)
            {
                action.OnExecutionFinished = null;
                action.CancelExecution(instigator);
            }
        }
    }
}