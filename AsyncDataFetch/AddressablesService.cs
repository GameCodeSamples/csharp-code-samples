/*
This script is a service class that provides methods for loading and instantiating assets from Addressable Assets, a Unity package that provides an alternative to the traditional asset loading system in Unity.

The methods included in the script are:

LoadAssetFromAddressables: Loads an asset with a specific ID and returns it as an AddressableInstance that contains the loaded asset and the handle used to load it.
InstantiateAssetFromAddressables: Instantiates an asset with a specific ID under a specified parent object.
LoadCatalog: Loads a content catalog file for a specific asset. Content catalogs are JSON files that contain information about the assets available for download from a remote server.
*/

using Cysharp.Threading.Tasks;
using Wonderland.Addressables;
using Wonderland.Networking;
using NetworkEntities.WonderlandGame;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UAddressables = UnityEngine.AddressableAssets.Addressables;

namespace Wonderland
{
    public class AddressablesService
    {
        private readonly List<string> catalogsLoaded = new List<string>();

        public async UniTask<AddressableInstance<GameObject>> LoadAssetFromAddressables(BodyCloth bodyCloth, CancellationToken cancellationToken)
        {
            UAddressables.ClearDependencyCacheAsync(bodyCloth.ClothId);
            AsyncOperationHandle<GameObject> currentLoadingAsset = UAddressables.LoadAssetAsync<GameObject>(bodyCloth.ClothId);
            if (currentLoadingAsset.Status == AsyncOperationStatus.Failed)
            {
                throw new ArgumentException($"Can't load, probably no exist addressable {bodyCloth.ClothId}");
            }
            // Debug.Log(bodyCloth.ClothId);
            await currentLoadingAsset.ToUniTask(cancellationToken: cancellationToken);
            return new AddressableInstance<GameObject>
            {
                addressableLoadedItem = currentLoadingAsset.Result,
                itemOperationHandle = currentLoadingAsset
            };
        }

        public async UniTask<AddressableInstance<GameObject>> LoadAssetFromAddressables(WonderlandAsset WonderlandAsset, CancellationToken cancellationToken)
        {
            UAddressables.ClearDependencyCacheAsync(WonderlandAsset.clothId);
            AsyncOperationHandle<GameObject> currentLoadingAsset = UAddressables.LoadAssetAsync<GameObject>(WonderlandAsset.clothId);
            if (currentLoadingAsset.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Can't load, probably no exist addressable {WonderlandAsset.clothId}");
                return default;
            }
            // Debug.Log(WonderlandAsset.ClothId);
            await currentLoadingAsset.ToUniTask(cancellationToken: cancellationToken);
            return new AddressableInstance<GameObject>
            {
                addressableLoadedItem = currentLoadingAsset.Result,
                itemOperationHandle = currentLoadingAsset
            };
        }

        public async Task<GameObject> InstantiateAssetFromAddressables(string addressableName, Transform parent)
        {
            AsyncOperationHandle<GameObject> currentLoadingAsset = UAddressables.InstantiateAsync(addressableName, parent, false);
            if (currentLoadingAsset.Status == AsyncOperationStatus.Failed)
            {
                throw new ArgumentException($"Can't load, probably no exist addressable {addressableName}");
            }
            // Debug.Log(cloth.ClothId);
            await currentLoadingAsset;
            return currentLoadingAsset.Result;
        }

        public async Task LoadCatalog(string assetId)
        {
            if (catalogsLoaded.Contains(assetId))
            {
                return;
            }

            string catalogFileName = $"catalog_{assetId}.json";
            string fullPath = $"{PathLocations.AssetsRemoteLoadPath}/{catalogFileName}";

            await UAddressables.LoadContentCatalogAsync(fullPath);
            catalogsLoaded.Add(assetId);
        }
    }
}
