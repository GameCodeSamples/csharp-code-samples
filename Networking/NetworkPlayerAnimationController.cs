using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mirror;
using NetworkEntities.WonderlandGame;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UAddressable = UnityEngine.AddressableAssets.Addressables;

namespace Wonderland
{
    public class NetworkPlayerAnimationController : NetworkBehaviour
    {
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private SkeletonComponent skeletonComponent;
        [SerializeField] private AssetReferenceT<PlayerAnimation> defaultAnimationRef;

        [SyncVar] private AnimationInfo currentLocomotion;
        [SyncVar]
        private string currentAnimation;

        private PlayerAnimation defaultAnimation;
        private AnimatorOverrideController animatorOverrideController;
        private PlayerAnimationOverrider playerAnimationOverrider;
        private Animator animator;
        private MovementEventChannel movementEventChannel;
        private readonly int animatorSpeedParam = Animator.StringToHash("Speed");

        private SortedDictionary<Priority, PlayerAnimation> animations = new();
        private KeyValuePair<Priority, PlayerAnimation> TopPriorityAnimation => animations.Last();

        public event Action<string> OnAnimationCompletedEvent;

#if !UNITY_SERVER || UNITY_EDITOR
        [Client]
        private void Awake() => skeletonComponent.OnSkeletonChanged += Initialize;

        [Client]
        private void OnDestroy()
        {
            skeletonComponent.OnSkeletonChanged -= Initialize;
            if (defaultAnimation)
                UAddressable.Release(defaultAnimation);
            if (isLocalPlayer)
                movementEventChannel.OnTargetPosChangedEvent -= RemoveAllAnimations;
        }
#endif

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            movementEventChannel = EventChannels.GetEventChannel<MovementEventChannel>();
            movementEventChannel.OnTargetPosChangedEvent += RemoveAllAnimations;
            ClientClearSurfaceLocomotions();
        }

        [Client]
        private void Update()
        {
            if (animator && playerMovement) animator.SetFloat(animatorSpeedParam, playerMovement.currentVelocity);
        }

        [Client]
        private async void Initialize()
        {
            animator = skeletonComponent.Animator;
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            playerAnimationOverrider = new PlayerAnimationOverrider();
            playerAnimationOverrider.Initialize(animator, animatorOverrideController);

            if (isLocalPlayer)
            {
                var defaultAnim = await GetDefaultAnimation();
                animations.TryAdd(Priority.Default, defaultAnim);

                var lastCustomLocomotion = LocomotionMemory.GetLast();
                if (lastCustomLocomotion.AnimationId != string.Empty)
                {
                    var anim = await AnimationLoader.LoadAnimationAsync(lastCustomLocomotion.AnimationId);
                    animations.TryAdd(Priority.CustomLocomotion, anim);
                }

                PlayTopPriorityAnimation(false);
            }
            else
            {
                InitializeAnimationRemote();
            }
        }

        [Client]
        private static void ClientClearSurfaceLocomotions()
        {
            var lastLocomotion = LocomotionMemory.GetLast();
            if (lastLocomotion.Priority is Priority.SurfaceHigh or Priority.SurfaceLow)
            {
                LocomotionMemory.UpdateLast(new AnimationInfo(""));
            }
        }

        [Client]
        private async void InitializeAnimationRemote()
        {
            if (!string.IsNullOrEmpty(currentLocomotion.AnimationId))
            {
                ClientPlayLocomotion(currentLocomotion);
            }
            else
            {
                var defaultAnim = await GetDefaultAnimation();
                ClientPlayLocomotion(new AnimationInfo(defaultAnim.animationGUID, Priority.Default));
            }

            if (!string.IsNullOrEmpty(currentAnimation))
            {
                ClientPlayAnimation(currentAnimation);
            }
        }

        [Client]
        private void RemoveAllAnimations(Vector3 position)
        {
            ClientRemoveAnimationsOfPriority(new[] {Priority.Interactable, Priority.CustomAnimation});
            CmdRemoveCurrentAnimation();
            PlayTopPriorityAnimation(false);
        }

        [Client]
        private void OnGestureAnimationFinished(string animationName)
        {
            if (!isLocalPlayer) return;
            if (PlayerController.localPlayer.OccupedInteractable != null) return;
            ClientRemoveAnimationsOfPriority(new[] {Priority.CustomAnimation});
            CmdRemoveCurrentAnimation();
            PlayTopPriorityAnimation(false);
        }

        [Client]
        public void TryPlayAnimation(string animationGuid, Priority priority, bool stopMovement)
        {
            if (!isLocalPlayer) return;
            TryPlayAnimation(AnimationLoader.LoadAnimation(animationGuid), priority, stopMovement);
        }

        [Client]
        public void TryPlayAnimation(AssetReferenceT<PlayerAnimation> playerAnimation, Priority priority, bool stopMovement)
        {
            if (!isLocalPlayer) return;
            TryPlayAnimation(AnimationLoader.LoadAnimation(playerAnimation), priority, stopMovement);
        }

        [Client]
        private void TryPlayAnimation(PlayerAnimation animation, Priority priority, bool stopMovement)
        {
            if (priority == Priority.CustomLocomotion)
                animations.AddOrReplace(priority, animation); //By design, we always equip a custom locomotion(but it is not always instantly played

            if (!CanBePlayed(priority)) return;

            ClientRemoveAnimationsOfPriority(new[] {priority});
            animations.Add(priority, animation);

            PlayTopPriorityAnimation(stopMovement);
        }

        [Client]
        private bool CanBePlayed(Priority priority) => priority >= TopPriorityAnimation.Key;

        [Client]
        private void ClientRemoveAnimationsOfPriority(IEnumerable<Priority> priorities)
        {
            foreach (var priority in priorities)
            {
                if (priority != Priority.Default && animations.ContainsKey(priority))
                {
                    animations.Remove(priority);
                }
            }
        }

        [Client]
        private async void PlayTopPriorityAnimation(bool stopMovement)
        {
            if (!isLocalPlayer) return;

            if (!animations.ContainsKey(Priority.Default))
            {
                var defaultAnim = await GetDefaultAnimation();
                animations.Add(Priority.Default, defaultAnim);
            }

            if (TopPriorityAnimation.Value.AnimationType() == ItemType.Locomotion)
            {
                CmdPlayLocomotion(new AnimationInfo(TopPriorityAnimation.Value.animationGUID, TopPriorityAnimation.Key));
            }
            else
            {
                if (stopMovement)
                {
                    PlayerController.localPlayer.PlayerMovement.StopAllMovement(false);
                }

                CmdPlayAnimation(TopPriorityAnimation.Value.animationGUID);
            }
        }

        [Client]
        private void ClientPlayLocomotion(AnimationInfo animationInfo)
        {
            var animationToPlay = UAddressable.LoadAssetAsync<PlayerAnimation>(animationInfo.AnimationId).WaitForCompletion();
            playerAnimationOverrider?.PlayAnimation(animationToPlay, skeletonComponent.CurrentGender);
            if (isLocalPlayer)
            {
                LocomotionMemory.UpdateLast(animationInfo);
            }
        }

        [Client]
        public void RemoveAnimation(Priority priority)
        {
            ClientRemoveAnimationsOfPriority(new[] {priority});
            PlayTopPriorityAnimation(false);
        }

        [Client]
        private void ClientPlayAnimation(string animationGuid)
        {
            var animationToPlay = UAddressable.LoadAssetAsync<PlayerAnimation>(animationGuid).WaitForCompletion();
            playerAnimationOverrider?.PlayAnimation(animationToPlay, skeletonComponent.CurrentGender, true);
        }

        [Client]
        public void AnimationCompleted(string clipName)
        {
            //this is because loop animations first play a gesture and then a loop, an we want to ignore the entry gesture
            if (clipName == "initialRepetition") return;
            OnGestureAnimationFinished(clipName);
            OnAnimationCompletedEvent?.Invoke(clipName);
        }

        [Client]
        private UniTask<PlayerAnimation> GetDefaultAnimation()
        {
            return defaultAnimation == null ? AnimationLoader.LoadAnimationAsync(defaultAnimationRef) : UniTask.FromResult(defaultAnimation);
        }

        [Command]
        private void CmdPlayLocomotion(AnimationInfo animationInfo)
        {
            currentLocomotion = animationInfo;
            RpcPlayLocomotion(animationInfo);
        }

        [Command]
        private void CmdRemoveCurrentAnimation() => currentAnimation = string.Empty;

        [Command]
        private void CmdPlayAnimation(string animationGuid)
        {
            currentAnimation = animationGuid;
            RpcPlayAnimation(animationGuid);
        }

        [Server]
        public override void OnStopServer()
        {
            base.OnStopServer();
            ServerClearSurfaceLocomotions();
        }

        [Server]
        private void ServerClearSurfaceLocomotions()
        {
            if (currentLocomotion.Priority is Priority.SurfaceHigh or Priority.SurfaceLow)
            {
                currentLocomotion = new AnimationInfo("");
            }
        }

        [ClientRpc]
        private void RpcPlayAnimation(string animationGuid) => ClientPlayAnimation(animationGuid);

        [ClientRpc]
        private void RpcPlayLocomotion(AnimationInfo animationInfo) => ClientPlayLocomotion(animationInfo);
    }

    public readonly struct AnimationInfo : IEquatable<AnimationInfo>
    {
        public readonly string AnimationId;
        public readonly Priority Priority;

        public AnimationInfo(string animationId = "", Priority priority = default)
        {
            AnimationId = animationId;
            Priority = priority;
        }

        public bool Equals(AnimationInfo other)
        {
            return AnimationId == other.AnimationId && Priority == other.Priority;
        }
    }
}