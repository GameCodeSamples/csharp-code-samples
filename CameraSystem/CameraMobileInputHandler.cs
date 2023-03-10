using Cinemachine;
using Wonderland.Services;
using Lean.Touch;
using UnityEngine;

namespace Wonderland
{
    public class CameraMobileInputHandler : MonoBehaviour
    {
        [SerializeField] private CameraData cameraData;
        private static TouchEventChannel TouchService => Context.AppContext.GetService<TouchEventChannel>();
        private CameraEventChannel cameraEventChannel;
        private CameraEventChannel CameraEventChannel
        {
            get
            {
                if (cameraEventChannel == null)
                {
                    cameraEventChannel = EventChannels.GetEventChannel<CameraEventChannel>();
                }
                return cameraEventChannel;
            }
        }

        private Vector2 dragDelta;
#if !UNITY_SERVER
        private void Start()
        {
            TouchService.OnPinchEvent += OnPinch;
            TouchService.OnDragWorldEvent += OnDrag;
            TouchService.OnDragCancelledEvent += DragCancelledEvent;
            //Overriding cinemachine mouse axis capturing
            CinemachineCore.GetInputAxis = GetDragAxisCustom;
        }

        private void OnDestroy()
        {
            TouchService.OnPinchEvent -= OnPinch;
            TouchService.OnDragWorldEvent -= OnDrag;
            TouchService.OnDragCancelledEvent -= DragCancelledEvent;
        }
#endif

        private float GetDragAxisCustom(string axisName)
        {
            return axisName switch
            {
                "Mouse X" => dragDelta.x,
                "Mouse Y" => dragDelta.y,
                _ => 0
            };
        }

        void DragCancelledEvent() 
        {
            dragDelta = Vector2.zero;
            CameraEventChannel.RaiseEndDragEvent();
        }


        private void OnPinch(float delta)
        {
            if (Mathf.Abs(delta) <= Mathf.Epsilon) return;

            CameraEventChannel.RaiseZoomEvent(LeanGesture.GetPinchScale());
        }

        private void OnDrag(Vector2 delta)
        {
            Vector2 filteredDrag;

            filteredDrag.x = FilterByThreshold(delta.x, cameraData.XRotationToStartThreshold);
            filteredDrag.y = FilterByThreshold(delta.y, cameraData.YRotationToStartThreshold);
            dragDelta = filteredDrag;
            CameraEventChannel.RaiseDragEvent(filteredDrag);
        }

        private static float FilterByThreshold(float delta, float threshold)
        {
            var absDelta = Mathf.Abs(delta);
            if (!(absDelta > threshold))
                return 0;

            return delta;
        }


    }
}