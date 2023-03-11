using Cysharp.Threading.Tasks;
using Wonderland;
using Wonderland.Services;
using UnityEngine;

namespace Core.Runtime.Interactables
{
    public class HasItemCondition : Condition
    {
        [SerializeField] private string assetId;
        private Inventory Inventory => Context.AppContext.GetService<Inventory>();
        //In case we want to check if has an space, need to add another condition, because the CheckAssetExists is only for asset infos
        public override async UniTask<bool> Evaluate() => Inventory.CheckAssetExist(assetId);
    }
}