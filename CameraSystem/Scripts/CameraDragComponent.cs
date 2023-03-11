using Cinemachine;
using UnityEngine;

namespace Wonderland
{
    public class CameraDragComponent : MonoBehaviour
    {
        [SerializeField] private CameraData cameraData;

        private CinemachineFreeLook _cinemachineFreeLook;
        private bool _dragging;


        private CameraEventChannel cameraEventChannel;

        public void Initialize(CinemachineFreeLook freeLookCam)
        {
            _cinemachineFreeLook = freeLookCam;
            ApplySettingsToFreeLookCam();

            Subscribe();
        }

        public void SetCmXAxisSpeed(float value)
        {
            _cinemachineFreeLook.m_XAxis.m_MaxSpeed = value;
        }

        public void SetCmYAxisSpeed(float value)
        {
            _cinemachineFreeLook.m_YAxis.m_MaxSpeed = value;
        }

        public void SetCmDeceleration(float value)
        {
            _cinemachineFreeLook.m_XAxis.m_DecelTime = value;
            _cinemachineFreeLook.m_YAxis.m_DecelTime = value;
        }

        public void SetCmAcceleration(float value)
        {
            _cinemachineFreeLook.m_XAxis.m_AccelTime = value;
            _cinemachineFreeLook.m_YAxis.m_AccelTime = value;
        }

        public void InvertCmYAxis(bool inverted)
        {
            _cinemachineFreeLook.m_YAxis.m_InvertInput = inverted;
        }

        public void InvertCmXAxis(bool inverted)
        {
            _cinemachineFreeLook.m_XAxis.m_InvertInput = inverted;
        }

        private void ApplySettingsToFreeLookCam()
        {
            SetCmXAxisSpeed(cameraData.XAxisMaxSpeed);
            SetCmYAxisSpeed(CameraData.YAxisMaxSpeed);
            SetCmAcceleration(CameraData.AxisAcceleration);
            SetCmDeceleration(CameraData.AxisDeceleration);
            InvertCmXAxis(CameraData.XAxisInverted);
            InvertCmYAxis(CameraData.YAxisInverted);
        }

        public CameraData CameraData
        {
            get => cameraData;
            set => cameraData = value;
        }

        private void Subscribe()
        {
            cameraEventChannel = EventChannels.GetEventChannel<CameraEventChannel>();
            cameraData.OnCameraValuesUpdated += ApplySettingsToFreeLookCam;
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void UnSubscribe()
        {
            cameraData.OnCameraValuesUpdated += ApplySettingsToFreeLookCam;
        }
    }
}