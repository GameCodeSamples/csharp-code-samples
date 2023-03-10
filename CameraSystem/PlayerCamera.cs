using System;
using Cinemachine;
using UnityEngine;

namespace Wonderland
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook cinemachineFreeLook;
        [SerializeField] private CameraInitialPosInfo initialCameraPosition;
        [SerializeField] private LookAtData lookAtData;

        [SerializeField] private CameraDragComponent dragComp;
        [SerializeField] private CameraZoomComponent zoomComp;

        private LookAtTarget lookAtTarget;

        private void Start()
        {
            zoomComp.Initialize(cinemachineFreeLook, initialCameraPosition);
            dragComp.Initialize(cinemachineFreeLook);
        }

        public void Initialize()
        {
            UpdateLookAtTarget(PlayerController.localPlayer.Avatar.Skeleton);
            PlayerController.localPlayer.SkeletonChanged += UpdateLookAtTarget;
        }

        private void UpdateLookAtTarget(SkeletonComponent skeletonComponent)
        {
            if (lookAtTarget == null)
            {
                lookAtTarget = new GameObject("CameraLookAtTarget").AddComponent<LookAtTarget>();
                lookAtTarget.transform.SetParent(PlayerController.localPlayer.transform);
                lookAtTarget.Init(skeletonComponent, lookAtData);
                
                cinemachineFreeLook.LookAt = lookAtTarget.transform;
                cinemachineFreeLook.Follow = lookAtTarget.transform;
                
                ApplyInitialPos();
            }

            lookAtTarget.TargetSkeleton = skeletonComponent;
        }

        private void ApplyInitialPos()
        {
            cinemachineFreeLook.m_XAxis.Value = initialCameraPosition.initialCameraXValue;
            cinemachineFreeLook.m_YAxis.Value = initialCameraPosition.initialCameraYValue;
        }

        private void OnDestroy() => PlayerController.localPlayer.SkeletonChanged -= UpdateLookAtTarget;
    }
}