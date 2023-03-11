using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Action = Core.Runtime.Interactables.Action;
using Random = UnityEngine.Random;

namespace Wonderland
{
    public class RandomAction : Action
    {
        [SerializeField]
        [ValidateInput("ValidateWeights", "Weights must sum 100 in total")]
        private List<WeightedActionList> actions;

        private ActionsSequence actionsSequence;

        public override void Execute(GameObject instigator)
        {
            actionsSequence = new ActionsSequence(GetRandomActions(actions));
            actionsSequence.OnExecutionFinished += OnExecutionFinished;

            actionsSequence.ExecuteActionsList(instigator);
        }

        private void OnExecutionFinished()
        {
            actionsSequence.OnExecutionFinished -= OnExecutionFinished;
            base.OnExecutionFinished?.Invoke();
        }

        public override void CancelExecution(GameObject instigator)
        {
            actionsSequence.InterruptActions();
        }

        List<Action> GetRandomActions(List<WeightedActionList> weightedValueList)
        {
            List<Action> output = new();

            //Getting a random weight value
            var totalWeight = weightedValueList.Sum(entry => entry.weight);
            var rndWeightValue = Random.Range(1, totalWeight + 1);

            //Checking where random weight value falls
            var processedWeight = 0;
            foreach (var entry in weightedValueList)
            {
                processedWeight += entry.weight;
                if (rndWeightValue > processedWeight) continue;
                output = entry.actions;
                break;
            }

            return output;
        }

        [ReferencedFromInspector]
        private bool ValidateWeights() => actions.Sum(x => x.weight) == 100;
    }

    [Serializable]
    public struct WeightedActionList
    {
        [MinValue(0), MaxValue(100)] public int weight;
        public List<Action> actions;
    }
}