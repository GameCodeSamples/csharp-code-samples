using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Runtime.Interactables
{
    public abstract class Condition : MonoBehaviour
    {
        //TODO: should be able to access a blackboard or global variable to access the player items, player coins, etc to evaluate the different conditions
        [SerializeField] private List<Action> onFailActions;
        public abstract UniTask<bool> Evaluate();
        public List<Action> OnFailActions => onFailActions;
    }
}