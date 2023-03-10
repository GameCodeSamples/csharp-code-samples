using NetworkEntities.WonderlandGame;
using UnityEngine;

namespace Core.Runtime.Interactables
{
    public class InteractionCost : MonoBehaviour
    {
        [SerializeField] private Currency currency;
        [SerializeField] private uint amountCost;

        public Currency Currency => currency;
        public uint AmountCost => amountCost;
    }
}