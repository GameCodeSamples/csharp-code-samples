using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Wonderland
{
    // This class represents a list of UI items that are pooled for better performance.
    // It uses generics to allow for different types of UIItems with its respective view models (data needed to update, usually structs).
    public class PooledItemsList<TView, TViewModel> where TView : UIItem<TViewModel>
    {
        private TView itemPrefab; // Renamed from animationItemPrefab for clarity
        private Transform parentTransform;
        private IObjectPool<TView> itemPool;
        private List<TView> activeItems = new List<TView>(); // Renamed from currentViews for clarity
        private int poolDefaultCapacity = 30;

        // Constructor that takes in the item prefab, parent transform, and optional pool default capacity.
        public PooledItemsList(TView itemPrefab, Transform parentTransform, int poolDefaultCapacity = 30)
        {
            this.itemPrefab = itemPrefab;
            this.parentTransform = parentTransform;
            this.poolDefaultCapacity = poolDefaultCapacity;
        }

        // Initializes the item pool with the specified default capacity.
        public void Initialize()
        {
            itemPool = new ObjectPool<TView>(CreatePooledItem, OnTakeFromPool, OnReturnToPool, defaultCapacity: poolDefaultCapacity);
        }
        
        // Repaints the UI items with the specified view models.
        // This method first releases all currently active items back into the pool,
        // then creates new UI items for each view model and adds them to the active items list.
        public void RepaintItems(List<TViewModel> viewModels)
        {
            // Release all currently active items back into the pool.
            foreach (TView item in activeItems)
            {
                itemPool.Release(item);
            }
            activeItems.Clear();

            // Create new UI items for each view model and add them to the active items list.
            foreach (TViewModel viewModel in viewModels)
            {
                TView item = itemPool.Get();
                item.Repaint(viewModel);
                item.transform.SetParent(parentTransform, false);
                activeItems.Add(item);
            }
        }

        // Called when an item is returned to the pool.
        // Deactivates the item's game object so it is not visible.
        private void OnReturnToPool(TView item)
        {
            item.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool.
        // Activates the item's game object and sets its transform to the last sibling so it appears on top.
        private void OnTakeFromPool(TView item)
        {
            item.transform.SetAsLastSibling();
            item.gameObject.SetActive(true);
        }

        // Creates a new item instance.
        private TView CreatePooledItem()
        {
            return Object.Instantiate(itemPrefab, parentTransform);
        }
    }
}
