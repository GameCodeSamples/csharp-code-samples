using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Wonderland
{
    //I attached this script to showcase the use of generics and the model-viewModel pattern that i use in UI system
    //For instance you can created a new pool of items for painting different types of list with different UIItems based on different view models
    //You can also apreciate the use of the object pool pattern(in this case i use the Unity built in pool but before it existed i used my own pool)

    public class PooledItemsList<TView, TViewModel> where TView : UIItem<TViewModel>
    {
        private TView animationItemPrefab;
        private Transform gridTransform;
        private IObjectPool<TView> viewsPool;
        private List<TView> currentViews = new();
        private int poolDefaultCapacity = 30;

        public PooledItemsList(TView animationItemPrefab, Transform gridTransform, int poolDefaultCapacity = 30)
        {
            this.animationItemPrefab = animationItemPrefab;
            this.gridTransform = gridTransform;
            this.poolDefaultCapacity = poolDefaultCapacity;
        }

        public void Initialize()
        {
            viewsPool = new ObjectPool<TView>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, defaultCapacity: poolDefaultCapacity);
        }
        
        public void RepaintItems(List<TViewModel> viewModelsList)
        {
            foreach (var view in currentViews) viewsPool.Release(view);
            currentViews.Clear();

            foreach (var item in viewModelsList)
            {
                viewsPool.Get(out TView itemUI);
                itemUI.Repaint(item);
                currentViews.Add(itemUI);
            }
        }
        private void OnReturnedToPool(TView obj) => obj.gameObject.SetActive(false);

        private void OnTakeFromPool(TView obj)
        {
            obj.transform.SetAsLastSibling();
            obj.gameObject.SetActive(true);
        }

        private TView CreatePooledItem() => UnityEngine.Object.Instantiate(animationItemPrefab, gridTransform);
    }
}
