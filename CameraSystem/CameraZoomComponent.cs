using System;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;

namespace Wonderland
{
    public class CameraZoomComponent : MonoBehaviour
    {
        [SerializeField] private CameraData cameraData;
        [ShowNonSerializedField] private float _currentZoomScale = 1;
        private float targetZoomScale = 1;

        private CameraEventChannel cameraEventChannel;

        private CinemachineFreeLook cinemachineFreeLook;
        private CinemachineFreeLook.Orbit[] originalOrbits;

        private float _zoomAxisValue;

        public void Initialize(CinemachineFreeLook freeLookCam, CameraInitialPosInfo cameraInitialPosInfo)
        {
            cinemachineFreeLook = freeLookCam;
            SetOriginalOrbits(cameraInitialPosInfo);

            cameraEventChannel = EventChannels.GetEventChannel<CameraEventChannel>();
            cameraEventChannel.ZoomEvent += OnZoom;
        }

        private void Update()
        {
            ZoomSmooth();
        }

        private void ZoomSmooth()
        {
            if (Math.Abs(_currentZoomScale - targetZoomScale) < Mathf.Epsilon) return;

            _currentZoomScale = Mathf.Lerp(_currentZoomScale, targetZoomScale, cameraData.ZoomDecelerationSpeed * Time.deltaTime);
            _currentZoomScale = Mathf.Clamp(_currentZoomScale, cameraData.MinZoomScale, cameraData.MaxZoomScale);

            AdjustRigsByZoomScale();
        }

        private void OnZoom(float value)
        {
            _zoomAxisValue = value;
            CalculateNewZoomScale();
        }

        private void CalculateNewZoomScale() => targetZoomScale = _currentZoomScale + _zoomAxisValue * cameraData.ZoomSense;

        private void AdjustRigsByZoomScale()
        {
            for (int i = 0; i < originalOrbits.Length; i++)
            {
                //modify rig radius and height
                cinemachineFreeLook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * _currentZoomScale;
                cinemachineFreeLook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * _currentZoomScale;
            }
        }

        private void SetOriginalOrbits(CameraInitialPosInfo initialPosInfo)
        {
            ChangeOrbits(ref cinemachineFreeLook.m_Orbits, initialPosInfo.orbits);

            originalOrbits = new CinemachineFreeLook.Orbit[cinemachineFreeLook.m_Orbits.Length];
            ChangeOrbits(ref originalOrbits, cinemachineFreeLook.m_Orbits);
        }

        private void ChangeOrbits(ref CinemachineFreeLook.Orbit[] orbitsToChange, CinemachineFreeLook.Orbit[] newOrbits)
        {
            for (int i = 0; i < orbitsToChange.Length; i++)
            {
                orbitsToChange[i].m_Height = newOrbits[i].m_Height;
                orbitsToChange[i].m_Radius = newOrbits[i].m_Radius;
            }
        }

        private void OnDestroy()
        {
            cameraEventChannel.ZoomEvent -= OnZoom;
        }
    }
}