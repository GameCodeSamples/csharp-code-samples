using System;
using Mirror;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Action = Core.Runtime.Interactables.Action;

namespace Wonderland
{
    public class PlayVFX : Action
    {
        [SerializeField] private Transform spawnTransform;
        [SerializeField] private string objectAddress;

        private AsyncOperationHandle<GameObject> _asyncOperationHandle;

        private PlayerController _playerController;

        public override void Execute(GameObject instigator)
        {
            if (!instigator.TryGetComponent(out PlayerController pc)) return;

            _playerController = pc;
            SpawnVFX(pc);
        }

        public override void CancelExecution(GameObject instigator) => UnSpawnVFX();

        private async void SpawnVFX(PlayerController playerController)
        { 
            _asyncOperationHandle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(objectAddress);
            var objectToSpawn = await _asyncOperationHandle.Task;
            
            SceneManager.SetActiveScene(gameObject.scene); //the interactable is in the logic scene
            Instantiate(objectToSpawn, spawnTransform.position, spawnTransform.rotation);
        }
        private void UnSpawnVFX()
        {
            UnityEngine.AddressableAssets.Addressables.Release(_asyncOperationHandle);
        }

        private void OnValidate()
        {
            // For now we don't have a way for the server communicating when instantiated object is unspawned, and design doesn't require it
            blocksExecution = false;
            cancelOnExitInteractable = false;
            executeOnClientOnly = false;
        }
    }
}